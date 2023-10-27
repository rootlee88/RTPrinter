using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GzPrinter
{
    [RunInstaller(true)]
    public partial class Installer1 : System.Configuration.Install.Installer
    {
        public Installer1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 重写安装完成后函数
        /// 实现安装完成后自动启动已安装的程序
        /// </summary>
        /// <param name="savedState"></param>
        protected override void OnAfterInstall(IDictionary savedState)
        {
            base.OnAfterInstall(savedState);
            Assembly asm = Assembly.GetExecutingAssembly();
            //网上查到的，安装时报错
            //string path = asm.Location.Remove(asm.Location.LastIndexOf("//")) + "//";
            //System.Diagnostics.Process.Start(path + "//PrintServer.exe");
            //修改后如下
            System.Diagnostics.Process.Start(asm.Location);
        }

        /// <summary>
        /// 重写安装过程方法
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }

        /// <summary>
        /// 重写安装之前方法
        /// </summary>
        /// <param name="savedState"></param>
        protected override void OnBeforeInstall(IDictionary savedState)
        {
            base.OnBeforeInstall(savedState);
        }

        /// <summary>
        /// 重写卸载方法
        /// </summary>
        /// <param name="savedState"></param>
        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
        }

        /// <summary>
        /// 重写回滚方法
        /// </summary>
        /// <param name="savedState"></param>
        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }
    }
}
