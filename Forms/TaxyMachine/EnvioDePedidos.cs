using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Web.WebView2.Core;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoTaxyMachine;
using SysIntegradorApp.ClassesAuxiliares.logs;
using SysIntegradorApp.ClassesDeConexaoComApps;
using SysIntegradorApp.data;
using SysIntegradorApp.data.InterfaceDeContexto;
using SysIntegradorApp.UserControls.TaxyMachine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysIntegradorApp.Forms.TaxyMachine
{
    public partial class EnvioDePedidos : Form
    {
        public List<Sequencia> PedidosAEnviar { get; set; } = new List<Sequencia>();
        private readonly IMeuContexto _Context;
        private Image backgroundImage;

        public EnvioDePedidos(MeuContexto context)
        {
            InitializeComponent();
            ClsEstiloComponentes.SetRoundedRegion(PanelDePedidosAbertos, 24);
            ClsEstiloComponentes.SetRoundedRegion(panel1, 24);
            ClsEstiloComponentes.SetRoundedRegion(panel2, 24);
            ClsEstiloComponentes.SetRoundedRegion(panel3, 24);
            ClsEstiloComponentes.SetRoundedRegion(PanelDePedidosAguardando, 24);
            this.Resize += (sender, e) =>
            {
                ClsEstiloComponentes.SetRoundedRegion(PanelDePedidosAbertos, 24);
                ClsEstiloComponentes.SetRoundedRegion(panel1, 24);
                ClsEstiloComponentes.SetRoundedRegion(PanelDePedidosAguardando, 24);
                ClsEstiloComponentes.SetRoundedRegion(panel2, 24);
                ClsEstiloComponentes.SetRoundedRegion(panel3, 24);
            };

            _Context = context;
        }

        private async void EnvioDePedidos_Load(object sender, EventArgs e)
        {
            await SetPainelPedidosAberto(true);
        }

        public async Task SetPainelPedidosAberto(bool pedidosEmAguarde)
        {
            try
            {
                PanelDePedidosAbertos.Controls.Clear();
                if (pedidosEmAguarde)
                {

                    List<Sequencia> PedidosAbertos = new List<Sequencia>();
                    List<UCPedidoTaxyMachine> UsersControl = new List<UCPedidoTaxyMachine>();

                    using (ApplicationDbContext db = await _Context.GetContextoAsync())
                    {
                        ParametrosDoSistema? Config = db.parametrosdosistema.FirstOrDefault();
                        ClsDeIntegracaoSys ClasseIntegracao = new ClsDeIntegracaoSys(new MeuContexto());

                        if (Config.IntegraOttoEntregas)
                            PedidosAbertos = await ClasseIntegracao.ListarPedidosAbertos(CodEntregador: "66", empresaId: "MachineId");



                        PanelDePedidosAbertos.SuspendLayout();
                        foreach (var pedido in PedidosAbertos.OrderBy(x => x.numConta))
                        {

                            UCPedidoTaxyMachine uCPedidoTaxyMachine = new UCPedidoTaxyMachine()
                            {
                                NumConta = pedido.numConta,
                                Sequencia = pedido
                            };

                            bool ExistePedidoEmAguardo = PanelDePedidosAguardando.Controls.Contains(uCPedidoTaxyMachine);

                            if (!ExistePedidoEmAguardo)
                            {
                                uCPedidoTaxyMachine.Click += new EventHandler(UserControl_Click);
                                PanelDePedidosAbertos.Controls.Add(uCPedidoTaxyMachine);
                            }
                        }
                        PanelDePedidosAbertos.ResumeLayout();

                    }
                }
                else
                {
                    List<Sequencia> PedidosAbertos = new List<Sequencia>();
                    List<UCPedidoTaxyMachine> UsersControl = new List<UCPedidoTaxyMachine>();

                    using (ApplicationDbContext db = await _Context.GetContextoAsync())
                    {
                        ParametrosDoSistema? Config = db.parametrosdosistema.FirstOrDefault();
                        ClsDeIntegracaoSys ClasseIntegracao = new ClsDeIntegracaoSys(new MeuContexto());

                        if (Config.IntegraOttoEntregas)
                            PedidosAbertos = await ClasseIntegracao.ListarPedidosJaEnviados(CodEntregador: "66", empresaId: "MachineId");

                        PanelDePedidosAbertos.SuspendLayout();
                        foreach (var pedido in PedidosAbertos.OrderBy(x => x.numConta))
                        {

                            UCPedidoTaxyMachine uCPedidoTaxyMachine = new UCPedidoTaxyMachine()
                            {
                                NumConta = pedido.numConta,
                                Sequencia = pedido
                            };

                            uCPedidoTaxyMachine.MudaLabelParaEnviado();

                            bool ExistePedidoEmAguardo = PanelDePedidosAguardando.Controls.Contains(uCPedidoTaxyMachine);

                            if (!ExistePedidoEmAguardo)
                            {
                                uCPedidoTaxyMachine.Click += new EventHandler(UserControl_Click);
                                PanelDePedidosAbertos.Controls.Add(uCPedidoTaxyMachine);
                            }
                        }
                        PanelDePedidosAbertos.ResumeLayout();

                    }
                }

            }
            catch (Exception ex)
            {
                await Logs.CriaLogDeErro(ex.ToString());
            }

        }

        public void DefinePanelPedidosAEnviar()
        {
        }

        private void UserControl_Click(object sender, EventArgs e)
        {
            var userControl = sender as UCPedidoTaxyMachine;
            if (userControl != null)
            {
                userControl.Click -= UserControl_Click;

                userControl.Click += new EventHandler(RemoveUserControl_Click);
                PedidosAEnviar.Add(userControl.Sequencia);
                PanelDePedidosAguardando.Controls.Add(userControl);
            }
        }

        private async void button1_Click(object sender, EventArgs e) //Botão de enviar pedidos
        {
            try
            {
                bool DesejaEnviar = false;

                if (checkBoxMostrarPedidosEnviados.Checked)
                {
                    DialogResult RespostaUser = MessageBox.Show("Pedidos já foram enviado uma vez, você deseja envia-los novamente ?", "Pedidos Já Enviados", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (RespostaUser == DialogResult.Yes)
                    {
                        DesejaEnviar = true;
                    }
                }
                else
                {
                    DesejaEnviar = true;
                }

                if (DesejaEnviar)
                {
                    button1.Enabled = false;
                    buttonCancelar.Enabled = false;

                    decimal minutos = NumeroDeMinutos.Value;
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        ParametrosDoSistema? Config = db.parametrosdosistema.FirstOrDefault();

                        this.Cursor = Cursors.WaitCursor;

                        if (Config.IntegraOttoEntregas)
                        {
                            OTTO Otto = new OTTO(new MeuContexto());

                            string? MenssagemDeErro = "";
                            bool EntregaImediata = false;
                            bool eRota = false;

                            if (checkBoxImediata.Checked)
                                EntregaImediata = true;

                            if (checkBoxRota.Checked)
                                eRota = true;

                            if (PedidosAEnviar.Count == 0)
                                throw new NullReferenceException("Nenhum pedido foi selecionado para ser enviado");

                            List<ClsApoioRespostaApi>? respostasEnvios = await Otto.EnviaPedidosManualmente(PedidosAbertos: PedidosAEnviar, imediata: EntregaImediata, codEntregador: "66", minutos: (int)minutos, rota: eRota);

                            MenssagemDeErro = Otto.retornaMensagemDeErro(respostasEnvios);

                            if (MenssagemDeErro is not null)
                            {                          
                                if (MenssagemDeErro.Length > 2)
                                {
                                    MessageBox.Show(MenssagemDeErro, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                else
                                {
                                    MessageBox.Show("Pedidos Enviados Com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    PanelDePedidosAguardando.Controls.Clear();
                                    await SetPainelPedidosAberto(true);

                                    PedidosAEnviar.Clear();
                                }
                            }

                        }
                    }
                }

            }
            catch (Exception ex) when (ex.Message.Contains("Nenhum pedido foi selecionado para ser enviado"))
            {
                MessageBox.Show(ex.Message, "Vazio", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception ex)
            {
                await Logs.CriaLogDeErro(ex.ToString());
            }
            finally
            {
                this.Cursor = Cursors.Default;

                button1.Enabled = true;
                buttonCancelar.Enabled = true;
            }
        }

        private void RemoveUserControl_Click(object sender, EventArgs e)
        {
            var userControl = sender as UCPedidoTaxyMachine;
            if (userControl != null)
            {
                userControl.Click -= RemoveUserControl_Click;
                userControl.Click += UserControl_Click;

                PedidosAEnviar.Remove(userControl.Sequencia);

                PanelDePedidosAguardando.Controls.Remove(userControl);
                PanelDePedidosAbertos.Controls.Add(userControl);
            }
        }

        private void buttonCancelar_Click(object sender, EventArgs e)
        {
            PedidosAEnviar.Clear();
            this.Close();
        }

        private void checkBoxAgendada_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAgendada.Checked == true)
                checkBoxImediata.Checked = false;
            
            if (checkBoxAgendada.Checked == false)
                checkBoxImediata.Checked = true;
        }

        private void checkBoxImediata_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxImediata.Checked == true)
                checkBoxAgendada.Checked = false;

            if (checkBoxImediata.Checked == false)
                checkBoxAgendada.Checked = true; 

        }

        private async void checkBoxMostrarPedidosEnviados_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxMostrarPedidosEnviados.Checked)
            {

                await SetPainelPedidosAberto(false);
                labelMostrarPedido.Text = "Pedidos Já Enviados";
                PanelDePedidosAguardando.Controls.Clear();
                PedidosAEnviar.Clear();
                checkBoxMostrarPedidosEnviados.Enabled = true;
            }

            if (!checkBoxMostrarPedidosEnviados.Checked)
            {
                checkBoxMostrarPedidosEnviados.Enabled = false;
                await SetPainelPedidosAberto(true);
                PanelDePedidosAguardando.Controls.Clear();
                labelMostrarPedido.Text = "Pedidos Em Aguardo";
                checkBoxMostrarPedidosEnviados.Enabled = true;
            }
        }

        private void EnvioDePedidos_FormClosing(object sender, FormClosingEventArgs e)
        {
            PedidosAEnviar.Clear();
        }

        private void PanelDePedidosAguardando_Paint(object sender, PaintEventArgs e)
        {
          
        }
    }


}
