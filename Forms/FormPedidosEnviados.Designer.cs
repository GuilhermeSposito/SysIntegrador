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
            labelDePEdidosAbertos = new Label();
            panelDePedidosJaEnviados = new FlowLayoutPanel();
            btnFechar = new Button();
            SuspendLayout();
            // 
            // labelDePEdidosAbertos
            // 
            labelDePEdidosAbertos.AutoSize = true;
            labelDePEdidosAbertos.Font = new Font("Segoe UI", 16F);
            labelDePEdidosAbertos.ForeColor = Color.Red;
            labelDePEdidosAbertos.Location = new Point(426, 9);
            labelDePEdidosAbertos.Name = "labelDePEdidosAbertos";
            labelDePEdidosAbertos.Size = new Size(425, 37);
            labelDePEdidosAbertos.TabIndex = 1;
            labelDePEdidosAbertos.Text = "Pedidos Já Enviados Pelo SysMenu";
            // 
            // panelDePedidosJaEnviados
            // 
            panelDePedidosJaEnviados.AutoScroll = true;
            panelDePedidosJaEnviados.FlowDirection = FlowDirection.TopDown;
            panelDePedidosJaEnviados.Location = new Point(54, 75);
            panelDePedidosJaEnviados.Name = "panelDePedidosJaEnviados";
            panelDePedidosJaEnviados.Size = new Size(1177, 504);
            panelDePedidosJaEnviados.TabIndex = 2;
            panelDePedidosJaEnviados.WrapContents = false;
            // 
            // btnFechar
            // 
            btnFechar.BackColor = Color.Red;
            btnFechar.Cursor = Cursors.Hand;
            btnFechar.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnFechar.ForeColor = Color.White;
            btnFechar.Location = new Point(572, 603);
            btnFechar.Name = "btnFechar";
            btnFechar.Size = new Size(171, 56);
            btnFechar.TabIndex = 3;
            btnFechar.Text = "Fechar";
            btnFechar.UseVisualStyleBackColor = false;
            btnFechar.Click += btnFechar_Click;
            // 
            // FormPedidosEnviados
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1266, 692);
            Controls.Add(btnFechar);
            Controls.Add(panelDePedidosJaEnviados);
            Controls.Add(labelDePEdidosAbertos);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormPedidosEnviados";
            StartPosition = FormStartPosition.CenterParent;
            Text = "FormPedidosEnviados";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelDePEdidosAbertos;
        private FlowLayoutPanel panelDePedidosJaEnviados;
        private Button btnFechar;
    }
}