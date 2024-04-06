namespace SysIntegradorApp
{
    partial class UCComplementoDoItem
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
            labelNomeComplemento = new Label();
            valorDoComplemento = new Label();
            SuspendLayout();
            // 
            // labelNomeComplemento
            // 
            labelNomeComplemento.AutoSize = true;
            labelNomeComplemento.Font = new Font("Segoe UI", 10F);
            labelNomeComplemento.Location = new Point(18, 18);
            labelNomeComplemento.Name = "labelNomeComplemento";
            labelNomeComplemento.Size = new Size(134, 23);
            labelNomeComplemento.TabIndex = 0;
            labelNomeComplemento.Text = "Complemento X";
            // 
            // valorDoComplemento
            // 
            valorDoComplemento.AutoSize = true;
            valorDoComplemento.Font = new Font("Segoe UI", 10F);
            valorDoComplemento.Location = new Point(531, 18);
            valorDoComplemento.Name = "valorDoComplemento";
            valorDoComplemento.Size = new Size(74, 23);
            valorDoComplemento.TabIndex = 1;
            valorDoComplemento.Text = "R$ 00,00";
            // 
            // UCComplementoDoItem
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(valorDoComplemento);
            Controls.Add(labelNomeComplemento);
            Name = "UCComplementoDoItem";
            Size = new Size(740, 62);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelNomeComplemento;
        private Label valorDoComplemento;
    }
}
