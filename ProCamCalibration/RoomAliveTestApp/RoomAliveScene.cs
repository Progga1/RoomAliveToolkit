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

		public class Head {
			public SharpMatrix projection;
			public Vector3 position;

			public Head() {
				position = new Vector3(-1,-0.5f,-0.2f);

				projection = SharpMatrix.PerspectiveFovLH(1.2f,1.2f,0.05f,10);
			}

			public SharpMatrix getView() {
				SharpMatrix result = getWorldTransform();
				result.Invert();
				return result;
			}

			public SharpMatrix getProjection() {
				return projection;
			}

			public SharpMatrix getProjectionTransp() {
				SharpMatrix result = projection;
				result.Transpose();
				return result;
			}

			public SharpMatrix getWorldTransform() {
				SharpMatrix worldTransform = SharpMatrix.Translation(position);
				return worldTransform;
			}

			public SharpMatrix getWorldTransformTransp() {
				SharpMatrix result = getWorldTransform();
				result.Transpose();
				return result;
			}

		}

		public ProjectorCameraEnsemble ensemble;
		public Mesh roomMesh;
		public Head head;

		public RoomAliveScene(string calibrationFile,string meshFile) {
			ensemble = RoomAliveToolkit.ProjectorCameraEnsemble.FromFile(calibrationFile);
			roomMesh = new Mesh();
			roomMesh.flipTexY = true;
			roomMesh.LoadFromOBJFile(meshFile);

			head = new Head();
		}

	}

}
