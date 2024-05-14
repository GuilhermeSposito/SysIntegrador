namespace SysIntegradorApp.Forms
{
    partial class FormPedidosEnviados
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPedidosEnviados));
            labelDePEdidosAbertos = new Label();
            panelDePedidosJaEnviados = new FlowLayoutPanel();
            btnFechar = new Button();
            pictureBox1 = new PictureBox();
            panel1 = new Panel();
            pictureBox2 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // labelDePEdidosAbertos
            // 
            labelDePEdidosAbertos.AutoSize = true;
            labelDePEdidosAbertos.Font = new Font("Segoe UI", 16F);
            labelDePEdidosAbertos.ForeColor = Color.White;
            labelDePEdidosAbertos.Location = new Point(152, 4);
            labelDePEdidosAbertos.Name = "labelDePEdidosAbertos";
            labelDePEdidosAbertos.Size = new Size(394, 37);
            labelDePEdidosAbertos.TabIndex = 1;
            labelDePEdidosAbertos.Text = "Pedidos Enviados Pelo SysMenu";
            // 
            // panelDePedidosJaEnviados
            // 
            panelDePedidosJaEnviados.AutoScroll = true;
            panelDePedidosJaEnviados.FlowDirection = FlowDirection.TopDown;
            panelDePedidosJaEnviados.Location = new Point(42, 72);
            panelDePedidosJaEnviados.Name = "panelDePedidosJaEnviados";
            panelDePedidosJaEnviados.Size = new Size(1212, 504);
            panelDePedidosJaEnviados.TabIndex = 2;
            panelDePedidosJaEnviados.WrapContents = false;
            // 
            // btnFechar
            // 
            btnFechar.BackColor = Color.Red;
            btnFechar.Cursor = Cursors.Hand;
            btnFechar.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnFechar.ForeColor = Color.White;
            btnFechar.Location = new Point(548, 602);
            btnFechar.Name = "btnFechar";
            btnFechar.Size = new Size(171, 56);
            btnFechar.TabIndex = 3;
            btnFechar.Text = "Fechar";
            btnFechar.UseVisualStyleBackColor = false;
            btnFechar.Click += btnFechar_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(83, 7);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(43, 34);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 4;
            pictureBox1.TabStop = false;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(219, 95, 7);
            panel1.Controls.Add(pictureBox2);
            panel1.Controls.Add(labelDePEdidosAbertos);
            panel1.Controls.Add(pictureBox1);
            panel1.Location = new Point(253, 11);
            panel1.Name = "panel1";
            panel1.Size = new Size(731, 46);
            panel1.TabIndex = 5;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(570, 7);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(43, 34);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 5;
            pictureBox2.TabStop = false;
            // 
            // FormPedidosEnviados
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1266, 692);
            Controls.Add(panel1);
            Controls.Add(btnFechar);
            Controls.Add(panelDePedidosJaEnviados);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormPedidosEnviados";
            StartPosition = FormStartPosition.CenterParent;
            Text = "FormPedidosEnviados";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Label labelDePEdidosAbertos;
        private FlowLayoutPanel panelDePedidosJaEnviados;
        private Button btnFechar;
        private PictureBox pictureBox1;
        private Panel panel1;
        private PictureBox pictureBox2;
    }
}