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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMenuInicial));
            panelPedidos = new FlowLayoutPanel();
            panelDetalhePedido = new FlowLayoutPanel();
            labelDeAvisoPedidoDetalhe = new Label();
            panel1 = new Panel();
            pictureBoxDelivery = new PictureBox();
            pollingManual = new Button();
            pictureBoxConfig = new PictureBox();
            pictureBoxChat = new PictureBox();
            pictureBoxHome = new PictureBox();
            pictureBox1 = new PictureBox();
            panelStatusLoja = new Panel();
            pictureBoxOfline = new PictureBox();
            pictureBoxOnline = new PictureBox();
            labelStatusLojaNM = new Label();
            panelDetalhePedido.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxDelivery).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxConfig).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxChat).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxHome).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panelStatusLoja.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOfline).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOnline).BeginInit();
            SuspendLayout();
            // 
            // panelPedidos
            // 
            panelPedidos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            panelPedidos.AutoScroll = true;
            panelPedidos.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelPedidos.BackColor = SystemColors.ButtonHighlight;
            panelPedidos.BorderStyle = BorderStyle.FixedSingle;
            panelPedidos.FlowDirection = FlowDirection.TopDown;
            panelPedidos.Location = new Point(13, 101);
            panelPedidos.Margin = new Padding(3, 300, 3, 3);
            panelPedidos.Name = "panelPedidos";
            panelPedidos.Size = new Size(380, 541);
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
            panelDetalhePedido.BorderStyle = BorderStyle.FixedSingle;
            panelDetalhePedido.Controls.Add(labelDeAvisoPedidoDetalhe);
            panelDetalhePedido.FlowDirection = FlowDirection.TopDown;
            panelDetalhePedido.Location = new Point(419, 101);
            panelDetalhePedido.Margin = new Padding(0);
            panelDetalhePedido.Name = "panelDetalhePedido";
            panelDetalhePedido.Padding = new Padding(85, 3, 0, 3);
            panelDetalhePedido.Size = new Size(974, 541);
            panelDetalhePedido.TabIndex = 1;
            panelDetalhePedido.WrapContents = false;
            // 
            // labelDeAvisoPedidoDetalhe
            // 
            labelDeAvisoPedidoDetalhe.AutoSize = true;
            labelDeAvisoPedidoDetalhe.Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelDeAvisoPedidoDetalhe.ForeColor = Color.Red;
            labelDeAvisoPedidoDetalhe.Location = new Point(135, 303);
            labelDeAvisoPedidoDetalhe.Margin = new Padding(50, 300, 0, 0);
            labelDeAvisoPedidoDetalhe.Name = "labelDeAvisoPedidoDetalhe";
            labelDeAvisoPedidoDetalhe.Size = new Size(831, 70);
            labelDeAvisoPedidoDetalhe.TabIndex = 0;
            labelDeAvisoPedidoDetalhe.Text = "Nenhum pedido Selecionado para mostrar seus detalhes, Clique em um pedido para obter seus detalhes.";
            labelDeAvisoPedidoDetalhe.TextAlign = ContentAlignment.TopCenter;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel1.BackColor = SystemColors.ButtonHighlight;
            panel1.Controls.Add(pictureBoxDelivery);
            panel1.Controls.Add(pollingManual);
            panel1.Controls.Add(pictureBoxConfig);
            panel1.Controls.Add(pictureBoxChat);
            panel1.Controls.Add(pictureBoxHome);
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(panelStatusLoja);
            panel1.Location = new Point(10, 10);
            panel1.Name = "panel1";
            panel1.Size = new Size(1383, 70);
            panel1.TabIndex = 2;
            // 
            // pictureBoxDelivery
            // 
            pictureBoxDelivery.Cursor = Cursors.Hand;
            pictureBoxDelivery.ErrorImage = (Image)resources.GetObject("pictureBoxDelivery.ErrorImage");
            pictureBoxDelivery.Image = (Image)resources.GetObject("pictureBoxDelivery.Image");
            pictureBoxDelivery.Location = new Point(448, 13);
            pictureBoxDelivery.Name = "pictureBoxDelivery";
            pictureBoxDelivery.Size = new Size(51, 45);
            pictureBoxDelivery.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxDelivery.TabIndex = 9;
            pictureBoxDelivery.TabStop = false;
            pictureBoxDelivery.Click += pictureBoxDelivery_Click;
            // 
            // pollingManual
            // 
            pollingManual.FlatAppearance.BorderColor = Color.Red;
            pollingManual.FlatAppearance.BorderSize = 3;
            pollingManual.FlatStyle = FlatStyle.Flat;
            pollingManual.ForeColor = Color.Red;
            pollingManual.Location = new Point(849, 13);
            pollingManual.Name = "pollingManual";
            pollingManual.Size = new Size(123, 39);
            pollingManual.TabIndex = 8;
            pollingManual.Text = "Polling";
            pollingManual.UseVisualStyleBackColor = true;
            pollingManual.Visible = false;
            pollingManual.Click += pollingManual_Click;
            // 
            // pictureBoxConfig
            // 
            pictureBoxConfig.Cursor = Cursors.Hand;
            pictureBoxConfig.ErrorImage = (Image)resources.GetObject("pictureBoxConfig.ErrorImage");
            pictureBoxConfig.Image = (Image)resources.GetObject("pictureBoxConfig.Image");
            pictureBoxConfig.Location = new Point(523, 13);
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
            pictureBoxChat.Location = new Point(597, 13);
            pictureBoxChat.Name = "pictureBoxChat";
            pictureBoxChat.Size = new Size(51, 45);
            pictureBoxChat.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxChat.TabIndex = 3;
            pictureBoxChat.TabStop = false;
            // 
            // pictureBoxHome
            // 
            pictureBoxHome.Cursor = Cursors.Hand;
            pictureBoxHome.Image = (Image)resources.GetObject("pictureBoxHome.Image");
            pictureBoxHome.Location = new Point(376, 13);
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
            pictureBox1.Location = new Point(-17, -84);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(352, 240);
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
            panelStatusLoja.Location = new Point(1200, 11);
            panelStatusLoja.Name = "panelStatusLoja";
            panelStatusLoja.Size = new Size(169, 47);
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
            // FormMenuInicial
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = SystemColors.ButtonShadow;
            ClientSize = new Size(1403, 655);
            Controls.Add(panel1);
            Controls.Add(panelPedidos);
            Controls.Add(panelDetalhePedido);
            Name = "FormMenuInicial";
            Padding = new Padding(10);
            StartPosition = FormStartPosition.CenterParent;
            Text = "Integrador De Apps SysLogica";
            WindowState = FormWindowState.Maximized;
            FormClosed += FormMenuInicial_FormClosed;
            Load += FormMenuInicial_Load;
            Shown += FormMenuInicial_Shown;
            panelDetalhePedido.ResumeLayout(false);
            panelDetalhePedido.PerformLayout();
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxDelivery).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxConfig).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxChat).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxHome).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panelStatusLoja.ResumeLayout(false);
            panelStatusLoja.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOfline).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOnline).EndInit();
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
        private static PictureBox pictureBoxOnline;
        private static PictureBox pictureBoxOfline;
        public static Label labelDeAvisoPedidoDetalhe;
        public static FlowLayoutPanel panelDetalhePedido;
        public static FlowLayoutPanel panelPedidos;
    }
}