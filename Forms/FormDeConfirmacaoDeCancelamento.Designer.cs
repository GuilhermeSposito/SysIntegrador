namespace SysIntegradorApp.Forms
{
    partial class FormDeConfirmacaoDeCancelamento
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
            labelDisplayId = new Label();
            labelPeloMotivo = new Label();
            panelConfirmaCaneclamento = new FlowLayoutPanel();
            naoBtn = new Button();
            simBtn = new Button();
            label2 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 15F);
            label1.Location = new Point(51, 9);
            label1.Name = "label1";
            label1.Size = new Size(457, 35);
            label1.TabIndex = 0;
            label1.Text = "Você está prestes a cancelar o pedido #";
            // 
            // labelDisplayId
            // 
            labelDisplayId.AutoSize = true;
            labelDisplayId.Font = new Font("Segoe UI", 15F);
            labelDisplayId.Location = new Point(499, 9);
            labelDisplayId.Name = "labelDisplayId";
            labelDisplayId.Size = new Size(67, 35);
            labelDisplayId.TabIndex = 1;
            labelDisplayId.Text = "8484";
            // 
            // labelPeloMotivo
            // 
            labelPeloMotivo.AutoSize = true;
            labelPeloMotivo.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelPeloMotivo.Location = new Point(41, 53);
            labelPeloMotivo.Name = "labelPeloMotivo";
            labelPeloMotivo.Size = new Size(162, 20);
            labelPeloMotivo.TabIndex = 2;
            labelPeloMotivo.Text = "Pelo Seguinte Motivo:";
            // 
            // panelConfirmaCaneclamento
            // 
            panelConfirmaCaneclamento.Location = new Point(41, 76);
            panelConfirmaCaneclamento.Name = "panelConfirmaCaneclamento";
            panelConfirmaCaneclamento.Size = new Size(546, 44);
            panelConfirmaCaneclamento.TabIndex = 3;
            // 
            // naoBtn
            // 
            naoBtn.BackColor = Color.Red;
            naoBtn.Cursor = Cursors.Hand;
            naoBtn.Font = new Font("Segoe UI", 15F, FontStyle.Bold, GraphicsUnit.Point, 0);
            naoBtn.ForeColor = SystemColors.Control;
            naoBtn.Location = new Point(388, 167);
            naoBtn.Name = "naoBtn";
            naoBtn.Size = new Size(178, 48);
            naoBtn.TabIndex = 4;
            naoBtn.Text = "NÃO";
            naoBtn.UseVisualStyleBackColor = false;
            naoBtn.Click += naoBtn_Click;
            // 
            // simBtn
            // 
            simBtn.BackColor = Color.Red;
            simBtn.Cursor = Cursors.Hand;
            simBtn.Font = new Font("Segoe UI", 15F, FontStyle.Bold, GraphicsUnit.Point, 0);
            simBtn.ForeColor = SystemColors.Control;
            simBtn.Location = new Point(51, 167);
            simBtn.Name = "simBtn";
            simBtn.Size = new Size(178, 48);
            simBtn.TabIndex = 5;
            simBtn.Text = "SIM";
            simBtn.UseVisualStyleBackColor = false;
            simBtn.Click += simBtn_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.White;
            label2.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.Red;
            label2.Location = new Point(156, 123);
            label2.Name = "label2";
            label2.Size = new Size(290, 31);
            label2.TabIndex = 6;
            label2.Text = "Você Confirma o motivo ?";
            // 
            // FormDeConfirmacaoDeCancelamento
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(637, 227);
            Controls.Add(label2);
            Controls.Add(simBtn);
            Controls.Add(naoBtn);
            Controls.Add(panelConfirmaCaneclamento);
            Controls.Add(labelPeloMotivo);
            Controls.Add(labelDisplayId);
            Controls.Add(label1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormDeConfirmacaoDeCancelamento";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Confirmação de cancelamento de pedido";
            Load += FormDeConfirmacaoDeCancelamento_Load;
            Paint += FormDeConfirmacaoDeCancelamento_Paint;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label labelDisplayId;
        private Label labelPeloMotivo;
        private FlowLayoutPanel panelConfirmaCaneclamento;
        private Button naoBtn;
        private Button simBtn;
        private Label label2;
    }
}