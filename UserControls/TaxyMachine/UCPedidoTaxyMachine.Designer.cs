namespace SysIntegradorApp.UserControls.TaxyMachine
{
    partial class UCPedidoTaxyMachine
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCPedidoTaxyMachine));
            labelNomeCliente = new Label();
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            labelEndereco = new Label();
            labelComplemento = new Label();
            flowLayoutPanel1 = new FlowLayoutPanel();
            LabelNumPedido = new Label();
            LabelEnviado = new Label();
            pictureBox3 = new PictureBox();
            labelValor = new Label();
            checkBoxRetorno = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            SuspendLayout();
            // 
            // labelNomeCliente
            // 
            labelNomeCliente.AutoSize = true;
            labelNomeCliente.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelNomeCliente.Location = new Point(25, 10);
            labelNomeCliente.Name = "labelNomeCliente";
            labelNomeCliente.Size = new Size(228, 23);
            labelNomeCliente.TabIndex = 0;
            labelNomeCliente.Text = "Guilherme Sposito Calandrin";
            labelNomeCliente.MouseEnter += labelNomeCliente_MouseEnter;
            labelNomeCliente.MouseLeave += labelNomeCliente_MouseLeave;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(3, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(24, 36);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            pictureBox1.MouseEnter += pictureBox1_MouseEnter;
            pictureBox1.MouseLeave += pictureBox1_MouseLeave;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(3, 45);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(24, 36);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 2;
            pictureBox2.TabStop = false;
            pictureBox2.MouseEnter += pictureBox2_MouseEnter;
            pictureBox2.MouseLeave += pictureBox2_MouseLeave;
            // 
            // labelEndereco
            // 
            labelEndereco.AutoSize = true;
            labelEndereco.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelEndereco.Location = new Point(25, 42);
            labelEndereco.Name = "labelEndereco";
            labelEndereco.Size = new Size(348, 23);
            labelEndereco.TabIndex = 3;
            labelEndereco.Text = "Rua Dr Jonas Novaes, 979 - Planalto Paraiso";
            labelEndereco.MouseEnter += labelEndereco_MouseEnter;
            labelEndereco.MouseLeave += labelEndereco_MouseLeave;
            // 
            // labelComplemento
            // 
            labelComplemento.AutoSize = true;
            labelComplemento.Location = new Point(33, 65);
            labelComplemento.Name = "labelComplemento";
            labelComplemento.Size = new Size(157, 20);
            labelComplemento.TabIndex = 4;
            labelComplemento.Text = "Complemento Se tiver";
            labelComplemento.MouseEnter += labelComplemento_MouseEnter;
            labelComplemento.MouseLeave += labelComplemento_MouseLeave;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(LabelNumPedido);
            flowLayoutPanel1.Location = new Point(354, 0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(62, 36);
            flowLayoutPanel1.TabIndex = 5;
            // 
            // LabelNumPedido
            // 
            LabelNumPedido.AutoSize = true;
            LabelNumPedido.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            LabelNumPedido.ForeColor = Color.Red;
            LabelNumPedido.Location = new Point(3, 0);
            LabelNumPedido.Name = "LabelNumPedido";
            LabelNumPedido.Size = new Size(48, 28);
            LabelNumPedido.TabIndex = 0;
            LabelNumPedido.Text = "100";
            LabelNumPedido.MouseEnter += LabelNumPedido_MouseEnter;
            LabelNumPedido.MouseLeave += LabelNumPedido_MouseLeave;
            // 
            // LabelEnviado
            // 
            LabelEnviado.AutoSize = true;
            LabelEnviado.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            LabelEnviado.ForeColor = Color.FromArgb(0, 192, 0);
            LabelEnviado.Location = new Point(286, 90);
            LabelEnviado.Name = "LabelEnviado";
            LabelEnviado.Size = new Size(87, 28);
            LabelEnviado.TabIndex = 8;
            LabelEnviado.Text = "Enviado";
            LabelEnviado.Visible = false;
            LabelEnviado.MouseEnter += LabelEnviado_MouseEnter;
            LabelEnviado.MouseLeave += LabelEnviado_MouseLeave;
            // 
            // pictureBox3
            // 
            pictureBox3.Image = (Image)resources.GetObject("pictureBox3.Image");
            pictureBox3.Location = new Point(3, 87);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(24, 36);
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.TabIndex = 9;
            pictureBox3.TabStop = false;
            pictureBox3.MouseEnter += pictureBox3_MouseEnter;
            pictureBox3.MouseLeave += pictureBox3_MouseLeave;
            // 
            // labelValor
            // 
            labelValor.AutoSize = true;
            labelValor.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelValor.Location = new Point(33, 96);
            labelValor.Name = "labelValor";
            labelValor.Size = new Size(82, 23);
            labelValor.TabIndex = 10;
            labelValor.Text = "R$ 100,00";
            labelValor.MouseEnter += labelValor_MouseEnter;
            labelValor.MouseLeave += labelValor_MouseLeave;
            // 
            // checkBoxRetorno
            // 
            checkBoxRetorno.AutoSize = true;
            checkBoxRetorno.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            checkBoxRetorno.Location = new Point(150, 96);
            checkBoxRetorno.Name = "checkBoxRetorno";
            checkBoxRetorno.Size = new Size(86, 24);
            checkBoxRetorno.TabIndex = 12;
            checkBoxRetorno.Text = "Retorno";
            checkBoxRetorno.UseVisualStyleBackColor = true;
            checkBoxRetorno.CheckedChanged += checkBoxRetorno_CheckedChanged;
            checkBoxRetorno.MouseEnter += checkBoxRetorno_MouseEnter;
            checkBoxRetorno.MouseLeave += checkBoxRetorno_MouseLeave;
            // 
            // UCPedidoTaxyMachine
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLight;
            Controls.Add(checkBoxRetorno);
            Controls.Add(labelValor);
            Controls.Add(pictureBox3);
            Controls.Add(LabelEnviado);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(labelComplemento);
            Controls.Add(labelEndereco);
            Controls.Add(pictureBox2);
            Controls.Add(pictureBox1);
            Controls.Add(labelNomeCliente);
            Cursor = Cursors.Hand;
            Margin = new Padding(10, 4, 3, 3);
            Name = "UCPedidoTaxyMachine";
            Size = new Size(414, 130);
            Load += UCPedidoTaxyMachine_Load;
            MouseEnter += UCPedidoTaxyMachine_MouseEnter;
            MouseLeave += UCPedidoTaxyMachine_MouseLeave;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelNomeCliente;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private Label labelEndereco;
        private Label labelComplemento;
        private FlowLayoutPanel flowLayoutPanel1;
        private Label LabelNumPedido;
        private Label LabelEnviado;
        private PictureBox pictureBox3;
        private Label labelValor;
        private CheckBox checkBoxRetorno;
    }
}
