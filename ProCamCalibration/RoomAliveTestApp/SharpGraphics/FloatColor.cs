using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGraphics {

	public struct FloatColor {
		public float r,g,b,a;

		public FloatColor(float r,float g,float b) : this() {
			this.r = r;
			this.g = g;
			this.b = b;
		}

		public FloatColor(float r,float g,float b,float a) : this() {
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}
	}

}
