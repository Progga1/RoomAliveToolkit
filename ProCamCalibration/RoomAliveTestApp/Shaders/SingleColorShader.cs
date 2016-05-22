using RoomAliveTestApp;
using SharpDX;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D3D11 = SharpDX.Direct3D11;
using GFX = SharpGraphics.GFX;

namespace SharpGraphics.Shaders {

	public class SingleColorShader : ShaderBase {

		protected FloatColor color = new FloatColor();

		public SingleColorShader(D3D11.Device device) : base(device,16*SIZE_F,4*SIZE_F) {
			base.fromFiles("singleColorVS.hlsl","singleColorPS.hlsl");
			base.setInputElements(GFX.PositionInputElements);
		}

		public void passColor(float r,float g,float b,float a) {
			color.r = r;
			color.g = g;
			color.b = b;
			color.a = a;
			updatePSConstantBuffer<FloatColor>(color);
		}

		public void passColor(FloatColor color) {
			updatePSConstantBuffer<FloatColor>(color);
		}

	}

}
