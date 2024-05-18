namespace SysIntegradorApp.UserControls.UCSDelMatch
{
    partial class UCPedidoDelMatch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCPedidoDelMatch));
            pictureBoxDELMATCH = new PictureBox();
            labelNumPedido = new Label();
            labelNomePedido = new Label();
            label1 = new Label();
            labelEntregarAte = new Label();
            labelStatus = new Label();
            labelHorarioDeEntrega = new Label();
            labelNumConta = new Label();
            pictureBoxImp = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBoxDELMATCH).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxImp).BeginInit();
            SuspendLayout();
            // 
            // pictureBoxDELMATCH
            // 
            pictureBoxDELMATCH.Image = (Image)resources.GetObject("pictureBoxDELMATCH.Image");
            pictureBoxDELMATCH.Location = new Point(0, -11);
            pictureBoxDELMATCH.Name = "pictureBoxDELMATCH";
            pictureBoxDELMATCH.Size = new Size(126, 137);
            pictureBoxDELMATCH.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxDELMATCH.TabIndex = 1;
            pictureBoxDELMATCH.TabStop = false;
            // 
            // labelNumPedido
            // 
            labelNumPedido.AutoSize = true;
            labelNumPedido.Font = new Font("Segoe UI Semibold", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelNumPedido.Location = new Point(130, -7);
            labelNumPedido.Name = "labelNumPedido";
            labelNumPedido.Size = new Size(98, 38);
            labelNumPedido.TabIndex = 2;
            labelNumPedido.Text = "#8686\r\n";
            // 
            // labelNomePedido
            // 
            labelNomePedido.AutoSize = true;
            labelNomePedido.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelNomePedido.Location = new Point(132, 31);
            labelNomePedido.Name = "labelNomePedido";
            labelNomePedido.Size = new Size(176, 25);
            labelNomePedido.TabIndex = 3;
            labelNomePedido.Text = "Guilherme Sposito";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 8F);
            label1.Location = new Point(132, 56);
            label1.Name = "label1";
            label1.Size = new Size(72, 19);
            label1.TabIndex = 7;
            label1.Text = "SysMenu: ";
            // 
            // labelEntregarAte
            // 
            labelEntregarAte.AutoSize = true;
            labelEntregarAte.Location = new Point(132, 75);
            labelEntregarAte.Name = "labelEntregarAte";
            labelEntregarAte.Size = new Size(99, 20);
            labelEntregarAte.TabIndex = 8;
            labelEntregarAte.Text = "Entregar Até: ";
            // 
            // labelStatus
            // 
            labelStatus.AutoSize = true;
            labelStatus.Location = new Point(132, 96);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(53, 20);
            labelStatus.TabIndex = 9;
            labelStatus.Text = "Placed";
            // 
            // labelHorarioDeEntrega
            // 
            labelHorarioDeEntrega.AutoSize = true;
            labelHorarioDeEntrega.Location = new Point(224, 75);
            labelHorarioDeEntrega.Name = "labelHorarioDeEntrega";
            labelHorarioDeEntrega.Size = new Size(44, 20);
            labelHorarioDeEntrega.TabIndex = 10;
            labelHorarioDeEntrega.Text = "10:40";
            // 
            // labelNumConta
            // 
            labelNumConta.AutoSize = true;
            labelNumConta.Font = new Font("Segoe UI", 8F);
            labelNumConta.Location = new Point(201, 56);
            labelNumConta.Name = "labelNumConta";
            labelNumConta.Size = new Size(17, 19);
            labelNumConta.TabIndex = 11;
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
            pictureBoxImp.TabIndex = 12;
            pictureBoxImp.TabStop = false;
            // 
            // UCPedidoDelMatch
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pictureBoxImp);
            Controls.Add(labelNumConta);
            Controls.Add(labelHorarioDeEntrega);
            Controls.Add(labelStatus);
            Controls.Add(labelEntregarAte);
            Controls.Add(label1);
            Controls.Add(labelNomePedido);
            Controls.Add(labelNumPedido);
            Controls.Add(pictureBoxDELMATCH);
            Cursor = Cursors.Hand;
            Name = "UCPedidoDelMatch";
            Size = new Size(350, 116);
            Click += UCPedidoDelMatch_Click;
            ((System.ComponentModel.ISupportInitialize)pictureBoxDELMATCH).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxImp).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBoxDELMATCH;
        private Label labelNumPedido;
        private Label labelNomePedido;
        private Label label1;
        private Label labelEntregarAte;
        private Label labelStatus;
        private Label labelHorarioDeEntrega;
        private Label labelNumConta;
        private PictureBox pictureBoxImp;
    }
}
