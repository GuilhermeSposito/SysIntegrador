namespace SysIntegradorApp.UserControls
{
    partial class UCNumPagina
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
            num = new Label();
            SuspendLayout();
            // 
            // num
            // 
            num.AutoSize = true;
            num.Cursor = Cursors.Hand;
            num.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            num.Location = new Point(0, 0);
            num.Name = "num";
            num.Size = new Size(24, 28);
            num.TabIndex = 0;
            num.Text = "1";
            // 
            // UCNumPagina
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            Controls.Add(num);
            Margin = new Padding(0, 0, 2, 0);
            Name = "UCNumPagina";
            Size = new Size(25, 27);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label num;
    }
}
