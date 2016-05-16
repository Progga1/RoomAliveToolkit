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
		private Matrix viewTransfTransp;

		public bool transposed = false;

		public Vector3 position;
		public float alpha = 0.0f, beta = 0.0f, distance = 1;

		public float maxBeta = PI/2-0.001f;

		private bool shiftDown = false;

		public float distanceFac = 0.2f;

		public CameraControl() {

		}

		protected void applyConstraints() {
			while(alpha>2*PI)
				alpha -= 2*PI;
			while(alpha<0)
				alpha += 2*PI;
			if(beta>maxBeta)
				beta = maxBeta;
			if(beta<-maxBeta)
				beta = -maxBeta;
			if(distance<0.01f)
				distance = 0.01f;
		}

		public void RawEvent(InputEvent ev) {

		}

		public void MouseDown(NormMouseEvent ev) {

		}

		public void MouseMove(NormMouseEvent ev) {

		}

		public void MouseDrag(NormMouseEvent ev) {
			if(!shiftDown) {
				alpha -= ev.dx;
				beta -= ev.dy;
				applyConstraints();
			} else {
				getViewMatrix();
				Vector4 right = viewTransf.Column1;
				Vector4 up = viewTransf.Column2;
				position -= new Vector3(right.X,right.Y,right.Z)*ev.dx;
				position -= new Vector3(up.X,up.Y,up.Z)*ev.dy;

//				Console.WriteLine("Right: "+right);
//				Console.WriteLine("POS: "+position);
			}
			
		}

		public void MouseUp(NormMouseEvent ev) {
			
		}

		public void MouseWheel(MouseWheelEvent ev) {
			distance -= ev.amount*distanceFac;
			applyConstraints();
		}

		public void KeyDown(KeyEvent ev) {
			if(ev.code==Keys.ShiftKey)
				shiftDown = true;
		}

		public void KeyUp(KeyEvent ev) {
			if(ev.code==Keys.ShiftKey)
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
			if(transposed) {
				viewTransfTransp = viewTransf;
				viewTransfTransp.Transpose();
				return viewTransfTransp;
			} else {
				return viewTransf;
			}
			
		}
	}

}
