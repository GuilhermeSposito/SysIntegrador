namespace SysIntegradorApp.Forms.CCM
{
    partial class FormDePedidoNaoAceito
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
            label1 = new Label();
            motivoCancelamento = new TextBox();
            btnCancelar = new Button();
            btnEnvir = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 15F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(75, 9);
            label1.Name = "label1";
            label1.Size = new Size(657, 35);
            label1.TabIndex = 7;
            label1.Text = "Por favor, digite o motivo do cancelamento do pedido!";
            // 
            // motivoCancelamento
            // 
            motivoCancelamento.Font = new Font("Segoe UI", 15F);
            motivoCancelamento.Location = new Point(44, 73);
            motivoCancelamento.Name = "motivoCancelamento";
            motivoCancelamento.PlaceholderText = "Digite o motivo que você está cancelando o pedido";
            motivoCancelamento.Size = new Size(740, 41);
            motivoCancelamento.TabIndex = 8;
            // 
            // btnCancelar
            // 
            btnCancelar.BackColor = Color.Red;
            btnCancelar.Cursor = Cursors.Hand;
            btnCancelar.FlatAppearance.BorderColor = Color.White;
            btnCancelar.FlatAppearance.BorderSize = 2;
            btnCancelar.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCancelar.ForeColor = SystemColors.ButtonHighlight;
            btnCancelar.Location = new Point(161, 145);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(158, 59);
            btnCancelar.TabIndex = 9;
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
            btnEnvir.Location = new Point(485, 145);
            btnEnvir.Name = "btnEnvir";
            btnEnvir.Size = new Size(158, 59);
            btnEnvir.TabIndex = 10;
            btnEnvir.Text = "Enviar";
            btnEnvir.UseVisualStyleBackColor = false;
            btnEnvir.Click += btnEnvir_Click;
            // 
            // FormDePedidoNaoAceito
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(829, 216);
            Controls.Add(btnEnvir);
            Controls.Add(btnCancelar);
            Controls.Add(motivoCancelamento);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormDePedidoNaoAceito";
            StartPosition = FormStartPosition.CenterParent;
            Text = "FormDePedidoNaoAceito";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox motivoCancelamento;
        private Button btnCancelar;
        private Button btnEnvir;
    }
}