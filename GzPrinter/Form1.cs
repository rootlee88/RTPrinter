using GzPrinter.Controllers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

/***********************************************************************
 *            Project: RTPrinter
 *        ProjectName: 旭日web打印服务
 *             Author: rootlee
 *              Email: 540478668@qq.com
 *         CreateTime: 2023/8/21 8:00:00
 *        Description: 暂无
 ***********************************************************************/

namespace GzPrinter
{
    public partial class Form1 : Form
    {
        //服务对象
        static MyHttpServer httpServer;
        private Icon mNetTrayIcon;
        private NotifyIcon TrayIcon;
        private ContextMenu notifyiconMnu;
        bool _realExitFlag = false;

        public Form1()
        {
            InitializeComponent();
            Type type = GetType();
            mNetTrayIcon = new Icon(type, "printer6.ico");
            //初始化托盘程序的各个要素 
            Initializenotifyicon();
            Icon=mNetTrayIcon;
            RoutesFactory.Init();
            StartHttpServer();
        }

        private void Initializenotifyicon()
        {
            try
            {
                //设定托盘程序的各个属性 
                TrayIcon = new NotifyIcon
                {
                    Icon = mNetTrayIcon,
                    Text = "旭日Web打印服务RTPrinter",
                    Visible = true
                };

                //定义一个MenuItem数组，并把此数组同时赋值给ContextMenu对象 
                MenuItem[] mnuItms = new MenuItem[2];
                mnuItms[0] = new MenuItem { Text = "主界面" };
                mnuItms[0].Click += new EventHandler(MainMenuItemClick);

                mnuItms[1] = new MenuItem { Text = "退出" };
                mnuItms[1].Click += new EventHandler(ExitSelect);
                mnuItms[1].DefaultItem = true;

                notifyiconMnu = new ContextMenu(mnuItms);
                TrayIcon.ContextMenu = notifyiconMnu;
                TrayIcon.DoubleClick += new EventHandler(MainMenuItemClick);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        public void MainMenuItemClick(object sender, System.EventArgs e)
        {
            Show();
        }

        public void ExitSelect(object sender, EventArgs e)
        {
            if (MessageBox.Show("退出后将不能执行Web打印，确认要退出吗？", "退出确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _realExitFlag = true;
                //隐藏托盘程序中的图标 
                TrayIcon.Visible = false;
                //关闭系统 
                Close();
                Application.Exit();
                Environment.Exit(0);
            }
        }

        void StartHttpServer()
        {
            //启动http服务
            httpServer = new MyHttpServer(RoutesFactory.GetRouteKeys());//初始化，传入路由
            httpServer.respNotice += DataHandle;//回调方法，接收到http请求时触发
            httpServer.Start(12333);//端口
        }

        /// <summary>
        /// 收到请求的回调函数
        /// </summary>
        /// <param name="data">客户端请求的数据</param>
        /// <param name="resp">respon对象</param>
        /// <param name="route">网址路径,如/api/test</param>
        /// <param name="request_type">请求类型，get或者post</param>
        void DataHandle(Dictionary<string, string> data, HttpListenerResponse resp, string route = "", string request_type = "get")
        {
            IController controller = RoutesFactory.GetController(route);
            try
            {
                if (controller.NeedAuth())
                {
                    string domain = string.Empty;
                    if (data.ContainsKey("domain"))
                    {
                        domain = data["domain"];
                    }
                    string token = string.Empty;
                    if (data.ContainsKey("token"))
                    {
                        token = data["token"];
                    }
                    string ip = MachineHelper.GetLocalIP4();
                    string mac = MachineHelper.GetMacAddress();
                    AuthService.CheckAuth(token,mac,domain);
                }
                controller.ProcessRequest(httpServer,data,resp,request_type);
            }
            catch(Exception ex)
            {
                try
                {
                    string rsp = JsonConvert.SerializeObject(new { error = ex.Message });
                    httpServer.ResponData(rsp, resp);
                }
                catch { }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //BeginInvoke(new MethodInvoker(delegate
            //{
            //    Hide();
            //}));
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            httpServer.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_realExitFlag)
            {
                Hide();
                e.Cancel = true;
            }
        }

        private void toolStripMenuItemAbout_Click(object sender, EventArgs e)
        {
            FormAbout formAbout = new FormAbout();
            formAbout.ShowDialog();
        }
    }
}
