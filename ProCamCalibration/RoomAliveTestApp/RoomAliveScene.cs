using RoomAliveToolkit;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpMatrix = SharpDX.Matrix;

namespace RoomAliveTestApp {

	class RoomAliveScene {

		public ProjectorCameraEnsemble ensemble;
		public Mesh roomMesh;
		public SharpMatrix headProjection;
		public SharpMatrix headPose;

		public RoomAliveScene(string calibrationFile,string meshFile) {
			ensemble = RoomAliveToolkit.ProjectorCameraEnsemble.FromFile(calibrationFile);
			roomMesh = new Mesh();
			roomMesh.flipTexY = true;
			roomMesh.LoadFromOBJFile(meshFile);

			headProjection = SharpMatrix.PerspectiveFovRH(1.5f,1.2f,0.01f,10);
			headPose = SharpMatrix.Translation(-1,-0.5f,-0.2f);
			headPose.Transpose();
			headProjection.Transpose();
			Console.WriteLine(SharpMatrix.Translation(-1,-0.5f,-0.2f));
		}

	}

}
