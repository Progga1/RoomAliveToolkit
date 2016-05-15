using SharpDX;
using SharpGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharpGraphics {

	class CameraControl : InputCallbacks {

		const float PI = 3.14159265f;

		private Matrix viewTransf;

		public Vector3 position;
		public float alpha = 0.0f, beta = 0.0f, distance = 1;

		private bool shiftDown = false;

		public float distanceFac = 0.2f;

		public CameraControl() {

		}

		public void MouseDown(NormMouseEvent ev) {
			
		}

		public void MouseMove(NormMouseEvent ev) {
			
		}

		public void MouseDrag(NormMouseEvent ev) {
			if(!shiftDown) {
				alpha -= ev.dx;
				beta -= ev.dy;
			} else {
				getViewMatrix();
				Vector4 right = viewTransf.Column1;
				Vector4 up = viewTransf.Column2;
				position -= new Vector3(right.X,right.Y,right.Z)*ev.dx;
				position -= new Vector3(up.X,up.Y,up.Z)*ev.dy;

				Console.WriteLine("Right: "+right);
				Console.WriteLine("POS: "+position);
			}
			
		}

		public void MouseUp(NormMouseEvent ev) {
			
		}

		public void MouseWheel(int amount,RenderSurface sender) {
			distance -= amount*distanceFac;
		}

		public void KeyDown(Keys code,RenderSurface sender) {
			if(code==Keys.ShiftKey)
				shiftDown = true;
		}

		public void KeyUp(Keys code,RenderSurface sender) {
			if(code==Keys.ShiftKey)
				shiftDown = false;
		}

		public Matrix getViewMatrix() {
			Vector3 eye = new Vector3(
				(float)(Math.Cos(beta)*Math.Sin(alpha)),
				(float)Math.Sin(beta),
				 (float)(Math.Cos(beta)*Math.Cos(alpha))
				 )*distance + position;
			Vector3 up = Vector3.Up;
			Matrix.LookAtRH(ref eye,ref position,ref up, out viewTransf);
//			viewTransf.Invert();
			return viewTransf;
		}
	}

}