using Microsoft.Web.WebView2.Core;
using SysIntegradorApp.ClassesAuxiliares.logs;
using SysIntegradorApp.data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysIntegradorApp.Forms;

public partial class FormMapaDelmatch : Form
{
    public FormMapaDelmatch()
    {
        InitializeComponent();
    }

    public async Task StartMapa()
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var dbConfigs = db.parametrosdosistema.FirstOrDefault();

                if (dbConfigs.IntegraIfood)
                {
                    string userDataFolder = "C:\\SysLogicaLogs\\MyAppUserDataFolder";
                    var environment = await CoreWebView2Environment.CreateAsync(null, userDataFolder, null);

                    await webView21.EnsureCoreWebView2Async(environment);

                    string htmlContent = @"<!DOCTYPE html>
                                    <html lang='en'>

                                    <head>
                                        <meta charset='UTF-8'>
                                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                                        <title>Document</title>
                                        <script async src='https://widgets.ifood.com.br/widget.js'></script>
                                        <script>
                                            window.addEventListener('load', () => {
                                                iFoodWidget.init({
                                                    widgetId: 'b1d473d7-d4b1-4ac8-a9ca-935686090095',
                                                    merchantIds: [
                                                        '9362018a-6ae2-439c-968b-a40177a085ea'
                                                    ],
                                                });
                                            });
                                        </script>
                                    </head>

                                    <body>
                                    </body>

                                    </html>";

                    // webViwer.CoreWebView2.NavigateToString(htmlContent);
                    webView21.CoreWebView2.Navigate("https://delmatchdelivery.com/");

                    await Task.Delay(500);

                    List<CoreWebView2Cookie> cookieList = await webView21.CoreWebView2.CookieManager.GetCookiesAsync("https://widgets.ifood.com.br/widget.js");
                    StringBuilder cookieResult = new StringBuilder(cookieList.Count + " cookie(s) received from https://widgets.ifood.com.br/widget.js\n");

                    for (int i = 0; i < cookieList.Count; ++i)
                    {
                        CoreWebView2Cookie cookie = webView21.CoreWebView2.CookieManager.CreateCookieWithSystemNetCookie(cookieList[i].ToSystemNetCookie());
                        cookieResult.Append($"\n{cookie.Name} {cookie.Value} {(cookie.IsSession ? "[session cookie]" : cookie.Expires.ToString("G"))}");
                    }

                }

            }

        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
        }


    }


}
