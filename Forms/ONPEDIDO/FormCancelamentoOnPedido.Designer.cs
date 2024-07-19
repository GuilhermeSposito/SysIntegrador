namespace SysIntegradorApp.Forms.ONPEDIDO
{
    partial class FormCancelamentoOnPedido
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCancelamentoOnPedido));
            btnCancelar = new Button();
            btnEnvir = new Button();
            motivoCancelamento = new TextBox();
            label1 = new Label();
            SuspendLayout();
            // 
            // btnCancelar
            // 
            btnCancelar.BackColor = Color.Red;
            btnCancelar.Cursor = Cursors.Hand;
            btnCancelar.FlatAppearance.BorderColor = Color.White;
            btnCancelar.FlatAppearance.BorderSize = 2;
            btnCancelar.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCancelar.ForeColor = SystemColors.ButtonHighlight;
            btnCancelar.Location = new Point(127, 145);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(158, 59);
            btnCancelar.TabIndex = 3;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = false;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // btnEnvir
            // 
            btnEnvir.BackColor = Color.Red;
            btnEnvir.Cursor = Cursors.Hand;
            btnEnvir.FlatAppearance.BorderColor = Color.White;
            btnEnvir.FlatAppearance.BorderSize = 2;
            btnEnvir.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnEnvir.ForeColor = SystemColors.ButtonHighlight;
            btnEnvir.Location = new Point(514, 145);
            btnEnvir.Name = "btnEnvir";
            btnEnvir.Size = new Size(158, 59);
            btnEnvir.TabIndex = 4;
            btnEnvir.Text = "Enviar";
            btnEnvir.UseVisualStyleBackColor = false;
            btnEnvir.Click += btnEnvir_Click;
            // 
            // motivoCancelamento
            // 
            motivoCancelamento.Font = new Font("Segoe UI", 15F);
            motivoCancelamento.Location = new Point(53, 77);
            motivoCancelamento.Name = "motivoCancelamento";
            motivoCancelamento.PlaceholderText = "Digite o motivo que você está cancelando o pedido";
            motivoCancelamento.Size = new Size(740, 41);
            motivoCancelamento.TabIndex = 5;
            motivoCancelamento.KeyPress += motivoCancelamento_KeyPress;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 15F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(88, 9);
            label1.Name = "label1";
            label1.Size = new Size(657, 35);
            label1.TabIndex = 6;
            label1.Text = "Por favor, digite o motivo do cancelamento do pedido!";
            // 
            // FormCancelamentoOnPedido
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(829, 216);
            Controls.Add(label1);
            Controls.Add(motivoCancelamento);
            Controls.Add(btnEnvir);
            Controls.Add(btnCancelar);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimizeBox = false;
            Name = "FormCancelamentoOnPedido";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Cancelamento de pedido";
            Load += FormCancelamentoOnPedido_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnCancelar;
        private Button btnEnvir;
        private TextBox motivoCancelamento;
        private Label label1;
    }
}