using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;


namespace SysIntegradorApp.Forms
{
    public partial class FormWebBrowser : Form
    {
        public FormWebBrowser()
        {
            InitializeComponent();
        }

        private void FormWebBrowser_Load(object sender, EventArgs e)
        {
          //  InitBrowser();
        }

        private async Task Initialized()
        {
            await webView21.EnsureCoreWebView2Async(null);
            webView21.CoreWebView2.Navigate("https://google.com.br/");
        }

        private async void InitBrowser()
        {
          
        }
    }
}
