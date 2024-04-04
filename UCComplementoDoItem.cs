using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysIntegradorApp
{
    public partial class UCComplementoDoItem : UserControl
    {
        public UCComplementoDoItem()
        {
            InitializeComponent();
        }

        public void SetLabels(string nomeComplemento, float valorComplemento)
        {
            labelNomeComplemento.Text = nomeComplemento;
            valorDoComplemento.Text = valorComplemento.ToString("c");
        }
    }
}
