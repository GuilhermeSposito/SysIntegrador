using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.Logging;
using Newtonsoft.Json;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoCCM;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoDelmatch;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoOnPedido;
using SysIntegradorApp.ClassesAuxiliares.logs;
using SysIntegradorApp.data;
using SysIntegradorApp.data.InterfaceDeContexto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SysIntegradorApp.ClassesDeConexaoComApps;

public class CCM
{
    private readonly IMeuContexto _Db;


    public CCM(IMeuContexto context)
    {
        _Db = context;
    }

    public async Task Pooling()
    {
        string? url = "https://api.ccmpedidoonline.com.br/wsccm_v2.php";
        try
        {
            bool AceitaPedidoAut = false;

            using (ApplicationDbContext db = await _Db.GetContextoAsync())
            {
                AceitaPedidoAut = db.parametrosdosistema.FirstOrDefault().AceitaPedidoAut;
            }

            HttpResponseMessage response = await RequisicaoHttp(url, "GET");

            if (response.IsSuccessStatusCode)
            {
                string? reponseXml = await response.Content.ReadAsStringAsync();

                XmlSerializer serializer = new XmlSerializer(typeof(Pedidos));

                using var streamReaderXML = new StringReader(reponseXml);

                Pedidos Pedidos = (Pedidos)serializer.Deserialize(streamReaderXML);

                if (Pedidos != null)
                {
                    if (Pedidos.PedidoList.Count() > 0)
                    {
                        foreach (var pedido in Pedidos.PedidoList)
                        {
                            await SetPedido(pedido);
                            if (AceitaPedidoAut)
                            {
                                await AceitaPedido(pedido.NroPedido);
                            }
                            await RequisicaoHttp(metodo: "LIMPAPEDIDO", numPedido: pedido.NroPedido);
                        }
                    }
                }

            }

        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Erro pooling CCM");
        }
    }

    public async Task AtualizaStatus(int nmroPedido, string status = "CONFIRMED", bool manualmente = false)
    {
        try
        {
            using (ApplicationDbContext db = await _Db.GetContextoAsync())
            {
                var Pedido = await db.parametrosdopedido.FirstOrDefaultAsync(x => x.PesquisaDisplayId == nmroPedido);

                if (!manualmente)
                {
                    Pedido.Situacao = status;

                    await db.SaveChangesAsync();

                    ClsDeSuporteAtualizarPanel.MudouDataBase = true;
                }
                else
                {
                    var config = await db.parametrosdosistema.FirstOrDefaultAsync();

                    string newStatus = "";

                    switch (status)
                    {
                        case "5":
                            newStatus = "DISPATCHED";
                            break;
                        case "6":
                            newStatus = "CONCLUDED";
                            break;
                        default:
                            newStatus = status;
                            break;
                    }

                    using HttpClient client = new HttpClient();
                    string Newurl = $"http://api.ccmpedidoonline.com.br/wsccm.php?token={config.TokenCCM}&funcao=updateStatus&pedido={nmroPedido}&valor={status}";

                    StringContent contentToPost = new StringContent("", Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(Newurl, contentToPost);

                    if (response.IsSuccessStatusCode)
                    {
                        Pedido.Situacao = newStatus;

                        await db.SaveChangesAsync();

                        switch (status)
                        {
                            case "5":
                                MessageBox.Show("Pedido Despachado", "Pedido Despachado com sucesso!");
                                break;
                            case "6":
                                MessageBox.Show("Pedido concluido", "Pedido concluido com sucesso!");
                                break;
                        }

                        ClsDeSuporteAtualizarPanel.MudouDataBase = true;
                    }

                }
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro ao Atualizar pedido", "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);
            await Logs.CriaLogDeErro(ex.ToString());
        }
    }


    public async Task AceitaPedido(int nmroPedido, string msg = "Seu pedido foi aceito e está em preparo, para mais informações contate-nos")
    {
        try
        {
            await RequisicaoHttp(metodo: "ACEITAPEDIDO", content: msg, numPedido: nmroPedido);

            await this.AtualizaStatus(nmroPedido);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro ao aceitar pedido", "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);
            await Logs.CriaLogDeErro(ex.ToString());
        }
    }

