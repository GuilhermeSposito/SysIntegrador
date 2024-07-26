using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoAnotaAi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysIntegradorApp.UserControls.UCSAnotaAi;

public partial class UCItemANotaAI : UserControl
{
    public UCItemANotaAI()
    {
        InitializeComponent();
        ClsEstiloComponentes.CustomizePanelBorder(panelValorDasOpcoes);
        ClsEstiloComponentes.CustomizePanelBorder(panelValorTotal);
        ClsEstiloComponentes.SetRoundedRegion(this, 24);
    }

    public void SetLabels(string nome, float quantity, float unitPrice, float totalPrice, List<SubItensAnotaAi> options, UCItemANotaAI instancia, ItemAnotaAi ItemAnotaAi)
    {
        quantidadeItem.Text = $"{quantity.ToString()}X";
        nomeDoItem.Text = nome;
        valorDoItem.Text = unitPrice.ToString("c");

        float valorDasOpcoesNumero = 0.0f;
        foreach(var item in options)
        {
            valorDasOpcoesNumero += item.TotalPrice;
        }

        valorDasOpcoes.Text = valorDasOpcoesNumero.ToString("c");
        valorTotalDoItem.Text = totalPrice.ToString("c");

        int currentY = 0; // Variável para controlar a posição Y dos controles adicionados
        int maxHeight = 0; // Variável para armazenar a altura máxima dos controles adicionados

        bool ePizza = ItemAnotaAi.externalCode == "G" || ItemAnotaAi.externalCode == "M" || ItemAnotaAi.externalCode == "P" || ItemAnotaAi.externalCode == "B" ? true : false;

        if (!ePizza)
        {
            bool ExisteExternalCode = ClsDeIntegracaoSys.VerificaSeExisteProdutoComExternalCode(ItemAnotaAi.externalCode);

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
            ucComplemento.SetLabels(item.name, item.Price);
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

    public void MudaPictureBoxDeAvisoExternalCode(UCItemANotaAI instancia)
    {
        instancia.pictureBoxExternalCode.Visible = true;
    }
}
