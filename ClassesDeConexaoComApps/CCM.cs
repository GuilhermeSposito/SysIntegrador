using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.Logging;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoCCM;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoDelmatch;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoOnPedido;
using SysIntegradorApp.ClassesAuxiliares.logs;
using SysIntegradorApp.data;
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
    private readonly ApplicationDbContext _Db;


    public CCM(ApplicationDbContext context)
    {
        _Db = context;
    }

    public async Task Pooling()
    {
        try
        {
            string? url = "https://api.ccmpedidoonline.com.br/wsccm_v2.php";

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
                            if (_Db.parametrosdosistema.FirstOrDefault().AceitaPedidoAut)
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

    public async Task AtualizaStatus(int nmroPedido, string status = "CONFIRMED")
    {
        try
        {
            var Pedido = await _Db.parametrosdopedido.FirstOrDefaultAsync(x => x.PesquisaDisplayId == nmroPedido);

            Pedido.Situacao = status;

            await _Db.SaveChangesAsync();

           // SetarPanelPedidos();
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

    public async Task SetPedido(Pedido pedido)
    {
        try
        {
            bool existePedido = _Db.parametrosdopedido.Any(x => x.Id == pedido.NroPedido.ToString());

            if (!existePedido)
            {
                string HorarioDoEntregarAte = " ";

                if (pedido.Retira != 1)
                {

                    HorarioDoEntregarAte = DateTime.Parse(pedido.DataHoraPedido).AddMinutes(_Db.parametrosdosistema.FirstOrDefault().TempoEntrega).ToString();
                }

                if (pedido.Retira == 1)
                {
                    HorarioDoEntregarAte = DateTime.Parse(pedido.HoraAgendamento).AddMinutes(_Db.parametrosdosistema.FirstOrDefault().TempoRetirada).ToString();
                }

                pedido.EntregarAte = HorarioDoEntregarAte;

                using var stringWriter = new StringWriter();
                XmlSerializer serializer = new XmlSerializer(typeof(Pedido));
                serializer.Serialize(stringWriter, pedido);

                string serializedXml = stringWriter.ToString();

                await _Db.parametrosdopedido.AddAsync(new ParametrosDoPedido()
                {
                    Id = pedido.NroPedido.ToString(),
                    Json = serializedXml,
                    Situacao = pedido.StatusAcompanhamento,
                    Conta = 0,
                    CriadoEm = DateTimeOffset.Now.ToString(),
                    DisplayId = Convert.ToInt32(pedido.NroPedido),
                    JsonPolling = "Sem Polling ID",
                    CriadoPor = "CCM",
                    PesquisaDisplayId = Convert.ToInt32(pedido.NroPedido)
                });

                await _Db.SaveChangesAsync();
                FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.SetarPanelPedidos()));
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
               pedidos =  await _Db.parametrosdopedido.Where(x => x.CriadoPor == "CCM" && x.PesquisaDisplayId == pesquisaID).ToListAsync();
            }
            else
            {
               pedidos = await _Db.parametrosdopedido.Where(x => x.CriadoPor == "CCM").ToListAsync();

            }


            return pedidos;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro ao inserir pedido na base de dados", "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);
            await Logs.CriaLogDeErro(ex.ToString());

        }
        return pedidos;
    }

    public async Task<Pedido> RetornaPedido(ParametrosDoPedido pedidoParaDeserializar)
    {
        Pedido pedido = new Pedido();
        try
        {
            using var stringReader = new StringReader(pedidoParaDeserializar.Json);
            var xmlReader = new ClsSuporteDeserializacaoXml(stringReader);

            XmlSerializer serializer = new XmlSerializer(typeof(Pedido));
            Pedido? PedidoDeserializado = (Pedido)serializer.Deserialize(xmlReader);



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
                DeliveryBy = "MERCHANT";
                dataLimite = DateTime.Parse(p.DataHoraPedido).AddMinutes(_Db.parametrosdosistema.FirstOrDefault().TempoEntrega).ToString();
            }

            if (p.Retira == 1)
            {
                DeliveryBy = "RETIRADA";
                dataLimite = DateTime.Parse(p.HoraAgendamento).AddMinutes(_Db.parametrosdosistema.FirstOrDefault().TempoRetirada).ToString();
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
            if (metodo == "GET")
            {
                using HttpClient client = new HttpClient();
                string Newurl = url += "?token=HAZZRWXX5GWYBNQ1BZXBQNK8WP5P6CQT";

                response = await client.GetAsync(Newurl);

                return response;
            }

            if (metodo == "POST")
            {
                using HttpClient client = new HttpClient();
                string Newurl = url += "?token=HAZZRWXX5GWYBNQ1BZXBQNK8WP5P6CQT";

                StringContent contentToPost = new StringContent(content, Encoding.UTF8, "application/json");

                response = await client.PostAsync(Newurl, contentToPost);

                return response;
            }

            if (metodo == "LIMPAPEDIDO")
            {
                using HttpClient client = new HttpClient();
                string Newurl = $"http://api.ccmpedidoonline.com.br/wsccm_v2.php?token=HAZZRWXX5GWYBNQ1BZXBQNK8WP5P6CQT&import={numPedido}";

                StringContent contentToPost = new StringContent(content, Encoding.UTF8, "application/json");

                response = await client.PostAsync(Newurl, contentToPost);

                return response;
            }

            if (metodo == "ACEITAPEDIDO")
            {
                using HttpClient client = new HttpClient();
                string Newurl = $"http://api.ccmpedidoonline.com.br/wsccm.php?token=HAZZRWXX5GWYBNQ1BZXBQNK8WP5P6CQT&funcao=aceitarPedido&pedido={numPedido}&msg={content}";

                StringContent contentToPost = new StringContent(content, Encoding.UTF8, "application/json");

                response = await client.PostAsync(Newurl, contentToPost);

                return response;
            }

            if (metodo == "RECUSAPEDIDO")
            {
                using HttpClient client = new HttpClient();
                string Newurl = $"http://api.ccmpedidoonline.com.br/wsccm.php?token=HAZZRWXX5GWYBNQ1BZXBQNK8WP5P6CQT&funcao=recusarPedido&pedido={numPedido}&msg={content}";

                StringContent contentToPost = new StringContent(content, Encoding.UTF8, "application/json");

                response = await client.PostAsync(Newurl, contentToPost);

                return response;
            }

        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Erro ao enviar Req CCM");
        }
        return response;
    }

}
