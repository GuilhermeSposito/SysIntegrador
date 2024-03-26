namespace SysIntegradorApp
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            pictureBox1 = new PictureBox();
            PedirCod = new Button();
            BtnCancelar = new Button();
            BrnAutorizar = new Button();
            LabelInfoToUser = new Label();
            LabelCodeToUser = new Label();
            CodeFromUser = new TextBox();
            CodeLabel1 = new Label();
            AvisoParaPegarCod = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.InitialImage = (Image)resources.GetObject("pictureBox1.InitialImage");
            pictureBox1.Location = new Point(132, -73);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(509, 212);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // PedirCod
            // 
            PedirCod.Location = new Point(91, 367);
            PedirCod.Name = "PedirCod";
            PedirCod.Size = new Size(180, 69);
            PedirCod.TabIndex = 1;
            PedirCod.Text = "Pedir Cod.";
            PedirCod.UseVisualStyleBackColor = true;
            PedirCod.Click += PedirCod_Click;
            // 
            // BtnCancelar
            // 
            BtnCancelar.Location = new Point(293, 367);
            BtnCancelar.Name = "BtnCancelar";
            BtnCancelar.Size = new Size(180, 71);
            BtnCancelar.TabIndex = 2;
            BtnCancelar.Text = "Cancelar";
            BtnCancelar.UseVisualStyleBackColor = true;
            BtnCancelar.Click += BtnCancelar_Click;
            // 
            // BrnAutorizar
            // 
            BrnAutorizar.Location = new Point(496, 367);
            BrnAutorizar.Name = "BrnAutorizar";
            BrnAutorizar.Size = new Size(180, 74);
            BrnAutorizar.TabIndex = 3;
            BrnAutorizar.Text = "Autorizar";
            BrnAutorizar.UseVisualStyleBackColor = true;
            BrnAutorizar.Click += BrnAutorizar_Click;
            // 
            // LabelInfoToUser
            // 
            LabelInfoToUser.AutoSize = true;
            LabelInfoToUser.Font = new Font("Tahoma", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            LabelInfoToUser.Location = new Point(207, 142);
            LabelInfoToUser.Name = "LabelInfoToUser";
            LabelInfoToUser.Size = new Size(356, 18);
            LabelInfoToUser.TabIndex = 4;
            LabelInfoToUser.Text = "Insira o Seguinte Código no portal do parceiro:";
            LabelInfoToUser.Visible = false;
            LabelInfoToUser.Click += LabelInfoToUser_Click;
            // 
            // LabelCodeToUser
            // 
            LabelCodeToUser.Font = new Font("Segoe UI", 15F);
            LabelCodeToUser.Location = new Point(293, 160);
            LabelCodeToUser.Name = "LabelCodeToUser";
            LabelCodeToUser.Size = new Size(170, 43);
            LabelCodeToUser.TabIndex = 5;
            LabelCodeToUser.Text = "BXBB-XXHS";
            LabelCodeToUser.TextAlign = ContentAlignment.MiddleCenter;
            LabelCodeToUser.Visible = false;
            // 
            // CodeFromUser
            // 
            CodeFromUser.Location = new Point(236, 265);
            CodeFromUser.Name = "CodeFromUser";
            CodeFromUser.PlaceholderText = "Insira o código Fornecido pelo ifood";
            CodeFromUser.Size = new Size(279, 27);
            CodeFromUser.TabIndex = 6;
            CodeFromUser.TextAlign = HorizontalAlignment.Center;
            CodeFromUser.Visible = false;
            CodeFromUser.KeyDown += CodeFromUser_KeyDown;
            // 
            // CodeLabel1
            // 
            CodeLabel1.AutoSize = true;
            CodeLabel1.Location = new Point(236, 242);
            CodeLabel1.Name = "CodeLabel1";
            CodeLabel1.Size = new Size(61, 20);
            CodeLabel1.TabIndex = 7;
            CodeLabel1.Text = "Código:";
            CodeLabel1.Visible = false;
            // 
            // AvisoParaPegarCod
            // 
            AvisoParaPegarCod.Font = new Font("Tahoma", 14F, FontStyle.Regular, GraphicsUnit.Point, 0);
            AvisoParaPegarCod.Location = new Point(38, 226);
            AvisoParaPegarCod.Name = "AvisoParaPegarCod";
            AvisoParaPegarCod.Size = new Size(750, 66);
            AvisoParaPegarCod.TabIndex = 8;
            AvisoParaPegarCod.Text = "Clique em Perdir Código para gerar um Cod. de Autorização";
            AvisoParaPegarCod.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(AvisoParaPegarCod);
            Controls.Add(CodeLabel1);
            Controls.Add(CodeFromUser);
            Controls.Add(LabelCodeToUser);
            Controls.Add(LabelInfoToUser);
            Controls.Add(BrnAutorizar);
            Controls.Add(BtnCancelar);
            Controls.Add(PedirCod);
            Controls.Add(pictureBox1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Autorização Ifood";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Button PedirCod;
        private Button BtnCancelar;
        private Button BrnAutorizar;
        private Label LabelInfoToUser;
        private Label LabelCodeToUser;
        private TextBox CodeFromUser;
        private Label CodeLabel1;
        private Label AvisoParaPegarCod;
    }
}
