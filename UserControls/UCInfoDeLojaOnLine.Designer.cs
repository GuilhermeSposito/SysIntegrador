namespace SysIntegradorApp.UserControls
{
    partial class UCInfoDeLojaOnLine
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCInfoDeLojaOnLine));
            LblNomeDaEmpresa = new Label();
            labelStatus = new Label();
            pictureBoxOfline = new PictureBox();
            pictureBoxOnline = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOfline).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOnline).BeginInit();
            SuspendLayout();
            // 
            // LblNomeDaEmpresa
            // 
            LblNomeDaEmpresa.BackColor = Color.Red;
            LblNomeDaEmpresa.Dock = DockStyle.Top;
            LblNomeDaEmpresa.Location = new Point(0, 0);
            LblNomeDaEmpresa.Name = "LblNomeDaEmpresa";
            LblNomeDaEmpresa.Size = new Size(339, 20);
            LblNomeDaEmpresa.TabIndex = 0;
            LblNomeDaEmpresa.Text = "Nome Da Loja";
            LblNomeDaEmpresa.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelStatus
            // 
            labelStatus.Dock = DockStyle.Top;
            labelStatus.Location = new Point(0, 20);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(339, 20);
            labelStatus.TabIndex = 1;
            labelStatus.Text = "ONLINE";
            labelStatus.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pictureBoxOfline
            // 
            pictureBoxOfline.Image = (Image)resources.GetObject("pictureBoxOfline.Image");
            pictureBoxOfline.Location = new Point(0, 40);
            pictureBoxOfline.Name = "pictureBoxOfline";
            pictureBoxOfline.Size = new Size(339, 80);
            pictureBoxOfline.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxOfline.TabIndex = 11;
            pictureBoxOfline.TabStop = false;
            // 
            // pictureBoxOnline
            // 
            pictureBoxOnline.Image = (Image)resources.GetObject("pictureBoxOnline.Image");
            pictureBoxOnline.Location = new Point(0, 43);
            pictureBoxOnline.Name = "pictureBoxOnline";
            pictureBoxOnline.Size = new Size(339, 77);
            pictureBoxOnline.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxOnline.TabIndex = 12;
            pictureBoxOnline.TabStop = false;
            pictureBoxOnline.Visible = false;
            // 
            // UCInfoDeLojaOnLine
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Red;
            Controls.Add(labelStatus);
            Controls.Add(LblNomeDaEmpresa);
            Controls.Add(pictureBoxOfline);
            Controls.Add(pictureBoxOnline);
            Name = "UCInfoDeLojaOnLine";
            Size = new Size(339, 120);
            ((System.ComponentModel.ISupportInitialize)pictureBoxOfline).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOnline).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Label LblNomeDaEmpresa;
        private Label labelStatus;
        private PictureBox pictureBoxOfline;
        private PictureBox pictureBoxOnline;
    }
}
