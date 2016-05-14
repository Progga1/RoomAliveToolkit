using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace SharpGraphics {

	public class NormMouseEvent {
		public float x, y;
		public MouseButtons button;
		public RenderSurface sender;
	}

	public interface InputCallbacks {

		void MouseDown(NormMouseEvent mouseEvent);
		void MouseMove(NormMouseEvent mouseEvent);
		void MouseDrag(NormMouseEvent mouseEvent);
		void MouseUp(NormMouseEvent mouseEvent);
		void MouseWheel(int amount,RenderSurface sender);
		void KeyDown(Keys code,RenderSurface sender);
		void KeyUp(Keys code,RenderSurface sender);
	}
}
