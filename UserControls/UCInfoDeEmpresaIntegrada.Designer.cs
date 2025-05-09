namespace SysIntegradorApp.UserControls
{
    partial class UCInfoDeEmpresaIntegrada
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCInfoDeEmpresaIntegrada));
            panelDoIfood = new Panel();
            AutBtn = new Button();
            pictureBox2 = new PictureBox();
            panelDeIfoodNome = new Panel();
            pictureBoxOffItegraIfood = new PictureBox();
            pictureBoxOnIntegraIfood = new PictureBox();
            LabelNomeDaEmpresa = new Label();
            panelDeTokensIfood = new Panel();
            textBoxVenceTokenIfoodEm = new TextBox();
            label29 = new Label();
            textBoxRefreshToken = new TextBox();
            textBoxAcessToken = new TextBox();
            label28 = new Label();
            label27 = new Label();
            panelDeClientSecret = new Panel();
            textBoxMerchantId = new TextBox();
            label26 = new Label();
            panelDoIfood.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            panelDeIfoodNome.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOffItegraIfood).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOnIntegraIfood).BeginInit();
            panelDeTokensIfood.SuspendLayout();
            panelDeClientSecret.SuspendLayout();
            SuspendLayout();
            // 
            // panelDoIfood
            // 
            panelDoIfood.AutoScroll = true;
            panelDoIfood.BackColor = Color.White;
            panelDoIfood.Controls.Add(AutBtn);
            panelDoIfood.Controls.Add(pictureBox2);
            panelDoIfood.Controls.Add(panelDeIfoodNome);
            panelDoIfood.Controls.Add(panelDeTokensIfood);
            panelDoIfood.Controls.Add(panelDeClientSecret);
            panelDoIfood.Dock = DockStyle.Fill;
            panelDoIfood.Location = new Point(0, 0);
            panelDoIfood.Name = "panelDoIfood";
            panelDoIfood.Size = new Size(1080, 563);
            panelDoIfood.TabIndex = 2;
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
            AutBtn.TabIndex = 10;
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
            pictureBox2.TabIndex = 7;
            pictureBox2.TabStop = false;
            // 
            // panelDeIfoodNome
            // 
            panelDeIfoodNome.Anchor = AnchorStyles.Top;
            panelDeIfoodNome.BackColor = SystemColors.ControlDark;
            panelDeIfoodNome.Controls.Add(pictureBoxOffItegraIfood);
            panelDeIfoodNome.Controls.Add(pictureBoxOnIntegraIfood);
            panelDeIfoodNome.Controls.Add(LabelNomeDaEmpresa);
            panelDeIfoodNome.Location = new Point(348, 0);
            panelDeIfoodNome.Name = "panelDeIfoodNome";
            panelDeIfoodNome.Size = new Size(295, 99);
            panelDeIfoodNome.TabIndex = 6;
            // 
            // pictureBoxOffItegraIfood
            // 
            pictureBoxOffItegraIfood.Anchor = AnchorStyles.None;
            pictureBoxOffItegraIfood.Cursor = Cursors.Hand;
            pictureBoxOffItegraIfood.Image = (Image)resources.GetObject("pictureBoxOffItegraIfood.Image");
            pictureBoxOffItegraIfood.Location = new Point(93, 38);
            pictureBoxOffItegraIfood.Name = "pictureBoxOffItegraIfood";
            pictureBoxOffItegraIfood.Size = new Size(84, 43);
            pictureBoxOffItegraIfood.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxOffItegraIfood.TabIndex = 28;
            pictureBoxOffItegraIfood.TabStop = false;
            pictureBoxOffItegraIfood.Click += pictureBoxOffItegraIfood_Click;
            // 
            // pictureBoxOnIntegraIfood
            // 
            pictureBoxOnIntegraIfood.Anchor = AnchorStyles.None;
            pictureBoxOnIntegraIfood.Cursor = Cursors.Hand;
            pictureBoxOnIntegraIfood.Image = (Image)resources.GetObject("pictureBoxOnIntegraIfood.Image");
            pictureBoxOnIntegraIfood.Location = new Point(93, 38);
            pictureBoxOnIntegraIfood.Name = "pictureBoxOnIntegraIfood";
            pictureBoxOnIntegraIfood.Size = new Size(84, 45);
            pictureBoxOnIntegraIfood.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxOnIntegraIfood.TabIndex = 27;
            pictureBoxOnIntegraIfood.TabStop = false;
            pictureBoxOnIntegraIfood.Visible = false;
            pictureBoxOnIntegraIfood.Click += pictureBoxOnIntegraIfood_Click;
            // 
            // LabelNomeDaEmpresa
            // 
            LabelNomeDaEmpresa.AutoSize = true;
            LabelNomeDaEmpresa.BackColor = Color.Transparent;
            LabelNomeDaEmpresa.ForeColor = SystemColors.ButtonHighlight;
            LabelNomeDaEmpresa.Location = new Point(70, 15);
            LabelNomeDaEmpresa.Name = "LabelNomeDaEmpresa";
            LabelNomeDaEmpresa.Size = new Size(124, 20);
            LabelNomeDaEmpresa.TabIndex = 0;
            LabelNomeDaEmpresa.Text = "Integração ifood ";
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
            panelDeTokensIfood.Location = new Point(47, 299);
            panelDeTokensIfood.Name = "panelDeTokensIfood";
            panelDeTokensIfood.Size = new Size(1016, 155);
            panelDeTokensIfood.TabIndex = 5;
            // 
            // textBoxVenceTokenIfoodEm
            // 
            textBoxVenceTokenIfoodEm.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
            textBoxVenceTokenIfoodEm.BackColor = Color.White;
            textBoxVenceTokenIfoodEm.Font = new Font("Segoe UI", 12F);
            textBoxVenceTokenIfoodEm.ForeColor = SystemColors.ControlText;
            textBoxVenceTokenIfoodEm.Location = new Point(675, 86);
            textBoxVenceTokenIfoodEm.Name = "textBoxVenceTokenIfoodEm";
            textBoxVenceTokenIfoodEm.Size = new Size(258, 34);
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
            textBoxRefreshToken.Size = new Size(582, 34);
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
            textBoxAcessToken.Size = new Size(582, 34);
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
            // panelDeClientSecret
            // 
            panelDeClientSecret.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelDeClientSecret.BorderStyle = BorderStyle.FixedSingle;
            panelDeClientSecret.Controls.Add(textBoxMerchantId);
            panelDeClientSecret.Controls.Add(label26);
            panelDeClientSecret.Location = new Point(47, 108);
            panelDeClientSecret.Name = "panelDeClientSecret";
            panelDeClientSecret.Size = new Size(1016, 170);
            panelDeClientSecret.TabIndex = 4;
            // 
            // textBoxMerchantId
            // 
            textBoxMerchantId.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxMerchantId.BackColor = Color.White;
            textBoxMerchantId.Font = new Font("Segoe UI", 12F);
            textBoxMerchantId.ForeColor = SystemColors.ControlText;
            textBoxMerchantId.Location = new Point(13, 67);
            textBoxMerchantId.Name = "textBoxMerchantId";
            textBoxMerchantId.PlaceholderText = "Merchant Id";
            textBoxMerchantId.ReadOnly = true;
            textBoxMerchantId.Size = new Size(981, 34);
            textBoxMerchantId.TabIndex = 8;
            // 
            // label26
            // 
            label26.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label26.AutoSize = true;
            label26.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label26.Location = new Point(13, 36);
            label26.Name = "label26";
            label26.Size = new Size(132, 28);
            label26.TabIndex = 7;
            label26.Text = "Merchant Id:";
            // 
            // UCInfoDeEmpresaIntegrada
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panelDoIfood);
            Name = "UCInfoDeEmpresaIntegrada";
            Size = new Size(1080, 563);
            panelDoIfood.ResumeLayout(false);
            panelDoIfood.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            panelDeIfoodNome.ResumeLayout(false);
            panelDeIfoodNome.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOffItegraIfood).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOnIntegraIfood).EndInit();
            panelDeTokensIfood.ResumeLayout(false);
            panelDeTokensIfood.PerformLayout();
            panelDeClientSecret.ResumeLayout(false);
            panelDeClientSecret.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelDoIfood;
        private PictureBox pictureBox2;
        private Panel panelDeIfoodNome;
        private PictureBox pictureBoxOffItegraIfood;
        private PictureBox pictureBoxOnIntegraIfood;
        private Label LabelNomeDaEmpresa;
        private Panel panelDeTokensIfood;
        private TextBox textBoxVenceTokenIfoodEm;
        private Label label29;
        private TextBox textBoxRefreshToken;
        private TextBox textBoxAcessToken;
        private Label label28;
        private Label label27;
        private Panel panelDeClientSecret;
        private TextBox textBoxMerchantId;
        private Label label26;
        private Button AutBtn;
    }
}
