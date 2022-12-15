using System;
using System.Runtime.InteropServices;

namespace Libraries.RestorableWindow {

	public class NativeMethods {

		[DllImport( "user32.dll" )]
		public static extern bool SetWindowPlacement(
			IntPtr hWnd,
			[In] ref WINDOWPLACEMENT lpwndpl );

		[DllImport( "user32.dll" )]
		public static extern bool GetWindowPlacement(
			IntPtr hWnd,
			out WINDOWPLACEMENT lpwndpl );
	}

	[Serializable]
	[StructLayout( LayoutKind.Sequential )]
	public struct WINDOWPLACEMENT {
		public int length;
		public int flags;
		public SW showCmd;
		public POINT minPosition;
		public POINT maxPosition;
		public RECT normalPosition;
	}

	[Serializable]
	[StructLayout( LayoutKind.Sequential )]
	public struct POINT {
		public int X;
		public int Y;

		public POINT( int x, int y ) {
			this.X = x;
			this.Y = y;
		}
	}

	[Serializable]
	[StructLayout( LayoutKind.Sequential )]
	public struct RECT {
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;

		public RECT( int left, int top, int right, int bottom ) {
			this.Left = left;
			this.Top = top;
			this.Right = right;
			this.Bottom = bottom;
		}
	}

	public enum SW {
		Hide = 0,
		ShowNormal = 1,
		ShowMinimized = 2,
		ShowMaximized = 3,
		ShowNoActivate = 4,
		Show = 5,
		Minimize = 6,
		ShowMinNoActive = 7,
		ShowNA = 8,
		Restore = 9,
		ShowDEFAULT = 10,
	}
}