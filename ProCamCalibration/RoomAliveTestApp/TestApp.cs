using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Windows;
using SharpDX.DXGI;
using SharpDX.WIC;

using D3D11 = SharpDX.Direct3D11;
using D3DDevice = SharpDX.Direct3D11.Device;
using D3DDeviceContext = SharpDX.Direct3D11.DeviceContext;

using RoomAliveToolkit;
using SharpDX.Direct3D;

namespace RoomAliveTestApp {

	class TestApp : ApplicationContext, IDisposable {

		private D3DDevice device;
		private D3DDeviceContext context;
		private SwapChain swapChain;
		private RenderForm renderForm;
		private D3D11.RenderTargetView renderTargetView;

		private ProjectorCameraEnsemble ensemble;

		[STAThread]
		static void Main(string[] args) {
			using(TestApp app = new TestApp()) {
				Application.Run(app);
			}
		}

		public TestApp() {
			ensemble = RoomAliveToolkit.ProjectorCameraEnsemble.FromFile("C:/Users/Progga/Documents/Visual Studio 2015/Projects/RoomAliveToolkit/calibration/single_projector4/calibration4.xml");

//			var factory = new Factory1();
//            Adapter adapter = factory.Adapters[0];
//			device = new SharpDX.Direct3D11.Device(adapter,DeviceCreationFlags.None);

			renderForm = new RenderForm("Render");
			renderForm.ClientSize = new System.Drawing.Size(640,480);

			ModeDescription backBufferDesc = new ModeDescription(renderForm.ClientSize.Width,renderForm.ClientSize.Height,new Rational(60,1),Format.R8G8B8A8_UNorm);

			SwapChainDescription swapChainDesc = new SwapChainDescription() {
				ModeDescription = backBufferDesc,
				SampleDescription = new SampleDescription(1,0),
				Usage = Usage.RenderTargetOutput,
				BufferCount = 1,
				OutputHandle = renderForm.Handle,
				IsWindowed = true
			};

			D3DDevice.CreateWithSwapChain(DriverType.Hardware,D3D11.DeviceCreationFlags.None, swapChainDesc, out device, out swapChain);
			context = device.ImmediateContext;

			using(D3D11.Texture2D backBuffer = swapChain.GetBackBuffer<D3D11.Texture2D>(0)) {
				renderTargetView = new D3D11.RenderTargetView(device,backBuffer);
			}

			context.OutputMerger.SetRenderTargets(renderTargetView);


			RenderLoop.Run(renderForm,RenderCallback);
		}

		private void RenderCallback() {
			context.ClearRenderTargetView(renderTargetView, new SharpDX.Color(5,5,60));

			swapChain.Present(1,PresentFlags.None);
		}

		public new void Dispose() {
			renderForm.Dispose();
			renderTargetView.Dispose();
			swapChain.Dispose();
			device.Dispose();
			context.Dispose();
			Console.WriteLine("Disposed");
		}

	}
}
