namespace SysIntegradorApp.UserControls
{
    partial class UCPedidoAbertoSys
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCPedidoAbertoSys));
            labelNomeCliente = new Label();
            labelNumConta = new Label();
            labelEndereco = new Label();
            labelValorPedido = new Label();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            picBoxCheck = new PictureBox();
            panel1 = new Panel();
            label5 = new Label();
            labelStatus = new Label();
            ((System.ComponentModel.ISupportInitialize)picBoxCheck).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // labelNomeCliente
            // 
            labelNomeCliente.AutoSize = true;
            labelNomeCliente.Font = new Font("Segoe UI", 11.8F, FontStyle.Bold);
            labelNomeCliente.ForeColor = SystemColors.Control;
            labelNomeCliente.Location = new Point(77, 37);
            labelNomeCliente.Name = "labelNomeCliente";
            labelNomeCliente.Size = new Size(0, 28);
            labelNomeCliente.TabIndex = 0;
            // 
            // labelNumConta
            // 
            labelNumConta.AutoSize = true;
            labelNumConta.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelNumConta.ForeColor = SystemColors.Control;
            labelNumConta.Location = new Point(0, 35);
            labelNumConta.Name = "labelNumConta";
            labelNumConta.Size = new Size(0, 31);
            labelNumConta.TabIndex = 1;
            // 
            // labelEndereco
            // 
            labelEndereco.AutoSize = true;
            labelEndereco.Font = new Font("Segoe UI", 11.8F, FontStyle.Bold);
            labelEndereco.ForeColor = SystemColors.Control;
            labelEndereco.Location = new Point(365, 35);
            labelEndereco.Name = "labelEndereco";
            labelEndereco.Size = new Size(0, 28);
            labelEndereco.TabIndex = 2;
            // 
            // labelValorPedido
            // 
            labelValorPedido.AutoSize = true;
            labelValorPedido.Font = new Font("Segoe UI", 11.8F, FontStyle.Bold);
            labelValorPedido.ForeColor = SystemColors.Control;
            labelValorPedido.Location = new Point(944, 35);
            labelValorPedido.Name = "labelValorPedido";
            labelValorPedido.Size = new Size(96, 28);
            labelValorPedido.TabIndex = 3;
            labelValorPedido.Text = "R$ 00,00";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = SystemColors.ControlLightLight;
            label1.Location = new Point(3, 1);
            label1.Name = "label1";
            label1.Size = new Size(51, 20);
            label1.TabIndex = 4;
            label1.Text = "Conta:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = SystemColors.ControlLightLight;
            label2.Location = new Point(365, 1);
            label2.Name = "label2";
            label2.Size = new Size(74, 20);
            label2.TabIndex = 5;
            label2.Text = "Endereço:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = SystemColors.ControlLightLight;
            label3.Location = new Point(77, 0);
            label3.Name = "label3";
            label3.Size = new Size(53, 20);
            label3.TabIndex = 6;
            label3.Text = "Nome:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = SystemColors.ControlLightLight;
            label4.Location = new Point(944, 1);
            label4.Name = "label4";
            label4.Size = new Size(46, 20);
            label4.TabIndex = 7;
            label4.Text = "Valor:";
            // 
            // picBoxCheck
            // 
            picBoxCheck.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            picBoxCheck.BackColor = Color.FromArgb(219, 95, 7);
            picBoxCheck.Image = (Image)resources.GetObject("picBoxCheck.Image");
            picBoxCheck.Location = new Point(0, 0);
            picBoxCheck.Name = "picBoxCheck";
            picBoxCheck.Size = new Size(64, 47);
            picBoxCheck.SizeMode = PictureBoxSizeMode.Zoom;
            picBoxCheck.TabIndex = 8;
            picBoxCheck.TabStop = false;
            picBoxCheck.Visible = false;
            picBoxCheck.Click += picBoxCheck_Click;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(219, 95, 7);
            panel1.Controls.Add(picBoxCheck);
            panel1.Location = new Point(1062, 8);
            panel1.Margin = new Padding(3, 3, 3, 6);
            panel1.Name = "panel1";
            panel1.Size = new Size(64, 50);
            panel1.TabIndex = 9;
            panel1.Click += panel1_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.ForeColor = SystemColors.ControlLightLight;
            label5.Location = new Point(761, 0);
            label5.Name = "label5";
            label5.Size = new Size(52, 20);
            label5.TabIndex = 10;
            label5.Text = "Status:";
            // 
            // labelStatus
            // 
            labelStatus.AutoSize = true;
            labelStatus.Font = new Font("Segoe UI", 11.8F, FontStyle.Bold);
            labelStatus.ForeColor = SystemColors.Control;
            labelStatus.Location = new Point(761, 35);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(0, 28);
            labelStatus.TabIndex = 11;
            // 
            // UCPedidoAbertoSys
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(219, 95, 7);
            Controls.Add(labelStatus);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(labelValorPedido);
            Controls.Add(labelEndereco);
            Controls.Add(labelNumConta);
            Controls.Add(labelNomeCliente);
            Controls.Add(panel1);
            Cursor = Cursors.Hand;
            Name = "UCPedidoAbertoSys";
            Size = new Size(1170, 72);
            Load += UCPedidoAbertoSys_Load;
            Click += UCPedidoAbertoSys_Click;
            Paint += UCPedidoAbertoSys_Paint;
            Enter += UCPedidoAbertoSys_Enter;
            ((System.ComponentModel.ISupportInitialize)picBoxCheck).EndInit();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelNomeCliente;
        private Label labelNumConta;
        private Label labelEndereco;
        private Label labelValorPedido;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private PictureBox picBoxCheck;
        private Panel panel1;
        private Label label5;
        private Label labelStatus;
    }
}
