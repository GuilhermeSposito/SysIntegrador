using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoOnPedido;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.UserControls.UCSDelMatch;
using SysIntegradorApp.UserControls.UCSOnPedido;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoCCM;

namespace SysIntegradorApp.UserControls.UCSccm;

public partial class UCItemCCM : UserControl
{
    public UCItemCCM()
    {
        InitializeComponent();
        ClsEstiloComponentes.SetRoundedRegion(this, 24);
        ClsEstiloComponentes.CustomizePanelBorder(panelValorDasOpcoes);
        ClsEstiloComponentes.CustomizePanelBorder(panelValorTotal);
    }

    public void SetLabels(string nome, float quantity, float unitPrice, float optionsPrice, float totalPrice, List<Adicional> options , UCItemCCM instancia, Item itemCCM)
    {
        quantidadeItem.Text = $"{quantity.ToString()}X";
        nomeDoItem.Text = nome;
        valorDoItem.Text = itemCCM.ValorUnit.ToString("c");
        valorDasOpcoes.Text = 0.0.ToString("c");
        valorTotalDoItem.Text = itemCCM.ValorUnit.ToString("c");

        int currentY = 0; // Variável para controlar a posição Y dos controles adicionados
        int maxHeight = 0; // Variável para armazenar a altura máxima dos controles adicionados

        bool ePizza = itemCCM.CodPdvGrupo == "G" || itemCCM.CodPdvGrupo == "M" || itemCCM.CodPdvGrupo == "P" || itemCCM.CodPdvGrupo == "B" || itemCCM.CodPdvGrupo == "BB" ? true : false;

        if (!ePizza)
        {
            bool ExisteExternalCode = ClsDeIntegracaoSys.VerificaSeExisteProdutoComExternalCode(itemCCM.CodPdv.ToString());

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
            UCComplementoDoItemDelMatch ucComplemento = new UCComplementoDoItemDelMatch();
            ucComplemento.SetLabels(item.Descricao, item.ValorUnit.ToString());
            ucComplemento.Size = new Size(600, 60);


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

        if(itemCCM.Parte.Count > 0)
        {
            foreach(var item in itemCCM.Parte)
            {
                if (ePizza)
                {
                    bool ExisteExternalCode = ClsDeIntegracaoSys.VerificaSeExisteProdutoComExternalCode(item.CodPdvItem.ToString());

                    if (!ExisteExternalCode)
                    {
                        instancia.MudaPictureBoxDeAvisoExternalCode(instancia);
                    }
                }
            }
        }


    }

    public void MudaPictureBoxDeAvisoExternalCode(UCItemCCM instancia)
    {
        instancia.pictureBoxExternalCode.Visible = true;
    }
}
