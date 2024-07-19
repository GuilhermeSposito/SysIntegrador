namespace SysIntegradorApp
{
    partial class UCPedido
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCPedido));
            pictureBox1 = new PictureBox();
            labelNumPedido = new Label();
            labelNomePedido = new Label();
            labelEntregarAte = new Label();
            labelHorarioDeEntrega = new Label();
            labelStatus = new Label();
            label1 = new Label();
            labelNumConta = new Label();
            pictureBoxImp = new PictureBox();
            pictureBoxDELMATCH = new PictureBox();
            pictureBoxOnPedido = new PictureBox();
            pictureBoxPedidoEnviado = new PictureBox();
            pictureBoxPedidoNaoEnviado = new PictureBox();
            pictureBoxAgendada = new PictureBox();
            pictureBoxCCM = new PictureBox();
            pictureBoxDeMEsa = new PictureBox();
            pictureBoxAnotaAi = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxImp).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxDELMATCH).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOnPedido).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPedidoEnviado).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPedidoNaoEnviado).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxAgendada).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxCCM).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxDeMEsa).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxAnotaAi).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(0, -9);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(126, 137);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // labelNumPedido
            // 
            labelNumPedido.AutoSize = true;
            labelNumPedido.Font = new Font("Segoe UI Semibold", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelNumPedido.Location = new Point(132, -6);
            labelNumPedido.Name = "labelNumPedido";
            labelNumPedido.Size = new Size(98, 38);
            labelNumPedido.TabIndex = 1;
            labelNumPedido.Text = "#8686\r\n";
            labelNumPedido.Click += labelNumPedido_Click;
            // 
            // labelNomePedido
            // 
            labelNomePedido.AutoSize = true;
            labelNomePedido.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelNomePedido.Location = new Point(132, 32);
            labelNomePedido.Name = "labelNomePedido";
            labelNomePedido.Size = new Size(176, 25);
            labelNomePedido.TabIndex = 2;
            labelNomePedido.Text = "Guilherme Sposito";
            labelNomePedido.Click += labelNomePedido_Click;
            // 
            // labelEntregarAte
            // 
            labelEntregarAte.AutoSize = true;
            labelEntregarAte.Location = new Point(132, 72);
            labelEntregarAte.Name = "labelEntregarAte";
            labelEntregarAte.Size = new Size(99, 20);
            labelEntregarAte.TabIndex = 3;
            labelEntregarAte.Text = "Entregar Até: ";
            labelEntregarAte.Click += labelEntregarAte_Click;
            // 
            // labelHorarioDeEntrega
            // 
            labelHorarioDeEntrega.AutoSize = true;
            labelHorarioDeEntrega.Location = new Point(223, 72);
            labelHorarioDeEntrega.Name = "labelHorarioDeEntrega";
            labelHorarioDeEntrega.Size = new Size(44, 20);
            labelHorarioDeEntrega.TabIndex = 4;
            labelHorarioDeEntrega.Text = "10:40";
            labelHorarioDeEntrega.Click += labelHorarioDeEntrega_Click;
            // 
            // labelStatus
            // 
            labelStatus.AutoSize = true;
            labelStatus.Location = new Point(133, 92);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(53, 20);
            labelStatus.TabIndex = 5;
            labelStatus.Text = "Placed";
            labelStatus.Click += labelStatus_Click_1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 8F);
            label1.Location = new Point(133, 55);
            label1.Name = "label1";
            label1.Size = new Size(72, 19);
            label1.TabIndex = 6;
            label1.Text = "SysMenu: ";
            // 
            // labelNumConta
            // 
            labelNumConta.AutoSize = true;
            labelNumConta.Font = new Font("Segoe UI", 8F);
            labelNumConta.Location = new Point(197, 55);
            labelNumConta.Name = "labelNumConta";
            labelNumConta.Size = new Size(17, 19);
            labelNumConta.TabIndex = 7;
            labelNumConta.Text = "0";
            // 
            // pictureBoxImp
            // 
            pictureBoxImp.BackColor = Color.Transparent;
            pictureBoxImp.Image = (Image)resources.GetObject("pictureBoxImp.Image");
            pictureBoxImp.Location = new Point(323, 3);
            pictureBoxImp.Name = "pictureBoxImp";
            pictureBoxImp.Size = new Size(24, 24);
            pictureBoxImp.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxImp.TabIndex = 8;
            pictureBoxImp.TabStop = false;
            pictureBoxImp.Click += pictureBoxImp_Click;
            // 
            // pictureBoxDELMATCH
            // 
            pictureBoxDELMATCH.Image = (Image)resources.GetObject("pictureBoxDELMATCH.Image");
            pictureBoxDELMATCH.Location = new Point(0, -9);
            pictureBoxDELMATCH.Name = "pictureBoxDELMATCH";
            pictureBoxDELMATCH.Size = new Size(126, 137);
            pictureBoxDELMATCH.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxDELMATCH.TabIndex = 9;
            pictureBoxDELMATCH.TabStop = false;
            pictureBoxDELMATCH.Visible = false;
            pictureBoxDELMATCH.Click += pictureBoxDELMATCH_Click;
            // 
            // pictureBoxOnPedido
            // 
            pictureBoxOnPedido.Image = (Image)resources.GetObject("pictureBoxOnPedido.Image");
            pictureBoxOnPedido.Location = new Point(0, -9);
            pictureBoxOnPedido.Name = "pictureBoxOnPedido";
            pictureBoxOnPedido.Size = new Size(126, 137);
            pictureBoxOnPedido.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxOnPedido.TabIndex = 10;
            pictureBoxOnPedido.TabStop = false;
            pictureBoxOnPedido.Visible = false;
            pictureBoxOnPedido.Click += pictureBoxOnPedido_Click;
            // 
            // pictureBoxPedidoEnviado
            // 
            pictureBoxPedidoEnviado.Image = (Image)resources.GetObject("pictureBoxPedidoEnviado.Image");
            pictureBoxPedidoEnviado.Location = new Point(280, 3);
            pictureBoxPedidoEnviado.Name = "pictureBoxPedidoEnviado";
            pictureBoxPedidoEnviado.Size = new Size(42, 22);
            pictureBoxPedidoEnviado.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxPedidoEnviado.TabIndex = 11;
            pictureBoxPedidoEnviado.TabStop = false;
            pictureBoxPedidoEnviado.Visible = false;
            // 
            // pictureBoxPedidoNaoEnviado
            // 
            pictureBoxPedidoNaoEnviado.Image = (Image)resources.GetObject("pictureBoxPedidoNaoEnviado.Image");
            pictureBoxPedidoNaoEnviado.Location = new Point(275, 3);
            pictureBoxPedidoNaoEnviado.Name = "pictureBoxPedidoNaoEnviado";
            pictureBoxPedidoNaoEnviado.Size = new Size(42, 22);
            pictureBoxPedidoNaoEnviado.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxPedidoNaoEnviado.TabIndex = 12;
            pictureBoxPedidoNaoEnviado.TabStop = false;
            pictureBoxPedidoNaoEnviado.Visible = false;
            // 
            // pictureBoxAgendada
            // 
            pictureBoxAgendada.Image = (Image)resources.GetObject("pictureBoxAgendada.Image");
            pictureBoxAgendada.Location = new Point(275, 3);
            pictureBoxAgendada.Name = "pictureBoxAgendada";
            pictureBoxAgendada.Size = new Size(42, 22);
            pictureBoxAgendada.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxAgendada.TabIndex = 13;
            pictureBoxAgendada.TabStop = false;
            pictureBoxAgendada.Visible = false;
            // 
            // pictureBoxCCM
            // 
            pictureBoxCCM.Image = (Image)resources.GetObject("pictureBoxCCM.Image");
            pictureBoxCCM.Location = new Point(0, -9);
            pictureBoxCCM.Name = "pictureBoxCCM";
            pictureBoxCCM.Size = new Size(126, 137);
            pictureBoxCCM.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxCCM.TabIndex = 14;
            pictureBoxCCM.TabStop = false;
            pictureBoxCCM.Visible = false;
            // 
            // pictureBoxDeMEsa
            // 
            pictureBoxDeMEsa.Image = (Image)resources.GetObject("pictureBoxDeMEsa.Image");
            pictureBoxDeMEsa.Location = new Point(292, 60);
            pictureBoxDeMEsa.Name = "pictureBoxDeMEsa";
            pictureBoxDeMEsa.Size = new Size(58, 43);
            pictureBoxDeMEsa.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxDeMEsa.TabIndex = 15;
            pictureBoxDeMEsa.TabStop = false;
            pictureBoxDeMEsa.Visible = false;
            pictureBoxDeMEsa.Click += pictureBoxDeMEsa_Click;
            // 
            // pictureBoxAnotaAi
            // 
            pictureBoxAnotaAi.Image = (Image)resources.GetObject("pictureBoxAnotaAi.Image");
            pictureBoxAnotaAi.Location = new Point(0, -9);
            pictureBoxAnotaAi.Name = "pictureBoxAnotaAi";
            pictureBoxAnotaAi.Size = new Size(126, 137);
            pictureBoxAnotaAi.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxAnotaAi.TabIndex = 16;
            pictureBoxAnotaAi.TabStop = false;
            pictureBoxAnotaAi.Visible = false;
            // 
            // UCPedido
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(pictureBoxAnotaAi);
            Controls.Add(pictureBoxDeMEsa);
            Controls.Add(pictureBoxCCM);
            Controls.Add(pictureBoxAgendada);
            Controls.Add(pictureBoxPedidoNaoEnviado);
            Controls.Add(pictureBoxPedidoEnviado);
            Controls.Add(pictureBoxOnPedido);
            Controls.Add(pictureBoxImp);
            Controls.Add(labelNumConta);
            Controls.Add(label1);
            Controls.Add(labelStatus);
            Controls.Add(labelHorarioDeEntrega);
            Controls.Add(labelEntregarAte);
            Controls.Add(labelNomePedido);
            Controls.Add(labelNumPedido);
            Controls.Add(pictureBoxDELMATCH);
            Controls.Add(pictureBox1);
            Cursor = Cursors.Hand;
            Name = "UCPedido";
            Size = new Size(350, 116);
            Load += UCPedido_Load;
            Click += UCPedido_Click;
            Enter += UCPedido_Enter;
            KeyDown += UCPedido_KeyDown;
            KeyPress += UCPedido_KeyPress;
            Leave += UCPedido_Leave;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxImp).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxDELMATCH).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOnPedido).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPedidoEnviado).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPedidoNaoEnviado).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxAgendada).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxCCM).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxDeMEsa).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxAnotaAi).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Label labelNumPedido;
        private Label labelNomePedido;
        private Label labelEntregarAte;
        private Label labelHorarioDeEntrega;
        private Label labelStatus;
        private Label label1;
        private Label labelNumConta;
        private PictureBox pictureBoxImp;
        private PictureBox pictureBoxDELMATCH;
        private PictureBox pictureBoxOnPedido;
        private PictureBox pictureBoxPedidoEnviado;
        private PictureBox pictureBoxPedidoNaoEnviado;
        private PictureBox pictureBoxAgendada;
        private PictureBox pictureBoxCCM;
        private PictureBox pictureBoxDeMEsa;
        private PictureBox pictureBoxAnotaAi;
    }
}
