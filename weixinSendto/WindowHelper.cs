using System;
using System.Runtime.InteropServices;
using System.Drawing;

public class WindowHelper
{
    // 导入 Windows API
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    // 定义 RECT 结构体    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }


    public static Point? GetCurrentWindowLeftTop()
    {
        // 获取当前焦点窗口的句柄
        IntPtr hWnd = GetForegroundWindow();
        if (hWnd != IntPtr.Zero)
        {
            // 获取窗口矩形坐标
            if (GetWindowRect(hWnd, out RECT rect))
            {
                int x = rect.Left;
                int y = rect.Top;
                //Console.WriteLine($"焦点窗口左上角坐标: X={x}, Y={y}");
                return new Point(x, y);
            }
            else
            {
                Console.WriteLine("获取窗口坐标失败！");
            }
        }
        else
        {
            Console.WriteLine("未找到焦点窗口！");
        }
        return null;
    }
}