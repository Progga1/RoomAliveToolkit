using SharpDX;
using SharpDX.DXGI;
using System;

using SharpDX.Direct3D11;
using D3D11 = SharpDX.Direct3D11;
using D3DDevice = SharpDX.Direct3D11.Device;
using D3DDeviceContext = SharpDX.Direct3D11.DeviceContext;
using SharpMatrix = SharpDX.Matrix;
using SharpGraphics.Shaders;
using RoomAliveToolkit;
using SharpDX.Direct3D;
using SharpDX.WIC;

namespace SharpGraphics {

	public class GFX : IDisposable {

		const int MAX_VERTICES = 1600000;

		public struct PosUVColor {
			public Vector3 position;
			public Vector2 uv;
			public Vector4 color;

			public PosUVColor(Vector3 position,Vector2 uv,Vector4 color) {
				this.position=position;
				this.uv=uv;
				this.color=color;
			}
		}

		public static InputElement PositionInputElement = new InputElement("POSITION",0,Format.R32G32B32_Float,0,0);
		public static InputElement UVInputElement = new InputElement("TEXCOORD",0,Format.R32G32_Float,0,1);
		public static InputElement ColorInputElement = new InputElement("COLOR",0,Format.R32G32B32A32_Float,0,2);

		public static InputElement[] PositionInputElements = new[] {
			PositionInputElement
		};

		public static InputElement[] PosUVColorElements = new[] {
			PositionInputElement,
			new InputElement("TEXCOORD",0,Format.R32G32_Float,12,0),
			new InputElement("COLOR",0,Format.R32G32B32A32_Float,20,0)
		};

		public static InputElement[] PosUVColorElementsSep = new[] {
			PositionInputElement,
			UVInputElement,
			ColorInputElement
		};

		public SingleColorShader singleColorShader { get; private set; }
		public PosUVColorShader posUVColorShader { get; private set; }
		public PosUVColorShaderSep posUVColorShaderSep { get; private set; }
		public PosColorShader posColorShader { get; private set; }

		public static ImagingFactory2 imagingFactory = new ImagingFactory2();

		private Vector3[] positions = new Vector3[MAX_VERTICES];
		private Vector2[] UVs = new Vector2[MAX_VERTICES];
		private FloatColor[] colors = new FloatColor[MAX_VERTICES];
		private PosUVColor[] posUVColorVals = new PosUVColor[MAX_VERTICES];
		private int[] indices = new int[MAX_VERTICES*2];

		private D3D11.Buffer positionBuffer;
		private VertexBufferBinding positionBinding;
		private D3D11.Buffer colorBuffer;
		private VertexBufferBinding colorBinding;
		private D3D11.Buffer uvBuffer;
		private VertexBufferBinding uvBinding;
		private D3D11.Buffer posUVColorBuffer;
		private VertexBufferBinding posUVColorBinding;
		private D3D11.Buffer indexBuffer;

		D3DDevice device;
		D3DDeviceContext context;

		private int positionPos = 0;
		private int colorPos = 0;
		private int uvPos = 0;
		private int indexPos = 0;
		private int posUVColorPos = 0;

		//Default states
		public RasterizerState defaultRasterizerState;
		private D3D11.DepthStencilState depthEnabledState;
		private D3D11.DepthStencilState depthDisabledState;
		public BlendState defaultBlendState;

