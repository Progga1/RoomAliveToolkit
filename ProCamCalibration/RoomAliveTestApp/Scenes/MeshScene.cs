using RoomAliveToolkit;
using SharpDX;
using SharpDX.WIC;
using SharpGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomAliveTestApp.Scenes {

	class MeshScene : Scene {

		RoomMesh rMesh;
		private CameraControl cameraControl = new CameraControl();

		protected override void PostInit() {
			rMesh = loadObj("Assets/FloorPlan.obj");

			surface.setDepthEnabled(true);
		}

		public override void OnDraw() {

			surface.setOrthographicProjection(cameraControl.distance,-1,100);
			surface.setPerspectiveProjection(1.2f,0.01f,10);

			SharpDX.Matrix projMat = surface.getProjectionMatrix();
			SharpDX.Matrix viewMat = cameraControl.getViewMatrix();
			rMesh.meshShader.SetVertexShaderConstants(context,SharpDX.Matrix.Identity,viewMat*projMat,pointLight.position);
			rMesh.meshShader.Render(context,rMesh.meshDeviceResources,pointLight,null,null,surface.viewport);
		}

		public override void RawEvent(InputEvent ev) {
			base.RawEvent(ev);
			ev.handle(cameraControl);
		}

	}

}
