using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D3D11 = SharpDX.Direct3D11;

namespace RoomAliveTestApp {

	class ShaderBase : IDisposable {

		private D3D11.Device device;
		private D3D11.DeviceContext context;
		private D3D11.VertexShader vertexShader;
		private D3D11.PixelShader pixelShader;
		private ShaderSignature inputSignature;
		private D3D11.InputLayout inputLayout;
		private D3D11.Buffer uniformBuffer;

		public ShaderBase(D3D11.Device device,int uniformBytes) {
			this.device = device;
			this.context = device.ImmediateContext;
			var constantBufferDesc = new BufferDescription() {
				Usage = ResourceUsage.Dynamic,
				BindFlags = BindFlags.ConstantBuffer,
				SizeInBytes = uniformBytes,
				CpuAccessFlags = CpuAccessFlags.Write,
				StructureByteStride = 0,
				OptionFlags = 0,
			};
			uniformBuffer = new D3D11.Buffer(device,constantBufferDesc);
		}

		public void fromFiles(string vsFilename,string psFilename) {
			using(var vertexShaderByteCode = ShaderBytecode.CompileFromFile(vsFilename,"main","vs_4_0",ShaderFlags.Debug)) {
				vertexShader = new D3D11.VertexShader(device,vertexShaderByteCode);
				inputSignature = ShaderSignature.GetInputSignature(vertexShaderByteCode);
			}
			using(var pixelShaderByteCode = ShaderBytecode.CompileFromFile(psFilename,"main","ps_4_0",ShaderFlags.Debug)) {
				pixelShader = new D3D11.PixelShader(device,pixelShaderByteCode);
			}
			
		}

		public void setInputElements(D3D11.InputElement[] elements) {
			inputLayout = new D3D11.InputLayout(device,inputSignature,elements);
		}

		public void updateUniforms<T>(T values) where T : struct {
			SharpDX.DataStream dataStream;
			context.MapSubresource(uniformBuffer,MapMode.WriteDiscard,MapFlags.None,out dataStream);
			dataStream.Write<T>(values);
			context.UnmapSubresource(uniformBuffer,0);
		}

		public void updateUniforms(float[] values) {
			SharpDX.DataStream dataStream;
			context.MapSubresource(uniformBuffer,MapMode.WriteDiscard,MapFlags.None,out dataStream);
			for(int i=0;i<16;i++)
				dataStream.Write(values[i]);
			context.UnmapSubresource(uniformBuffer,0);
		}

		public void activate() {
			context.VertexShader.Set(vertexShader);
			context.PixelShader.Set(pixelShader);
			context.VertexShader.SetConstantBuffer(0,uniformBuffer);
			context.InputAssembler.InputLayout = inputLayout;
		}

		public void Dispose() {
			vertexShader.Dispose();
			pixelShader.Dispose();
			inputSignature.Dispose();
			inputLayout.Dispose();
		}
	}
}
