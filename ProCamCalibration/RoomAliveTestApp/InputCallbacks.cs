using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace SharpGraphics {

	public abstract class InputEvent {
		public const int ACTION_NONE = -1;
		public bool handled = false;
		public RenderSurface sender;

		public virtual bool handle(InputCallbacks callback) {
			callback.RawEvent(this);
			return handled;
		}
	}

	public class NormMouseEvent : InputEvent {

		public const int ACTION_MOUSEDOWN = 0;
		public const int ACTION_MOUSEMOVE = 1;
		public const int ACTION_MOUSEDRAG = 2;
		public const int ACTION_MOUSEUP = 3;

		public int action;
		public float x, y;
		public float dx, dy;
		public MouseButtons button;

		public override bool handle(InputCallbacks callback) {
			if(base.handle(callback))
				return true;
			switch(action) {
				case ACTION_MOUSEDOWN:
					callback.MouseDown(this);
					return handled;
				case ACTION_MOUSEMOVE:
					callback.MouseMove(this);
					return handled;
				case ACTION_MOUSEDRAG:
					callback.MouseDrag(this);
					return handled;
				case ACTION_MOUSEUP:
					callback.MouseDown(this);
					return handled;
			}
			return false;
		}

		public bool isLeft() {
			return button==MouseButtons.Left;
		}

		public bool isMiddle() {
			return button==MouseButtons.Middle;
		}

		public bool isRight() {
			return button==MouseButtons.Right;
		}

	}

	public class KeyEvent : InputEvent {
		public const int ACTION_KEYDOWN = 0;
		public const int ACTION_KEYUP = 1;
		public int action;
		public Keys code;

		public override bool handle(InputCallbacks callback) {
			if(base.handle(callback))
				return true;
			switch(action) {
				case ACTION_KEYDOWN:
					callback.KeyDown(this);
					return handled;
				case ACTION_KEYUP:
					callback.KeyUp(this);
					return handled;
			}
			return false;
		}
	}

	public class MouseWheelEvent : InputEvent {
		public int amount;

		public override bool handle(InputCallbacks callback) {
			if(base.handle(callback))
				return true;
			callback.MouseWheel(this);
			return handled;
		}
	}

	public interface InputCallbacks {
		void RawEvent(InputEvent ev);
		void MouseDown(NormMouseEvent ev);
		void MouseMove(NormMouseEvent ev);
		void MouseDrag(NormMouseEvent ev);
		void MouseUp(NormMouseEvent ev);
		void MouseWheel(MouseWheelEvent ev);
		void KeyDown(KeyEvent ev);
		void KeyUp(KeyEvent ev);
	}
}
