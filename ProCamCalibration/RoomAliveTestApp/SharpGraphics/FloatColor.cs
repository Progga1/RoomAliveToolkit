using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGraphics {

	public struct FloatColor {

		public static FloatColor White = new FloatColor(1,1,1,1);
		public static FloatColor Red = new FloatColor(1,0,0,1);
		public static FloatColor Green = new FloatColor(0,1,0,1);
		public static FloatColor Blue = new FloatColor(0,0,1,1);
		public static FloatColor Black = new FloatColor(0,0,0,1);

		public float r,g,b,a;

		public FloatColor(float r,float g,float b) : this() {
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = 1;
		}

		public FloatColor(float r,float g,float b,float a) : this() {
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}

		public static FloatColor operator *(float factor,FloatColor color) {
			return new FloatColor(factor*color.r,factor*color.g,factor*color.b,color.a);
		}

		public static FloatColor operator *(FloatColor color,float factor) {
			return new FloatColor(factor*color.r,factor*color.g,factor*color.b,color.a);
		}
	}

}
