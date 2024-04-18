namespace SysIntegradorApp
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            BrnAutorizar = new Button();
            CodeFromUser = new TextBox();
            pictureBoxSysLogica = new PictureBox();
            majorPanel = new Panel();
            label1 = new Label();
            pictureBoxCadeado = new PictureBox();
            groupBoxAut = new GroupBox();
            panelDeColar = new Panel();
            linkLabel1 = new LinkLabel();
            pictureBoxDeColar = new PictureBox();
            labelCodigo = new Label();
            panelInstrucoes = new Panel();
            groupBoxInstru = new GroupBox();
            label7 = new Label();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            pictureBoxInfo = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBoxSysLogica).BeginInit();
            majorPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxCadeado).BeginInit();
            groupBoxAut.SuspendLayout();
            panelDeColar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxDeColar).BeginInit();
            panelInstrucoes.SuspendLayout();
            groupBoxInstru.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxInfo).BeginInit();
            SuspendLayout();
            // 
            // BrnAutorizar
            // 
            BrnAutorizar.BackColor = Color.Red;
            BrnAutorizar.FlatAppearance.BorderColor = Color.White;
            BrnAutorizar.FlatAppearance.BorderSize = 2;
            BrnAutorizar.FlatStyle = FlatStyle.Flat;
            BrnAutorizar.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            BrnAutorizar.ForeColor = SystemColors.ButtonHighlight;
            BrnAutorizar.Location = new Point(180, 107);
            BrnAutorizar.Name = "BrnAutorizar";
            BrnAutorizar.Size = new Size(180, 74);
            BrnAutorizar.TabIndex = 3;
            BrnAutorizar.Text = "Autorizar";
            BrnAutorizar.UseVisualStyleBackColor = false;
            BrnAutorizar.Click += BrnAutorizar_Click;
            // 
            // CodeFromUser
            // 
            CodeFromUser.BackColor = Color.DarkGray;
            CodeFromUser.BorderStyle = BorderStyle.None;
            CodeFromUser.Cursor = Cursors.Hand;
            CodeFromUser.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            CodeFromUser.ForeColor = SystemColors.Control;
            CodeFromUser.Location = new Point(66, 57);
            CodeFromUser.Multiline = true;
            CodeFromUser.Name = "CodeFromUser";
            CodeFromUser.PlaceholderText = "Insira o código Fornecido pelo ifood";
            CodeFromUser.Size = new Size(345, 44);
            CodeFromUser.TabIndex = 6;
            CodeFromUser.TextAlign = HorizontalAlignment.Center;
            CodeFromUser.KeyDown += CodeFromUser_KeyDown;
            // 
            // pictureBoxSysLogica
            // 
            pictureBoxSysLogica.Image = (Image)resources.GetObject("pictureBoxSysLogica.Image");
            pictureBoxSysLogica.Location = new Point(203, 0);
            pictureBoxSysLogica.Name = "pictureBoxSysLogica";
            pictureBoxSysLogica.Size = new Size(379, 109);
            pictureBoxSysLogica.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxSysLogica.TabIndex = 8;
            pictureBoxSysLogica.TabStop = false;
            // 
            // majorPanel
            // 
            majorPanel.BackColor = SystemColors.ActiveBorder;
            majorPanel.Controls.Add(label1);
            majorPanel.Controls.Add(pictureBoxCadeado);
            majorPanel.Location = new Point(92, 130);
            majorPanel.Name = "majorPanel";
            majorPanel.Size = new Size(598, 195);
            majorPanel.TabIndex = 9;
            // 
            // label1
            // 
            label1.Font = new Font("Segoe UI", 19.8000011F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.ButtonHighlight;
            label1.Location = new Point(192, 3);
            label1.Name = "label1";
            label1.Size = new Size(401, 160);
            label1.TabIndex = 1;
            label1.Text = "Clique no Cadeado Para Autorizar Nosso Aplicativo ";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pictureBoxCadeado
            // 
            pictureBoxCadeado.Cursor = Cursors.Hand;
            pictureBoxCadeado.Image = (Image)resources.GetObject("pictureBoxCadeado.Image");
            pictureBoxCadeado.Location = new Point(0, 3);
            pictureBoxCadeado.Name = "pictureBoxCadeado";
            pictureBoxCadeado.Size = new Size(186, 189);
            pictureBoxCadeado.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxCadeado.TabIndex = 0;
            pictureBoxCadeado.TabStop = false;
            pictureBoxCadeado.Click += pictureBoxCadeado_Click;
            // 
            // groupBoxAut
            // 
            groupBoxAut.Controls.Add(panelDeColar);
            groupBoxAut.Controls.Add(labelCodigo);
            groupBoxAut.Controls.Add(BrnAutorizar);
            groupBoxAut.Controls.Add(CodeFromUser);
            groupBoxAut.Location = new Point(92, 343);
            groupBoxAut.Name = "groupBoxAut";
            groupBoxAut.Size = new Size(598, 192);
            groupBoxAut.TabIndex = 10;
            groupBoxAut.TabStop = false;
            groupBoxAut.Text = "Autorização";
            groupBoxAut.Visible = false;
            // 
            // panelDeColar
            // 
            panelDeColar.Controls.Add(linkLabel1);
            panelDeColar.Controls.Add(pictureBoxDeColar);
            panelDeColar.ImeMode = ImeMode.Hiragana;
            panelDeColar.Location = new Point(425, 57);
            panelDeColar.Name = "panelDeColar";
            panelDeColar.Size = new Size(130, 44);
            panelDeColar.TabIndex = 12;
            panelDeColar.Click += panelDeColar_Click;
            panelDeColar.Paint += panelDeColar_Paint;
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Cursor = Cursors.Hand;
            linkLabel1.Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold, GraphicsUnit.Point, 0);
            linkLabel1.LinkColor = Color.Red;
            linkLabel1.Location = new Point(42, 3);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(75, 35);
            linkLabel1.TabIndex = 1;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Colar";
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // pictureBoxDeColar
            // 
            pictureBoxDeColar.Cursor = Cursors.Hand;
            pictureBoxDeColar.Image = (Image)resources.GetObject("pictureBoxDeColar.Image");
            pictureBoxDeColar.Location = new Point(0, 0);
            pictureBoxDeColar.Name = "pictureBoxDeColar";
            pictureBoxDeColar.Size = new Size(36, 44);
            pictureBoxDeColar.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxDeColar.TabIndex = 0;
            pictureBoxDeColar.TabStop = false;
            pictureBoxDeColar.Click += pictureBoxDeColar_Click;
            // 
            // labelCodigo
            // 
            labelCodigo.AutoSize = true;
            labelCodigo.Font = new Font("Segoe UI", 9F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            labelCodigo.Location = new Point(66, 34);
            labelCodigo.Name = "labelCodigo";
            labelCodigo.Size = new Size(64, 20);
            labelCodigo.TabIndex = 11;
            labelCodigo.Text = "Código:";
            // 
            // panelInstrucoes
            // 
            panelInstrucoes.BackColor = SystemColors.ButtonHighlight;
            panelInstrucoes.Controls.Add(groupBoxInstru);
            panelInstrucoes.Cursor = Cursors.Help;
            panelInstrucoes.Location = new Point(92, 331);
            panelInstrucoes.Name = "panelInstrucoes";
            panelInstrucoes.Size = new Size(598, 229);
            panelInstrucoes.TabIndex = 11;
            // 
            // groupBoxInstru
            // 
            groupBoxInstru.Controls.Add(label7);
            groupBoxInstru.Controls.Add(label6);
            groupBoxInstru.Controls.Add(label5);
            groupBoxInstru.Controls.Add(label4);
            groupBoxInstru.Controls.Add(label3);
            groupBoxInstru.Controls.Add(label2);
            groupBoxInstru.Location = new Point(12, 12);
            groupBoxInstru.Name = "groupBoxInstru";
            groupBoxInstru.Size = new Size(572, 212);
            groupBoxInstru.TabIndex = 0;
            groupBoxInstru.TabStop = false;
            groupBoxInstru.Text = "Instruções";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label7.Location = new Point(17, 161);
            label7.Name = "label7";
            label7.Size = new Size(364, 20);
            label7.TabIndex = 5;
            label7.Text = "* Após colar o código aqui, Clique no botão \"Autorizar\".";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label6.Location = new Point(17, 141);
            label6.Name = "label6";
            label6.Size = new Size(393, 20);
            label6.TabIndex = 4;
            label6.Text = "* Copie o código gerado pelo ifood e cole aqui no aplicativo.";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label5.Location = new Point(17, 121);
            label5.Name = "label5";
            label5.Size = new Size(149, 20);
            label5.TabIndex = 3;
            label5.Text = "* Clique em autorizar.";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label4.Location = new Point(17, 101);
            label4.Name = "label4";
            label4.Size = new Size(305, 20);
            label4.TabIndex = 2;
            label4.Text = "* Ele vai abrir direto na pagina de autorização.";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label3.Location = new Point(17, 81);
            label3.Name = "label3";
            label3.Size = new Size(242, 20);
            label3.TabIndex = 1;
            label3.Text = "* Faça o login na sua conta do ifood.";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label2.Location = new Point(17, 61);
            label2.Name = "label2";
            label2.Size = new Size(469, 20);
            label2.TabIndex = 0;
            label2.Text = "* Clique no cadeado e você será redirecionado para o portal do parceiro.";
            // 
            // pictureBoxInfo
            // 
            pictureBoxInfo.Cursor = Cursors.Hand;
            pictureBoxInfo.Image = (Image)resources.GetObject("pictureBoxInfo.Image");
            pictureBoxInfo.Location = new Point(10, 6);
            pictureBoxInfo.Name = "pictureBoxInfo";
            pictureBoxInfo.Size = new Size(36, 32);
            pictureBoxInfo.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxInfo.TabIndex = 12;
            pictureBoxInfo.TabStop = false;
            pictureBoxInfo.Click += pictureBoxInfo_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ButtonHighlight;
            ClientSize = new Size(782, 572);
            Controls.Add(pictureBoxInfo);
            Controls.Add(panelInstrucoes);
            Controls.Add(majorPanel);
            Controls.Add(pictureBoxSysLogica);
            Controls.Add(groupBoxAut);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Autorização Do Aplicativo";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBoxSysLogica).EndInit();
            majorPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxCadeado).EndInit();
            groupBoxAut.ResumeLayout(false);
            groupBoxAut.PerformLayout();
            panelDeColar.ResumeLayout(false);
            panelDeColar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxDeColar).EndInit();
            panelInstrucoes.ResumeLayout(false);
            groupBoxInstru.ResumeLayout(false);
            groupBoxInstru.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxInfo).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Button BrnAutorizar;
        private TextBox CodeFromUser;
        private PictureBox pictureBoxSysLogica;
        private Panel majorPanel;
        private Label label1;
        private PictureBox pictureBoxCadeado;
        private Label labelCodigo;
        public GroupBox groupBoxAut;
        private Panel panelInstrucoes;
        private GroupBox groupBoxInstru;
        private Label label2;
        private Label label4;
        private Label label3;
        private Label label7;
        private Label label6;
        private Label label5;
        private PictureBox pictureBoxInfo;
        private Panel panelDeColar;
        private PictureBox pictureBoxDeColar;
        private LinkLabel linkLabel1;
    }
}
