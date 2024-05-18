using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysIntegradorApp.UserControls.UCSDelMatch
{
    public partial class UCComplementoDoItemDelMatch : UserControl
    {
        public UCComplementoDoItemDelMatch()
        {
            InitializeComponent();
        }

        public void SetLabels(string nomeComplemento, string valorComplemento)
        {
            labelNomeComplemento.Text = nomeComplemento;
            valorDoComplemento.Text = "R$"+ " " + valorComplemento.Replace(".", ",");
        }
    }
}
