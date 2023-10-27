﻿/***********************************************************************
 *            Project: RTPrinter
 *        ProjectName: 旭日web打印服务
 *             Author: rootlee
 *              Email: 540478668@qq.com
 *         CreateTime: 2023/8/21 8:00:00
 *        Description: 暂无
 ***********************************************************************/
(function(){function x(a,c,f){c=f.pagerNumStyle;return c=c.replace(/#p/g,a)}function m(a){this.init(a)}if(!window.jQuery){var n=document.createElement("script");n.src="http://cdn.dadang.cn/jquery/1.9.1/jquery.min.js";document.getElementsByTagName("head")[0].appendChild(n)}var w={rtp_header:".rtp-header",rtp_footer:".rtp-footer",rtp_item:".rtp-item",font:{name:"Verdana, Geneva, sans-serif",size:"12pt"},printer:{name:"",paperName:"",pageWidth:210,pageHeight:297,dpi:96,landscape:!1,copies:1},margins:{top:40,bottom:40,left:40,right:40},usePager:!0,pagerNumStyle:"\u7b2c#p\u9875/\u5171#P\u9875",preview:!1,tableBorder:!0,appendTheadPerPage:!0,server:"http://localhost:12333",downloadUrl:"http://shop.wanyouyinli.com/download/rtprinter.zip",token:""};m.prototype={init:function(a){this.settings=$.extend(!0,w,a||{})},print:function(a,c){this.$template=$(a);this.autoPage(a);var f=[];$(".rtp-print-page-wrap .rtp-print-page").each(function(){f.push($(this).get(0).outerHTML)});var b=location.host,b={printer:this.settings.printer.name,copies:this.settings.printer.copies,landscape:this.settings.printer.landscape?1:0,html:JSON.stringify(f),preview:this.settings.preview?1:0,token:this.settings.token,paperName:this.settings.printer.paperName,domain:b};$.ajax({type:"POST",url:this.settings.server+"/print",dataType:"json",contentType:"application/json",processData:!1,data:JSON.stringify(b),success:function(b){console.log(b);c&&c(b)},error:function(b){c&&c({error:b})}})},autoPage:function(a){function c(b){f(y,b);g++;y=b;k=0}function f(a,c){var e=$('\x3cdiv class\x3d"rtp-print-page"\x3e\x3cdiv class\x3d"rtp-print-inner"\x3e\x3c/div\x3e\x3c/div\x3e'),d=e.find(".rtp-print-inner");m.append(e);0==g&&d.append(z.clone());a!=c&&(e=A.clone().append(r.slice(a,c)),!b.settings.appendTheadPerPage&&0<g&&(e=A.clone().find("thead").remove().end().append(r.slice(a,c))),d.append(e));c==t&&(e=u-(k+h),b.settings.appendTheadPerPage||(e+=h),E<=e?d.append(B.clone()):(g++,h=k=0,f(c,c)));b.settings.usePager&&(e=$('\x3cdiv class\x3d"rtp-print-pager" style\x3d"text-align:right;"\x3e\x3c/div\x3e'),e.html(x(g+1,0,b.settings)),d.append(e));c!=t&&d.append("\x3cdiv class\x3d'rtp-page-break' style\x3d'page-break-after:always;'\x3e\x3c/div\x3e")}var b=this,d=$(a);a=$(".rtp-print-content");a.length||(a=$('\x3cdiv class\x3d"rtp-print-content" style\x3d"position:absolute;left:-1000%;"\x3e\x3cdiv class\x3d"rtp-print-page rtp-tpl"\x3e\x3cdiv class\x3d"rtp-print-inner"\x3e\x3c/div\x3e\x3c/div\x3e\x3cdiv class\x3d"rtp-print-page-wrap"\x3e\x3c/div\x3e\x3c/div\x3e'),a.appendTo("body"));var m=a.find(".rtp-print-page-wrap");m.html("");var p=Math.round(1*b.settings.printer.pageWidth/10/2.54*b.settings.printer.dpi),q=Math.round(1*b.settings.printer.pageHeight/10/2.54*b.settings.printer.dpi),n=(b.settings.printer.dpi/96).toFixed(2)+"em";a.find(".rtp-print-page").css({"font-family":b.settings.font.name,"font-size":b.settings.font.size,width:(b.settings.printer.landscape?q:p)+"px"});a.find(".rtp-print-inner").css({"margin-left":b.settings.margins.left+"px","margin-top":b.settings.margins.top+"px","margin-right":b.settings.margins.right+"px","margin-bottom":b.settings.margins.bottom+"px","font-size":n});var l=a.find(".rtp-tpl .rtp-print-inner");l.html(d.clone());d=0;b.settings.usePager&&(d=$('\x3cdiv class\x3d"rtp-print-pager" style\x3d"text-align:right;"\x3e\x3c/div\x3e'),d.html(x(1,1,b.settings)),l.append(d),d=d.outerHeight(!0));var z=l.find(b.settings.rtp_header),B=l.find(b.settings.rtp_footer),v=l.find(b.settings.rtp_item),r=v.find("tbody tr");b.settings.tableBorder||v.find("table,td,th").css({"border-width":0});var u=0,u=b.settings.printer.landscape?p-b.settings.margins.top-b.settings.margins.bottom:q-b.settings.margins.top-b.settings.margins.bottom,w=z.outerHeight(!0),C=v.find("thead"),h=0;C.length&&(h=C.outerHeight(!0));var E=B.outerHeight(!0),F=u-w-h-d,D=u-h-d;b.settings.appendTheadPerPage||(D+=h);var A=v.find("table").clone().find("tbody").remove().end().removeAttr("id"),k=0,y=0,g=0,t=r.length;0<t?r.each(function(b){var a=$(this).outerHeight(!0);$(this).height(a);k+a<(0==g?F:D)?(k+=a,b==t-1&&c(b+1)):(c(b),k+=a)}):c(0);a.find(".rtp-print-pager").each(function(){$(this).html($(this).html().replace(/#P/g,g))});l.html("");b.settings.usePager?a.find(".rtp-print-page").css({"font-family":b.settings.font.name,"font-size":b.settings.font.size,width:(b.settings.printer.landscape?q:p)+"px",height:(b.settings.printer.landscape?p:q)+"px"}):a.find(".rtp-print-page").css({"font-family":b.settings.font.name,"font-size":b.settings.font.size,width:(b.settings.printer.landscape?q:p)+"px"});a.find(".rtp-print-inner").css({"margin-left":b.settings.margins.left+"px","margin-top":b.settings.margins.top+"px","margin-right":b.settings.margins.right+"px","margin-bottom":b.settings.margins.bottom+"px","font-size":n})},getPrinterList:function(a){$.ajax({type:"GET",url:this.settings.server+"/printers",dataType:"json",data:{token:this.settings.token,domain:location.host},success:function(c){a&&a(c)},error:function(c){a&&a({error:c})}})},getPapers:function(a,c){$.ajax({type:"GET",url:this.settings.server+"/paper",dataType:"json",data:{token:this.settings.token,domain:location.host,printer:a},success:function(a){c&&c(a)},error:function(a){c&&c({error:a})}})},getPrinterDpi:function(a,c){$.ajax({type:"GET",url:this.settings.server+"/dpi",dataType:"json",data:{token:this.settings.token,domain:location.host,printer:a},success:function(a){c&&c(a)},error:function(a){c&&c({error:a})}})},check:function(){var a=this,c=!1;$.ajax({type:"GET",url:a.settings.server+"/index?token\x3d"+a.settings.token,dataType:"json",async:!1,timeout:3E3,success:function(a){1==a.code&&(c=!0)},error:function(c){alert("\u672a\u5b89\u88c5RTPrinter\u6253\u5370\u670d\u52a1\uff0c\u70b9\u786e\u5b9a\u5c06\u81ea\u52a8\u4e0b\u8f7d\u5b89\u88c5\u5305\uff0c\u4e0b\u8f7d\u540e\u8bf7\u89e3\u538b\u5b89\u88c5\u3002");c=$('\x3ca href\x3d"'+a.settings.downloadUrl+'" target\x3d"_blank"\x3e\u4e0b\u8f7dRTPrinter\x3c/a\x3e');c.appendTo("body");c[0].click();c.remove()}});return c}};window.getRTP=function(a){return new m(a)}})();