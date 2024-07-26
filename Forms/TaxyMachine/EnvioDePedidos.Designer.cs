namespace SysIntegradorApp.Forms.TaxyMachine
{
    partial class EnvioDePedidos
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EnvioDePedidos));
            PanelDePedidosAbertos = new FlowLayoutPanel();
            PanelDePedidosAguardando = new FlowLayoutPanel();
            checkBoxRota = new CheckBox();
            checkBoxImediata = new CheckBox();
            checkBoxAgendada = new CheckBox();
            NumeroDeMinutos = new NumericUpDown();
            buttonCancelar = new Button();
            button1 = new Button();
            panel1 = new Panel();
            labelMostrarPedido = new Label();
            panel2 = new Panel();
            checkBoxMostrarPedidosEnviados = new CheckBox();
            panel3 = new Panel();
            ((System.ComponentModel.ISupportInitialize)NumeroDeMinutos).BeginInit();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // PanelDePedidosAbertos
            // 
            PanelDePedidosAbertos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            PanelDePedidosAbertos.AutoScroll = true;
            PanelDePedidosAbertos.BackColor = SystemColors.ButtonHighlight;
            PanelDePedidosAbertos.Location = new Point(12, 57);
            PanelDePedidosAbertos.Name = "PanelDePedidosAbertos";
            PanelDePedidosAbertos.Size = new Size(447, 651);
            PanelDePedidosAbertos.TabIndex = 0;
            // 
            // PanelDePedidosAguardando
            // 
            PanelDePedidosAguardando.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            PanelDePedidosAguardando.AutoScroll = true;
            PanelDePedidosAguardando.BackColor = SystemColors.ButtonHighlight;
            PanelDePedidosAguardando.BackgroundImage = (Image)resources.GetObject("PanelDePedidosAguardando.BackgroundImage");
            PanelDePedidosAguardando.BackgroundImageLayout = ImageLayout.Zoom;
            PanelDePedidosAguardando.Location = new Point(474, 57);
            PanelDePedidosAguardando.Name = "PanelDePedidosAguardando";
            PanelDePedidosAguardando.Padding = new Padding(30, 0, 0, 0);
            PanelDePedidosAguardando.Size = new Size(723, 651);
            PanelDePedidosAguardando.TabIndex = 1;
            PanelDePedidosAguardando.Paint += PanelDePedidosAguardando_Paint;
            // 
            // checkBoxRota
            // 
            checkBoxRota.AutoSize = true;
            checkBoxRota.Cursor = Cursors.Hand;
            checkBoxRota.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            checkBoxRota.Location = new Point(519, 22);
            checkBoxRota.Name = "checkBoxRota";
            checkBoxRota.Size = new Size(148, 24);
            checkBoxRota.TabIndex = 2;
            checkBoxRota.Text = "Enviar como rota";
            checkBoxRota.UseVisualStyleBackColor = true;
            // 
            // checkBoxImediata
            // 
            checkBoxImediata.Anchor = AnchorStyles.Top;
            checkBoxImediata.AutoSize = true;
            checkBoxImediata.Cursor = Cursors.Hand;
            checkBoxImediata.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            checkBoxImediata.Location = new Point(755, 22);
            checkBoxImediata.Name = "checkBoxImediata";
            checkBoxImediata.Size = new Size(90, 24);
            checkBoxImediata.TabIndex = 3;
            checkBoxImediata.Text = "Imediata";
            checkBoxImediata.UseVisualStyleBackColor = true;
            checkBoxImediata.CheckedChanged += checkBoxImediata_CheckedChanged;
            // 
            // checkBoxAgendada
            // 
            checkBoxAgendada.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            checkBoxAgendada.AutoSize = true;
            checkBoxAgendada.Checked = true;
            checkBoxAgendada.CheckState = CheckState.Checked;
            checkBoxAgendada.Cursor = Cursors.Hand;
            checkBoxAgendada.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            checkBoxAgendada.Location = new Point(962, 22);
            checkBoxAgendada.Name = "checkBoxAgendada";
            checkBoxAgendada.Size = new Size(101, 24);
            checkBoxAgendada.TabIndex = 4;
            checkBoxAgendada.Text = "Agendada";
            checkBoxAgendada.UseVisualStyleBackColor = true;
            checkBoxAgendada.CheckedChanged += checkBoxAgendada_CheckedChanged;
            // 
            // NumeroDeMinutos
            // 
            NumeroDeMinutos.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            NumeroDeMinutos.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            NumeroDeMinutos.Location = new Point(1069, 19);
            NumeroDeMinutos.Name = "NumeroDeMinutos";
            NumeroDeMinutos.Size = new Size(54, 27);
            NumeroDeMinutos.TabIndex = 5;
            NumeroDeMinutos.Value = new decimal(new int[] { 40, 0, 0, 0 });
            // 
            // buttonCancelar
            // 
            buttonCancelar.Anchor = AnchorStyles.Bottom;
            buttonCancelar.BackColor = Color.Red;
            buttonCancelar.Cursor = Cursors.Hand;
            buttonCancelar.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            buttonCancelar.ForeColor = SystemColors.ButtonHighlight;
            buttonCancelar.Location = new Point(577, 715);
            buttonCancelar.Name = "buttonCancelar";
            buttonCancelar.Size = new Size(194, 55);
            buttonCancelar.TabIndex = 6;
            buttonCancelar.Text = "Cancelar";
            buttonCancelar.UseVisualStyleBackColor = false;
            buttonCancelar.Click += buttonCancelar_Click;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom;
            button1.BackColor = Color.Red;
            button1.Cursor = Cursors.Hand;
            button1.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.ForeColor = SystemColors.ButtonHighlight;
            button1.Location = new Point(901, 715);
            button1.Name = "button1";
            button1.Size = new Size(194, 55);
            button1.TabIndex = 7;
            button1.Text = "Enviar";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ButtonHighlight;
            panel1.Controls.Add(labelMostrarPedido);
            panel1.Location = new Point(12, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(447, 39);
            panel1.TabIndex = 9;
            // 
            // labelMostrarPedido
            // 
            labelMostrarPedido.AutoSize = true;
            labelMostrarPedido.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelMostrarPedido.Location = new Point(124, 4);
            labelMostrarPedido.Name = "labelMostrarPedido";
            labelMostrarPedido.Size = new Size(202, 28);
            labelMostrarPedido.TabIndex = 0;
            labelMostrarPedido.Text = "Pedidos Em Aguardo";
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            panel2.BackColor = SystemColors.ButtonHighlight;
            panel2.Controls.Add(checkBoxMostrarPedidosEnviados);
            panel2.Location = new Point(12, 714);
            panel2.Name = "panel2";
            panel2.Size = new Size(447, 56);
            panel2.TabIndex = 10;
            // 
            // checkBoxMostrarPedidosEnviados
            // 
            checkBoxMostrarPedidosEnviados.Cursor = Cursors.Hand;
            checkBoxMostrarPedidosEnviados.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            checkBoxMostrarPedidosEnviados.Location = new Point(96, 6);
            checkBoxMostrarPedidosEnviados.Name = "checkBoxMostrarPedidosEnviados";
            checkBoxMostrarPedidosEnviados.Size = new Size(249, 50);
            checkBoxMostrarPedidosEnviados.TabIndex = 3;
            checkBoxMostrarPedidosEnviados.Text = "Mostrar Pedidos Já enviados";
            checkBoxMostrarPedidosEnviados.UseVisualStyleBackColor = true;
            checkBoxMostrarPedidosEnviados.CheckedChanged += checkBoxMostrarPedidosEnviados_CheckedChanged;
            // 
            // panel3
            // 
            panel3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel3.BackColor = SystemColors.ButtonHighlight;
            panel3.Location = new Point(474, 713);
            panel3.Name = "panel3";
            panel3.Size = new Size(723, 57);
            panel3.TabIndex = 11;
            // 
            // EnvioDePedidos
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDark;
            ClientSize = new Size(1209, 781);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(button1);
            Controls.Add(buttonCancelar);
            Controls.Add(NumeroDeMinutos);
            Controls.Add(checkBoxAgendada);
            Controls.Add(checkBoxImediata);
            Controls.Add(checkBoxRota);
            Controls.Add(PanelDePedidosAguardando);
            Controls.Add(PanelDePedidosAbertos);
            Controls.Add(panel3);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "EnvioDePedidos";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Envio De Pedidos para delivery";
            WindowState = FormWindowState.Maximized;
            FormClosing += EnvioDePedidos_FormClosing;
            Load += EnvioDePedidos_Load;
            ((System.ComponentModel.ISupportInitialize)NumeroDeMinutos).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private FlowLayoutPanel PanelDePedidosAbertos;
        private FlowLayoutPanel PanelDePedidosAguardando;
        private CheckBox checkBoxRota;
        private CheckBox checkBoxImediata;
        private CheckBox checkBoxAgendada;
        private NumericUpDown NumeroDeMinutos;
        private Button buttonCancelar;
        private Button button1;
        private Panel panel1;
        private Label labelMostrarPedido;
        private Panel panel2;
        private Panel panel3;
        private CheckBox checkBoxMostrarPedidosEnviados;
    }
}