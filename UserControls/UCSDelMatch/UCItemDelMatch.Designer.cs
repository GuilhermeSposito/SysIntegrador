namespace SysIntegradorApp.UserControls.UCSDelMatch
{
    partial class UCItemDelMatch
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            nomeDoItem = new Label();
            quantidadeItem = new Label();
            valorDoItem = new Label();
            panelDeComplementos = new FlowLayoutPanel();
            groupBoxComplementos = new GroupBox();
            panelValorDasOpcoes = new Panel();
            valorDasOpcoes = new Label();
            labelValorDasOpcoesNM = new Label();
            panelValorTotal = new Panel();
            valorTotalDoItem = new Label();
            labelValorTotalItemNM = new Label();
            panelValorDasOpcoes.SuspendLayout();
            panelValorTotal.SuspendLayout();
            SuspendLayout();
            // 
            // nomeDoItem
            // 
            nomeDoItem.AutoSize = true;
            nomeDoItem.Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold, GraphicsUnit.Point, 0);
            nomeDoItem.Location = new Point(68, 16);
            nomeDoItem.Name = "nomeDoItem";
            nomeDoItem.Size = new Size(371, 35);
            nomeDoItem.TabIndex = 2;
            nomeDoItem.Text = "Marmita Média Com Churrasco";
            // 
            // quantidadeItem
            // 
            quantidadeItem.AutoSize = true;
            quantidadeItem.Font = new Font("Segoe UI", 15F);
            quantidadeItem.Location = new Point(19, 16);
            quantidadeItem.Name = "quantidadeItem";
            quantidadeItem.Size = new Size(43, 35);
            quantidadeItem.TabIndex = 3;
            quantidadeItem.Text = "1X";
            // 
            // valorDoItem
            // 
            valorDoItem.AutoSize = true;
            valorDoItem.Font = new Font("Segoe UI", 15F);
            valorDoItem.Location = new Point(672, 16);
            valorDoItem.Name = "valorDoItem";
            valorDoItem.Size = new Size(107, 35);
            valorDoItem.TabIndex = 4;
            valorDoItem.Text = "R$ 34,00";
            // 
            // panelDeComplementos
            // 
            panelDeComplementos.FlowDirection = FlowDirection.TopDown;
            panelDeComplementos.Location = new Point(49, 99);
            panelDeComplementos.Name = "panelDeComplementos";
            panelDeComplementos.Size = new Size(740, 291);
            panelDeComplementos.TabIndex = 10;
            panelDeComplementos.WrapContents = false;
            // 
            // groupBoxComplementos
            // 
            groupBoxComplementos.Location = new Point(35, 76);
            groupBoxComplementos.Name = "groupBoxComplementos";
            groupBoxComplementos.Size = new Size(762, 332);
            groupBoxComplementos.TabIndex = 11;
            groupBoxComplementos.TabStop = false;
            groupBoxComplementos.Text = "Complementos:";
            // 
            // panelValorDasOpcoes
            // 
            panelValorDasOpcoes.Controls.Add(valorDasOpcoes);
            panelValorDasOpcoes.Controls.Add(labelValorDasOpcoesNM);
            panelValorDasOpcoes.Location = new Point(52, 425);
            panelValorDasOpcoes.Name = "panelValorDasOpcoes";
            panelValorDasOpcoes.Size = new Size(740, 62);
            panelValorDasOpcoes.TabIndex = 12;
            // 
            // valorDasOpcoes
            // 
            valorDasOpcoes.AutoSize = true;
            valorDasOpcoes.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            valorDasOpcoes.Location = new Point(648, 21);
            valorDasOpcoes.Name = "valorDasOpcoes";
            valorDasOpcoes.Size = new Size(61, 23);
            valorDasOpcoes.TabIndex = 1;
            valorDasOpcoes.Text = "R$: 0,0";
            // 
            // labelValorDasOpcoesNM
            // 
            labelValorDasOpcoesNM.AutoSize = true;
            labelValorDasOpcoesNM.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelValorDasOpcoesNM.Location = new Point(19, 21);
            labelValorDasOpcoesNM.Name = "labelValorDasOpcoesNM";
            labelValorDasOpcoesNM.Size = new Size(164, 23);
            labelValorDasOpcoesNM.TabIndex = 0;
            labelValorDasOpcoesNM.Text = "Valores Das Opções:";
            // 
            // panelValorTotal
            // 
            panelValorTotal.Controls.Add(valorTotalDoItem);
            panelValorTotal.Controls.Add(labelValorTotalItemNM);
            panelValorTotal.Location = new Point(52, 502);
            panelValorTotal.Name = "panelValorTotal";
            panelValorTotal.Size = new Size(740, 62);
            panelValorTotal.TabIndex = 13;
            // 
            // valorTotalDoItem
            // 
            valorTotalDoItem.AutoSize = true;
            valorTotalDoItem.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            valorTotalDoItem.Location = new Point(648, 21);
            valorTotalDoItem.Name = "valorTotalDoItem";
            valorTotalDoItem.Size = new Size(79, 23);
            valorTotalDoItem.TabIndex = 1;
            valorTotalDoItem.Text = "R$: 83,00";
            // 
            // labelValorTotalItemNM
            // 
            labelValorTotalItemNM.AutoSize = true;
            labelValorTotalItemNM.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelValorTotalItemNM.Location = new Point(19, 21);
            labelValorTotalItemNM.Name = "labelValorTotalItemNM";
            labelValorTotalItemNM.Size = new Size(136, 23);
            labelValorTotalItemNM.TabIndex = 0;
            labelValorTotalItemNM.Text = "Valores Do Item:";
            // 
            // UCItemDelMatch
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLight;
            Controls.Add(panelValorTotal);
            Controls.Add(panelValorDasOpcoes);
            Controls.Add(panelDeComplementos);
            Controls.Add(groupBoxComplementos);
            Controls.Add(valorDoItem);
            Controls.Add(quantidadeItem);
            Controls.Add(nomeDoItem);
            Margin = new Padding(3, 8, 3, 3);
            Name = "UCItemDelMatch";
            Size = new Size(792, 800);
            panelValorDasOpcoes.ResumeLayout(false);
            panelValorDasOpcoes.PerformLayout();
            panelValorTotal.ResumeLayout(false);
            panelValorTotal.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label nomeDoItem;
        private Label quantidadeItem;
        private Label valorDoItem;
        private FlowLayoutPanel panelDeComplementos;
        private GroupBox groupBoxComplementos;
        private Panel panelValorDasOpcoes;
        private Label valorDasOpcoes;
        private Label labelValorDasOpcoesNM;
        private Panel panelValorTotal;
        private Label valorTotalDoItem;
        private Label labelValorTotalItemNM;
    }
}
