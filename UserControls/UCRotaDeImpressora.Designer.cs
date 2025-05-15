namespace SysIntegradorApp.UserControls
{
    partial class UCRotaDeImpressora
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCRotaDeImpressora));
            pictureBox2 = new PictureBox();
            label93 = new Label();
            textBoxNomeRotaImp = new TextBox();
            BtnApagarImpressora = new Button();
            PanelDeAtivacao = new Panel();
            Off = new PictureBox();
            On = new PictureBox();
            LabelNomeDaEmpresa = new Label();
            panelImps = new Panel();
            comboBoxImpressoraBarAdicionar = new ComboBox();
            label86 = new Label();
            comboBoxImpressoraAuxiliarAdicionar = new ComboBox();
            label87 = new Label();
            label88 = new Label();
            comboBoxImpressoraCozinha3Adicionar = new ComboBox();
            label89 = new Label();
            comboBoxImpressoraCozinha2Adicionar = new ComboBox();
            label90 = new Label();
            comboBoxImpressoraCozinha1Adicionar = new ComboBox();
            label91 = new Label();
            comboBoxImpressoraCaixaAdicionar = new ComboBox();
            label92 = new Label();
            panelNomeRota = new Panel();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            PanelDeAtivacao.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Off).BeginInit();
            ((System.ComponentModel.ISupportInitialize)On).BeginInit();
            panelImps.SuspendLayout();
            panelNomeRota.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(0, 0);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(84, 83);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 8;
            pictureBox2.TabStop = false;
            // 
            // label93
            // 
            label93.AutoSize = true;
            label93.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label93.Location = new Point(19, 41);
            label93.Name = "label93";
            label93.Size = new Size(127, 23);
            label93.TabIndex = 23;
            label93.Text = "Nome Da Rota";
            // 
            // textBoxNomeRotaImp
            // 
            textBoxNomeRotaImp.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxNomeRotaImp.BackColor = Color.White;
            textBoxNomeRotaImp.Font = new Font("Segoe UI", 12F);
            textBoxNomeRotaImp.ForeColor = SystemColors.ControlText;
            textBoxNomeRotaImp.Location = new Point(19, 67);
            textBoxNomeRotaImp.Name = "textBoxNomeRotaImp";
            textBoxNomeRotaImp.PlaceholderText = "Nome da rota exemplo: MESAS";
            textBoxNomeRotaImp.Size = new Size(335, 34);
            textBoxNomeRotaImp.TabIndex = 22;
            textBoxNomeRotaImp.TextChanged += textBoxNomeRotaImp_TextChanged;
            // 
            // BtnApagarImpressora
            // 
            BtnApagarImpressora.BackColor = Color.Red;
            BtnApagarImpressora.Cursor = Cursors.Hand;
            BtnApagarImpressora.Dock = DockStyle.Bottom;
            BtnApagarImpressora.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            BtnApagarImpressora.ForeColor = SystemColors.ButtonFace;
            BtnApagarImpressora.Location = new Point(0, 511);
            BtnApagarImpressora.Name = "BtnApagarImpressora";
            BtnApagarImpressora.Size = new Size(1080, 52);
            BtnApagarImpressora.TabIndex = 37;
            BtnApagarImpressora.Text = "Apagar Rota";
            BtnApagarImpressora.UseVisualStyleBackColor = false;
            BtnApagarImpressora.Click += BtnApagarImpressora_Click;
            // 
            // PanelDeAtivacao
            // 
            PanelDeAtivacao.Anchor = AnchorStyles.Top;
            PanelDeAtivacao.BackColor = SystemColors.ControlDark;
            PanelDeAtivacao.Controls.Add(Off);
            PanelDeAtivacao.Controls.Add(On);
            PanelDeAtivacao.Controls.Add(LabelNomeDaEmpresa);
            PanelDeAtivacao.Location = new Point(678, 51);
            PanelDeAtivacao.Name = "PanelDeAtivacao";
            PanelDeAtivacao.Size = new Size(371, 117);
            PanelDeAtivacao.TabIndex = 38;
            // 
            // Off
            // 
            Off.Anchor = AnchorStyles.None;
            Off.Cursor = Cursors.Hand;
            Off.Image = (Image)resources.GetObject("Off.Image");
            Off.Location = new Point(144, 53);
            Off.Name = "Off";
            Off.Size = new Size(84, 43);
            Off.SizeMode = PictureBoxSizeMode.Zoom;
            Off.TabIndex = 28;
            Off.TabStop = false;
            Off.Click += Off_Click;
            // 
            // On
            // 
            On.Anchor = AnchorStyles.None;
            On.Cursor = Cursors.Hand;
            On.Image = (Image)resources.GetObject("On.Image");
            On.Location = new Point(144, 53);
            On.Name = "On";
            On.Size = new Size(84, 45);
            On.SizeMode = PictureBoxSizeMode.Zoom;
            On.TabIndex = 27;
            On.TabStop = false;
            On.Visible = false;
            On.Click += On_Click;
            // 
            // LabelNomeDaEmpresa
            // 
            LabelNomeDaEmpresa.BackColor = Color.Transparent;
            LabelNomeDaEmpresa.Dock = DockStyle.Top;
            LabelNomeDaEmpresa.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            LabelNomeDaEmpresa.ForeColor = SystemColors.ButtonHighlight;
            LabelNomeDaEmpresa.Location = new Point(0, 0);
            LabelNomeDaEmpresa.Name = "LabelNomeDaEmpresa";
            LabelNomeDaEmpresa.Size = new Size(371, 31);
            LabelNomeDaEmpresa.TabIndex = 0;
            LabelNomeDaEmpresa.Text = "Ativar Rota";
            LabelNomeDaEmpresa.TextAlign = ContentAlignment.TopCenter;
            // 
            // panelImps
            // 
            panelImps.Anchor = AnchorStyles.Top;
            panelImps.BackColor = SystemColors.ControlDark;
            panelImps.Controls.Add(comboBoxImpressoraBarAdicionar);
            panelImps.Controls.Add(label86);
            panelImps.Controls.Add(comboBoxImpressoraAuxiliarAdicionar);
            panelImps.Controls.Add(label87);
            panelImps.Controls.Add(label88);
            panelImps.Controls.Add(comboBoxImpressoraCozinha3Adicionar);
            panelImps.Controls.Add(label89);
            panelImps.Controls.Add(comboBoxImpressoraCozinha2Adicionar);
            panelImps.Controls.Add(label90);
            panelImps.Controls.Add(comboBoxImpressoraCozinha1Adicionar);
            panelImps.Controls.Add(label91);
            panelImps.Controls.Add(comboBoxImpressoraCaixaAdicionar);
            panelImps.Controls.Add(label92);
            panelImps.Location = new Point(123, 23);
            panelImps.Name = "panelImps";
            panelImps.Size = new Size(295, 460);
            panelImps.TabIndex = 39;
            // 
            // comboBoxImpressoraBarAdicionar
            // 
            comboBoxImpressoraBarAdicionar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            comboBoxImpressoraBarAdicionar.Font = new Font("Segoe UI", 12F);
            comboBoxImpressoraBarAdicionar.FormattingEnabled = true;
            comboBoxImpressoraBarAdicionar.Items.AddRange(new object[] { "Sem Impressora" });
            comboBoxImpressoraBarAdicionar.Location = new Point(35, 410);
            comboBoxImpressoraBarAdicionar.Name = "comboBoxImpressoraBarAdicionar";
            comboBoxImpressoraBarAdicionar.Size = new Size(230, 36);
            comboBoxImpressoraBarAdicionar.TabIndex = 46;
            comboBoxImpressoraBarAdicionar.Text = "Sem Impressora";
            comboBoxImpressoraBarAdicionar.SelectedIndexChanged += comboBoxImpressoraBarAdicionar_SelectedIndexChanged;
            // 
            // label86
            // 
            label86.AutoSize = true;
            label86.Font = new Font("Segoe UI", 12F);
            label86.Location = new Point(32, 98);
            label86.Name = "label86";
            label86.Size = new Size(81, 28);
            label86.TabIndex = 49;
            label86.Text = "Auxiliar:";
            // 
            // comboBoxImpressoraAuxiliarAdicionar
            // 
            comboBoxImpressoraAuxiliarAdicionar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            comboBoxImpressoraAuxiliarAdicionar.Font = new Font("Segoe UI", 12F);
            comboBoxImpressoraAuxiliarAdicionar.FormattingEnabled = true;
            comboBoxImpressoraAuxiliarAdicionar.Items.AddRange(new object[] { "Sem Impressora" });
            comboBoxImpressoraAuxiliarAdicionar.Location = new Point(32, 129);
            comboBoxImpressoraAuxiliarAdicionar.Name = "comboBoxImpressoraAuxiliarAdicionar";
            comboBoxImpressoraAuxiliarAdicionar.Size = new Size(233, 36);
            comboBoxImpressoraAuxiliarAdicionar.TabIndex = 48;
            comboBoxImpressoraAuxiliarAdicionar.Text = "Sem Impressora";
            comboBoxImpressoraAuxiliarAdicionar.SelectedIndexChanged += comboBoxImpressoraAuxiliarAdicionar_SelectedIndexChanged;
            // 
            // label87
            // 
            label87.AutoSize = true;
            label87.Font = new Font("Segoe UI", 12F);
            label87.Location = new Point(32, 379);
            label87.Name = "label87";
            label87.Size = new Size(44, 28);
            label87.TabIndex = 47;
            label87.Text = "Bar:";
            // 
            // label88
            // 
            label88.AutoSize = true;
            label88.Font = new Font("Segoe UI", 12F);
            label88.Location = new Point(32, 308);
            label88.Name = "label88";
            label88.Size = new Size(102, 28);
            label88.TabIndex = 45;
            label88.Text = "Cozinha 3:";
            // 
            // comboBoxImpressoraCozinha3Adicionar
            // 
            comboBoxImpressoraCozinha3Adicionar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            comboBoxImpressoraCozinha3Adicionar.Font = new Font("Segoe UI", 12F);
            comboBoxImpressoraCozinha3Adicionar.FormattingEnabled = true;
            comboBoxImpressoraCozinha3Adicionar.Items.AddRange(new object[] { "Sem Impressora" });
            comboBoxImpressoraCozinha3Adicionar.Location = new Point(32, 340);
            comboBoxImpressoraCozinha3Adicionar.Name = "comboBoxImpressoraCozinha3Adicionar";
            comboBoxImpressoraCozinha3Adicionar.Size = new Size(233, 36);
            comboBoxImpressoraCozinha3Adicionar.TabIndex = 44;
            comboBoxImpressoraCozinha3Adicionar.Text = "Sem Impressora";
            comboBoxImpressoraCozinha3Adicionar.SelectedIndexChanged += comboBoxImpressoraCozinha3Adicionar_SelectedIndexChanged;
            // 
            // label89
            // 
            label89.AutoSize = true;
            label89.Font = new Font("Segoe UI", 12F);
            label89.Location = new Point(32, 238);
            label89.Name = "label89";
            label89.Size = new Size(102, 28);
            label89.TabIndex = 43;
            label89.Text = "Cozinha 2:";
            // 
            // comboBoxImpressoraCozinha2Adicionar
            // 
            comboBoxImpressoraCozinha2Adicionar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            comboBoxImpressoraCozinha2Adicionar.Font = new Font("Segoe UI", 12F);
            comboBoxImpressoraCozinha2Adicionar.FormattingEnabled = true;
            comboBoxImpressoraCozinha2Adicionar.Items.AddRange(new object[] { "Sem Impressora" });
            comboBoxImpressoraCozinha2Adicionar.Location = new Point(32, 269);
            comboBoxImpressoraCozinha2Adicionar.Name = "comboBoxImpressoraCozinha2Adicionar";
            comboBoxImpressoraCozinha2Adicionar.Size = new Size(233, 36);
            comboBoxImpressoraCozinha2Adicionar.TabIndex = 42;
            comboBoxImpressoraCozinha2Adicionar.Text = "Sem Impressora";
            comboBoxImpressoraCozinha2Adicionar.SelectedIndexChanged += comboBoxImpressoraCozinha2Adicionar_SelectedIndexChanged;
            // 
            // label90
            // 
            label90.AutoSize = true;
            label90.Font = new Font("Segoe UI", 12F);
            label90.Location = new Point(32, 168);
            label90.Name = "label90";
            label90.Size = new Size(102, 28);
            label90.TabIndex = 41;
            label90.Text = "Cozinha 1:";
            // 
            // comboBoxImpressoraCozinha1Adicionar
            // 
            comboBoxImpressoraCozinha1Adicionar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            comboBoxImpressoraCozinha1Adicionar.Font = new Font("Segoe UI", 12F);
            comboBoxImpressoraCozinha1Adicionar.FormattingEnabled = true;
            comboBoxImpressoraCozinha1Adicionar.Items.AddRange(new object[] { "Sem Impressora" });
            comboBoxImpressoraCozinha1Adicionar.Location = new Point(29, 199);
            comboBoxImpressoraCozinha1Adicionar.Name = "comboBoxImpressoraCozinha1Adicionar";
            comboBoxImpressoraCozinha1Adicionar.Size = new Size(236, 36);
            comboBoxImpressoraCozinha1Adicionar.TabIndex = 40;
            comboBoxImpressoraCozinha1Adicionar.Text = "Sem Impressora";
            comboBoxImpressoraCozinha1Adicionar.SelectedIndexChanged += comboBoxImpressoraCozinha1Adicionar_SelectedIndexChanged;
            // 
            // label91
            // 
            label91.AutoSize = true;
            label91.Font = new Font("Segoe UI", 12F);
            label91.Location = new Point(32, 30);
            label91.Name = "label91";
            label91.Size = new Size(62, 28);
            label91.TabIndex = 39;
            label91.Text = "Caixa:";
            // 
            // comboBoxImpressoraCaixaAdicionar
            // 
            comboBoxImpressoraCaixaAdicionar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            comboBoxImpressoraCaixaAdicionar.Font = new Font("Segoe UI", 12F);
            comboBoxImpressoraCaixaAdicionar.FormattingEnabled = true;
            comboBoxImpressoraCaixaAdicionar.Items.AddRange(new object[] { "Sem Impressora" });
            comboBoxImpressoraCaixaAdicionar.Location = new Point(32, 59);
            comboBoxImpressoraCaixaAdicionar.Name = "comboBoxImpressoraCaixaAdicionar";
            comboBoxImpressoraCaixaAdicionar.Size = new Size(233, 36);
            comboBoxImpressoraCaixaAdicionar.TabIndex = 38;
            comboBoxImpressoraCaixaAdicionar.Text = "Sem Impressora";
            comboBoxImpressoraCaixaAdicionar.SelectedIndexChanged += comboBoxImpressoraCaixaAdicionar_SelectedIndexChanged;
            // 
            // label92
            // 
            label92.AutoSize = true;
            label92.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label92.Location = new Point(100, 15);
            label92.Name = "label92";
            label92.Size = new Size(105, 23);
            label92.TabIndex = 37;
            label92.Text = "Impressoras";
            // 
            // panelNomeRota
            // 
            panelNomeRota.Anchor = AnchorStyles.Top;
            panelNomeRota.BackColor = SystemColors.ControlDark;
            panelNomeRota.Controls.Add(label93);
            panelNomeRota.Controls.Add(textBoxNomeRotaImp);
            panelNomeRota.Location = new Point(678, 212);
            panelNomeRota.Name = "panelNomeRota";
            panelNomeRota.Size = new Size(371, 137);
            panelNomeRota.TabIndex = 40;
            // 
            // UCRotaDeImpressora
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLight;
            Controls.Add(panelNomeRota);
            Controls.Add(panelImps);
            Controls.Add(PanelDeAtivacao);
            Controls.Add(BtnApagarImpressora);
            Controls.Add(pictureBox2);
            Margin = new Padding(30, 30, 3, 3);
            Name = "UCRotaDeImpressora";
            Size = new Size(1080, 563);
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            PanelDeAtivacao.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)Off).EndInit();
            ((System.ComponentModel.ISupportInitialize)On).EndInit();
            panelImps.ResumeLayout(false);
            panelImps.PerformLayout();
            panelNomeRota.ResumeLayout(false);
            panelNomeRota.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox2;
        private Label label93;
        private TextBox textBoxNomeRotaImp;
        private Button BtnApagarImpressora;
        private Panel PanelDeAtivacao;
        private PictureBox Off;
        private PictureBox On;
        private Label LabelNomeDaEmpresa;
        private Panel panelImps;
        private ComboBox comboBoxImpressoraBarAdicionar;
        private Label label86;
        private ComboBox comboBoxImpressoraAuxiliarAdicionar;
        private Label label87;
        private Label label88;
        private ComboBox comboBoxImpressoraCozinha3Adicionar;
        private Label label89;
        private ComboBox comboBoxImpressoraCozinha2Adicionar;
        private Label label90;
        private ComboBox comboBoxImpressoraCozinha1Adicionar;
        private Label label91;
        private ComboBox comboBoxImpressoraCaixaAdicionar;
        private Label label92;
        private Panel panelNomeRota;
    }
}
