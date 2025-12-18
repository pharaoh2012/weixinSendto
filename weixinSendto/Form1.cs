using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;

namespace weixinSendto
{
    public partial class Form1 : Form
    {
        // Windows API 声明
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // 热键标识
        const int HOTKEY_ID = 999;

        // 修饰键常量
        const uint MOD_WIN = 0x0008;  // Windows 键
        const uint MOD_SHIFT = 0x0004;  // Shift 键

        // 虚拟键码：'A' 键
        const uint VK_A = 0x41;

        //var simulator = new WindowsInput.Simulate.;

        public Form1()
        {
            InitializeComponent();
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            // 注册 Win + Shift + A
            this.Visible = false;
            bool registered = RegisterHotKey(this.Handle, HOTKEY_ID, MOD_WIN | MOD_SHIFT, VK_A);
            if (!registered)
            {
                Console.WriteLine("❌ 热键注册失败！可能已被其他程序占用。");
                MessageBox.Show(this, "❌ 热键注册失败！可能已被其他程序占用。");
                this.Close();
            }
            else
            {
                Console.WriteLine("✅ 成功注册全局热键: Win + Shift + A");
            }


        }

        protected override void WndProc(ref Message m)
        {

            const int WM_HOTKEY = 0x0312;
            if (m.Msg == WM_HOTKEY)
            {
                if (m.WParam.ToInt32() == HOTKEY_ID)
                {
                    Winxin.SendTo();
                }
            }
            base.WndProc(ref m);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            UnregisterHotKey(this.Handle, HOTKEY_ID);
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
