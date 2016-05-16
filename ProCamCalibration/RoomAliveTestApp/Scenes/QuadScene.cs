using RoomAliveTestApp.Shaders;
using SharpDX;
using SharpDX.Direct3D;
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
		private SingleColorShader singleColorShader2;
		private CameraControl cameraControl = new CameraControl();

		protected override void PostInit() {
			quadVertexBuffer = D3D11.Buffer.Create<Vector3>(device,D3D11.BindFlags.VertexBuffer,DefaultMeshes.quadVertices);
			quadBinding = new D3D11.VertexBufferBinding(quadVertexBuffer,Utilities.SizeOf<Vector3>(),0);
			singleColorShader2 = new SingleColorShader(device);
		}

		public override void OnDraw() {
			base.OnDraw();
			SharpDX.Matrix viewMat = cameraControl.getViewMatrix();
			surface.setOrthographicProjection(cameraControl.distance,-1,100);
			//				meshSurface.setPerspectiveProjection(1.4f,0.01f,10);
			viewMat.Transpose();

			SharpDX.Matrix projMat = surface.getProjectionMatrix();
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

		public override void RawEvent(InputEvent ev) {
			ev.handle(cameraControl);
		}

	}
}
