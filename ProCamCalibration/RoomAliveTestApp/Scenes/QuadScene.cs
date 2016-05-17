using RoomAliveTestApp.Shaders;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.DXGI;
using SharpGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using D3D11 = SharpDX.Direct3D11;
using D3DDevice = SharpDX.Direct3D11.Device;
using D3DDeviceContext = SharpDX.Direct3D11.DeviceContext;

namespace RoomAliveTestApp.Scenes {

	class QuadScene : Scene {

		private D3D11.Buffer quadVertexBuffer;
		private D3D11.VertexBufferBinding quadBinding;
		private SingleColorShader singleColorShader;
		private D3D11.Buffer cubeVertexBuffer;
		private D3D11.VertexBufferBinding cubeBinding;
		private D3D11.Buffer cubeIndexBuffer;
		private CameraControl cameraControl = new CameraControl();

		protected override void PostInit() {
			quadVertexBuffer = D3D11.Buffer.Create<Vector3>(device,D3D11.BindFlags.VertexBuffer,DefaultMeshes.quadVertices);
			quadBinding = new D3D11.VertexBufferBinding(quadVertexBuffer,Utilities.SizeOf<Vector3>(),0);

			cubeVertexBuffer = D3D11.Buffer.Create<Vector3>(device,D3D11.BindFlags.VertexBuffer,DefaultMeshes.centerCubeVertices);
			cubeBinding = new D3D11.VertexBufferBinding(cubeVertexBuffer,Utilities.SizeOf<Vector3>(),0);
			cubeIndexBuffer = D3D11.Buffer.Create<int>(device,D3D11.BindFlags.IndexBuffer,DefaultMeshes.cubeIndices);

			cameraControl.transposed = true;
			singleColorShader = new SingleColorShader(device);
		}

		public override void OnDraw() {
			base.OnDraw();
			SharpDX.Matrix viewMat = cameraControl.getViewMatrix();
			surface.setOrthographicProjection(cameraControl.distance,-1,100);
			surface.setPerspectiveProjection(1.4f,0.01f,10);

			SharpDX.Matrix projMat = surface.getProjectionMatrix();
			projMat.Transpose();
			SharpDX.Matrix mvpMat = SharpDX.Matrix.Multiply(projMat,viewMat);
			float[] mvp = mvpMat.ToArray();
			singleColorShader.activate();
			singleColorShader.updateVSConstantBuffer(mvp);

			singleColorShader.passColor(1,0,0,1);
			context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
			context.InputAssembler.SetVertexBuffers(0,quadBinding);
			context.Draw(4,0);

			singleColorShader.passColor(1,1,0,1);
			context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
			context.InputAssembler.SetVertexBuffers(0,cubeBinding);
			context.InputAssembler.SetIndexBuffer(cubeIndexBuffer,SharpDX.DXGI.Format.R32_UInt,0);
			context.DrawIndexed(DefaultMeshes.cubeIndices.Length,0,0);

			singleColorShader.passColor(0,1,0,1);
			graphics.putPos(1,1,1);
			graphics.putPos(-1,0.5f,1);
			graphics.putIndex(0);
			graphics.putIndex(1);
			context.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
			graphics.flush();
		}

		public override void RawEvent(InputEvent ev) {
			ev.handle(cameraControl);
		}

		public override void Dispose() {
			singleColorShader.Dispose();
		}

	}
}
