using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomAliveTestApp {
	static class DefaultMeshes {

		public static Vector3[] triangleVertices = new Vector3[] { new Vector3(-0.5f,0.5f,0.0f),new Vector3(0.5f,0.5f,0.0f),new Vector3(0.0f,-0.5f,0.0f) };
		public static Vector3[] quadVertices = new Vector3[] { new Vector3(0,1,0.05f),new Vector3(1,1,0.05f),new Vector3(0,0,0.05f),new Vector3(1,0,0.05f) };

	}
}
