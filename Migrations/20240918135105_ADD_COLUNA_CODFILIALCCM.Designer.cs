﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SysIntegradorApp.data;

#nullable disable

namespace SysIntegradorApp.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240918135105_ADD_COLUNA_CODFILIALCCM")]
    partial class ADD_COLUNA_CODFILIALCCM
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SysIntegradorApp.ClassesAuxiliares.ApoioOnPedido", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Action")
                        .HasColumnType("text")
                        .HasColumnName("action");

                    b.Property<int>("Id_Pedido")
                        .HasColumnType("integer")
                        .HasColumnName("id_pedido");

                    b.HasKey("Id");

                    b.ToTable("apoioonpedido");
                });

            modelBuilder.Entity("SysIntegradorApp.ClassesAuxiliares.ParametrosDoPedido", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<int>("Conta")
                        .HasColumnType("integer")
                        .HasColumnName("conta");

                    b.Property<string>("CriadoEm")
                        .HasColumnType("text")
                        .HasColumnName("criadoem");

                    b.Property<string>("CriadoPor")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("criadopor");

                    b.Property<int>("DisplayId")
                        .HasColumnType("integer")
                        .HasColumnName("displayid");

                    b.Property<string>("Json")
                        .HasColumnType("text")
                        .HasColumnName("json");

                    b.Property<string>("JsonPolling")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("jsonpolling");

                    b.Property<int>("PesquisaDisplayId")
                        .HasColumnType("integer")
                        .HasColumnName("pesquisadisplayid");

                    b.Property<string>("PesquisaNome")
                        .HasColumnType("text")
                        .HasColumnName("pesquisanome");

                    b.Property<string>("Situacao")
                        .HasColumnType("text")
                        .HasColumnName("situacao");

                    b.HasKey("Id");

                    b.ToTable("parametrosdopedido");
                });

            modelBuilder.Entity("SysIntegradorApp.ClassesAuxiliares.ParametrosDoSistema", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("AceitaPedidoAut")
                        .HasColumnType("boolean")
                        .HasColumnName("aceitapedidoaut");

                    b.Property<bool>("AgruparComandas")
                        .HasColumnType("boolean")
                        .HasColumnName("agruparcomandas");

                    b.Property<string>("ApiKeyTaxyMachine")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("apikeytaxymachine");

                    b.Property<string>("CaminhoServidor")
                        .HasColumnType("text")
                        .HasColumnName("caminhoservidor");

                    b.Property<string>("CaminhodoBanco")
                        .HasColumnType("text")
                        .HasColumnName("caminhodobanco");

                    b.Property<string>("CardapioUsando")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("cardapiousando");

                    b.Property<string>("Cidade")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("cidade");

                    b.Property<string>("ClientId")
                        .HasColumnType("text")
                        .HasColumnName("clientid");

                    b.Property<string>("ClientSecret")
                        .HasColumnType("text")
                        .HasColumnName("clientsecret");

                    b.Property<string>("CodFilialCCM")
                        .HasColumnType("text")
                        .HasColumnName("codfilialccm");

                    b.Property<bool>("ComandaReduzida")
                        .HasColumnType("boolean")
                        .HasColumnName("comandareduzida");

                    b.Property<string>("DelMatchId")
                        .HasColumnType("text")
                        .HasColumnName("delmatchid");

                    b.Property<bool>("DestacarObs")
                        .HasColumnType("boolean")
                        .HasColumnName("destacarobs");

                    b.Property<string>("DtUltimaVerif")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("dtultimaverif");

                    b.Property<string>("EmpresadeEntrega")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("empresadeentrega");

                    b.Property<string>("Endereco")
                        .HasColumnType("text")
                        .HasColumnName("endereco");

                    b.Property<bool>("EnviaPedidoAut")
                        .HasColumnType("boolean")
                        .HasColumnName("enviapedidoaut");

                    b.Property<bool>("ImpCompacta")
                        .HasColumnType("boolean")
                        .HasColumnName("impcompacta");

                    b.Property<bool>("ImpressaoAut")
                        .HasColumnType("boolean")
                        .HasColumnName("impressaoaut");

                    b.Property<string>("Impressora1")
                        .HasColumnType("text")
                        .HasColumnName("impressora1");

                    b.Property<string>("Impressora2")
                        .HasColumnType("text")
                        .HasColumnName("impressora2");

                    b.Property<string>("Impressora3")
                        .HasColumnType("text")
                        .HasColumnName("impressora3");

                    b.Property<string>("Impressora4")
                        .HasColumnType("text")
                        .HasColumnName("impressora4");

                    b.Property<string>("Impressora5")
                        .HasColumnType("text")
                        .HasColumnName("impressora5");

                    b.Property<string>("ImpressoraAux")
                        .HasColumnType("text")
                        .HasColumnName("impressoraaux");

                    b.Property<bool>("ImprimirComandaNoCaixa")
                        .HasColumnType("boolean")
                        .HasColumnName("imprimircomandacaixa");

                    b.Property<bool>("IntegraAnotaAi")
                        .HasColumnType("boolean")
                        .HasColumnName("integraanotaai");

                    b.Property<bool>("IntegraCCM")
                        .HasColumnType("boolean")
                        .HasColumnName("integraccm");

                    b.Property<bool>("IntegraDelMatch")
                        .HasColumnType("boolean")
                        .HasColumnName("integradelmatch");

                    b.Property<bool>("IntegraDelmatchEntregas")
                        .HasColumnType("boolean")
                        .HasColumnName("integradelmatchentregas");

                    b.Property<bool>("IntegraIfood")
                        .HasColumnType("boolean")
                        .HasColumnName("integraifood");

                    b.Property<bool>("IntegraOnOPedido")
                        .HasColumnType("boolean")
                        .HasColumnName("integraonpedido");

                    b.Property<bool>("IntegraOttoEntregas")
                        .HasColumnType("boolean")
                        .HasColumnName("integraottoentregas");

                    b.Property<bool>("IntegracaoSysMenu")
                        .HasColumnType("boolean")
                        .HasColumnName("integracaosysmenu");

                    b.Property<string>("MerchantId")
                        .HasColumnType("text")
                        .HasColumnName("merchantid");

                    b.Property<string>("NomeFantasia")
                        .HasColumnType("text")
                        .HasColumnName("nomefantasia");

                    b.Property<int>("NumDeViasDeComanda")
                        .HasColumnType("integer")
                        .HasColumnName("numviascomanda");

                    b.Property<string>("PasswordTaxyMachine")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("passwordtaxymachine");

                    b.Property<bool>("RemoveComplementos")
                        .HasColumnType("boolean")
                        .HasColumnName("removecomplmentos");

                    b.Property<string>("SenhaDelMatch")
                        .HasColumnType("text")
                        .HasColumnName("senhadelmatch");

                    b.Property<string>("SenhaOnPedido")
                        .HasColumnType("text")
                        .HasColumnName("senhaonpedido");

                    b.Property<string>("Telefone")
                        .HasColumnType("text")
                        .HasColumnName("telefone");

                    b.Property<int>("TempoConclonPedido")
                        .HasColumnType("integer")
                        .HasColumnName("tempoconclonpedido");

                    b.Property<int>("TempoEntrega")
                        .HasColumnType("integer")
                        .HasColumnName("tempoentrega");

                    b.Property<int>("TempoRetirada")
                        .HasColumnType("integer")
                        .HasColumnName("temporetirada");

                    b.Property<int>("TipoComanda")
                        .HasColumnType("integer")
                        .HasColumnName("tipocomanda");

                    b.Property<string>("TipoPagamentoTaxyMachine")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("tipodepagamentotaxymachine");

                    b.Property<string>("TokenAnotaAi")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("tokenanotaai");

                    b.Property<string>("TokenCCM")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("tokenccm");

                    b.Property<string>("TokenOnPedido")
                        .HasColumnType("text")
                        .HasColumnName("tokenonpedido");

                    b.Property<bool>("UsarNomeNaComanda")
                        .HasColumnType("boolean")
                        .HasColumnName("usarnomenacomanda");

                    b.Property<string>("UserDelMatch")
                        .HasColumnType("text")
                        .HasColumnName("userdelmatch");

                    b.Property<string>("UserNameTaxyMachine")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("usernametaxymachine");

                    b.Property<string>("UserOnPedido")
                        .HasColumnType("text")
                        .HasColumnName("useronpedido");

                    b.HasKey("Id");

                    b.ToTable("parametrosdosistema");
                });

            modelBuilder.Entity("SysIntegradorApp.ClassesAuxiliares.Token", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("id"));

                    b.Property<string>("TokenDelMatch")
                        .HasColumnType("text")
                        .HasColumnName("tokendelmatch");

                    b.Property<string>("TokenOnPedido")
                        .HasColumnType("text")
                        .HasColumnName("tokenonpedido");

                    b.Property<string>("VenceEm")
                        .HasColumnType("text")
                        .HasColumnName("venceem");

                    b.Property<string>("VenceEmDelMatch")
                        .HasColumnType("text")
                        .HasColumnName("venceemdelmatch");

                    b.Property<string>("VenceEmOnPedido")
                        .HasColumnType("text")
                        .HasColumnName("venceemonpedido");

                    b.Property<string>("accessToken")
                        .HasColumnType("text")
                        .HasColumnName("accesstoken");

                    b.Property<int>("expiresIn")
                        .HasColumnType("integer")
                        .HasColumnName("expiresin");

                    b.Property<string>("refreshToken")
                        .HasColumnType("text")
                        .HasColumnName("refreshtoken");

                    b.Property<string>("type")
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.ToTable("parametrosdeautenticacao");
                });
#pragma warning restore 612, 618
        }
    }
}
