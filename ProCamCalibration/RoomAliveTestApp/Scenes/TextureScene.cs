using SharpGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX.Direct3D11;
using GFX = SharpGraphics.SharpGraphics;
using D3D11 = SharpDX.Direct3D11;
using D3DDevice = SharpDX.Direct3D11.Device;
using D3DDeviceContext = SharpDX.Direct3D11.DeviceContext;
using CommonDX;
using RoomAliveTestApp.Shaders;
using SharpDX.Direct3D;
using SharpDX;
using SharpDX.WIC;

namespace RoomAliveTestApp.Scenes {

	class TextureScene : Scene {

		public Texture2D texture;
		public SamplerState textureSampler;
		public ShaderResourceView texResource;
		public PosUVColorShader posUVColorShader;

		protected override void PostInit() {
			texture = TextureLoader.CreateTexture2DFromBitmap(device,TextureLoader.LoadBitmap(GFX.imagingFactory,"Assets/cucco.png"));

			// color sampler state
			var samplerStateDesc = new SamplerStateDescription() {
				Filter = Filter.MinMagMipLinear,
				AddressU = TextureAddressMode.Wrap,
				AddressV = TextureAddressMode.Wrap,
				AddressW = TextureAddressMode.Wrap,
				//BorderColor = new SharpDX.Color4(0.5f, 0.5f, 0.5f, 1.0f),
				//BorderColor = new SharpDX.Color4(0, 0, 0, 1.0f),
			};

			texResource = new ShaderResourceView(device,texture);
			textureSampler = new SamplerState(device,samplerStateDesc);

			posUVColorShader = new PosUVColorShader(device);
			
		}

		public override void OnDraw() {
			base.OnDraw();

			context.OutputMerger.SetBlendState(graphics.defaultBlendState);

			singleColorShader.activate();
			setMVP(surface.getProjectionMatrix());
			singleColorShader.updateVSConstantBuffer(mvpTransp);
			singleColorShader.passColor(1,0,0,1);

			context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

			float s = 0.4f;
			float off = 0.3f;
			graphics.putIndicesQuad();
			graphics.putPos(-s-off,s,0);
			graphics.putPos(s-off,s,0);
			graphics.putPos(-s-off,-s,0);
			graphics.putPos(s-off,-s,0);
			graphics.flush();

			posUVColorShader.activate();
			posUVColorShader.updateVSConstantBuffer(mvpTransp);
			posUVColorShader.passColor(1,1,1,1);

			context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

			context.PixelShader.SetSampler(0,textureSampler);
			context.PixelShader.SetShaderResource(0,texResource);

			graphics.putIndicesQuad();
			graphics.putPosUVColor(new Vector3(-s+off,s,0),new Vector2(0,0),new Vector4(1,1,1,1));
			graphics.putPosUVColor(new Vector3(s+off,s,0),new Vector2(1,0),new Vector4(1,0.4f,1,1));
			graphics.putPosUVColor(new Vector3(-s+off,-s,0),new Vector2(0,1),new Vector4(1,1,1,1));
			graphics.putPosUVColor(new Vector3(s+off,-s,0),new Vector2(1,1),new Vector4(1,1,1,1));
			graphics.flush();
		}

	}

}
