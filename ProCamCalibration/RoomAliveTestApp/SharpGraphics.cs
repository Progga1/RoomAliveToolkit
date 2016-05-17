using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using D3D11 = SharpDX.Direct3D11;
using D3DDevice = SharpDX.Direct3D11.Device;
using D3DDeviceContext = SharpDX.Direct3D11.DeviceContext;

namespace SharpGraphics {

	public class SharpGraphics {

		const int MAX_VERTICES = 100000;

		public Vector3[] positions = new Vector3[MAX_VERTICES];
		public int[] indices = new int[MAX_VERTICES*2];

		D3DDevice device;
		D3DDeviceContext context;

		private D3D11.DepthStencilState depthEnabledState;
		private D3D11.DepthStencilState depthDisabledState;

		public SharpGraphics(D3DDevice device) {
			this.device = device;
			this.context = device.ImmediateContext;
			var depthStencilDesc = new D3D11.DepthStencilStateDescription {
				DepthWriteMask = D3D11.DepthWriteMask.All,
				DepthComparison = D3D11.Comparison.Less,
				IsDepthEnabled = true
			};
			depthEnabledState = new D3D11.DepthStencilState(device,depthStencilDesc);
			depthDisabledState = new D3D11.DepthStencilState(device,new D3D11.DepthStencilStateDescription { });
			setDepthEnabled(true);
		}

		public void setDepthEnabled(bool enabled) {
			context.OutputMerger.SetDepthStencilState(enabled ? depthEnabledState : depthDisabledState);
		}

	}

}
