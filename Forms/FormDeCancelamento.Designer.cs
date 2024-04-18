namespace SysIntegradorApp.Forms
{
    partial class FormDeCancelamento
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
            labelAvisoCancelamento = new Label();
            btnCancelar = new Button();
            labelMotivoNM = new Label();
            panelDeMotivos = new FlowLayoutPanel();
            SuspendLayout();
            // 
            // labelAvisoCancelamento
            // 
            labelAvisoCancelamento.AutoSize = true;
            labelAvisoCancelamento.BackColor = SystemColors.ButtonHighlight;
            labelAvisoCancelamento.Font = new Font("Segoe UI", 17F);
            labelAvisoCancelamento.ForeColor = Color.Red;
            labelAvisoCancelamento.Location = new Point(90, 9);
            labelAvisoCancelamento.Name = "labelAvisoCancelamento";
            labelAvisoCancelamento.Size = new Size(515, 40);
            labelAvisoCancelamento.TabIndex = 0;
            labelAvisoCancelamento.Text = "Você está preste a cancelar um pedido!";
            // 
            // btnCancelar
            // 
            btnCancelar.BackColor = Color.Red;
            btnCancelar.Cursor = Cursors.Hand;
            btnCancelar.FlatAppearance.BorderColor = Color.White;
            btnCancelar.FlatAppearance.BorderSize = 2;
            btnCancelar.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCancelar.ForeColor = SystemColors.ButtonHighlight;
            btnCancelar.Location = new Point(272, 417);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(158, 59);
            btnCancelar.TabIndex = 2;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = false;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // labelMotivoNM
            // 
            labelMotivoNM.AutoSize = true;
            labelMotivoNM.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelMotivoNM.Location = new Point(247, 49);
            labelMotivoNM.Name = "labelMotivoNM";
            labelMotivoNM.Size = new Size(183, 23);
            labelMotivoNM.TabIndex = 4;
            labelMotivoNM.Text = "Selecione um motivo:";
            // 
            // panelDeMotivos
            // 
            panelDeMotivos.AutoScroll = true;
            panelDeMotivos.FlowDirection = FlowDirection.TopDown;
            panelDeMotivos.Location = new Point(60, 75);
            panelDeMotivos.Name = "panelDeMotivos";
            panelDeMotivos.Size = new Size(590, 326);
            panelDeMotivos.TabIndex = 5;
            panelDeMotivos.WrapContents = false;
            // 
            // FormDeCancelamento
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ButtonFace;
            ClientSize = new Size(698, 512);
            Controls.Add(panelDeMotivos);
            Controls.Add(labelMotivoNM);
            Controls.Add(btnCancelar);
            Controls.Add(labelAvisoCancelamento);
            MaximizeBox = false;
            Name = "FormDeCancelamento";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Cancelamento";
            Load += FormDeCancelamento_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelAvisoCancelamento;
        private Button btnCancelar;
        private Label labelMotivoNM;
        private FlowLayoutPanel panelDeMotivos;
    }
}