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
		private ProjectorCameraEnsemble ensemble;

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
			ensemble = RoomAliveToolkit.ProjectorCameraEnsemble.FromFile("Assets/calibration/calibration4.xml");

			new Thread(new ThreadStart(() => {
				outsideViewScene = new OutsideViewScene(ensemble);
				outsideSurface = new RenderSurface(outsideViewScene.RenderCallback);
				outsideSurface.initWindowed("Outside view",800,600);
				outsideSurface.setInputCallback(outsideViewScene);
				outsideViewScene.Init(outsideSurface);

				outsideSurface.run();
			})).Start();

		}

		

		public new void Dispose() {
			base.Dispose();
			throw new NotImplementedException();
		}

	}

}
