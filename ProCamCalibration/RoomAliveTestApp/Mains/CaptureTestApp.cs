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
using System.Threading;
using RoomAliveTestApps.Scenes;

namespace RoomAliveTestApps {

	class CaptureTestApp : ApplicationContext, IDisposable, InputCallbacks {

		RenderSurface surface;
		private VirtualSceneBase capScene;


		[STAThread]
		static int Main(string[] args) {
			using(CaptureTestApp app = new CaptureTestApp()) {
				Application.Run(app);
			}

			return 0;
		}

		public CaptureTestApp() {

			new Thread(new ThreadStart(() => {
                capScene = new CaptureScene();
                surface = new RenderSurface(capScene.RenderCallback);
				surface.setInputCallback(capScene);
				surface.initWindowed("Triangle",640,480);
				
				capScene.Init(surface);

				surface.run();
			})).Start();

		}

		private void RenderCallback(D3DDeviceContext context,RenderSurface sender) {
			if(sender==surface)
				capScene.OnDraw();
        }

		public new void Dispose() {
			base.Dispose();
			surface.Dispose();
			capScene.Dispose();
		}

		public void RawEvent(InputEvent ev) {
			if(ev.sender==surface)
				ev.handle(capScene);
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
