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
using SharpDX.WIC;

using D3D11 = SharpDX.Direct3D11;
using D3DDevice = SharpDX.Direct3D11.Device;
using D3DDeviceContext = SharpDX.Direct3D11.DeviceContext;

using RoomAliveToolkit;
using SharpDX.Direct3D;

namespace RoomAliveTestApp {

	class TestApp : ApplicationContext, IDisposable {

		//SharpDX objects
		private D3DDevice device;
		private D3DDeviceContext context;
		private SwapChain swapChain;
		private RenderForm renderForm;
		private D3D11.RenderTargetView renderTargetView;

		//Triangle members
		private Vector3[] vertices = new Vector3[] { new Vector3(-0.5f,0.5f,0.0f),new Vector3(0.5f,0.5f,0.0f),new Vector3(0.0f,-0.5f,0.0f) };
		private D3D11.InputElement[] inputElements = new D3D11.InputElement[] { new D3D11.InputElement("POSITION", 0, Format.R32G32B32_Float, 0) };
		private D3D11.Buffer triangleVertexBuffer;
		private ShaderSignature inputSignature;
		private D3D11.InputLayout inputLayout;
		private D3D11.VertexShader vertexShader;
		private D3D11.PixelShader pixelShader;

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

			//----Init-SharpDX----

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


			//----Init-drawing----

			triangleVertexBuffer = D3D11.Buffer.Create<Vector3>(device,D3D11.BindFlags.VertexBuffer,vertices);

			using(var vertexShaderByteCode = ShaderBytecode.CompileFromFile("vertexShader.hlsl","main","vs_4_0",ShaderFlags.Debug)) {
				vertexShader = new D3D11.VertexShader(device,vertexShaderByteCode);
				inputSignature = ShaderSignature.GetInputSignature(vertexShaderByteCode);
			}
			using(var pixelShaderByteCode = ShaderBytecode.CompileFromFile("pixelShader.hlsl","main","ps_4_0",ShaderFlags.Debug)) {
				pixelShader = new D3D11.PixelShader(device,pixelShaderByteCode);
			}
			inputLayout = new D3D11.InputLayout(device,inputSignature,inputElements);
			context.InputAssembler.InputLayout = inputLayout;
			context.Rasterizer.SetViewport(new Viewport(0,0,renderForm.ClientSize.Width,renderForm.ClientSize.Height));

			//----Init-RoomAlive----

			ensemble = RoomAliveToolkit.ProjectorCameraEnsemble.FromFile("C:/Users/Progga/Documents/Visual Studio 2015/Projects/RoomAliveToolkit/calibration/single_projector4/calibration4.xml");


			//----Start-----

			RenderLoop.Run(renderForm,RenderCallback);
			renderForm.FormClosing += OnMainFormClosed;
		}

		private void RenderCallback() {
			context.ClearRenderTargetView(renderTargetView, new SharpDX.Color(5,5,60));

			context.VertexShader.Set(vertexShader);
			context.PixelShader.Set(pixelShader);
			context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
			context.InputAssembler.SetVertexBuffers(0,new D3D11.VertexBufferBinding(triangleVertexBuffer,Utilities.SizeOf<Vector3>(),0));
			context.Draw(vertices.Count(),0);

			swapChain.Present(1,PresentFlags.None);
		}

		private void OnMainFormClosed(object sender,FormClosingEventArgs e) {
			Console.WriteLine("CLOSE");
			Environment.Exit(0);
		}

		public new void Dispose() {
			renderForm.Dispose();
			renderTargetView.Dispose();
			swapChain.Dispose();
			device.Dispose();
			context.Dispose();
			triangleVertexBuffer.Dispose();
			vertexShader.Dispose();
			pixelShader.Dispose();
			inputLayout.Dispose();
			inputSignature.Dispose();
			Console.WriteLine("Disposed");
		}

	}
}
