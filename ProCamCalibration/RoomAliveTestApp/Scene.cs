using RoomAliveTestApp;
using RoomAliveToolkit;
using SharpDX.WIC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using D3D11 = SharpDX.Direct3D11;
using D3DDevice = SharpDX.Direct3D11.Device;
using D3DDeviceContext = SharpDX.Direct3D11.DeviceContext;

namespace SharpGraphics {

	abstract class Scene : InputCallbacks,IDisposable {

		protected RenderSurface surface;
		protected D3DDevice device;
		protected D3DDeviceContext context;

		protected abstract void PostInit();

		public void Init(RenderSurface surface) {
			this.surface = surface;
			this.device = surface.device;
			this.context = surface.context;
			PostInit();
		}

		public RoomMesh loadObj(string filename) {
			RoomMesh mesh = new RoomMesh(device);
			mesh.loadObj(filename);
			return mesh;
		}

		public virtual void OnDraw() {

		}

		public void RenderCallback(D3DDeviceContext context,RenderSurface sender) {
			OnDraw();
		}

		public virtual void RawEvent(InputEvent ev) {

		}

		public virtual void MouseDown(NormMouseEvent ev) {
			
		}

		public virtual void MouseMove(NormMouseEvent ev) {
			
		}

		public virtual void MouseDrag(NormMouseEvent ev) {
			
		}

		public virtual void MouseUp(NormMouseEvent ev) {
			
		}

		public virtual void MouseWheel(MouseWheelEvent ev) {
			
		}

		public virtual void KeyDown(KeyEvent ev) {
			
		}

		public virtual void KeyUp(KeyEvent ev) {
			
		}

		public virtual void Dispose() {
			
		}
	}

}
