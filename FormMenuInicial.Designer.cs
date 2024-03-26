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
            panel1 = new Panel();
            pictureBox1 = new PictureBox();
            btnTeste = new Button();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
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
            panelDetalhePedido.FlowDirection = FlowDirection.TopDown;
            panelDetalhePedido.Location = new Point(419, 101);
            panelDetalhePedido.Margin = new Padding(0);
            panelDetalhePedido.Name = "panelDetalhePedido";
            panelDetalhePedido.Size = new Size(974, 541);
            panelDetalhePedido.TabIndex = 1;
            panelDetalhePedido.WrapContents = false;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel1.BackColor = SystemColors.ButtonHighlight;
            panel1.Controls.Add(btnTeste);
            panel1.Controls.Add(pictureBox1);
            panel1.Location = new Point(10, 10);
            panel1.Name = "panel1";
            panel1.Size = new Size(1383, 70);
            panel1.TabIndex = 2;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(-12, -80);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(352, 240);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // btnTeste
            // 
            btnTeste.Cursor = Cursors.Hand;
            btnTeste.FlatAppearance.BorderColor = Color.Red;
            btnTeste.FlatAppearance.BorderSize = 3;
            btnTeste.FlatStyle = FlatStyle.Flat;
            btnTeste.Font = new Font("Segoe UI", 11F);
            btnTeste.ForeColor = Color.Red;
            btnTeste.Location = new Point(378, 13);
            btnTeste.Name = "btnTeste";
            btnTeste.Size = new Size(151, 45);
            btnTeste.TabIndex = 1;
            btnTeste.Text = "Teste";
            btnTeste.UseVisualStyleBackColor = true;
            btnTeste.Click += btnTeste_Click;
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
            Text = "FormMenuInicial";
            FormClosed += FormMenuInicial_FormClosed;
            Load += FormMenuInicial_Load;
            Shown += FormMenuInicial_Shown;
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private PictureBox pictureBox1;
        private Button btnTeste;
        public  static FlowLayoutPanel panelDetalhePedido;
        public  static FlowLayoutPanel panelPedidos;
    }
}