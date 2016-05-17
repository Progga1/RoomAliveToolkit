using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using D3D11 = SharpDX.Direct3D11;
using D3DDevice = SharpDX.Direct3D11.Device;
using D3DDeviceContext = SharpDX.Direct3D11.DeviceContext;

namespace SharpGraphics {

	public class SharpGraphics {

		const int MAX_VERTICES = 100000;

		private Vector3[] positions = new Vector3[MAX_VERTICES];
		private int[] indices = new int[MAX_VERTICES*2];

		private D3D11.Buffer vertexBuffer;
		private D3D11.VertexBufferBinding vertexBinding;
		private D3D11.Buffer indexBuffer;

		D3DDevice device;
		D3DDeviceContext context;

		private int vertexPos = 0;
		private int indexPos = 0;

		private D3D11.DepthStencilState depthEnabledState;
		private D3D11.DepthStencilState depthDisabledState;

		public SharpGraphics(D3DDevice device) {
			this.device = device;
			this.context = device.ImmediateContext;

			//--Init-buffers--
			var vertexBufferDesc = new D3D11.BufferDescription() {
				Usage = D3D11.ResourceUsage.Dynamic,
				BindFlags = D3D11.BindFlags.VertexBuffer,
				SizeInBytes = positions.Length,
				CpuAccessFlags = D3D11.CpuAccessFlags.Write,
				StructureByteStride = 0,
				OptionFlags = 0,
			};
//			vertexBuffer = D3D11.Buffer.Create<Vector3>(device,D3D11.BindFlags.VertexBuffer,positions);
			vertexBuffer = new D3D11.Buffer(device,vertexBufferDesc);
			vertexBinding = new D3D11.VertexBufferBinding(vertexBuffer,Utilities.SizeOf<Vector3>(),0);

			var indexBufferDesc = new D3D11.BufferDescription() {
				Usage = D3D11.ResourceUsage.Dynamic,
				BindFlags = D3D11.BindFlags.IndexBuffer,
				SizeInBytes = indices.Length,
				CpuAccessFlags = D3D11.CpuAccessFlags.Write,
				StructureByteStride = 0,
				OptionFlags = 0,
			};
			//indexBuffer = D3D11.Buffer.Create<int>(device,D3D11.BindFlags.IndexBuffer,indices);
			indexBuffer = new D3D11.Buffer(device,indexBufferDesc);

			//--Init-depth--
			var depthStencilDesc = new D3D11.DepthStencilStateDescription {
				DepthWriteMask = D3D11.DepthWriteMask.All,
				DepthComparison = D3D11.Comparison.Less,
				IsDepthEnabled = true
			};
			depthEnabledState = new D3D11.DepthStencilState(device,depthStencilDesc);
			depthDisabledState = new D3D11.DepthStencilState(device,new D3D11.DepthStencilStateDescription { });
			setDepthEnabled(true);
		}

		public void setDepthEnabled(bool enabled) {
			context.OutputMerger.SetDepthStencilState(enabled ? depthEnabledState : depthDisabledState);
		}

		public void putPos(float x,float y,float z) {
			positions[vertexPos++] = new Vector3(x,y,z);
		}

		public void putPos(Vector3 pos) {
			positions[vertexPos++] = pos;
		}

		public void putPos(Vector3[] pos) {
			foreach(Vector3 p in pos)
				positions[vertexPos++] = p;
		}

		public void putCubeLineIndices() {
			int baseIndex = indexPos;
			indices[indexPos++] = baseIndex;
			indices[indexPos++] = baseIndex+1;
			indices[indexPos++] = baseIndex+1;
			indices[indexPos++] = baseIndex+3;
			indices[indexPos++] = baseIndex+3;
			indices[indexPos++] = baseIndex+2;
			indices[indexPos++] = baseIndex+2;
			indices[indexPos++] = baseIndex;

			indices[indexPos++] = baseIndex+4;
			indices[indexPos++] = baseIndex+5;
			indices[indexPos++] = baseIndex+5;
			indices[indexPos++] = baseIndex+7;
			indices[indexPos++] = baseIndex+7;
			indices[indexPos++] = baseIndex+6;
			indices[indexPos++] = baseIndex+6;
			indices[indexPos++] = baseIndex+4;

			indices[indexPos++] = baseIndex;
			indices[indexPos++] = baseIndex+4;
			indices[indexPos++] = baseIndex+1;
			indices[indexPos++] = baseIndex+5;
			indices[indexPos++] = baseIndex+2;
			indices[indexPos++] = baseIndex+6;
			indices[indexPos++] = baseIndex+3;
			indices[indexPos++] = baseIndex+7;
		}

		public void putIndex(int index) {
			indices[indexPos++] = index;
		}

		public void putIndexTriangle(int index1,int index2,int index3) {
			indices[indexPos++] = index1;
			indices[indexPos++] = index2;
			indices[indexPos++] = index3;
		}

		public void flush() {
			if(indexPos==0)
				return;
			SharpDX.DataStream dataStream;
//			context.MapSubresource(psConstantBuffer,MapMode.WriteDiscard,MapFlags.None,out dataStream);
//			dataStream.Write<T>(values);
//			context.UnmapSubresource(psConstantBuffer,0);
			context.MapSubresource(vertexBuffer,D3D11.MapMode.WriteDiscard,D3D11.MapFlags.None,out dataStream);
			dataStream.WriteRange<Vector3>(positions,0,vertexPos);
			context.UnmapSubresource(vertexBuffer,0);

			context.MapSubresource(indexBuffer,D3D11.MapMode.WriteDiscard,D3D11.MapFlags.None,out dataStream);
			dataStream.WriteRange<int>(indices,0,indexPos);
			context.UnmapSubresource(indexBuffer,0);

//			context.UpdateSubresource(positions,vertexBuffer);
//			context.UpdateSubresource(indices,indexBuffer);

			context.InputAssembler.SetVertexBuffers(0,vertexBinding);
			context.InputAssembler.SetIndexBuffer(indexBuffer,SharpDX.DXGI.Format.R32_UInt,0);

			context.DrawIndexed(indexPos,0,0);

			indexPos = 0;
			vertexPos = 0;
		}

	}

}
