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

		public SharpGraphics graphics;

		//Callbacks
		private Action<D3DDeviceContext,RenderSurface> OnDraw;
		private InputCallbacks inputCallbacks;
		private NormMouseEvent mouseEvent = new NormMouseEvent();
		private MouseWheelEvent mouseWheelEvent = new MouseWheelEvent();
		private KeyEvent keyEvent = new KeyEvent();
		private MouseButtons mouseDragButton = MouseButtons.None;
		private float lstX, lstY;

		//SharpDX objects
		public D3DDevice device;
		public D3DDeviceContext context;
		public DepthStencilView depthStencilView;
		public SwapChain swapChain;
		public Viewport viewport { get; private set; }
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
			viewport = new Viewport(0,0,width,height);
			context.Rasterizer.SetViewport(viewport);

			//--Init-depth--
			var zBufferTextureDescription = new D3D11.Texture2DDescription {
				Format = Format.D16_UNorm,
				ArraySize = 1,
				MipLevels = 1,
				Width = getWidth(),
				Height = getHeight(),
				SampleDescription = new SampleDescription(1,0),
				Usage = D3D11.ResourceUsage.Default,
				BindFlags = D3D11.BindFlags.DepthStencil,
				CpuAccessFlags = D3D11.CpuAccessFlags.None,
				OptionFlags = D3D11.ResourceOptionFlags.None
			};
			using(var zBufferTexture = new Texture2D(device,zBufferTextureDescription))
				depthStencilView = new DepthStencilView(device,zBufferTexture);
			context.OutputMerger.SetTargets(depthStencilView,renderTargetView);

			graphics = new SharpGraphics(device);
		}

		public void setOrthographicProjection(float zoom,float near,float far) {
			Matrix.OrthoRH(ratioX*zoom*2,zoom*2,near,far,out projMat);
		}

		public void setOrthographicProjection() {
			setOrthographicProjection(1,-1,1);
		}

		public void setPerspectiveProjection(float fovy,float near,float far) {
			Matrix.PerspectiveFovRH(fovy,ratioX,near,far,out projMat);
		}

		public Matrix getProjectionMatrix() {
			return projMat;
		}

		public void setInputCallback(InputCallbacks callbacks) {
			this.inputCallbacks = callbacks;
		}

		private void RenderCallback() {
			context.ClearRenderTargetView(renderTargetView,clearColor);
			context.ClearDepthStencilView(depthStencilView,DepthStencilClearFlags.Depth,1,0);
			OnDraw(context,this);
			swapChain.Present(1,PresentFlags.None);
		}

		public void run() {
			RenderLoop.Run(renderForm,RenderCallback);
		}

		public float pixelToNormX(int pixelX) {
			return ((float)pixelX/width*2-1)*ratioX;
		}

		public float pixelToNormY(int pixelY) {
			return -(float)pixelY/height*2+1;
		}

		public int getWidth() {
			return width;
		}

		public int getHeight() {
			return height;
		}

		private NormMouseEvent CreateMouseEvent(int action,MouseEventArgs ev) {
			mouseEvent.handled = false;
			mouseEvent.sender = this;
			mouseEvent.action = action;
			mouseEvent.x = pixelToNormX(ev.X);
			mouseEvent.y = pixelToNormY(ev.Y);
			mouseEvent.dx = mouseEvent.x-lstX;
			mouseEvent.dy = mouseEvent.y-lstY;
			lstX = mouseEvent.x;
			lstY = mouseEvent.y;
			mouseEvent.button = ev.Button;
			return mouseEvent;
		}

		public void MouseDown(object sender,MouseEventArgs ev) {
			lstX = pixelToNormX(ev.X);
			lstY = pixelToNormY(ev.Y);
			mouseDragButton = ev.Button;
			if(inputCallbacks!=null) {
				CreateMouseEvent(NormMouseEvent.ACTION_MOUSEDOWN,ev).handle(inputCallbacks);
			}
		}

		public void MouseMove(object sender,MouseEventArgs ev) {
			if(inputCallbacks!=null) {
				if(mouseDragButton==MouseButtons.None)
					CreateMouseEvent(NormMouseEvent.ACTION_MOUSEMOVE,ev).handle(inputCallbacks);
				else {
					NormMouseEvent nEv = CreateMouseEvent(NormMouseEvent.ACTION_MOUSEDRAG,ev);
					nEv.button = mouseDragButton;
					nEv.handle(inputCallbacks);
				}
			}
		}

		public void MouseUp(object sender,MouseEventArgs ev) {
			mouseDragButton = MouseButtons.None;
			if(inputCallbacks!=null) {
				CreateMouseEvent(NormMouseEvent.ACTION_MOUSEUP,ev).handle(inputCallbacks);
			}
		}

		public void MouseWheel(object sender, MouseEventArgs ev) {
			if(inputCallbacks!=null) {
				mouseWheelEvent.handled = false;
				mouseWheelEvent.sender = this;
				mouseWheelEvent.amount = ev.Delta>0 ? 1 : -1;
				mouseWheelEvent.handle(inputCallbacks);
			}
		}

		private KeyEvent createKeyEvent(int action,KeyEventArgs ev) {
			keyEvent.handled = false;
			keyEvent.sender = this;
			keyEvent.action = action;
			keyEvent.code = ev.KeyCode;
			return keyEvent;
		}

		public void KeyDown(object sender,KeyEventArgs ev) {
			if(inputCallbacks!=null) {
				createKeyEvent(KeyEvent.ACTION_KEYDOWN,ev).handle(inputCallbacks);
			}
		}

		public void KeyUp(object sender,KeyEventArgs ev) {
			if(inputCallbacks!=null) {
				createKeyEvent(KeyEvent.ACTION_KEYUP,ev).handle(inputCallbacks);
			}
		}

		public void Dispose() {
			device.Dispose();
			context.Dispose();
			renderForm.Dispose();
			renderTargetView.Dispose();
			swapChain.Dispose();
		}

	}

}
