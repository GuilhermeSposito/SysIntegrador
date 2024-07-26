using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoTaxyMachine;

public class AbrirSolicitacao
{
    [JsonIgnore] public int numConta { get; set; }
    [JsonProperty("forma_pagamento")] public string? FormaDePagamento { get; set; }
    [JsonProperty("empresa_id")] public string? EmpresaId { get; set; }
    [JsonProperty("condutor")] public Condutor Condutor { get; set; } = new Condutor();
    [JsonProperty("categoria_id")] public string? CategoriaId { get; set; }
    [JsonProperty("categoria_nome")] public string? CategoriaNome { get; set; }
    [JsonProperty("partida")] public Partida Partida { get; set; } = new Partida();
    [JsonProperty("paradas")] public List<Paradas> Paradas { get; set; } = new List<Paradas>();
    [JsonProperty("retorno")] public bool Retorno { get; set; }
    [JsonProperty("data")] public string? Data { get; set; }
    [JsonProperty("hora")] public string? Hora { get; set; }
    [JsonProperty("antecedencia")] public int Antecedencia { get; set; }

}

public class Condutor
{
    [JsonProperty("tipo_identificacao")] public string? EmpresaId { get; set; }
    [JsonProperty("identificacao")] public int Identificacao { get; set; }

}

public class Partida
{
    [JsonProperty("endereco")] public string? EnderecoPartida { get; set; }
    [JsonProperty("bairro")] public string? BairroPartida { get; set; }
    [JsonProperty("complemento")] public string? ComplementoPartida { get; set; }
    [JsonProperty("cidade")] public string? CidadePartida { get; set; }
    [JsonProperty("estado")] public string? EstadoPartida { get; set; }
    [JsonProperty("referencia")] public string? ReferenciaPartida { get; set; }
    [JsonProperty("lat")] public string? LatPartida { get; set; }
    [JsonProperty("lng")] public string? LngPartida { get; set; }

}

public class Paradas
{
    [JsonProperty("endereco_parada")] public string? EnderecoParada { get; set; }
    [JsonProperty("bairro_parada")] public string? BairroParada { get; set; }
    [JsonProperty("complemento_parada")] public string? ComplementoParada { get; set; }
    [JsonProperty("cidade_parada")] public string? CidadeParada { get; set; }
    [JsonProperty("estado_parada")] public string? EstadoParada { get; set; }
    [JsonProperty("referencia_parada")] public string? ReferenciaParada { get; set; }
    [JsonProperty("lat_parada")] public string? LatParada { get; set; }
    [JsonProperty("lng_parada")] public string? LngParada { get; set; }
    [JsonProperty("id_externo")] public string? IdExterno { get; set; }
    [JsonProperty("observacao_parada")] public string? ObservacoesParada { get; set; }
    [JsonProperty("nome_cliente_parada")] public string? NomeClienteParada { get; set; }
    [JsonProperty("telefone_cliente_parada")] public string? TelefoneClienteParada { get; set; }

}