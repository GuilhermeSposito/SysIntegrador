using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysIntegradorApp.Forms
{
    public partial class FormLoginConfigs : Form
    {

        public static string User { get; set; } = "admin";
        public static string Senha { get; set; } = "69063360";
        public static bool NaoPermiteFecharForm { get; set; } = true;

        public FormLoginConfigs()
        {
            InitializeComponent();
            NaoPermiteFecharForm = true;

        }

        public static void ValidaLogin(string? user, string? senha)
        {
            try
            {
                if (user == User && senha == Senha)
                {
                    NaoPermiteFecharForm = false;
                    if (Application.OpenForms["FormLoginConfigs"] != null)
                    {
                        // Se o formulário estiver aberto, feche-o
                        Application.OpenForms["FormLoginConfigs"].Close();
                    }
                }
                else
                {
                    throw new Exception("Senha Ou Usuario Incorretos");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ops");
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            NaoPermiteFecharForm = false;
            this.Close();
            if (Application.OpenForms["FormDeParametrosDoSistema"] != null)
            {
                // Se o formulário estiver aberto, feche-o
                Application.OpenForms["FormDeParametrosDoSistema"].Close();
            }


            if (Application.OpenForms["NewFormConfiguracoes"] != null)
            {
                // Se o formulário estiver aberto, feche-o
                Application.OpenForms["NewFormConfiguracoes"].Close();
            }
        }

        private void btnEntrar_Click(object sender, EventArgs e)
        {
            string user = textBoxUser.Text;
            string senha = textSenha.Text;


            ValidaLogin(user, senha);
        }

        private void FormLoginConfigs_FormClosed(object sender, FormClosedEventArgs e) { }

        private void FormLoginConfigs_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = NaoPermiteFecharForm;
        }

        private void textSenha_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnEntrar_Click(sender, e);
            }
        }

        private void FormLoginConfigs_Load(object sender, EventArgs e)
        {
            textSenha.Focus();
        }

        private void FormLoginConfigs_Shown(object sender, EventArgs e)
        {
            textSenha.Focus();
        }
    }
}
