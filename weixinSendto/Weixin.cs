using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput.Events;
using WindowsInput.Native;

namespace weixinSendto
{
    internal class Weixin
    {
        private static int[] tmp;

        public static void initTmp(int[] t)
        {
            tmp = t;
        }

        public static async Task<string> SendTo()
        {
            // Log.Write("Weixin SendTo Start");
            // 1. 获取当前鼠标位置
            Point cursorPos = Cursor.Position;

            await WindowsInput.Simulate.Events().Release(KeyCode.LWin).Release(KeyCode.LShift).Release(KeyCode.A).Invoke();
            // .Release(KeyCode.LShift)

            await WindowsInput.Simulate.Events().MoveTo(cursorPos.X, cursorPos.Y).Click(ButtonCode.Right).Invoke();

            var dx = Properties.Settings.Default.dx;
            var dy = Properties.Settings.Default.dy;
            var imgWidth = Properties.Settings.Default.imgWidth;
            var imgHeight = Properties.Settings.Default.imgHeight;

            // MouseClicker.RightClick(cursorPos.X, cursorPos.Y);
            Rectangle rect = new Rectangle(cursorPos.X + dx, cursorPos.Y + dy, imgWidth, imgHeight);

            Thread.Sleep(500);
            // Log.Write("Weixin SendTo CaptureScreen");
            // 2. 截取屏幕区域
            Bitmap screenshot = CaptureScreen(rect);

            // Log.Write("Weixin SendTo BinarizeAndCountBlackPixels");
            // 3. 二值化并统计每行黑点数量
            int[] blackPixelCounts = BinarizeAndCountBlackPixels(screenshot);

            try
            {
                int index = find(blackPixelCounts);
                if (index != -1)
                {
                    Console.WriteLine("找到:" + index);
                    await WindowsInput.Simulate.Events().MoveTo(cursorPos.X + dx + imgWidth / 2, cursorPos.Y + dy + index + 5).Click(ButtonCode.Left).Invoke();
                    // MouseClicker.Click(cursorPos.X + 5 + 40, cursorPos.Y + 5 + index + 5);
                    Thread.Sleep(1000);
                    await WindowsInput.Simulate.Events().Click(Properties.Settings.Default.key).Click(KeyCode.Return).Wait(500).Invoke();
                    //SendKeys.Send("temp");
                    //SendKeys.Send("{ENTER}"); // 模拟回车键
                    //Thread.Sleep(500);
                    var pt = WindowHelper.GetCurrentWindowLeftTop();
                    if (pt != null)
                    {
                        await WindowsInput.Simulate.Events()
                            .MoveTo(pt.Value.X + Properties.Settings.Default.search_x, pt.Value.Y + Properties.Settings.Default.search_y) // 移动到搜索的第一个结果
                            .Click(ButtonCode.Left).Wait(500)
                            .MoveTo(pt.Value.X + Properties.Settings.Default.sendbtn_x, pt.Value.Y + Properties.Settings.Default.sendbtn_y) // 移动到发送按钮位置
                            .Click(ButtonCode.Left).Wait(500).MoveTo(cursorPos.X, cursorPos.Y)

                            .Invoke();
                        //MouseClicker.Click(pt.Value.X + 140, pt.Value.Y + 125); // 选择temp联系人
                        //Thread.Sleep(500);
                        //MouseClicker.Click(pt.Value.X + 420, pt.Value.Y + 520); // 点击确定
                    }
                    return null;
                }
                else
                {
                    var ptText = string.Join(",", blackPixelCounts);
                    string result = Regex.Replace(ptText, @"(0,){3,}", "\n");
                    Log.Write("未找到匹配项:" + result);
                    Console.WriteLine("未找到匹配项");
                    screenshot.Save("log\\screenshot.png", ImageFormat.Png);
                    return "未找到匹配项";
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message);
                return ex.Message;
            }
            finally
            {
                screenshot.Dispose();
            }
        }

        private static int find(int[] arr)
        {
            // int[] tmp = new int[] { 5, 6, 14, 15, 10, 11, 9, 9, 9, 14, 14, 9 };

            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == 0) continue;
                if (arr[i] == tmp[0])
                {
                    bool find = true;
                    for (int j = 0; j < tmp.Length; j++)
                    {
                        if (Math.Abs(arr[i + j] - tmp[j]) > 2)
                        {
                            find = false;
                            break;
                        }
                    }
                    if (find)
                    {
                        Console.WriteLine("找到");
                        SendKeys.Send("temp");
                        SendKeys.Send("{ENTER}"); // 模拟回车键
                        return i;
                    }
                }
            }

            return -1;
        }

        // 屏幕截图函数
        private static Bitmap CaptureScreen(Rectangle bounds)
        {
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
            }
            // bitmap.Save("screenshot.png", ImageFormat.Png);
            return bitmap;
        }

        // 二值化图像并统计每行黑色像素数
        private static int[] BinarizeAndCountBlackPixels(Bitmap bmp)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            int[] counts = new int[height];

            // 锁定图像数据以提高性能
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height),
                                              ImageLockMode.ReadOnly,
                                              PixelFormat.Format32bppArgb);

            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * height;
            byte[] rgbValues = new byte[bytes];
            Marshal.Copy(ptr, rgbValues, 0, bytes);

            // 每行处理
            for (int y = 0; y < height; y++)
            {
                int count = 0;
                for (int x = 0; x < width; x++)
                {
                    // 计算像素偏移（Format32bppArgb: B G R A）
                    int index = y * bmpData.Stride + x * 4;
                    byte b = rgbValues[index];     // Blue
                    byte g = rgbValues[index + 1]; // Green
                    byte r = rgbValues[index + 2]; // Red

                    // 转灰度（加权平均）
                    int gray = (int)(0.299 * r + 0.587 * g + 0.114 * b);

                    // 二值化：阈值设为 128（可调整）
                    bool isBlack = gray < 128; // 黑色为1，即灰度 < 128 视为黑

                    if (isBlack)
                        count++;
                }
                counts[y] = count;
            }

            bmp.UnlockBits(bmpData);
            return counts;
        }
    }
}
