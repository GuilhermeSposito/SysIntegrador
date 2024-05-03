namespace SysIntegradorApp.UserControls
{
    partial class UCMotivoCancelamento
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            labelDeMotivo = new Label();
            SuspendLayout();
            // 
            // labelDeMotivo
            // 
            labelDeMotivo.AutoSize = true;
            labelDeMotivo.Font = new Font("Segoe UI", 12F);
            labelDeMotivo.ForeColor = SystemColors.Control;
            labelDeMotivo.Location = new Point(25, 0);
            labelDeMotivo.Name = "labelDeMotivo";
            labelDeMotivo.Size = new Size(534, 28);
            labelDeMotivo.TabIndex = 0;
            labelDeMotivo.Text = "O pedido foi feito fora do horário de funcionamento da loja";
            labelDeMotivo.Click += labelDeMotivo_Click_1;
            // 
            // UCMotivoCancelamento
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            Controls.Add(labelDeMotivo);
            Cursor = Cursors.Hand;
            Name = "UCMotivoCancelamento";
            Size = new Size(561, 34);
            Load += UCMotivoCancelamento_Load;
            Click += UCMotivoCancelamento_Click;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelDeMotivo;
    }
}
