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

		public static int SIZE_F = 4;

		private D3D11.Device device;
		private D3D11.DeviceContext context;
		private D3D11.VertexShader vertexShader;
		private D3D11.PixelShader pixelShader;
		private ShaderBytecode vertexShaderByteCode;
		private ShaderSignature inputSignature;
		private D3D11.InputLayout inputLayout;
		private D3D11.Buffer vsConstantBuffer;
		private D3D11.Buffer psConstantBuffer;

		public ShaderBase(D3D11.Device device,int vsConstantBufferBytes,int psConstantBufferBytes) {
			this.device = device;
			this.context = device.ImmediateContext;
			if(vsConstantBufferBytes>0) {
				var vsConstantBufferDesc = new BufferDescription() {
					Usage = ResourceUsage.Dynamic,
					BindFlags = BindFlags.ConstantBuffer,
					SizeInBytes = vsConstantBufferBytes,
					CpuAccessFlags = CpuAccessFlags.Write,
					StructureByteStride = 0,
					OptionFlags = 0,
				};
				vsConstantBuffer = new D3D11.Buffer(device,vsConstantBufferDesc);
			}

			if(psConstantBufferBytes>0) {
				var psConstantBufferDesc = new BufferDescription() {
					Usage = ResourceUsage.Dynamic,
					BindFlags = BindFlags.ConstantBuffer,
					SizeInBytes = psConstantBufferBytes,
					CpuAccessFlags = CpuAccessFlags.Write,
					StructureByteStride = 0,
					OptionFlags = 0,
				};
				psConstantBuffer = new D3D11.Buffer(device,psConstantBufferDesc);
			}
		}

		protected void fromFiles(string vsFilename,string psFilename) {
			vertexShaderByteCode = ShaderBytecode.CompileFromFile("Shaders/"+vsFilename,"main","vs_4_0",ShaderFlags.Debug);
			vertexShader = new D3D11.VertexShader(device,vertexShaderByteCode);
			inputSignature = ShaderSignature.GetInputSignature(vertexShaderByteCode);
			
			using(var pixelShaderByteCode = ShaderBytecode.CompileFromFile("Shaders/"+psFilename,"main","ps_4_0",ShaderFlags.Debug)) {
				pixelShader = new D3D11.PixelShader(device,pixelShaderByteCode);
			}
			
		}

		protected void setInputElements(D3D11.InputElement[] elements) {
			inputLayout = new D3D11.InputLayout(device,vertexShaderByteCode.Data,elements);
		}

		public void updateVSConstantBuffer<T>(T values) where T : struct {
			SharpDX.DataStream dataStream;
			context.MapSubresource(vsConstantBuffer,MapMode.WriteDiscard,MapFlags.None,out dataStream);
			dataStream.Write<T>(values);
			context.UnmapSubresource(vsConstantBuffer,0);
		}

		public void updateVSConstantBuffer(float[] values) {
			SharpDX.DataStream dataStream;
			context.MapSubresource(vsConstantBuffer,MapMode.WriteDiscard,MapFlags.None,out dataStream);
			for(int i=0;i<values.Length;i++)
				dataStream.Write(values[i]);
			context.UnmapSubresource(vsConstantBuffer,0);
		}

		public void updatePSConstantBuffer<T>(T values) where T : struct {
			SharpDX.DataStream dataStream;
			context.MapSubresource(psConstantBuffer,MapMode.WriteDiscard,MapFlags.None,out dataStream);
			dataStream.Write<T>(values);
			context.UnmapSubresource(psConstantBuffer,0);
		}

		public void updatePSConstantBuffer(float[] values) {
			SharpDX.DataStream dataStream;
			context.MapSubresource(psConstantBuffer,MapMode.WriteDiscard,MapFlags.None,out dataStream);
			for(int i = 0; i<values.Length; i++)
				dataStream.Write(values[i]);
			context.UnmapSubresource(psConstantBuffer,0);
		}

		public void activate() {
			context.VertexShader.Set(vertexShader);
			context.PixelShader.Set(pixelShader);
			if(vsConstantBuffer!=null)
				context.VertexShader.SetConstantBuffer(0,vsConstantBuffer);
			if(psConstantBuffer!=null)
				context.PixelShader.SetConstantBuffer(0,psConstantBuffer);
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
