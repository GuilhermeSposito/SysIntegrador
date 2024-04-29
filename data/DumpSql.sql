create table parametrosdosistema (
  id serial primary key,
  nomefantasia text not null,
  endereco text,
  impressaoaut bool not null,
  aceitapedidoaut bool not null,
  caminhodoBanco text, 
  caminhoservidor text, 
  integracaosysmenu bool not null,
  impressora1 text,
  impressora2 text,
  impressora3 text, 
  impressora4 text, 
  impressora5 text,
  impressoraaux text,
  telefone text,
  clientid text,
  clientsecret text,
  merchantid text,
  delmatchid text,
  agruparcomandas bool,
);

create table parametrosdopedido(
  id serial primary key,
  json text not null,
  situacao text,
  conta int,
  criadoem text
);

create table parametrosdeautenticacao(
	id serial primary key,
	accesstoken text not null, 
	refreshtoken text,
	type text,
	expiresin int
);