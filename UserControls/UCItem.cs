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
            ClsEstiloComponentes.CustomizePanelBorder(panelValorDasOpcoes);
            ClsEstiloComponentes.CustomizePanelBorder(panelValorTotal);
            ClsEstiloComponentes.SetRoundedRegion(this, 24);
        }

        //método que define todas as labels e panel dentro do UCITEM
        public void SetLabels(string nome, float quantity, float unitPrice, float optionsPrice, float totalPrice, List<Options> options, UCItem instancia, Items itemIfood)
        {
            quantidadeItem.Text = $"{quantity.ToString()}X";
            nomeDoItem.Text = nome;
            valorDoItem.Text = unitPrice.ToString("c");
            valorDasOpcoes.Text = optionsPrice.ToString("c");
            valorTotalDoItem.Text = totalPrice.ToString("c");

            int currentY = 0; // Variável para controlar a posição Y dos controles adicionados
            int maxHeight = 0; // Variável para armazenar a altura máxima dos controles adicionados

            bool ePizza = itemIfood.externalCode == "G" || itemIfood.externalCode == "M" || itemIfood.externalCode == "P" || itemIfood.externalCode == "B" ? true : false;

            if (!ePizza)
            {
                bool ExisteExternalCode = ClsDeIntegracaoSys.VerificaSeExisteProdutoComExternalCode(itemIfood.externalCode);

                if (!ExisteExternalCode)
                {
                    instancia.MudaPictureBoxDeAvisoExternalCode(instancia);
                }
            }

            if (options.Count() == 0)
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
                ucComplemento.Size = new Size(600, 60);

                if (!item.externalCode.Contains("m") && ePizza)
                {
                    bool ExisteExternalCode = ClsDeIntegracaoSys.VerificaSeExisteProdutoComExternalCode(item.externalCode);

                    if (!ExisteExternalCode)
                    {
                        instancia.MudaPictureBoxDeAvisoExternalCode(instancia);
                    }
                }

                Panel panelParaLeyout = new Panel();
                panelParaLeyout.Controls.Add(ucComplemento);
                panelParaLeyout.Size = new Size(640, 64);

                ClsEstiloComponentes.CustomizePanelBorder(panelParaLeyout);
                panelDeComplementos.Controls.Add(panelParaLeyout);

                panelDeComplementos.Height += 30;
                groupBoxComplementos.Height += 30;

                panelValorDasOpcoes.Location = new Point(panelValorDasOpcoes.Location.X, panelValorDasOpcoes.Location.Y + 30);
                panelValorTotal.Location = new Point(panelValorTotal.Location.X, panelValorTotal.Location.Y + 30);
            }

        }

        public void MudaPictureBoxDeAvisoExternalCode(UCItem instancia)
        {
            instancia.pictureBoxExternalCode.Visible = true;
        }

    }
}
