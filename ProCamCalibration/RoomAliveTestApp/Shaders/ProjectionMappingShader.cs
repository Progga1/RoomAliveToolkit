using SharpDX;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using D3D11 = SharpDX.Direct3D11;
using GFX = SharpGraphics.GFX;
using SharpMatrix = SharpDX.Matrix;

namespace SharpGraphics.Shaders {

	public class ProjectionMappingShader : ShaderBase {

		[StructLayout(LayoutKind.Explicit,Size = 64*2)]
		public unsafe struct VSConstants {
			[FieldOffset(0)]
			public SharpMatrix mvpProjector;
			[FieldOffset(64)]
			public SharpMatrix mvpUser;
		}

		protected FloatColor color = new FloatColor();

		public ProjectionMappingShader(D3D11.Device device) : base(device,2*16*4,4*4) {
			base.fromFiles("projectionMappingVS.hlsl","projectionMappingPS.hlsl");
			base.setInputElements(new[] { GFX.positionInputElement,new D3D11.InputElement("COLOR",0,Format.R32G32B32A32_Float,0,1) });
		}

		public void passTransformations(SharpMatrix m,SharpMatrix projVP,SharpMatrix userV,SharpMatrix userP) {
			var cb = new VSConstants();

			GFX.projectionNormToTexCoords(ref userP);
			cb.mvpProjector = m * projVP;
			cb.mvpUser = m * userV * userP;
			cb.mvpProjector.Transpose();
			cb.mvpUser.Transpose();
			base.updateVSConstantBuffer(cb);
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
