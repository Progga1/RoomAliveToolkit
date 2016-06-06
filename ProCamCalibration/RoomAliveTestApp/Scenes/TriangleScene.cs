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

namespace RoomAliveTestApps.Scenes {

	class TriangleScene : VirtualSceneBase {

		private Vector3[] triangleVertices = new Vector3[] { new Vector3(-0.5f,0.5f,0.0f),new Vector3(0.5f,0.5f,0.0f),new Vector3(0.0f,-0.5f,0.0f) };

		private D3D11.Buffer triangleVertexBuffer;
		private D3D11.VertexBufferBinding triangleBinding;

		protected override void PostInit() {
			triangleVertexBuffer = D3D11.Buffer.Create<Vector3>(device,D3D11.BindFlags.VertexBuffer,triangleVertices);
			triangleBinding = new D3D11.VertexBufferBinding(triangleVertexBuffer,Utilities.SizeOf<Vector3>(),0);
		}

		public override void OnDraw() {
			singleColorShader.activate();
			SharpDX.Matrix mvpMat = surface.getProjectionMatrix();
			float[] mvp = mvpMat.ToArray();
			singleColorShader.updateVSConstantBuffer(mvp);
			singleColorShader.passColor(1,0.5f,0,1);
			context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
			context.InputAssembler.SetVertexBuffers(0,triangleBinding);
			context.Draw(3,0);
		}

		public override void MouseDrag(NormMouseEvent ev) {
			base.MouseDrag(ev);
			int id = 0;
			if(ev.button==MouseButtons.Right)
				id = 1;
			if(ev.button==MouseButtons.Middle)
				id = 2;
			triangleVertices[id].X = ev.x;
			triangleVertices[id].Y = ev.y;

			context.UpdateSubresource(triangleVertices,triangleVertexBuffer);
		}

	}

}
