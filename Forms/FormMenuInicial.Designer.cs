namespace SysIntegradorApp
{
    partial class FormMenuInicial
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
        public void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMenuInicial));
            panelPedidos = new FlowLayoutPanel();
            panelDetalhePedido = new FlowLayoutPanel();
            labelDeAvisoPedidoDetalhe = new Label();
            panel1 = new Panel();
            ChatLabel = new Label();
            pictureBox3 = new PictureBox();
            label7 = new Label();
            pictureBox2 = new PictureBox();
            progressBar1 = new ProgressBar();
            label6 = new Label();
            BtnBuscar = new Button();
            textBoxBuscarPedido = new TextBox();
            label5 = new Label();
            pictureBoxLupa = new PictureBox();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            pictureBoxDelivery = new PictureBox();
            pictureBoxConfig = new PictureBox();
            pictureBoxChat = new PictureBox();
            pictureBoxHome = new PictureBox();
            pictureBox1 = new PictureBox();
            panelStatusLoja = new Panel();
            pictureBoxOfline = new PictureBox();
            pictureBoxOnline = new PictureBox();
            labelStatusLojaNM = new Label();
            pollingManual = new Button();
            checkBoxConcluido = new CheckBox();
            panelDePaginas = new FlowLayoutPanel();
            checkBoxDespachados = new CheckBox();
            checkBoxConfirmados = new CheckBox();
            notifyIcon1 = new NotifyIcon(components);
            panelDetalhePedido.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxLupa).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxDelivery).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxConfig).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxChat).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxHome).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panelStatusLoja.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOfline).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOnline).BeginInit();
            panelDePaginas.SuspendLayout();
            SuspendLayout();
            // 
            // panelPedidos
            // 
            panelPedidos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            panelPedidos.AutoScroll = true;
            panelPedidos.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelPedidos.BackColor = SystemColors.ButtonHighlight;
            panelPedidos.FlowDirection = FlowDirection.TopDown;
            panelPedidos.Location = new Point(13, 101);
            panelPedidos.Margin = new Padding(3, 300, 3, 3);
            panelPedidos.Name = "panelPedidos";
            panelPedidos.Size = new Size(380, 500);
            panelPedidos.TabIndex = 0;
            panelPedidos.WrapContents = false;
            panelPedidos.Paint += panelPedidos_Paint;
            // 
            // panelDetalhePedido
            // 
            panelDetalhePedido.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelDetalhePedido.AutoScroll = true;
            panelDetalhePedido.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelDetalhePedido.BackColor = SystemColors.ButtonHighlight;
            panelDetalhePedido.Controls.Add(labelDeAvisoPedidoDetalhe);
            panelDetalhePedido.FlowDirection = FlowDirection.TopDown;
            panelDetalhePedido.Location = new Point(419, 101);
            panelDetalhePedido.Margin = new Padding(0);
            panelDetalhePedido.Name = "panelDetalhePedido";
            panelDetalhePedido.Size = new Size(974, 541);
            panelDetalhePedido.TabIndex = 1;
            panelDetalhePedido.WrapContents = false;
            // 
            // labelDeAvisoPedidoDetalhe
            // 
            labelDeAvisoPedidoDetalhe.AutoSize = true;
            labelDeAvisoPedidoDetalhe.Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelDeAvisoPedidoDetalhe.ForeColor = Color.Red;
            labelDeAvisoPedidoDetalhe.Location = new Point(50, 300);
            labelDeAvisoPedidoDetalhe.Margin = new Padding(50, 300, 0, 0);
            labelDeAvisoPedidoDetalhe.Name = "labelDeAvisoPedidoDetalhe";
            labelDeAvisoPedidoDetalhe.Size = new Size(918, 70);
            labelDeAvisoPedidoDetalhe.TabIndex = 0;
            labelDeAvisoPedidoDetalhe.Text = "Nenhum pedido Selecionado para mostrar seus detalhes, Clique em um pedido para obter seus detalhes.";
            labelDeAvisoPedidoDetalhe.TextAlign = ContentAlignment.TopCenter;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel1.BackColor = SystemColors.ButtonHighlight;
            panel1.Controls.Add(ChatLabel);
            panel1.Controls.Add(pictureBox3);
            panel1.Controls.Add(label7);
            panel1.Controls.Add(pictureBox2);
            panel1.Controls.Add(progressBar1);
            panel1.Controls.Add(label6);
            panel1.Controls.Add(BtnBuscar);
            panel1.Controls.Add(textBoxBuscarPedido);
            panel1.Controls.Add(label5);
            panel1.Controls.Add(pictureBoxLupa);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(pictureBoxDelivery);
            panel1.Controls.Add(pictureBoxConfig);
            panel1.Controls.Add(pictureBoxChat);
            panel1.Controls.Add(pictureBoxHome);
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(panelStatusLoja);
            panel1.Controls.Add(pollingManual);
            panel1.Location = new Point(10, 10);
            panel1.Name = "panel1";
            panel1.Size = new Size(1383, 70);
            panel1.TabIndex = 2;
            // 
            // ChatLabel
            // 
            ChatLabel.AutoSize = true;
            ChatLabel.Font = new Font("Segoe UI Semibold", 7.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ChatLabel.Location = new Point(1099, 52);
            ChatLabel.Name = "ChatLabel";
            ChatLabel.Size = new Size(36, 17);
            ChatLabel.TabIndex = 24;
            ChatLabel.Text = "Chat";
            ChatLabel.Visible = false;
            // 
            // pictureBox3
            // 
            pictureBox3.Cursor = Cursors.Hand;
            pictureBox3.Image = (Image)resources.GetObject("pictureBox3.Image");
            pictureBox3.Location = new Point(1099, 6);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(51, 45);
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.TabIndex = 23;
            pictureBox3.TabStop = false;
            pictureBox3.Visible = false;
            pictureBox3.Click += pictureBox3_Click;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI Semibold", 7.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label7.Location = new Point(478, 49);
            label7.Name = "label7";
            label7.Size = new Size(53, 17);
            label7.TabIndex = 22;
            label7.Text = "Params";
            // 
            // pictureBox2
            // 
            pictureBox2.Cursor = Cursors.Hand;
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(479, 3);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(51, 45);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 21;
            pictureBox2.TabStop = false;
            pictureBox2.Click += pictureBox2_Click;
            // 
            // progressBar1
            // 
            progressBar1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            progressBar1.Location = new Point(1217, 52);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(125, 14);
            progressBar1.Step = 1;
            progressBar1.TabIndex = 20;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 6F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.Location = new Point(585, 3);
            label6.Name = "label6";
            label6.Size = new Size(16, 12);
            label6.TabIndex = 18;
            label6.Text = "F2";
            // 
            // BtnBuscar
            // 
            BtnBuscar.Cursor = Cursors.Hand;
            BtnBuscar.FlatAppearance.MouseOverBackColor = Color.FromArgb(224, 224, 224);
            BtnBuscar.Font = new Font("Segoe UI", 12F);
            BtnBuscar.Location = new Point(843, 9);
            BtnBuscar.Name = "BtnBuscar";
            BtnBuscar.Size = new Size(104, 37);
            BtnBuscar.TabIndex = 17;
            BtnBuscar.Text = "Pesquisar";
            BtnBuscar.UseVisualStyleBackColor = true;
            BtnBuscar.Visible = false;
            BtnBuscar.Click += BtnBuscar_Click;
            // 
            // textBoxBuscarPedido
            // 
            textBoxBuscarPedido.Font = new Font("Segoe UI", 12F);
            textBoxBuscarPedido.Location = new Point(593, 12);
            textBoxBuscarPedido.Name = "textBoxBuscarPedido";
            textBoxBuscarPedido.PlaceholderText = "Digite o número do pedido";
            textBoxBuscarPedido.Size = new Size(244, 34);
            textBoxBuscarPedido.TabIndex = 16;
            textBoxBuscarPedido.Visible = false;
            textBoxBuscarPedido.KeyPress += textBoxBuscarPedido_KeyPress;
            textBoxBuscarPedido.Leave += textBoxBuscarPedido_Leave;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI Semibold", 7.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(539, 49);
            label5.Name = "label5";
            label5.Size = new Size(48, 17);
            label5.TabIndex = 15;
            label5.Text = "Buscar";
            // 
            // pictureBoxLupa
            // 
            pictureBoxLupa.Cursor = Cursors.Hand;
            pictureBoxLupa.Image = (Image)resources.GetObject("pictureBoxLupa.Image");
            pictureBoxLupa.Location = new Point(536, 3);
            pictureBoxLupa.Name = "pictureBoxLupa";
            pictureBoxLupa.Size = new Size(51, 45);
            pictureBoxLupa.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxLupa.TabIndex = 14;
            pictureBoxLupa.TabStop = false;
            pictureBoxLupa.Click += pictureBoxLupa_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI Semibold", 7.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.Location = new Point(425, 49);
            label4.Name = "label4";
            label4.Size = new Size(44, 17);
            label4.TabIndex = 13;
            label4.Text = "Portal";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 7.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(369, 49);
            label3.Name = "label3";
            label3.Size = new Size(50, 17);
            label3.TabIndex = 12;
            label3.Text = "Config.";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 7.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(305, 49);
            label2.Name = "label2";
            label2.Size = new Size(61, 17);
            label2.TabIndex = 11;
            label2.Text = "Entregas";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 7.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(255, 49);
            label1.Name = "label1";
            label1.Size = new Size(45, 17);
            label1.TabIndex = 10;
            label1.Text = "Home";
            // 
            // pictureBoxDelivery
            // 
            pictureBoxDelivery.Cursor = Cursors.Hand;
            pictureBoxDelivery.ErrorImage = (Image)resources.GetObject("pictureBoxDelivery.ErrorImage");
            pictureBoxDelivery.Image = (Image)resources.GetObject("pictureBoxDelivery.Image");
            pictureBoxDelivery.Location = new Point(308, 3);
            pictureBoxDelivery.Name = "pictureBoxDelivery";
            pictureBoxDelivery.Size = new Size(51, 45);
            pictureBoxDelivery.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxDelivery.TabIndex = 9;
            pictureBoxDelivery.TabStop = false;
            pictureBoxDelivery.Click += pictureBoxDelivery_Click;
            // 
            // pictureBoxConfig
            // 
            pictureBoxConfig.Cursor = Cursors.Hand;
            pictureBoxConfig.ErrorImage = (Image)resources.GetObject("pictureBoxConfig.ErrorImage");
            pictureBoxConfig.Image = (Image)resources.GetObject("pictureBoxConfig.Image");
            pictureBoxConfig.Location = new Point(365, 3);
            pictureBoxConfig.Name = "pictureBoxConfig";
            pictureBoxConfig.Size = new Size(51, 45);
            pictureBoxConfig.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxConfig.TabIndex = 4;
            pictureBoxConfig.TabStop = false;
            pictureBoxConfig.Click += pictureBoxConfig_Click;
            // 
            // pictureBoxChat
            // 
            pictureBoxChat.Cursor = Cursors.Hand;
            pictureBoxChat.Image = (Image)resources.GetObject("pictureBoxChat.Image");
            pictureBoxChat.Location = new Point(422, 3);
            pictureBoxChat.Name = "pictureBoxChat";
            pictureBoxChat.Size = new Size(51, 45);
            pictureBoxChat.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxChat.TabIndex = 3;
            pictureBoxChat.TabStop = false;
            pictureBoxChat.Click += pictureBoxChat_Click;
            // 
            // pictureBoxHome
            // 
            pictureBoxHome.Cursor = Cursors.Hand;
            pictureBoxHome.Image = (Image)resources.GetObject("pictureBoxHome.Image");
            pictureBoxHome.Location = new Point(251, 3);
            pictureBoxHome.Name = "pictureBoxHome";
            pictureBoxHome.Size = new Size(51, 45);
            pictureBoxHome.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxHome.TabIndex = 2;
            pictureBoxHome.TabStop = false;
            pictureBoxHome.Click += pictureBoxHome_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(0, -27);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(237, 128);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // panelStatusLoja
            // 
            panelStatusLoja.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            panelStatusLoja.Controls.Add(pictureBoxOfline);
            panelStatusLoja.Controls.Add(pictureBoxOnline);
            panelStatusLoja.Controls.Add(labelStatusLojaNM);
            panelStatusLoja.Location = new Point(1208, 3);
            panelStatusLoja.Name = "panelStatusLoja";
            panelStatusLoja.Size = new Size(145, 47);
            panelStatusLoja.TabIndex = 7;
            // 
            // pictureBoxOfline
            // 
            pictureBoxOfline.Image = (Image)resources.GetObject("pictureBoxOfline.Image");
            pictureBoxOfline.Location = new Point(106, 12);
            pictureBoxOfline.Name = "pictureBoxOfline";
            pictureBoxOfline.Size = new Size(35, 23);
            pictureBoxOfline.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxOfline.TabIndex = 10;
            pictureBoxOfline.TabStop = false;
            // 
            // pictureBoxOnline
            // 
            pictureBoxOnline.Image = (Image)resources.GetObject("pictureBoxOnline.Image");
            pictureBoxOnline.Location = new Point(106, 12);
            pictureBoxOnline.Name = "pictureBoxOnline";
            pictureBoxOnline.Size = new Size(35, 23);
            pictureBoxOnline.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxOnline.TabIndex = 6;
            pictureBoxOnline.TabStop = false;
            pictureBoxOnline.Visible = false;
            // 
            // labelStatusLojaNM
            // 
            labelStatusLojaNM.AutoSize = true;
            labelStatusLojaNM.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelStatusLojaNM.Location = new Point(14, 12);
            labelStatusLojaNM.Name = "labelStatusLojaNM";
            labelStatusLojaNM.Size = new Size(86, 23);
            labelStatusLojaNM.TabIndex = 5;
            labelStatusLojaNM.Text = "Loja Oline";
            // 
            // pollingManual
            // 
            pollingManual.FlatAppearance.BorderColor = Color.Red;
            pollingManual.FlatAppearance.BorderSize = 3;
            pollingManual.FlatStyle = FlatStyle.Flat;
            pollingManual.ForeColor = Color.Red;
            pollingManual.Location = new Point(1082, 10);
            pollingManual.Name = "pollingManual";
            pollingManual.Size = new Size(134, 40);
            pollingManual.TabIndex = 8;
            pollingManual.Text = "Polling";
            pollingManual.UseVisualStyleBackColor = true;
            pollingManual.Visible = false;
            pollingManual.Click += pollingManual_Click;
            // 
            // checkBoxConcluido
            // 
            checkBoxConcluido.AutoSize = true;
            checkBoxConcluido.Location = new Point(18, 6);
            checkBoxConcluido.Margin = new Padding(18, 3, 3, 3);
            checkBoxConcluido.Name = "checkBoxConcluido";
            checkBoxConcluido.Size = new Size(104, 24);
            checkBoxConcluido.TabIndex = 0;
            checkBoxConcluido.Text = "Concluidos";
            checkBoxConcluido.UseVisualStyleBackColor = true;
            checkBoxConcluido.Click += checkBoxConcluido_Click;
            // 
            // panelDePaginas
            // 
            panelDePaginas.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            panelDePaginas.AutoScroll = true;
            panelDePaginas.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelDePaginas.BackColor = SystemColors.ButtonHighlight;
            panelDePaginas.Controls.Add(checkBoxConcluido);
            panelDePaginas.Controls.Add(checkBoxDespachados);
            panelDePaginas.Controls.Add(checkBoxConfirmados);
            panelDePaginas.Location = new Point(13, 608);
            panelDePaginas.Margin = new Padding(3, 300, 3, 3);
            panelDePaginas.Name = "panelDePaginas";
            panelDePaginas.Padding = new Padding(0, 3, 0, 0);
            panelDePaginas.Size = new Size(380, 34);
            panelDePaginas.TabIndex = 3;
            panelDePaginas.WrapContents = false;
            // 
            // checkBoxDespachados
            // 
            checkBoxDespachados.AutoSize = true;
            checkBoxDespachados.Checked = true;
            checkBoxDespachados.CheckState = CheckState.Checked;
            checkBoxDespachados.Location = new Point(128, 6);
            checkBoxDespachados.Name = "checkBoxDespachados";
            checkBoxDespachados.Size = new Size(120, 24);
            checkBoxDespachados.TabIndex = 1;
            checkBoxDespachados.Text = "Despachados";
            checkBoxDespachados.UseVisualStyleBackColor = true;
            checkBoxDespachados.CheckedChanged += checkBoxDespachados_CheckedChanged;
            // 
            // checkBoxConfirmados
            // 
            checkBoxConfirmados.AutoSize = true;
            checkBoxConfirmados.Checked = true;
            checkBoxConfirmados.CheckState = CheckState.Checked;
            checkBoxConfirmados.Location = new Point(254, 6);
            checkBoxConfirmados.Name = "checkBoxConfirmados";
            checkBoxConfirmados.Size = new Size(116, 24);
            checkBoxConfirmados.TabIndex = 2;
            checkBoxConfirmados.Text = "Confirmados";
            checkBoxConfirmados.UseVisualStyleBackColor = true;
            checkBoxConfirmados.CheckedChanged += checkBoxConfirmados_CheckedChanged;
            // 
            // notifyIcon1
            // 
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon1.BalloonTipText = "SysIntegradorApp";
            notifyIcon1.BalloonTipTitle = "SysIntegradorApp";
            notifyIcon1.Icon = (Icon)resources.GetObject("notifyIcon1.Icon");
            notifyIcon1.Tag = "SysIntegradorApp";
            notifyIcon1.Text = "SysIntegradorApp";
            notifyIcon1.Visible = true;
            notifyIcon1.BalloonTipClicked += notifyIcon1_BalloonTipClicked;
            notifyIcon1.Click += notifyIcon1_Click;
            notifyIcon1.MouseDoubleClick += notifyIcon1_MouseDoubleClick;
            // 
            // FormMenuInicial
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = SystemColors.ButtonShadow;
            ClientSize = new Size(1403, 655);
            Controls.Add(panelDePaginas);
            Controls.Add(panel1);
            Controls.Add(panelPedidos);
            Controls.Add(panelDetalhePedido);
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            Name = "FormMenuInicial";
            Padding = new Padding(10);
            StartPosition = FormStartPosition.CenterParent;
            Text = "Integrador De Apps SysLogica";
            WindowState = FormWindowState.Maximized;
            FormClosed += FormMenuInicial_FormClosed;
            Load += FormMenuInicial_Load;
            Shown += FormMenuInicial_Shown;
            KeyDown += FormMenuInicial_KeyDown;
            KeyPress += FormMenuInicial_KeyPress;
            panelDetalhePedido.ResumeLayout(false);
            panelDetalhePedido.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxLupa).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxDelivery).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxConfig).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxChat).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxHome).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panelStatusLoja.ResumeLayout(false);
            panelStatusLoja.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOfline).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOnline).EndInit();
            panelDePaginas.ResumeLayout(false);
            panelDePaginas.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private PictureBox pictureBox1;
        private PictureBox pictureBoxHome;
        private PictureBox pictureBoxConfig;
        private PictureBox pictureBoxChat;
        private Label labelStatusLojaNM;
        private Panel panelStatusLoja;
        private Button pollingManual;
        private PictureBox pictureBoxDelivery;
        private Label label1;
        private Label label3;
        private Label label2;
        private Label label4;
        private Label label5;
        private PictureBox pictureBoxLupa;
        private Button BtnBuscar;
        private TextBox textBoxBuscarPedido;
        private Label label6;
        private Label label7;
        private PictureBox pictureBox2;
        public NotifyIcon notifyIcon1;
        private Label ChatLabel;
        private PictureBox pictureBox3;
        private static CheckBox checkBoxConcluido;
        public static FlowLayoutPanel panelDePaginas;
        private static CheckBox checkBoxDespachados;
        private static CheckBox checkBoxConfirmados;
        private static ProgressBar progressBar1;
        private static PictureBox pictureBoxOnline;
        private static PictureBox pictureBoxOfline;
        public static Label labelDeAvisoPedidoDetalhe;
        public static FlowLayoutPanel panelDetalhePedido;
        public static FlowLayoutPanel panelPedidos;
    }
}

/*      private static CheckBox checkBoxConcluido;
        public static FlowLayoutPanel panelDePaginas;
        private static CheckBox checkBoxDespachados;
        private static CheckBox checkBoxConfirmados;
        private static ProgressBar progressBar1;
        private static PictureBox pictureBoxOnline;
        private static PictureBox pictureBoxOfline;
        public static Label labelDeAvisoPedidoDetalhe;
        public static FlowLayoutPanel panelDetalhePedido;
        public static FlowLayoutPanel panelPedidos;;*/