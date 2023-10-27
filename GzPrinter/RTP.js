/***********************************************************************
 *            Project: RTPrinter
 *        ProjectName: 旭日web打印服务
 *             Author: rootlee
 *              Email: 540478668@qq.com
 *         CreateTime: 2023/8/21 8:00:00
 *        Description: 暂无
 ***********************************************************************/
(function () {
    //加载jq
    if (!window.jQuery) {
        var script = document.createElement('script');
        script.src = 'http://cdn.dadang.cn/jquery/1.9.1/jquery.min.js';
        document.getElementsByTagName('head')[0].appendChild(script);
    }

    var defaults = {
        rtp_header: ".rtp-header",
        rtp_footer: ".rtp-footer",
        rtp_item: '.rtp-item',
        font: {
            name:'Verdana, Geneva, sans-serif',
            size: '12pt'
        },
        printer: {
            name: '',
            paperName:'',
            pageWidth: 210,   //单位mm
            pageHeight: 297,
            dpi:96,  //打印机分辨率
            landscape: false,
            copies:1
        },
        margins: {
            top: 40,  //px
            bottom: 40,
            left: 40,
            right:40
        },
        usePager:true,
        pagerNumStyle: "第#p页/共#P页",
        preview: false,
        tableBorder:true,
        appendTheadPerPage: true,
        server: 'http://localhost:12333',
        downloadUrl:'http://shop.wanyouyinli.com/download/rtprinter.zip',
        token:''
    };

    //获取分页样式
    function getPageStyle(currentPage, pageCount,settings) {
        var numStr = settings.pagerNumStyle;
        numStr = numStr.replace(/#p/g, currentPage);
        //numStr = numStr.replace(/#P/g, pageCount);
        return numStr;
    }

    //添加分页符
    function addPageBreak() {
        return "<div class='rtp-page-break' style='page-break-after:always;'></div>";
    }

    //将mm转换为像素
    function convertMM2Px(mm, dpi) {
        return Math.round(((mm*1.0 / 10) / 2.54) * dpi);
    }

    function RTP(options) {
        this.init(options);
    }

    RTP.prototype = {
        init: function (options) {
            var g = this;
            g.settings = $.extend(true, defaults, options || {});
        },
        print: function (template,callback) {
            var g = this;
            g.$template = $(template);
            g.autoPage(template);
            var arr = [];
            $('.rtp-print-page-wrap .rtp-print-page').each(function () {
                arr.push($(this).get(0).outerHTML);
            });
            var domain = location.host;
            var data = {
                printer: g.settings.printer.name,
                copies: g.settings.printer.copies,
                landscape: g.settings.printer.landscape?1:0,
                html: JSON.stringify(arr),
                preview: g.settings.preview ? 1 : 0,
                token: g.settings.token,
                paperName: g.settings.printer.paperName,
                domain:domain
            };
            $.ajax({
                type: 'POST',
                url: g.settings.server + '/print',
                dataType: 'json',
                contentType: 'application/json',
                processData: false,
                data: JSON.stringify(data),
                success: function (rsp) {
                    console.log(rsp);
                    callback && callback(rsp);
                },
                error: function (ex) {
                    callback && callback({ error: ex });
                }
            });
        },
        autoPage: function (template) {
            var g = this;
            var $template = $(template);
            var $rtpPrintContent = $('.rtp-print-content');
            if (!$rtpPrintContent.length) {
                $rtpPrintContent = $('<div class="rtp-print-content" style="position:absolute;left:-1000%;"><div class="rtp-print-page rtp-tpl"><div class="rtp-print-inner"></div></div><div class="rtp-print-page-wrap"></div></div>')
                $rtpPrintContent.appendTo('body');
            }
            var $pageWrap = $rtpPrintContent.find('.rtp-print-page-wrap');
            $pageWrap.html('');
            var pageWidthPx = convertMM2Px(g.settings.printer.pageWidth, g.settings.printer.dpi);
            var pageHeightPx = convertMM2Px(g.settings.printer.pageHeight, g.settings.printer.dpi);
            var fontRem = (g.settings.printer.dpi / 96).toFixed(2) + 'em';
            $rtpPrintContent.find('.rtp-print-page').css({
                "font-family": g.settings.font.name,
                "font-size": g.settings.font.size,
                "width": (g.settings.printer.landscape ? pageHeightPx : pageWidthPx) + "px",
                //"height": (settings.printer.landscape ? pageWidthPx : pageHeightPx) + "px"
            });
            $rtpPrintContent.find('.rtp-print-inner').css({
                "margin-left": g.settings.margins.left + "px",
                "margin-top": g.settings.margins.top + "px",
                "margin-right": g.settings.margins.right + "px",
                "margin-bottom": g.settings.margins.bottom + "px",
                "font-size": fontRem,
            });

            //将要打印的内容复制一份到容器中，从而测量高度
            var $tpl = $rtpPrintContent.find('.rtp-tpl .rtp-print-inner');
            $tpl.html($template.clone());
            //添加一个pager用于测量pager高度
            var pagerHeight = 0;
            if (g.settings.usePager) {
                var $pagerTest = $('<div class="rtp-print-pager" style="text-align:right;"></div>');
                $pagerTest.html(getPageStyle(1, 1, g.settings));
                $tpl.append($pagerTest);
                pagerHeight = $pagerTest.outerHeight(true);
            }

            var $header = $tpl.find(g.settings.rtp_header);
            var $footer = $tpl.find(g.settings.rtp_footer);
            var $item = $tpl.find(g.settings.rtp_item);
            var $trs = $item.find('tbody tr');
            //设置边框
            if (!g.settings.tableBorder) {
                $item.find('table,td,th').css({'border-width':0});
            }
            var contentHeight = 0;
            if (g.settings.printer.landscape) {
                contentHeight = pageWidthPx - g.settings.margins.top - g.settings.margins.bottom;
            } else {
                contentHeight = pageHeightPx - g.settings.margins.top - g.settings.margins.bottom;
            }

            var headHeight = $header.outerHeight(true);
            var $thead = $item.find('thead');
            var theadHeight = 0;
            if ($thead.length) {
                theadHeight = $thead.outerHeight(true);
            }
            var footHeight = $footer.outerHeight(true);

            var firstTBodyHeight = contentHeight - headHeight - theadHeight - pagerHeight;
            var otherTBodyHeight = contentHeight - theadHeight - pagerHeight;
            if (!g.settings.appendTheadPerPage) {
                otherTBodyHeight += theadHeight;
            }

            var $table = $item.find('table');
            var $content = $table.clone().find("tbody").remove().end().removeAttr("id");

            var totalHeight = 0;
            var startLine = 0;
            var currentPage = 0;
            var rowCount = $trs.length;
            if (rowCount > 0) {
                $trs.each(function (i) {
                    var cHeight = $(this).outerHeight(true);
                    $(this).height(cHeight);
                    var h = currentPage == 0 ? firstTBodyHeight : otherTBodyHeight;
                    if ((totalHeight + cHeight) < h) {
                        totalHeight += cHeight;
                        if (i == rowCount - 1) {
                            newPage(i + 1);
                        }
                    } else {
                        //console.log({ totalHeight, h, pagerHeight, theadHeight, contentHeight, headHeight, i });
                        newPage(i);
                        totalHeight += cHeight;  //将本行高度计算上，不然下一页高度少算了1行
                    }
                });
            } else {
                newPage(0);
            }
            //更改页面中的pageCount
            $rtpPrintContent.find('.rtp-print-pager').each(function () {
                $(this).html($(this).html().replace(/#P/g, currentPage));
            });
            //清除掉复制过来的模板
            $tpl.html('');

            if (g.settings.usePager) {
                $rtpPrintContent.find('.rtp-print-page').css({
                    "font-family": g.settings.font.name,
                    "font-size": g.settings.font.size,
                    "width": (g.settings.printer.landscape ? pageHeightPx : pageWidthPx) + "px",
                    "height": (g.settings.printer.landscape ? pageWidthPx : pageHeightPx) + "px"
                });
            } else {
                $rtpPrintContent.find('.rtp-print-page').css({
                    "font-family": g.settings.font.name,
                    "font-size": g.settings.font.size,
                    "width": (g.settings.printer.landscape ? pageHeightPx : pageWidthPx) + "px"
                });
            }

            $rtpPrintContent.find('.rtp-print-inner').css({
                "margin-left": g.settings.margins.left + "px",
                "margin-top": g.settings.margins.top + "px",
                "margin-right": g.settings.margins.right + "px",
                "margin-bottom": g.settings.margins.bottom + "px",
                "font-size": fontRem,
            });

            function newPage(index) {
                createPage(startLine, index);
                currentPage++;
                startLine = index;
                totalHeight = 0;
            }

            function createPage(startLine, offsetLine) {
                var $page = $('<div class="rtp-print-page"><div class="rtp-print-inner"></div></div>');
                var $inner = $page.find('.rtp-print-inner');
                $pageWrap.append($page);
                if (currentPage == 0) {
                    $inner.append($header.clone());
                }
                if (startLine != offsetLine) {
                    var $pageContent = $content.clone().append(getTrRecord(startLine, offsetLine));
                    if (!g.settings.appendTheadPerPage && currentPage>0) {
                        $pageContent = $content.clone().find('thead').remove().end().append(getTrRecord(startLine, offsetLine));
                    }
                    $inner.append($pageContent);
                }
                //最后一页
                if (offsetLine == rowCount) {
                    var leftHeight = contentHeight - (totalHeight + theadHeight);
                    if (!g.settings.appendTheadPerPage) {
                        leftHeight += theadHeight;
                    }
                    if (footHeight <= leftHeight) {
                        $inner.append($footer.clone());
                    } else {
                        currentPage++;
                        totalHeight = 0;
                        theadHeight = 0;
                        createPage(offsetLine, offsetLine);
                    }
                }
                if (g.settings.usePager) {
                    var $pager = $('<div class="rtp-print-pager" style="text-align:right;"></div>');
                    $pager.html(getPageStyle(currentPage + 1, 0, g.settings));
                    $inner.append($pager);
                }
                if (offsetLine != rowCount) {
                    $inner.append(addPageBreak());
                }
            }

            //获取记录
            function getTrRecord(startLine, offsetLine) {
                //return $trs.clone().slice(startLine, offsetLine);
                return $trs.slice(startLine, offsetLine);
            }
        },
        getPrinterList: function (callback) {
            var g = this;
            var domain = location.host;
            var data = {
                token: g.settings.token,
                domain: domain
            }
            $.ajax({
                type: 'GET',
                url: g.settings.server + '/printers',
                dataType: 'json',
                data:data,
                success: function (rsp) {
                    callback && callback(rsp);
                },
                error: function (ex) {
                    callback && callback({ error: ex });
                }
            });
        },
        getPapers: function (printerName, callback) {
            var g = this;
            var domain = location.host;
            var data = {
                token: g.settings.token,
                domain: domain,
                printer: printerName
            }
            $.ajax({
                type: 'GET',
                url: g.settings.server + '/paper',
                dataType: 'json',
                data:data,
                success: function (rsp) {
                    callback && callback(rsp);
                },
                error: function (ex) {
                    callback && callback({ error: ex });
                }
            });
        },
        getPrinterDpi: function (printerName, callback) {
            var g = this;
            var domain = location.host;
            var data = {
                token: g.settings.token,
                domain: domain,
                printer: printerName
            }
            $.ajax({
                type: 'GET',
                url: g.settings.server + '/dpi',
                dataType: 'json',
                data:data,
                success: function (rsp) {
                    callback && callback(rsp);
                },
                error: function (ex) {
                    callback && callback({ error: ex });
                }
            });
        },
        check: function () {
            var g = this;
            var flag = false;
            $.ajax({
                type: 'GET',
                url: g.settings.server + '/index?token=' + g.settings.token,
                dataType: 'json',
                async: false,
                timeout:3000,
                success: function (rsp) {
                    if (rsp.code == 1) {
                        flag = true;
                        //alert('已安装RTPrinter打印服务');
                    }
                },
                error: function (ex) {
                    alert('未安装RTPrinter打印服务，点确定将自动下载安装包，下载后请解压安装。');
                    var $a = $('<a href="' + g.settings.downloadUrl + '" target="_blank">下载RTPrinter</a>')
                    $a.appendTo('body');
                    $a[0].click();
                    $a.remove();
                }
            });
            return flag;
        }
    };

    window.getRTP = function (options) {
        return new RTP(options);
    };
})();