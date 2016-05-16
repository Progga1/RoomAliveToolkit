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

		protected D3DDevice device;
		protected D3DDeviceContext context;

		static ImagingFactory2 imagingFactory = new ImagingFactory2();

		public MeshShader meshShader;
		public MeshDeviceResources meshDeviceResources;

		public RoomMesh(D3DDevice device) {
			this.device = device;
			this.context = device.ImmediateContext;
		}

		public void loadObj(string filename) {
			Mesh mesh = Mesh.FromOBJFile(filename);
			meshShader = new MeshShader(device);
			meshDeviceResources = new MeshDeviceResources(device,imagingFactory,mesh);
		}

	}
}
