/***********************************************************************
 *            Project: RTPrinter
 *        ProjectName: 旭日web打印服务
 *             Author: rootlee
 *              Email: 540478668@qq.com
 *         CreateTime: 2023/8/21 8:00:00
 *        Description: 暂无
 ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Linq;

namespace GzPrinter
{
    /// <summary>
    /// 图片打印
    /// </summary>
    public class PrintHelper
    {
        private PaperSize _paperSize = null;
       // 当前打印页码
        private int printNum = 0;
        private List<Image> _imgs = new List<Image>();
        private const int HeadHeight = 0;
        //表头字体
        private readonly Font _headFont = new Font("Verdana", 20, FontStyle.Bold);
        private readonly SolidBrush _drawBrush = new SolidBrush(Color.Black);
        private string _headText = string.Empty;
        private int _pBottom;
        private int _pLeft;
        private int _pRight;
        private int _pTop;
        private int _pWidth;
        private int _pHeigh;
        private bool _landscape = false;

        [DllImportAttribute("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minimumWorkingSetSize, int maximumWorkingSetSize);

        public static void FlushMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
        }

        public static async Task<Image> HtmlToImage(string html, int width = 1024, int height = 768)
        {
            var taskCompletionSource = new TaskCompletionSource<Image>();
            var thread = new Thread(() => {
                var browser = new WebBrowserEx { Width = width, Height = height, ScrollBarsEnabled = false };
                browser.DocumentCompleted += (s, e) => {
                    var b = s as WebBrowserEx;
                    if (b == null) { return; }
                    int sh = int.Parse(b.Document.Body.GetAttribute("scrollHeight"));
                    int sw = int.Parse(b.Document.Body.GetAttribute("scrollWidth"));
                    b.Size = new Size(sw, sh);
                    var bmp = new Bitmap(b.Width, b.Height);
                    b.DrawToBitmap(bmp, new Rectangle(0, 0, b.Width, b.Height));
                    //bmp.Save(AppDomain.CurrentDomain.BaseDirectory + "\\"+Guid.NewGuid()+".jpg");
                    taskCompletionSource.SetResult(bmp);
                    //Application.ExitThread();
                    b.Dispose();
                };
                string content = @"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"" />
    <title>打印</title>
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1,user-scalable=0"">
    <style type=""text/css"">
       html,body{margin:0;padding:0;}
       body{font-family:Verdana, Geneva, sans-serif;font-size:12pt;}
       table{width:100%;}
       table{border:1px solid #000;border-collapse: collapse;}
       td,th{border:1px solid #000;}
    </style>
</head>
<body>";
                content += html;
                content += "</body></html>";
                //browser.Navigate("about:blank");
                browser.DocumentText = content;
                //Application.Run();
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return await taskCompletionSource.Task;
        }

        public static Image HtmlToImage2(string html, int width = 1024, int height = 768)
        {
            Image ret=null;
            var thread = new Thread(() => {
                var browser = new WebBrowserEx {
                    Width = width,
                    Height = height,
                    ScrollBarsEnabled = false,
                    AllowNavigation=false
                };
                browser.DocumentCompleted += (s, e) => {
                    var b = s as WebBrowserEx;
                    if (b == null) { return; }
                    int sh = int.Parse(b.Document.Body.GetAttribute("scrollHeight"));
                    int sw = int.Parse(b.Document.Body.GetAttribute("scrollWidth"));
                    b.Size = new Size(sw, sh);
                    var bmp = new Bitmap(b.Width, b.Height);
                    b.DrawToBitmap(bmp, new Rectangle(0, 0, b.Width, b.Height));
                    //bmp.Save(AppDomain.CurrentDomain.BaseDirectory + "\\"+Guid.NewGuid()+".jpg");
                    ret = bmp;
                };
                string content = @"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"" />
    <title>打印</title>
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1,user-scalable=0"">
    <style type=""text/css"">
       html,body{margin:0;padding:0;}
       body{font-family:Verdana, Geneva, sans-serif;font-size:12pt;}
       table{width:100%;}
       table{border:1px solid #000;border-collapse: collapse;}
       td,th{border:1px solid #000;}
    </style>
</head>
<body>";
                content += html;
                content += "</body></html>";
                browser.DocumentText = content;
                while (browser.ReadyState != WebBrowserReadyState.Complete)
                {
                    Application.DoEvents();
                }
                browser.DocumentText = "";
                browser.Dispose(true);
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            FlushMemory();
            return ret;
        }

        /// <summary>
        /// 将图片按页面大小自动裁剪为多张图片
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public List<Image> CutPages(Image image)
        {
            List<Image> images= new List<Image>();
            //将图片尺寸改为和纸张同宽
            int w = _pWidth - _pLeft - _pRight;
            if (image.Width > w)
            {
                int newWidth = w;
                int newHeight = w * image.Height / image.Width;
                image = Resize(image, newWidth, newHeight);
            }
            if (image.Height < _pHeigh)
            {
                images.Add(image);
            }
            else
            {
                int perHeight = _pHeigh - _pTop-_pBottom;
                int count = (int)Math.Ceiling((decimal)image.Height / perHeight);
                int y = 0;
                for(int i = 0; i < count; i++)
                {
                    Rectangle srcRange = new Rectangle(0, y, image.Width, perHeight);
                    Rectangle descRange = new Rectangle(0, 0, image.Width, perHeight);
                    Image bitmap = new Bitmap(descRange.Width, descRange.Height);
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        //设置高质量插值法
                        g.InterpolationMode = InterpolationMode.High;
                        //设置高质量,低速度呈现平滑程度
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        //清空画布并以透明背景色填充
                        g.Clear(Color.Transparent);
                        //在指定位置并且按指定大小绘制原图片的指定部分
                        g.DrawImage(image, descRange, srcRange, GraphicsUnit.Pixel);
                    }
                    y += perHeight;
                    images.Add(bitmap);
                }
            }
            return images;
        }

        private Image Resize(Image iSource, int newWidth, int newHeight)
        {
            Bitmap ob = new Bitmap(newWidth, newHeight);
            using (Graphics graph = Graphics.FromImage(ob))
            {
                graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graph.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graph.CompositingQuality = CompositingQuality.HighQuality;
                graph.SmoothingMode = SmoothingMode.AntiAlias;
                graph.DrawImage(iSource, new Rectangle(0, 0, newWidth, newHeight));
            }
            return ob;
        }

    public void Print(string title,
            List<string> htmls,
            bool previewFlag = false,
            string printerName = "",
            bool landscape = false,
            short copies=1,
            string paperName = "")
        {
            List<Image> images = new List<Image>();
            foreach(var html in htmls)
            {
                var img = HtmlToImage2(html, 300, 300);
                images.Add(img);
            }
            Print(title, images, previewFlag,printerName, landscape,copies,paperName);
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="title"></param>
        /// <param name="imgs"></param>
        /// <param name="previewFlag"></param>
        /// <param name="printerName"></param>
        /// <param name="landscape">是否横向，true:横向 false:纵向</param>
        public void Print(string title,
            List<Image> imgs,
            bool previewFlag=false, 
            string printerName = "",
            bool landscape=false,
            short copies=1,
            string paperName="")
         {
            if(imgs==null || imgs.Count==0) return;
            printNum = 0;

            _headText = title;
            _landscape = landscape;

            // 打印机对象
            PrintDocument imgToPrint = new PrintDocument();
            imgToPrint.DocumentName= title;

            #region 打印机相关设置
            if (!string.IsNullOrEmpty(printerName))
            {
                imgToPrint.PrinterSettings.PrinterName = printerName;
            }
            imgToPrint.PrinterSettings.Copies = copies;

            imgToPrint.DefaultPageSettings.Margins.Left = 0;
            imgToPrint.DefaultPageSettings.Margins.Top = 0;
            imgToPrint.DefaultPageSettings.Margins.Bottom = 0;
            imgToPrint.DefaultPageSettings.Margins.Right = 0;
            //var pageSize = imgToPrint.PrinterSettings.PaperSizes;
            //paperSize = pageSize[pageSize.Count - 1];
            //paperSize = imgToPrint.DefaultPageSettings.PaperSize;
            if (!string.IsNullOrEmpty(paperName))
            {
                var papers = imgToPrint.PrinterSettings.PaperSizes;
                foreach (PaperSize paper in papers)
                {
                    if (paper.PaperName.Equals(paperName, StringComparison.OrdinalIgnoreCase))
                    {
                        imgToPrint.DefaultPageSettings.PaperSize= paper;
                        _paperSize = paper;
                        break;
                    }
                }
            }
            //imgToPrint.DefaultPageSettings.PaperSize=new PaperSize()
            _pLeft = imgToPrint.DefaultPageSettings.Margins.Left;
            _pRight = imgToPrint.DefaultPageSettings.Margins.Right;
            _pTop = imgToPrint.DefaultPageSettings.Margins.Top;
            _pBottom = imgToPrint.DefaultPageSettings.Margins.Bottom;
            _pWidth = imgToPrint.DefaultPageSettings.Bounds.Width;
            _pHeigh = imgToPrint.DefaultPageSettings.Bounds.Height;

            //自动裁剪图片
            _imgs.Clear();
            foreach (var img in imgs)
            {
                //_imgs.AddRange(CutPages(img));
                _imgs.Add(img);
            }

            // 打印方向设置
            imgToPrint.DefaultPageSettings.Landscape = landscape;
            // 打印纸张大小设置
            //imgToPrint.DefaultPageSettings.PaperSize = paperSize;
            // 打印分辨率设置
            //imgToPrint.DefaultPageSettings.PrinterResolution.Kind = PrinterResolutionKind.High;
            // 打印边距设置
            //imgToPrint.DefaultPageSettings.Margins = new Margins(40, 40, 40, 40);

            // 打印开始事件
            imgToPrint.BeginPrint += new PrintEventHandler(this.imgToPrint_BeginPrint);
             //打印结束事件
             imgToPrint.EndPrint += new PrintEventHandler(this.imgToPrint_EndPrint);
             // 打印内容设置
             imgToPrint.PrintPage += new PrintPageEventHandler(this.imgToPrint_PrintPage);
            #endregion

            imgToPrint.PrintController = new StandardPrintController();

            if (previewFlag)
            {
                // 预览打印
                Task.Run(() => {
                    using (PrintPreviewDialog pvDialog = new PrintPreviewDialog())
                    {
                        pvDialog.WindowState = FormWindowState.Maximized;
                        pvDialog.TopMost = true;
                        pvDialog.Document = imgToPrint;
                        pvDialog.ShowDialog();
                        FlushMemory();
                    }
                });
            }
            else
            {
                
                imgToPrint.Print();
                FlushMemory();
            }

             // 打印弹框确认
             //PrintDialog printDialog = new PrintDialog();
             //printDialog.AllowSomePages = true;
            //printDialog.ShowHelp = true;
            //printDialog.Document = imgToPrint;
            //if (printDialog.ShowDialog() == DialogResult.OK)
            //{
            //    imgToPrint.Print();
            //}

            // 预览打印
            //PrintPreviewDialog pvDialog = new PrintPreviewDialog();
            //pvDialog.Document = imgToPrint;
            //pvDialog.ShowDialog();
        }

        public List<string> GetInstalledPrinters()
        {
            List<string> items = new List<string>();
            for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
            {
                items.Add(PrinterSettings.InstalledPrinters[i]);
            }
            return items;
        }

        public List<PaperSize> GetPaperSizes(string printer)
        {
            PrintDocument doc = new PrintDocument();
            doc.PrinterSettings.PrinterName = printer;
            var items = new List<PaperSize>();
            var arr = doc.PrinterSettings.PaperSizes;
            foreach (PaperSize item in arr)
            {
                items.Add(item);
            }
            return items;
        }

        public Tuple<int,int> GetDpi(string printer)
        {
            PrintDocument doc = new PrintDocument();
            if (!string.IsNullOrEmpty(printer))
            {
                doc.PrinterSettings.PrinterName = printer;
            }
            var g=doc.PrinterSettings.CreateMeasurementGraphics();
            return new Tuple<int, int>((int)g.DpiX, (int)g.DpiY);
        }

        /// <summary>
         /// 打印开始事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imgToPrint_BeginPrint(object sender, PrintEventArgs e)
        {
             //if (imageList.Count == 0)
                 //imageList.Add(imageFile);
        }

        /// <summary>
        /// 打印结束事件
        /// </summary>
        /// <param name="sender"></param>
         /// <param name="e"></param>
         private void imgToPrint_EndPrint(object sender, PrintEventArgs e)
         {

         }

        /// <summary>
        /// 设置打印内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imgToPrint_PrintPage(object sender, PrintPageEventArgs e)
        {
            //打印表头
            var subtitleHeight = 0;
            //if (!string.IsNullOrEmpty(_headText))
            //{
            //    var font = new StringFormat { Alignment = StringAlignment.Center };
            //    //打印表头
            //    var arr = _headText.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            //    if (arr.Length > 0)
            //    {
            //        e.Graphics.DrawString(arr[0],
            //            _headFont,
            //            _drawBrush,
            //            new Point(e.PageBounds.Width / 2, _pTop), font);
            //    }
            //    //副标题
            //    for (int i = 1; i < arr.Length; i++)
            //    {
            //        e.Graphics.DrawString(arr[i],
            //            new Font("Verdana", 12, FontStyle.Regular),
            //            _drawBrush,
            //            new Point(e.PageBounds.Width / 2, _pTop + _headFont.Height),
            //            font);
            //        subtitleHeight += new Font("Verdana", 12, FontStyle.Regular).Height;
            //    }
            //}
            //图片文本质量
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
             // 图片插值质量
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
             // 图片合成质量
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            // 图片抗锯齿
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
             // 设置缩放比例
            //e.Graphics.PageScale = 0.3F;
            // 设置打印内容的边距
            //e.PageSettings.Margins = new Margins(40, 40, 40, 40);
            // 设置是否横向打印
            e.PageSettings.Landscape = _landscape;
            // 设置纸张大小
            //e.PageSettings.PaperSize = _paperSize;
            // 设置打印分辨率
            //e.PageSettings.PrinterResolution.Kind = PrinterResolutionKind.High;
            //  using (Stream fs = new FileStream(imageList[printNum].ToString().Trim(), FileMode.Open, FileAccess.Read))
            //  {
            //      System.Drawing.Image image = System.Drawing.Image.FromStream(fs);
            //      int w = image.Width;
            //      int h = image.Height;
            //      // 绘制Image
            //      e.Graphics.DrawImage(image, 40, 40, 410, 600);
            //      if (printNum < imageList.Count - 1)
            //      {
            //          printNum++;
            //          // HasMorePages为true则再次运行PrintPage事件
            //        e.HasMorePages = true;
            //          return;
            //      }
            //    e.HasMorePages = false;
            //}
            Image image = _imgs[printNum];
            int w = image.Width;
            int h = image.Height;
            // 绘制Image
            int y = _pTop;
            if (printNum == 0)
            {
                y = _pTop + HeadHeight + subtitleHeight;
            }
            //像素转换为英寸
            float dpiX = e.Graphics.DpiX;
            float dpiY = e.Graphics.DpiY;
            int incWidth = (int)Math.Round(w / dpiX * 100,0);
            int incHeight = (int)Math.Round(h / dpiY * 100,0);
            e.Graphics.DrawImage(image,new Rectangle(0,0,incWidth,incHeight),new Rectangle(0,0,w,h),GraphicsUnit.Pixel);
            if (printNum < _imgs.Count - 1)
            {
                printNum++;
                // HasMorePages为true则再次运行PrintPage事件
                e.HasMorePages = true;
                return;
            }
            printNum = 0;
            e.HasMorePages = false;
         }
     }
}
