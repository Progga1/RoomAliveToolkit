using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpMatrix = SharpDX.Matrix;
using RoomMatrix = RoomAliveToolkit.Matrix;

namespace SharpGraphics {

	public static class MatrixOps {

		private static float[] vals = new float[4];
		private static float[] inVals = new float[4];

		public static void applyMat(SharpMatrix mat,ref Vector3 point) {
			inVals[0] = point.X;
			inVals[1] = point.Y;
			inVals[2] = point.Z;
			inVals[3] = 1;

			for(int r=0;r<4;r++) {
				float sum = 0;
				for(int c=0;c<4;c++) {
					sum += mat[r,c]*inVals[c];
				}
				vals[r] = sum;
			}
			float w = 1f/vals[3];
			point.X = vals[0]*w;
			point.Y = vals[1]*w;
			point.Z = vals[2]*w;
		}

		public static Vector3 applyMat(SharpMatrix mat,float x,float y,float z) {
			Vector3 result = new Vector3(x,y,z);
			applyMat(mat,ref result);
			return result;
		}

		public static SharpMatrix getSharpMatrix(RoomMatrix matrix) {
			SharpMatrix mat = new SharpDX.Matrix();
			for(int i = 0; i<16; i++) {
				mat[i] = (float)matrix[i];
			}
			return mat;
		}

	}
}
