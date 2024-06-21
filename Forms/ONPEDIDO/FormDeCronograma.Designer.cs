namespace SysIntegradorApp.Forms.ONPEDIDO
{
    partial class FormDeCronograma
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
            textBoxTempoEntrega = new TextBox();
            label1 = new Label();
            textBoxTempoDeRetirada = new TextBox();
            textBoxTempoConclPedido = new TextBox();
            btnCancelar = new Button();
            btnEnvir = new Button();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            SuspendLayout();
            // 
            // textBoxTempoEntrega
            // 
            textBoxTempoEntrega.Font = new Font("Segoe UI", 12F);
            textBoxTempoEntrega.Location = new Point(107, 81);
            textBoxTempoEntrega.Name = "textBoxTempoEntrega";
            textBoxTempoEntrega.PlaceholderText = "Coloque o valor em minutos";
            textBoxTempoEntrega.Size = new Size(257, 34);
            textBoxTempoEntrega.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 15F);
            label1.Location = new Point(97, 9);
            label1.Name = "label1";
            label1.Size = new Size(286, 35);
            label1.TabIndex = 1;
            label1.Text = "Cronograma de Delivery";
            // 
            // textBoxTempoDeRetirada
            // 
            textBoxTempoDeRetirada.Font = new Font("Segoe UI", 12F);
            textBoxTempoDeRetirada.Location = new Point(107, 143);
            textBoxTempoDeRetirada.Name = "textBoxTempoDeRetirada";
            textBoxTempoDeRetirada.PlaceholderText = "Coloque o valor em minutos";
            textBoxTempoDeRetirada.Size = new Size(257, 34);
            textBoxTempoDeRetirada.TabIndex = 2;
            // 
            // textBoxTempoConclPedido
            // 
            textBoxTempoConclPedido.Font = new Font("Segoe UI", 12F);
            textBoxTempoConclPedido.Location = new Point(107, 205);
            textBoxTempoConclPedido.Name = "textBoxTempoConclPedido";
            textBoxTempoConclPedido.PlaceholderText = "Coloque o valor em minutos";
            textBoxTempoConclPedido.Size = new Size(257, 34);
            textBoxTempoConclPedido.TabIndex = 3;
            // 
            // btnCancelar
            // 
            btnCancelar.BackColor = Color.Red;
            btnCancelar.Cursor = Cursors.Hand;
            btnCancelar.FlatAppearance.BorderColor = Color.White;
            btnCancelar.FlatAppearance.BorderSize = 2;
            btnCancelar.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCancelar.ForeColor = SystemColors.ButtonHighlight;
            btnCancelar.Location = new Point(97, 257);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(130, 59);
            btnCancelar.TabIndex = 4;
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
            btnEnvir.Location = new Point(245, 257);
            btnEnvir.Name = "btnEnvir";
            btnEnvir.Size = new Size(138, 59);
            btnEnvir.TabIndex = 5;
            btnEnvir.Text = "Salvar";
            btnEnvir.UseVisualStyleBackColor = false;
            btnEnvir.Click += btnEnvir_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(107, 58);
            label2.Name = "label2";
            label2.Size = new Size(144, 20);
            label2.TabIndex = 6;
            label2.Text = "Tempo para entrega";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(107, 120);
            label3.Name = "label3";
            label3.Size = new Size(148, 20);
            label3.TabIndex = 7;
            label3.Text = "Tempo para retirada:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(107, 182);
            label4.Name = "label4";
            label4.Size = new Size(236, 20);
            label4.TabIndex = 8;
            label4.Text = "Tempo para conclusão do pedido:";
            // 
            // FormDeCronograma
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(474, 318);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(btnEnvir);
            Controls.Add(btnCancelar);
            Controls.Add(textBoxTempoConclPedido);
            Controls.Add(textBoxTempoDeRetirada);
            Controls.Add(label1);
            Controls.Add(textBoxTempoEntrega);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormDeCronograma";
            StartPosition = FormStartPosition.CenterParent;
            Text = "FormDeCronograma";
            Load += FormDeCronograma_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBoxTempoEntrega;
        private Label label1;
        private TextBox textBoxTempoDeRetirada;
        private TextBox textBoxTempoConclPedido;
        private Button btnCancelar;
        private Button btnEnvir;
        private Label label2;
        private Label label3;
        private Label label4;
    }
}