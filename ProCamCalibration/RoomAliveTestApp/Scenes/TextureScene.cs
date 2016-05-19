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

namespace RoomAliveTestApp.Scenes {

	class TextureScene : Scene {

		public Texture2D texture;
		public PosUVColorShader posUVColorShader;

		protected override void PostInit() {
			texture = TextureLoader.CreateTexture2DFromBitmap(device,TextureLoader.LoadBitmap(GFX.imagingFactory,"Assets/cucco.png"));
			posUVColorShader = new PosUVColorShader(device);
		}

		public override void OnDraw() {
			base.OnDraw();

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
			posUVColorShader.passColor(1,0.5f,0.2f,1);

			context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

			graphics.putIndicesQuad();
			graphics.putPosUVColor(new Vector3(-s+off,s,0),new Vector2(0,0),new Vector4(1,1,1,1));
			graphics.putPosUVColor(new Vector3(s+off,s,0),new Vector2(1,0),new Vector4(1,0,0,1));
			graphics.putPosUVColor(new Vector3(-s+off,-s,0),new Vector2(0,1),new Vector4(1,1,1,1));
			graphics.putPosUVColor(new Vector3(s+off,-s,0),new Vector2(1,1),new Vector4(1,1,1,1));
			graphics.flush();
		}

	}

}
