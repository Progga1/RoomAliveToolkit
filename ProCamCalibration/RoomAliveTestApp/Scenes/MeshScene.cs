using SharpDX;
using SharpGraphics;

namespace RoomAliveTestApps.Scenes {

	class MeshScene : VirtualSceneBase {

		RoomMesh rMesh;
		private CameraControl cameraControl = new CameraControl();

		protected override void PostInit() {
			rMesh = loadObj("Assets/FloorPlan.obj");

			graphics.setDepthEnabled(true);
		}

		public override void DrawContent(SharpDX.Matrix mvpMat) {
			base.DrawContent(mvpMat);
			base.setMVP(mvpMat);

			if(false) {
				rMesh.meshShader.SetVertexShaderConstants(context,SharpDX.Matrix.Identity,mvp,pointLight.position);
				rMesh.meshShader.Render(context,rMesh.meshDeviceResources,pointLight,null,null,surface.viewport);
			} else {
				graphics.drawMesh(rMesh,mvp,new FloatColor(0.9f),new Vector3(0,1,0));
			}
		}

		public override void OnDraw() {
			surface.setPerspectiveProjection(1.2f,0.01f,10);
			
			DrawContent(cameraControl.getViewMatrix()*surface.getProjectionMatrix());
		}

		public override void RawEvent(InputEvent ev) {
			base.RawEvent(ev);
			ev.handle(cameraControl);
		}

	}

}
