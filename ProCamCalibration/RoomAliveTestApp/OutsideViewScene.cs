using RoomAliveTestApp;
using RoomAliveToolkit;
using SharpDX.Direct3D;
using SharpGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectionMappingApp {

	class OutsideViewScene : Scene {

		ProjectorCameraEnsemble ensemble;

		CameraControl cameraControl = new CameraControl();
		RoomMesh rMesh;

		public OutsideViewScene(ProjectorCameraEnsemble ensemble) {
			this.ensemble = ensemble;
		}

		protected override void PostInit() {
			rMesh = loadObj("Assets/calibration/desktop_room4.obj",true);
			cameraControl.distance = 5;
		}

		public override void OnDraw() {
			base.OnDraw();
			surface.setOrthographicProjection(cameraControl.distance,-1,100);
			surface.setPerspectiveProjection(1.2f,0.01f,30);

			SharpDX.Matrix projMat = surface.getProjectionMatrix();
			SharpDX.Matrix viewMat = cameraControl.getViewMatrix();
			rMesh.meshShader.SetVertexShaderConstants(context,SharpDX.Matrix.Identity,viewMat*projMat,pointLight.position);
			rMesh.meshShader.Render(context,rMesh.meshDeviceResources,pointLight,null,null,surface.viewport);

			graphics.putPos(1,1,1);
			graphics.putPos(-1,0.5f,1);
			graphics.putIndex(0);
			graphics.putIndex(1);
			context.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
			graphics.flush();
		}

		public override void RawEvent(InputEvent ev) {
			base.RawEvent(ev);
			ev.handle(cameraControl);
		}

	}

}
