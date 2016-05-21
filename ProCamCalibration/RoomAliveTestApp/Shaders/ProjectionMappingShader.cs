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

		[StructLayout(LayoutKind.Explicit,Size = 64*3)]
		public unsafe struct VSConstants {
			[FieldOffset(0)]
			public SharpMatrix m;
			[FieldOffset(64)]
			public SharpMatrix vp;
			[FieldOffset(128)]
			public SharpMatrix userVP;
		}

		protected FloatColor color = new FloatColor();

		public ProjectionMappingShader(D3D11.Device device) : base(device,3*16*4,4*4) {
			base.fromFiles("projectionMappingVS.hlsl","projectionMappingPS.hlsl");
			base.setInputElements(GFX.positionInputElements);
		}

		public void passTransformations(SharpMatrix m,SharpMatrix vp,SharpMatrix userVP) {
			var cb = new VSConstants();
			m.Transpose();
			vp.Transpose();
			userVP.Transpose();
			cb.m = m;
			cb.vp = vp;
			cb.userVP = userVP;
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
