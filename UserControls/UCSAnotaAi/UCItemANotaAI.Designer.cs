namespace SysIntegradorApp.UserControls.UCSAnotaAi
{
    partial class UCItemANotaAI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCItemANotaAI));
            quantidadeItem = new Label();
            nomeDoItem = new Label();
            pictureBoxExternalCode = new PictureBox();
            valorDoItem = new Label();
            groupBoxComplementos = new GroupBox();
            panelDeComplementos = new FlowLayoutPanel();
            panelValorDasOpcoes = new Panel();
            valorDasOpcoes = new Label();
            labelValorDasOpcoesNM = new Label();
            panelValorTotal = new Panel();
            valorTotalDoItem = new Label();
            labelValorTotalItemNM = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBoxExternalCode).BeginInit();
            groupBoxComplementos.SuspendLayout();
            panelValorDasOpcoes.SuspendLayout();
            panelValorTotal.SuspendLayout();
            SuspendLayout();
            // 
            // quantidadeItem
            // 
            quantidadeItem.AutoSize = true;
            quantidadeItem.Font = new Font("Segoe UI", 15F);
            quantidadeItem.Location = new Point(19, 16);
            quantidadeItem.Name = "quantidadeItem";
            quantidadeItem.Size = new Size(43, 35);
            quantidadeItem.TabIndex = 1;
            quantidadeItem.Text = "1X";
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
            // pictureBoxExternalCode
            // 
            pictureBoxExternalCode.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureBoxExternalCode.Image = (Image)resources.GetObject("pictureBoxExternalCode.Image");
            pictureBoxExternalCode.Location = new Point(573, 16);
            pictureBoxExternalCode.Name = "pictureBoxExternalCode";
            pictureBoxExternalCode.Size = new Size(38, 48);
            pictureBoxExternalCode.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxExternalCode.TabIndex = 27;
            pictureBoxExternalCode.TabStop = false;
            pictureBoxExternalCode.Visible = false;
            // 
            // valorDoItem
            // 
            valorDoItem.AutoSize = true;
            valorDoItem.Font = new Font("Segoe UI", 15F);
            valorDoItem.Location = new Point(669, 16);
            valorDoItem.Name = "valorDoItem";
            valorDoItem.Size = new Size(107, 35);
            valorDoItem.TabIndex = 28;
            valorDoItem.Text = "R$ 34,00";
            // 
            // groupBoxComplementos
            // 
            groupBoxComplementos.Controls.Add(panelDeComplementos);
            groupBoxComplementos.Location = new Point(35, 76);
            groupBoxComplementos.Name = "groupBoxComplementos";
            groupBoxComplementos.Size = new Size(762, 332);
            groupBoxComplementos.TabIndex = 29;
            groupBoxComplementos.TabStop = false;
            groupBoxComplementos.Text = "Complementos:";
            // 
            // panelDeComplementos
            // 
            panelDeComplementos.FlowDirection = FlowDirection.TopDown;
            panelDeComplementos.Location = new Point(6, 26);
            panelDeComplementos.Name = "panelDeComplementos";
            panelDeComplementos.Size = new Size(740, 291);
            panelDeComplementos.TabIndex = 10;
            panelDeComplementos.WrapContents = false;
            // 
            // panelValorDasOpcoes
            // 
            panelValorDasOpcoes.Controls.Add(valorDasOpcoes);
            panelValorDasOpcoes.Controls.Add(labelValorDasOpcoesNM);
            panelValorDasOpcoes.Location = new Point(52, 425);
            panelValorDasOpcoes.Name = "panelValorDasOpcoes";
            panelValorDasOpcoes.Size = new Size(740, 62);
            panelValorDasOpcoes.TabIndex = 30;
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
            panelValorTotal.TabIndex = 31;
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
            // UCItemANotaAI
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLight;
            Controls.Add(panelValorTotal);
            Controls.Add(panelValorDasOpcoes);
            Controls.Add(groupBoxComplementos);
            Controls.Add(valorDoItem);
            Controls.Add(pictureBoxExternalCode);
            Controls.Add(nomeDoItem);
            Controls.Add(quantidadeItem);
            Name = "UCItemANotaAI";
            Size = new Size(792, 800);
            ((System.ComponentModel.ISupportInitialize)pictureBoxExternalCode).EndInit();
            groupBoxComplementos.ResumeLayout(false);
            panelValorDasOpcoes.ResumeLayout(false);
            panelValorDasOpcoes.PerformLayout();
            panelValorTotal.ResumeLayout(false);
            panelValorTotal.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label quantidadeItem;
        private Label nomeDoItem;
        private PictureBox pictureBoxExternalCode;
        private Label valorDoItem;
        private GroupBox groupBoxComplementos;
        private FlowLayoutPanel panelDeComplementos;
        private Panel panelValorDasOpcoes;
        private Label valorDasOpcoes;
        private Label labelValorDasOpcoesNM;
        private Panel panelValorTotal;
        private Label valorTotalDoItem;
        private Label labelValorTotalItemNM;
    }
}
