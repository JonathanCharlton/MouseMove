using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MouseMover
{
    public class SysTrayApp : Form
    {
        public BackgroundWorker bw = new BackgroundWorker();

        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;
        bool looper = false;
        int sleepPeriod = 250;

        public SysTrayApp()
        {
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += bw_DoWork;

            // Create a simple tray menu with only one item.
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add(new MenuItem("Simulate", start_click));
            trayMenu.MenuItems.Add("Exit", OnExit);

            // Create a tray icon. In this example we use a
            // standard system icon for simplicity, but you
            // can of course use your own custom icon too.
            trayIcon = new NotifyIcon();
            trayIcon.Text = "Mouse Mover";
            trayIcon.Icon = new Icon(Properties.Resources.Iconka_Tailwaggers_Dog_pug, 40, 40);

            // Add menu to tray icon and show it.
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;
        }

        private void start_click(object sender, EventArgs e)
        {       
            IntPtr _hookID = IntPtr.Zero;
            _hookID = InterceptKeys.SetHook(WaitForEscape);

            SimulateMotion();
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.

            base.OnLoad(e);
        }

        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // Release the icon resource.
                trayIcon.Dispose();
            }

            base.Dispose(isDisposing);
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            
            if ((worker.CancellationPending == true))
            {
                e.Cancel = true;
            }
            else
            {
                looper = true;

                while (looper)
                {
                    MouseMoveHelper.MoveTo(0, 0);
                    var y = 0;
                    var x = 0;

                    if (looper)
                    {
                        CursorMoveLoop(false, false, ref y, ref x);
                    }

                    if (looper)
                    {
                        CursorMoveLoop(false, true, ref y, ref x);
                    }

                    if (looper)
                    {
                        CursorMoveLoop(true, false, ref y, ref x);
                    }

                    if (looper)
                    {
                        CursorMoveLoop(true, true, ref y, ref x);
                    }
                }
            }
        }

        private void CursorMoveLoop(bool negate, bool shiftX, ref int y, ref int x)
        {
            for (var localValue = (shiftX) ? x : y; ((negate)?(localValue >= 0):(localValue <= 500)); localValue = (negate) ? localValue - 100 : localValue + 100)
            {
                if (!looper) return;

                MouseMoveHelper.MoveTo((shiftX) ? localValue : x, (shiftX) ? y : localValue);
                x = (shiftX) ? localValue : x;
                y = (shiftX) ? y : localValue;
                System.Threading.Thread.Sleep(sleepPeriod);
            }
        }

        private void SimulateMotion()
        {
            bw.RunWorkerAsync();            
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        public IntPtr WaitForEscape(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)InterceptKeys.WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                if ((Keys)vkCode == Keys.Escape)
                {
                    looper = false;
                }
            }

            return IntPtr.Zero;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SysTrayApp
            // 
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Name = "SysTrayApp";
            this.ResumeLayout(false);

        }
    }
}
