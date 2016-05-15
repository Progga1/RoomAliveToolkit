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
		public float dx, dy;
		public MouseButtons button;
		public RenderSurface sender;
	}

	public interface InputCallbacks {

		void MouseDown(NormMouseEvent ev);
		void MouseMove(NormMouseEvent ev);
		void MouseDrag(NormMouseEvent ev);
		void MouseUp(NormMouseEvent ev);
		void MouseWheel(int amount,RenderSurface sender);
		void KeyDown(Keys code,RenderSurface sender);
		void KeyUp(Keys code,RenderSurface sender);
	}
}
