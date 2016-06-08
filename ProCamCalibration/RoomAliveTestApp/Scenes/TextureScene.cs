using SharpGraphics;
using SharpDX.Direct3D11;
using GFX = SharpGraphics.GFX;
using D3D11 = SharpDX.Direct3D11;
using D3DDevice = SharpDX.Direct3D11.Device;
using D3DDeviceContext = SharpDX.Direct3D11.DeviceContext;
using CommonDX;
using SharpDX.Direct3D;
using SharpDX;
using SharpGraphics.Shaders;

namespace RoomAliveTestApps.Scenes {

	class TextureScene : VirtualSceneBase {

		public SharpTexture texture;

		protected override void PostInit() {
			texture = new SharpTexture(device,"Assets/cucco.png");
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

			graphics.bindTexture(texture);

			graphics.putIndicesQuad();
			graphics.putPosUVColor(new Vector3(-s+off,s,0),new Vector2(0,0),new Vector4(1,1,1,1));
			graphics.putPosUVColor(new Vector3(s+off,s,0),new Vector2(1,0),new Vector4(1,0.4f,1,1));
			graphics.putPosUVColor(new Vector3(-s+off,-s,0),new Vector2(0,1),new Vector4(1,1,1,1));
			graphics.putPosUVColor(new Vector3(s+off,-s,0),new Vector2(1,1),new Vector4(1,1,1,1));
			graphics.flush();
		}

	}

}
