namespace SysIntegradorApp.Forms.ONPEDIDO
{
    partial class FormAguardoDeConclPedidos
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAguardoDeConclPedidos));
            label1 = new Label();
            NumPedidos = new Label();
            label2 = new Label();
            pictureBox1 = new PictureBox();
            label3 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Red;
            label1.Font = new Font("Segoe UI", 15F);
            label1.Location = new Point(0, -1);
            label1.Name = "label1";
            label1.Size = new Size(399, 35);
            label1.TabIndex = 0;
            label1.Text = "Por favor, Aguarde enquanto seus ";
            // 
            // NumPedidos
            // 
            NumPedidos.AutoSize = true;
            NumPedidos.BackColor = Color.Red;
            NumPedidos.Font = new Font("Segoe UI", 15F);
            NumPedidos.Location = new Point(389, -1);
            NumPedidos.Name = "NumPedidos";
            NumPedidos.Size = new Size(28, 35);
            NumPedidos.TabIndex = 1;
            NumPedidos.Text = "5";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Red;
            label2.Font = new Font("Segoe UI", 15F);
            label2.Location = new Point(417, -1);
            label2.Name = "label2";
            label2.Size = new Size(584, 35);
            label2.TabIndex = 2;
            label2.Text = "pedidos estão sendo confirmado automaticamente";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(332, 49);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(267, 151);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(153, 212);
            label3.Name = "label3";
            label3.Size = new Size(643, 20);
            label3.TabIndex = 4;
            label3.Text = "Obs: Para que esse aviso não apareça, lembre sempre de concluir seus pedidos WEB ONPEDIDO";
            // 
            // FormAguardoDeConclPedidos
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(999, 241);
            Controls.Add(label3);
            Controls.Add(pictureBox1);
            Controls.Add(NumPedidos);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormAguardoDeConclPedidos";
            StartPosition = FormStartPosition.CenterParent;
            Text = "FormAguardoDeConclPedidos";
            Load += FormAguardoDeConclPedidos_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private static Label NumPedidos;
        private Label label2;
        private PictureBox pictureBox1;
        private Label label3;
    }
}