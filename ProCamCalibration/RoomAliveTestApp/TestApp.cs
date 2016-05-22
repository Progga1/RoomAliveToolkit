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
using RoomAliveTestApp.Scenes;

namespace RoomAliveTestApp {

	class TestApp : ApplicationContext, IDisposable, InputCallbacks {

		RenderSurface triangleSurface;
		RenderSurface quadSurface;
		RenderSurface meshSurface;
		RenderSurface textureSurface;

		private VirtualSceneBase triangleScene;
		private VirtualSceneBase quadScene;
		private VirtualSceneBase meshScene;
		private VirtualSceneBase textureScene;


		[STAThread]
		static int Main(string[] args) {
			using(TestApp app = new TestApp()) {
				Application.Run(app);
			}

			return 0;
		}

		public TestApp() {

			new Thread(new ThreadStart(() => {
				triangleSurface = new RenderSurface(RenderCallback);
				triangleSurface.setInputCallback(this);
				triangleSurface.initWindowed("Triangle",640,480);

				triangleScene = new TriangleScene();
				triangleScene.Init(triangleSurface);

				triangleSurface.run();
			})).Start();

			new Thread(new ThreadStart(() => {
				quadSurface = new RenderSurface(RenderCallback);
				quadSurface.setInputCallback(this);
				quadSurface.clearColor = new Color(45,45,45);
				quadSurface.initWindowed("Quad",640,480);

				quadScene = new QuadScene();
				quadScene.Init(quadSurface);

				quadSurface.run();
			})).Start();

			new Thread(new ThreadStart(() => {
				meshSurface = new RenderSurface(RenderCallback);
				meshSurface.clearColor = new Color(45,45,45);
				meshSurface.initWindowed("Mesh",800,600);

				meshScene = new MeshScene();
				meshScene.Init(meshSurface);

				meshSurface.setInputCallback(this);
				meshSurface.run();
			})).Start();

			new Thread(new ThreadStart(() => {
				textureSurface = new RenderSurface(RenderCallback);
				textureSurface.clearColor = new Color(15,15,45);
				textureSurface.initWindowed("Texture",640,480);

				textureScene = new TextureScene();
				textureScene.Init(textureSurface);

				textureSurface.setInputCallback(this);
				textureSurface.run();
			})).Start();
		}

		private void RenderCallback(D3DDeviceContext context,RenderSurface sender) {
			if(sender==triangleSurface)
				triangleScene.OnDraw();
			if(sender==quadSurface)
				quadScene.OnDraw();
			if(sender==meshSurface)
				meshScene.OnDraw();
			if(sender==textureSurface)
				textureScene.OnDraw();
		}

		public new void Dispose() {
			base.Dispose();
			triangleSurface.Dispose();
			quadSurface.Dispose();
			triangleScene.Dispose();
			quadScene.Dispose();
			meshScene.Dispose();
			textureSurface.Dispose();
			textureScene.Dispose();
		}

		public void RawEvent(InputEvent ev) {
			if(ev.sender==triangleSurface)
				ev.handle(triangleScene);
			if(ev.sender==quadSurface)
				ev.handle(quadScene);
			if(ev.sender==meshSurface)
				ev.handle(meshScene);
			if(ev.sender==textureSurface)
				ev.handle(textureScene);
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
