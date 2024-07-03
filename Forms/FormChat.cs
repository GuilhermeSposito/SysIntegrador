using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysIntegradorApp.Forms
{
    public partial class FormChat : Form
    {
        public FormChat()
        {
            InitializeComponent();
            InitializedChat();
        }

        public async Task InitializedChat()
        {
            await webView21.EnsureCoreWebView2Async();

            string htmlContent = @"
            <!DOCTYPE html>
            <html lang='en'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>WebView2 Example</title>
                <script async src='https://widgets.ifood.com.br/widget.js'></script>
                <script>
                    window.addEventListener('load', () => {
                        console.log('Window loaded');
                        iFoodWidget.init({
                            widgetId: 'b1d473d7-d4b1-4ac8-a9ca-935686090095',
                            merchantId: '9362018a-6ae2-439c-968b-a40177a085ea',
                        });
                    });
                </script>
            </head>
            <body>
                <h1></h1>
            </body>
            </html>";

            webView21.CoreWebView2.NavigateToString(htmlContent);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
