using RoomAliveToolkit;
using SharpDX.WIC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D3D11 = SharpDX.Direct3D11;
using D3DDevice = SharpDX.Direct3D11.Device;
using D3DDeviceContext = SharpDX.Direct3D11.DeviceContext;

namespace RoomAliveTestApp {

	class RoomMesh {

		public bool flipTexY = false;

		protected D3DDevice device;
		protected D3DDeviceContext context;

		static ImagingFactory2 imagingFactory = new ImagingFactory2();

		public Mesh mesh;
		public MeshShader meshShader;
		public MeshDeviceResources meshDeviceResources;

		public RoomMesh(D3DDevice device) {
			this.device = device;
			this.context = device.ImmediateContext;
		}

		public RoomMesh loadObj(string filename) {
			mesh = new Mesh();
			mesh.flipTexY = flipTexY;
			mesh.LoadFromOBJFile(filename);
			meshShader = new MeshShader(device);
			meshDeviceResources = new MeshDeviceResources(device,imagingFactory,mesh);
			return this;
		}

		public RoomMesh setMesh(Mesh mesh) {
			this.mesh = mesh;
			flipTexY = mesh.flipTexY;
			meshShader = new MeshShader(device);
			meshDeviceResources = new MeshDeviceResources(device,imagingFactory,mesh);
			return this;
		}

	}
}
