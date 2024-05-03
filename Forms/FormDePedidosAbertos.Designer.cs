namespace SysIntegradorApp.Forms
{
    partial class FormDePedidosAbertos
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDePedidosAbertos));
            labelDePEdidosAbertos = new Label();
            panelDepedidosAbertos = new FlowLayoutPanel();
            btnEnviar = new Button();
            btnCancelar = new Button();
            SuspendLayout();
            // 
            // labelDePEdidosAbertos
            // 
            labelDePEdidosAbertos.AutoSize = true;
            labelDePEdidosAbertos.Font = new Font("Segoe UI", 16F);
            labelDePEdidosAbertos.ForeColor = Color.Red;
            labelDePEdidosAbertos.Location = new Point(400, 9);
            labelDePEdidosAbertos.Name = "labelDePEdidosAbertos";
            labelDePEdidosAbertos.Size = new Size(393, 37);
            labelDePEdidosAbertos.TabIndex = 0;
            labelDePEdidosAbertos.Text = "Pedidos Em Aberto no SysMenu";
            // 
            // panelDepedidosAbertos
            // 
            panelDepedidosAbertos.AutoScroll = true;
            panelDepedidosAbertos.FlowDirection = FlowDirection.TopDown;
            panelDepedidosAbertos.Location = new Point(40, 61);
            panelDepedidosAbertos.Name = "panelDepedidosAbertos";
            panelDepedidosAbertos.Size = new Size(1177, 504);
            panelDepedidosAbertos.TabIndex = 1;
            panelDepedidosAbertos.WrapContents = false;
            // 
            // btnEnviar
            // 
            btnEnviar.BackColor = Color.Red;
            btnEnviar.Cursor = Cursors.Hand;
            btnEnviar.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnEnviar.ForeColor = Color.White;
            btnEnviar.Location = new Point(807, 595);
            btnEnviar.Name = "btnEnviar";
            btnEnviar.Size = new Size(171, 56);
            btnEnviar.TabIndex = 3;
            btnEnviar.Text = "Enviar";
            btnEnviar.UseVisualStyleBackColor = false;
            btnEnviar.Click += btnEnviar_Click;
            // 
            // btnCancelar
            // 
            btnCancelar.BackColor = Color.Red;
            btnCancelar.Cursor = Cursors.Hand;
            btnCancelar.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCancelar.ForeColor = Color.White;
            btnCancelar.Location = new Point(312, 595);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(171, 56);
            btnCancelar.TabIndex = 2;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = false;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // FormDePedidosAbertos
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1266, 692);
            Controls.Add(btnEnviar);
            Controls.Add(btnCancelar);
            Controls.Add(panelDepedidosAbertos);
            Controls.Add(labelDePEdidosAbertos);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormDePedidosAbertos";
            StartPosition = FormStartPosition.CenterParent;
            Text = "FormDePedidosAbertos";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelDePEdidosAbertos;
        private FlowLayoutPanel panelDepedidosAbertos;
        private Button btnEnviar;
        private Button btnCancelar;
    }
}