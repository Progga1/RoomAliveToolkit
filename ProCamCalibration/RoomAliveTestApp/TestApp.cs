using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;
using SharpDX.Windows;
using SharpDX.DXGI;
using D3D11 = SharpDX.Direct3D11;
using D3DDevice = SharpDX.Direct3D11.Device;
using D3DDeviceContext = SharpDX.Direct3D11.DeviceContext;
using SharpDX.Direct3D;
using SharpGraphics;

using RoomAliveToolkit;
using System.Runtime.InteropServices;
using RoomAliveTestApp.Shaders;
using System.Threading;
using RoomAliveTestApp.Scenes;

namespace RoomAliveTestApp {

	class TestApp : ApplicationContext, IDisposable, InputCallbacks {

		RenderSurface mainSurface;
		RenderSurface quadSurface;

		private Scene triangleScene;
		private Scene quadScene;

		//RoomAlive objects
		private ProjectorCameraEnsemble ensemble;


		[STAThread]
		static int Main(string[] args) {
			using(TestApp app = new TestApp()) {
				Application.Run(app);
			}

			return 0;
		}

		public TestApp() {

			ensemble = RoomAliveToolkit.ProjectorCameraEnsemble.FromFile("C:/Users/Progga/Documents/Visual Studio 2015/Projects/RoomAliveToolkit/calibration/single_projector4/calibration4.xml");

			triangleScene = new TriangleScene();
			quadScene = new QuadScene();

			new Thread(new ThreadStart(() => {
				mainSurface = new RenderSurface(RenderCallback);
				mainSurface.setInputCallback(this);
				mainSurface.initWindowed("Triangle",640,480);

				triangleScene.Init(mainSurface);

				mainSurface.run();
			})).Start();

			new Thread(new ThreadStart(() => {
				quadSurface = new RenderSurface(RenderCallback);
				quadSurface.setInputCallback(this);
				quadSurface.clearColor = new Color(45,45,45);
				quadSurface.initWindowed("Quad",640,480);

				quadScene.Init(quadSurface);

				quadSurface.run();
			})).Start();


		}

		private void RenderCallback(D3DDeviceContext context,RenderSurface sender) {
			if(sender==mainSurface) {
				triangleScene.OnDraw();
			}
			if(sender==quadSurface) {
				quadScene.OnDraw();
			}
		}

		public new void Dispose() {
			mainSurface.Dispose();
			quadSurface.Dispose();
			triangleScene.Dispose();
			quadScene.Dispose();
		}

		public void RawEvent(InputEvent ev) {
			if(ev.sender==mainSurface) {
				ev.handle(triangleScene);
			}
			if(ev.sender==quadSurface) {
				ev.handle(quadScene);
			}
		}

		public void MouseDown(NormMouseEvent mouseEvent) {

		}

		public void MouseMove(NormMouseEvent mouseEvent) {
			
		}

		public void MouseDrag(NormMouseEvent mouseEvent) {

		}

		public void MouseUp(NormMouseEvent mouseEvent) {

		}

		public void MouseWheel(MouseWheelEvent ev) {

		}

		public void KeyDown(KeyEvent ev) {

		}

		public void KeyUp(KeyEvent ev) {

		}
	}
}
