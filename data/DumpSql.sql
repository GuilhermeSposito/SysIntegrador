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
  impressora5 text
);

