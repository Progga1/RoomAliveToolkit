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

namespace RoomAliveTestApp {

	class TestApp : ApplicationContext, IDisposable, InputCallbacks {

		RenderSurface mainSurface;
		RenderSurface meshSurface;

		private Vector3[] triangleVertices = new Vector3[] { new Vector3(-0.5f,0.5f,0.0f),new Vector3(0.5f,0.5f,0.0f),new Vector3(0.0f,-0.5f,0.0f) };
		private Vector3[] quadVertices = new Vector3[] { new Vector3(0,1,0.05f),new Vector3(1,1,0.05f),new Vector3(0,0,0.05f),new Vector3(1,0,0.05f) };
		private D3D11.Buffer triangleVertexBuffer;
		private D3D11.Buffer quadVertexBuffer;
		private D3D11.VertexBufferBinding triangleBinding;
		private D3D11.VertexBufferBinding quadBinding;
		private SingleColorShader singleColorShader;
		private SingleColorShader singleColorShader2;
		private CameraControl cameraControl = new CameraControl();

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

			new Thread(new ThreadStart(() => {
				mainSurface = new RenderSurface(RenderCallback);
				mainSurface.setInputCallback(this);
				mainSurface.initWindowed("Triangle",640,480);

				triangleVertexBuffer = D3D11.Buffer.Create<Vector3>(mainSurface.device,D3D11.BindFlags.VertexBuffer,triangleVertices);
				triangleBinding = new D3D11.VertexBufferBinding(triangleVertexBuffer,Utilities.SizeOf<Vector3>(),0);
				singleColorShader = new SingleColorShader(mainSurface.device);

				mainSurface.run();
			})).Start();

			new Thread(new ThreadStart(() => {
				meshSurface = new RenderSurface(RenderCallback);
				meshSurface.setInputCallback(this);
				meshSurface.clearColor = new Color(45,45,45);
				meshSurface.initWindowed("Mesh",640,480);
				meshSurface.setOrthographicProjection(1,0,10);

				quadVertexBuffer = D3D11.Buffer.Create<Vector3>(meshSurface.device,D3D11.BindFlags.VertexBuffer,quadVertices);
				quadBinding = new D3D11.VertexBufferBinding(quadVertexBuffer,Utilities.SizeOf<Vector3>(),0);
				singleColorShader2 = new SingleColorShader(meshSurface.device);

				meshSurface.run();
			})).Start();
		}

		private void RenderCallback(D3DDeviceContext context,RenderSurface sender) {
			if(sender==mainSurface) {
				singleColorShader.activate();
				SharpDX.Matrix mvpMat = mainSurface.getProjectionMatrix();
				float[] mvp = mvpMat.ToArray();
				singleColorShader.updateVSConstantBuffer(mvp);
				singleColorShader.passColor(1,0.5f,0,1);
				context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
				context.InputAssembler.SetVertexBuffers(0,triangleBinding);
				context.Draw(triangleVertices.Count(),0);
			}
			if(sender==meshSurface) {
				SharpDX.Matrix viewMat = cameraControl.getViewMatrix();
				meshSurface.setOrthographicProjection(cameraControl.distance,-1,100);
//				meshSurface.setPerspectiveProjection(1.4f,0.01f,10);
				viewMat.Transpose();

				SharpDX.Matrix projMat = meshSurface.getProjectionMatrix();
				projMat.Transpose();
				SharpDX.Matrix mvpMat = SharpDX.Matrix.Multiply(projMat,viewMat);
				float[] mvp = mvpMat.ToArray();
				context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
				singleColorShader2.activate();
				singleColorShader2.updateVSConstantBuffer(mvp);
				singleColorShader2.passColor(1,0,0,1);
				context.InputAssembler.SetVertexBuffers(0,quadBinding);
				context.Draw(4,0);
			}
		}

		public new void Dispose() {
			mainSurface.Dispose();
			meshSurface.Dispose();
			triangleVertexBuffer.Dispose();
			singleColorShader.Dispose();
		}

		public void MouseDown(NormMouseEvent mouseEvent) {

		}

		public void MouseMove(NormMouseEvent mouseEvent) {
			
		}

		public void MouseDrag(NormMouseEvent mouseEvent) {
			if(mouseEvent.sender==mainSurface) {
				int id = 0;
				if(mouseEvent.button==MouseButtons.Right)
					id = 1;
				if(mouseEvent.button==MouseButtons.Middle)
					id = 2;
				triangleVertices[id].X = mouseEvent.x;
				triangleVertices[id].Y = mouseEvent.y;

				mainSurface.context.UpdateSubresource(triangleVertices,triangleVertexBuffer);
			}
			if(mouseEvent.sender==meshSurface) {
				cameraControl.MouseDrag(mouseEvent);
			}
		}

		public void MouseUp(NormMouseEvent mouseEvent) {
			
		}

		public void MouseWheel(int amount,RenderSurface sender) {
			if(sender==meshSurface)
				cameraControl.MouseWheel(amount,sender);
		}

		public void KeyDown(Keys code,RenderSurface sender) {
			if(sender==meshSurface)
				cameraControl.KeyDown(code,sender);
		}

		public void KeyUp(Keys code,RenderSurface sender) {
			if(sender==meshSurface)
				cameraControl.KeyUp(code,sender);
		}
	}
}
