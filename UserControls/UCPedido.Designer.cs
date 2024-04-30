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
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
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
            labelNumPedido.Location = new Point(133, 0);
            labelNumPedido.Name = "labelNumPedido";
            labelNumPedido.Size = new Size(98, 38);
            labelNumPedido.TabIndex = 1;
            labelNumPedido.Text = "#8686\r\n";
            // 
            // labelNomePedido
            // 
            labelNomePedido.AutoSize = true;
            labelNomePedido.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelNomePedido.Location = new Point(133, 47);
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
            // 
            // labelHorarioDeEntrega
            // 
            labelHorarioDeEntrega.AutoSize = true;
            labelHorarioDeEntrega.Location = new Point(223, 72);
            labelHorarioDeEntrega.Name = "labelHorarioDeEntrega";
            labelHorarioDeEntrega.Size = new Size(44, 20);
            labelHorarioDeEntrega.TabIndex = 4;
            labelHorarioDeEntrega.Text = "10:40";
            // 
            // labelStatus
            // 
            labelStatus.AutoSize = true;
            labelStatus.Location = new Point(133, 92);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(53, 20);
            labelStatus.TabIndex = 5;
            labelStatus.Text = "Placed";
            // 
            // UCPedido
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(labelStatus);
            Controls.Add(labelHorarioDeEntrega);
            Controls.Add(labelEntregarAte);
            Controls.Add(labelNomePedido);
            Controls.Add(labelNumPedido);
            Controls.Add(pictureBox1);
            Cursor = Cursors.Hand;
            Name = "UCPedido";
            Size = new Size(350, 116);
            Load += UCPedido_Load;
            Click += UCPedido_Click;
            Enter += UCPedido_Enter;
            Leave += UCPedido_Leave;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
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
    }
}
