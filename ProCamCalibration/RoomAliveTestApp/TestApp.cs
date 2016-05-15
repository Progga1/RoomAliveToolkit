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

namespace RoomAliveTestApp {

	class TestApp : ApplicationContext, IDisposable, InputCallbacks {

		RenderSurface surface;
		private D3DDevice device;
		private D3DDeviceContext context;

		//Triangle members
		private Vector3[] vertices = new Vector3[] { new Vector3(-0.5f,0.5f,0.0f),new Vector3(0.5f,0.5f,0.0f),new Vector3(0.0f,-0.5f,0.0f) };
		private D3D11.Buffer triangleVertexBuffer;
		private D3D11.VertexBufferBinding binding;
		private SingleColorShader singleColorShader;
		private float[] mvp = new float[16];

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

			surface = new RenderSurface(RenderCallback);
			surface.initWindowed("Render",640,480);

			device = surface.device;
			context = surface.context;

			surface.setInputCallback(this);

			//----Init-drawing----

			triangleVertexBuffer = D3D11.Buffer.Create<Vector3>(device,D3D11.BindFlags.VertexBuffer,vertices);
			binding = new D3D11.VertexBufferBinding(triangleVertexBuffer,Utilities.SizeOf<Vector3>(),0);

			singleColorShader = new SingleColorShader(device);

			//----Init-RoomAlive----

			ensemble = RoomAliveToolkit.ProjectorCameraEnsemble.FromFile("C:/Users/Progga/Documents/Visual Studio 2015/Projects/RoomAliveToolkit/calibration/single_projector4/calibration4.xml");

			//----Start-----

			surface.run();
		}

		private void RenderCallback(D3DDeviceContext context,RenderSurface sender) {
			singleColorShader.activate();
			SharpDX.Matrix mvpMat = surface.getProjectionMatrix();
			mvp = mvpMat.ToArray();
			singleColorShader.updateVSConstantBuffer(mvp);
			singleColorShader.passColor(1,0.5f,0,1);
			context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
			context.InputAssembler.SetVertexBuffers(0,binding);
			context.Draw(vertices.Count(),0);
		}

		public new void Dispose() {
			surface.Dispose();
			triangleVertexBuffer.Dispose();
			singleColorShader.Dispose();
		}

		public void MouseDown(NormMouseEvent mouseEvent) {

		}

		public void MouseMove(NormMouseEvent mouseEvent) {
			
		}

		public void MouseDrag(NormMouseEvent mouseEvent) {
			int id = 0;
			if(mouseEvent.button==MouseButtons.Right)
				id = 1;
			if(mouseEvent.button==MouseButtons.Middle)
				id = 2;
			vertices[id].X = mouseEvent.x;
			vertices[id].Y = mouseEvent.y;

			context.UpdateSubresource(vertices,triangleVertexBuffer);
		}

		public void MouseUp(NormMouseEvent mouseEvent) {
			
		}

		public void MouseWheel(int amount,RenderSurface sender) {

		}

		public void KeyDown(Keys code,RenderSurface sender) {

		}

		public void KeyUp(Keys code,RenderSurface sender) {
			
		}
	}
}
