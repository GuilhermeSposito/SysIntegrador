using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysIntegradorApp.Forms;

public partial class FormLogin : Form
{
    private readonly string _usuario = "admin";
    private readonly string _senha = "69063360";
    public static bool NaoPermiteFecharForm { get; set; } = true;


    private Label lblTitle;
    private Label lblUsuario;
    private Label lblSenha;
    private TextBox txtUsuario;
    private TextBox txtSenha;
    private Button btnEntrar;
    private Button btnCancelar;

    public FormLogin()
    {
        AdicionaDesign();
        InitializeComponent();

    }

    public void AdicionaDesign()
    {
        this.BackColor = Color.FromArgb(240, 244, 248);
        this.FormBorderStyle = FormBorderStyle.FixedSingle;

        // Título
        lblTitle = new Label
        {
            Text = "SysLogica\nDesenvolvimento de Sistemas",
            Font = new Font("Arial", 14, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 60, 90),
            TextAlign = ContentAlignment.MiddleCenter,
            Size = new Size(360, 60),
            Location = new Point(20, 30)
        };

        // Label Usuário
        lblUsuario = new Label
        {
            Text = "Usuário",
            Font = new Font("Arial", 10, FontStyle.Regular),
            ForeColor = Color.FromArgb(50, 50, 50),
            Size = new Size(100, 20),
            Location = new Point(40, 120)
        };

        // TextBox Usuário
        txtUsuario = new TextBox
        {
            Text = "admin",
            Font = new Font("Arial", 10),
            Size = new Size(300, 30),
            Location = new Point(40, 150),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.White,
            ForeColor = Color.Black
        };

        // Label Senha
        lblSenha = new Label
        {
            Text = "Senha",
            Font = new Font("Arial", 10, FontStyle.Regular),
            ForeColor = Color.FromArgb(50, 50, 50),
            Size = new Size(100, 20),
            Location = new Point(40, 200)
        };

        // TextBox Senha
        txtSenha = new TextBox
        {
            UseSystemPasswordChar = true,
            Font = new Font("Arial", 10),
            Size = new Size(300, 30),
            Location = new Point(40, 230),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.White,
            ForeColor = Color.Black
        };
        txtSenha.KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Impede que o Enter produza um som de "beep"
                e.SuppressKeyPress = true;

                // Executa a mesma lógica do botão Entrar
                if (txtUsuario.Text == _usuario && txtSenha.Text == _senha)
                {
                    NaoPermiteFecharForm = false;
                    this.Close();
                }
                else
                {
                    txtSenha.SelectAll();
                    MessageBox.Show("Usuário ou senha incorretos.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        };

        // Botão Entrar
        btnEntrar = new Button
        {
            Text = "Entrar",
            Font = new Font("Arial", 10, FontStyle.Bold),
            Size = new Size(120, 40),
            Location = new Point(220, 300),
            BackColor = Color.FromArgb(30, 120, 215),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnEntrar.FlatAppearance.BorderSize = 0;
        btnEntrar.Click += (s, e) =>
        {

            if (txtUsuario.Text == _usuario && txtSenha.Text == _senha)
            {
                NaoPermiteFecharForm = false;

                this.Close();
            }
            else
            {
                MessageBox.Show("Usuário ou senha incorretos.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtSenha.SelectAll();
            }

        };

        // Botão Cancelar
        btnCancelar = new Button
        {
            Text = "Cancelar",
            Font = new Font("Arial", 10, FontStyle.Bold),
            Size = new Size(120, 40),
            Location = new Point(80, 300),
            BackColor = Color.FromArgb(200, 200, 200),
            ForeColor = Color.Black,
            FlatStyle = FlatStyle.Flat
        };
        btnCancelar.FlatAppearance.BorderSize = 0;
        btnCancelar.Click += (s, e) =>
        {
            NaoPermiteFecharForm = false;
            this.Close();

            if (Application.OpenForms["NewFormConfiguracoes"] is not null)
            {
                // Se o formulário estiver aberto, feche-o
                Application.OpenForms["NewFormConfiguracoes"]!.Close();
            }

        };



        // Adiciona os controles ao formulário
        this.Controls.AddRange(new Control[] { lblTitle, lblUsuario, txtUsuario, lblSenha, txtSenha, btnEntrar, btnCancelar });

    }

    private void FormLogin_FormClosing(object sender, FormClosingEventArgs e)
    {
        e.Cancel = NaoPermiteFecharForm;
    }

    private void FormLogin_Load(object sender, EventArgs e)
    {

        txtSenha.Focus();
    }
}
