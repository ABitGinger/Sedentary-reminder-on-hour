using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reminder
{
    public partial class MainFrm : Form
    {
        private static WorkFrm wrkFrm;
        private static RestFrm restFrm;
        private static bool isAlignMode = true;
        private static MainFrm instance;

        public static MainFrm Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MainFrm();
                }
                return instance;
            }
        }

        public MainFrm()
        {
            InitializeComponent();
            // 设置默认值
            this.numWrkTime.Value = 27;
            this.numRstTime.Value = 3;
            this.ckBoxInput.Checked = true;
            
            // 启动时隐藏窗口并设置窗口状态
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // 在窗体加载完成后开始计时
            StartTimer(true);
        }

        private void CloseAllTimers()
        {
            // 关闭所有工作计时窗口
            foreach (Form form in Application.OpenForms.OfType<WorkFrm>().ToList())
            {
                try
                {
                    if (!form.IsDisposed)
                    {
                        form.Close();
                    }
                    form.Dispose();
                }
                catch { }
            }

            // 关闭所有休息计时窗口
            foreach (Form form in Application.OpenForms.OfType<RestFrm>().ToList())
            {
                try
                {
                    if (!form.IsDisposed)
                    {
                        form.Close();
                    }
                    form.Dispose();
                }
                catch { }
            }

            // 清理静态引用
            if (wrkFrm != null)
            {
                wrkFrm = null;
            }
            if (restFrm != null)
            {
                restFrm = null;
            }

            // 强制GC回收
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void StartTimer(bool autoAlign = true)
        {
            // 确保关闭并清理之前的计时器
            CloseAllTimers();

            int minutes;
            int seconds;
            if (autoAlign)
            {
                // 获取当前时间到下一个时间点的精确时间
                DateTime now = DateTime.Now;
                int currentMinute = now.Minute;
                int currentSecond = now.Second;
                
                if (currentMinute >= 30)
                {
                    // 距离下一个整点的时间
                    minutes = 59 - currentMinute;
                    seconds = 60 - currentSecond;
                    if (seconds == 60)
                    {
                        seconds = 0;
                        minutes++;
                    }
                }
                else
                {
                    // 距离下一个半点的时间
                    minutes = 29 - currentMinute;
                    seconds = 60 - currentSecond;
                    if (seconds == 60)
                    {
                        seconds = 0;
                        minutes++;
                    }
                }
            }
            else
            {
                // 手动启动时使用设置的时间
                minutes = (int)this.numWrkTime.Value;
                seconds = 0;
            }

            bool input_flag = this.ckBoxInput.Checked;
            wrkFrm = new WorkFrm(minutes, seconds, (int)this.numRstTime.Value, input_flag);
            wrkFrm.Show();
            this.Visible = false;
        }

        private void Btn_start_Click(object sender, EventArgs e)
        {
            CloseAllTimers();
            isAlignMode = false;
            StartTimer(false);
        }

        private void 主窗体ToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            CloseAllTimers();
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void 恢复ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAllTimers();
            isAlignMode = true;
            StartTimer(true);
        }

        private void MainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {            
            //取消关闭窗口
            e.Cancel = true;
            //最小化主窗口
            this.WindowState = FormWindowState.Minimized;
            this.Visible = false;
            //不在系统任务栏显示主窗口图标
            this.ShowInTaskbar = false;
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAllTimers();
            notifyIcon1.Visible = false;
            notifyIcon1.Dispose();
            System.Environment.Exit(0);
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        // 添加静态方法供其他类使用
        public static bool IsAlignMode()
        {
            return isAlignMode;
        }
    }
}
