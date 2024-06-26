﻿using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoDelmatch;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysIntegradorApp.UserControls.UCSDelMatch;

public partial class UCItemDelMatch : UserControl
{
    public UCItemDelMatch()
    {
        InitializeComponent();
        ClsEstiloComponentes.SetRoundedRegion(this,24);
        ClsEstiloComponentes.CustomizePanelBorder(panelValorDasOpcoes);
        ClsEstiloComponentes.CustomizePanelBorder(panelValorTotal);
    }

    public void SetLabels(string nome, float quantity, float unitPrice, float optionsPrice, float totalPrice, List<SubItens> options, UCItemDelMatch instancia, items itemDelMatch)
    {
        quantidadeItem.Text = $"{quantity.ToString()}X";
        nomeDoItem.Text = nome;
        valorDoItem.Text = unitPrice.ToString("c");
        valorDasOpcoes.Text = optionsPrice.ToString("c");
        valorTotalDoItem.Text = totalPrice.ToString("c");

        int currentY = 0; // Variável para controlar a posição Y dos controles adicionados
        int maxHeight = 0; // Variável para armazenar a altura máxima dos controles adicionados

        bool ePizza = itemDelMatch.ExternalCode == "G" || itemDelMatch.ExternalCode == "M" || itemDelMatch.ExternalCode == "P" || itemDelMatch.ExternalCode == "B" ? true : false;

        if (!ePizza)
        {
            bool ExisteExternalCode = ClsDeIntegracaoSys.VerificaSeExisteProdutoComExternalCode(itemDelMatch.ExternalCode);

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
            ucComplemento.SetLabels(item.Name, item.TotalPrice);
            ucComplemento.Size = new Size(600, 60);

            if (!item.ExternalCode.Contains("m") && ePizza)
            {
                bool ExisteExternalCode = ClsDeIntegracaoSys.VerificaSeExisteProdutoComExternalCode(item.ExternalCode);

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

    public void MudaPictureBoxDeAvisoExternalCode(UCItemDelMatch instancia)
    {
        instancia.pictureBoxExternalCode.Visible = true;
    }

}
