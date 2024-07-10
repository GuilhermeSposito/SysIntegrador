namespace SysIntegradorApp;

partial class DeliveryForm
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeliveryForm));
        panelDeListarPedidos = new Panel();
        label1 = new Label();
        label2 = new Label();
        panelDeIniciarEntrega = new Panel();
        pictureBoxEmpresaDelivery = new PictureBox();
        label3 = new Label();
        label4 = new Label();
        pictureBox1 = new PictureBox();
        pictureBox2 = new PictureBox();
        label6 = new Label();
        panelDeListarPedidos.SuspendLayout();
        panelDeIniciarEntrega.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pictureBoxEmpresaDelivery).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
        SuspendLayout();
        // 
        // panelDeListarPedidos
        // 
        panelDeListarPedidos.BackColor = Color.FromArgb(219, 95, 7);
        panelDeListarPedidos.Controls.Add(label1);
        panelDeListarPedidos.Cursor = Cursors.Hand;
        panelDeListarPedidos.Location = new Point(75, 103);
        panelDeListarPedidos.Name = "panelDeListarPedidos";
        panelDeListarPedidos.Size = new Size(726, 179);
        panelDeListarPedidos.TabIndex = 0;
        panelDeListarPedidos.Click += panelDeListarPedidos_Click;
        panelDeListarPedidos.MouseEnter += panelDeListarPedidos_MouseEnter;
        panelDeListarPedidos.MouseLeave += panelDeListarPedidos_MouseLeave;
        // 
        // label1
        // 
        label1.Font = new Font("Segoe UI", 15F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label1.ForeColor = Color.FromArgb(255, 246, 193);
        label1.Location = new Point(198, 26);
        label1.Name = "label1";
        label1.Size = new Size(330, 117);
        label1.TabIndex = 1;
        label1.Text = "Clique aqui para listar os pedidos";
        label1.TextAlign = ContentAlignment.MiddleCenter;
        label1.Click += label1_Click;
        label1.MouseEnter += label1_MouseEnter;
        // 
        // label2
        // 
        label2.Font = new Font("Segoe UI", 15F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label2.ForeColor = Color.FromArgb(255, 246, 193);
        label2.Location = new Point(198, 25);
        label2.Name = "label2";
        label2.Size = new Size(330, 117);
        label2.TabIndex = 2;
        label2.Text = "Clique aqui para iniciar um novo pedido";
        label2.TextAlign = ContentAlignment.MiddleCenter;
        label2.Click += label2_Click;
        label2.MouseEnter += label2_MouseEnter;
        label2.MouseLeave += label2_MouseLeave;
        // 
        // panelDeIniciarEntrega
        // 
        panelDeIniciarEntrega.BackColor = Color.FromArgb(219, 95, 7);
        panelDeIniciarEntrega.Controls.Add(label2);
        panelDeIniciarEntrega.Cursor = Cursors.Hand;
        panelDeIniciarEntrega.Location = new Point(75, 306);
        panelDeIniciarEntrega.Name = "panelDeIniciarEntrega";
        panelDeIniciarEntrega.Size = new Size(726, 179);
        panelDeIniciarEntrega.TabIndex = 1;
        panelDeIniciarEntrega.Click += panelDeIniciarEntrega_Click;
        panelDeIniciarEntrega.Paint += panelDeIniciarEntrega_Paint;
        panelDeIniciarEntrega.MouseEnter += panelDeIniciarEntrega_MouseEnter;
        panelDeIniciarEntrega.MouseLeave += panelDeIniciarEntrega_MouseLeave;
        // 
        // pictureBoxEmpresaDelivery
        // 
        pictureBoxEmpresaDelivery.Cursor = Cursors.Hand;
        pictureBoxEmpresaDelivery.Image = (Image)resources.GetObject("pictureBoxEmpresaDelivery.Image");
        pictureBoxEmpresaDelivery.Location = new Point(112, 563);
        pictureBoxEmpresaDelivery.Name = "pictureBoxEmpresaDelivery";
        pictureBoxEmpresaDelivery.Size = new Size(262, 123);
        pictureBoxEmpresaDelivery.SizeMode = PictureBoxSizeMode.Zoom;
        pictureBoxEmpresaDelivery.TabIndex = 2;
        pictureBoxEmpresaDelivery.TabStop = false;
        pictureBoxEmpresaDelivery.Click += pictureBoxEmpresaDelivery_Click;
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Font = new Font("Tahoma", 17.2F, FontStyle.Bold | FontStyle.Italic);
        label3.ForeColor = Color.FromArgb(219, 95, 7);
        label3.Location = new Point(162, 30);
        label3.Name = "label3";
        label3.Size = new Size(556, 35);
        label3.TabIndex = 3;
        label3.Text = "Integrador SysMenu com a DelMatch";
        // 
        // label4
        // 
        label4.AutoSize = true;
        label4.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label4.Location = new Point(179, 540);
        label4.Name = "label4";
        label4.Size = new Size(145, 20);
        label4.TabIndex = 4;
        label4.Text = "Empresa Integrada:";
        // 
        // pictureBox1
        // 
        pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
        pictureBox1.Location = new Point(59, 12);
        pictureBox1.Name = "pictureBox1";
        pictureBox1.Size = new Size(97, 58);
        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        pictureBox1.TabIndex = 5;
        pictureBox1.TabStop = false;
        // 
        // pictureBox2
        // 
        pictureBox2.Cursor = Cursors.Hand;
        pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
        pictureBox2.Location = new Point(515, 563);
        pictureBox2.Name = "pictureBox2";
        pictureBox2.Size = new Size(138, 123);
        pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
        pictureBox2.TabIndex = 6;
        pictureBox2.TabStop = false;
        pictureBox2.Click += pictureBox2_Click;
        // 
        // label6
        // 
        label6.AutoSize = true;
        label6.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label6.Location = new Point(550, 540);
        label6.Name = "label6";
        label6.Size = new Size(53, 20);
        label6.TabIndex = 8;
        label6.Text = "MAPA";
        // 
        // DeliveryForm
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;
        ClientSize = new Size(882, 682);
        Controls.Add(label6);
        Controls.Add(pictureBox2);
        Controls.Add(pictureBox1);
        Controls.Add(label4);
        Controls.Add(label3);
        Controls.Add(pictureBoxEmpresaDelivery);
        Controls.Add(panelDeIniciarEntrega);
        Controls.Add(panelDeListarPedidos);
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        Icon = (Icon)resources.GetObject("$this.Icon");
        Name = "DeliveryForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Delivery";
        FormClosed += DeliveryForm_FormClosed;
        Paint += DeliveryForm_Paint;
        panelDeListarPedidos.ResumeLayout(false);
        panelDeIniciarEntrega.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pictureBoxEmpresaDelivery).EndInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Panel panelDeListarPedidos;
    private Label label1;
    private Panel panelDeIniciarEntrega;
    private Label label2;
    private PictureBox pictureBoxEmpresaDelivery;
    private Label label3;
    private Label label4;
    private PictureBox pictureBox1;
    private PictureBox pictureBox2;
    private Label label6;
}