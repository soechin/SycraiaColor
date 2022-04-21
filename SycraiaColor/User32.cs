using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace SycraiaColor
{
  class User32
  {
    #region Window
    [DllImport("user32.dll")]
    static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    static extern ulong GetWindowLongPtr(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    static extern uint SetWindowLong(IntPtr hWnd, int nIndex, uint nValue);

    [DllImport("user32.dll")]
    static extern ulong SetWindowLongPtr(IntPtr hWnd, int nIndex, ulong nValue);

    static void ClickThrough32(IntPtr hWnd, bool enabled)
    {
      const int GWL_EXSTYLE = -20;
      const uint WS_EX_TRANSPARENT = 0x00000020;
      uint style = GetWindowLong(hWnd, GWL_EXSTYLE);

      if (enabled)
      {
        style |= WS_EX_TRANSPARENT;
      }
      else
      {
        style &= ~WS_EX_TRANSPARENT;
      }

      SetWindowLong(hWnd, GWL_EXSTYLE, style);
    }

    static void ClickThrough64(IntPtr hWnd, bool enabled)
    {
      const int GWL_EXSTYLE = -20;
      const uint WS_EX_TRANSPARENT = 0x00000020;
      ulong style = GetWindowLongPtr(hWnd, GWL_EXSTYLE);

      if (enabled)
      {
        style |= WS_EX_TRANSPARENT;
      }
      else
      {
        style &= ~WS_EX_TRANSPARENT;
      }

      SetWindowLongPtr(hWnd, GWL_EXSTYLE, style);
    }

    public static void ClickThrough(Window window, bool enabled)
    {
      IntPtr hwnd = new WindowInteropHelper(window).Handle;

      if (IntPtr.Size == 4)
      {
        ClickThrough32(hwnd, enabled);
      }
      else if (IntPtr.Size == 8)
      {
        ClickThrough64(hwnd, enabled);
      }
    }
    #endregion

    #region Hook
    public delegate IntPtr HOOKPROC(int nCode, IntPtr wParam, IntPtr lParam);

    [StructLayout(LayoutKind.Sequential)]
    public struct KBDLLHOOKSTRUCT
    {
      public int vkCode;
      public int scanCode;
      public int flags;
      public int time;
      public IntPtr dwExtraInfo;
    }

    [DllImport("user32.dll", SetLastError = false)]
    public static extern short GetAsyncKeyState(int vk);

    [DllImport("user32.dll", SetLastError = false)]
    public static extern IntPtr CallNextHookEx(IntPtr hHook, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr SetWindowsHookEx(int idHook, HOOKPROC lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool UnhookWindowsHookEx(IntPtr hHook);
    #endregion
  }
}
