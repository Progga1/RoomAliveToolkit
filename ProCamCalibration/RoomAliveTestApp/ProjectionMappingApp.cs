using RoomAliveTestApp;
using RoomAliveToolkit;
using SharpGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectionMappingApp {

	class ProjectionMappingApp : ApplicationContext,IDisposable {

		//RoomAlive objects
		private RoomAliveScene roomScene;

		private RenderSurface outsideSurface;
		private OutsideViewScene outsideViewScene;

		[STAThread]
		static int Main(string[] args) {
			using(ProjectionMappingApp app = new ProjectionMappingApp()) {
				Application.Run(app);
			}

			return 0;
		}

		ProjectionMappingApp() {
			roomScene = new RoomAliveScene("Assets/calibration/calibration4.xml","Assets/calibration/desktop_scene4_verylow.obj");

			new Thread(new ThreadStart(() => {
				outsideViewScene = new OutsideViewScene(roomScene);
				outsideSurface = new RenderSurface(outsideViewScene.RenderCallback);
				outsideSurface.initWindowed("Outside view",800,600);
				outsideViewScene.Init(outsideSurface);

				outsideSurface.setInputCallback(outsideViewScene);
				outsideSurface.run();
			})).Start();

		}

		

		public new void Dispose() {
			base.Dispose();
			throw new NotImplementedException();
		}

	}

}
