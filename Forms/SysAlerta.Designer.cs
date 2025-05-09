namespace SysIntegradorApp.Forms
{
    partial class SysAlerta
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SysAlerta));
            BtnSim = new Button();
            BtnNao = new Button();
            BtnOk = new Button();
            panelTituloContainer = new Panel();
            panelDeTitulo = new Panel();
            lblTitulo = new Label();
            panelDeMensagem = new Panel();
            LblMensagem = new Label();
            pictureBoxSucesso = new PictureBox();
            pictureBoxError = new PictureBox();
            pictureBoxAviso = new PictureBox();
            panelTituloContainer.SuspendLayout();
            panelDeTitulo.SuspendLayout();
            panelDeMensagem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxSucesso).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxError).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxAviso).BeginInit();
            SuspendLayout();
            // 
            // BtnSim
            // 
            BtnSim.Anchor = AnchorStyles.Left;
            BtnSim.BackColor = Color.Red;
            BtnSim.Cursor = Cursors.Hand;
            BtnSim.Font = new Font("Segoe UI", 15F, FontStyle.Bold, GraphicsUnit.Point, 0);
            BtnSim.ForeColor = SystemColors.ActiveCaptionText;
            BtnSim.Location = new Point(73, 338);
            BtnSim.Name = "BtnSim";
            BtnSim.Size = new Size(178, 48);
            BtnSim.TabIndex = 6;
            BtnSim.Text = "SIM";
            BtnSim.UseVisualStyleBackColor = false;
            BtnSim.Click += BtnSim_Click;
            // 
            // BtnNao
            // 
            BtnNao.Anchor = AnchorStyles.Right;
            BtnNao.BackColor = Color.Red;
            BtnNao.Cursor = Cursors.Hand;
            BtnNao.Font = new Font("Segoe UI", 15F, FontStyle.Bold, GraphicsUnit.Point, 0);
            BtnNao.ForeColor = SystemColors.ActiveCaptionText;
            BtnNao.Location = new Point(485, 338);
            BtnNao.Name = "BtnNao";
            BtnNao.Size = new Size(178, 48);
            BtnNao.TabIndex = 7;
            BtnNao.Text = "NÃO";
            BtnNao.UseVisualStyleBackColor = false;
            BtnNao.Click += BtnNao_Click;
            // 
            // BtnOk
            // 
            BtnOk.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            BtnOk.BackColor = Color.Red;
            BtnOk.Cursor = Cursors.Hand;
            BtnOk.Font = new Font("Segoe UI", 15F, FontStyle.Bold, GraphicsUnit.Point, 0);
            BtnOk.ForeColor = SystemColors.ActiveCaptionText;
            BtnOk.Location = new Point(279, 338);
            BtnOk.Name = "BtnOk";
            BtnOk.Size = new Size(178, 48);
            BtnOk.TabIndex = 8;
            BtnOk.Text = "OK";
            BtnOk.UseVisualStyleBackColor = false;
            BtnOk.Click += BtnOk_Click;
            // 
            // panelTituloContainer
            // 
            panelTituloContainer.BackColor = Color.Transparent;
            panelTituloContainer.Controls.Add(panelDeTitulo);
            panelTituloContainer.Dock = DockStyle.Top;
            panelTituloContainer.Location = new Point(0, 0);
            panelTituloContainer.Name = "panelTituloContainer";
            panelTituloContainer.Size = new Size(723, 70);
            panelTituloContainer.TabIndex = 9;
            // 
            // panelDeTitulo
            // 
            panelDeTitulo.BackColor = Color.FromArgb(242, 193, 46);
            panelDeTitulo.Controls.Add(lblTitulo);
            panelDeTitulo.Dock = DockStyle.Fill;
            panelDeTitulo.Location = new Point(0, 0);
            panelDeTitulo.Name = "panelDeTitulo";
            panelDeTitulo.Size = new Size(723, 70);
            panelDeTitulo.TabIndex = 0;
            // 
            // lblTitulo
            // 
            lblTitulo.Dock = DockStyle.Fill;
            lblTitulo.Font = new Font("Segoe UI", 19.8000011F, FontStyle.Bold | FontStyle.Italic);
            lblTitulo.Location = new Point(0, 0);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(723, 70);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Titulo do aviso";
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panelDeMensagem
            // 
            panelDeMensagem.AutoScroll = true;
            panelDeMensagem.BackColor = Color.Transparent;
            panelDeMensagem.Controls.Add(LblMensagem);
            panelDeMensagem.Controls.Add(pictureBoxSucesso);
            panelDeMensagem.Controls.Add(pictureBoxError);
            panelDeMensagem.Controls.Add(pictureBoxAviso);
            panelDeMensagem.Dock = DockStyle.Top;
            panelDeMensagem.Location = new Point(0, 70);
            panelDeMensagem.Name = "panelDeMensagem";
            panelDeMensagem.Size = new Size(723, 250);
            panelDeMensagem.TabIndex = 10;
            // 
            // LblMensagem
            // 
            LblMensagem.Dock = DockStyle.Fill;
            LblMensagem.Font = new Font("Segoe UI", 19.8000011F, FontStyle.Bold | FontStyle.Italic);
            LblMensagem.Location = new Point(0, 336);
            LblMensagem.Name = "LblMensagem";
            LblMensagem.Size = new Size(702, 0);
            LblMensagem.TabIndex = 3;
            LblMensagem.Text = "Mensagem de erro que vai aparecer aqui caso de um erro u um aviso de mensagem ";
            LblMensagem.TextAlign = ContentAlignment.TopCenter;
            // 
            // pictureBoxSucesso
            // 
            pictureBoxSucesso.Dock = DockStyle.Top;
            pictureBoxSucesso.Image = (Image)resources.GetObject("pictureBoxSucesso.Image");
            pictureBoxSucesso.Location = new Point(0, 224);
            pictureBoxSucesso.Name = "pictureBoxSucesso";
            pictureBoxSucesso.Size = new Size(702, 112);
            pictureBoxSucesso.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxSucesso.TabIndex = 2;
            pictureBoxSucesso.TabStop = false;
            pictureBoxSucesso.Visible = false;
            // 
            // pictureBoxError
            // 
            pictureBoxError.Dock = DockStyle.Top;
            pictureBoxError.Image = (Image)resources.GetObject("pictureBoxError.Image");
            pictureBoxError.Location = new Point(0, 112);
            pictureBoxError.Name = "pictureBoxError";
            pictureBoxError.Size = new Size(702, 112);
            pictureBoxError.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxError.TabIndex = 1;
            pictureBoxError.TabStop = false;
            pictureBoxError.Visible = false;
            // 
            // pictureBoxAviso
            // 
            pictureBoxAviso.Dock = DockStyle.Top;
            pictureBoxAviso.ErrorImage = (Image)resources.GetObject("pictureBoxAviso.ErrorImage");
            pictureBoxAviso.Image = (Image)resources.GetObject("pictureBoxAviso.Image");
            pictureBoxAviso.Location = new Point(0, 0);
            pictureBoxAviso.Margin = new Padding(3, 30, 3, 3);
            pictureBoxAviso.Name = "pictureBoxAviso";
            pictureBoxAviso.Size = new Size(702, 112);
            pictureBoxAviso.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxAviso.TabIndex = 0;
            pictureBoxAviso.TabStop = false;
            // 
            // SysAlerta
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            BackColor = SystemColors.ControlDark;
            ClientSize = new Size(723, 433);
            Controls.Add(panelDeMensagem);
            Controls.Add(panelTituloContainer);
            Controls.Add(BtnOk);
            Controls.Add(BtnNao);
            Controls.Add(BtnSim);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "SysAlerta";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SysAlerta";
            panelTituloContainer.ResumeLayout(false);
            panelDeTitulo.ResumeLayout(false);
            panelDeMensagem.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxSucesso).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxError).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxAviso).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Button BtnSim;
        private Button BtnNao;
        private Button BtnOk;
        private Panel panelTituloContainer;
        private Panel panelDeTitulo;
        private Panel panelDeMensagem;
        private Label lblTitulo;
        private PictureBox pictureBoxAviso;
        private PictureBox pictureBoxError;
        private Label LblMensagem;
        private PictureBox pictureBoxSucesso;
    }
}