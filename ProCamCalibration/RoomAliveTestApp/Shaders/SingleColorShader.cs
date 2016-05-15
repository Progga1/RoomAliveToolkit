using SharpDX;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D3D11 = SharpDX.Direct3D11;

namespace RoomAliveTestApp.Shaders {

	class SingleColorShader : ShaderBase {

		protected FloatColor color = new FloatColor();

		private D3D11.InputElement[] inputElements = new D3D11.InputElement[] { new D3D11.InputElement("POSITION",0,Format.R32G32B32_Float,0) };

		public SingleColorShader(D3D11.Device device) : base(device,16*SIZE_F,4*SIZE_F) {
			base.fromFiles("singleColorVS.hlsl","singleColorPS.hlsl");
			base.setInputElements(inputElements);
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
