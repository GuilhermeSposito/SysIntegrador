namespace SysIntegradorApp.UserControls.TaxyMachine
{
    partial class UCEmpresaIntegradaTaxyMachine
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCEmpresaIntegradaTaxyMachine));
            AutBtn = new Button();
            panelDeIfoodNome = new Panel();
            NomeEmpresa = new Label();
            pictureBoxOffItegra = new PictureBox();
            pictureBoxOnIntegra = new PictureBox();
            panel20 = new Panel();
            label74 = new Label();
            label75 = new Label();
            textBoxTipoPagamento = new TextBox();
            label76 = new Label();
            label77 = new Label();
            textBoxToken = new TextBox();
            panel24 = new Panel();
            textBoxSenha = new TextBox();
            label78 = new Label();
            textBoxUserName = new TextBox();
            label82 = new Label();
            pictureBoxOTTO = new PictureBox();
            pictureBoxJUMA = new PictureBox();
            panelDeIfoodNome.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOffItegra).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOnIntegra).BeginInit();
            panel20.SuspendLayout();
            panel24.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOTTO).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxJUMA).BeginInit();
            SuspendLayout();
            // 
            // AutBtn
            // 
            AutBtn.AutoSize = true;
            AutBtn.BackColor = Color.Red;
            AutBtn.Cursor = Cursors.Hand;
            AutBtn.Dock = DockStyle.Bottom;
            AutBtn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            AutBtn.ForeColor = SystemColors.ButtonFace;
            AutBtn.Location = new Point(0, 512);
            AutBtn.Name = "AutBtn";
            AutBtn.Size = new Size(1139, 52);
            AutBtn.TabIndex = 14;
            AutBtn.Text = "Apagar Empresa";
            AutBtn.UseVisualStyleBackColor = false;
            AutBtn.Click += AutBtn_Click;
            // 
            // panelDeIfoodNome
            // 
            panelDeIfoodNome.Anchor = AnchorStyles.Top;
            panelDeIfoodNome.BackColor = SystemColors.ButtonFace;
            panelDeIfoodNome.Controls.Add(NomeEmpresa);
            panelDeIfoodNome.Controls.Add(pictureBoxOffItegra);
            panelDeIfoodNome.Controls.Add(pictureBoxOnIntegra);
            panelDeIfoodNome.Location = new Point(380, 0);
            panelDeIfoodNome.Name = "panelDeIfoodNome";
            panelDeIfoodNome.Size = new Size(295, 99);
            panelDeIfoodNome.TabIndex = 13;
            // 
            // NomeEmpresa
            // 
            NomeEmpresa.Dock = DockStyle.Top;
            NomeEmpresa.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            NomeEmpresa.Location = new Point(0, 0);
            NomeEmpresa.Name = "NomeEmpresa";
            NomeEmpresa.Size = new Size(295, 28);
            NomeEmpresa.TabIndex = 29;
            NomeEmpresa.Text = "Integração ";
            NomeEmpresa.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pictureBoxOffItegra
            // 
            pictureBoxOffItegra.Anchor = AnchorStyles.None;
            pictureBoxOffItegra.Cursor = Cursors.Hand;
            pictureBoxOffItegra.Image = (Image)resources.GetObject("pictureBoxOffItegra.Image");
            pictureBoxOffItegra.Location = new Point(103, 41);
            pictureBoxOffItegra.Name = "pictureBoxOffItegra";
            pictureBoxOffItegra.Size = new Size(84, 43);
            pictureBoxOffItegra.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxOffItegra.TabIndex = 28;
            pictureBoxOffItegra.TabStop = false;
            pictureBoxOffItegra.Click += pictureBoxOffItegra_Click;
            // 
            // pictureBoxOnIntegra
            // 
            pictureBoxOnIntegra.Anchor = AnchorStyles.None;
            pictureBoxOnIntegra.Cursor = Cursors.Hand;
            pictureBoxOnIntegra.Image = (Image)resources.GetObject("pictureBoxOnIntegra.Image");
            pictureBoxOnIntegra.Location = new Point(103, 39);
            pictureBoxOnIntegra.Name = "pictureBoxOnIntegra";
            pictureBoxOnIntegra.Size = new Size(84, 45);
            pictureBoxOnIntegra.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxOnIntegra.TabIndex = 27;
            pictureBoxOnIntegra.TabStop = false;
            pictureBoxOnIntegra.Visible = false;
            pictureBoxOnIntegra.Click += pictureBoxOnIntegra_Click;
            // 
            // panel20
            // 
            panel20.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel20.BorderStyle = BorderStyle.FixedSingle;
            panel20.Controls.Add(label74);
            panel20.Controls.Add(label75);
            panel20.Controls.Add(textBoxTipoPagamento);
            panel20.Controls.Add(label76);
            panel20.Controls.Add(label77);
            panel20.Controls.Add(textBoxToken);
            panel20.Location = new Point(92, 300);
            panel20.Name = "panel20";
            panel20.Size = new Size(883, 178);
            panel20.TabIndex = 7;
            // 
            // label74
            // 
            label74.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label74.AutoSize = true;
            label74.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label74.Location = new Point(586, -2);
            label74.Name = "label74";
            label74.Size = new Size(214, 28);
            label74.TabIndex = 16;
            label74.Text = "Tipos de pagamentos";
            // 
            // label75
            // 
            label75.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label75.Font = new Font("Segoe UI Semibold", 7.20000029F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            label75.Location = new Point(519, 26);
            label75.Name = "label75";
            label75.Size = new Size(342, 63);
            label75.TabIndex = 15;
            label75.Text = "Dinheiro (D) Débito (B) Crédito (C)      Pix (X)      PicPay (P) WhatsApp (H) Faturado (F) Carteira de Créditos (R)";
            label75.TextAlign = ContentAlignment.TopCenter;
            // 
            // textBoxTipoPagamento
            // 
            textBoxTipoPagamento.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxTipoPagamento.BackColor = Color.White;
            textBoxTipoPagamento.Font = new Font("Segoe UI", 12F);
            textBoxTipoPagamento.ForeColor = SystemColors.ControlText;
            textBoxTipoPagamento.Location = new Point(13, 139);
            textBoxTipoPagamento.MaxLength = 1;
            textBoxTipoPagamento.Name = "textBoxTipoPagamento";
            textBoxTipoPagamento.Size = new Size(461, 34);
            textBoxTipoPagamento.TabIndex = 14;
            // 
            // label76
            // 
            label76.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label76.AutoSize = true;
            label76.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label76.Location = new Point(13, 102);
            label76.Name = "label76";
            label76.Size = new Size(204, 28);
            label76.TabIndex = 13;
            label76.Text = "Tipo De Pagamento:";
            // 
            // label77
            // 
            label77.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label77.AutoSize = true;
            label77.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label77.Location = new Point(13, 15);
            label77.Name = "label77";
            label77.Size = new Size(326, 28);
            label77.TabIndex = 12;
            label77.Text = "Token de integração de entregas:";
            // 
            // textBoxToken
            // 
            textBoxToken.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxToken.BackColor = Color.White;
            textBoxToken.Font = new Font("Segoe UI", 12F);
            textBoxToken.ForeColor = SystemColors.ControlText;
            textBoxToken.Location = new Point(13, 55);
            textBoxToken.Name = "textBoxToken";
            textBoxToken.Size = new Size(461, 34);
            textBoxToken.TabIndex = 11;
            // 
            // panel24
            // 
            panel24.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel24.BorderStyle = BorderStyle.FixedSingle;
            panel24.Controls.Add(textBoxSenha);
            panel24.Controls.Add(label78);
            panel24.Controls.Add(textBoxUserName);
            panel24.Controls.Add(label82);
            panel24.Location = new Point(92, 105);
            panel24.Name = "panel24";
            panel24.Size = new Size(883, 168);
            panel24.TabIndex = 6;
            // 
            // textBoxSenha
            // 
            textBoxSenha.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxSenha.BackColor = Color.White;
            textBoxSenha.Font = new Font("Segoe UI", 12F);
            textBoxSenha.ForeColor = SystemColors.ControlText;
            textBoxSenha.Location = new Point(13, 112);
            textBoxSenha.Name = "textBoxSenha";
            textBoxSenha.Size = new Size(848, 34);
            textBoxSenha.TabIndex = 6;
            textBoxSenha.UseSystemPasswordChar = true;
            // 
            // label78
            // 
            label78.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label78.AutoSize = true;
            label78.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label78.Location = new Point(13, 81);
            label78.Name = "label78";
            label78.Size = new Size(74, 28);
            label78.TabIndex = 5;
            label78.Text = "Senha:";
            // 
            // textBoxUserName
            // 
            textBoxUserName.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxUserName.BackColor = Color.White;
            textBoxUserName.Font = new Font("Segoe UI", 12F);
            textBoxUserName.ForeColor = SystemColors.ControlText;
            textBoxUserName.Location = new Point(13, 44);
            textBoxUserName.Name = "textBoxUserName";
            textBoxUserName.Size = new Size(848, 34);
            textBoxUserName.TabIndex = 4;
            // 
            // label82
            // 
            label82.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label82.AutoSize = true;
            label82.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label82.Location = new Point(13, 13);
            label82.Name = "label82";
            label82.Size = new Size(111, 28);
            label82.TabIndex = 3;
            label82.Text = "Username:";
            // 
            // pictureBoxOTTO
            // 
            pictureBoxOTTO.Cursor = Cursors.Hand;
            pictureBoxOTTO.Image = (Image)resources.GetObject("pictureBoxOTTO.Image");
            pictureBoxOTTO.Location = new Point(0, 0);
            pictureBoxOTTO.Name = "pictureBoxOTTO";
            pictureBoxOTTO.Size = new Size(84, 83);
            pictureBoxOTTO.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxOTTO.TabIndex = 15;
            pictureBoxOTTO.TabStop = false;
            pictureBoxOTTO.Visible = false;
            // 
            // pictureBoxJUMA
            // 
            pictureBoxJUMA.Cursor = Cursors.Hand;
            pictureBoxJUMA.Image = (Image)resources.GetObject("pictureBoxJUMA.Image");
            pictureBoxJUMA.Location = new Point(3, 0);
            pictureBoxJUMA.Name = "pictureBoxJUMA";
            pictureBoxJUMA.Size = new Size(84, 83);
            pictureBoxJUMA.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxJUMA.TabIndex = 16;
            pictureBoxJUMA.TabStop = false;
            pictureBoxJUMA.Visible = false;
            // 
            // UCEmpresaIntegradaTaxyMachine
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDark;
            Controls.Add(pictureBoxJUMA);
            Controls.Add(pictureBoxOTTO);
            Controls.Add(panel20);
            Controls.Add(AutBtn);
            Controls.Add(panel24);
            Controls.Add(panelDeIfoodNome);
            Name = "UCEmpresaIntegradaTaxyMachine";
            Size = new Size(1139, 564);
            panelDeIfoodNome.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxOffItegra).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOnIntegra).EndInit();
            panel20.ResumeLayout(false);
            panel20.PerformLayout();
            panel24.ResumeLayout(false);
            panel24.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOTTO).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxJUMA).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button AutBtn;
        private Panel panelDeIfoodNome;
        private PictureBox pictureBoxOffItegra;
        private PictureBox pictureBoxOnIntegra;
        private Panel panel20;
        private Label label74;
        private Label label75;
        private TextBox textBoxTipoPagamento;
        private Label label76;
        private Label label77;
        private TextBox textBoxToken;
        private Panel panel24;
        private TextBox textBoxSenha;
        private Label label78;
        private TextBox textBoxUserName;
        private Label label82;
        private Label NomeEmpresa;
        private PictureBox pictureBoxOTTO;
        private PictureBox pictureBoxJUMA;
    }
}
