namespace SysIntegradorApp.Forms
{
    partial class FormLoginConfigs
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLoginConfigs));
            pictureBox1 = new PictureBox();
            groupBox1 = new GroupBox();
            btnEntrar = new Button();
            btnCancelar = new Button();
            label2 = new Label();
            label1 = new Label();
            textSenha = new TextBox();
            textBoxUser = new TextBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(147, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(182, 62);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnEntrar);
            groupBox1.Controls.Add(btnCancelar);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(textSenha);
            groupBox1.Controls.Add(textBoxUser);
            groupBox1.ForeColor = SystemColors.ActiveCaptionText;
            groupBox1.Location = new Point(37, 80);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(404, 261);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Login";
            // 
            // btnEntrar
            // 
            btnEntrar.Cursor = Cursors.Hand;
            btnEntrar.Location = new Point(222, 186);
            btnEntrar.Name = "btnEntrar";
            btnEntrar.Size = new Size(111, 45);
            btnEntrar.TabIndex = 5;
            btnEntrar.Text = "Entrar";
            btnEntrar.UseVisualStyleBackColor = true;
            btnEntrar.Click += btnEntrar_Click;
            // 
            // btnCancelar
            // 
            btnCancelar.Cursor = Cursors.Hand;
            btnCancelar.Location = new Point(61, 186);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(111, 45);
            btnCancelar.TabIndex = 4;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(61, 112);
            label2.Name = "label2";
            label2.Size = new Size(49, 20);
            label2.TabIndex = 3;
            label2.Text = "Senha";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(61, 50);
            label1.Name = "label1";
            label1.Size = new Size(59, 20);
            label1.TabIndex = 2;
            label1.Text = "Usuario";
            // 
            // textSenha
            // 
            textSenha.Font = new Font("Segoe UI", 12F);
            textSenha.Location = new Point(61, 135);
            textSenha.Name = "textSenha";
            textSenha.Size = new Size(272, 34);
            textSenha.TabIndex = 1;
            textSenha.UseSystemPasswordChar = true;
            textSenha.KeyPress += textSenha_KeyPress;
            // 
            // textBoxUser
            // 
            textBoxUser.Font = new Font("Segoe UI", 12F);
            textBoxUser.Location = new Point(61, 73);
            textBoxUser.Name = "textBoxUser";
            textBoxUser.Size = new Size(272, 34);
            textBoxUser.TabIndex = 0;
            textBoxUser.Text = "admin";
            // 
            // FormLoginConfigs
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ButtonShadow;
            ClientSize = new Size(484, 381);
            Controls.Add(groupBox1);
            Controls.Add(pictureBox1);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormLoginConfigs";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Login";
            FormClosing += FormLoginConfigs_FormClosing;
            FormClosed += FormLoginConfigs_FormClosed;
            Load += FormLoginConfigs_Load;
            Shown += FormLoginConfigs_Shown;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox1;
        private GroupBox groupBox1;
        private TextBox textSenha;
        private TextBox textBoxUser;
        private Label label2;
        private Label label1;
        private Button btnEntrar;
        private Button btnCancelar;
    }
}