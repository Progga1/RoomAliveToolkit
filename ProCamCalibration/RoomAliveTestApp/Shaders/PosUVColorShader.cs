using D3D11 = SharpDX.Direct3D11;

namespace SharpGraphics.Shaders {

	public class PosUVColorShader : ShaderBase {

		protected FloatColor color = new FloatColor();

		public PosUVColorShader(D3D11.Device device) : base(device,16*4,4*4) {
			base.fromFiles("posUVColorVS.hlsl","posUVColorPS.hlsl");
			base.setInputElements(GFX.PosUVColorElements);
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
