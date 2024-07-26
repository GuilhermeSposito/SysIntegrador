using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.Logging;
using Newtonsoft.Json;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoCCM;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoDelmatch;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoTaxyMachine;
using SysIntegradorApp.ClassesAuxiliares.logs;
using SysIntegradorApp.data;
using SysIntegradorApp.data.InterfaceDeContexto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace SysIntegradorApp.ClassesDeConexaoComApps;

public class TaxyMachine
{
    private readonly IMeuContexto _Context;
    public string? _ApiKey { get; set; }
    public string? UrlApi { get { return "https://cloud.taximachine.com.br/api/integracao"; } }
    public string? Username { get; set; } = string.Empty;
    public string? Password { get; set; } = string.Empty;

    public TaxyMachine(MeuContexto context)
    {
        _Context = context;
    }

    public async Task<string?> GetApiAuthAsync()
    {
        try
        {
            await using (ApplicationDbContext db = await _Context.GetContextoAsync())
            {
                ParametrosDoSistema? ConfigSist = await db.parametrosdosistema.FirstOrDefaultAsync();

                _ApiKey = ConfigSist.ApiKeyTaxyMachine;
                Username = ConfigSist.UserNameTaxyMachine;
                Password = ConfigSist.PasswordTaxyMachine;

                return ConfigSist.ApiKeyTaxyMachine;
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.Message);
            MessageBox.Show("Erro ao ter chave de acesso do banco de dados", "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        return null;
    }

    public async Task<EmpresaInfos?> GetInfosEmpresa()
    {
        try
        {
            HttpResponseMessage? Response = await ReqConstructor("GET", "/empresa");

            if (Response is null)
                throw new Exception("Erro ao enviar requisição Taxy Machine");


            if (Response.IsSuccessStatusCode)
            {
                EmpresaInfos? Empresa = JsonConvert.DeserializeObject<EmpresaInfos>(await Response.Content.ReadAsStringAsync());

                return Empresa;
            }

        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.Message);
            MessageBox.Show("Erro ao acessar informações da empresa!", "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
        return null;
    }

    public async Task<List<AbrirSolicitacao>?> EnviaPedidosAutomaticamente(string? codEntregador, bool imediata = false, int minutos = 40)
    {
        try
        {
            using (ApplicationDbContext Db = await _Context.GetContextoAsync())
            {
                ParametrosDoSistema? Config = await Db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                {
                    minutos = Config.TempoEntrega - 20;
                }
            }

            EmpresaInfos? Empresa = await GetInfosEmpresa();

            string? Data = null;
            string? Hora = null;

            List<AbrirSolicitacao> Solicitacoes = new List<AbrirSolicitacao>();
            ClsDeIntegracaoSys IntegraClass = new ClsDeIntegracaoSys(new MeuContexto());

            if (codEntregador is null)
                throw new NullReferenceException("Informe Um código de entregador valido");

            List<Sequencia> PedidosAbertos = await IntegraClass.ListarPedidosAbertos(CodEntregador: codEntregador, empresaId: "MachineId"); //teste com esse campo e id por enquanto

            if (!imediata)
            {
                Data = DateTime.Now.AddMinutes(minutos).ToString().Substring(0, 10);
                Hora = DateTime.Now.AddMinutes(minutos).ToString().Substring(11, 5);
            }

            List<ClsApoioUpdateId> clsApoioUpdateIds = new List<ClsApoioUpdateId>();
            List<ClsApoioRespostaApi>? clsApoioReturn = new List<ClsApoioRespostaApi>();

            foreach (var pedido in PedidosAbertos)
            {
                Solicitacoes.Add(new AbrirSolicitacao()
                {
                    numConta = pedido.numConta,
                    FormaDePagamento = "D",
                    EmpresaId = Empresa.Infos.Id,
                    Partida = new Partida()
                    {
                        EnderecoPartida = Empresa.Infos.Endereco,
                        BairroPartida = Empresa.Infos.Bairro,
                        ComplementoPartida = null,
                        CidadePartida = Empresa.Infos.Cidade,
                        ReferenciaPartida = null,
                        LatPartida = Empresa.Infos.Latitude,
                        LngPartida = Empresa.Infos.Longitude,
                    },
                    Paradas = new List<Paradas>()
                    {
                        new Paradas()
                        {
                            EnderecoParada = pedido.DeliveryAddress.FormattedAddress,
                            BairroParada = pedido.DeliveryAddress.Neighborhood,
                            ComplementoParada = pedido.DeliveryAddress.Complement,
                            CidadeParada = Empresa.Infos.Cidade,
                            EstadoParada = "SP",
                            ReferenciaParada = null,
                            IdExterno = pedido.numConta.ToString().PadLeft(4,'0'),
                            NomeClienteParada = pedido.Customer.Name,
                            TelefoneClienteParada = pedido.Customer.Phone,
                        }
                    },
                    Data = Data,
                    Hora = Hora,
                    Retorno = false,
                    Antecedencia = 1
                });
            }

            string jsoncontent = JsonConvert.SerializeObject(Solicitacoes);

            List<int> PedidosEnviados = new List<int>();
            if (Solicitacoes.Count > 0)
                foreach (var solicitacao in Solicitacoes)
                {
                    var Response = await ReqConstructor(metodo: "POST", endpoint: "/abrirSolicitacao", content: JsonConvert.SerializeObject(solicitacao));

                    if (Response.IsSuccessStatusCode)
                    {
                        string? respostaJson = await Response.Content.ReadAsStringAsync();

                        ClsApoioRespostaApi? clsApoioRespostaApi = JsonConvert.DeserializeObject<ClsApoioRespostaApi>(respostaJson);

                        clsApoioUpdateIds.Add(new ClsApoioUpdateId()
                        {
                            NumConta = solicitacao.numConta,
                            MachineId = clsApoioRespostaApi.Response.Machine_Id,
                            EPedidoAgrupado = false
                        });

                        clsApoioReturn.Add(clsApoioRespostaApi);
                    }
                    else
                    {
                        string? respostaJson = await Response.Content.ReadAsStringAsync();
                        ClsApoioRespostaApi? clsApoioRespostaApi = JsonConvert.DeserializeObject<ClsApoioRespostaApi>(respostaJson);
                        clsApoioRespostaApi.NumConta = solicitacao.numConta;


                        clsApoioUpdateIds.Add(new ClsApoioUpdateId()
                        {
                            NumConta = solicitacao.numConta,
                            MachineId = "ERRO",
                            EPedidoAgrupado = false
                        });

                        clsApoioReturn.Add(clsApoioRespostaApi);
                    }
                }

            if (clsApoioUpdateIds.Count > 0)
                IntegraClass.UpdateMachineId(clsApoioUpdateIds);

           string? Erro =  retornaMensagemDeErro(clsApoioReturn);

            if (Erro is not null && Erro.Length > 0)
                MessageBox.Show(Erro, "Erro ao enviar pedidos automaticamente!", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show("Erro Enviar Pedido Automatico!", "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        return null;
    }

    public async Task<List<ClsApoioRespostaApi>?> EnviaPedidosManualmente(List<Sequencia> PedidosAbertos, string? codEntregador, bool imediata = false, int minutos = 40, bool rota = false)
    {
        try
        {
            EmpresaInfos? Empresa = await GetInfosEmpresa();

            string? Data = null;
            string? Hora = null;

            List<AbrirSolicitacao> Solicitacoes = new List<AbrirSolicitacao>();
            ClsDeIntegracaoSys IntegraClass = new ClsDeIntegracaoSys(new MeuContexto());

            if (codEntregador is null)
                throw new NullReferenceException("Informe Um código de entregador valido");

            if (!imediata)
            {
                Data = DateTime.Now.AddMinutes(minutos).ToString().Substring(0, 10);
                Hora = DateTime.Now.AddMinutes(minutos).ToString().Substring(11, 5);
            }

            if (!rota)
            {
                List<ClsApoioUpdateId> clsApoioUpdateIds = new List<ClsApoioUpdateId>();
                List<ClsApoioRespostaApi>? clsApoioReturn = new List<ClsApoioRespostaApi>();

                foreach (var pedido in PedidosAbertos)
                {
                    Solicitacoes.Add(new AbrirSolicitacao()
                    {
                        numConta = pedido.numConta,
                        FormaDePagamento = "D",
                        EmpresaId = Empresa.Infos.Id,
                        Partida = new Partida()
                        {
                            EnderecoPartida = Empresa.Infos.Endereco,
                            BairroPartida = Empresa.Infos.Bairro,
                            ComplementoPartida = null,
                            CidadePartida = Empresa.Infos.Cidade,
                            ReferenciaPartida = null,
                            LatPartida = Empresa.Infos.Latitude,
                            LngPartida = Empresa.Infos.Longitude,
                        },
                        Paradas = new List<Paradas>()
                    {
                        new Paradas()
                        {
                            EnderecoParada = pedido.DeliveryAddress.FormattedAddress,
                            BairroParada = pedido.DeliveryAddress.Neighborhood,
                            ComplementoParada = pedido.DeliveryAddress.Complement,
                            CidadeParada = Empresa.Infos.Cidade,
                            EstadoParada = "SP",
                            ReferenciaParada = null,
                            IdExterno = pedido.numConta.ToString().PadLeft(4,'0'),
                            NomeClienteParada = pedido.Customer.Name,
                            TelefoneClienteParada = pedido.Customer.Phone,
                        }
                    },
                        Data = Data,
                        Hora = Hora,
                        Retorno = pedido.Retorno,
                        Antecedencia = 1
                    });
                }

                string jsoncontent = JsonConvert.SerializeObject(Solicitacoes);

                List<int> PedidosEnviados = new List<int>();
                if (Solicitacoes.Count > 0)
                    foreach (var solicitacao in Solicitacoes)
                    {
                        var Response = await ReqConstructor(metodo: "POST", endpoint: "/abrirSolicitacao", content: JsonConvert.SerializeObject(solicitacao));

                        string? respostaJsonTeste = await Response.Content.ReadAsStringAsync();

                        if (Response.IsSuccessStatusCode)
                        {
                            string? respostaJson = await Response.Content.ReadAsStringAsync();

                            ClsApoioRespostaApi? clsApoioRespostaApi = JsonConvert.DeserializeObject<ClsApoioRespostaApi>(respostaJson);

                            clsApoioUpdateIds.Add(new ClsApoioUpdateId()
                            {
                                NumConta = solicitacao.numConta,
                                MachineId = clsApoioRespostaApi.Response.Machine_Id,
                                EPedidoAgrupado = false
                            });

                            clsApoioReturn.Add(clsApoioRespostaApi);
                        }
                        else
                        {
                            string? respostaJson = await Response.Content.ReadAsStringAsync();
                            ClsApoioRespostaApi? clsApoioRespostaApi = JsonConvert.DeserializeObject<ClsApoioRespostaApi>(respostaJson);
                            clsApoioRespostaApi.NumConta = solicitacao.numConta;

                            clsApoioReturn.Add(clsApoioRespostaApi);
                        }

                    }

                if (clsApoioUpdateIds.Count > 0)
                    IntegraClass.UpdateMachineId(clsApoioUpdateIds);

                return clsApoioReturn;
            }
            else
            {
                List<int> PedidosEnviados = new List<int>();
                List<ClsApoioUpdateId> clsApoioUpdateIds = new List<ClsApoioUpdateId>();
                List<ClsApoioRespostaApi>? clsApoioReturn = new List<ClsApoioRespostaApi>();

                bool TemPedidoComRetorno = PedidosAbertos.Any(x => x.Retorno == true);

                var SolicitacaoComRota = new AbrirSolicitacao()
                {
                    FormaDePagamento = "D",
                    EmpresaId = Empresa.Infos.Id,
                    Partida = new Partida()
                    {
                        EnderecoPartida = Empresa.Infos.Endereco,
                        BairroPartida = Empresa.Infos.Bairro,
                        ComplementoPartida = null,
                        CidadePartida = Empresa.Infos.Cidade,
                        ReferenciaPartida = null,
                        LatPartida = Empresa.Infos.Latitude,
                        LngPartida = Empresa.Infos.Longitude,
                    },
                    Paradas = new List<Paradas>(),
                    Data = Data,
                    Hora = Hora,
                    Retorno = TemPedidoComRetorno,
                    Antecedencia = 1
                };

                foreach (var pedido in PedidosAbertos)
                {
                    SolicitacaoComRota.Paradas.Add(
                        new Paradas()
                        {
                            EnderecoParada = pedido.DeliveryAddress.FormattedAddress,
                            BairroParada = pedido.DeliveryAddress.Neighborhood,
                            ComplementoParada = pedido.DeliveryAddress.Complement,
                            CidadeParada = Empresa.Infos.Cidade,
                            EstadoParada = "SP",
                            ReferenciaParada = null,
                            IdExterno = pedido.numConta.ToString().PadLeft(4, '0'),
                            NomeClienteParada = pedido.Customer.Name,
                            TelefoneClienteParada = pedido.Customer.Phone,
                        }
                        );
                    PedidosEnviados.Add(pedido.numConta);
                }

                string jsoncontent = JsonConvert.SerializeObject(SolicitacaoComRota);

                if (SolicitacaoComRota is not null)
                {
                    var Response = await ReqConstructor(metodo: "POST", endpoint: "/abrirSolicitacao", content: jsoncontent);

                    string? respostaJson = await Response.Content.ReadAsStringAsync();

                    if (Response.IsSuccessStatusCode)
                    {
                        ClsApoioRespostaApi? clsApoioRespostaApi = JsonConvert.DeserializeObject<ClsApoioRespostaApi>(respostaJson);

                        clsApoioUpdateIds.Add(new ClsApoioUpdateId()
                        {
                            MachineId = clsApoioRespostaApi.Response.Machine_Id,
                            EPedidoAgrupado = true,
                            NumerosDeConta = PedidosEnviados
                        });

                        clsApoioReturn.Add(clsApoioRespostaApi);
                    }
                    else
                    {
                        ClsApoioRespostaApi? clsApoioRespostaApi = JsonConvert.DeserializeObject<ClsApoioRespostaApi>(respostaJson);
                        clsApoioRespostaApi.NumConta = SolicitacaoComRota.numConta;

                        clsApoioReturn.Add(clsApoioRespostaApi);
                    }

                }

                if (clsApoioUpdateIds.Count > 0)
                    IntegraClass.UpdateMachineId(clsApoioUpdateIds);

                return clsApoioReturn;
            }


        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show("Erro Enviar Pedido Automatico!", "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        return null;
    }

    public string? retornaMensagemDeErro(List<ClsApoioRespostaApi>? respostasEnvios)
    {
        string? MenssagemDeErro = "";

        if (respostasEnvios is not null)
        {
            foreach (var resposta in respostasEnvios)
            {
                if (!resposta.Success)
                {
                    MenssagemDeErro += $"Erro ao enviar Pedido {resposta.NumConta}\n" +
                        $"Motivo:";
                    foreach (var erro in resposta.Errors)
                    {
                        MenssagemDeErro += " " + erro.Message + "\n\n";
                    }
                }
            }
        }

        return MenssagemDeErro;
    }

    public async Task<HttpResponseMessage?> ReqConstructor(string? metodo, string endpoint, string? content = null)
    {
        try
        {
            string? url = UrlApi + endpoint;

            string? ApiKey = await GetApiAuthAsync();

            if (metodo == "GET")
            {
                using var requestClient = new HttpClient();
                requestClient.DefaultRequestHeaders.Add("api-key", ApiKey);

                var byteArray = Encoding.ASCII.GetBytes($"{Username}:{Password}");
                requestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                return await requestClient.GetAsync(url);
            }

            if (metodo == "POST")
            {
                using HttpClient requestClient = new HttpClient();
                requestClient.DefaultRequestHeaders.Add("api-key", ApiKey);

                var byteArray = Encoding.ASCII.GetBytes($"{Username}:{Password}");
                requestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                StringContent contentToReq = new StringContent(content, Encoding.UTF8, "application/json");

                return await requestClient.PostAsync(url, contentToReq);
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
        }
        return null;
    }
}
