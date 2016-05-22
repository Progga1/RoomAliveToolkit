using RoomAliveTestApp;
using RoomAliveTestApp.Scenes;
using RoomAliveToolkit;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpGraphics;
using SharpGraphics.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RoomMatrix = RoomAliveToolkit.Matrix;
using SharpMatrix = SharpDX.Matrix;

namespace ProjectionMappingApp {

	class OutsideViewScene : VirtualSceneBase {

		RoomAliveScene roomScene;
		VirtualSceneBase virtualScene;

		RoomAliveScene.Head head;
		RoomAliveScene.HeadViewRendering headRendering;
		ProjectorCameraEnsemble ensemble;

		private ProjectorForm projectorForm;
		private ProjectionMappingShader projMapShader;

		SharpTexture cubeTex;

		protected SharpMatrix vSceneWorldMat;

		CameraControl cameraControl = new CameraControl();
		RoomMesh rMesh;

		private SharpMatrix worldToNorm = new SharpMatrix();
		private SharpMatrix normToWorld = new SharpMatrix();
		private SharpMatrix projection = new SharpMatrix();
		private Vector3[] points = new Vector3[8];

		public OutsideViewScene(RoomAliveScene roomScene) {
			this.roomScene = roomScene;
			this.head = roomScene.head;
			this.ensemble = roomScene.ensemble;
		}

		protected override void PostInit() {
			rMesh = new RoomMesh(device).setMesh(roomScene.roomMesh);
			cameraControl.distance = 4;
			cameraControl.position.Z = 2.4f;
			cameraControl.alpha = PI;

			virtualScene = new QuadScene();
			virtualScene.Init(surface);
			headRendering = new RoomAliveScene.HeadViewRendering(roomScene,surface);

			cubeTex = new SharpTexture(device,"Assets/cube.png");

			//--Init-projector--
			var factory = new Factory1();
			projectorForm = new ProjectorForm(factory,device,new object(),ensemble.projectors[0]);
			projectorForm.FullScreen = false; // TODO: fix this so can be called after Show
			projectorForm.ClientSize = new System.Drawing.Size(640,480);
			projectorForm.Show();

			projMapShader = new ProjectionMappingShader(device);
		}

		private void drawViewFrustum(SharpMatrix worldTransform,SharpMatrix projection,FloatColor color) {
			
			worldToNorm = projection* worldTransform;
			normToWorld = worldToNorm;
			normToWorld.Invert();

			points[0] = MatrixOps.applyMat(normToWorld,-1,1,0);
			points[1] = MatrixOps.applyMat(normToWorld,1,1,0);
			points[2] = MatrixOps.applyMat(normToWorld,-1,-1,0);
			points[3] = MatrixOps.applyMat(normToWorld,1,-1,0);

			points[4] = MatrixOps.applyMat(normToWorld,-1,1,1);
			points[5] = MatrixOps.applyMat(normToWorld,1,1,1);
			points[6] = MatrixOps.applyMat(normToWorld,-1,-1,1);
			points[7] = MatrixOps.applyMat(normToWorld,1,-1,1);

			graphics.putCubeLineIndices();
			graphics.putPos(points);

			singleColorShader.activate();
			singleColorShader.passColor(color);
			singleColorShader.updateVSConstantBuffer(mvpTransp);
			context.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
			graphics.flush();
		}

		private void drawViewFrustum(RoomMatrix pose,RoomMatrix intrinsics,FloatColor color,int imgWidth,int imgHeight,float near,float far) {
			SharpMatrix extrinsics = MatrixOps.getSharpMatrix(pose);
			extrinsics.Invert();
			float fx = (float)intrinsics[0,0];
			float fy = (float)intrinsics[1,1];
			float cx = (float)intrinsics[0,2];
			float cy = (float)intrinsics[1,2];
			projection = GraphicsTransforms.ProjectionMatrixFromCameraMatrix(fx,fy,cx,cy,imgWidth,imgHeight,near,far);

			drawViewFrustum(extrinsics,projection,color);
		}

