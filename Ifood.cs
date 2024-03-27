﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.EntityFrameworkCore;
using SysIntegradorApp.ClassesAuxiliares;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace SysIntegradorApp;

internal class Ifood
{
    private static System.Timers.Timer aTimer;

    public static async Task Pulling() //pulling feito de 30 em 30 Segundos, Caso seja encontrado algum novo pedido ele chama o GetPedidos
    {
        string url = @"https://merchant-api.ifood.com.br/order/v1.0/events";
        // Token.TokenDaSessao = "eyJraWQiOiJlZGI4NWY2Mi00ZWY5LTExZTktODY0Ny1kNjYzYmQ4NzNkOTMiLCJ0eXAiOiJKV1QiLCJhbGciOiJSUzUxMiJ9.eyJzdWIiOiJiZDg2MmYwNy0zYTgxLTRkZTYtYWM5Ni05NzJiNjZhNDljZTciLCJvd25lcl9uYW1lIjoiZ3VpbGhlcm1ldGVzdGVzIiwiaXNzIjoiaUZvb2QiLCJjbGllbnRfaWQiOiJjYzQ0Y2Q2MS1jYmI3LTQ0MjQtOTE5Yi1hM2RmNDI4N2FlYzEiLCJhcHBfbmFtZSI6Imd1aWxoZXJtZXRlc3Rlcy10ZXN0ZS1kIiwiYXVkIjpbInNoaXBwaW5nIiwiY2F0YWxvZyIsInJldmlldyIsImZpbmFuY2lhbCIsIm1lcmNoYW50IiwibG9naXN0aWNzIiwiZ3JvY2VyaWVzIiwiZXZlbnRzIiwib3JkZXIiLCJvYXV0aC1zZXJ2ZXIiXSwic2NvcGUiOlsic2hpcHBpbmciLCJjYXRhbG9nIiwicmV2aWV3IiwibWVyY2hhbnQiLCJsb2dpc3RpY3MiLCJncm9jZXJpZXMiLCJldmVudHMiLCJvcmRlciIsImNvbmNpbGlhdG9yIl0sInR2ZXIiOiJ2MiIsIm1lcmNoYW50X3Njb3BlIjpbIjkzNjIwMThhLTZhZTItNDM5Yy05NjhiLWE0MDE3N2EwODVlYTptZXJjaGFudCIsIjkzNjIwMThhLTZhZTItNDM5Yy05NjhiLWE0MDE3N2EwODVlYTpvcmRlciIsIjkzNjIwMThhLTZhZTItNDM5Yy05NjhiLWE0MDE3N2EwODVlYTpjYXRhbG9nIiwiOTM2MjAxOGEtNmFlMi00MzljLTk2OGItYTQwMTc3YTA4NWVhOmNvbmNpbGlhdG9yIiwiOTM2MjAxOGEtNmFlMi00MzljLTk2OGItYTQwMTc3YTA4NWVhOnJldmlldyIsIjkzNjIwMThhLTZhZTItNDM5Yy05NjhiLWE0MDE3N2EwODVlYTpsb2dpc3RpY3MiLCI5MzYyMDE4YS02YWUyLTQzOWMtOTY4Yi1hNDAxNzdhMDg1ZWE6c2hpcHBpbmciLCI5MzYyMDE4YS02YWUyLTQzOWMtOTY4Yi1hNDAxNzdhMDg1ZWE6Z3JvY2VyaWVzIiwiOTM2MjAxOGEtNmFlMi00MzljLTk2OGItYTQwMTc3YTA4NWVhOmV2ZW50cyJdLCJleHAiOjE3MTEwNzAzNzEsImlhdCI6MTcxMTA0ODc3MSwianRpIjoiYmQ4NjJmMDctM2E4MS00ZGU2LWFjOTYtOTcyYjY2YTQ5Y2U3OmNjNDRjZDYxLWNiYjctNDQyNC05MTliLWEzZGY0Mjg3YWVjMSIsIm1lcmNoYW50X3Njb3BlZCI6dHJ1ZX0.K6i31FGzFFaJmc2nxCN5u3s9pNGaBr_SfsAkQpBj_zY4Ve7BQ_oPX-j5p80rszThN0fw-VPm-teQFwN5T0E7X3itab2cOklALgxHjy0Um5DP0xcV1IW4ywj6E49rtlkAVBUe9KNa2AiVe-zV2gaMZE7x9N9PzBzQJiqZyeaF50I";
        try
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token.TokenDaSessao);
            HttpResponseMessage reponse = await client.GetAsync($"{url}:polling");

