using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SysIntegradorApp.ClassesAuxiliares;

namespace SysIntegradorApp
{
    public partial class UCItem : UserControl
    {
        public UCItem()
        {
            InitializeComponent();
            CustomizePanelBorder(panelValorDasOpcoes);
            CustomizePanelBorder(panelValorTotal);
            SetRoundedRegion(this, 24);
        }

        //método que define todas as labels e panel dentro do UCITEM
        public void SetLabels(string nome, float quantity, float unitPrice, float optionsPrice, float totalPrice, List<Options> options)
        {
            quantidadeItem.Text = $"{quantity.ToString()}X";
            nomeDoItem.Text = nome;
            valorDoItem.Text = unitPrice.ToString("c");
            valorDasOpcoes.Text = optionsPrice.ToString("c");
            valorTotalDoItem.Text = totalPrice.ToString("c");

            if(options.Count() == 0)
            {
                panelDeComplementos.Visible = false;
                groupBoxComplementos.Visible = false;
                panelValorDasOpcoes.Location = new Point(groupBoxComplementos.Location.X, groupBoxComplementos.Location.Y);
                panelValorTotal.Location = new Point(panelValorDasOpcoes.Location.X, panelValorDasOpcoes.Location.Y + 90);
                this.Size = new System.Drawing.Size(792, 300);
            }

            foreach (var item in options)
            {
                UCComplementoDoItem ucComplemento = new UCComplementoDoItem();
                ucComplemento.SetLabels(item.name, item.price);
                ucComplemento.Size = new Size(600,60);
                Panel panelParaLeyout = new Panel();  
                panelParaLeyout.Controls.Add(ucComplemento);
                panelParaLeyout.Size = new Size(640, 64);
                CustomizePanelBorder(panelParaLeyout);
                panelDeComplementos.Controls.Add(panelParaLeyout);
            }

        }

        private void SetRoundedRegion(Control control, int radius) //Método para arredondar os cantos dos UserCntrol
        {
            GraphicsPath path = new GraphicsPath();
            int width = control.Width;
            int height = control.Height;
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(width - radius, 0, radius, radius, 270, 90);
            path.AddArc(width - radius, height - radius, radius, radius, 0, 90);
            path.AddArc(0, height - radius, radius, radius, 90, 90);
            path.CloseFigure();

            control.Region = new Region(path);
        }

        private void CustomizePanelBorder(Panel panel) //comando que deixa o panel apenas com uma borda em baixo
        {
            // Definir o estilo da borda para None para que não haja borda padrão
            panel.BorderStyle = BorderStyle.None;

            // Manipular o evento Paint do painel para desenhar uma borda personalizada
            panel.Paint += (sender, e) =>
            {
                int borderWidth = 2; // Largura da borda
                Color borderColor = Color.Black; // Cor da borda

                // Calcular as coordenadas da borda inferior
                int y = panel.Height - borderWidth;
                int x1 = 0;
                int x2 = panel.Width;

                // Desenhar a borda inferior
                using (Pen pen = new Pen(borderColor, borderWidth))
                {
                    e.Graphics.DrawLine(pen, x1, y, x2, y);
                }
            };
        }

      
    }
}