		public override void OnDraw() {
			base.OnDraw();
			graphics.setDepthEnabled(true);
			context.Rasterizer.State = graphics.defaultRasterizerState;

			surface.setOrthographicProjection(cameraControl.distance,-1,100);
			surface.setPerspectiveProjection(1.2f,0.01f,30);

			SharpMatrix projMat = surface.getProjectionMatrix();
			SharpMatrix viewMat = cameraControl.getViewMatrix();
			setMVP(viewMat*projMat);

			//rMesh.meshShader.SetVertexShaderConstants(context,SharpMatrix.Identity,mvp,pointLight.position);
			//rMesh.meshShader.Render(context,rMesh.meshDeviceResources,pointLight,null,null,surface.viewport);

			if(false) {
				singleColorShader.activate();
				singleColorShader.passColor(new FloatColor(1,0.5f,0,1));
				singleColorShader.updateVSConstantBuffer(mvpTransp);
				graphics.putPositionsMesh(rMesh.mesh);
				graphics.flush();
			}

			//--Draw-frustums--
			viewMat = cameraControl.getViewMatrix();
			projMat = surface.getProjectionMatrix();
			foreach(var projector in ensemble.projectors) {
				drawViewFrustum(projector.pose,projector.cameraMatrix,new FloatColor(0.5f,0.4f,0.05f),projector.width,projector.height,0.1f,3.0f);
			}

			foreach(var camera in ensemble.cameras) {
				drawViewFrustum(camera.pose,camera.calibration.depthCameraMatrix,new FloatColor(0.2f,0.2f,0.6f),512,424,0.5f,4.5f);
			}
			drawViewFrustum(head.getViewTransp(),head.getProjectionTransp(),new FloatColor(0.7f,0.1f,0.05f));

			vSceneWorldMat = SharpMatrix.RotationY(PI) * SharpMatrix.Scaling(0.6f,1.2f,0.6f) * SharpMatrix.Translation(0,0,2.5f);

			SharpMatrix vMVP = vSceneWorldMat * mvp;

			virtualScene.DrawContent(vMVP);

			headRendering.beginRendering();
			vMVP = vSceneWorldMat * head.getView() * head.projMat;
			virtualScene.DrawContent(vMVP);
			headRendering.endRendering();
	
			graphics.setDepthEnabled(false);
			surface.setOrthographicProjection();
			projMat = surface.getProjectionMatrix();
			projMat.Transpose();
			posUVColorShader.activate();
			posUVColorShader.updateVSConstantBuffer(projMat);
			posUVColorShader.passColor(1,1,1,1);
			context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
			graphics.bindTexture(headRendering.texture);
			float s = 0.75f;
			graphics.putRectPosUVColor(-surface.ratioX,-1+s,s*headRendering.getRatioX(),s, new Vector4(1,1,1,0.5f));
			graphics.flush();

			//--Projector-rendering--
			context.ClearRenderTargetView(projectorForm.renderTargetView,Color4.Black);
			context.ClearDepthStencilView(projectorForm.depthStencilView,DepthStencilClearFlags.Depth,1,0);
			context.OutputMerger.SetRenderTargets(projectorForm.depthStencilView,projectorForm.renderTargetView);
			context.Rasterizer.SetViewport(projectorForm.viewport);

			context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

			projMapShader.activate();
			projMapShader.passTransformations(SharpMatrix.Identity,projectorForm.view * projectorForm.projection,head.getView(),head.getProjection());
			projMapShader.passColor(0.8f,0.8f,1,1);
			graphics.bindTexture(headRendering.texture);

//			setMVP(m*vp);
//			singleColorShader.activate();
//			singleColorShader.passColor(new FloatColor(1,0,1,1));
//			context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
//			singleColorShader.updateVSConstantBuffer(mvpTransp);
			graphics.putPositionsMesh(rMesh.mesh);
			graphics.putColor(FloatColor.White,rMesh.mesh.vertices.Count);
			graphics.flush();

			projectorForm.swapChain.Present(0,PresentFlags.None);

			surface.restoreRenderTargets();
		}

		public override void RawEvent(InputEvent ev) {
			base.RawEvent(ev);
			ev.handle(cameraControl);
		}

		public override void Dispose() {
			virtualScene.Dispose();
		}

		public override void KeyDown(KeyEvent ev) {
			base.KeyDown(ev);
			const float HEAD_STEP = 0.1f;
			//Rotate 180� -> inv X
			if(ev.code==Keys.D)
				head.position.X -= HEAD_STEP;
			if(ev.code==Keys.A)
				head.position.X += HEAD_STEP;
			if(ev.code==Keys.W)
				head.position.Y += HEAD_STEP;
			if(ev.code==Keys.S)
				head.position.Y -= HEAD_STEP;
		}

	}

}