            int statusCode = (int)reponse.StatusCode;
            if (statusCode == 200)
            {

                string jsonContent = await reponse.Content.ReadAsStringAsync();
                List<Pedido>? pedidos = JsonSerializer.Deserialize<List<Pedido>>(jsonContent);



                using (var dbContex = new ApplicationDbContext())
                {
                    var pullingsNaBase = dbContex.pulling.ToList();
                    foreach (var pullingAtual in pedidos)
                    {
                        var confereSeJaExiste = pullingsNaBase.Any((p) => p.id.Contains(pullingAtual.id));

                        await Console.Out.WriteLineAsync(pullingAtual.fullCode);

                        if (!confereSeJaExiste) //só entra aqui caso o pulling não existir
                        {
                            dbContex.pulling.Add(new Pulling() { id = pullingAtual.id });
                            dbContex.SaveChanges();
                            await SetPedido(pullingAtual.orderId, pullingAtual.fullCode);
                        }

                        var order = dbContex.pedidocompleto.Where(p => p.id == pullingAtual.orderId).FirstOrDefault();

                        if (order != null)
                        {
                            order.StatusCode = pullingAtual.fullCode;
                            dbContex.SaveChanges();
                        }

                    }

                    var pulingsToJson = JsonSerializer.Serialize(pullingsNaBase);
                    StringContent content = new StringContent(pulingsToJson, Encoding.UTF8, "application/json");

                    await client.PostAsync($"{url}/acknowledgment", content);

                    FormMenuInicial.panelPedidos.Invoke(new Action(async () => await Ifood.GetPedido()));

                }

            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message, ex.StackTrace);
        }

    }

    //Função que Insere o pediddo que vem no pulling no banco de dados
    public static async Task SetPedido(string orderId, string statusCode = "PLACED")
    {
        // Token.TokenDaSessao = "eyJraWQiOiJlZGI4NWY2Mi00ZWY5LTExZTktODY0Ny1kNjYzYmQ4NzNkOTMiLCJ0eXAiOiJKV1QiLCJhbGciOiJSUzUxMiJ9.eyJzdWIiOiJiZDg2MmYwNy0zYTgxLTRkZTYtYWM5Ni05NzJiNjZhNDljZTciLCJvd25lcl9uYW1lIjoiZ3VpbGhlcm1ldGVzdGVzIiwiaXNzIjoiaUZvb2QiLCJjbGllbnRfaWQiOiJjYzQ0Y2Q2MS1jYmI3LTQ0MjQtOTE5Yi1hM2RmNDI4N2FlYzEiLCJhcHBfbmFtZSI6Imd1aWxoZXJtZXRlc3Rlcy10ZXN0ZS1kIiwiYXVkIjpbInNoaXBwaW5nIiwiY2F0YWxvZyIsInJldmlldyIsImZpbmFuY2lhbCIsIm1lcmNoYW50IiwibG9naXN0aWNzIiwiZ3JvY2VyaWVzIiwiZXZlbnRzIiwib3JkZXIiLCJvYXV0aC1zZXJ2ZXIiXSwic2NvcGUiOlsic2hpcHBpbmciLCJjYXRhbG9nIiwicmV2aWV3IiwibWVyY2hhbnQiLCJsb2dpc3RpY3MiLCJncm9jZXJpZXMiLCJldmVudHMiLCJvcmRlciIsImNvbmNpbGlhdG9yIl0sInR2ZXIiOiJ2MiIsIm1lcmNoYW50X3Njb3BlIjpbIjkzNjIwMThhLTZhZTItNDM5Yy05NjhiLWE0MDE3N2EwODVlYTptZXJjaGFudCIsIjkzNjIwMThhLTZhZTItNDM5Yy05NjhiLWE0MDE3N2EwODVlYTpvcmRlciIsIjkzNjIwMThhLTZhZTItNDM5Yy05NjhiLWE0MDE3N2EwODVlYTpjYXRhbG9nIiwiOTM2MjAxOGEtNmFlMi00MzljLTk2OGItYTQwMTc3YTA4NWVhOmNvbmNpbGlhdG9yIiwiOTM2MjAxOGEtNmFlMi00MzljLTk2OGItYTQwMTc3YTA4NWVhOnJldmlldyIsIjkzNjIwMThhLTZhZTItNDM5Yy05NjhiLWE0MDE3N2EwODVlYTpsb2dpc3RpY3MiLCI5MzYyMDE4YS02YWUyLTQzOWMtOTY4Yi1hNDAxNzdhMDg1ZWE6c2hpcHBpbmciLCI5MzYyMDE4YS02YWUyLTQzOWMtOTY4Yi1hNDAxNzdhMDg1ZWE6Z3JvY2VyaWVzIiwiOTM2MjAxOGEtNmFlMi00MzljLTk2OGItYTQwMTc3YTA4NWVhOmV2ZW50cyJdLCJleHAiOjE3MTA5NTk3NDIsImlhdCI6MTcxMDkzODE0MiwianRpIjoiYmQ4NjJmMDctM2E4MS00ZGU2LWFjOTYtOTcyYjY2YTQ5Y2U3OmNjNDRjZDYxLWNiYjctNDQyNC05MTliLWEzZGY0Mjg3YWVjMSIsIm1lcmNoYW50X3Njb3BlZCI6dHJ1ZX0.NO1_-hgj4h4XeN8bZXIWBKztACHnJZzWYnuvDClluXzjYE6b7sm7wwzbMow7wOHRHgkGjRkHduiUVNAFB7-yULunwX350PLRiIGxuBf_cFUyK1_xvO_M14p59s4yGkntobm6pj57ZH1MxPnbxTw4Rgftqc7eQCW54cfhdebbO7s";
        string url = $"https://merchant-api.ifood.com.br/order/v1.0/orders/{orderId}";
        try
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token.TokenDaSessao);
            HttpResponseMessage response = await client.GetAsync(url);

            if (Convert.ToInt32(response.StatusCode) == 404)
            {
                throw new HttpRequestException("Pedido Não Encontrado");
            }


            string leituraDoPedido = await response.Content.ReadAsStringAsync();
            PedidoCompleto? pedidoCompletoTotal = JsonSerializer.Deserialize<PedidoCompleto>(leituraDoPedido);

            pedidocompleto? pedidocompletoDB = JsonSerializer.Deserialize<pedidocompleto>(leituraDoPedido);

            //setar o id_pedido de cada objeto relacionado para inserção no banco
            //fazer o insert no banco de dados separando todo o pedido em tabelas relacionadas

            using (var db = new ApplicationDbContext())
            {
                //primeiro insere na coluna pedidocompleto (primeiro verifica se o pedido já existe)
                if (db.pedidocompleto.Find(pedidocompletoDB.id) != null)
                {
                    throw new Exception("Pedido já encontrado no banco de dados");
                }

                //caso exista vai ser inserido o pedido no banco de dados
                pedidocompletoDB.StatusCode = statusCode;
                string jsonContent = JsonSerializer.Serialize(pedidocompletoDB);
                db.pedidocompleto.Add(pedidocompletoDB);
                db.SaveChanges();

                //segundo insere na coluna delivery relacionando com o id do pedido 
                pedidoCompletoTotal.delivery.id_pedido = pedidoCompletoTotal.id;
                db.delivery.Add(pedidoCompletoTotal.delivery);
                db.SaveChanges();

                //terceito insere na coluna deliveryaddress relacionando com o delivery
                pedidoCompletoTotal.delivery.deliveryAddress.id_pedido = pedidocompletoDB.id;
                pedidoCompletoTotal.delivery.deliveryAddress.id_delivery = pedidoCompletoTotal.delivery.id;
                db.deliveryaddress.Add(pedidoCompletoTotal.delivery.deliveryAddress);
                db.SaveChanges();

                //quarto insere na coluna coordinates relacionando com o deliveryaddress 
                pedidoCompletoTotal.delivery.deliveryAddress.coordinates.id_DeliveryAddress = pedidoCompletoTotal.delivery.deliveryAddress.id;
                db.coordinates.Add(pedidoCompletoTotal.delivery.deliveryAddress.coordinates);
                db.SaveChanges();

                //quinto insere na tabela merchant 
                pedidoCompletoTotal.merchant.id_pedido = pedidoCompletoTotal.id;
                db.merchant.Add(pedidoCompletoTotal.merchant);
                db.SaveChanges();

                //sexto faz a inserção na tabela Customer relacionando com o id do pedido (Porém verifica se já existe antes) (Só vai ser inserido o phone também se já não existir o customer)
                pedidoCompletoTotal.customer.id_pedido = pedidoCompletoTotal.id;
                db.customer.Add(pedidoCompletoTotal.customer);
                db.SaveChanges();

                //setimo faz a inserção na tabela phone relacionando com a coluna id_db da tabela customer 
                pedidoCompletoTotal.customer.phone.id_customer_pedido = pedidoCompletoTotal.customer.id_pedido;
                pedidoCompletoTotal.customer.phone.id_pedido = pedidocompletoDB.id;
                db.phone.Add(pedidoCompletoTotal.customer.phone);
                db.SaveChanges();


                //oitavo insere um array de itens fazerndo um loop para uma inserção de cada vez
                foreach (var item in pedidoCompletoTotal.items)
                {
                    item.id_pedido = pedidocompletoDB.id;
                    db.items.Add(item);
                    db.SaveChanges();
                }

                //nono insere na tabela total relacionando o id do pedido com a coluna id_pedido da tabela total
                pedidoCompletoTotal.total.id_pedido = pedidocompletoDB.id;
                db.total.Add(pedidoCompletoTotal.total);
                db.SaveChanges();

                //decimo insere na tabela Payments relacionando a coluna id_pedido com a tabela pedidototal
                pedidoCompletoTotal.payments.id_pedido = pedidocompletoDB.id;
                db.payments.Add(pedidoCompletoTotal.payments);
                db.SaveChanges();

                //decimo primeiro faz um for e insere na tabela methods relacionando as colunas payments_id com o id do paymant e id_pedido relacionando com o id da coluna pedidocompleto 
                foreach (var method in pedidoCompletoTotal.payments.methods)
                {

                    method.id_pedido = pedidocompletoDB.id;
                    method.payments_id = pedidoCompletoTotal.payments.id;
                    db.methods.Add(method);
                    db.SaveChanges();

                }

                //decimo segundo insere na tabela additionalinfo para depois poder relacionar a tabela metadata com a additionalinfo
                pedidoCompletoTotal.additionalInfo.id_pedido = pedidocompletoDB.id;
                db.additionalinfo.Add(pedidoCompletoTotal.additionalInfo);
                db.SaveChanges();

                //decimo terceiro insere na tabela metadata relacionando com a tabela id do pedidototal e id da addicionalinfo 
                pedidoCompletoTotal.additionalInfo.metadata.id_pedido = pedidocompletoDB.id;
                pedidoCompletoTotal.additionalInfo.metadata.id_additionalinfo = pedidoCompletoTotal.additionalInfo.id;
                db.metadata.Add(pedidoCompletoTotal.additionalInfo.metadata);
                db.SaveChanges();

                Console.ForegroundColor = ConsoleColor.Green;
                await Console.Out.WriteLineAsync("Pedido inserido na base de dados");
                Console.ForegroundColor = ConsoleColor.White;

            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    //função que está mostrando os pedidos no panel
    public static async Task<List<PedidoParaOFront>> GetPedido(/*string pedido_id*/)
    {
        List<PedidoParaOFront> pedidoCompleto = new List<PedidoParaOFront>();
        string path = @"C:\Users\gui-c\OneDrive\Área de Trabalho\primeiro\testeSeriliazeJson.json";
        try
        {
            using ApplicationDbContext db = new ApplicationDbContext();



                var resultado = from a in db.pedidocompleto // aqui faz o select no pedidocompleto
                                join b in db.items on a.id equals b.id_pedido // aqui faz o select no items 
                                join c in db.payments on a.id equals c.id_pedido // aqui faz o select no payment
                                join d in db.methods on c.id equals d.payments_id // aqui faz o select no methods
                                join e in db.total on a.id equals e.id_pedido  // aqui faz o selectno total
                                join f in db.delivery on a.id equals f.id_pedido // aqui faz o select no delivery
                                join g in db.deliveryaddress on a.id equals g.id_pedido // aqui faz o select no deliveryAdress
                                join h in db.coordinates on g.id equals h.id_DeliveryAddress // aqui faz o select no cordinates relacionando com o deliveryadress
                                join i in db.customer on a.id equals i.id_pedido //aqui faz o select na tabela customer (Cliente)
                                join j in db.phone on a.id equals j.id_pedido
                                group new { a, b, c, d, e, f, g, h, i, j } by a into grupo
                                select new
                                {
                                    PedidoInfos = grupo.Key,
                                    Items = grupo.Select(x => x.b).ToList(),
                                    Delivery = new
                                    {
                                        mode = grupo.Select(p => p.f.mode).FirstOrDefault(),
                                        deliveryBy = grupo.Select(p => p.f.deliveredBy).FirstOrDefault(),
                                        deliveryDateTime = grupo.Select(p => p.f.deliveryDateTime).FirstOrDefault(),
                                        observations = grupo.Select(p => p.f.observations).FirstOrDefault(),
                                        pickupCode = grupo.Select(p => p.f.pickupCode).FirstOrDefault(),
                                        deliveryAddress = new
                                        {
                                            streetName = grupo.Select(p => p.g.streetName).FirstOrDefault(),
                                            streetNumber = grupo.Select(p => p.g.streetNumber).FirstOrDefault(),
                                            formattedAddress = grupo.Select(p => p.g.formattedAddress).FirstOrDefault(),
                                            neighborhood = grupo.Select(p => p.g.neighborhood).FirstOrDefault(),
                                            complement = grupo.Select(p => p.g.complement).FirstOrDefault(),
                                            postalCode = grupo.Select(p => p.g.postalCode).FirstOrDefault(),
                                            city = grupo.Select(p => p.g.city).FirstOrDefault(),
                                            state = "SP",
                                            country = "BRASIL",
                                            coordinates = new
                                            {
                                                latitude = grupo.Select(p => p.h.latitude).FirstOrDefault(),
                                                longitude = grupo.Select(p => p.h.longitude).FirstOrDefault(),
                                            }
                                        },

                                    },
                                    customer = new
                                    {
                                        id = grupo.Select(p => p.i.id).FirstOrDefault(),
                                        name = grupo.Select(p => p.i.name).FirstOrDefault(),
                                        documentNumber = grupo.Select(p => p.i.documentNumber).FirstOrDefault(),
                                        phone = new
                                        {
                                            number = grupo.Select(p => p.j.number).FirstOrDefault(),
                                            localizer = grupo.Select(p => p.j.localizer).FirstOrDefault(),
                                            localizerExpiration = grupo.Select(p => p.j.localizerExpiration).FirstOrDefault()
                                        }

                                    },
                                    Payments = new
                                    {
                                        IdPedido = grupo.Select(p => p.c.id_pedido).FirstOrDefault(),
                                        Prepaid = grupo.Select(p => p.c.prepaid).FirstOrDefault(),
                                        Pending = grupo.Select(p => p.c.pending).FirstOrDefault(),
                                        Methods = grupo.Select(x => x.d).Take(1).ToList(),
                                    },
                                    Total = grupo.Select(p => p.e).FirstOrDefault()


                                };

            var resultadoList = await resultado.ToListAsync();
            string pedidoSerializado = JsonSerializer.Serialize(resultado);

            List<PedidoParaOFront>? pedidoDeserializado = JsonSerializer.Deserialize<List<PedidoParaOFront>>(pedidoSerializado);
            foreach (PedidoParaOFront item in pedidoDeserializado)
            {
                pedidoCompleto.Add(item);

            }

            return pedidoCompleto; // Retorne a lista deserializada ou uma lista vazia se for nula
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            await Console.Out.WriteLineAsync(ex.Message);
        }

        return pedidoCompleto;

    }

    public static async void SetTimer()//set timer pra fazer o acionamento a cada 30 segundos do pulling
    {

        aTimer = new System.Timers.Timer(30000);
        aTimer.Elapsed += OnTimedEvent;
        aTimer.AutoReset = true;
        aTimer.Enabled = true;

        await Console.Out.WriteLineAsync();
    }

    private static async void OnTimedEvent(System.Object source, ElapsedEventArgs e)
    {
        await Pulling();
        // Função para corrigir a diferença de thread 
        
    }
}
