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
        pictureBoxDeListarPedidos = new PictureBox();
        panelDeIniciarEntrega = new Panel();
        label2 = new Label();
        pictureBoxDeInciarPedido = new PictureBox();
        pictureBoxEmpresaDelivery = new PictureBox();
        panelDeListarPedidos.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pictureBoxDeListarPedidos).BeginInit();
        panelDeIniciarEntrega.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pictureBoxDeInciarPedido).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pictureBoxEmpresaDelivery).BeginInit();
        SuspendLayout();
        // 
        // panelDeListarPedidos
        // 
        panelDeListarPedidos.BackColor = SystemColors.AppWorkspace;
        panelDeListarPedidos.Controls.Add(label1);
        panelDeListarPedidos.Controls.Add(pictureBoxDeListarPedidos);
        panelDeListarPedidos.Location = new Point(62, 131);
        panelDeListarPedidos.Name = "panelDeListarPedidos";
        panelDeListarPedidos.Size = new Size(523, 179);
        panelDeListarPedidos.TabIndex = 0;
        // 
        // label1
        // 
        label1.Font = new Font("Segoe UI", 15F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label1.ForeColor = SystemColors.ButtonHighlight;
        label1.Location = new Point(170, 30);
        label1.Name = "label1";
        label1.Size = new Size(330, 117);
        label1.TabIndex = 1;
        label1.Text = "Clique na prancheta para listar os pedidos";
        label1.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // pictureBoxDeListarPedidos
        // 
        pictureBoxDeListarPedidos.Cursor = Cursors.Hand;
        pictureBoxDeListarPedidos.Image = (Image)resources.GetObject("pictureBoxDeListarPedidos.Image");
        pictureBoxDeListarPedidos.Location = new Point(3, 0);
        pictureBoxDeListarPedidos.Name = "pictureBoxDeListarPedidos";
        pictureBoxDeListarPedidos.Size = new Size(151, 176);
        pictureBoxDeListarPedidos.SizeMode = PictureBoxSizeMode.Zoom;
        pictureBoxDeListarPedidos.TabIndex = 0;
        pictureBoxDeListarPedidos.TabStop = false;
        // 
        // panelDeIniciarEntrega
        // 
        panelDeIniciarEntrega.BackColor = SystemColors.AppWorkspace;
        panelDeIniciarEntrega.Controls.Add(label2);
        panelDeIniciarEntrega.Controls.Add(pictureBoxDeInciarPedido);
        panelDeIniciarEntrega.Location = new Point(62, 331);
        panelDeIniciarEntrega.Name = "panelDeIniciarEntrega";
        panelDeIniciarEntrega.Size = new Size(523, 179);
        panelDeIniciarEntrega.TabIndex = 1;
        // 
        // label2
        // 
        label2.Font = new Font("Segoe UI", 15F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label2.ForeColor = SystemColors.ButtonHighlight;
        label2.Location = new Point(170, 26);
        label2.Name = "label2";
        label2.Size = new Size(330, 117);
        label2.TabIndex = 2;
        label2.Text = "Clique no motoboy para inicar um novo pedido.";
        label2.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // pictureBoxDeInciarPedido
        // 
        pictureBoxDeInciarPedido.Cursor = Cursors.Hand;
        pictureBoxDeInciarPedido.Image = (Image)resources.GetObject("pictureBoxDeInciarPedido.Image");
        pictureBoxDeInciarPedido.Location = new Point(3, 3);
        pictureBoxDeInciarPedido.Name = "pictureBoxDeInciarPedido";
        pictureBoxDeInciarPedido.Size = new Size(151, 176);
        pictureBoxDeInciarPedido.SizeMode = PictureBoxSizeMode.Zoom;
        pictureBoxDeInciarPedido.TabIndex = 1;
        pictureBoxDeInciarPedido.TabStop = false;
        pictureBoxDeInciarPedido.Click += pictureBoxDeInciarPedido_Click;
        // 
        // pictureBoxEmpresaDelivery
        // 
        pictureBoxEmpresaDelivery.Cursor = Cursors.Hand;
        pictureBoxEmpresaDelivery.Image = (Image)resources.GetObject("pictureBoxEmpresaDelivery.Image");
        pictureBoxEmpresaDelivery.Location = new Point(254, 12);
        pictureBoxEmpresaDelivery.Name = "pictureBoxEmpresaDelivery";
        pictureBoxEmpresaDelivery.Size = new Size(114, 113);
        pictureBoxEmpresaDelivery.SizeMode = PictureBoxSizeMode.Zoom;
        pictureBoxEmpresaDelivery.TabIndex = 2;
        pictureBoxEmpresaDelivery.TabStop = false;
        // 
        // DeliveryForm
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(649, 618);
        Controls.Add(pictureBoxEmpresaDelivery);
        Controls.Add(panelDeIniciarEntrega);
        Controls.Add(panelDeListarPedidos);
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        Name = "DeliveryForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "DeliveryForm";
        panelDeListarPedidos.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pictureBoxDeListarPedidos).EndInit();
        panelDeIniciarEntrega.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pictureBoxDeInciarPedido).EndInit();
        ((System.ComponentModel.ISupportInitialize)pictureBoxEmpresaDelivery).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private Panel panelDeListarPedidos;
    private Label label1;
    private PictureBox pictureBoxDeListarPedidos;
    private Panel panelDeIniciarEntrega;
    private PictureBox pictureBoxDeInciarPedido;
    private Label label2;
    private PictureBox pictureBoxEmpresaDelivery;
}