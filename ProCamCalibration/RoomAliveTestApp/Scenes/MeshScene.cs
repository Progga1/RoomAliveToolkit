using RoomAliveToolkit;
using SharpGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomAliveTestApp.Scenes {

	class MeshScene : Scene {

		Mesh mesh;
		private CameraControl cameraControl = new CameraControl();

		protected override void PostInit() {
			mesh = new Mesh();
			surface.setDepthEnabled(true);
		}

		public override void RawEvent(InputEvent ev) {
			base.RawEvent(ev);
			ev.handle(cameraControl);
		}

	}

}
