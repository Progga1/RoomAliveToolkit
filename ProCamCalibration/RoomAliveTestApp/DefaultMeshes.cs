using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomAliveTestApp {
	static class DefaultMeshes {

		public static Vector3[] triangleVertices = new Vector3[] { new Vector3(-0.5f,0.5f,0.0f),new Vector3(0.5f,0.5f,0.0f),new Vector3(0.0f,-0.5f,0.0f) };
		public static Vector3[] quadVertices = new Vector3[] { new Vector3(0,1,0),new Vector3(1,1,0),new Vector3(0,0,0),new Vector3(1,0,0) };
		private const float S = 0.5f;
		public static Vector3[] centerCubeVertices = new Vector3[] { new Vector3(-S,S,S),new Vector3(S,S,S),new Vector3(-S,-S,S),new Vector3(S,-S,S),new Vector3(-S,S,-S),new Vector3(S,S,-S),new Vector3(-S,-S,-S),new Vector3(S,-S,-S) };
		public static int[] cubeIndices = new int[] { 0,1,2, 1,3,2,  1,5,3, 5,7,3,  5,4,7, 4,6,7,  4,0,6, 0,2,6,  4,5,1, 1,0,4,  3,7,2, 6,2,7 };
	}
}
