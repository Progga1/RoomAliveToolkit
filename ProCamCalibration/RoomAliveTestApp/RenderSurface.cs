using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;
using SharpDX.Windows;
using SharpDX.DXGI;
using SharpDX.WIC;

using D3D11 = SharpDX.Direct3D11;
using D3DDevice = SharpDX.Direct3D11.Device;
using D3DDeviceContext = SharpDX.Direct3D11.DeviceContext;
using SharpDX.Direct3D;
using System.Windows.Forms;

namespace SharpGraphics {

	public class RenderSurface : IDisposable {

		public int width { get; private set; }
		public int height { get; private set; }
		public float ratioX { get; private set; }
		public SharpDX.Color clearColor = new SharpDX.Color(5,5,60);

		//Callbacks
		private Action<D3DDeviceContext,RenderSurface> OnDraw;
		private InputCallbacks inputCallbacks;
		private NormMouseEvent mouseEvent = new NormMouseEvent();
		private MouseButtons mouseDragButton = MouseButtons.None;

		//SharpDX objects
		public D3DDevice device;
		public D3DDeviceContext context;
		public SwapChain swapChain;
		public RenderForm renderForm { get; private set; }
		public D3D11.RenderTargetView renderTargetView { get; private set; }

		//Rendering
		private Matrix projMat;

		public RenderSurface(Action<D3DDeviceContext,RenderSurface> drawCallback) {
			OnDraw = drawCallback;
		}

		public void initWindowed(string title,int width,int height) {
			this.width = width;
			this.height = height;
			this.ratioX = (float)width/height;
			setOrthographicProjection();
			renderForm = new RenderForm(title);
			renderForm.ClientSize = new System.Drawing.Size(width,height);

			renderForm.MouseDown += MouseDown;
			renderForm.MouseMove += MouseMove;
			renderForm.MouseUp += MouseUp;
			renderForm.MouseWheel += MouseWheel;
			renderForm.KeyDown += KeyDown;
			renderForm.KeyUp += KeyUp;

			ModeDescription backBufferDesc = new ModeDescription(renderForm.ClientSize.Width,renderForm.ClientSize.Height,new Rational(60,1),Format.R8G8B8A8_UNorm);

			SwapChainDescription swapChainDesc = new SwapChainDescription() {
				ModeDescription = backBufferDesc,
				SampleDescription = new SampleDescription(1,0),
				Usage = Usage.RenderTargetOutput,
				BufferCount = 1,
				OutputHandle = renderForm.Handle,
				IsWindowed = true
			};

			D3DDevice.CreateWithSwapChain(DriverType.Hardware,D3D11.DeviceCreationFlags.None,swapChainDesc,out device,out swapChain);
			context = device.ImmediateContext;

			using(D3D11.Texture2D backBuffer = swapChain.GetBackBuffer<D3D11.Texture2D>(0)) {
				renderTargetView = new D3D11.RenderTargetView(device,backBuffer);
			}

			context.OutputMerger.SetRenderTargets(renderTargetView);
		}

		public void setOrthographicProjection(int zoom,float near,float far) {
			Matrix.OrthoLH(ratioX*zoom,zoom,near,far,out projMat);
		}

		public void setOrthographicProjection() {
			setOrthographicProjection(1,-1,1);
		}

		public Matrix getProjectionMatrix() {
			return projMat;
		}

		public void setInputCallback(InputCallbacks callbacks) {
			this.inputCallbacks = callbacks;
		}

		private void RenderCallback() {
			context.ClearRenderTargetView(renderTargetView,clearColor);
			OnDraw(context,this);
			swapChain.Present(1,PresentFlags.None);
		}

		public void run() {
			RenderLoop.Run(renderForm,RenderCallback);
		}

		public void Dispose() {
			device.Dispose();
			context.Dispose();
			renderForm.Dispose();
			renderTargetView.Dispose();
			swapChain.Dispose();
		}

		public float pixelToNormX(int pixelX) {
			return ((float)pixelX/width*2-1)*ratioX;
		}

		public float pixelToNormY(int pixelY) {
			return -(float)pixelY/height*2+1;
		}

		private NormMouseEvent CreateMouseEvent(MouseEventArgs ev) {
			mouseEvent.x = pixelToNormX(ev.X);
			mouseEvent.y = pixelToNormY(ev.Y);
			mouseEvent.button = ev.Button;
			mouseEvent.sender = this;
			return mouseEvent;
		}

		public void MouseDown(object sender,MouseEventArgs ev) {
			mouseDragButton = ev.Button;
			if(inputCallbacks!=null) {
				CreateMouseEvent(ev);
			}
		}

		public void MouseMove(object sender,MouseEventArgs ev) {
			if(inputCallbacks!=null) {
				if(mouseDragButton==MouseButtons.None)
					inputCallbacks.MouseMove(CreateMouseEvent(ev));
				else {
					NormMouseEvent nEv = CreateMouseEvent(ev);
					nEv.button = mouseDragButton;
					inputCallbacks.MouseDrag(nEv);
				}
			}
		}

		public void MouseWheel(object sender, MouseEventArgs ev) {
			if(inputCallbacks!=null) {
				inputCallbacks.MouseWheel(ev.Delta,this);
			}
		}

		public void MouseUp(object sender,MouseEventArgs ev) {
			mouseDragButton = MouseButtons.None;
			if(inputCallbacks!=null) {
				inputCallbacks.MouseUp(CreateMouseEvent(ev));
			}
		}

		public void KeyDown(object sender,KeyEventArgs ev) {
			if(inputCallbacks!=null) {
				inputCallbacks.KeyDown(ev.KeyCode,this);
			}
		}

		public void KeyUp(object sender,KeyEventArgs ev) {
			if(inputCallbacks!=null) {
				inputCallbacks.KeyUp(ev.KeyCode,this);
			}
		}

	}

}
