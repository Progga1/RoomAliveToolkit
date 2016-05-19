using SharpDX;
using SharpDX.DXGI;
using SharpDX.WIC;
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

		public struct PosUVColor {
			public Vector3 position;
			public Vector2 UV;
			public Vector4 color;

			public PosUVColor(Vector3 position,Vector2 uv,Vector4 color) {
				this.position=position;
				this.UV=uv;
				this.color=color;
			}
		}

		public static D3D11.InputElement[] positionInputElements = new[] {
			new D3D11.InputElement("POSITION",0,Format.R32G32B32_Float,0,0)
		};

		public static D3D11.InputElement[] posUVColorElements = new[] {
			new D3D11.InputElement("POSITION",0,Format.R32G32B32_Float,0,0),
			new D3D11.InputElement("TEXCOORD",0,Format.R32G32_Float,16,0),
			new D3D11.InputElement("COLOR",0,Format.R32G32B32A32_Float,24,0)
		};

		public static ImagingFactory2 imagingFactory = new ImagingFactory2();

		private Vector3[] positions = new Vector3[MAX_VERTICES];
		private PosUVColor[] posUVColorVals = new PosUVColor[MAX_VERTICES];
		private int[] indices = new int[MAX_VERTICES*2];

		private D3D11.Buffer positionBuffer;
		private D3D11.VertexBufferBinding positionBinding;
		private D3D11.Buffer posUVColorBuffer;
		private D3D11.VertexBufferBinding posUVColorBinding;
		private D3D11.Buffer indexBuffer;

		D3DDevice device;
		D3DDeviceContext context;

		private int positionPos = 0;
		private int indexPos = 0;
		private int posUVColorPos = 0;

		private D3D11.DepthStencilState depthEnabledState;
		private D3D11.DepthStencilState depthDisabledState;

		public static D3D11.DepthStencilView createZBuffer(D3DDevice device,int width,int height) {
			var zBufferTextureDescription = new D3D11.Texture2DDescription {
				Format = Format.D16_UNorm,
				ArraySize = 1,
				MipLevels = 1,
				Width = width,
				Height = height,
				SampleDescription = new SampleDescription(1,0),
				Usage = D3D11.ResourceUsage.Default,
				BindFlags = D3D11.BindFlags.DepthStencil,
				CpuAccessFlags = D3D11.CpuAccessFlags.None,
				OptionFlags = D3D11.ResourceOptionFlags.None
			};
			using(var zBufferTexture = new D3D11.Texture2D(device,zBufferTextureDescription))
				return new D3D11.DepthStencilView(device,zBufferTexture);
		}

		public SharpGraphics(D3DDevice device) {
			this.device = device;
			this.context = device.ImmediateContext;

			//--Init-buffers--
			var indexBufferDesc = new D3D11.BufferDescription() {
				Usage = D3D11.ResourceUsage.Dynamic,
				BindFlags = D3D11.BindFlags.IndexBuffer,
				SizeInBytes = indices.Length,
				CpuAccessFlags = D3D11.CpuAccessFlags.Write,
				StructureByteStride = 0,
				OptionFlags = 0,
			};
			indexBuffer = new D3D11.Buffer(device,indexBufferDesc);

			var positionBufferDesc = new D3D11.BufferDescription() {
				Usage = D3D11.ResourceUsage.Dynamic,
				BindFlags = D3D11.BindFlags.VertexBuffer,
				SizeInBytes = positions.Length,
				CpuAccessFlags = D3D11.CpuAccessFlags.Write,
				StructureByteStride = 0,
				OptionFlags = 0,
			};
			positionBuffer = new D3D11.Buffer(device,positionBufferDesc);
			positionBinding = new D3D11.VertexBufferBinding(positionBuffer,Utilities.SizeOf<Vector3>(),0);

			var vertexBufferDesc = new D3D11.BufferDescription() {
				Usage = D3D11.ResourceUsage.Dynamic,
				BindFlags = D3D11.BindFlags.VertexBuffer,
				SizeInBytes = posUVColorVals.Length,
				CpuAccessFlags = D3D11.CpuAccessFlags.Write,
				StructureByteStride = 0,
				OptionFlags = 0,
			};
			posUVColorBuffer = new D3D11.Buffer(device,vertexBufferDesc);
			posUVColorBinding = new D3D11.VertexBufferBinding(posUVColorBuffer,Utilities.SizeOf<PosUVColor>(),0);

			//--Init-depth--
			var depthStencilDesc = new D3D11.DepthStencilStateDescription {
				DepthWriteMask = D3D11.DepthWriteMask.All,
				DepthComparison = D3D11.Comparison.LessEqual,
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
			positions[positionPos++] = new Vector3(x,y,z);
		}

		public void putPos(Vector3 pos) {
			positions[positionPos++] = pos;
		}

		public void putPos(Vector3[] pos) {
			foreach(Vector3 p in pos)
				positions[positionPos++] = p;
		}

		public void putCubeLineIndices() {
			int baseIndex = positionPos;
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

		public void putIndicesTriangle(int index1,int index2,int index3) {
			indices[indexPos++] = index1;
			indices[indexPos++] = index2;
			indices[indexPos++] = index3;
		}

		public void putIndicesQuad() {
			int baseIndex = positionPos;
			indices[indexPos++] = baseIndex;
			indices[indexPos++] = baseIndex+1;
			indices[indexPos++] = baseIndex+2;
			indices[indexPos++] = baseIndex+3;
			indices[indexPos++] = baseIndex+2;
			indices[indexPos++] = baseIndex+1;
		}

		public void putPosUVColor(Vector3 position,Vector2 uv,Vector4 color) {
			posUVColorVals[posUVColorPos++] = new PosUVColor(position,uv,color);
		}

		public void flush() {
			if(indexPos==0)
				return;
			SharpDX.DataStream dataStream;

			context.MapSubresource(indexBuffer,D3D11.MapMode.WriteDiscard,D3D11.MapFlags.None,out dataStream);
			dataStream.WriteRange<int>(indices,0,indexPos);
			context.UnmapSubresource(indexBuffer,0);
			//alternative: context.UpdateSubresource(indices,indexBuffer);

			context.InputAssembler.SetIndexBuffer(indexBuffer,SharpDX.DXGI.Format.R32_UInt,0);

			if(positionPos>0) {
				context.MapSubresource(positionBuffer,D3D11.MapMode.WriteDiscard,D3D11.MapFlags.None,out dataStream);
				dataStream.WriteRange<Vector3>(positions,0,positionPos);
				context.UnmapSubresource(positionBuffer,0);

				context.InputAssembler.SetVertexBuffers(0,positionBinding);

				context.DrawIndexed(indexPos,0,0);
				positionPos = 0;
			}

			if(posUVColorPos>0) {
				context.MapSubresource(posUVColorBuffer,D3D11.MapMode.WriteDiscard,D3D11.MapFlags.None,out dataStream);
				//dataStream.WriteRange<float>(posUVColorData,0,posUVColorPos*4*4);
				dataStream.WriteRange<PosUVColor>(posUVColorVals,0,posUVColorPos);
				context.UnmapSubresource(posUVColorBuffer,0);

				context.InputAssembler.SetVertexBuffers(0,posUVColorBinding);

				context.DrawIndexed(indexPos,0,0);
				posUVColorPos = 0;
			}

			indexPos = 0;
		}

	}

}
