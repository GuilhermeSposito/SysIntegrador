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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDeCronograma));
            textBoxTempoEntrega = new TextBox();
            label1 = new Label();
            textBoxTempoDeRetirada = new TextBox();
            textBoxTempoConclPedido = new TextBox();
            btnCancelar = new Button();
            btnEnvir = new Button();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            panel1 = new Panel();
            labelIfood = new Label();
            panel2 = new Panel();
            OnIfood = new PictureBox();
            OffIfood = new PictureBox();
            panel3 = new Panel();
            OnCardapio = new PictureBox();
            OffCardapio = new PictureBox();
            labelCardapio = new Label();
            panel4 = new Panel();
            OnEntrega = new PictureBox();
            OffEntrega = new PictureBox();
            labelEntregaAut = new Label();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)OnIfood).BeginInit();
            ((System.ComponentModel.ISupportInitialize)OffIfood).BeginInit();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)OnCardapio).BeginInit();
            ((System.ComponentModel.ISupportInitialize)OffCardapio).BeginInit();
            panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)OnEntrega).BeginInit();
            ((System.ComponentModel.ISupportInitialize)OffEntrega).BeginInit();
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
            btnCancelar.Location = new Point(61, 568);
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
            btnEnvir.Location = new Point(257, 568);
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
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 15F);
            label5.Location = new Point(128, 253);
            label5.Name = "label5";
            label5.Size = new Size(220, 35);
            label5.TabIndex = 9;
            label5.Text = "Integrações ativas:";
            // 
            // panel1
            // 
            panel1.Location = new Point(61, 9);
            panel1.Name = "panel1";
            panel1.Size = new Size(349, 241);
            panel1.TabIndex = 10;
            // 
            // labelIfood
            // 
            labelIfood.AutoSize = true;
            labelIfood.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelIfood.Location = new Point(15, 10);
            labelIfood.Name = "labelIfood";
            labelIfood.Size = new Size(62, 28);
            labelIfood.TabIndex = 11;
            labelIfood.Text = "Ifood";
            // 
            // panel2
            // 
            panel2.Controls.Add(OnIfood);
            panel2.Controls.Add(OffIfood);
            panel2.Controls.Add(labelIfood);
            panel2.Location = new Point(61, 291);
            panel2.Name = "panel2";
            panel2.Size = new Size(349, 55);
            panel2.TabIndex = 12;
            // 
            // OnIfood
            // 
            OnIfood.Cursor = Cursors.Hand;
            OnIfood.Image = (Image)resources.GetObject("OnIfood.Image");
            OnIfood.Location = new Point(250, 1);
            OnIfood.Name = "OnIfood";
            OnIfood.Size = new Size(84, 45);
            OnIfood.SizeMode = PictureBoxSizeMode.Zoom;
            OnIfood.TabIndex = 25;
            OnIfood.TabStop = false;
            OnIfood.Visible = false;
            OnIfood.Click += OnIfood_Click;
            // 
            // OffIfood
            // 
            OffIfood.Cursor = Cursors.Hand;
            OffIfood.Image = (Image)resources.GetObject("OffIfood.Image");
            OffIfood.Location = new Point(250, 1);
            OffIfood.Name = "OffIfood";
            OffIfood.Size = new Size(84, 43);
            OffIfood.SizeMode = PictureBoxSizeMode.Zoom;
            OffIfood.TabIndex = 24;
            OffIfood.TabStop = false;
            OffIfood.Visible = false;
            OffIfood.Click += OffIfood_Click;
            // 
            // panel3
            // 
            panel3.Controls.Add(OnCardapio);
            panel3.Controls.Add(OffCardapio);
            panel3.Controls.Add(labelCardapio);
            panel3.Location = new Point(61, 350);
            panel3.Name = "panel3";
            panel3.Size = new Size(349, 55);
            panel3.TabIndex = 13;
            // 
            // OnCardapio
            // 
            OnCardapio.Cursor = Cursors.Hand;
            OnCardapio.Image = (Image)resources.GetObject("OnCardapio.Image");
            OnCardapio.Location = new Point(250, 1);
            OnCardapio.Name = "OnCardapio";
            OnCardapio.Size = new Size(84, 45);
            OnCardapio.SizeMode = PictureBoxSizeMode.Zoom;
            OnCardapio.TabIndex = 25;
            OnCardapio.TabStop = false;
            OnCardapio.Visible = false;
            OnCardapio.Click += OnCardapio_Click;
            // 
            // OffCardapio
            // 
            OffCardapio.Cursor = Cursors.Hand;
            OffCardapio.Image = (Image)resources.GetObject("OffCardapio.Image");
            OffCardapio.Location = new Point(250, 1);
            OffCardapio.Name = "OffCardapio";
            OffCardapio.Size = new Size(84, 43);
            OffCardapio.SizeMode = PictureBoxSizeMode.Zoom;
            OffCardapio.TabIndex = 24;
            OffCardapio.TabStop = false;
            OffCardapio.Visible = false;
            OffCardapio.Click += OffCardapio_Click;
            // 
            // labelCardapio
            // 
            labelCardapio.AutoSize = true;
            labelCardapio.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelCardapio.Location = new Point(15, 10);
            labelCardapio.Name = "labelCardapio";
            labelCardapio.Size = new Size(96, 28);
            labelCardapio.TabIndex = 11;
            labelCardapio.Text = "Cardápio";
            // 
            // panel4
            // 
            panel4.Controls.Add(OnEntrega);
            panel4.Controls.Add(OffEntrega);
            panel4.Controls.Add(labelEntregaAut);
            panel4.Location = new Point(61, 410);
            panel4.Name = "panel4";
            panel4.Size = new Size(349, 55);
            panel4.TabIndex = 14;
            // 
            // OnEntrega
            // 
            OnEntrega.Cursor = Cursors.Hand;
            OnEntrega.Image = (Image)resources.GetObject("OnEntrega.Image");
            OnEntrega.Location = new Point(250, 1);
            OnEntrega.Name = "OnEntrega";
            OnEntrega.Size = new Size(84, 45);
            OnEntrega.SizeMode = PictureBoxSizeMode.Zoom;
            OnEntrega.TabIndex = 25;
            OnEntrega.TabStop = false;
            OnEntrega.Visible = false;
            OnEntrega.Click += OnEntrega_Click;
            // 
            // OffEntrega
            // 
            OffEntrega.Cursor = Cursors.Hand;
            OffEntrega.Image = (Image)resources.GetObject("OffEntrega.Image");
            OffEntrega.Location = new Point(250, 3);
            OffEntrega.Name = "OffEntrega";
            OffEntrega.Size = new Size(84, 43);
            OffEntrega.SizeMode = PictureBoxSizeMode.Zoom;
            OffEntrega.TabIndex = 24;
            OffEntrega.TabStop = false;
            OffEntrega.Visible = false;
            OffEntrega.Click += OffEntrega_Click;
            // 
            // labelEntregaAut
            // 
            labelEntregaAut.AutoSize = true;
            labelEntregaAut.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelEntregaAut.Location = new Point(15, 10);
            labelEntregaAut.Name = "labelEntregaAut";
            labelEntregaAut.Size = new Size(125, 28);
            labelEntregaAut.TabIndex = 11;
            labelEntregaAut.Text = "Entrega Aut";
            // 
            // FormDeCronograma
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(474, 639);
            Controls.Add(panel4);
            Controls.Add(panel3);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(btnEnvir);
            Controls.Add(btnCancelar);
            Controls.Add(textBoxTempoConclPedido);
            Controls.Add(textBoxTempoDeRetirada);
            Controls.Add(label1);
            Controls.Add(textBoxTempoEntrega);
            Controls.Add(panel1);
            Controls.Add(panel2);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormDeCronograma";
            StartPosition = FormStartPosition.CenterParent;
            Text = "FormDeCronograma";
            Load += FormDeCronograma_Load;
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)OnIfood).EndInit();
            ((System.ComponentModel.ISupportInitialize)OffIfood).EndInit();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)OnCardapio).EndInit();
            ((System.ComponentModel.ISupportInitialize)OffCardapio).EndInit();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)OnEntrega).EndInit();
            ((System.ComponentModel.ISupportInitialize)OffEntrega).EndInit();
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
        private Label label5;
        private Panel panel1;
        private Label labelIfood;
        private Panel panel2;
        private PictureBox OffIfood;
        private PictureBox OnIfood;
        private Panel panel3;
        private PictureBox OnCardapio;
        private PictureBox OffCardapio;
        private Label labelCardapio;
        private Panel panel4;
        private PictureBox OnEntrega;
        private PictureBox OffEntrega;
        private Label labelEntregaAut;
    }
}