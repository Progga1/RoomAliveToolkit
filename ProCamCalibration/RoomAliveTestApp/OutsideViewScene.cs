using RoomAliveToolkit;
using SharpGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectionMappingApp {

	class OutsideViewScene : Scene {

		ProjectorCameraEnsemble ensemble;

		public OutsideViewScene(ProjectorCameraEnsemble ensemble) {
			this.ensemble = ensemble;
		}

		protected override void PostInit() {
		
		}

		public override void OnDraw() {
			base.OnDraw();

		}

	}

}
