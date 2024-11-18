using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares.ClassesGarcomSysMenu;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysIntegradorApp.UserControls.UcsGarcom;

public partial class UCItemGarcom : UserControl
{
    public UCItemGarcom()
    {
        InitializeComponent();
        ClsEstiloComponentes.CustomizePanelBorder(panelValorDasOpcoes);
        ClsEstiloComponentes.CustomizePanelBorder(panelValorTotal);
        ClsEstiloComponentes.SetRoundedRegion(this, 24);
    }

    //método que define todas as labels e panel dentro do UCITEM
    public void SetLabels(Produto item)
    {
        if(item.Requisicao is not null)
        {

        }

        double PrecoDOItem = 0;
        if (item.Tamanho == "Grande")
        {
            PrecoDOItem = item.Preco3 * item.Quantidade;
        }
        else if (item.Tamanho == "Medio")
        {
            PrecoDOItem = item.Preco2 * item.Quantidade;
        }
        else if (item.Tamanho == "Pequeno")
        {
            PrecoDOItem = item.Preco1 * item.Quantidade;
        }
        else
        {
            PrecoDOItem = item.Preco1 * item.Quantidade;
        }

        double ValorDosIncrementos = 0;
        foreach (var incremento in item.incrementos)
        {
            ValorDosIncrementos += incremento.Valor * item.Quantidade;
        }

        double totalPrice = PrecoDOItem + ValorDosIncrementos;


        quantidadeItem.Text = $"{item.Quantidade.ToString()}X";
        nomeDoItem.Text = item.Descricao;
        valorDoItem.Text = PrecoDOItem.ToString("c");
        valorDasOpcoes.Text = ValorDosIncrementos.ToString("c");
        valorTotalDoItem.Text = totalPrice.ToString("c");

        int currentY = 0; // Variável para controlar a posição Y dos controles adicionados
        int maxHeight = 0; // Variável para armazenar a altura máxima dos controles adicionados

       

        if (item.incrementos.Count() == 0)
        {
            panelDeComplementos.Visible = false;
            groupBoxComplementos.Visible = false;
            panelValorDasOpcoes.Location = new Point(groupBoxComplementos.Location.X, groupBoxComplementos.Location.Y);
            panelValorTotal.Location = new Point(panelValorDasOpcoes.Location.X, panelValorDasOpcoes.Location.Y + 90);
            this.Size = new System.Drawing.Size(792, 300);
        }

        foreach (var incremento in item.incrementos)
        {
            UCComplementoDoItem ucComplemento = new UCComplementoDoItem();
            ucComplemento.SetLabels(incremento!.Descricao, Convert.ToSingle(incremento.Valor));
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

    }
}
