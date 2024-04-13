using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MouseTracker
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        private Timer timer;

        public Form1()
        {
            InitializeComponent();
            timer = new Timer();
            timer.Interval = 50; // Koordinatları her 50 ms'de bir günceller
            timer.Tick += Timer_Tick;
            timer.Start();

            // NotifyIcon nesnesi oluştur ve ayarla
            MouseTracker_ = new NotifyIcon();
            MouseTracker_.Text = "MouseTracker";
            MouseTracker_.Visible = true;

            // NotifyIcon'a çift tıklama olayı ekle
            MouseTracker_.MouseDoubleClick += TrayIcon_MouseDoubleClick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            POINT p;
            if (GetCursorPos(out p))
            {
                Color color = GetColorAt(p.X, p.Y);
                label1.Text = $"X: {p.X}, Y: {p.Y}";
                label2.Text = $"R: {color.R}, G: {color.G}, B: {color.B}";
            }
        }

        private Color GetColorAt(int x, int y)
        {
            Bitmap screenPixel = new Bitmap(1, 1);
            Graphics g = Graphics.FromImage(screenPixel);
            g.CopyFromScreen(x, y, 0, 0, new Size(1, 1));
            Color color = screenPixel.GetPixel(0, 0);
            screenPixel.Dispose();
            g.Dispose();
            return color;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false; // Görev çubuğunda gösterme
            this.Location = Properties.Settings.Default.WindowLocation;
        }

        private void TrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Çift tıklandığında formu göster
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            MouseTracker_.Visible = false;
            Properties.Settings.Default.WindowLocation = this.Location;
            Properties.Settings.Default.Save(); // Ayarları kaydet

            MouseTracker_.Dispose();
            Application.Exit();
        }
    }
}



// Açıklama: Bu uygulama mouse'nin ekran kordinatları ve bulunduğu pixelin RGB kodlarını gösterir.
// Tarih: 13 Nisan 2024
// Yazar: Adem Ulker