using D3D11 = SharpDX.Direct3D11;

namespace SharpGraphics.Shaders {

	public class PosColorShader : ShaderBase {

		protected FloatColor color = new FloatColor();

		public PosColorShader(D3D11.Device device) : base(device,16*SIZE_F,4*SIZE_F) {
			base.fromFiles("posColorVS.hlsl","posColorPS.hlsl");
			base.setInputElements(new[] { GFX.PositionInputElement,GFX.ColorInputElement });
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