    public async Task RecusaPedido(int nmroPedido, string msg = "Infelizmente não poderemos aceitar seu pedido, para mais informações ligue-nos!")
    {
        try
        {
            await RequisicaoHttp(metodo: "RECUSAPEDIDO", content: msg, numPedido: nmroPedido);

            await this.AtualizaStatus(nmroPedido, status: "CANCELLED");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro ao negar pedido", "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);
            await Logs.CriaLogDeErro(ex.ToString());
        }
    }

    public async Task SetPedido(Pedido pedido)
    {
        try
        {
            using (ApplicationDbContext db = await _Db.GetContextoAsync())
            {
                bool existePedido = await db.parametrosdopedido.AnyAsync(x => x.Id == pedido.NroPedido.ToString());

                if (!existePedido)
                {
                    string HorarioDoEntregarAte = " ";

                    if (pedido.Retira != 1)
                    {

                        HorarioDoEntregarAte = DateTime.Parse(pedido.DataHoraPedido).AddMinutes(db.parametrosdosistema.FirstOrDefault().TempoEntrega).ToString();
                    }

                    if (pedido.Retira == 1)
                    {
                        HorarioDoEntregarAte = DateTime.Parse(pedido.HoraAgendamento).AddMinutes(db.parametrosdosistema.FirstOrDefault().TempoRetirada).ToString();
                    }

                    pedido.EntregarAte = HorarioDoEntregarAte;

                    string jsonContent = JsonConvert.SerializeObject(pedido);

                    await db.parametrosdopedido.AddAsync(new ParametrosDoPedido()
                    {
                        Id = pedido.NroPedido.ToString(),
                        Json = jsonContent,//serializedXml,
                        Situacao = pedido.StatusAcompanhamento,
                        Conta = 0,
                        CriadoEm = DateTimeOffset.Now.ToString(),
                        DisplayId = Convert.ToInt32(pedido.NroPedido),
                        JsonPolling = "Sem Polling ID",
                        CriadoPor = "CCM",
                        PesquisaDisplayId = Convert.ToInt32(pedido.NroPedido)
                    });

                    await db.SaveChangesAsync();

                    ClsDeSuporteAtualizarPanel.MudouDataBase = true;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro ao inserir pedido na base de dados", "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);
            await Logs.CriaLogDeErro(ex.ToString());
        }
    }

    public async Task<IEnumerable<ParametrosDoPedido>> GetPedidos(int? pesquisaID = null)
    {
        IEnumerable<ParametrosDoPedido> pedidos = new List<ParametrosDoPedido>();
        try
        {
            if (pesquisaID != null)
            {
                using (ApplicationDbContext db = await _Db.GetContextoAsync())
                {
                    pedidos = await db.parametrosdopedido.Where(x => x.CriadoPor == "CCM" && x.PesquisaDisplayId == pesquisaID).ToListAsync();
                }
            }
            else
            {
                using (ApplicationDbContext db = await _Db.GetContextoAsync())
                {
                    pedidos = await db.parametrosdopedido.Where(x => x.CriadoPor == "CCM").ToListAsync();
                }

            }


            return pedidos;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro ao buscar pedido na base de dados", "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);
            await Logs.CriaLogDeErro(ex.ToString());

        }
        return pedidos;
    }

    public async Task<Pedido> RetornaPedido(ParametrosDoPedido pedidoParaDeserializar)
    {
        Pedido pedido = new Pedido();
        try
        {
            Pedido? PedidoDeserializado = JsonConvert.DeserializeObject<Pedido>(pedidoParaDeserializar.Json);

            return PedidoDeserializado;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro ao inserir pedido na base de dados", "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);
            await Logs.CriaLogDeErro(ex.ToString());

        }
        return pedido;
    }

    public async Task<PedidoCompleto> CCMPedidoCompleto(Pedido p)
    {
        PedidoCompleto pedidoCompleto = new PedidoCompleto();
        try
        {
            pedidoCompleto.CriadoPor = "CCM"; // Valor fixo de exemplo
            pedidoCompleto.JsonPolling = "{}"; // Valor fixo de exemplo
            pedidoCompleto.id = p.NroPedido.ToString();
            pedidoCompleto.displayId = p.NroPedido.ToString();
            pedidoCompleto.createdAt = p.DataHoraPedido;
            pedidoCompleto.orderTiming = p.Agendamento == 1 ? "SCHEDULED" : "IMMEDIATE"; // Valor fixo de exemplo
            pedidoCompleto.orderType = p.Retira == 1 ? "TAKEOUT" : "DELIVERY";

            string? dataLimite = "";
            string? DeliveryBy = "";

            if (p.Retira != 1)
            {
                using (ApplicationDbContext db = await _Db.GetContextoAsync())
                {

                    DeliveryBy = "MERCHANT";
                    dataLimite = DateTime.Parse(p.DataHoraPedido).AddMinutes(db.parametrosdosistema.FirstOrDefault().TempoEntrega).ToString();

                }
            }

            if (p.Retira == 1)
            {
                using (ApplicationDbContext db = await _Db.GetContextoAsync())
                {
                    DeliveryBy = "RETIRADA";
                    dataLimite = DateTime.Parse(p.HoraAgendamento).AddMinutes(db.parametrosdosistema.FirstOrDefault().TempoRetirada).ToString();
                }
            }

            //if (p.Return.Type == "INDOOR")
            // {
            //    DeliveryBy = "MESA";
            //  dataLimite = p.Return.Indoor.IndoorDateTime;
            // }

            pedidoCompleto.delivery.deliveredBy = DeliveryBy;
            pedidoCompleto.delivery.deliveryDateTime = dataLimite;
            pedidoCompleto.customer.id = p.Cliente.Codigo.ToString();
            pedidoCompleto.customer.name = p.Cliente.Nome;
            pedidoCompleto.customer.documentNumber = p.Cliente.Telefone;
            pedidoCompleto.salesChannel = "CCM"; // Valor fixo de exemplo

            return pedidoCompleto;

        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro ao inserir pedido na base de dados", "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);
            await Logs.CriaLogDeErro(ex.ToString());

        }
        return pedidoCompleto;
    }



    public async Task<HttpResponseMessage> RequisicaoHttp(string? url = null, string? metodo = null, string content = "", int numPedido = 0)
    {
        HttpResponseMessage response = new HttpResponseMessage();
        try
        {
            string TokenCCM = "";

            using (ApplicationDbContext db = await _Db.GetContextoAsync())
            {
                var Config = db.parametrosdosistema.FirstOrDefault();
                TokenCCM = Config.TokenCCM;
            }

            if (metodo == "GET")
            {
                using HttpClient client = new HttpClient();
                string Newurl = url += $"?token={TokenCCM}";

                response = await client.GetAsync(Newurl);

                return response;
            }

            if (metodo == "POST")
            {
                using HttpClient client = new HttpClient();
                string Newurl = url += $"?token={TokenCCM}";

                StringContent contentToPost = new StringContent(content, Encoding.UTF8, "application/json");

                response = await client.PostAsync(Newurl, contentToPost);

                return response;
            }

            if (metodo == "LIMPAPEDIDO")
            {
                using HttpClient client = new HttpClient();
                string Newurl = $"http://api.ccmpedidoonline.com.br/wsccm_v2.php?token={TokenCCM}&import={numPedido}";

                StringContent contentToPost = new StringContent(content, Encoding.UTF8, "application/json");

                response = await client.PostAsync(Newurl, contentToPost);

                return response;
            }

            if (metodo == "ACEITAPEDIDO")
            {
                using HttpClient client = new HttpClient();
                string Newurl = $"http://api.ccmpedidoonline.com.br/wsccm.php?token={TokenCCM}&funcao=aceitarPedido&pedido={numPedido}&msg={content}";

                StringContent contentToPost = new StringContent(content, Encoding.UTF8, "application/json");

                response = await client.PostAsync(Newurl, contentToPost);

                return response;
            }

            if (metodo == "RECUSAPEDIDO")
            {
                using HttpClient client = new HttpClient();
                string Newurl = $"http://api.ccmpedidoonline.com.br/wsccm.php?token={TokenCCM}&funcao=recusarPedido&pedido={numPedido}&msg={content}";

                StringContent contentToPost = new StringContent(content, Encoding.UTF8, "application/json");

                response = await client.PostAsync(Newurl, contentToPost);

                return response;
            }

        }//HAZZRWXX5GWYBNQ1BZXBQNK8WP5P6CQT
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Erro ao enviar Req CCM");
        }
        return response;
    }

}
