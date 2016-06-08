using RoomAliveToolkit;
using SharpDX;
using System;
using SharpMatrix = SharpDX.Matrix;
using D3DDevice = SharpDX.Direct3D11.Device;
using D3DDeviceContext = SharpDX.Direct3D11.DeviceContext;
using SharpGraphics.Shaders;

namespace SharpGraphics {

	abstract class VirtualSceneBase : InputCallbacks,IDisposable {

		public const float PI = 3.1415926535f;

		protected RenderSurface surface;
		protected GFX graphics;
		protected D3DDevice device;
		protected D3DDeviceContext context;
		protected PointLight pointLight = new PointLight();

		protected SharpDX.Matrix mvp;
		protected SharpDX.Matrix mvpTransp;

		protected SingleColorShader singleColorShader;
		protected PosUVColorShader posUVColorShader;

		protected abstract void PostInit();

		public void Init(RenderSurface surface) {
			this.surface = surface;
			this.device = surface.device;
			this.context = surface.context;
			this.graphics = surface.graphics;
			pointLight.position = new Vector3(0,2,0);
			pointLight.Ia = new Vector3(0.1f,0.1f,0.1f);
			singleColorShader = graphics.singleColorShader;
			posUVColorShader = graphics.posUVColorShader;
			PostInit();
		}

		public RoomMesh loadObj(string filename,bool flipTexY) {
			RoomMesh mesh = new RoomMesh(device);
			mesh.flipTexY = flipTexY;
			mesh.loadObj(filename);
			return mesh;
		}

		public RoomMesh loadObj(string filename) {
			return loadObj(filename,false);
		}

		protected void setMVP(SharpDX.Matrix mvp) {
			this.mvp = mvp;
			this.mvpTransp = mvp;
			this.mvpTransp.Transpose();
		}

		public virtual void DrawContent(SharpMatrix mvpMat) {

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
