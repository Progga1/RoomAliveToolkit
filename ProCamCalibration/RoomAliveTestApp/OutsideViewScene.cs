using RoomAliveTestApp;
using RoomAliveTestApp.Shaders;
using RoomAliveToolkit;
using SharpDX;
using SharpDX.Direct3D;
using SharpGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoomMatrix = RoomAliveToolkit.Matrix;
using SharpMatrix = SharpDX.Matrix;

namespace ProjectionMappingApp {

	class OutsideViewScene : Scene {

		RoomAliveScene roomScene;
		ProjectorCameraEnsemble ensemble;

		private SingleColorShader singleColorShader;

		CameraControl cameraControl = new CameraControl();
		RoomMesh rMesh;

		private SharpMatrix worldToNorm = new SharpMatrix();
		private SharpMatrix normToWorld = new SharpMatrix();
		private SharpMatrix projection = new SharpMatrix();
		private Vector3[] points = new Vector3[8];

		public OutsideViewScene(RoomAliveScene roomScene) {
			this.roomScene = roomScene;
			this.ensemble = roomScene.ensemble;
		}

		protected override void PostInit() {
			singleColorShader = new SingleColorShader(device);
			rMesh = new RoomMesh(device).setMesh(roomScene.roomMesh);
			cameraControl.distance = 5;
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
			surface.setOrthographicProjection(cameraControl.distance,-1,100);
			surface.setPerspectiveProjection(1.2f,0.01f,30);

			SharpMatrix projMat = surface.getProjectionMatrix();
			SharpMatrix viewMat = cameraControl.getViewMatrix();
			setMVP(viewMat*projMat);

			rMesh.meshShader.SetVertexShaderConstants(context,SharpMatrix.Identity,mvp,pointLight.position);
			rMesh.meshShader.Render(context,rMesh.meshDeviceResources,pointLight,null,null,surface.viewport);

			viewMat = cameraControl.getViewMatrix();
			projMat = surface.getProjectionMatrix();
			foreach(var projector in ensemble.projectors) {
				drawViewFrustum(projector.pose,projector.cameraMatrix,new FloatColor(0.5f,0.4f,0.05f),projector.width,projector.height,0.1f,3.0f);
			}

			foreach(var camera in ensemble.cameras) {
				drawViewFrustum(camera.pose,camera.calibration.depthCameraMatrix,new FloatColor(0.2f,0.2f,0.6f),512,424,0.5f,4.5f);
			}

			drawViewFrustum(roomScene.headPose,roomScene.headProjection,new FloatColor(0.7f,0.1f,0.05f));

			if(false) {
				singleColorShader.activate();
				singleColorShader.updateVSConstantBuffer(mvpTransp);
				singleColorShader.passColor(0,1,0,1);
				graphics.putPos(1,1,1);
				graphics.putPos(-1,0.5f,1);
				graphics.putIndex(0);
				graphics.putIndex(1);
				context.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
				graphics.flush();
			}
		}

		public override void RawEvent(InputEvent ev) {
			base.RawEvent(ev);
			ev.handle(cameraControl);
		}

		public override void Dispose() {
			singleColorShader.Dispose();
		}

	}

}
