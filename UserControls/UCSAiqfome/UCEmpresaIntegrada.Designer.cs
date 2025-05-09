namespace SysIntegradorApp.UserControls.UCSAiqfome
{
    partial class UCEmpresaIntegrada
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCEmpresaIntegrada));
            panelDeIfoodNome = new Panel();
            pictureBoxOffItegraAiQueFome = new PictureBox();
            pictureBoxOnIntegraAiQueFome = new PictureBox();
            LabelNomeDaEmpresa = new Label();
            panelDeClientSecret = new Panel();
            textBoxMerchantId = new TextBox();
            label26 = new Label();
            panelDeTokensIfood = new Panel();
            textBoxVenceTokenIfoodEm = new TextBox();
            label29 = new Label();
            textBoxRefreshToken = new TextBox();
            textBoxAcessToken = new TextBox();
            label28 = new Label();
            label27 = new Label();
            AutBtn = new Button();
            pictureBox2 = new PictureBox();
            panelDeIfoodNome.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOffItegraAiQueFome).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOnIntegraAiQueFome).BeginInit();
            panelDeClientSecret.SuspendLayout();
            panelDeTokensIfood.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // panelDeIfoodNome
            // 
            panelDeIfoodNome.Anchor = AnchorStyles.Top;
            panelDeIfoodNome.BackColor = SystemColors.ControlDark;
            panelDeIfoodNome.Controls.Add(pictureBoxOffItegraAiQueFome);
            panelDeIfoodNome.Controls.Add(pictureBoxOnIntegraAiQueFome);
            panelDeIfoodNome.Controls.Add(LabelNomeDaEmpresa);
            panelDeIfoodNome.Location = new Point(350, 3);
            panelDeIfoodNome.Name = "panelDeIfoodNome";
            panelDeIfoodNome.Size = new Size(295, 99);
            panelDeIfoodNome.TabIndex = 7;
            // 
            // pictureBoxOffItegraAiQueFome
            // 
            pictureBoxOffItegraAiQueFome.Anchor = AnchorStyles.None;
            pictureBoxOffItegraAiQueFome.Cursor = Cursors.Hand;
            pictureBoxOffItegraAiQueFome.Image = (Image)resources.GetObject("pictureBoxOffItegraAiQueFome.Image");
            pictureBoxOffItegraAiQueFome.Location = new Point(110, 51);
            pictureBoxOffItegraAiQueFome.Name = "pictureBoxOffItegraAiQueFome";
            pictureBoxOffItegraAiQueFome.Size = new Size(84, 43);
            pictureBoxOffItegraAiQueFome.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxOffItegraAiQueFome.TabIndex = 28;
            pictureBoxOffItegraAiQueFome.TabStop = false;
            pictureBoxOffItegraAiQueFome.Click += pictureBoxOffItegraAiQueFome_Click;
            // 
            // pictureBoxOnIntegraAiQueFome
            // 
            pictureBoxOnIntegraAiQueFome.Anchor = AnchorStyles.None;
            pictureBoxOnIntegraAiQueFome.Cursor = Cursors.Hand;
            pictureBoxOnIntegraAiQueFome.Image = (Image)resources.GetObject("pictureBoxOnIntegraAiQueFome.Image");
            pictureBoxOnIntegraAiQueFome.Location = new Point(110, 51);
            pictureBoxOnIntegraAiQueFome.Name = "pictureBoxOnIntegraAiQueFome";
            pictureBoxOnIntegraAiQueFome.Size = new Size(84, 45);
            pictureBoxOnIntegraAiQueFome.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxOnIntegraAiQueFome.TabIndex = 27;
            pictureBoxOnIntegraAiQueFome.TabStop = false;
            pictureBoxOnIntegraAiQueFome.Visible = false;
            pictureBoxOnIntegraAiQueFome.Click += pictureBoxOnIntegraAiQueFome_Click;
            // 
            // LabelNomeDaEmpresa
            // 
            LabelNomeDaEmpresa.BackColor = Color.Transparent;
            LabelNomeDaEmpresa.Dock = DockStyle.Top;
            LabelNomeDaEmpresa.ForeColor = SystemColors.ButtonHighlight;
            LabelNomeDaEmpresa.Location = new Point(0, 0);
            LabelNomeDaEmpresa.Name = "LabelNomeDaEmpresa";
            LabelNomeDaEmpresa.Size = new Size(295, 48);
            LabelNomeDaEmpresa.TabIndex = 0;
            LabelNomeDaEmpresa.Text = "INTEGRAÇÃO aiquefome";
            LabelNomeDaEmpresa.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panelDeClientSecret
            // 
            panelDeClientSecret.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelDeClientSecret.BorderStyle = BorderStyle.FixedSingle;
            panelDeClientSecret.Controls.Add(textBoxMerchantId);
            panelDeClientSecret.Controls.Add(label26);
            panelDeClientSecret.Location = new Point(27, 117);
            panelDeClientSecret.Name = "panelDeClientSecret";
            panelDeClientSecret.Size = new Size(1016, 170);
            panelDeClientSecret.TabIndex = 8;
            // 
            // textBoxMerchantId
            // 
            textBoxMerchantId.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxMerchantId.BackColor = Color.White;
            textBoxMerchantId.Font = new Font("Segoe UI", 12F);
            textBoxMerchantId.ForeColor = SystemColors.ControlText;
            textBoxMerchantId.Location = new Point(13, 67);
            textBoxMerchantId.Name = "textBoxMerchantId";
            textBoxMerchantId.PlaceholderText = "ClientID";
            textBoxMerchantId.ReadOnly = true;
            textBoxMerchantId.Size = new Size(900, 34);
            textBoxMerchantId.TabIndex = 8;
            // 
            // label26
            // 
            label26.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label26.AutoSize = true;
            label26.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label26.Location = new Point(13, 36);
            label26.Name = "label26";
            label26.Size = new Size(100, 28);
            label26.TabIndex = 7;
            label26.Text = "CLient Id:";
            // 
            // panelDeTokensIfood
            // 
            panelDeTokensIfood.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelDeTokensIfood.BorderStyle = BorderStyle.FixedSingle;
            panelDeTokensIfood.Controls.Add(textBoxVenceTokenIfoodEm);
            panelDeTokensIfood.Controls.Add(label29);
            panelDeTokensIfood.Controls.Add(textBoxRefreshToken);
            panelDeTokensIfood.Controls.Add(textBoxAcessToken);
            panelDeTokensIfood.Controls.Add(label28);
            panelDeTokensIfood.Controls.Add(label27);
            panelDeTokensIfood.Location = new Point(27, 293);
            panelDeTokensIfood.Name = "panelDeTokensIfood";
            panelDeTokensIfood.Size = new Size(1016, 155);
            panelDeTokensIfood.TabIndex = 9;
            // 
            // textBoxVenceTokenIfoodEm
            // 
            textBoxVenceTokenIfoodEm.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
            textBoxVenceTokenIfoodEm.BackColor = Color.White;
            textBoxVenceTokenIfoodEm.Font = new Font("Segoe UI", 12F);
            textBoxVenceTokenIfoodEm.ForeColor = SystemColors.ControlText;
            textBoxVenceTokenIfoodEm.Location = new Point(675, 86);
            textBoxVenceTokenIfoodEm.Name = "textBoxVenceTokenIfoodEm";
            textBoxVenceTokenIfoodEm.Size = new Size(171, 34);
            textBoxVenceTokenIfoodEm.TabIndex = 9;
            // 
            // label29
            // 
            label29.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label29.AutoSize = true;
            label29.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label29.Location = new Point(675, 52);
            label29.Name = "label29";
            label29.Size = new Size(170, 28);
            label29.TabIndex = 8;
            label29.Text = "Vence Token em:";
            // 
            // textBoxRefreshToken
            // 
            textBoxRefreshToken.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxRefreshToken.BackColor = Color.White;
            textBoxRefreshToken.Font = new Font("Segoe UI", 12F);
            textBoxRefreshToken.ForeColor = SystemColors.ControlText;
            textBoxRefreshToken.Location = new Point(13, 116);
            textBoxRefreshToken.Name = "textBoxRefreshToken";
            textBoxRefreshToken.PlaceholderText = " ";
            textBoxRefreshToken.Size = new Size(619, 34);
            textBoxRefreshToken.TabIndex = 7;
            // 
            // textBoxAcessToken
            // 
            textBoxAcessToken.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxAcessToken.BackColor = Color.White;
            textBoxAcessToken.Font = new Font("Segoe UI", 12F);
            textBoxAcessToken.ForeColor = SystemColors.ControlText;
            textBoxAcessToken.Location = new Point(13, 49);
            textBoxAcessToken.Name = "textBoxAcessToken";
            textBoxAcessToken.Size = new Size(619, 34);
            textBoxAcessToken.TabIndex = 6;
            // 
            // label28
            // 
            label28.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label28.AutoSize = true;
            label28.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label28.Location = new Point(13, 86);
            label28.Name = "label28";
            label28.Size = new Size(139, 28);
            label28.TabIndex = 5;
            label28.Text = "Refres Token:";
            // 
            // label27
            // 
            label27.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label27.AutoSize = true;
            label27.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label27.Location = new Point(13, 18);
            label27.Name = "label27";
            label27.Size = new Size(142, 28);
            label27.TabIndex = 4;
            label27.Text = "Access Token:";
            // 
            // AutBtn
            // 
            AutBtn.AutoSize = true;
            AutBtn.BackColor = Color.Red;
            AutBtn.Cursor = Cursors.Hand;
            AutBtn.Dock = DockStyle.Bottom;
            AutBtn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            AutBtn.ForeColor = SystemColors.ButtonFace;
            AutBtn.Location = new Point(0, 511);
            AutBtn.Name = "AutBtn";
            AutBtn.Size = new Size(1080, 52);
            AutBtn.TabIndex = 11;
            AutBtn.Text = "Apagar Empresa";
            AutBtn.UseVisualStyleBackColor = false;
            AutBtn.Click += AutBtn_Click;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(0, 0);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(84, 83);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 12;
            pictureBox2.TabStop = false;
            // 
            // UCEmpresaIntegrada
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pictureBox2);
            Controls.Add(AutBtn);
            Controls.Add(panelDeTokensIfood);
            Controls.Add(panelDeClientSecret);
            Controls.Add(panelDeIfoodNome);
            Name = "UCEmpresaIntegrada";
            Size = new Size(1080, 563);
            panelDeIfoodNome.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxOffItegraAiQueFome).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOnIntegraAiQueFome).EndInit();
            panelDeClientSecret.ResumeLayout(false);
            panelDeClientSecret.PerformLayout();
            panelDeTokensIfood.ResumeLayout(false);
            panelDeTokensIfood.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panelDeIfoodNome;
        private PictureBox pictureBoxOffItegraAiQueFome;
        private PictureBox pictureBoxOnIntegraAiQueFome;
        private Label LabelNomeDaEmpresa;
        private Panel panelDeClientSecret;
        private TextBox textBoxMerchantId;
        private Label label26;
        private Panel panelDeTokensIfood;
        private TextBox textBoxVenceTokenIfoodEm;
        private Label label29;
        private TextBox textBoxRefreshToken;
        private TextBox textBoxAcessToken;
        private Label label28;
        private Label label27;
        private Button AutBtn;
        private PictureBox pictureBox2;
    }
}