		public static D3D11.DepthStencilView createZBuffer(D3DDevice device,int width,int height) {
			var zBufferTextureDescription = new D3D11.Texture2DDescription {
				Format = Format.D32_Float,
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

		public GFX(D3DDevice device) {
			this.device = device;
			this.context = device.ImmediateContext;

			//--Init-rasterizer-state--
			var rasterizerStateDesc = new RasterizerStateDescription() {
				CullMode = CullMode.None,
				FillMode = FillMode.Solid,
				IsDepthClipEnabled = true,
//				IsFrontCounterClockwise = true,
//				IsMultisampleEnabled = true,
			};
			defaultRasterizerState = new RasterizerState(device,rasterizerStateDesc);
			context.Rasterizer.State = defaultRasterizerState;

			//--Init-buffers--
			var indexBufferDesc = new BufferDescription() {
				Usage = ResourceUsage.Dynamic,
				BindFlags = BindFlags.IndexBuffer,
				SizeInBytes = indices.Length*4,
				CpuAccessFlags = CpuAccessFlags.Write,
				StructureByteStride = 0,
				OptionFlags = 0,
			};
			indexBuffer = new D3D11.Buffer(device,indexBufferDesc);

			var positionBufferDesc = new BufferDescription() {
				Usage = ResourceUsage.Dynamic,
				BindFlags = BindFlags.VertexBuffer,
				SizeInBytes = positions.Length*Utilities.SizeOf<Vector3>(),
				CpuAccessFlags = CpuAccessFlags.Write,
				StructureByteStride = 0,
				OptionFlags = 0,
			};
			positionBuffer = new D3D11.Buffer(device,positionBufferDesc);
			positionBinding = new VertexBufferBinding(positionBuffer,Utilities.SizeOf<Vector3>(),0);

			var uvBufferDesc = new BufferDescription() {
				Usage = ResourceUsage.Dynamic,
				BindFlags = BindFlags.VertexBuffer,
				SizeInBytes = UVs.Length*Utilities.SizeOf<Vector2>(),
				CpuAccessFlags = CpuAccessFlags.Write,
				StructureByteStride = 0,
				OptionFlags = 0,
			};
			uvBuffer = new D3D11.Buffer(device,uvBufferDesc);
			uvBinding = new VertexBufferBinding(uvBuffer,Utilities.SizeOf<Vector2>(),0);

			var colorBufferDesc = new BufferDescription() {
				Usage = ResourceUsage.Dynamic,
				BindFlags = BindFlags.VertexBuffer,
				SizeInBytes = colors.Length*Utilities.SizeOf<FloatColor>(),
				CpuAccessFlags = CpuAccessFlags.Write,
				StructureByteStride = 0,
				OptionFlags = 0,
			};
			colorBuffer = new D3D11.Buffer(device,colorBufferDesc);
			colorBinding = new VertexBufferBinding(colorBuffer,Utilities.SizeOf<FloatColor>(),0);

			var vertexBufferDesc = new BufferDescription() {
				Usage = ResourceUsage.Dynamic,
				BindFlags = BindFlags.VertexBuffer,
				SizeInBytes = posUVColorVals.Length,
				CpuAccessFlags = CpuAccessFlags.Write,
				StructureByteStride = 0,
				OptionFlags = 0,
			};
			posUVColorBuffer = new D3D11.Buffer(device,vertexBufferDesc);
			posUVColorBinding = new VertexBufferBinding(posUVColorBuffer,Utilities.SizeOf<PosUVColor>(),0);

			//--Init-depth--
			var depthStencilDesc = new DepthStencilStateDescription {
				DepthWriteMask = DepthWriteMask.All,
				DepthComparison = Comparison.LessEqual,
				IsDepthEnabled = true
			};
			depthEnabledState = new DepthStencilState(device,depthStencilDesc);
			depthDisabledState = new DepthStencilState(device,new DepthStencilStateDescription { });
			setDepthEnabled(true);

			//--Init-blending
			var blendDesc = new RenderTargetBlendDescription(
					true,
					BlendOption.SourceAlpha,
					BlendOption.InverseSourceAlpha,
					BlendOperation.Add,
					BlendOption.One,
					BlendOption.Zero,
					BlendOperation.Add,
					ColorWriteMaskFlags.All);
			var blendDescription = new BlendStateDescription();
			blendDescription.RenderTarget[0] = blendDesc;
			defaultBlendState = new BlendState(device,blendDescription);
			context.OutputMerger.SetBlendState(defaultBlendState);

			//--Init-shaders--
			singleColorShader = new SingleColorShader(device);
			posUVColorShader = new PosUVColorShader(device);
			posUVColorShaderSep = new PosUVColorShaderSep(device);
			posColorShader = new PosColorShader(device);
		}

		public void setDepthEnabled(bool enabled) {
			context.OutputMerger.SetDepthStencilState(enabled ? depthEnabledState : depthDisabledState);
		}

		public void putPos(float x,float y,float z) {
			positions[positionPos++] = new Vector3(x,y,z);
		}

		public void putPositionsMesh(Mesh mesh) {
			foreach(Mesh.VertexPositionNormalTexture elem in mesh.vertices) {
				indices[indexPos++] = positionPos;
				positions[positionPos++] = new Vector3(elem.position.X,elem.position.Y,elem.position.Z);
			}
		}

		public void putPositionsMesh(Mesh mesh,int start,int count) {
			int i = -1;
			foreach(Mesh.VertexPositionNormalTexture elem in mesh.vertices) {
				i++;
				if(i<start || i>=start+count)
					continue;
				indices[indexPos++] = positionPos;
				positions[positionPos++] = new Vector3(elem.position.X,elem.position.Y,elem.position.Z);
			}
		}

		public void putColorsByMeshNormals(Mesh mesh,FloatColor color,float minDot,Vector3 brightDir) {
			foreach(Mesh.VertexPositionNormalTexture elem in mesh.vertices) {
				float dot = Vector3.Dot(brightDir,elem.normal);
				if(dot<minDot)
					dot = minDot;
				colors[colorPos++] = dot * color;
			}
		}

		public void putColorsByMeshNormals(Mesh mesh,FloatColor color,float minDot,Vector3 brightDir,int start,int count) {
			int i = -1;
			foreach(Mesh.VertexPositionNormalTexture elem in mesh.vertices) {
				i++;
				if(i<start || i>=start+count)
					continue;
				float dot = Vector3.Dot(brightDir,elem.normal);
				if(dot<minDot)
					dot = minDot;
				colors[colorPos++] = dot * color;
			}
		}

		public void putUVsMesh(Mesh mesh) {
			foreach(Mesh.VertexPositionNormalTexture elem in mesh.vertices) {
				UVs[uvPos++] = elem.texture;
			}
		}

		public void putUVsMesh(Mesh mesh,int start,int count) {
			int i = -1;
			foreach(Mesh.VertexPositionNormalTexture elem in mesh.vertices) {
				i++;
				if(i<start || i>=start+count)
					continue;
				UVs[uvPos++] = elem.texture;
			}
		}

		public void drawMesh(RoomMesh rMesh,SharpMatrix mvp,FloatColor colorFactor,Vector3 lightNormal) {
			mvp.Transpose();
			context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
			bool useLight = lightNormal!=Vector3.Zero;
			foreach(var subset in rMesh.mesh.subsets) {
				putPositionsMesh(rMesh.mesh,subset.start,subset.length);
				putUVsMesh(rMesh.mesh,subset.start,subset.length);
				if(useLight)
					putColorsByMeshNormals(rMesh.mesh,new FloatColor(subset.material.diffuseColor),0.05f,lightNormal,subset.start,subset.length);
				else
					putColor(FloatColor.White,subset.length);
				if(subset.material.textureFilename == null) {
					posColorShader.activate();
					posColorShader.passColor(colorFactor);
					posColorShader.updateVSConstantBuffer(mvp);
				} else {
					context.PixelShader.SetSampler(0,rMesh.meshShader.colorSamplerState);
					context.PixelShader.SetShaderResource(0,rMesh.meshDeviceResources.textureRVs[subset]);
					posUVColorShaderSep.activate();
					posUVColorShaderSep.passColor(colorFactor);
					posUVColorShaderSep.updateVSConstantBuffer(mvp);
				}
				flush();
			}
		}

		public static void projectionNormToTexCoords(ref SharpMatrix projMat) {
			projMat[0,0] /= 2;
			projMat[1,1] /= -2;
			projMat[2,0] += 0.5f; //obligue, implicit 0.5*z/w, wheras w=z
			projMat[2,1] += 0.5f;
		}

		public void putPos(Vector3 pos) {
			positions[positionPos++] = pos;
		}

		public void putPos(Vector3[] pos) {
			foreach(Vector3 p in pos)
				positions[positionPos++] = p;
		}

		public void putColor(FloatColor color) {
			colors[colorPos++] = color;
		}

		public void putColor(FloatColor color,int amount) {
			for(int i = 0; i<amount; i++)
				colors[colorPos++] = color;
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

		public void putUVs(Vector2 uv) {
			UVs[uvPos++] = uv;
		}

		public void putUVs(float u,float v) {
			UVs[uvPos++] = new Vector2(u,v);
		}

		public void putIndex(int index) {
			indices[indexPos++] = index;
		}

		public void putIndicesTriangle(int index1,int index2,int index3) {
			indices[indexPos++] = index1;
			indices[indexPos++] = index2;
			indices[indexPos++] = index3;
		}

		public void putRectPosUVColor(float left,float top, float width,float height, Vector4 color) {
			putIndicesQuad();
			float uvLeft = 0;
			float uvTop = 0;
			float uvRight = 1;
			float uvBottom = 1;
			float z = 0;
			putPosUVColor(new Vector3(left,top,z),new Vector2(uvLeft,uvTop),color);
			putPosUVColor(new Vector3(left+width,top,z),new Vector2(uvRight,uvTop),color);
			putPosUVColor(new Vector3(left,top-height,z),new Vector2(uvLeft,uvBottom),color);
			putPosUVColor(new Vector3(left+width,top-height,z),new Vector2(uvRight,uvBottom),color);
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

		public void bindTexture(SharpTexture texture) {
			context.PixelShader.SetSampler(0,texture.sampler);
			context.PixelShader.SetShaderResource(0,texture.resource);
		}

		private SharpDX.DataStream dataStream;
		private void updateBuffer<T>(D3D11.Buffer buffer,T[] data,int count) where T:struct {
			context.MapSubresource(buffer,D3D11.MapMode.WriteDiscard,D3D11.MapFlags.None,out dataStream);
			dataStream.WriteRange(data,0,count);
			context.UnmapSubresource(positionBuffer,0);
		}

		public void flush() {
			if(indexPos==0)
				return;

			context.MapSubresource(indexBuffer,D3D11.MapMode.WriteDiscard,D3D11.MapFlags.None,out dataStream);
			dataStream.WriteRange<int>(indices,0,indexPos);
			context.UnmapSubresource(indexBuffer,0);
			//alternative: context.UpdateSubresource(indices,indexBuffer);

			context.InputAssembler.SetIndexBuffer(indexBuffer,SharpDX.DXGI.Format.R32_UInt,0);

			if(positionPos>0) {
				updateBuffer(positionBuffer,positions,positionPos);
				context.InputAssembler.SetVertexBuffers(0,positionBinding);
				positionPos = 0;

				if(uvPos>0) {
					updateBuffer(uvBuffer,UVs,uvPos);
					context.InputAssembler.SetVertexBuffers(1,uvBinding);
					uvPos = 0;
				}

				if(colorPos>0) {
					updateBuffer(colorBuffer,colors,colorPos);
					context.InputAssembler.SetVertexBuffers(2,colorBinding);
					colorPos = 0;
				}

				context.DrawIndexed(indexPos,0,0);

				
			}

			if(posUVColorPos>0) {
				context.MapSubresource(posUVColorBuffer,D3D11.MapMode.WriteDiscard,D3D11.MapFlags.None,out dataStream);
				dataStream.WriteRange<PosUVColor>(posUVColorVals,0,posUVColorPos);
				context.UnmapSubresource(posUVColorBuffer,0);

				context.InputAssembler.SetVertexBuffers(0,posUVColorBinding);

				context.DrawIndexed(indexPos,0,0);
				posUVColorPos = 0;
			}

			indexPos = 0;
		}

		public void Dispose() {
			singleColorShader.Dispose();
			posUVColorShader.Dispose();
			positionBuffer.Dispose();
			posUVColorBuffer.Dispose();
			indexBuffer.Dispose();
		}
	}

}
