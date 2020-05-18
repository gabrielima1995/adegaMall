using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public static class VersionHistory
    {
        public static IReadOnlyCollection<VersionEntry> Entries { get; private set; }
        public static VersionEntry Current { get; private set; }

        static VersionHistory()
        {
            var list = new List<VersionEntry>
            {
                new VersionEntry(
                    2, 46, 10, "Hotfix produto digital", new DateTime(2020, 05, 11),
                    new[]
                    {
                        "Ao tentar cadastrar um produto digital, não está sendo passado o valor do shipping e assim, informava que não havia sido selecionado um método de entrega."
                    }),

                new VersionEntry(
                    2, 46, 9, "Hotfix imagens", new DateTime(2020, 05, 08),
                    new[]
                    {
                        "Correção da exibição das imagens na home page."
                    }),

                new VersionEntry(
                    2, 46, 8, "Hotfix", new DateTime(2020, 05, 08),
                    new[]
                    {
                        "Elimina possibilidade de NullReferenceException em NullDateTimeBinder, e refatora o código para facilitar o entendimento",
                        "Corrige erro na geração de SiteMap, e refatora o código para facilitar o entendimento",
                        "Corrige erro da busca, ao montar hidden fields para os filtros laterais (facetas, variantes), que às vezes lançava NullReferenceException",
                        "Corrige erro na rendeização de custom fields no cadastro de vendedor, que às vezes lançava KeyNotFoundException",
                        "Corrige erro ao cadastrar comprador durante o envio de orçamento; tentava gerar senha para o cadastro usando PersonId ao invés de UserId"
                    }),

                new VersionEntry(
                    2, 46, 7, "Hotfix gerais relacionadas ao login, frete do carrinho e dados cadastrais do vendedor", new DateTime(2020, 05, 06),
                    new[]
                    {
                        "Ao efetuar o cadastro inicial sem preencher os dados iniciais (endereço, documento e etc), após isso, se deslogar da conta e fazer o fluxo de compra, gerava um erro relacioado a busca de frete que não possuia endereço de entrega.",
                        "O redirecionamento após o login não estáva feito de forma acertiva, focando na compra dos produtos/serviços e por isso, mesmo tendo itens no carrinho, após ser efetuado o login, o comprador não era redirecionado para página do carrinho.",
                        "Erros relacionados a falta de cadastro do comprador, que impedem de ser efetuado a compra, são exibidos para o comprador e redirecinado para a página de cadastro desses dados."
                    }),

                new VersionEntry(
                    2, 46, 6, "Hotfix deletar portfóio e busca de portfólio", new DateTime(2020, 05, 05),
                    new[]
                    {
                        "Não deletamos mais os itens de portfólio mas marcamos eles como deleted = true e deletedAt = DateTime.Utc.Now",
                        "A mudança foi feita pois não era possível deletar um item de portfólio que estava ligado a um orçamento.",
                        "A busca dos portfólios foi corrigida para refletir a nova coluna da tabela portfólioItems. Desse modo, não irá exibir itens do portfólio que já forma deletados."
                    }),

                new VersionEntry(
                    2, 46, 5, "Hotfix cadastro do produto pela importação da planilha", new DateTime(2020, 05, 05),
                    new[]
                    {
                        "O cadastro de produtos obrigava a preencher dois dados: DefaultShippingAmount e DefaultShippingDeliveryDays, caso eles fossem nulos. Porém, esses dados não são obrigatórios."
                    }),

                new VersionEntry(
                    2, 46, 4, "Hotfix configs padrões da plataforma", new DateTime(2020, 05, 05),
                    new[]
                    {
                        "Adicionado todas as configs que devem ser padrão na plataforma."
                    }),

                new VersionEntry(
                    2, 46, 3, "Hotfix labels na plataforma", new DateTime(2020, 05, 04),
                    new[]
                    {
                        "Foram trocadas e adicionados as labels na plataforma para alterar as mensagens fora do padrão (PT-BR) e links que eram antigos."
                    }),

                new VersionEntry(
                    2, 46, 2, "Hotfix", new DateTime(2020, 05, 04),
                    new[]
                    {
                        "Corrige uma tentativa de verificar um LoginProfile que havia sido definido como nulo",
                        "Corrige erro de referência nula ao tentar montar a string de paginação da busca",
                        "Corrige erro de referência nulo ao tentar cadastrar um token para PushNotifications"
                    }),

                new VersionEntry(
                    2, 46, 1, "Correção do cadastro de vendedor", new DateTime(2020, 05, 04),
                    new[]
                    {
                        "Corrige um redirecionamento incorreto após o cadastro de vendedor, que causava erro de tela azul, tipo 404 (NotFound)"
                    }),

                new VersionEntry(
                    2, 46, 0, "Login único de comprador e vendedor", new DateTime(2020, 05, 04),
                    new[]
                    {
                        "Adiciona flag de configuração > singleUserMultipleProfilesConfig > enabled (padrão true)",
                        "Habilita o cadastro de perfis adicionais, gerenciados pelo mesmo login de usuário",
                        "Permite apenas um perfil de cada tipo",
                        "Entidade Person ganhou UserId para fazer referência ao usuário que controla o perfil, permitindo que um usuário controle vários perfis",
                        "LoginProfile (antigo UserLoginProfile) traz informações do usuário gerenciado: o próprio perfil quando logado como Buyer, Seller ou Admin; o perfil Seller quando logado como SellerAssistant",
                        "Ainda em LoginProfile, traz AssistantUserId e AssistantPersonId quando logado como assistente. Importante notar que, no login do assistente, UserId, PersonId e demais informações correspondem ao Seller",
                        "Não se deve mais usar verificações do tipo IsInRole na maioria dos casos; agora, um usuário pode estar relacionado a vários deste. O correto é verificar com qual perfil o usuário está logado (LoginProfile responde)",
                        "Novos atributos de Authorization específicos para cada perfil de login: AuthorizeAdmin, AuthorizeMaster, AuthorizeBuyer, AuthorizeSeller, AuthorizeSellerAssistant",
                        "Também traz atributos de perfis combinados: AuthorizeBuyerOrSeller, AuthorizeSellerOrSellerAssistant, entre outros",
                        "Vários pacotes NuGet atualizados. Entre eles: Unity, Mvc, WebApi, entre outros"
                    }),

                new VersionEntry(
                    2, 45, 2, "Correção dropdownview variantes produtos", new DateTime(2020, 04, 28),
                    new[]
                    {
                        "Adicionado js para poder criar um dropdown que tenha campo de busca. Assim sendo, é possível efetuar a busca de variantes.",
                        "Alterado a descrição da label CART_PRODUCT_STOCK_OUT para '(Sem estoque)'."
                    }),

                new VersionEntry(
                    2, 45, 1, "Assistente do Vendedor: labels", new DateTime(2020, 04, 26),
                    new[]
                    {
                        "Define labels de tradução pendentes para a funcionalidade",
                    }),

                new VersionEntry(
                    2, 45, 0, "Assistente do Vendedor", new DateTime(2020, 04, 26),
                    new[]
                    {
                        "Altera todas as Controllers que obtinham Id de Vendedor para obterem Id do Vendedor Gerenciado",
                        "Valida permissões do Assistente em todas as páginas que são de acesso do vendedor",
                        "Remove usuário GestaoINA dos emails copiados para admins",
                        "Desabilita links e botões na visualização do assistente de acordo com as permissões",
                        "Links e botões desabilitados para o assistente aparecem em cor mais clara e têm tooltip explicativa",
                        "Envia email para assistente com orientações iniciais de acesso"
                    }),

                new VersionEntry(
                    2, 44, 0, "Criado dropdownlist para variantes", new DateTime(2020, 04, 24),
                    new[]
                    {
                        "Esse dropdown funciona apartir de uma config -> Product.Variants.enableForNItemsOrHigher, quando a quantidade de variantes é igual ou maior do que foi setado na config, as variantes se transforma em um dropdownlist.",
                        "Caso uma ou mais variantes não possuam estoque, elas se tornam inativas(desabled) e não podem ser selecionadas para a compra."
                    }),

                new VersionEntry(
                    2, 43, 33, "Hotfix gatewayFee", new DateTime(2020, 04, 29),
                    new[]
                    {
                        "Criada a config para calcular o gatewayFee na plataforma e salvar esse dado no banco.",
                        "O valor dessa taxa modificava cada valor da parcela e foi feito uma método para contabilizar isso.",
                        "Corrigido a variável responsável por dizer se as taxas do gateway eram percentuais ou fixas.",
                        "Criada variável responsável por setar o nome na fatura do cartão(Wirecard). Caso seja null, é setado o código do pedido."
                    }),

               new VersionEntry(
                    2, 43, 32, "Hotfix email avaliação", new DateTime(2020, 04, 28),
                    new[]
                    {
                        "Corrigido o bug do envio de email para avaliar comprador/vendedor após o pagamento do orçamento. Sendo que deve ser enviado quando o status do pedidos está como concluído."
                    }),

               new VersionEntry(
                    2, 43, 31, "Hotfix email orçamento aceito", new DateTime(2020, 04, 28),
                    new[]
                    {
                        "O email responsável por avisar quando um comprador aceitou o orçamento, possue a variável QUOTE_CONTENT mas não adicionava dados nela. Portanto, ao colocar essa variável no email, pelo painel admin, acabava que não mostrava nenhuma informação."
                    }),

               new VersionEntry(
                    2, 43, 30, "Hotfix alternative name nos emails + Aumentado tamanho do campo descrição no Portfólio + Vitrine best sellers", new DateTime(2020, 04, 27),
                    new[]
                    {
                        "Correção do nome do seller nos email, quando existe um Alternative name, é necessário substituir o name por essa variável pelo alternative name.",
                        "Aumentado o tamnho do campo da descrição do portfólio que era de 1024 caracteres para 2024.",
                        "Adicionado verificação para mostrar os seller moderados caso tenha moderação ativa na plataforma."
                    }),

               new VersionEntry(
                    2, 43, 29, "Hotfix exibição dos customfields do carrinho e pedido", new DateTime(2020, 04, 27),
                    new[]
                    {
                        "Corrigido o bug da troca das informações (customFields) dos itens do carrinho e nos detalhes do pedido. Como haviam duas variáveis para os customFields, uma com o nome e outra com o valor, as vezes o nome e valor não eram correspondentes.",
                        "Corrigido a query de busca dos valores e nomes dos customFields."
                    }),

               new VersionEntry(
                    2, 43, 28, "Hotfix dropdown atividades agenda", new DateTime(2020, 04, 27),
                    new[]
                    {
                        "Corrigido o bug que não exibia as atividades existentes na agenda do vendedor. Foi identificado que a query que buscava as atividades estava bem diferenta da query que buscava a lista de atividades para o dropdown."
                    }),

               new VersionEntry(
                    2, 43, 27, "Hotfix pagamento estilo ecommerce", new DateTime(2020, 04, 24),
                    new[]
                    {
                        "Correção para deixar possível o pagemento entre comprador e vendedor simulando um ecommerce.",
                        "Criado config MoipConfig.Current.MarketplaceZeroCommission que define a comissão zerada para o marketplace. Pois, o marketplace será somente o intermediário na venda."
                    }),

               new VersionEntry(
                    2, 43, 26, "Hotfix Shipping Information", new DateTime(2020, 04, 24),
                    new[]
                    {
                        "Corrigido o bug responsável por não exibir as informações de entrega do carrinho, quando o shipping é ActualNoShipping, OnSitePickup e NoShipping."
                    }),

               new VersionEntry(
                    2, 43, 25, "Hotfix moderação comprador", new DateTime(2020, 04, 23),
                    new[]
                    {
                        "Corrigido o bug da moderação automática do buyer mesmo com a config de moderação ativa na plataforma."
                    }),

               new VersionEntry(
                    2, 43, 24, "Hotfix admin customização", new DateTime(2020, 04, 23),
                    new[]
                    {
                        "Corrigido o bug que impedia de alterar os dados da seção 'Como funciona' no painel admin.",
                        "Quando havia uma tentativa de adicionar um dado nos campos, a controller não permitia o post para adicionar os dados no banco pois, havia dados em html sendo salvos e por isso, a controller impede o post por pensar que é uma requisição perigosa."
                    }),

               new VersionEntry(
                    2, 43, 23, "Hotfix slpit pagamento wirecard", new DateTime(2020, 04, 23),
                    new[]
                    {
                        "Agora é possível fazer uma conta com somente um recebedor. Portanto, na venda de produtos, a plataforma pode se compartar como um ecommerce que faz a transação de um comprador para um vendedor.",
                        "Essa alteração só foi feita para o gateway da wirecard."
                    }),

               new VersionEntry(
                    2, 43, 22, "Hotfix exibição do portfólio", new DateTime(2020, 04, 22),
                    new[]
                    {
                        "Corrigido a exibição de true ou false no portfólio."
                    }),

               new VersionEntry(
                    2, 43, 21, "Hotfix api sellers", new DateTime(2020, 04, 22),
                    new[]
                    {
                        "Corrigido o bug que impedia de fazer as requisições na api de sellers, pois o parâmetro routeValues, vinha com atributos nulos. Adicionado verifição para identificar se os parâmetros são nulos."
                    }),

               new VersionEntry(
                    2, 43, 20, "Correções exibições", new DateTime(2020, 04, 20),
                    new[]
                    {
                        "Correção na exibição da imagens na vitrine da homepage. Não era verificado as imagens que possuiam producttemplateimage em seus links.",
                        "Correção na exibição do preço do produto na vitrine,não estava sendo posicionado a pontuação do preço de forma correta."
                    }),

               new VersionEntry(
                    2, 43, 19, "Hotfix cancelamento assinatura", new DateTime(2020, 04, 20),
                    new[]
                    {
                        "Correção no SellerCompleteProfile que não tornava a assinatura cancelada no json e por isso, não mostrava isso para o seller. Desse modo, ficava disponível o link para cancelar a assinatura mesmo estando já cancelada."
                    }),

               new VersionEntry(
                    2, 43, 18, "Correção na indexação dos sellers", new DateTime(2020, 04, 17),
                    new[]
                    {
                        "Corrigido bug de indexações dos vendedores que não levava em conta os dados do SellerCompleteProfile e por isso, acabava indexando sellers que não tinha completado o cadastro."
                    }),

               new VersionEntry(
                    2, 43, 17, "Correções pontuais", new DateTime(2020, 04, 16),
                    new[]
                    {
                       "Corrigido a label que informava incorretamente que tinha feito o cancelamento do orçamento.",
                       "Corrigido a label que é um place holder do campo 'descrição do porfolio' que não estava de acordo com o que deveria ser preenchido.",
                       "Adicionado classe na tabela das vendas do vendedor para poder diferenciar as colunas.",
                       "Corrigido a frase no painel admin que dizia que o comprador estava com o perfil incompleto e deveria se logar com ele para poder arrumar. Porém, não é possível fazer isso com o comprador."
                    }),

               new VersionEntry(
                    2, 43, 16, "Tabela de pagamentos em garantia", new DateTime(2020, 04, 16),
                    new[]
                    {
                        "Tabela criada no painel admin para identificar os pagamentos em garantia. Se localiza em /admin/sellers/details. Funciona somente para pagamentos em garantia da pagarme.",
                        "A verificação é feita somente no banco de dados da plataforma, não é feita nenhuma requisição para pagarme para confirmar se os dados do bd."
                    }),

               new VersionEntry(
                    2, 43, 15, "Hotfix compartilhar links", new DateTime(2020, 04, 16),
                    new[]
                    {
                        "Corrigido o bug de compartilhamento de links pelas redes sociais. A plataforma tentava identifcar o cultura escolhida pelo usuário, porém, isso não é um valor existente quando se tenta compartilhar o link do produto."
                    }),

               new VersionEntry(
                    2, 43, 14, "Hotfix edição de produtos rentals", new DateTime(2020, 04, 15),
                    new[]
                    {
                        "Criado um script para verificar se foi feita modificações na página e assim, informar para usuário uma mensagem de que ao adicionar uma disponibilidade antes de salvar o produto, não irá salvar novos os dados do produto."
                    }),

                new VersionEntry(
                    2, 43, 13, "Calendário do vendedor responsivo (mobile)", new DateTime(2020, 04, 15),
                    new[]
                    {
                        "Adapta o componente full-calendar para renderização mobile"
                    }),

                new VersionEntry(
                    2, 43, 12, "Distância na busca de produtos", new DateTime(2020, 04, 14),
                    new[]
                    {
                        "Adiciona a opção de ordenação por distância na busca de produtos",
                        "Opção da distância só fica acessível se o usuário informar um endereço",
                        "Nos resultados da busca, mostra a distância relativa ao endereço informado",
                        "Formatação inteligente da distância 396 metros (abaixo de 1km) e 1,5 km (a partir de 1km)"
                    }),

                new VersionEntry(
                    2, 43, 11, "Melhorias ajax frete", new DateTime(2020, 04, 14),
                    new[]
                    {
                        "ProductController agora trabalha com a sessão em modo somente leitura, habilitando paralelismo de requisições que operam com o mesmo modo de sessão",
                        "Adiciona configuração para tempo limite de resposta dos Correios para cálculo de preço e prazo de frete, config > product > correiosTimeoutSeconds (padrão = 6)",
                        "Adiciona labels de tradução para as mensagens de espera de carregamento do carrinho: CART_LOADING_WAIT_MSG_1 e CART_LOADING_WAIT_MSG_2",
                        "No anúncio do produto, só ativa o ícone de loading de informações de frete se o usuário estiver logado como comprador",
                        "Incorpora CSS padrão para corrigir rolagem dos banners quando banners estão ativos"
                    }),

                new VersionEntry(
                    2, 43, 10, "Hotfix sellerCompleteProfile + exibição dos customFields nos detalhes do pedido + UX checkout pagamento com cartão", new DateTime(2020, 04, 14),
                    new[]
                    {
                        "Criado endpoint admin/sellers/sellersvalidation para revalidar os dados do cadastros dos sellers.",
                        "Criado verificação da existência de dados no customfields do carrinho. Para não mostra um campo vazio na tela ou deixar os dados fora da ordem.",
                        "Adicionado icones para facilitar o preenchimeto dos dados bancários durante a compra."
                    }),

                new VersionEntry(
                    2, 43, 9, "Hotfix envio de emails de agendamento + Hotfix compra buyer não moderado", new DateTime(2020, 04, 13),
                    new[]
                    {
                        "O envio de emails de agendamento acontecia mesmo após o período de agendamento ter passado.",
                        "O buyer mesmo não moderado podia efetuar compras pela plataforma."
                    }),

                new VersionEntry(
                    2, 43, 8, "Hotfix pagmento pedido cartão de crédito/boleto", new DateTime(2020, 04, 13),
                    new[]
                    {
                        "Corregido o bug do pagamento de pedido com o valor = 0. Agora os pedidos com valor 0 não são enviados ao gateway de pagamento",
                        "Quando só há produto com valor 0 no checkout, as opções de pagamento e boleto são omitidas. Portanto, não ficam disponíves para pagamento do pedido."
                    }),

                new VersionEntry(
                    2, 43, 7, "Hotfix PendingPaymentRecurrentJob", new DateTime(2020, 04, 13),
                    new[]
                    {
                        "Foi adicionado mais um termo na busca da job para os casos em que o pedido fica como processado. Assim sendo, resolvendo o problema de pedidos que ficam com o status processado indefinidamente."
                    }),

                new VersionEntry(
                    2, 43, 6, "Frete Padrão para falha dos Correios", new DateTime(2020, 04, 10),
                    new[]
                    {
                        "Quando PAC ou Sedex estão habilitados na plataforma, abre-se a configuração de Frete Padrão",
                        "Frete Padrão pode ser configurado produto-a-produto ou para toda a loja",
                        "Trata-se de um valor e prazo padrão, aplicado em caso de falha na comunicação com os Correios (demora excessiva ou não-reposta)",
                        "É uma alternativa para evitar a perda de vendas por falhas dos Correios"
                    }),

                new VersionEntry(
                    2, 43, 5, "Hotfix cancelamento assinatura", new DateTime(2020, 04, 09),
                    new[]
                    {
                        "Criado variável na view model Subscription para informar quando é o fim da assinatura.",
                        "Adicionado informação explicando que ao cancelar a assinatura, o usuário perde os benefícios dela, mesmo tendo tempo vigente"
                    }),

                new VersionEntry(
                    2, 43, 4, "Correções", new DateTime(2020, 04, 08),
                    new[]
                    {
                        "Correção no ícone de loading dos fretes na página do detalhe do produto.",
                        "Correção da exibição do endereço nos detalhes do pedido.",
                        "Correção da exibição de fretes endereço de entrega no carrinho no caso do produto não ser físico."
                    }),

                new VersionEntry(
                    2, 43, 3, "Hotfix importação de produtos", new DateTime(2020, 04, 08),
                    new[]
                    {
                        "Não era possível importar produtos quando o produto tinha a entrega por frota privada mas não enviava os dados de frota privada (raio, preço).",
                        "Criado variáveis para receber os dados da frota privada, se o produto não tiver frota privada, é possível enviar com o valor 0.",
                        "Melhoria no template de exemplo para informar que os campos devem ser preenchido, mesmo que seja com o valor 0"
                    }),

                new VersionEntry(
                    2, 43, 2, "Carregamento ajax do carrinho + cache dos dados de frete", new DateTime(2020, 04, 07),
                    new[]
                    {
                        "Agora a view index do carrinho exibe uma imagem de carregamento na página até carregar os dados do carrinho, pois é feita uma requisição ajax dos dados.",
                        "Os dados do carrinho são armazenados em cache até 15 minutos. O cache foi implementado no design patters decorator.",
                        "Detalhes do produto com requisição ajax do correios.",
                        "Erro de timeout do correios mapeado e exibido para o comprador."
                    }),

                new VersionEntry(
                    2, 43, 1, "Customização na agenda do vendedor na página de perfil", new DateTime(2020, 04, 01),
                    new[]
                    {
                        "config > product > businessModel > rental > seller > schedule > configuration > profile > minTime (padrão: '00:00:00')",
                        "config > product > businessModel > rental > seller > schedule > configuration > profile > maxTime (padrão: '24:00:00')",
                        "Configura hora inicial e final para cada dia na agenda semanal",
                        "config > product > businessModel > rental > seller > schedule > configuration > profile > slotDuration (padrão: '01:00:00')",
                        "Configura o tempo de cada bloco de duração dos eventos na agenda semanal",
                        "config > product > businessModel > rental > seller > schedule > configuration > profile > slotLabelInterval (padrão: null)",
                        "Configura o intervalo de tempo para os blocos de duração dos eventos na agenda semanal",
                        "config > product > businessModel > rental > seller > schedule > configuration > profile > hiddenDays (padrão: vazio)",
                        "Configura os dias da semana que não irão aparecer na agenda semanal",
                        "Parâmetros de customização da exibição da agenda semanal na página do perfil do vendedor"
                    }),

                new VersionEntry(
                    2, 43, 0, "Agenda do vendedor na página de perfil", new DateTime(2020, 03, 24),
                    new[]
                    {
                        "config > product > businessModel > rental > seller > schedule > configuration > profile > enabled (padrão: false)",
                        "Habilita a agenda do vendedor na página de perfil dele",
                        "Comprador pode visualizar os produtos e selecionar um horário para locação, abrindo a página do produto com a seleção já marcada"
                    }),

                new VersionEntry(
                    2, 42, 15, "Carregamento ajax do carrinho + cache dos dados de frete", new DateTime(2020, 04, 03),
                    new[]
                    {
                        "Agora a view index do carrinho exibe uma imagem de carregamento na página até carregar os dados do carrinho, pois é feita uma requisição ajax dos dados.",
                        "Os dados do carrinho são armazenados em cache até 15 minutos. O cache foi implementado no design patters decorator."
                    }),

                new VersionEntry(
                    2, 42, 14, "Correção filtro distância na busca", new DateTime(2020, 04, 02),
                    new[]
                    {
                        "Plataforma tentava filtrar ao mesmo tempo por distância e por bounds, bounds ganhava prioridade, e assim a distância a partir da origem não era considerada",
                        "Agora, quando campo para filtro por distância está habilitado na busca, repassa o filtro Location + RangeInKm, não repassando mais o Bounds",
                        "Correção DateTime binder, em alguns casos não detectava corretamente a cultura do usuário e não conseguia converter o valor"
                    }),

                new VersionEntry(
                    2, 42, 13, "Melhoria indexação e busca de produtos com agendamento", new DateTime(2020, 04, 01),
                    new[]
                    {
                        "Antes, cada produto poderia ter até 876.000 itens num DateTime[]",
                        "Agora, cada produto pode ter até 1.825 (redução de 99,998%) itens num int[]",
                        "Correção na indexação, que não desconsiderava produtos com IsDisabled = true",
                        "Correção dos filtros de data OnlySatAndSun e OnlyMonToFri, que trazia produtos não disponíveis nas datas correspondentes",
                        "Próximo passo é reduzir a quantidade de DateTimes criados durante o processo para avaliar a disponibilidade do produto",
                        "Hoje é feito minuto-a-minuto (525.600 minutos num ano, isso é feito para cada produto); encontrar uma forma de obter o mesmo resultado apenas com início e fim de cada disponibilidade",
                        "Objetivo é reduzir custo de memória, que chega a mais de 3gb em alguns clientes e tira marketplaces do ar"
                    }),

                new VersionEntry(
                    2, 42, 12, "Hotfix indexação de produtos", new DateTime(2020, 04, 01),
                    new[]
                    {
                        "Indexação de grade de horário só é realizada se modelo de agendamento/locação estiver habilitado",
                    }),

                new VersionEntry(
                    2, 42, 11, "Início isolamento hangfire", new DateTime(2020, 04, 01),
                    new[]
                    {
                        "Alto nível de dependência da plataforma no Hangifre, mais de 5 projetos faziam referência",
                        "Reduzido para apenas 2, sendo um adapter (CrossCutting.Hangfire) e Presentation",
                        "Próximo passo é tirar de Presentation e reestruturar o CrossCutting.Hangfire",
                        "Reestruturação deverá isolar o Hangfire dos outros projetos e impedir jobs duplicadas",
                        "Jobs duplicadas é um problema que causa picos de CPU, deixas muitas plataformas lentas e tira algumas do ar"
                    }),

                new VersionEntry(
                    2, 42, 10, "Hotfix frete retirada no local", new DateTime(2020, 03, 31),
                    new[]
                    {
                        "Alterado o local de exibição do endereço de retirado do produto para o checkout. Assim sendo, fica mais congruente para ele revisar esse dado antes do pagamento.",
                        "Corrigido bug ao selecionar o frete de retirada pela plataforma. Quando o buyer selecionava essa opção, os outros fretes desapareciam e não era possível selecionar um novo endereço."
                    }),

                new VersionEntry(
                    2, 42, 9, "Hotfix UX no filtros de busca em mobile + links de artigos em telas admin", new DateTime(2020, 03, 31),
                    new[]
                    {
                        "Quando era feita uma busca pelo mobile, não ficava visível o botão para expandir os filtros de busca. Portanto, foi trocado esse filtro para deixar de ser um link para um botão, facilitando a sua visibilidade.",
                        "Adicionado artigos para facilitar a usabilidade da plataforma pelo administrador."
                    }),

                new VersionEntry(
                    2, 42, 8, "Hotfix tela assinaturas admin + Hotfix postback assinaturas pagarme", new DateTime(2020, 03, 31),
                    new[]
                    {
                        "A tela reponsável por mostrar as assinaturas feitas na plataforma, trazia status dois quais o admin não ficava sabendo se foi pago ou cancelado a assinatura do seller (Trialing). Assim sendo, foi colocada uma verificação para saber se a assinatura é trialing, e se for, ela é exibida com assinatura paga.",
                        "Implementado endpoint para o postback de assinaturas da pagarme e também o map responsável por converter os dados passados pela pagarme em classes utilizáveis. Desse modo, quando houver alteração de assinaturas na pagarme, isso deve ser replicado para a nossa plataforma."
                    }),

                new VersionEntry(
                    2, 42, 7, "Hotfix pagamento", new DateTime(2020, 03, 30),
                    new[]
                    {
                        "Alterado o tipo de requisição da pagarme, antes era feita pelo framework deles, agora usamos a rquisição por Rest. Desse modo, corrigindo a possibilidade de vir algo nulo da própia request, que era o que gerava erro."
                    }),

                new VersionEntry(
                    2, 42, 6, "Hotfix filtro dos produtos na agenda + config para frete dobrado", new DateTime(2020, 03, 27),
                    new[]
                    {
                        "A lista dos produtos no filtro não verificava se o produto já tinha sido deletado, e por isso, acabava trazendo produtos que não existiam mais.",
                        "Criada config para impedir de aparcer a box de frete dobrado caso isso não seja pentinente a plataforma."
                    }),

                new VersionEntry(
                    2, 42, 5, "Hotfix na exibição de categorias na home", new DateTime(2020, 03, 27),
                    new[]
                    {
                        "Ao alterar o nome de uma categoria da home, não era exibido a alteração na home page e por isso, gerava uma incongruência na plataforma."
                    }),

                new VersionEntry(
                    2, 42, 4, "Hotfix frete carrinho", new DateTime(2020, 03, 27),
                    new[]
                    {
                        "O cálculo do frete não estava prevendo os casos que poderiam dar erro e por isso, quando o erro ocorria, uma variável não era adicionada e vinha como null. Desse modo, ao tentar manipular a variável, acabava gerando exceção."
                    }),

                new VersionEntry(
                    2, 42, 3, "Correção na grade de horários para cadastro de disponibilidade de produto", new DateTime(2020, 03, 23),
                    new[]
                    {
                        "Corrigida a grade de horários quando há cadastro de disponibilidades para o produto"
                    }),

                new VersionEntry(
                    2, 42, 2, "Ajuste na exibição de preços nas listagens de produtos", new DateTime(2020, 03, 20),
                    new[]
                    {
                        "Ajuste na exibição de texto complementar e dados adicionais de preço nas listagens de produtos"
                    }),

                new VersionEntry(
                    2, 42, 1, "Correção no cadastro de horário de funcionamento", new DateTime(2020, 03, 19),
                    new[]
                    {
                        "Corrigido problema ao salvar o horário de funcionamento para toda a loja, após ter selecionado um produto específico"
                    }),

                new VersionEntry(
                    2, 42, 0, "Agenda do vendedor - Modo Timeline", new DateTime(2020, 03, 18),
                    new[]
                    {
                        "config > environment > fullCalendar > hasPremiumLicense (padrão: false)",
                        "Habilita o plugin fullcalendar-scheduler, que é pago",
                        "config > environment > fullCalendar > licenseKey (padrão: null)",
                        "Chave da licença para o fullcalendar-scheduler",
                        "config > product > businessModel > rental > seller > schedule > viewMode (padrão: default. Valores possíveis: default e timeline)",
                        "Novo modo de visualização da agenda do vendedor. O modo timeline exibe as disponibilidades dos horários dos produtos e possibilidade ver se há locações para cada horário"
                    }),

                new VersionEntry(
                    2, 41, 23, "Hotfixes", new DateTime(2020, 03, 26),
                    new[]
                    {
                        "Correção no model-binding dos filtros de variantes (facetas) na busca; exceção quando vinha nulo",
                        "Melhor disposição dos campos de filtro por data no responsivo",
                        "Correção no preço dos produtos no painel admin > detalhes do vendedor > produtos; agora usa o campo de preço calculado, como o restante da plataforma"
                    }),

                new VersionEntry(
                    2, 41, 22, "Hotfix em deletar os templates de produtos", new DateTime(2020, 03, 26),
                    new[]
                    {
                        "Corrigido a query responsável por deletar os produtos que foram feitos com o template e o própio template."
                    }),

                new VersionEntry(
                    2, 41, 21, "Hotfix na busca de portfolios", new DateTime(2020, 03, 26),
                    new[]
                    {
                        "Corrigido a busca de portfólio que buscava sellers pelo lucene ao invés de busca os portfólios pela busca antiga."
                    }),

               new VersionEntry(
                    2, 41, 20, "Correção na compra de créditos", new DateTime(2020, 03, 25),
                    new[]
                    {
                        "Não era feito o cadastro no gateway e por isso, não era possível fazer a compra de créditos."
                    }),

               new VersionEntry(
                    2, 41, 19, "Hotfix dashboard seller", new DateTime(2020, 03, 24),
                    new[]
                    {
                        "Correção na query que selecionava os produtos vendidos pela plataforma, não era levando em conta os produtos que o pagamento era por fora da plataforma."
                    }),

               new VersionEntry(
                    2, 41, 18, "Hotfix na ordenacao dos itens na homepage", new DateTime(2020, 03, 23),
                    new[]
                    {
                        "Foi passado a partial view dos itens 'próximos a você' para cima.Desse modo, fica mais congruente a disposição dos itens na tela da home page."
                    }),

               new VersionEntry(
                    2, 41, 17, "Hotfix UX frete e endereço de entrega", new DateTime(2020, 03, 23),
                    new[]
                    {
                        "Melhorado a UX das respostas de retorno do correios, tornando mais fácil a correção do endereço de entrega.",
                        "Centralizado a seleção do endereço de entrega + método de entrega.",
                        "Panel colocado acima para facilitar o preenchimento em caso de erro do correios ou de outro tipo de método de entrega. Caso ocorra, é necessário o buyer trocar o endereço de entrega para continuar a compra."
                    }),

               new VersionEntry(
                    2, 41, 16, "Correção na busca de sellers na homepage", new DateTime(2020, 03, 23),
                    new[]
                    {
                        "Corrigido a busca dos sellers que gerava exceção por não achar a controller responsável pela busca."
                    }),

               new VersionEntry(
                    2, 41, 15, "Hotfix config de busca para o produto", new DateTime(2020, 03, 20),
                    new[]
                    {
                        "A busca do produto não estava levando em conta uma config do produto (IsDisabled), e por isso, produtos que estavam desativados ainda apareciam nas buscas."
                    }),

               new VersionEntry(
                    2, 41, 14, "Hotfix compra de creditos", new DateTime(2020, 03, 20),
                    new[]
                    {
                        "Quando o o usuário não completava o cadastro e tentava efetuar a compra de créditos, acabava gerando erro mas não informava qual era o erro. Assim sendo, foi feita uma alteração na view informando que ele deve completar o cadastro para poder efetuar a compra de créditos."
                    }),

               new VersionEntry(
                    2, 41, 13, "Hotfix contagem buyer admin", new DateTime(2020, 03, 20),
                    new[]
                    {
                        "A contagem estava errada pois a query trazia somente os buyers que já tinham sido moderados, sendo que, deveria trazer somente os ativos da plataforma."
                    }),

               new VersionEntry(
                    2, 41, 12, "Base para perfil de acesso do vendedor", new DateTime(2020, 03, 20),
                    new[]
                    {
                        "AspNetUsers perde as colunas AdminId, BuyerId e SellerId, substituídas por ProfileId + ProfileType, que indicado o ID de perfil principal do login",
                        "Novo tipo SellerAssistant, derivado de Person, que traz o Campo SellerId, indicando qual vendedor ele gerencia",
                        "Fluxo de cadastro do assistente e definição de permissões está implementado",
                        "Pendente de implementação do login de assistente e validação das permissões em todas as páginas internas do vendedor"
                    }),

               new VersionEntry(
                    2, 41, 11, "Hotfix busca de orcamentos por localidade", new DateTime(2020, 03, 19),
                    new[]
                    {
                        "Correção na busca, por localidade, de sellers para o orçamento. A quando o seller fazia uma busca por uma localidade abrangente, não era verificado o raio de busca. Sendo assim, buscava todas as pessoa naquela região mas não verificava o raio de busca do MaasConfig."
                    }),

                new VersionEntry(
                    2, 41, 10, "Hotfix status wirecard", new DateTime(2020, 03, 19),
                    new[]
                    {
                        "Como foi feito a troca do que recebemos da wirecard (recebiamos o códigode multi-pagamento e agora recebemos o do pedido), a requisição não era feita. Pois, necessitava de um id de multi-pagamento e não de pedido.",
                        "Foi trocada a requisição feita para wirecard. Agora é feita uma requisição de pedido."
                    }),

                new VersionEntry(
                    2, 41, 9, "Hotfix endereço no carrinho", new DateTime(2020, 03, 19),
                    new[]
                    {
                        "Corrigido o bug referente a partial view responsável por alterar ou cadastrar um endereço de entrega, o caminho para a partial view tinha que ser por extenso, o que não era o caso."
                    }),

                new VersionEntry(
                    2, 41, 8, "Hotfix correcao na compra de creditos leads", new DateTime(2020, 03, 18),
                    new[]
                    {
                        "Não era possível efetuar a compra de créditos pois o buyer não era cadastrado na hora da compra.",
                        "Foi colocada uma verificação no telefone para que não comece com o número 0.",
                        "Implementado exibição de erros amigáveis para o usuário preencher o cadastro caso não tenha sido feito."
                    }),

                new VersionEntry(
                    2, 41, 7, "Hotfix moderacao comprador", new DateTime(2020, 03, 18),
                    new[]
                    {
                        "Estava havendo um bypass na moderação, quando o buyer completava o cadastro, o buyer era automaticamente deixado como moderado."
                    }),

                new VersionEntry(
                    2, 41, 6, "Hotfix anuncio leads", new DateTime(2020, 03, 17),
                    new[]
                    {
                        "Quando o seller cancela sua assinatura, a plataforma considera ele com assinatura até o termino do tempo da assinatura. Assim sendo, ao terminar o tempo de assinatura, ele volta a não ter mais o cadastro completo e portanto, precisará adquirir uma nova assinatura."
                    }),

                new VersionEntry(
                    2, 41, 5, "Hotfix portfolio incompleto", new DateTime(2020, 03, 17),
                    new[]
                    {
                        "O adicionado uma verificação a mais no SellerComplete profile para as categorias de portfólio.",
                        "Corrigido o bug de ter categoria cadastrada, pois o seller tinha portfólio e nele possui categorias.",
                        "Mudado a forma de verificar se possui categorias de portfólio na partial view _ProfileHireButton, agora a variável HasCategories é atribuida a variável SellerCompletProfile.PortfolioCategories.IsComplete."
                    }),

                new VersionEntry(
                    2, 41, 4, "Hotfix nome do comercial nos emails", new DateTime(2020, 03, 12),
                    new[]
                    {
                        "Agora todos os email (que possuem a variável SellerName) da plataforma verificam se existe o nome comercial, caso exista, o nome do seller é substituido pelo nome comercial no email.",
                        "Corrigido a variável que estava sendo declarada mais de uma vez."
                    }),

                new VersionEntry(
                    2, 41, 3, "Hotfix configs CloudStorage", new DateTime(2020, 03, 13),
                    new[]
                    {
                        "As connfigurações como SecretKey, AccessKey e BucketName estavam referênciando o arquivo appsettings, porém não utilizamos mais o arquivo. Atualmente é utilizado a Classe MaasConfig para guardar as configs da plataforma. Portanto, as configs do CloudStorage estão recebendo o valor do MaasConfig."
                    }),

                new VersionEntry(
                    2, 41, 2, "Hotfix endereco do pedido no dashboard admin", new DateTime(2020, 03, 13),
                    new[]
                    {
                        "Quando o pedido do produto era feito na plataforma, não era visto se o pedido era por netrega ou retirada. Desse modo, o pedido na tela admin estava com o endereço do comprador ao invés do endereço do vendedor."
                    }),

                new VersionEntry(
                    2, 41, 1, "AspNetUsers para autenticação e Person para perfis", new DateTime(2020, 03, 17),
                    new[]
                    {
                        "Criada tabela Person a partir de AspNetUsers. Person é focada em perfis (comprador, vendedor, admins), e AspNetUsers é focada em autenticação",
                        "Separação é base para permitir cadastro de comprador e vendedor sob o mesmo login, além da futura funcionalidade 'perfil de acesso', que permitirá ao vendedor criar logins que possam gerenciar sua conta",
                        "Migration foi amplamente testada em cópias das bases de todas as demos. Testados os processos de update-database e rollback múltiplas vezes em cada base."
                    }),

                new VersionEntry(
                    2, 41, 0, "Filtro por distância e Produtos Próximos na home", new DateTime(2020, 03, 12),
                    new[]
                    {
                        "search > searchFields > locationRange (booleano) (padrão: false)",
                        "Habilita filtro para busca de produtos por distância",
                        "nearbyProducts > enabled (booleano) (padrão: false)",
                        "Habilita seção Produtos Próximos na home do marketplace"
                    }),

               new VersionEntry(
                    2, 40, 10, "Hotfix dashboard admin seller", new DateTime(2020, 03, 13),
                    new[]
                    {
                       "O select dos seller no painel admin, na página admin/sellers, não estava levando em conta que poderia existir sellers com o atributo RequiredModeration = null, e por isso, acabava trazendo menos seller que deveria, pois só estava conferindo se o RequiredModeration = false."
                    }),

               new VersionEntry(
                    2, 40, 9, "Hotfix endereço de envio do produto (ShippingAddress)", new DateTime(2020, 03, 12),
                    new[]
                    {
                       "Agora o cadastro e troca do endereço para envio de um produto, é feito no carrinho. Portanto, não existe mais a modal e as etapas de cadastro de endereço na aba página de checkout. Foi feito assim, pois fica mais fácil do comprador(buyer) selecionar o endereço de entrega caso tenha algum erro."
                    }),

               new VersionEntry(
                    2, 40, 8, "Hotfix bug modos de plataformas ativos ao mesmo tempo", new DateTime(2020, 03, 11),
                    new[]
                    {
                        "Não era possível ativar o modo de agendamento + orçamento para a mesma plataforma. Pois, não tinhamos nos deparado com a possibilidade de trabalhar com os dois modos ativos oque levou a problemas ao ativar essas configs."
                    }),

               new VersionEntry(
                    2, 40, 7, "Hotfix bug wirecard multipagamentos", new DateTime(2020, 03, 11),
                    new[]
                    {
                       "Quando havia mais de um item de diferentes vendedores, a nossa plataforma não identificava qual era cada pagamento de cada pedido. Por isso, a plataforma deixava os dois pedidos com o mesmo pagamento (na verdade, somava os valores dos dois pagamentos).",
                       "Agora a plataforma traz os dados de multipagamentos dos pagamentos. Na Interface ITransaction, foi adicionado os campos multipaymentitem e ismultipayment para poder trazer os dados corretos da wirecard.",
                       "Quando a transaction é feito por outro gateway, ele ignora que a requisição seja de multipagamento e segue da forma que foi antes dessa feature."
                    }),

               new VersionEntry(
                    2, 40, 6, "Hotfix troca do nome ShippingDay para DeliveryDay", new DateTime(2020, 03, 11),
                    new[]
                    {
                        "O nome shippingday, no caso, era uma nomenclatura errada para a funcionalidade e por isso, para deixar mais congruente a plataforma, foi trocado o nome."
                    }),

               new VersionEntry(
                    2, 40, 5, "Equivalência de preço entre busca e anúncio de produtos", new DateTime(2020, 03, 10),
                    new[]
                    {
                        "Dependendo de configurações, busca mostra um preço menor que o anúncio (ex. preço unitário)",
                        "Anúncio mostra o preço final que o cliente vai pagar, para evitar problemas",
                        "Esse cenário gera, por exemplo, Busca = 90 por unidade VS Anúncio = 450 por 5 unidades",
                        "Esta melhoria evidencia a equivalência de preços entre busca e anúncio, mostrando preço da busca logo abaixo do preço final do produto"
                    }),

                new VersionEntry(
                    2, 40, 4, "Correção lista de produtos do vendedor", new DateTime(2020, 03, 10),
                    new[]
                    {
                        "Lista de produtos foi migrada para datatables trabalhando server side e apresentou problemas na renderização",
                        "Imagem volta a ser redimensionada e carregada por CDN",
                        "Corrigido bug no controle de paginação, que mostrava páginas a mais",
                        "Removido controle de configuração do tamanho da página, para trabalhar sempre com 25, fixo",
                        "Melhor renderização do preço do produto, indicando o preço formatado e desconto onde houver",
                        "Removidos os controles de ordenação, até recurso ser implementado"
                    }),

                new VersionEntry(
                    2, 40, 3, "Correção contagem de variantes da busca de produtos", new DateTime(2020, 03, 09),
                    new[]
                    {
                        "Não contava todas as variantes dentre todos os resultados da base; contagem era parcial"
                    }),

                new VersionEntry(
                    2, 40, 2, "Hotfix indexação da busca de produtos", new DateTime(2020, 03, 06),
                    new[]
                    {
                        "Indexação travava em while (true)",
                        "Trava acontecia por produtos antigos sem configuração de duração fixa",
                        "Inserida cláusula de saída forçada, impedindo o loop infinito"
                    }),

                new VersionEntry(
                    2, 40, 1, "Novo campo Modo de Disponibilidade do Produto", new DateTime(2020, 03, 05),
                    new[]
                    {
                        "Novo campo, no cadastro do Produto, para indicar o Modo de Disponibilidade (Automático ou Manual)",
                        "Modo Automático, habilita o link para criar bloqueios na Agenda do Produto",
                        "Modo Manual, habilita o cadastro de Disponibilidade do Produto (Tanto Livre como Recorrente)",
                        "Campo somente é visível se a funcionalidade de Disponibilidade de Produto estiver habilitada",
                        "Grade de horários carrega os horários disponíveis com base no modo cadastrado no produto"
                    }),

                new VersionEntry(
                    2, 40, 0, "Nova busca de produtos no SmartSearch", new DateTime(2020, 03, 03),
                    new[]
                    {
                        "search > searchFields > periodAvailability (booleano) (padrão: false)",
                        "Habilita o filtro de busca por datas, para produtos de locação, na tela de busca de produtos",
                        "Habilita o filtro de busca por datas, para produtos de locação, na seção da home para busca de produtos",
                        "Nova busca de produtos no SmartSearch. Se aplica tanto para produtos de venda e de locação",
                        "Ajuste na exibição do preço do produto nos resultados da busca de produto"
                    }),

               new VersionEntry(
                    2, 39, 21, "Hotfix tela admin assinaturas e contagem de assinaturas no dashboard admin", new DateTime(2020, 03, 05),
                    new[]
                    {
                       "Não estavam sendo mostrados os status das assinaturas na tela admin/subscriptions. Assim sendo, o dado de status não era exportado na planilha excel. Existia também um problema na exibição da contagem de assinaturas pagas (não relevava as assinaturas triais)."
                    }),

               new VersionEntry(
                    2, 39, 20, "Hotfix endereço no pedido", new DateTime(2020, 03, 05),
                    new[]
                    {
                       "Quando o usuário completava o carrinho de compras (fazia o checkin), o endereço do seller não era exibido na tela do pedido. Porém, agora está sendo disponibilizado por uma config(Product.Order.ShowAddressAfterPayment), que quando ativa + o produto estando pago ou o pagamento é por fora da plataforma, mostra o endereço do seller."
                    }),

               new VersionEntry(
                    2, 39, 19, "Hotfix adicionar carrinho não estando logado", new DateTime(2020, 03, 05),
                    new[]
                    {
                       "Ao tentar adicionar um item ao carrinho, quando não logado, e a plataforma tem customFields para o cartItem, esses customFields não eram adicionados no carrinho. Por esse motivo, acabava gerando uma incongruência na plataforma."
                    }),

               new VersionEntry(
                    2, 39, 18, "Hotfix no cadastro de conta bancária wirecard", new DateTime(2020, 03, 04),
                    new[]
                    {
                       "Quando o usuário utilizava a funcionalidade de trazer os dados de endereço base para os dados da conta bancária, não era trazido a cidade e estado da forma correta (faltava o hífen separando a cidade e estado) e por isso, gerava uma exceção de tela azul para o usuário."
                    }),

               new VersionEntry(
                    2, 39, 17, "Hotfix na renderização da tabela dos produtos na loja do seller", new DateTime(2020, 03, 03),
                    new[]
                    {
                       "Com o aumento do número de produtos em uma loja, a renderização das imagens tornavam o carregamento da tela bem lenta ou não carregava. Por esse motivo, foi feito uma renderização via ajax para que a tabela fosse renderizada por página, diminuindo o tempo de carregamento."
                    }),

               new VersionEntry(
                    2, 39, 16, "Hotfix na troca de conta na pagarme", new DateTime(2020, 03, 03),
                    new[]
                    {
                       "Não estava sendo enviado o tipo da conta (conta corrente ou poupança) e por esse motivo, não era possível trocar o tipo da conta e não apresentava erro na view. Pois, ocorria com sucesso mas não trocava o tipo da conta."
                    }),

               new VersionEntry(
                    2, 39, 15, "Hotfix bug seleção nos dropdown list", new DateTime(2020, 03, 02),
                    new[]
                    {
                       "Quando era selecionado uma opção nos dropdowns da plataforma, fazia com que a página atual descesse até o rodapé (final da pág). OBS: isso ocorria somente com macbook."
                    }),

               new VersionEntry(
                    2, 39, 14, "Hotfix bug cadastro seller facebook", new DateTime(2020, 03, 02),
                    new[]
                    {
                       "Quando o seller efetuava o cadastro com os dados do facebook, era redirecionado para a página de cadastro do seller e tentava criar uma uri da imagem, sendo que não havia imagem. Por esse motivo, acabava gerando uma exceção."
                    }),

               new VersionEntry(
                    2, 39, 13, "Hotfix Imagens SellerCompletProfile subscription ", new DateTime(2020, 02, 28),
                    new[]
                    {
                       "Quando o seller comprava uma assinatura na plataforma, não era disparado o evento dizendo que ele comprou uma assinatura, e por isso, nas plataformas que assinatura era obrigatóra, não era informado que já estava com assinatura, causando incongruências na plataforma."
                    }),

               new VersionEntry(
                    2, 39, 12, "Implementação da data de prazo de entrega do produto", new DateTime(2020, 02, 28),
                    new[]
                    {
                        "As entrgas pelos Correios, retornam uma data estimada da entraga do produto em dias, foi formatado, a data, para que fosse retornado para o comprador e vendedor uma estimativa para a entrega, tanto para pac, quanto para sedex.",
                        "O prazo de entrega é mostrado na tela de detalhes do produto (PublicPage), no tela do carrinho, no checkout e na tela de detalhes do pedidos, quando já foi efetuado a venda.",
                        "Quando há mais de um produto, a data de entrega é sempre a maior, dando tempo para todos os produtos serem entregues."
                    }),

               new VersionEntry(
                    2, 39, 11, "Hotfix pág admin orçamentos sem vendedores", new DateTime(2020, 02, 26),
                    new[]
                    {
                       "Quando o admin tentava acessar um orçamento sem vendedores, era gerado uma excessão pois o método tentava adicionar o seller id sendo que não havia vendedores para aquele orçamento."
                    }),

               new VersionEntry(
                    2, 39, 10, "Campos customFields", new DateTime(2020, 02, 18),
                    new[]
                    {
                        "Agora é possível adicionar campos customizáveis na compra de produtos.",
                        "Config CustomField.CartItem criada com a finalidade de separar um novo customField na plataforma."
                    }),

               new VersionEntry(
                    2, 39, 9, "Correções e Melhorias", new DateTime(2020, 02, 18),
                    new[]
                    {
                        "Adicionado campos na verificação do cadastro completo, os campos são: Endereço, Data de Nascimento e CPF/CNPJ.",
                        "Melhoria PushNotification no admin/notification, adicionado mais opções para as notificações."
                    }),

               new VersionEntry(
                    2, 39, 8, "Hotfix no link de compra de assinaturas", new DateTime(2020, 02, 19),
                    new[]
                    {
                        "Corrigido o link que redirecionava para compra de assinaturas. Infelizmente ele redirecionava para a página errada."
                    }),

                new VersionEntry(
                    2, 39, 7, "Hotfix compra de assinatura na venda de leads", new DateTime(2020, 02, 14),
                    new[]
                    {
                        "Corrigido a compra de assinatura nos marketplaces de leads, era feito uma tentativa de cadastro no gateway, sem a necessidade do mesmo.",
                        "Corrigido verificação do Id do gateway sem necessidade.",
                        "Corrigido bug que não informava que era necessário preencher o cadastro de endereço para poder comprar a assinatura."
                    }),

                new VersionEntry(
                    2, 39, 6, "Correções", new DateTime(2020, 02, 13),
                    new[]
                    {
                        "Corrigido o bug referente ao mostrar produtos relacionados de sellers que não tinham completado o cadastro.",
                        "Corrigido o bug que enviava email ao admin informando que havia orçamentos sem vendedores porém, tinha vendedores que haviam recebido o orçamento."
                    }),

                new VersionEntry(
                    2, 39, 5, "Melhoria UX completar cadastro seller", new DateTime(2020, 02, 13),
                    new[]
                    {
                        "Implementação que melhora a ux para informar que o cadastro do seller não está completo. SellerCompleteProfileSteps",
                        "Correção no ProfileInvisibleMessage. Quando não ere seller, estourava exception por não ter o campo SellerCompleteProfile preenchido."
                    }),

                new VersionEntry(
                    2, 39, 4, "Correções e melhorias", new DateTime(2020, 02, 12),
                    new[]
                    {
                        "Ajuste nas consultas de eventos das agendas de vendedor e comprador",
                        "Valor de desconto no cadastro de disponibilidade de recorrência do produto",
                        "Formatação da hora inicial ao alterar o campo no cadastro de disponibilidade do produto"
                    }),

                new VersionEntry(
                    2, 39, 3, "Correção do campo peso do produto", new DateTime(2020, 02, 11),
                    new[]
                    {
                        "Modificado campo do peso (colocado formatação de 3 casas decimais + alterado o formato no banco para 8,3).",
                        "Todos os campos de dimensões agora possuem 2 casas decimais depois da vírgula.",
                        "Alteração na exibição do produto para mostrar mais casas decimais, agora são três casas sendo exibidas."
                    }),

                new VersionEntry(
                    2, 39, 2, "Usar endereço base para o cadastro dos dados bancários", new DateTime(2020, 02, 10),
                    new[]
                    {
                        "Agora é possível utilizar o mesmo dado, de endereço, que foi cadastrado no perfil do seller. Desse modo, ao clicar no botão 'Usar o mesmo endereço do cadastro base', os campos de endereço são preenchidos pelos dados de endereço do seller."
                    }),

                new VersionEntry(
                    2, 39, 1, "Disponibilidade recorrente de produtos do tipo pacote de créditos", new DateTime(2020, 02, 07),
                    new[]
                    {
                        "product > businessModel > rental > productAvailability > recurring > enabled (booleano) (padrão: false)",
                        "Habilita a opção de cadastrar a disponibilidade recorrente de um produto por horário, do tipo pacote de créditos, informando estoque",
                        "Habilita a gestão de horários de locação recorrente e estoque via cadastro de disponibilidade",
                        "Habilita a exibição das disponibilidades recorrentes cadastradas para o produto"
                    }),

                new VersionEntry(
                    2, 39, 0, "Disponibilidade de estoque por horário", new DateTime(2020, 01, 23),
                    new[]
                    {
                        "product > businessModel > rental > productAvailability > enabled (booleano) (padrão: false)",
                        "Habilita a opção de cadastrar a disponibilidade de um produto por horário, informando estoque e desconto",
                        "Habilita a gestão de horários de locação e estoque via cadastro de disponibilidade",
                        "Habilita a exibição de desconto por horário na tela de detalhes do produto"
                    }),

                new VersionEntry(
                    2, 38, 29, "Hotfix dos campos de dimensões do produto", new DateTime(2020, 02, 08),
                    new[]
                    {
                        "Formatação incorreta dos números nos campos de dimensão alterava o valor por deslocar a vírgula",
                        "Antes, o campo determinava apenas 1 casa decimal. A mudança alterou para 2 casas decimais",
                        "Com isso, o valor 25.0 cm passava a aparecer como 2.50 cm",
                        "Da mesma forma o peso, que antes era 0.3 kg passava a aparecer como 3 kg"
                    }),

                new VersionEntry(
                    2, 38, 28, "Correções", new DateTime(2020, 02, 07),
                    new[]
                    {
                        "Correção no campo peso, no cadastro e edição de produtos, para facilitar seu preenchimento.",
                        "Correção do bug das imagens na view _HomeBestSellers, não concatenava o link corretamente.",
                        "Correção na query do SiteMapAppService.cs"
                    }),

                new VersionEntry(
                    2, 38, 27, "Implementação da checkbox BlockChanges", new DateTime(2020, 02, 07),
                    new[]
                    {
                        "Implementado a checkbox BlockChanges que bloqueia campos após moderão, os campos que tem essa funcionalidade são: DocumentNumber(CPF/CNPJ), DateOfBirth e CustomFields. Essa nova funcionalidade é tanto para sellers quanto buyers, podendo ser ativada no painel admin."
                    }),

                new VersionEntry(
                    2, 38, 26, "Melhorias em emails e datalhes de pedidos na aba admin", new DateTime(2020, 02, 05),
                    new[]
                    {
                        "Implementado variável, RentalStarDate e RentalEndDate, nós emails de 'Admin, novo pedido' e 'admin, novo pedido com pagamento efetuado'.",
                        "Implementado variável, RentalStarDate e RentalEndDate, nos detalhes de pedidos na aba do admin."
                    }),

                new VersionEntry(
                    2, 38, 25, "Hotfix das buscas de produtos por localização", new DateTime(2020, 02, 05),
                    new[]
                    {
                        "Quando era feita uma busca de produtos por localização e o produto não tinha endereço, gerava uma exceção."
                    }),

                new VersionEntry(
                    2, 38, 24, "Hotfix das imagens vitrine home", new DateTime(2020, 02, 05),
                    new[]
                    {
                        "Foi adicionado as imagens, dos itens da vitrine, ao cloudfront para otimiziar o carregamento da home page."
                    }),

                new VersionEntry(
                    2, 38, 23, "Hotfix dos modelos de produtos", new DateTime(2020, 02, 04),
                    new[]
                    {
                        "Correção no cadastro de variantes",
                        "Ao tentar adicionar nova variante ao produto com o botão (+), a plataforma mostrava erro e não adicionava"
                    }),

                new VersionEntry(
                    2, 38, 22, "Correções no cadastro completo do seller", new DateTime(2020, 02, 04),
                    new[]
                    {
                        "Corrigido bug dos customfields que estavam sendo contabilizados para completar o cadastro, sendo que não eram obrigatórios seus preenchimentos.",
                        "Corrigido bug dos produtos que estavam sendo contabilizados para completar o cadastro, sendo que eram em plataformas aonde não tinha o cadastro de produto habilitado."
                    }),

                new VersionEntry(
                    2, 38, 21, "Correção na compra de pacotes de créditos (venda de leads)", new DateTime(2020, 01, 31),
                    new[]
                    {
                        "Foi reformulado a etapa de verificação do seller, caso o supplier seja null, que é o caso da compra de créditos na venda de leads."
                    }),

                new VersionEntry(
                    2, 38, 20, "Correções e Melhorias", new DateTime(2020, 01, 31),
                    new[]
                    {
                        "Otimizado as buscas para verificar se o cadastro do seller foi completo.",
                        "Corrigido as buscas para as méticas do Dashboard Admin.",
                        "Melhorado o popover na tela de moderação dos Admins (agora mostra o que falta no cadastro do seller para ele estar completo)."
                    }),

                new VersionEntry(
                    2, 38, 19, "Melhoria na verificação do cadastro completo do Seller", new DateTime(2020, 01, 29),
                    new[]
                    {
                      "Implementado colunas no banco responsável por salvar, a versão em que está sendo feita a verificação do cadastro completo (SellerCompleteProfileVersion) e um campo json resposável por salvar os dados que estão o não completos (SellerCompleteProfile), tabela AspNetUsers.",
                      "Implementado job responsável por fazer a verificação da versão em que está o cadastro completo, e o preenchimento do json caso a versão não esteja de acondo com a da plataforma.",
                      "Retirada a classe CompleteSellerSpecification pois não a mais uso para ela.",
                      "Implementado verificação nos customFields do seller, feita a exibição caso não seja preenchido e for obrigatório."
                    }),

                new VersionEntry(
                    2, 38, 18, "Correção nos detalhes de pedidos sem vendedor, tela Admin", new DateTime(2020, 01, 28),
                    new[]
                    {
                      "Corrigido o bug referente aos orçamentos sem pedidos na tela do Admin. Quando era redirecionado para a tela de detalhes, era feito a tentativa de gerar uma Seller.FriendlyUrl mesmo quando o seller era nullo."
                    }),

                new VersionEntry(
                    2, 38, 17, "Frete por Correios com código de contrato", new DateTime(2020, 01, 29),
                    new[]
                    {
                        "Novas configurações em config.product.correiosContract para especificar dados de contrato personalizado com os Correios",
                        "Configurações se aplicam para todo o marketplace"
                    }),

                new VersionEntry(
                    2, 38, 16, "Hotfix modelos de produtos", new DateTime(2020, 01, 28),
                    new[]
                    {
                        "Correção em erro de merge",
                    }),

                new VersionEntry(
                    2, 38, 15, "Modelos de produtos", new DateTime(2020, 01, 28),
                    new[]
                    {
                        "Configuração para ativar a funcionalidade em config.products.templates.enable",
                        "Habilita nova seção no painel admin (produtos >  modelos de produtos), onde é possível cadastrar modelos prontos, especificando quais campos podem ser alterados pelo vendedor",
                        "Vendedores cadastram produtos a partir de um modelo, definindo preço, frete, parâmetros de locação, agenda e outros, mas respeitando os demais campos conforme configuração do template",
                        "Configuração para determinar que o vendedor só pode cadastrar produtos através de um modelo em config.products.templates.forceSellerToUseTemplates",
                        "Neste caso a tela não apresenta mais a opção de cadastro livre de produtos, oferecendo somente o cadastro a partir de modelos",
                        "Se o admin altera o template e define como não editável um campo que antes era editável, a platforma replica o valor do campo para todos os produtos do tempalte, sobrescrevendo",
                        "Se o admin exclui o template, a plataforma exclui todos os produtos associados ao mesmo"
                    }),

                new VersionEntry(
                    2, 38, 14, "Melhorias", new DateTime(2020, 01, 23),
                    new[]
                    {
                        "Implementado flag para ocultar campo 'Código da atividade'. Na etapa de cadastro de produto, existe um campo chamado 'Código da atividade', que agora, pode ser ocultado por Product.CustomId.",
                        "Implementado flag para ocultar campo 'Preço promocional'. Na etapa de cadastro de produto, existe um campo chamado 'Preço promocional', que agora, pode ser ocultado por Product.discountPrice.",
                        "Implementado flag para fixar a quantidade de vagas da atividade (estoque do produto) num número determinado em configuração(padrão 1).Se esta flag estiver habilitada, ocultar a seção de vagas/ estoque da atividade, simplificando o cadastro.",
                        "Implementado flag para fixar a duração da atividade. Agora é possível fixiar um horário nas configs da plataforma, desativando o campo Duração fixa (desable no campo), e colocando o valor setado na config."
                    }),

                new VersionEntry(
                    2, 38, 13, "Correção no CustomFields do portfólio", new DateTime(2020, 01, 23),
                    new[]
                    {
                        "Quando era feito o cadastro de um item no portfólio, não era mostrado o erro de falta de preenchimento, caso o usuário, não preenchesse os customFields obrigatórios."
                    }),

                new VersionEntry(
                    2, 38, 12, "Hotfix importação de imagens", new DateTime(2020, 01, 21),
                    new[]
                    {
                        "Corrige a importação de imagens de produtos, que não respeitava a ordem das imagens e não associava corretamente as imagens vs produtos",
                        "A ordem é importante no momento de definir a imagem de capa do produto (sempre a primeira)"
                    }),

                new VersionEntry(
                    2, 38, 11, "Correções e melhorias", new DateTime(2020, 01, 17),
                    new[]
                    {
                        "Criada verificação para ver se o seller preencheu o campo WhatsApp e config criada para tornar, ou não, obrigatório para completar o cadastro.",
                        "Melhorado a responsividade dos banners",
                        "Corrigido o Bug do link para o Paniel Admin não aparecer no menu-mobile."
                    }),

                new VersionEntry(
                    2, 38, 10, "Criada a flag para bloquear agendamentos", new DateTime(2020, 01, 14),
                    new[]
                    {
                        "Se o produto tiver essa flag marcada, não será possível fazer agendamentos no mesmo horário que esse produto."
                    }),

                new VersionEntry(
                    2, 38, 9, "Correções e melhorias", new DateTime(2020, 01, 14),
                    new[]
                    {
                        "Ajustes na exibição das políticas de cancelamento e reagendamento de locação na tela de detalhes do pedido do comprador",
                        "Implementado Pop-up responsável por perguntar se o seller realmente deseja deletar o seu produto da loja.",
                        "Retirado a obrigatoriedade do cadastro de itens no portifólio para tornar o cadastro visível. Também foi criado config para deixar obrigatório caso seja necessário.",
                        "Implementado verificação da imagem do seller, para poder informar que o perfil estará invisível se não for cadastrada a imagem. Também foi criada uma config verificando se é obrigatório, ou não, a adição da imagem ao perfil.",
                        "Adição do ModifiedAt nas classes responsáveis por salvar os dados bancários e de produto. Agora quando forem modificados dados referentes a essas classes, o modified será alterado."
                    }),

                new VersionEntry(
                    2, 38, 8, "Correções Pagamento Wirecard", new DateTime(2020, 01, 13),
                    new[]
                    {
                        "A plataforma fazia uma verificação dos dados em uma tabela (BankAccount) que não está mais em uso, e que em alguns casos, ela não está preenchida. Por isso, não efetuava a transação com a wirecard."
                    }),

                new VersionEntry(
                    2, 38, 7, "Correções e Melhorias", new DateTime(2020, 01, 10),
                    new[]
                    {
                        "Implementado campo ModifiedAt na aba de administração dos seller para poder identificar aqueles seller que fizeram alterações em seus cadastros."
                    }),

                new VersionEntry(
                    2, 38, 6, "Correção login facebook", new DateTime(2020, 01, 10),
                    new[]
                    {
                        "Problemas na requisição com o facebook. O Facebook lia o scheme do chrome para montar a url de redirecionamento mas esse url as vezes ia com http em vez de https. Por isso, foi necessário forçar o https no scheme para funcionar."
                    }),

                new VersionEntry(
                    2, 38, 5, "Correções e melhorias", new DateTime(2020, 01, 07),
                    new[]
                    {
                        "Adicionado campo no dashboard do seller para informar o valor bruto e valor líquido das vendas efetuadas na plataforma de marketplace.",
                        "Implementado a visualização total do saques na tela de 'meu saldo'.",
                        "Retirado o 'ver mais detalhes' dos orçamentos quando a plataforma é por venda de leads."
                    }),

                new VersionEntry(
                    2, 38, 4, "Correções e melhorias", new DateTime(2020, 01, 03),
                    new[]
                    {
                        "Quando um produto que possuia agendamento era deletado, não era deletado o seu bloqueio na agenda.",
                        "Foi adicionado a variável ExtraInfo nos emails, para que os emails de Order e Rental, pudessem mostrar dados adicionais como: Nome de aluno.",
                        "Adicionado reticências no final das avaliações para que a descrição delas não ficassem cortada.",
                        "Corrigido bug dos campos não centralizados de busca na home page.",
                        "O produto estava mostrando um dado antigo de pacote de créditos sendo que não era mais vendido dessa forma.",
                        "Formatação do horário na agenda agora é:'dd/mm/yyyy HH:mm'."
                    }),

                new VersionEntry(
                    2, 38, 3, "Correção", new DateTime(2020, 01, 03),
                    new[]
                    {
                        "Ajuste ao calcular o totalizador de estoque ao processar as transações"
                    }),

                new VersionEntry(
                    2, 38, 2, "Correção", new DateTime(2020, 01, 03),
                    new[]
                    {
                        "Ajuste ao retornar estoque de produto com variantes, quando o pedido é cancelado"
                    }),

                new VersionEntry(
                    2, 38, 1, "Correção", new DateTime(2020, 01, 03),
                    new[]
                    {
                        "Ajuste ao zerar o estoque geral no cadastro do produto"
                    }),

                new VersionEntry(
                    2, 38, 0, "Configuração de opção de cancelamento e reagendamento por atividade", new DateTime(2019, 12, 27),
                    new[]
                    {
                        "Novos campos para Política de Cancelamento e Reagendamento na tela de cadastro de produto",
                        "Mantêm as políticas do cadastro do produto junto ao pedido",
                        "Exibição das políticas na tela de detalhes do produto",
                        "Exibição de botões e tooltips referente às políticas do produto para cancelamento e reagendamento na tela de detalhes do pedido",
                        "Cancelamento pró-rata de locações de produto do tipo Pacote de créditos",
                        "Estorno parcial de compra para gateway de pagamento Pagar.me",
                        "Estorno parcial de compra para gateway de pagamento Wirecard",
                        "Valor do estorno nas telas de detalhes do pedido e detalhes da compra"
                    }),

                new VersionEntry(
                    2, 37, 25, "Correções e Melhorias", new DateTime(2019, 12, 27),
                    new[]
                    {
                       "Quando o seller muda o status do produto para entregue, é avançado automaticamente o status para concluído.",
                       "Corrigido bug de responsividade dos campos de busca da homa page.",
                       "Corrigido bug de subcategorias repetidas no campo de busca por subcategoria na home page."
                    }),

                new VersionEntry(
                    2, 37, 24, "Correções e Melhorias", new DateTime(2019, 12, 26),
                    new[]
                    {
                        "Corrigido bug na condição de mostrar a foto do buyer na etapa de aceitar o orçamento, mesmo quando já tinha fechado negócio, não mostra a foto.",
                        "Adicionado função Math.Abs() no cálculo de saldo do vendedor, que se localiza no painel admin/meu saldo, para não resultar em valores negativos. Se aplica para o gateway da wirecard."
                    }),

                new VersionEntry(
                    2, 37, 23, "Correções e Melhorias", new DateTime(2019, 12, 23),
                    new[]
                    {
                        "Corrigido bug que impedia do admin receber o email informando que foi feito uma venda na plataforma. Assim sendo, foi criado o email que informa quando a venda é feita e o pagamento é efetuado.",
                        "Adicionado mais uma verificação no if da moderação, agora podendo moderar os usuários que estão com a coluna RequireModeration nula.",
                        "Adicionado label, PRODUCT_FILTER_LOCATION_INPUT, que adiciona um placeHolder no campo de busca por local.",
                        "Corrigido bug responsável por mostrar adicional vazio do produto na vitrine da home page."
                    }),

                new VersionEntry(
                    2, 37, 22, "Correções e Melhorias", new DateTime(2019, 12, 19),
                    new[]
                    {
                        "Corrigido o bug quando era escolhido o método de 'entraga por fora da plataforma', assim que era escolhido, sumia com as outra opções de escolha.",
                        "Corrigido o bug de login que acontecia quando o usuário tentava se logar com um email não existente no banco de dados."
                    }),

                new VersionEntry(
                    2, 37, 21, "Correções e Melhorias", new DateTime(2019, 12, 16),
                    new[]
                    {
                        "Corrigido bug na exibição dos botões para a moderação (Aprovar cadastro, Reprovar cadastro)."
                    }),

                new VersionEntry(
                    2, 37, 20, "Problemas no carrinho de compras", new DateTime(2019, 12, 13),
                    new[]
                    {
                        "Corrigido bug na hora de informar que existia erros na entrega do produto (o shipping era null, por isso gerava erro)."
                    }),

                new VersionEntry(
                    2, 37, 19, "Restrição idade de comprador e vendedor", new DateTime(2019, 12, 13),
                    new[]
                    {
                        "Configuração para restringir idade mínima e máxima para comprador e vendedor",
                        "Padrão é mínimo de 0 anos e máximo de 100 anos",
                        "config.buyer.dateOfBirth.minAge|maxAge, e config.seller.dateOfBirth.minAge|maxAge",
                        "Idade é validada no momento do cadastro e alterar cadastro",
                        "Registra data de modificação de cadastro de comprador e vendedor"
                    }),

                new VersionEntry(
                    2, 37, 18, "Dados de login no header da plataforma", new DateTime(2019, 12, 12),
                    new[]
                    {
                        "Inclui hidden fields no header (menu) com dados do usuário logado",
                        "Dados são usados por javascript para autocompletar certos campos"
                    }),

                new VersionEntry(
                    2, 37, 17, "Correções e Melhorias", new DateTime(2019, 12, 11),
                    new[]
                    {
                        "Corrigido o bug de enviar email do cadastro aprovado. Vendedores que tiveram seu cadastro reprovados, recebiam esse email."
                    }),

                new VersionEntry(
                    2, 37, 16, "Correções e Melhorias", new DateTime(2019, 12, 10),
                    new[]
                    {
                        "Criada verificação de telefone caso seja inserido o número de país ao invés do DDD.",
                        "Criado Labels no arquivo de tradução, referenta a modal de ShippingWarning.",
                        "Corrigido o bug de enviar email do cadastro aprovado. Vendedores que tiveram seu cadastro reprovados, recebiam esse email."
                    }),

                new VersionEntry(
                    2, 37, 15, "Melhorado retorno dos códigos e mensagens  recebido pelo correios", new DateTime(2019, 12, 06),
                    new[]
                    {
                        "Agora quando existe alguma mensagem, do correios, diferente de sucesso, isso é mostrado para o comprador e caso não seja possível fazer o envio do correios, a aplicação mostra o erro.",
                        "Corrigido problemas no envio do Email OrderCompletedPaymentMessageToSeller",
                        "Corrigido o problema de inserção de tags html no arquivo de tradução.",
                        "Os titles prestadores podiam aparecer vazios na tela de pesquisa pois usavasse somente o AlternativeName para preenchelo, agora quando o AlternativeName é null, é preenchido com o FullName."
                    }),

                new VersionEntry(
                    2, 37, 14, "Correções e Melhorias", new DateTime(2019, 12, 06),
                    new[]
                    {
                        "product > businessModel > rental > enforceNoOverlapingInCart (booleano) (padrão: true)",
                        "Configuração para habilitar/desabilitar a funcionalidade de não permitir períodos conflitantes em todos os produtos do carrinho.",
                        "Caso a configuração for desabilitada, o sistema não irá permitir períodos conflitantes somente quando for um mesmo produto."
                    }),

                new VersionEntry(
                    2, 37, 13, "Correções e Melhorias", new DateTime(2019, 12, 04),
                    new[]
                    {
                        "Corrigido problema nas logos nos Web Apps."
                    }),

                new VersionEntry(
                    2, 37, 12, "Correções e Melhorias", new DateTime(2019, 12, 03),
                    new[]
                    {
                        "Arrumado o banner que não rodava automaticamente antes do primeiro clique nele.",
                        "As logos estavam sendo cropadas nos emails, isso gerava logos cortadas. Desse modo, tiramos o crop das logos nos emails.",
                        "Arrumado arquivo de autocomplete do googlemaps que não setava o bounds no autoComplete.",
                        "Ajeitado problema em setar nome do produto quando for abrir o carrinho.",
                        "Problemas na busca de produtos nos searchFilters.",
                        "Tabela no carrinho arrumada (estava com linhas e colunas desalinhadas.)",
                        "Resolvido problema dos links no email OrderProcessingPaymentMessageToBuyer."
                    }),

                new VersionEntry(
                    2, 37, 11, "Nova engine de markdown com suporte a inclusão de vídeos", new DateTime(2019, 12, 03),
                    new[]
                    {
                        "Troca da engine de Markdown para [Markdig](https://github.com/lunet-io/markdig), que tem melhor suporte para a especificação",
                        "Suporta inclusão de vídeos do YouTube, Vimeo e outros, conforme [documentação](https://github.com/lunet-io/markdig/blob/master/src/Markdig.Tests/Specs/MediaSpecs.md)"
                    }),

                new VersionEntry(
                    2, 37, 10, "Otimização na busca das grades de agendamento", new DateTime(2019, 11, 28),
                    new[]
                    {
                        "Ajuste no carregamento das grades de agendamento"
                    }),

                new VersionEntry(
                    2, 37, 9, "Correções e melhorias", new DateTime(2019, 11, 28),
                    new[]
                    {
                        "Resolvido problema do link do blog não aparecer no menu moblie.",
                        "Arrumado a formatação dos valores de numéricos no excel dos assinantes (admins/subscriptions).",
                        "Quando um vendedor se cadastrava na plataforma, os campos customfields que eram obrigatórios não estvam validando corretamente. Além disso, esses campos, não estavam apontando a falto do preenchimento."
                    }),

                new VersionEntry(
                    2, 37, 8, "Correções e melhorias", new DateTime(2019, 11, 27),
                    new[]
                    {
                        "Corrigido o problema de mostrar cadastro como completos, sendo que estão incompletos.",
                        "Resolvido a incongruência no número de Vendedores Ativos. Adicionado verificação da config de moderação na busca de vendedores ativo/moderados e compradores ativos/moderados.",
                        "Problema das repostas dos orçamentos resolvido. Quando o prestador de serviços repondia mais de uma vez o pedido de orçamento, só aparecia a primeira resposta feita."
                    }),

                new VersionEntry(
                    2, 37, 7, "Redirecionamento após login conforme config", new DateTime(2019, 11, 26),
                    new[]
                    {
                        "Ao fazer login, usuários compradores e vendedores passam a ser redirecionados conforme configuração buyer.mainPageUrl e seller.mainPageUrl"
                    }),

                new VersionEntry(
                    2, 37, 6, "Correções e Melhorias", new DateTime(2019, 11, 26),
                    new[]
                    {
                        "Mudado a forma de moderação dos cadastros de vendedores, agora quando aprovamos o cadastro, ele fica ativo e quando reprovamos, ele fica inativo"
                    }),

                new VersionEntry(
                    2, 37, 5, "Hotfix transferência pgto garantia Pagarme", new DateTime(2019, 11, 26),
                    new[]
                    {
                        "Configuração config.payment.pagarme.transferCost agora especifica o custo de transferência cobrado pelo Pagarme",
                        "Este valor é especificado em reais (3.67 no padrão), e é descontado do valor da transferência na job recorrente de transferências de pagamento em garantia",
                        "Se não descontar este valor na tentativa de transferência, o Pagarme retorna erro de saldo insuficiente",
                        "Mudado a forma de moderação dos cadastros de vendedores, agora quando aprovamos o cadastro, ele fica ativo e quando reprovamos, ele fica inativo."
                    }),

                new VersionEntry(
                    2, 37, 4, "Hotfix", new DateTime(2019, 11, 25),
                    new[]
                    {
                        "Job recorrente de transferência de valores: continua o processamento de todas as transferências mesmo se alguma der errado",
                        "Só registra erro no Hangfire (através de exceção) se erro não for falta de fundos para transferência"
                    }),

                new VersionEntry(
                    2, 37, 3, "Correções e Melhorias", new DateTime(2019, 11, 25),
                    new[]
                    {
                        "Adicionado campo de pacote de créditos na view do carrinho caso o produto seja um PackageCredit",
                        "Nova Label criada CART_ITEM_PACKEGE_CREDIT_LABEL",
                        "Alterado o ProductAppService para uma busca de produtos mais assertiva.",
                        "Corrigido problema em que os vendedores desativados estavam na aba de aguardando moderação, ajustado para que fossem para tela de desativados."
                    }),

                new VersionEntry(
                    2, 37, 2, "Ajuste preço do produto com duração fixa + Correções e Melhorias", new DateTime(2019, 11, 22),
                    new[]
                    {
                        "Ajuste no cálculo de preço do produto com duração fixa",
                        "Ajuste no cadastro do produto, quando configuração de duração fixa está habilitada",
                        "Ajuste na exibição de período de duração fixa na tela de detalhes do produto",
                        "Cod de banco Adicionado 290 - PagSeguro + alteração banco cod 41.",
                        "Implementado Controller responsável por: receber os dados (Id do produto/Id customizável, startDate e endDate ) de trava da agenda e adicioná-los na agenda do marketplace."
                    }),

                new VersionEntry(
                    2, 37, 1, "Correções e melhorias", new DateTime(2019, 11, 21),
                    new[]
                    {
                        "Adicionado variável, links de produtos, no template de email OrderProcessingPaymentMessageModel.",
                        "Removido tag html usada sem necessidade.",
                        "Criado Api para recebimento de bloqueios na agenda do vendedor.",
                        "A job responsável por calcular os preços (CalculatePriceOfProductsVersionOutJob), não pegava produtos que tinha o preço igual a 0."
                    }),

                new VersionEntry(
                    2, 37, 0, "Moderação de respostas de orçamento", new DateTime(2019, 11, 20),
                    new[]
                    {
                        "quote > moderation > enabled (booleano) (padrão: false)",
                        "Configuração para habilitar/desabilitar a funcionalidade de moderar respostas nos orçamentos. O administrador da plataforma poderá enviar mensagens dentro do orçamento",
                        "NewModeratorQuoteAnswerMessageToTarget: Novo template de email de mensagem do moderador no pedido de orçamento para vendedor e comprador",
                        "layout > images > moderatorAvatar (string) (padrão: '/assets/images/user.png')",
                        "Configuração da imagem padrão do avatar do moderador",
                        "quote > phoneForRegister > enabled (booleano) (padrão: false)",
                        "quote > phoneForRegister > mandatory (booleano) (padrão: false)",
                        "Configuração para habilitar/desabilitar, e deixar obrigatório ou não, o campo de Telefone no registro do usuário no pedido de orçamento.",
                        "Novo campo Telefone no registro do usuário no pedido de orçamento"
                    }),

                new VersionEntry(
                    2, 36, 0, "Página específica para item de portfólio", new DateTime(2019, 11, 20),
                    new[]
                    {
                        "portfolio > showOwnPage (booleano) (padrão: false)",
                        "Configuração para habilitar/desabilitar a funcionalidade do comprador poder abrir uma página específica de exibição de um item de portfólio do vendedor",
                        "Novo campo para armazenar url amigável do item do portfólio",
                        "Rotina para gerar a url amigável dos itens de portfólio que ainda não tem",
                        "seller > changeAvatar (booleano) (padrão: true)",
                        "Configuração para habilitar/desabilitar a funcionalidade de alteração do avatar do vendedor",
                        "Cálculo de preço para exibição nas listas",
                        "Correção de exibição do cálculo de preço do produto"
                    }),

                new VersionEntry(
                    2, 35, 3, "Correções e melhorias", new DateTime(2019, 11, 19),
                    new[]
                    {
                        "Criado nova label resposável por mostrar que os dados (comprador) estão ocultos",
                        "Resolvido problema de mostrar os dados do comprador depois de confirmar a venda, mas o pagamento não ter sido efetuado (para pagamentos para fora da plataforma, irá mostrar os dados)",
                        "Adicionado a config.search.requiredLogin na busca de vendedores, caso o cliente deseje que para a busca, seja necessário estar logado.",
                        "O vendedor agora pode ver os dados dos campos customizáveis caso eles estejam públicos.",
                        "Adicionado links no html avancado e na parte de notificação, para solucionar dúvidas sobre essas funcionalidades."
                    }),

                new VersionEntry(
                    2, 35, 2, "Hotfix", new DateTime(2019, 11, 25),
                    new[]
                    {
                        "Job recorrente de transferência de valores: continua o processamento de todas as transferências mesmo se alguma der errado",
                        "Só registra erro no Hangfire (através de exceção) se erro não for falta de fundos para transferência"
                    }),

                new VersionEntry(
                    2, 35, 1, "Correções e melhorias", new DateTime(2019, 11, 13),
                    new[]
                    {
                        "Nova configuração permite definir qual será a página inicial do Comprador e Vendedor",
                        "Configs em buyer.mainPageUrl e seller.mainPageUrl",
                        "O cálculo de parcelas na view _ProductMainBord estava incorreto e foi ajustado",
                        "Resolvido o problema no if que verificava a config.buyer.mainPageUrl"
                    }),

                new VersionEntry(
                    2, 35, 0, "Locação com duração em minutos e centralização do cálculo de preço do produto", new DateTime(2019, 11, 12),
                    new[]
                    {
                        "product > businessModel > rental > fixedPeriodRental > registerByBasePrice (booleano) (padrão: false)",
                        "Configuração para habilitar/desabilitar a funcionalidade de informar o preço, no cadastro do produto, referente ao total da duração fixa",
                        "Nova unidade de locação de produtos: Minutos",
                        "Ajuste na grade de horários para produtos para unidade de locação em minutos",
                        "Centralização do cálculo de preço do produto para exibição nas telas do marketplace",
                        "Exibição do preço do produto a partir dos dados já calculados previamente, ao salvar o produto",
                        "Rotinas para manter o cálculo de preço dos produtos atualizado, quando é gerada uma nova versão de cálculo",
                        "Rotinas para manter o cálculo de preço dos produtos atualizado, quando é alterada alguma configuração no marketplace que possa modificar o cálculo",
                        "Melhorias de desempenho ao exibir o preço do produto, não necessitando fazer o cálculo sob demanda"
                    }),

                new VersionEntry(
                    2, 34, 12, "Correções e Melhorias", new DateTime(2019, 11, 12),
                    new[]
                    {
                        "Criado cod do Banco 085 (Cecredi) para cadastro de bancos do seller",
                        "Alterado na view _ProductInfos o campo do código para que o seller possa escolher no arquivo de tradução a opção desejada",
                        "Correção da renderização da logo nos templates de email, através da variável [APP_LOGO]. Tamanho limitado em 60px",
                        "Implementado o envio de email para vendedor, quando o serviço está para começar e terminar.",
                        "Implementado customização do redirecionamento do botão minha conta (config.Buyer.MainPageUrl e config.Seller.MainPageUrl)"
                    }),

                new VersionEntry(
                    2, 34, 11, "Correções e Melhorias", new DateTime(2019, 11, 11),
                    new[]
                    {
                        "Criado cod do Banco 085 (Cecredi) para cadastro de bancos do seller.",
                        "Alterado na view _ProductInfos o campo do código para que o seller possa escolher no arquivo de tradução a opção desejada."
                    }),

                new VersionEntry(
                    2, 34, 10, "Correção na hora de mostrar a data do saque para o seller e o tipo do Frete (PAC Sedex)", new DateTime(2019, 11, 08),
                    new[]
                    {
                        "Quando o o seller iria var a carterira, existe uma model na parte abaixo das ações da page. Porém, a data vinha com formato errado, exemplo: 32/9/2019",
                        "Havia uma problema em que era trocado o cálculo do frete entre SEDEX e PAC"
                    }),

                new VersionEntry(
                    2, 34, 9, "Melhoria dos _SearchFilters", new DateTime(2019, 11, 06),
                    new[]
                    {
                        "Quando havia só um campo de filtro de busca, ele fica desclocado na view e por isso acaba não ficando centralizado",
                        "Mais centralizado porém ainda falta centralizar caso haja somente um botão",
                        "Pré-compilação da view de _SearchFields com o RazorGenerator"
                    }),

                new VersionEntry(
                    2, 34, 8, "Melhorado os botões de filtros na view _SearchFilters", new DateTime(2019, 11, 05),
                    new[]
                    {
                        "Quando havia só um botão de filtro, ele fica desclocado na view e por isso acaba não ficando centralizado",
                        "O campo de carrinhos abandonados no dashboard do admin, mostrava um valor diferente do que estava na página de gerenciamento dos carrinhos abandonados",
                        "Alterado as queries que faziam o count e as buscas dos carrinhos abandonados",
                        "Também foi alterado uma query que criava temp tables sem necessidade",
                    }),

                new VersionEntry(
                    2, 34, 7, "Arrumado as queries, de carrinhos abandonados, fazendo count com incongruência", new DateTime(2019, 11, 05),
                    new[]
                    {
                        "O campo de carrinhos abandonados no dashboard do admin, mostrava um valor diferenta do que estava na página de gerenciamento dos carrinhos abandonados",
                        "Alterado as queries que faziam o count e as buscas dos carrinhos abandonados",
                        "Também foi alterado uma query que criava temp tables sem necessidade"
                    }),

                new VersionEntry(
                    2, 34, 6, "Revisão do pagamento em garantia com Pagarme", new DateTime(2019, 11, 04),
                    new[]
                    {
                        "Revisão do processo de pagamento em garantia com o Pagarme",
                        "No caso do Pagarme, é feito split e o dinheiro já fica disponível para o vendedor",
                        "Saque automático deve estar desabilitado, sendo completamente gerenciado pela plataforma",
                        "Transferência (saque) fica por conta de um job recorrente, que verifica pedidos concluídos, que signficam repasse autorizado",
                        "Documentação interna detalhada disponível no Nuclino",
                        "Hotfix da compilação da view Views/Products/MySales/_SaleDetails.cshtml"
                    }),

                new VersionEntry(
                    2, 34, 5, "Correções e melhorias", new DateTime(2019, 10, 30),
                    new[]
                    {
                        "Correção da renderização da logo da home, que estava forçando crop e scale",
                        "Identificadores CSS adicionados na página de login para facilitar customização por HTML avançado"
                    }),

                new VersionEntry(
                    2, 34, 4, "Correções e melhorias", new DateTime(2019, 10, 29),
                    new[]
                    {
                        "Menor tamanho da logo que é inclusa nos emails pela tag [APP_LOGO]",
                        "Correção na validação de locações sobrepostas no carrinho avançando para o checkout",
                        "Otimização na validação de estoque e frete do carrinho, reduzindo o número de chamadas ao banco de dados",
                        "Melhor formatação de data e hora para não mostrar os segundos (irrelevantes para o contexto da plataforma)"
                    }),

                new VersionEntry(
                    2, 34, 3, "Correções e melhorias", new DateTime(2019, 10, 28),
                    new[]
                    {
                        "Friendly URL passa a usar sufixo ao invés de prefixo quando precisa desviar colisões",
                        "Correção nos envios de email das notificações de locação prestes a iniciar e prestes a terminar"
                    }),

                new VersionEntry(
                    2, 34, 2, "Correções e melhorias", new DateTime(2019, 10, 24),
                    new[]
                    {
                        "Configuração para restringir visualização da agenda do vendedor a períodos menores que 24h (ex. das 8 às 18h): config.product.rental.seller.schedule.configuration.minTime|maxTime",
                        "Mostrar nome do comprador/vendedor como autor da última resposta no pedido de orçamento",
                        "Variáveis condicionais no template de email. A primeira é [IF:IS_OFFLINE_PAYMENT] no email de Novo Pedido para o vendedor"
                    }),

                new VersionEntry(
                    2, 34, 1, "Novo template de email", new DateTime(2019, 10, 23),
                    new[]
                    {
                        "Novo template de email para o quando o comprador tem seu perfil aprovado pelo moderador"
                    }),

                new VersionEntry(
                    2, 34, 0, "Nome comercial codificado e melhorias", new DateTime(2019, 10, 18),
                    new[]
                    {
                        "config > seller > alternativeName > generateEncoded - (booleano) (padrão: false)",
                        "Configuração para habilitar/desabilitar a funcionalidade de geração automática de nome comercial codificado para o vendedor",
                        "config > seller > alternativeName > encodedTemplate - (string) (padrão: Empresa {0})",
                        "Template para o nome comercial codificado do vendedor",
                        "Melhoria na geração de nickname e friendlyUrl para o vendedor"
                    }),

                new VersionEntry(
                    2, 33, 0, "Moderação de comprador e melhorias", new DateTime(2019, 10, 15),
                    new[]
                    {
                        "config > buyer > moderation > enabled - (booleano) (padrão: false)",
                        "Configuração para habilitar/desabilitar a funcionalidade de moderação de comprador",
                        "Exibição de mensagem de perfil invisível para o comprador, quando ainda não foi aprovado pela moderação",
                        "Bloqueio no pedido de orçamento, quando o comprador ainda não foi aprovado pela moderação",
                        "Melhoria no disparo de eventos para invalidação de cache dos dados do comprador",
                        "Melhoria no disparo de eventos para invalidação de cache dos dados do usuário logado",
                        "Melhoria no disparo de eventos para reindexação dos dados do vendedor",
                        "Ajuste no login com senha no pedido de orçamento, quando o usuário não está logado"
                    }),

                new VersionEntry(
                    2, 32, 19, "Correções e melhorias", new DateTime(2019, 10, 21),
                    new[]
                    {
                        "Na mensagem de completar perfil, link do cadastro de portfólio agora leva para o cadastro propriamente dito",
                        "Nos detalhes do grupo de orçamento para o comprador, mostra sempre a última proposta (preço e prazo) de cada vendedor",
                        "Revisão da seção de status nos detalhes do pedido, visão do vendedor",
                        "Orçamentos em aberto passa a ordenar pelos mais recentes por padrão",
                        "Avaliação do vendedor passa a mostrar Nome Alternativo (se houver), usando Nome Completo como fallback"
                    }),

                new VersionEntry(
                    2, 32, 18, "Correções e melhorias", new DateTime(2019, 10, 15),
                    new[]
                    {
                        "Cópia do email de novas vendas para os admins da plataforma passa a vir ligado por padrão",
                        "Correção de ocorrências de \"extornar\" para \"estornar\" no arquivo de tradução",
                        "Controle de finalização é do vendedor para pedidos feitos antes de ativar o pagamento em garantia na plataforma",
                        "Mostra tipo de frete do pedido no painel admin, sempre (antes só mostrava se houvesse entrega física, ex. PAC, Sedex ou frota privada)",
                        "Mostra vendedores sem avaliações na Home Best Sellers mesmo quando avaliações estão ativas"
                    }),

                new VersionEntry(
                    2, 32, 17, "Correções e melhorias", new DateTime(2019, 10, 14),
                    new[]
                    {
                        "Ordem das mensagens de perfil invisível do vendedor: 1 categorias, 2 portfólio, 3 dados bancários, 4 assinatura, 5 moderação",
                        "Ícone de quantidade no pedido de orçamento trocado de uma balança para cubos empilhados",
                        "Campo quantidade no pedido de orçamento oculto quando flag desabilitada",
                        "Flags de configuração para definir as formas disponíveis para o vendedor configurar bloqueios na sua agenda, em config.product.businessModel.rental.seller.schedule.configuration"
                    }),

                new VersionEntry(
                    2, 32, 16, "Hotfix", new DateTime(2019, 10, 14),
                    new[]
                    {
                        "Corrigido problema ao calcular os horários disponíveis de produtos com duração fixa"
                    }),

                new VersionEntry(
                    2, 32, 15, "Hotfix", new DateTime(2019, 10, 14),
                    new[]
                    {
                        "Quantidade de agendamento e reagendamento de locação não era reconhecida"
                    }),

                new VersionEntry(
                    2, 32, 14, "Melhorias e correções gerais", new DateTime(2019, 10, 11),
                    new[]
                    {
                        "Ocultar filtros da busca de produtos no mobile, com opção para expandir",
                        "Informações sobre agendamento de produto do tipo pacote foram movidas para seção especial ao final da página: políticas de agendamento",
                        "Configuração para restringir visualização da agenda do comprador a períodos menores que 24h (ex. das 8 às 18h): config.product.rental.buyer.schedule.minTime|maxTime",
                        "Correção bug que não mostrava mais filtros da busca de produtos",
                        "Mais elementos da página de anúncio do produto foram identificados com classes CSS para customização"
                    }),

                new VersionEntry(
                    2, 32, 13, "Hotfix", new DateTime(2019, 10, 10),
                    new[]
                    {
                        "Botão \"Avalie este comprador\" aparecia na tela do comprador"
                    }),

                new VersionEntry(
                    2, 32, 12, "Melhores vendedores da home", new DateTime(2019, 10, 10),
                    new[]
                    {
                        "Revisão do motor de busca de vendedores, para dar suporte à ordenação por score",
                        "Score é a some de qtde de produtos, qtde de orçamentos, qtde de produtos, qtde de itens de portfólio, avaliação média * 1000"
                    }),

                new VersionEntry(
                    2, 32, 11, "Correções e melhorias gerais", new DateTime(2019, 10, 09),
                    new[]
                    {
                        "Alteração do mínimo de 2 créditos para 1 na criação de um produto do tipo Pacote de Créditos",
                        "Orçamento geral: validação de obrigatoriedade do campo de filtro por localidade (google autocomplete)",
                        "Cópia do email de nova venda para os admins da plataforma, se habilitado em config.Products.Order.SendOrderCopyToAdmins",
                        "Acompanhamento de saques do vendedor"
                    }),

                new VersionEntry(
                    2, 32, 10, "Melhoria na tela Configuração de disponibilidade", new DateTime(2019, 10, 08),
                    new[]
                    {
                        "Melhoria de usabilidade na tela de configuração de disponibilidade de agendamento",
                        "Agora, ao alterar os produtos no dropdown ou voltando para a opção Para toda a loja, os horários de disponibilidade são carregados automaticamente"
                    }),

                new VersionEntry(
                    2, 32, 9, "Campo para informações adicionais no pedido", new DateTime(2019, 10, 08),
                    new[]
                    {
                        "Adiciona campo texto livre no carrinho de compras para especificar informações ou instruções extras do pedido",
                        "Campo não é habilitado por padrão. Necessário ativar em config.products.order.extraInfo.enabled",
                        "Pode ser configurado como obrigatório",
                        "Informação aparece formatada em markdown nos detalhes do pedido para todos os perfis (comprador, vendedor, admin)",
                        "Correção dos filtros da lista de carrinhos abandonados no admin, que estava redirecionando de volta para dashboard/index"
                    }),

                new VersionEntry(
                    2, 32, 8, "Correções e melhorias gerais", new DateTime(2019, 10, 07),
                    new[]
                    {
                        "Correção do cadastro de URLs de redes sociais para o rodapé - não transforma mais em lowercase (YouTube é case sensitive)",
                        "Alteração do modo dos bloqueios criados pelo comando de definir disponibilidade (agenda do vendedor) de trava parcial para trava total",
                        "Melhora a mensagem de erro de saque na Wirecard"
                    }),

                new VersionEntry(
                    2, 32, 7, "Revisão fluxo pagamento em garantia", new DateTime(2019, 10, 07),
                    new[]
                    {
                        "Revisão da tela de disputa, reorganizando as informações",
                        "Revisão do fluxo de pagamento em garantia",
                        "Revisão do layout da tela de perfil público do vendedor",
                        "Revisão do layout das avaliações no perfil público do vendedor"
                    }),

                new VersionEntry(
                    2, 32, 6, "Correções e melhorias", new DateTime(2019, 10, 03),
                    new[]
                    {
                        "Nos orçamentos em grupo, visão do comprador, mostra qtde total de vendedores que receberam o orçamento",
                        "Na lista de orçamentos, oculta campos adicionais à descrição, com um link para abrir os detalhes - melhor visualização",
                        "Revisões menores de espaçamento, tamanho de fonte e relevância de outras informações na lista de orçamentos - melhor visualização"
                    }),

                new VersionEntry(
                    2, 32, 5, "Correções e melhorias", new DateTime(2019, 10, 02),
                    new[]
                    {
                        "Funcionalidade de Configurar disponibilidade na agenda exibe os horários de disponibilidade para edição",
                        "Ajuste na exibição de Quantide e Item do portfólio nos detalhes do orçamento",
                        "Centralização do botão Pedir orçamento na modal do item do portfólio",
                        "Título padrão para o pedido de orçamento, quando for para um item do portfólio",
                        "Formulário 'completo' para pedido de orçamento, quando for para um item do portfólio",
                        "Corrigido título Seus Dados que aparecia quando os seus campos estavam desabilitados no pedido de orçamento",
                        "config > quote > specificQuote > quantity > enabled - (booleano) (padrão: true)",
                        "config > quote > specificQuote > quantity > mandatory - (booleano) (padrão: true)",
                        "Configuração para habilitar/desabilitar o campo Quantidade na tela de pedido de orçamento para um item do portfólio"
                    }),

                new VersionEntry(
                    2, 32, 4, "Template de email de boas vindas separado para vendedores", new DateTime(2019, 10, 02),
                    new[]
                    {
                        "Template de boas vindas era o mesmo para compradores e vendedores, agora há dois emails separados",
                        "Para plataformas existentes, o email do vendedor começa copiando o de comprador, assim mantém-se a configuração atual",
                        "Para customizar, entrar nos templates de email e localizar o email de boas vindas específico para vendedores",
                        "Melhores mensagens de erro de pagamento na integração Wirecard"
                    }),

                new VersionEntry(
                    2, 32, 3, "Evitar bypass no pedido de orçamento", new DateTime(2019, 10, 01),
                    new[]
                    {
                        "Não mostra os vendedores num pedido de orçamento geral até que eles respondam",
                        "A função é ativada ao desligar a flag config.quote.generalQuote.showSellersWithoutAnswer, que vem ligada por padrão",
                        "Orçamentos direcionados para um vendedor específico continuam funcionando como antes",
                        "Admin > Vendedor > Produtos: só mostra botão de moderação se config.Product.CanPauseProducts estiver habilitado"
                    }),

                new VersionEntry(
                    2, 32, 2, "Hotfix da edição de dados bancários", new DateTime(2019, 10, 01),
                    new[]
                    {
                        "Revisão das mensagens de registro e alteração de conta no gateway",
                        "Respiro entre mensagens de perfil invisível"
                    }),

                new VersionEntry(
                    2, 32, 1, "Editar dados bancários para recebimento do vendedor", new DateTime(2019, 09, 30),
                    new[]
                    {
                        "Separação das telas de dados gerais (minha conta) e dados para recebimento",
                        "Permite alterar os dados para recebimento nos gateways Pagarme e Wirecard",
                        "Alteração é condicional: nova conta deve pertencer ao mesmo CPF/CNPJ ou vendedor não pode ter saldo em conta (criação de nova conta)",
                        "Melhoria visual nas páginas internas, com melhor contraste: cinza mais claro, menu em branco",
                        "Mensagens de perfil invisível agrupadas em um único elemento",
                        "Customização de busca do ex-cliente ImHealth removida definitivamente"
                    }),

                new VersionEntry(
                    2, 32, 0, "Formulário de orçamento reduzido e orçamento com item de portfólio", new DateTime(2019, 09, 24),
                    new[]
                    {
                        "config > quote > specificQuote > shortQuote > enabled - (booleano) (padrão: false)",
                        "Habilita formulário reduzido para pedido de orçamento para um vendedor específico",
                        "config > quote > portfolioItemQuote > enabled - (booleano) (padrão: false)",
                        "Habilita opção parapara pedido de orçamento para um item de portfólio de um vendedor",
                        "Corrigida busca de vendedores por localização, no pedido de orçamento, que aparecia quando o orçamento era para um vendedor específico",
                        "Corrigida listagem de orçamentos individuais que não apareciam se existissem orçamentos em grupo para o comprador"
                    }),

                new VersionEntry(
                    2, 31, 15, "Hotfix avaliação", new DateTime(2019, 09, 26),
                    new[]
                    {
                        "Mostra corretamente a opção para avaliar a contraparte nos detalhes do pedido quando ainda está dentro do prazo"
                    }),

                new VersionEntry(
                    2, 31, 14, "Correções e melhorias", new DateTime(2019, 09, 24),
                    new[]
                    {
                        "Detalhes do pedido ref a pagamento de orçamento agora mostra link para orçamento em questão na tela do vendedor",
                        "Melhor organização das informações nos detalhes do pedido para o vendedor",
                        "Corrige bug que não mostrava nome do cliente nos comentários de orçamento após aceite da proposta",
                        "Corrige bug de validação de campos de data com máscara no mobile",
                        "Mostra cidade/UF na lista de vendedores para moderação",
                        "Corrige imagens no carrossel de melhores vendedores da home",
                        "Configuração para habilitar/desabilitar o campo de Valor Mínimo para Locação no módulo de locação e agendamento"
                    }),

                new VersionEntry(
                    2, 31, 13, "Correções e melhorias", new DateTime(2019, 09, 23),
                    new[]
                    {
                        "Corrige input de dados formatados por máscara, trazendo de volta versão anterior do plugin (a versão mais recente está com problema)",
                        "Corrige controle de conclusão do pedido e abertura de disputa no pagamento em garantia; poder é do comprador, estava invertido (poder do vendedor)"
                    }),

                new VersionEntry(
                    2, 31, 12, "Correções e melhorias", new DateTime(2019, 09, 20),
                    new[]
                    {
                        "Corrige agrupamento de orçamentos para o comprador",
                        "Corrige ordenação por orçamentos mais recentes na página de orçamentos em aberto",
                        "Mostra forma de pagamento nos detalhes do pedido",
                        "Mostra valor líquido a receber nos detalhes do pedido para o vendedor"
                    }),

                new VersionEntry(
                    2, 31, 11, "Template de email para Retirada no Local", new DateTime(2019, 09, 20),
                    new[]
                    {
                        "Novo template de email para Comprador e Vendedor quando o método de entrega é Retirada no Local"
                    }),

                new VersionEntry(
                    2, 31, 10, "Correções e melhorias gerais", new DateTime(2019, 09, 20),
                    new[]
                    {
                        "Corrigida validação e registro de novos agendamentos para produtos do tipo Pacote de créditos comprados em mais de uma unidade",
                        "Ajuste no parâmetro de ReturnUrl para todos os fluxos de login e cadastro de vendedores e compradores",
                        "Botão de Adicionar aos favoritos para produtos de vitrine ecommerce na página inicial",
                        "Botão de Adicionar aos favoritos ativo mesmo se usuário estiver deslogado. Ao clicar, o usuário é redirecionado para o login e o produto é adicionado aos favoritos",
                        "Tooltip para o botão de Adicionar aos favoritos no cards de produtos",
                        "Corrigida hora inicial da grade de horários de produto",
                        "Ajuste de UX na grade de horários quando o produto for do tipo Pacote de créditos",
                        "Nova tela para configuração de disponibilidade de locação, para toda a loja ou por produto"
                    }),

                new VersionEntry(
                    2, 31, 9, "Correções e melhorias", new DateTime(2019, 09, 19),
                    new[]
                    {
                        "Revisão da seção Melhores Vendedores da home, aproveitando o motor de busca - corrige ordenação levando em conta melhores avaliados",
                        "Revisão da query que lista avaliações dos usuários, com melhor performance, e agregada à seção Melhores Vendedores",
                        "Desabilita o datepicker para campos de data de nascimento no cadastro de comprador e vendedor",
                        "Oculta subtotal no carrinho e checkout se frete também for ocultado"
                    }),

                new VersionEntry(
                    2, 31, 8, "Revisão do fluxo de agendamento e locação, e fluxo de avaliação", new DateTime(2019, 09, 19),
                    new[]
                    {
                        "Oculta informações de frete quando comprador está pagando orçamento, também quando o frete é ActualNoShipping",
                        "Ajustes menores em templates de email, melhorando visualização e nomes de links",
                        "Corrigido o link da notificação mobile de pagamento confirmado para o vendedor",
                        "Vendedor agora pode marcar um pedido de agendamento como concluído após seu pagamento",
                        "Fluxo de avaliação revisado e funcional",
                        "Telas admin da avaliação de comprador e vendedor foram revisadas com ajustes de layout",
                        "Telas de avaliação de comprador e vendedor foram revisadas com ajustes de layout",
                        "Garante que o dinheiro só será pago ao vendedor se o pedido estiver em status concluído",
                        "Avaliação aparece para comprador e vendedor somente após pedido concluído",
                        "Nos detalhes do pedido para comprador, quando pagamento foi liberado, mostra data de liberação (mesma data do pedido alterado para concluído)",
                        "Nos detalhes do pedido para vendedor, quando é orçamento com pgto em garantia, mostra mensagem explicando porque vendedor não tem opção para concluir pedido (controle é do comprador)",
                        "Melhorias de layout nos detalhes do pedido para comprador e vendedor"
                    }),

                new VersionEntry(
                    2, 31, 7, "Revisão de emails transacionais", new DateTime(2019, 09, 13),
                    new[]
                    {
                        "Template de email em padrão de design mais moderno",
                        "Revisão dos textos de email (afeta somente novas plataformas)",
                        "Correção do botão para salvar alterações no HTML customização, que não estava sendo habilitado",
                        "API de lista de vendedores aceita filtro por gateway_id"
                    }),

                new VersionEntry(
                    2, 31, 6, "Correção prazo liberação automática do pagamento em garantia", new DateTime(2019, 09, 12),
                    new[]
                    {
                        "Correção layout botões (+) e (-) para gerenciar as variantes do produto",
                        "Ordenação das listas (produtos, orçamentos, créditos, avaliações) nos detalhes do vendedor - painel admin",
                        "Correção para datepicker no mobile",
                        "Correção do cálculo de prazo para liberação automática do prazo em garantia"
                    }),

                new VersionEntry(
                    2, 31, 5, "Hotfix saque conta Moip", new DateTime(2019, 09, 11),
                    new[]
                    {
                        "O saque agora referencia a conta bancária já cadastrada em nome do vendedor, ao invés de tentar criar uma nova toda vez"
                    }),

                new VersionEntry(
                    2, 31, 4, "Correções e melhorias gerais", new DateTime(2019, 09, 10),
                    new[]
                    {
                        "Desabilitados os campos do cadastro em gateway de pagamentos, que precisa ser refeito (acompanhando a refação do cadastro no gateway para UI)",
                        "Api de consulta de vendedores passa a aceitar campo document, que filtra por documento CPF ou CNPJ do cadastro",
                        "Fechamento e cancelamento de orçamentos pelo comprador agora geram evento para atualizar busca de orçamentos em aberto",
                        "Correção na validação de URL de perfil linkedin e facebook, no cadastro de vendedor",
                        "Correção de texto no botão de busca da home, para fazer quebra de linha em textos grandes",
                        "Correção da busca de orçamentos para listar para o comprador em Meus Orçamentos",
                        "Correção do fluxo de pedidos de orçamento sem vendedor, para listar todos os orçamentos do cliente (mesmo sem vendedor) com uma mensagem",
                        "Correção do fluxo de pedidos de orçamento sem vendedor, no admin, para listar o FullName do vendedor ao invés de AlternativeName, caso ele não tenha",
                        "Atualização da lista de bancos para o cadastro de vendedor (recebedor)"
                    }),

                new VersionEntry(
                    2, 31, 3, "Consolidação de versões", new DateTime(2019, 09, 06),
                    new[]
                    {
                        "Consolidação das versões menores de 2.30.x com as mais recentes em 2.31.x"
                    }),

                new VersionEntry(
                    2, 31, 2, "Verificação de estoque disponível quando grade de horários habilitada", new DateTime(2019, 08, 31),
                    new[]
                    {
                        "Quando a grade de horários estiver habilitada, somente serão exibidos os horários que tiverem estoque disponível",
                        "Regra vale também para reagendamento de locação"
                    }),

                new VersionEntry(
                    2, 31, 1, "Duração fixa quando grade de horários habilitada", new DateTime(2019, 08, 31),
                    new[]
                    {
                        "Quando a grade de horários estiver habilitada, é possível definir uma duração fixa da locação (maior que 1) no cadastro do produto",
                        "A exibição dos horários na grade obedecem o intervalo da duração fixa, a partir do primeiro horário disponível",
                        "Regra vale também para reagendamento de locação"
                    }),

                new VersionEntry(
                    2, 31, 0, "Produto Pacote de Créditos", new DateTime(2019, 08, 31),
                    new[]
                    {
                        "config > product > businessModel > rental > package > enabled - (booleano) (padrão: false)",
                        "Habilita a opção do vendedor poder vender produtos como Pacote de Créditos, indicando no cadastro quantas unidades compreendem o pacote",
                        "Comprador tem a possibilidade de fazer a compra desse produto, podendo agendar um horário para cada unidade do pacote, após a compra, na tela de detalhes do pedido",
                        "Outras funcionalidades trabalham em conjunto com essa, como o cancelamento de pedido via comprador e reagendamento de locação",
                        "Visualização do preço por unidade na tela de busca de produtos, produtos do vendedor e produtos relacionados, quando o produto é do tipo Pacote de Créditos"
                    }),

                new VersionEntry(
                    2, 30, 11, "Ajuste botão funcionalidade de favoritos", new DateTime(2019, 08, 31),
                    new[]
                    {
                        "Botão de adicionar o produto aos favoritos na tela de detalhes do produto está mais evidente"
                    }),

                new VersionEntry(
                    2, 30, 11, "Correções e melhorias", new DateTime(2019, 09, 05),
                    new[]
                    {
                        "Correção do link direto para tela de vendedores pendentes de moderação no menu do admin",
                        "Revisão do cálculo de preço público de produtos (para tela de anúncio, resultados da busca e home)",
                        "Correção: volta a mostrar o label de unidade de tempo de locação na tela de produtos quando duração é fixa (ex. 4 horas)",
                        "Correção no cálculo de preço público de produtos para locação em turnos na busca",
                        "Correção na formatação (CSS) do spot na home"
                    }),

                new VersionEntry(
                    2, 30, 10, "Ícones de redes sociais no rodapé", new DateTime(2019, 08, 30),
                    new[]
                    {
                        "Configuração enterprise > socialNetworks > redeSocial",
                        "Ao configurar uma URL (qualquer URL válida), um ícone da rede social aparece no rodapé",
                        "Opção de ícones coloridos (padrão), pretos ou brancos"
                    }),

                new VersionEntry(
                    2, 30, 9, "Correções e melhorias", new DateTime(2019, 08, 30),
                    new[]
                    {
                        "Melhor UX da tela de vendedores no admin - separados por ativos, desativados e aguardando moderação",
                        "Otimização da consulta ao banco de dados para montar a lista de vendedores aguardando moderação",
                        "Melhor editor de código HTML na customização avançada pelo admin (plugin CodeMirror)",
                        "Campo Lógica de Preço adicionado quando plataforma habilita multiplicador de preços, facilitando o preenchimento caso o produto não tenha multiplicador",
                        "Endpoint de API para consulta de dados de vendedores (GET /api/v1/sellers)",
                        "Correção de cálculo e apresentação de preço com desconto no modelo de aluguel",
                        "Correção na importação de produtos, ao selecionar códigos de vendeores pelos emails relacionados aos produtos"
                    }),

                new VersionEntry(
                    2, 30, 8, "Preço com desconto na venda de produtos", new DateTime(2019, 08, 22),
                    new[]
                    {
                        "Campo Preço com Desconto na configuração do produto",
                        "Se o desconto estiver preenchido, envidencia o desconto nos resultados da busca e anúncio do produto",
                        "Desconto é evidenciado com badge de 20% off (exemplo) e preço original riscado, como de costume em plataformas de e-commerce"
                    }),

                new VersionEntry(
                    2, 30, 7, "Correções gerais", new DateTime(2019, 08, 20),
                    new[]
                    {
                        "URLs da tag og:url (compartilhamento) passam a usar o protocolo HTTPS quando o site está configurado com HTTPS em environment.appUrl",
                        "Correção no cadastro de itens customizados para a vitrine da home (também conhecido como home do ecommerce)",
                        "Adicionado og:image:secure_url às meta tags para compartilhar imagens servidas por HTTPS",
                        "Adicionada página /cart/checkoutcomplete para usar como gatilho de conversão em campanhas de marketing",
                        "Correção no botão de cancelar todos os pedidos pedidos de orçamento do grupo, a partir do perfil do comprador"
                    }),

                new VersionEntry(
                    2, 30, 6, "Reagendamento de locação com certo tempo de antecedência", new DateTime(2019, 08, 16),
                    new[]
                    {
                        "config > product > businessModel > rental > buyer > changePeriod > enabled - (booleano) (padrão: false)",
                        "config > product > businessModel > rental > buyer > changePeriod > beforeInHours - (número inteiro) (padrão: 12)",
                        "Habilita a opção do comprador poder reagendar uma locação, verificando o tempo mínimo de antecedência para executar essa ação",
                        "Envia email para o comprador sobre o reagendamento da locação",
                        "Envia email para o vendedor sobre o reagendamento da locação"
                    }),

                new VersionEntry(
                    2, 30, 5, "Sincronização das versões 2.29.10 até 2.30.4", new DateTime(2019, 08, 16),
                    new[]
                    {
                        "Merge das versões"
                    }),

                new VersionEntry(
                    2, 30, 4, "Bloqueio, em lote, de horários na agenda do vendedor", new DateTime(2019, 08, 14),
                    new[]
                    {
                        "Opções de bloqueio em lote: Bloquear horários da noite, da madrugada e do final de semana"
                    }),

                new VersionEntry(
                    2, 30, 3, "Grade de seleção de períodos de locação", new DateTime(2019, 08, 14),
                    new[]
                    {
                        "config > product > businessModel > rental > gridView > enabled - (booleano) (padrão: false)",
                        "config > product > businessModel > rental > gridView > days - (número inteiro) (padrão: 5): Número de dias para exibir na grade",
                        "config > product > businessModel > rental > gridView > periods - (número inteiro) (padrão: -1, todos): Número de horários para exibir em cada dia na grade",
                        "Habilita a grade de seleção de horários na página de detalhes do produto"
                    }),

                new VersionEntry(
                    2, 30, 2, "Agenda de locações do comprador", new DateTime(2019, 08, 14),
                    new[]
                    {
                        "config > product > businessModel > rental > buyer > schedule > enabled - (booleano) (padrão: false)",
                        "Habilita a opção do comprador visualizar a agenda de locações que ele adquiriu"
                    }),

                new VersionEntry(
                    2, 30, 1, "Cancelamento de locação com certo tempo de antecedência", new DateTime(2019, 08, 14),
                    new[]
                    {
                        "config > product > businessModel > rental > buyer > cancel > enabled - (booleano) (padrão: false)",
                        "config > product > businessModel > rental > buyer > cancel > beforeInHours - (número inteiro) (padrão: 12)",
                        "Habilita a opção do comprador poder cancelar uma locação, verificando o tempo mínimo de antecedência para executar essa ação",
                        "Envia email para o comprador sobre o cancelamento da locação"
                    }),

                new VersionEntry(
                    2, 30, 0, "Produtos favoritos (lista de desejos)", new DateTime(2019, 08, 14),
                    new[]
                    {
                        "config > product > wishlist > enabled - (booleano) (padrão: false)",
                        "Habilita a opção do comprador poder favoritar produtos",
                        "Habilita tela de acesso no painel do comprador para a lista de produtos favoritados"
                    }),

                new VersionEntry(
                    2, 29, 10, "Correções gerais", new DateTime(2019, 08, 15),
                    new[]
                    {
                        "Marketplace de agendamento: opção de formatação de preço do produto por unidade (ex. R$ 100 por mês, total de R$ 300 (supondo config. mín de 3 meses))",
                        "Correção bug de ordenação por distância",
                        "Calendário na página de adicionar resposta ao pedido de orçamento",
                        "Removido preço cheio do produto",
                        "Correção do cálculo de parcelamento de produtos para locação com período mínimo",
                        "Correção do cálculo de período de agendamento para unidades de locação em dias e meses (horas estava ok)",
                        "Correção do horário de agendamento mostrado no carrinho, que convertia para UTC duas vezes e mostrava horário bem diferente do selecionado",
                        "Removido caracter (;) da home",
                        "Corrigido problema de visualição de preço com adicional 'por pessoal' no anúncio do produto em mobile (correção de CSS)",
                        "Pacote NullGuard.Fody adicionado a presentation para que seja restaurado pelo CLI do nuget",
                        "Correção no tratamento de tags UTM (UtmFilters)",
                        "Correção do cálculo de parcelamento na Wirecard quando configuração é para que comprador NÃO assuma taxas de antecipação e parcelamento",
                        "Correção autocomplete google maps no cadastro de vendedor por facebook"
                    }),

                new VersionEntry(
                    2, 29, 9, "Hotfix planos assinatura Pagarme", new DateTime(2019, 08, 13),
                    new[]
                    {
                        "Corrigido erro ao carregar planos de assinatura configurados no Dashboard do Pagarme"
                    }),

                new VersionEntry(
                    2, 29, 8, "Rastreamento de conversão de campanhas", new DateTime(2019, 08, 07),
                    new[]
                    {
                        "Registra parâmetros UTM em cookies quando o usuário visita o site",
                        "No momento da conversão de cadastro ou pedido de orçamento, registra os parâmetros associado-os ao usuário",
                        "Funciona para compradores e vendedores"
                    }),

                new VersionEntry(
                    2, 29, 7, "Hotfix orçamento sem vendedor", new DateTime(2019, 08, 01),
                    new[]
                    {
                        "Correção da listagem de orçamentos sem vendedor",
                        "Correção no registro de pedidos de orçamento em grupo, registros não eram agrupados corretamente, impactava no cadastro de custom fields",
                        "Correção na importação de produtos",
                        "Correção de bug ao cadastrar pedido de orçamento + customfields direcionado para vendedor",
                        "Criação de URL /quotes/requestconfirmed para que seja possível registrar conversões de orçamento por URL visitada"
                    }),

                new VersionEntry(
                    2, 29, 6, "Orçamento geral sem vendedor", new DateTime(2019, 07, 25),
                    new[]
                    {
                        "Chave de configuração para ativar este modelo: config.quotes.generalQuote.registerQuoteEvenWhenNoSellers (ligado por padrão)",
                        "Quanto ativado, o pedido de orçamento geral envia para sellerId = null no caso de não haver vendedores que possam atender a demanda",
                        "Este pedido entra na lista de Orçamentos em Aberto (se a funcionalidade estiver ativa)",
                        "Admin entra nos detalhes do pedido e pode encaminhar para um ou mais vendedores",
                        "Orçamento permanece na fila, mas identificado como resolvido e pintado em verde, para fins de histórico e análise"
                    }),

                new VersionEntry(
                    2, 29, 5, "Hotfix importação de produtos", new DateTime(2019, 07, 19),
                    new[]
                    {
                        "Correção de bug quando o vendedor ainda não tinha uma loja configurada no banco de dados",
                        "Melhorias na navegação e clareza das informaçãoes, títulos de tela, campos, outros"
                    }),

                new VersionEntry(
                    2, 29, 4, "Importação de produtos", new DateTime(2019, 07, 18),
                    new[]
                    {
                        "Correção do cálculo de disponibilidade de produtos com base em turnos",
                        "Exportação de templates de arquivos CSV e XLSX para preenchimento guiado",
                        "Importação de produtos através de arquivos CSV e XLSX no formato do template",
                        "Importação de imagens de produtos"
                    }),

                new VersionEntry(
                    2, 29, 3, "Correções e melhorias gerais", new DateTime(2019, 07, 10),
                    new[]
                    {
                        "Correção do formulário de Cookie Consent (GDPR) que ficava fixo no mobile mesmo após aceite",
                        "Label e texto para campo de filtro por localização no pedido de orçamento geral",
                        "Correção de indexação de orçamentos gerais em aberto, que levava em conta orçamentos fechados",
                        "Seção Melhores Vendedores da home não mostra mais vendedores desativados ou pendentes de moderação",
                        "Mostra nome alternativo do comprador e vendedor nas respostas do orçamento (tela de detalhes)",
                        "Correção na inicialização do app da Wirecard para operar pagamentos de marketplace",
                        "Configuração para ocultar nome e foto do comprador até o aceite de uma proposta de orçamento",
                        "Mensagem para comprador e vendedor sobre orçamento com pagamento em garantia",
                        "Correção na lista de vendedores pendentes de moderação",
                        "Filtro de contatos no módulo de orçamentos passa a filtrar somente email e telefone (libera URLs)",
                        "Verificação recorrente (de hora em hora) no gateway sobre status de transações com pagamento pendente",
                        "Melhoria do cadastro de turnos pelo admin, levando em conta fuso-horário"
                    }),

                new VersionEntry(
                    2, 29, 2, "Hotfix", new DateTime(2019, 07, 01),
                    new[]
                    {
                        "Correção na validação de períodos mínimo e máximo de locação",
                        "Correção no cálculo de preço dos itens a partir da entrada no carrinho, até o pagamento",
                        "Correção no cadastro de turnos de locação (RentalShifts) para levar em conta a TimeZone",
                        "Na lista de orçamentos do comprador, avatares de vendedores agora são links para perfil",
                        "Carrinho não permite adicionar produtos para locação em horários conflitantes",
                        "Produtos no carrinho são ordenados por data e hora de locação",
                        "Correção no cadastro de vendedor quando gateway de pagamento é Moip/Wirecard",
                        "Orçamento com pagamento pela plataforma agora direciona para detalhes do pedido quando pagamento é por boleto (mandava para carrinho vazio)",
                        "Revisão e refatoração do fluxo de checkout (processos internos)"
                    }),

                new VersionEntry(
                    2, 29, 1, "Hotfix", new DateTime(2019, 06, 18),
                    new[]
                    {
                        "Revisão validação número telefone no pedido de orçamento",
                        "Correção no cadastro de vendedor com pagamento habilitado mas sem que o vendedor receba pela plataforma (ex. módulo assinatura)",
                        "Correção no envio de email para outros idiomas (es, ca)"
                    }),

                new VersionEntry(
                    2, 29, 0, "Gateway de pagamento Stripe - Assinatura", new DateTime(2019, 06, 14),
                    new[]
                    {
                        "Assinatura de leads via Stripe",
                        "config > subscription > stripe > productId: Identificação do produto de assinatura na conta Stripe do Marketplace",
                        "Ajustes de código no módulo de assinaturas"
                    }),

                new VersionEntry(
                    2, 28, 4, "Hotfix", new DateTime(2019, 06, 18),
                    new[]
                    {
                        "Método OrderService.ProcessGatewayPostbackInternal tornado público para ser invocado via Hangfire",
                        "GlobalizationHelper passa a retornar label sem formatação caso formatação dê erro, ajuda a detectar e corrigir",
                        "Correção CSS formatação do link de comprar créditos no modelo de venda de leads",
                        "Preço do produto em caso de locação de produtos por turno, mostra o preço do menor turno (a partir de) ao invés do valor de hora",
                        "Campo no cadastro de produto para valor mínimo de locação por produto. O carrinho soma as ocorrências de todas as locações de um produto para validar o mínimo",
                        "Links adicionais corrigidos: passa a mostrar no rodapé, inclui corretamente tradução conforme label",
                        "Busca de produtos mostra preço do menor turno quando locação por turnos está ativada",
                        "Validação de telefone agora permite formatos internacionais (começando com + e código do país)",
                        "No painel admin, mostra botão de atualizar status da transação para pgtos Cartão de Crédito no Moip/Wirecard",
                        "Arquivos de localização para idiomas Catalão e Espanhol da Espanha",
                        "Mapeia custom fields no cadastro de comprador no resultado do retorno de um POST; antes, carregava apenas na chamada inicial (GET)",
                        "Campo documento de Buyer agora permite configuração similar à de Seller (AllowCpf, AllowCnpj, AllowFreeText)",
                        "Permite desabilitar campos de endereço do comprador caso não haja pagamento pela plataforma ou pagamento seja só por parte do vendedor"
                    }),

                new VersionEntry(
                    2, 28, 3, "Hotfix Wirecard", new DateTime(2019, 06, 12),
                    new[]
                    {
                        "Correção em OrderService.ProcessGatewayPostback que impedia processamento de atualização de status",
                        "Melhorias no reconhecimento de status de pgto na Wirecard",
                        "Obtém mensagem de motivo do cancelamento, se houver (ex. rejeitado pelo órgão emissor)"
                    }),

                new VersionEntry(
                    2, 28, 2, "Hotfix", new DateTime(2019, 06, 12),
                    new[]
                    {
                        "Copia campos customizados de orçamento a partir da opção dos orçamentos públicos",
                        "Somente campos base do orçamento eram copiados"
                    }),

                new VersionEntry(
                    2, 28, 1, "Hotfix", new DateTime(2019, 06, 12),
                    new[]
                    {
                        "Processamento de postback do gateway passa a estourar exceção em caso de Result.IsFailure, para que haja retentativa"
                    }),

                new VersionEntry(
                    2, 28, 0, "Gateway de pagamento Stripe", new DateTime(2019, 06, 10),
                    new[]
                    {
                        "Cobrança de leads via Stripe",
                        "config > payment > stripe > publishableKey: Chave pública obtida na conta Stripe do Marketplace",
                        "config > payment > stripe > secretKey: Chave privada obtida na conta Stripe do Marketplace",
                        "config > payment > stripe > currency: Código ISO da moeda suportada no Stripe do Marketplace",
                    }),

                new VersionEntry(
                    2, 27, 6, "Wirecard atualização automática de pedido em análise", new DateTime(2019, 06, 11),
                    new[]
                    {
                        "Trata corretamente casos em que o pagamento não foi aprovado"
                    }),

                new VersionEntry(
                    2, 27, 5, "Wirecard pagamento em análise", new DateTime(2019, 06, 11),
                    new[]
                    {
                        "Considera que pagamentos por cartão na Wirecard podem não ser aprovados automaticamente",
                        "Plataforma deixa de retornar erro caso status seja WAITING ou IN_ANALYSIS",
                        "Confirmação de checkout apresenta mensagem condizente, que informa que aguarda confirmação do pagamento",
                        "Pedidos em análise agora recebem postback para atualização automática de status enviada pela Wirecard",
                        "Correção no cálculo de taxas do marketplace e do gateway na transação"
                    }),

                new VersionEntry(
                    2, 27, 4, "Hotifx", new DateTime(2019, 06, 06),
                    new[]
                    {
                        "Vários pontos da plataforma tentavam gravar campos customizados nulos ou vazios",
                        "Filtra informações de contato no pedido de orçamento se flag habilitada",
                        "Refactory de parte das regras de negócio de orçamento e resposta de orçamento movidas para as entidades",
                        "Novas labels no arquivo de tradução"
                    }),

                new VersionEntry(
                    2, 27, 3, "Hotifx campos customizados", new DateTime(2019, 06, 05),
                    new[]
                    {
                        "O cadastro de respostas para campos customizados foi otimizado, de forma que tudo é feito em uma única chamada para o banco de dados",
                        "O cadastro de respostas para campos customizados de pedidos de orçamento de grupo foi ainda mais especializado: uma chamada para os campos de todos os orçamentos envolvidos"
                    }),

                new VersionEntry(
                    2, 27, 2, "Hotifx", new DateTime(2019, 06, 05),
                    new[]
                    {
                        "Cálculo de preço por turno na visão do comprador",
                        "Correções menores"
                    }),

                new VersionEntry(
                    2, 27, 1, "Hotifx", new DateTime(2019, 06, 05),
                    new[]
                    {
                        "Validação da configuração para garantir que Rental e Sale (locação e venda de produtos) não são habilitados simultaneamente",
                        "Oculta períodos de locação no carrinho quando Product.BusinessModel.Rental.HideShiftDurationFromUsers está habilitado"
                    }),

                new VersionEntry(
                    2, 27, 0, "Melhoria dos formulários de gateway de pagamento", new DateTime(2019, 06, 03),
                    new[]
                    {
                        "Agora no cadastro do vendedor, para cada gateway de pagamento, tem um formulário para cadastro de dados nas respectivas plataformas.",
                        "Funciona para Moip e Pagar.me, quando habilitados, no marketplace de produto."
                    }),

                new VersionEntry(
                    2, 26, 0, "Locação por turnos", new DateTime(2019, 06, 04),
                    new[]
                    {
                        "Config para habilitar locação por turnos: config.Product.BusinessModel.Rental.RentalPeriodMode = \"shifts\"",
                        "Admin pode cadastrar os turnos + horários que serão suportados pela plataforma (ex. manhã = 08:00 - 12:00)",
                        "Vendedor associa os turnos atendidos em cada produto; pode selecionar um ou mais",
                        "Ao informar o preço do produto, plataforma mostra o preço por turno, de acordo com a duração em horas",
                        "Comprador passa a locar o produto selecionando por turno vs dia da semana, numa visualização de calendário semanal",
                        "Config para ocultar os horários de cada turno da visão de usuários finais (comprador e vendedor), mostrando apenas o nome do turno"
                    }),

                new VersionEntry(
                    2, 25, 7, "Hotfix", new DateTime(2019, 05, 29),
                    new[]
                    {
                        "Moip: correção do valor dos produtos repassados ao gateway quando qtde > 1"
                    }),

                new VersionEntry(
                    2, 25, 6, "Hotfix", new DateTime(2019, 05, 27),
                    new[]
                    {
                        "Textos de cobrança de orçamento agora são customizáveis",
                        "Resolvido bug que não permitia que comprador alterasse foto de perfil",
                        "Admin mostra corretamente vendedores com perfil completo/incompleto",
                        "Reflete instantaneamente alteração de avatar do perfil do comprador"
                    }),

                new VersionEntry(
                    2, 25, 5, "Hotfix", new DateTime(2019, 05, 24),
                    new[]
                    {
                        "No autocomplete de endereço do vendedor, garante que a parte foi realmente encontrada antes de travar o campo (tivemos problema com bairro em um endereço)",
                        "Ao final do pedido de orçamento, adicionado link para ver todos os pedidos",
                        "Na busca de vendedores, ordenação por distância só fica disponível se filtro por Localização estiver ativo (é necessário um ponto de referência)",
                        "No perfil do vendedor, oculta coluna à esquerda se informações estiverem em branco",
                        "No pedido de orçamento direcionado, só traz as categorias e subcategorias associadas ao vendedor",
                        "Refactory ViewModel da tela do carrinho simplifica desenvolvimento da View",
                        "Telas do carrinho e checkout tiveram mais elementos identificados por classes CSS, facilitando customização por HTML no admin"
                    }),

                new VersionEntry(
                    2, 25, 4, "Hotfix", new DateTime(2019, 05, 23),
                    new[]
                    {
                        "Corrige problema no cadastro de usuários, problema ocorria no envio de email (template não encontrado)",
                        "Seleciona automaticamente o método de entrega suportado pelo produto (se for o primeiro produto do carrinho)"
                    }),

                new VersionEntry(
                    2, 25, 3, "Hotifx", new DateTime(2019, 05, 22),
                    new[]
                    {
                        "Correção da criação de contas Wirecard para vendedores",
                        "No caso de conta CNPJ, conta bancária passa a ser em nome do CNPJ também",
                        "Labels no arquivo de tradução especilizadas para cadastro de vendedores com gateway Wirecard"
                    }),

                new VersionEntry(
                    2, 25, 2, "Hotifx", new DateTime(2019, 05, 21),
                    new[]
                    {
                        "Link QuoteSearch adicionado ao menu mobile",
                        "Valida documento da conta bancária como CPF caso gateway seja Moip",
                        "Adicionada variável [SELLER_ADDRESS] ao email de produto disponível para retirada, enviado para o comprador"
                    }),

                new VersionEntry(
                    2, 25, 1, "Hotfix", new DateTime(2019, 05, 17),
                    new[]
                    {
                        "Obrigatoriedade de cadastro de conta bancária p/ vendedor calculado corretamente",
                        "Mostra menu 'gestão' para o vendedor quando módulo de assinaturas está habilitado",
                        "Mensagem de perfil incompleto do vendedor passa a levar em consideração categorias associadas no nível de portfólio",
                        "Mensagem de perfil incompleto do vendedor passa a verificar se cadastrou itens de portfolio quando é ligada a flag config.quote.isPortfolioRequiredForGeneralQuote",
                        "Busca de vendedores: passa a levar em consideração categorias associadas no nível de portfólio",
                        "Busca de vendedores: leva em consideração a flag config.quote.isPortfolioRequiredForGeneralQuote, sabendo não faz sentido mostrar na busca um seller incompleto que um seller incompleto",
                        "Valida o prazo de uma proposta no momento da sua postagem e também na aceitação (só permite aceitar propostas para datas futuras)",
                        "Pgto em garantia: passa a agendar transferência para vendedor também em caso de boleto (considerava apenas cartão de crédito)",
                        "Pedidos com ShippingMethod.ActualNoShipping não enviavam notificação de pagamento concluído para comprador nem vendedor",
                        "Busca de produtos passa a procurar pela ocorrência das palavras em qualquer ordem",
                        "Configuração de API no Framework 4.7.2 para upload de imagens",
                        "Nova label para diferenciar o título das páginas de cadastro de comprador e vendedor"
                    }),

                new VersionEntry(
                    2, 25, 0, "Suporte a múltiplos idiomas", new DateTime(2019, 05, 16),
                    new[]
                    {
                        "Suporte adicionado para Espanhol e Catalão, além de Português e Inglês que já eram suportados e foram mantidos",
                        "Qualquer idioma pode ser definido como padrão",
                        "Quando há mais de um idioma ativo, as bandeiras correspondes são mostradas no cabeçalho/menu do site",
                        "Necessário haver pelo menos 1 idioma ativo, mas sem limite superior",
                        "Técnico: NewContext adicionado para desenvolvimento de novas features de forma isolada de código legado"
                    }),

                new VersionEntry(
                    2, 24, 10, "Hotfix", new DateTime(2019, 05, 10),
                    new[]
                    {
                        "Calcula o preço do frete PAC/Sedex mesmo com o erro 11 (região de risco com delay na entrega)"
                    }),

                new VersionEntry(
                    2, 24, 9, "Hotfix", new DateTime(2019, 05, 10),
                    new[]
                    {
                        "Seleção de vendedores para envio de orçamento geral passa usar a mesma engine da busca, ficando consistente",
                        "Pedido de orçamento geral é tratado como tal mesmo que apenas um vendedor possa atendê-lo",
                        "Motor de busca de vendedor indexa e trata eventos mesmo com a busca desligada, para viabilizar os cenários de negócio que passam a usar a busca"
                    }),

                new VersionEntry(
                    2, 24, 8, "Hotfix", new DateTime(2019, 05, 10),
                    new[]
                    {
                        "Validação do documento da conta bancária seguia as mesmas regras de validação do documento do seller",
                        "A plataforma agora aceita CPF ou CNPJ para documento da conta bancária, mesmo que seller tenha restrições de somente CPF ou somente CNPJ",
                        "No pedido de orçamento geral, form avança automaticamente para passo 2 se passo 1 estiver preenchido"
                    }),

                new VersionEntry(
                    2, 24, 7, "Hotfix", new DateTime(2019, 05, 10),
                    new[]
                    {
                        "Correção da mensagem NO_TRACKING_CODE_SUPPLIED (mostrava o Label ao invés da mensagem)",
                    }),

                new VersionEntry(
                    2, 24, 6, "Código de rastreamento do produto", new DateTime(2019, 05, 07),
                    new[]
                    {
                        "Vendedor pode informar o código de rastreamento do produto",
                        "Link de rastreamento aparece nos detalhes do pedido para comprador e vendedor",
                        "Link de rastreamento pode ser colocado no email (variável [TRACKING_LINK])"
                    }),

                new VersionEntry(
                    2, 24, 5, "Hotfix", new DateTime(2019, 05, 03),
                    new[]
                    {
                        "Correção de reconhecimento de boleto pago para integração Moip/Wirecard"
                    }),

                new VersionEntry(
                    2, 24, 4, "Hotfix", new DateTime(2019, 05, 03),
                    new[]
                    {
                        "Orçamento a partir da busca não funcionava quando haviam erros de validação no backend",
                        "Seleciona automaticamente categoria e subcategoria para orçamentos a partir da busca, quando info disponível",
                        "Realiza todas as validações de custom fields de orçamento de uma só vez",
                        "Trocada string do status 'pedido processado' para 'pedido registrado', evitando confusão",
                    }),

                new VersionEntry(
                    2, 24, 3, "Hotfix", new DateTime(2019, 05, 03),
                    new[]
                    {
                        "Pequenos ajustes de usabilidade no cadastro de produto"
                    }),

                new VersionEntry(
                    2, 24, 2, "Hotfix", new DateTime(2019, 05, 02),
                    new[]
                    {
                        "Corrigido cadastro de conta de recebedor no gateway Pagar.me",
                        "Passa a levar em conta o novo campo Titular da Conta, ao invés do Nome"
                    }),

                new VersionEntry(
                    2, 24, 1, "Hotfix", new DateTime(2019, 04, 30),
                    new[]
                    {
                        "Corrigida validação de email do usuário na ação de redefinir a senha",
                        "Corrigida validação de locação de produto/serviço na unidade Mês"
                    }),

                new VersionEntry(
                    2, 24, 0, "Estoque de produto por característica", new DateTime(2019, 04, 30),
                    new[]
                    {
                        "config > product > variants > stockByVariantEnabled > true|false (padrão: false)",
                        "Opção que habilita, para o marketplace, no cadastro do produto do vendedor, a opção de gestão de estoque por característica do produto.",
                        "A gestão de estoque por característica faz com que o comprador possa comprar mais de uma unidade, de característica diferente, de um mesmo produto."
                    }),

                new VersionEntry(
                    2, 23, 12, "Cadastro de lead de comprador no pedido de orçamento", new DateTime(2019, 04, 26),
                    new[]
                    {
                        "Ao enviar pedido para toda a base, o comprador preenche um cadastro inicial, que agora é salvo antes de finalizar o preenchimento do briefing",
                        "Esta melhoria visa ter os contatos de potenciais compradores (leads) mesmo que eles não cheguem a pedir um orçamento"
                    }),

                new VersionEntry(
                    2, 23, 11, "Hotfix", new DateTime(2019, 04, 26),
                    new[]
                    {
                        "Removidas informações de frete para produto digital (download) no anúncio do produto."
                    }),

                new VersionEntry(
                    2, 23, 10, "Hotfix", new DateTime(2019, 04, 26),
                    new[]
                    {
                        "Corrigido processamento de transação de gateway de pagamento, que não alterava para 'aguardando retirada' ou 'aguardando entrega', quando era o caso.",
                        "Ajuste no consulta de cidades, pois estava fazendo o .Take antes de ordenar e não estava ordenando por nome no final.",
                        "Validação da marcação obrigatória do Termos de uso ao efetuar um cadastro de comprador ou vendedor",
                        "Validação no objeto de configuração para alertar da necessidade da url de Termos de uso. Caso não informada, não aparecerá a opção de concordar com os termos no cadastro de comprador e vendedor."
                    }),

                new VersionEntry(
                    2, 23, 9, "Gerar cobrança automaticamente para orçamentos aceitos", new DateTime(2019, 04, 25),
                    new[]
                    {
                        "Quando o comprador aceitar uma proposta de orçamento, gera automaticamente o link para cobrança",
                        "Esta é uma nova configuração (config > quoteBilling > autoGenerateBillingWhenBuyerAcceptsProposal > true|false), habilitada por padrão"
                    }),

                new VersionEntry(
                    2, 23, 8, "Hotfix", new DateTime(2019, 04, 25),
                    new[]
                    {
                        "Contagem de orçamentos ganhos levava em conta data de criação ao invés de data de fechamento"
                    }),

                new VersionEntry(
                    2, 23, 7, "GDPR cookie consent message", new DateTime(2019, 04, 25),
                    new[]
                    {
                        "config > gdpr > showCookieConsentMessage > true|false",
                        "Se ligado, mostra mensagem de aviso em todas as páginas, até que o usuário confirme",
                        "Se houver política de privacidade, será mostrado um link na mensagem",
                        "Classe global estática CookieConsent (em Presentation) indica se usuário aceitou, o que pode ser usado para controlar visibilidade de componentes"
                    }),

                new VersionEntry(
                    2, 23, 6, "Hotfix", new DateTime(2019, 04, 24),
                    new[]
                    {
                        "Wirecard: Corrigido problema ao cadastrar no número de telefone do vendedor",
                        "Ajuste para 1 (um) quando não informado parcelamento máximo no cadastro de produto",
                        "Corrigida alteração dos dados de conta bancária do vendedor no gateway de pagamento",
                        "Opção config > quote > priceInAnswerRequired > true|false na configuração de orçamento para deixar o campo de valor na resposta do orçamento como obrigatório (false, por padrão)",
                        "Corrigida validação do campo de documento, quando esse era inválido e ficava desabilitado no cadastro do comprador",
                        "Corrigida validação do formulário de resposta do orçamento, que submetia mesmo com erros de validação e limpava o formulário"
                    }),

                new VersionEntry(
                    2, 23, 5, "Hotfix", new DateTime(2019, 04, 16),
                    new[]
                    {
                        "Corrigido job que gera as urls amigáveis de vendedores"
                    }),

                new VersionEntry(
                    2, 23, 4, "Hotfix", new DateTime(2019, 04, 10),
                    new[]
                    {
                        "Retirado o autocomplete dos campos de email e senha no pedido de orçamento",
                        "Melhoria nas mensagens de validação quando é feita integração com Wirecard na criação de conta bancária",
                        "Corrigida validação de CNPJ e mensagem no cadastro do vendedor",
                        "Pequenas correções no cadastro de vendedor"
                    }),

                new VersionEntry(
                    2, 23, 3, "Hotfix", new DateTime(2019, 04, 05),
                    new[]
                    {
                        "Corrigido job que gera as urls amigáveis de produtos e vendedores"
                    }),

                new VersionEntry(
                    2, 23, 2, "PWA - Notificações Push", new DateTime(2019, 03, 27),
                    new[]
                    {
                        "Adicionadas notificações Push para diversos eventos da plataforma",
                        "Revisão de campos de manifesto WebApp",
                        "Corrigido botão de carrinho sendo exibido incorretamente no menu mobile",
                        "Adicionada flag para habilitar exibição de produtos relacionados",
                        "Adicionada tela para envio de notificações para base de usuários",
                        "Busca de cidades para cadastro de vendedor melhorada para informar resultados mais relevantes",
                    }),

                new VersionEntry(
                    2, 23, 1, "PWA - Adicionado manifesto webApp", new DateTime(2019, 03, 14),
                    new[]
                    {
                        "Adicionada geração de manifesto de WebApp",
                        "Removida label de método de envio em formulário de produto sem frente",
                        "Corrigido campo que não estava sendo atualizado na customização da home",
                        "Atualizado filtro de mensagens de orçamento para incluir URLs e mais formatos de telefone",
                        "Corrigida opção de split de taxa do gateway de pagamento para o vendedor no Wirecard"
                    }),

                new VersionEntry(
                    2, 22, 20, "Configuração de Gateway de pagamentos", new DateTime(2019, 03, 07),
                    new[]
                    {
                        "Adicionada opção no painel admin para configurar gateway de pagamentos",
                        "Adicionados campos para exportação em tabela de produtos no admin",
                        "Nova validação para campo de CPF/CNPJ em cadastros com salvamento parcial"
                    }),

                new VersionEntry(
                    2, 22, 19, "Configurações de Pagamento no Painel de Customização", new DateTime(2019, 02, 27),
                    new []
                    {
                        "Adicionado painel de customização de taxas de comissão e opções de pagamento",
                        "Correção de erro no pedido de orçamento com cadastro",
                        "Correção no módulo de Redirect HTTPS",
                        "Adicionado WhatsApp no painel de customização do cadastro do vendedor",
                        "Removido GooglePlus",
                        "Vendedores inativos excluídos da query de melhores vendedores",
                    }),

                new VersionEntry(
                    2, 22, 18, "Número de WhatsApp e Personalização de Links Adicionais", new DateTime(2019, 02, 21),
                    new []
                    {
                        "Adicionado campo opcional de número de whatsapp do vendedor com integração click-to-chat no perfil público",
                        "Adicionado painel de customização dos links adicionais do menu",
                        "Refeita flag de categoria obrigatória na busca de produtos",
                        "Correção de exceções de parâmetro nulo em páginas de busca"
                    }),

                new VersionEntry(
                    2, 22, 17, "Limite de Cobrança e Filtro de Mensagens", new DateTime(2019, 02, 18),
                    new []
                    {
                        "Aumentado limite máximo de valor aceito para geração de cobrança de orçamento",
                        "Adicionada flag que permite filtrar informações de contato em orçamentos e notificar admins",
                        "Whitespaces em campos de cadastro agora são removidos antes de enviar para o gateway",
                        "User-agent google xrawler adicionado a lista de ignorados de error report",
                    }),

                new VersionEntry(
                    2, 22, 16, "Refactor e Hotfixes", new DateTime(2019, 02, 13),
                    new []
                    {
                        "Refactor de interfaces de avaliações e de seção melhores vendedores",
                        "Adicionada flag que torna obrigatório selecionar uma categoria na busca de produtos",
                        "Corrigido erro que era gerado ao tentar fazer uma assinatura Wirecard com um cliente sem CustomerId"
                    }),

                new VersionEntry(
                    2, 22, 15, "Botões de pré-setup", new DateTime(2019, 02, 06),
                    new []
                    {
                        "Adicionados botões de pré-setup para configurar modelo de negócio",
                        "Corrigida referência inválida em validação de vendedor",
                        "Adicionada label informativa de período trial para assinaturas",
                        "Corrigido bug de envio de email solicitando avaliações",
                        "Respostas de orçamento agora exibem o horário compensado para o fuso horário do usuário",
                        "Adicionado email informativo de envio de cobrança de orçamento",
                    }),

                 new VersionEntry(
                    2, 22, 14, "Arquivo de recursos e hotfixes diversos", new DateTime(2019, 01, 31),
                    new []
                    {
                        "Configurações padrão movidas para arquivo de recurso centralizado",
                        "Corrigido bug de validação de campos customizados em vendedor, produtos e portfolio",
                        "Lista de vendedores no painel admin agora exibe perfis completos mesmo se estiver aguardando moderação",
                        "Mensagens de erro de pagamento são exibidas com mais clareza",
                        "Adicionada validação para tamanho máximo de campo de mensagem de disponibilidade para produtos de agendamento",
                        "Tag de 'cadastro completo' no painel Admin agora não considera mais o status de moderação"
                    }),

                new VersionEntry(
                    2, 22, 13, "Customização de campos gerais de orçamento", new DateTime(2019, 01, 28),
                    new []
                    {
                        "Adicionada tela no painel de customização para personalização de campos gerais de orçamento",
                        "Corrigido bug que impedia que vendedores fossem exibidos corretamente na seção 'Melhores vendedores' sob certas condições",
                        "Cadastro de telefone via orçamento não requer mais que o campo esteja visível ao vendedor",
                        "Corrigido envio de emails de confirmação de pagamento de boleto ao criar um pedido",
                        "Adicionada flag para desabilitar campo de anexo em pedidos de orçamento",
                    }),

                 new VersionEntry(
                    2, 22, 12, "Arquivo de tradução para Admins", new DateTime(2019, 01, 23),
                    new []
                    {
                        "Corrigida geração de pedidos Moip para agendamentos",
                        "Adicionada opção de 'restaurar padrão' para arquivo de tradução",
                        "Liberado acesso público do arquivo de tradução para administradores",
                        "Adicionada opção no painel de customização para editar plano de fundo secundário"
                    }),

                new VersionEntry(
                    2, 22, 11, "Mensagens de erro do Gateway", new DateTime(2019, 01, 22),
                    new []
                    {
                        "Correção de processamento para certos postbacks de Boleto Moip",
                        "Adicionados botões de chamado para assinatura em marketplaces de venda de leads",
                        "Correção de exibição de valores de parcelamento no produto",
                        "Adicionadas mensagens de erro do Gateway no cadastro do cliente",
                        "Subcategorias de categorias deletadas não são mais exibidas no painel admin",
                        "Adicionada flag para salvar telefone do orçamento no cadastro do comprador ao criar conta",
                    }),

                new VersionEntry(
                    2, 22, 10, "Múltiplos agendamentos em um pedido", new DateTime(2019, 01, 15),
                    new []
                    {
                        "Alterada lógica de agendamento de produtos para permitir múltiplos agendamentos em um único pedido",
                        "É possível adicionar o mesmo item no carrinho várias vezes com múltiplos períodos diferentes:",
                        "Contato que os períodos não se sobreponham",
                        "Adicionada nova flag que permite que assinaturas sejam marcadas como opcionais (apenas para venda de créditos)",
                    }),

                new VersionEntry(
                    2, 22, 9, "Hotfix Moip", new DateTime(2019, 01, 09),
                    new []
                    {
                        "Alterado formulário de conta bancária para plataformas operando via Moip",
                        "Textos foram reformulados para refletir nova lógica de negócios",
                        "Corrigido erro que impedia que certos cadastros de pessoas jurídicas fossem enviados ao Moip",
                        "Adicionado novo evento para envio de email ao aprovar cadastros via moderação"
                    }),

                new VersionEntry(
                    2, 22, 8, "Melhorias e Hotfixes", new DateTime(2019, 01, 08),
                    new []
                    {
                        "Adicionadas flags que permitem ignorar validação de CPF, CEP e Cidade para cadastro de comprador",
                        "Corrigido erro que impedia que vendedores sem produtos aparecessem em busca",
                        "Alterado cálculo do split padrão do Moip para direcionar taxa do gateway para a marketplace",
                        "Corrigido erro com custom fields em tela de completar cadastro de comprador",
                        "Corrigido erro ao visualizar detalhes de objetos com custom fields deletados",
                    }),

                new VersionEntry(
                    2, 22, 7, "Melhorias e Hotfixes", new DateTime(2019, 01, 04),
                    new []
                    {
                        "Corrigido exibição incorreta de preços com multiplicador em perfil de vendedor",
                        "Liberação automática de pagamento para serviços agendados agora utiliza data do agendamento como base de cálculo",
                        "Corrigido bug que permitia que usuários sem GatewayID vissem a opção 'Meu Saldo' no menu de gestão do vendedor",
                    }),

                new VersionEntry(
                    2, 22, 6, "Hotfix da busca por raio a partir de localização exata", new DateTime(2018, 12, 27),
                    new []
                    {
                        "Atualização do pacote SmartSearch (core) para versão 1.0.4",
                        "Migração do arquivo de versões para uma classe especializada em Domain"
                    }),

                new VersionEntry(
                    2, 22, 5, "Melhorias e Hotfixes", new DateTime(2018, 12, 20),
                    new []
                    {
                        "Corrigido exibição botão de orçamento na busca de vendedores em marketplaces sem orçamento para base",
                        "Adicionada validação adicional no painel de customização da Home",
                        "Adicionado botão de inserção de tags de imagens em templates de e-mail",
                        "Corrigida referência inválida a custom fields deletados",
                        "Ajustes de exibição no menu Admin",
                        "Corrigido bug na integração API do Google Places"
                    }),

                new VersionEntry(
                    2, 22, 4, "Hotfix busca vendedores", new DateTime(2018, 12, 18),
                    new []
                    {
                        "Problema na ordenação dos resultados por Score"
                    }),

                new VersionEntry(
                    2, 22, 3, "Orçamento para vendedores retornados pela busca", new DateTime(2018, 12, 18),
                    new []
                    {
                        "Adicionado novo botão na tela de busca de vendedores em marketplaces de orçamento",
                        "Permite pedir orçamento para todos os vendedores retornados na busca",
                        "Comprador passa a ter mais controle e transparência sobre quem recebe seu pedido de orçamento"
                    }),

                new VersionEntry(
                    2, 22, 2, "API de métricas do marketplace", new DateTime(2018, 12, 18),
                    new []
                    {
                        "Adicionado endpoint para API de métricas do marketplace"
                    }),

                new VersionEntry(
                    2, 22, 1, "Ajustes de Layout Responsivo", new DateTime(2018, 12, 17),
                    new []
                    {
                        "Layouts de telas do painel de vendedor e comprador foram ajustados para melhor exibição em mobile",
                        "Tabelas e Dashboard foram ajustadas, entre outros detalhes",
                        "Corrigido erro de paginação da lista de avaliações de usuário",
                        "Corrigida indexação indevida de páginas de avaliação vazias"
                    }),

                new VersionEntry(
                    2, 22, 0, "Busca de vendedores via SmartSearch", new DateTime(2018, 12, 14),
                    new []
                    {
                        "Motor de busca trocado de banco de dados + cache para SmartSearch (live)",
                        "Indexação na inicialização da plataforma + resposta a eventos de atualização de vendedores",
                        "Documentação completa em https://app.nuclino.com/Ideianoar/Mapa-de-Funcionalidades/Busca-de-Vendedores-via-SmartSearch-2e9d4f56-424a-4c47-9abf-5e7502b37ab9"
                    }),

                new VersionEntry(
                    2, 21, 15, "Sitemap para Search Engine Optimization", new DateTime(2018, 12, 14),
                    new []
                    {
                        "Adicionado sitemap na rota /sitemap.xml",
                        "Sitemap é gerado dinamicamente com o conteúdo relevante do site",
                        "É usado por crawlers e motores de busca para indexar páginas relevantes"
                    }),

                new VersionEntry(
                    2, 21, 14, "Redesign do Painel de Customização", new DateTime(2018, 12, 13),
                    new []
                    {
                        "Experiência de usuário otimizada para painel de customização",
                        "Corrigido bug que permitia que vendedor aguardando moderação pudesse ser visualizado em seção de produto",
                        "Corrigida exibição incorreta de menu de moderação quando a funcionalidade estava desabilitada"
                    }),

                new VersionEntry(
                    2, 21, 13, "Campo customizado de arquivo", new DateTime(2018, 12, 12),
                    new []
                    {
                        "Adicionado novo tipo de campo customizado: Arquivo",
                        "Permite envio de diversos tipos de arquivo:",
                        "Imagens, textos, áudios, vídeos e .PDFs são permitidos",
                        "Adicionada config para inserir ID do ShareThis para monitorar métricas de compartilhamento em mídias sociais"
                    }),

                new VersionEntry(
                    2, 21, 12, "Exibicação dos valores fixo + adicional", new DateTime(2018, 12, 10),
                    new []
                    {
                        "Adicionado verificação para mostrar valor fixo, adicional ou ambos dependendo da configuração dos valores"
                    }),

                new VersionEntry(
                    2, 21, 11, "Compartilhamento em redes sociais", new DateTime(2018, 12, 7),
                    new []
                    {
                        "Adicionados botões de compartilhamento em redes sociais",
                        "São exibidos nas páginas de detalhes de um Produto/Serviço ou no perfil publico de um vendedor",
                        "Podem ser desligados quando necessário através da flag Config.SocialMedia.EnableShareButtons"
                    }),

                new VersionEntry(
                    2, 21, 10, "Integração de Venda de Leads com Busca de Orçamentos", new DateTime(2018, 12, 5),
                    new []
                    {
                        "Busca de orçamentos agora funciona com venda de leads",
                        "Quando módulo de venda de leads está ligado, passa a funcionar da seguinte forma:",
                        "Orçamentos têm detalhes ocultados quando vendedor não possui o orçamento liberado, mesmo estando logado",
                        "Ao clicar em 'Responder orçamento', vendedor é redirecionado para tela de desbloqueio de orçamentos",
                        "Se orçamento já tiver sido desbloqueado, detalhes ficam visíveis, e 'Responder' apenas redireciona para tela de detalhes",
                        "Layout da tela de busca foi ligeiramente ajustado"
                    }),

                new VersionEntry(
                    2, 21, 9, "Moderação V2 e avaliação Double Blind", new DateTime(2018, 12, 3),
                    new []
                    {
                        "Moderação de vendedores agora permite que os vendedores façam login e editem seu perfil normalmente",
                        "Vendedor permanecerá invisível na plataforma até ser aprovado por um moderador",
                        "Administrador poderá ver uma lista com todos os usuários pendentes de moderação no painel admin",
                        "Avaliações agora tem um prazo máximo em dias para serem realizadas",
                        "Novo modelo de avaliação Double Blind:",
                        "Avaliação de vendedores E compradores deve estar habilitada, além de flag na config.",
                        "Neste modelo, avaliações só se tornam públicas após as duas partes terem se avaliado, ou após passado o prazo para avaliar"
                    }),

                new VersionEntry(
                    2, 21, 8, "Configuração Self-Service", new DateTime(2018, 11, 30),
                    new []
                    {
                        "Adicionadas duas novas configurações para disponíveis para o usuário admin:",
                        "Link para o blog, e scripts de integração com serviços de chat terceirizados",
                        "Configurações estão disponíveis para o cliente a partir da aba Outros do painel de customização do admin"
                    }),

                new VersionEntry(
                    2, 21, 7, "Table Responsive no painel do vendedor", new DateTime(2017, 12, 5),
                    new []
                    {
                        "Foi adicionado uma div com classe \"Table Responsive\" para melhorar a usabilidade da plataforma no mobile"
                    }),

                new VersionEntry(
                    2, 21, 6, "Feature de acesso ao Painel de Sugestões", new DateTime(2018, 11, 28),
                    new []
                    {
                        "Foi adicionado um token de acesso para o novo Painel de Sugestões do Ideia no Ar",
                        "Foi adicionado label para o texto do Document Number no rodapé"
                    }),

                new VersionEntry(
                    2, 21, 5, "Busca de orçamentos em aberto", new DateTime(2018, 11, 28),
                    new []
                    {
                        "Nova funcionalidade opcional, integrada com novo modelo de busca SmartSearch",
                        "Permite que prestadores de serviço, (logados ou não), pesquisem todos os orçamentos em aberto",
                        "Pesquisa inclui uma série de filtros configuráveis e parâmetros de ordenação",
                        "Prestador pode responder orçamentos mesmo que não tenham sido solicitados a ele",
                        "Admin pode ocultar detalhes para usuários sem login"
                    }),

                new VersionEntry(
                    2, 21, 4, "Feature pular cadastro e perfil incompleto", new DateTime(2018, 11, 21),
                    new []
                    {
                        "Foi adicionado opção no config para habilitar botão \"pular cadastro\"",
                        "Foi adicionado status perfil completo/incompleto no painel admin",
                        "Corrigido bug que exibia campos não públicos em detalhes de orçamento",
                        "Corrigida referência inválida nos detalhes do vendedor no painel admin",
                        "Corrigido evento recorrente sendo executado em marketplaces onde não deveria"
                    }),

                new VersionEntry(
                    2, 21, 3, "Integração do modelo com Hero Unit e Banner simultâneos", new DateTime(2018, 11, 21),
                    new []
                    {
                        "Foi adaptado o método de exibição do Hero Unit junto ao Banner"
                    }),

                new VersionEntry(
                    2, 21, 2, "Pacotes de créditos avulsos", new DateTime(2018, 11, 19),
                    new []
                    {
                        "Adicioanda opção de cadastrar pacotes de créditos para compra avulsa",
                        "Pode ser usado juntamente com módulo de assinatura, ou sozinho",
                        "Habilitado em Config.Quote.LeadSales.IndividualPacks.Enabled"
                    }),

                new VersionEntry(
                    2, 21, 1, "Label de Redes Sociais no Placeholder", new DateTime(2018, 11, 13),
                    new []
                    {
                        "Label para o placeholder de redes sociais",
                        "Corrigido bug que impedia pagamento de items sem preço base (apenas adicionais)",
                        "Corrigida falha ao salvar os campos customizados quando um item de portfolio era criado"
                    }),

                new VersionEntry(
                    2, 21, 0, "Modelo de compra/venda de leads de orçamento", new DateTime(2018, 11, 8),
                    new []
                    {
                        "Módulo opcional que funciona sobre o fluxo de orçamentos",
                        "Permite que orçamentos tenham seus detalhes bloqueados",
                        "Vendedor deverá gastar créditos para desbloquear o orçamento",
                        "Integrado com módulo de assinaturas",
                        "Planos de assinatura podem conter pacotes de créditos mensais"
                    }),

                new VersionEntry(
                    2, 20, 2, "Hotfix label additional info 2", new DateTime(2018, 11, 8),
                    new []
                    {
                        "Corrigido erro relacionado ao arquivo de tradução",
                        "Adicionado link do perfil do vendedor nas respostas de pedido de orçamento"
                    }),

                new VersionEntry(
                    2, 20, 1, "Hotfix importação em massa", new DateTime(2018, 11, 5),
                    new []
                    {
                        "Erro ao baixar template de importação"
                    }),

                new VersionEntry(
                    2, 20, 0, "Importação em massa de prestadores de serviço", new DateTime(2018, 11, 5),
                    new []
                    {
                        "Link na tela de vendedores, Admin escolhe entre formatos CSV e Excel",
                        "Plataforma oferece link para baixar template",
                        "Template e importação obedecem config de campos da plataforma, inclusive campos customizados",
                        "Registros são validados antes da importação. Importação só acontecem se todos registros forem válidos",
                        "Lib BatchImport deve facilitar implementação de importação de outras entidades (ex. Comprador, Produto, etc)",
                        "Corrigido bug no script de AutoComplete do GoogleMaps"
                    }),

                new VersionEntry(
                    2, 19, 2, "Adicionada opção de embutir taxas de parcelamento no pedido (Moip)", new DateTime(2018, 11, 1),
                    new []
                    {
                        "Taxas devem ser pré-configuradas em Config.Payment.Moip.Options",
                        "Valor é repassado para o comprador quando ativada a flag Config.Payment.Moip.Options.CustomerPaysFees",
                        "Valor das taxas é exibido no valor das parcelas no checkout e nos detalhes do pedido após o pagamento"
                    }),

                new VersionEntry(
                    2, 19, 1, "Preço adicional por pessoa para agendamentos", new DateTime(2018, 10, 29),
                    new []
                    {
                        "Adicionada opção de configurar precificação adicional para agendamentos",
                        "Unidades mínima e máxima, além do preço por unidade são configuráveis na página de produto",
                        "Deve ser habilitada pela flag Config.Product.BusinessModel.Rental.EnablePriceMultiplier"
                    }),

                new VersionEntry(
                    2, 19, 0, "Integração com Gateway de Pagamentos Wirecard/Moip", null,
                    new []
                    {
                        "Nova opção de gateway de pagamentos",
                        "Disponível com todas as funcionalidades presentes em outros gateways",
                        "Pode ser configurado em Config.Payment.Moip",
                        "Adicionada validação de tamanho de senha em cadastro durante pedido de orçamento",
                        "Aumentado limite de tamanho na descrição dos vendedores para 4000 caracteres"
                    }),

                new VersionEntry(
                    2, 18, 11, "Geolocalização de browser para buscas", new DateTime(2018, 10, 25),
                    new []
                    {
                        "Buscas que utilizam autocomplete do GoogleMaps agora pedem localização do browser",
                        "Campo de busca é preenchido automaticamente com a localidade do usuário quando aceito",
                        "Um botão é fornecido para limpar os campos de busca",
                        "Corrigido comportamento indevido em margens de agendamento",
                        "Pode ser desligada na Config caso necessário"
                    }),

                new VersionEntry(
                    2, 18, 10, "Margem mínima entre agendamentos", new DateTime(2018, 10, 23),
                    new []
                    {
                        "Adicionado campo de margem mínima entre agendamentos",
                        "Pode ser configurado na página do serviço/produto",
                        "Define um período mínimo de intervalo entre dois agendamentos para um mesmo item",
                        "Lógica desacoplada do período mínimo de notificação",
                        "Refatoração de custom fields para atender aos princípios do DDD",
                        "E-mails enviados pela plataforma agora utilizam corretamente a variável DefaultFromName da Config"
                    }),

                new VersionEntry(
                    2, 18, 9, "Melhorias diversas", new DateTime(2018, 10, 15),
                    new []
                    {
                        "Adicionado link para o perfil público do vendedor na tela de detalhes do orçamento para o comprador",
                        "Melhorado fluxo de edição de campos customizados",
                        "Adicionados mais feedbacks de validação no Painel Admin",
                        "Adicionada opção de enviar cópia de orçamentos recebidos para o Admin",
                        "Comprador agora é logado automaticamente ao registrar quando pede seu primeiro orçamento",
                        "Correção na exibição de opções de parcelamento na página pública de produto com parcelas customizadas"
                    }),

                new VersionEntry(
                    2, 18, 8, "Filtro personalizado de vendedores", new DateTime(2018, 10, 10),
                    new []
                    {
                        "Filtro de busca personalizada adicionado para o I'm Health"
                    }),

                new VersionEntry(
                    2, 18, 7, "Validação de documentos e outras melhorias", new DateTime(2018, 10, 8),
                    new []
                    {
                        "Adicionada validação por dígito verificador para CPFs e CNPJs",
                        "Adicionada opção de call to action para compradores",
                        "Usabilidade melhorada para opção de período de locação por produto",
                        "Opções de frete agora são ocultadas em modelos de negócio sem frete",
                        "Corrigido bug que impedia que locações fossem exibidas na agenda"
                    }),

                new VersionEntry(
                    2, 18, 6, "Acusar tipos de Frete no Carrinho", new DateTime(2018, 10, 1),
                    new []
                    {
                        "Carrinho agora lista para cada tipo de frete os produtos que são incompatíveis com ele",
                        "Retrabalhada validação de períodos de locação para sugerir períodos válidos e mais relevantes",
                        "Removido links para detalhes de produto de cobrança de orçamento no carrinho",
                        "Nova imagem padrão para produtos de cobrança de orçamento",
                        "Tabela com listagem de pedidos de um comprador agora exibe o nome comercial do vendedor",
                        "Adicionada possibilidade de definir opção máxima de parcelamento por produto",
                        "Corrigido bug na customização que poderia fazer com que algumas strings fossem deletadas",
                        "Corrigido bug que impedia que campos customizados fossem exibidos em alguns tipos de perfil de vendedor",
                        "Corrigido bug que impedia alterar taxas customizadas de alguns vendedores",
                        "Corrigido bug na listagem de categorias de portfólio em browsers mobile",
                        "Corrigido exibição incorreta de checkboxes de campos customizados em certos formulários"
                    }),

                new VersionEntry(
                    2, 18, 5, "Unidade de locação por produto", new DateTime(2018, 9, 27),
                    new []
                    {
                        "Adicionada a possibilidade de configurar unidade de locação individualmente por produto",
                        "Feature deve ser habilitada em Config.Product.BusinessModel.Rental.EnablePeriodPerProduct",
                        "Quando desligada, ou não especificado no produto, o valor global da Config é utilizado como padrão.",
                        "Adicionada estrutura para integração automática com UserVoice",
                        "Link para login é gerado ao salvar configuração, e é visível no rodapé do Painel Admin",
                        "Passa a ser visível quando a flag Config.UserVoice.ShowLink está habilitada",
                        "Média de avaliações do vendedor agora é exibida nos detalhes do orçamento"
                    }),

                new VersionEntry(
                    2, 18, 4, "Campos personalizados de cadastro", new DateTime(2018, 9, 24),
                    "E outras correções",
                    new []
                    {
                        "Nova feature permite a definição de campos personalizados para cadastro de Vendedores, Compradores e pedidos de orçamento",
                        "Feature deve ser habilitada manualmente nas flags dentro de Config.CustomFields",
                        "Quando habilitada, admin pode criar e editar campos através do painel de customização",
                        "Campos de Vendedor e Comprador são acessíveis através de botão em suas respectivas páginas no painel",
                        "Campos de Orçamento são acessíveis através de botão na aba 'Outros' do painel de customização",
                        "Alterada funcionalidade salvamento parcial de cadastros para não permitir envio pro gateway de cadastro incompleto",
                        "Produtos agora são adicionados corretamente ao carrinho após fazer login pelo Facebook",
                        "Corrigido bug que quebrava o layout da barra lateral do painel admin em telas pequenas/mobile",
                        "Corrigido bug que permitia que vendedores com perfil incompleto fossem exibidos na busca sob certas condições",
                        "Melhorias de usabilidade na listagem de orçamentos do comprador",
                        "Corrigido bug que exibia teclado em telas mobile para seleção de categorias na home",
                        "Corrigido bug que permitia que produtos de envio de cobrança fossem visíveis em seções de produto"
                    }),

                new VersionEntry(
                    2, 18, 3, "Template de e-mail de moderação", new DateTime(2018, 9, 19),
                    new []
                    {
                        "Corrigido bug na tela de detalhes do comprador painel admin",
                        "Novo template de e-mail para moderação de vendedores",
                        "Admin da plataforma agora é notificado quando há um novo cadastro aguardando moderação",
                        "Requer a flag Config.Seller.Moderation ativada",
                        "Corrigido bug em orçamento com campos personalizados"
                    }),

                new VersionEntry(
                    2, 18, 2, "Filtro de Orçamentos por Portfolio", new DateTime(2018, 9, 17),
                    new []
                    {
                        "Nova flag em Config.Quotes.RequirePortfolioForGeneralQuote",
                        "Quando ativada, adiciona um filtro adicional ao orçamento para a base",
                        "Só recebem o orçamento os vendedores que possuirem ao menos um item de portfolio que atenda a subcategoria selecionada",
                        "Corrigido bug que impedia que alguns produtos com bloqueios recorrentes fossem listados na agenda",
                        "Corrigido erro em template padrão de e-mail sobre envio de orçamento"
                    }),

                new VersionEntry(
                    2, 18, 1, "Hotfixes", new DateTime(2018, 9, 14),
                    new []
                    {
                        "Corrigido bug que permitia que vendedores com perfil incompleto recebessem pedidos de orçamento para a base",
                        "Corrigido bug que poderia atrasar envio de transferências para transações agendadas"
                    }),

                new VersionEntry(
                    2, 18, 0, "Feature - Avaliação Pública", new DateTime(2018, 9, 13),
                    "Nova funcionalidade : avaliação de usuários e outros ajustes",
                    new []
                    {
                        "Compradores e Vendedores podem se avaliar ao concluir uma negociação",
                        "Média de avaliações de um vendedor são exibidas na home, perfil, produto e tela de orçamentos",
                        "Usuários podem ver listagem completa de avaliações no perfil do vendedor",
                        "Listagem completa pode ser vista em detalhes em tabelas Painel Admin",
                        "Comentários podem ser ocultados no painel admin. Estes ainda contam para o cálculo da média.",
                        "Flag desabilitada por padrão, permite que o Admin exclua avaliações permanentemente",
                        "Cadastro de documento de vendedor agora suporta Texto Livre ou CPF e/ou CNPJ",
                        "Os tipos suportados podem ser configurados em flags em Config.Seller.DocumentNumber",
                        "Vendedor agora pode pausar o anúncio de um produto, mediante flag habilitada em config",
                        "Produtos pausados não são mais exibidos nas buscas ou perfis",
                        "Administrador pode pausar/despausar anúncios de produtos pelo painel Admin no perfil do vendedor",
                        "Corrigido apontamento incorreto de mensagem de 'Seu Perfil está Invisível'",
                        "Corrigido bug que poderia impedir alguns e-mails de serem enviados"
                    }),

                new VersionEntry(
                    2, 17, 16, "Flag para Split de Frete", new DateTime(2018, 9, 11),
                    "Nova flag na config",
                    new []
                    {
                        "Adicionada nova Flag na config : Payment.Common.EnableShippingSplitToMarketplace",
                        "Flag desligada por padrão, quando ativada permite que o valor do frete seja repassado para a Marketplace ao invés do comprador"
                    }),

                new VersionEntry(
                    2, 17, 15, "E-mail de boas vindas e hotfixes diversos", new DateTime(2018, 9, 5),
                    "Série de melhorias solicitadas",
                    new []
                    {
                        "Adicionado Template de e-mail de boas vindas",
                        "Painel de métricas não exibe mais métricas de produto em marketplaces com envio de cobrança habilitado",
                        "Labels de senhas para cadastro em orçamento substituidas por placeholders",
                        "Adicionada paginação dinâmica para painel de orçamentos do Admin",
                        "Campos customizados agora são obrigatórios, e dropdowns vem em branco",
                        "Nova mensagem personalizável é exibida quando não são encontrados resultados na busca",
                        "Esta mensagem é formatável com Markdown, permitindo links externos",
                        "Botão de \"Complete seus dados\" agora redireciona corretamente para a tela de cadastro do perfil",
                        "Corrigido bug que impedia que itens de locação com duração fixa fossem adicionados ao carrinho ao logar"
                    }),

                new VersionEntry(
                    2, 17, 14, "Painel de Customização", new DateTime(2018, 8, 29),
                    "Juntamente com melhorias e correções diversas",
                    new []
                    {
                        "Adicionado novo painel de personalização disponível para o Admin",
                        "WebAPI agora suporta requesições de domínios externos",
                        "Opção deve ser habilitada no appsettings.config",
                        "Domínios permitidos devem ser configurados na appsettings",
                        "Bloqueio de agendamento agora dá uma margem de 1 minuto",
                        "Isto permite que agendamentos sejam feitos começando no último minuto de um bloqueio, ou terminando exatamente no primeiro",
                        "Painel de métricas não exibe mais métricas de produto em marketplaces com envio de cobrança habilitado",
                        "Usuário agora pode cadastrar uma senha diretamente ao registrar Nome e Email ao pedir orçamento sem ter feito login",
                        "Comprador de marketplace de orçamentos agora é redirecionado para painel de orçamentos após login",
                        "Corrigido bug que permitia que e-mails desabilitados fossem enviados"
                    }),

                new VersionEntry(
                    2, 17, 13, "Ajustes Agendamento", new DateTime(2018, 8, 21),
                    "Melhorias diversas",
                    new []
                    {
                        "Em plataformas de agendamento/locação, produtos com duração fixa exibirão preço total ao invés de por período",
                        "Nova flag Seller.SavePartialProfiles",
                        "Permite salvar perfis com dados incompletos",
                        "Correção do campo de validação da URL do facebook",
                        "Validação dos campos da Taxa de Comissão e Comissão Fixa para aceitar preenchimento em branco"
                    }),

                new VersionEntry(
                    2, 17, 12, "Hotfix envio de cobrança", new DateTime(2018, 8, 17),
                    "Correção de erros",
                    new []
                    {
                        "Corrigido bug que exibia opção de gerar cobrança mesmo com Billing desabilitado",
                        "Telefone agora é exibido no orçamento quando enviado",
                        "Nova flag PrioritizeExpressShipping que permite priorizar envio por Sedex nas opções de frete de produto"
                    }),

                new VersionEntry(
                    2, 17, 11, "Hotfix tabela de orçamentos", new DateTime(2018, 8, 13),
                    "Correção de erros",
                    new []
                    {
                        "Corrigido potencial erro de exibição na tabela de orçamentos arquivados",
                        "Corrigida filtro por gatewayId na busca em plataformas com módulo de assinatura",
                        "Adicionada flag na config com limite máximo configurável de categorias de portfolio"
                    }),

                new VersionEntry(
                    2, 17, 10, "Hotfix busca e vitrine", new DateTime(2018, 8, 10),
                    "Correção de erros",
                    new []
                    {
                        "Corrigido erro na exibição de alguns itens na vitrine da Home",
                        "Corrigido erro que exibia vendedores e portfolios com cadastro incompleto na busca"
                    }),

                new VersionEntry(
                    2, 17, 9, "Painel de métricas V2", new DateTime(2018, 8, 9),
                    "Ajustes no painel de métricas",
                    new []
                    {
                        "Diversas alterações de usabilidade e layout no dashboard do Admin e Vendedor",
                        "Nova visualização detalhada de carrinhos abandonados",
                        "Inclui dados do vendedor e listagem completa dos itens contidos",
                        "Nova flag opcional para módulo de orçamentos que permite recusar um orçamento sem responder antes"
                    }),

                new VersionEntry(
                    2, 17, 8, "Aluguel com duração fixa e moderação de vendedores", new DateTime(2018, 8, 9),
                    "Nova opção de aluguel",
                    new []
                    {
                        "É possível habilitar aluguel/agendamento com duração fixa, caso a caso",
                        "É possível ocultar os campos de período minimo e máximo quando duração fixa habilitada",
                        "Período de locação é calculado automaticamente ao adicionar item ao carrinho",
                        "Nova opção de moderação de vendedores",
                        "Quando habilitada, perfis de vendedores são desabilitados automaticamente ao completar cadastro",
                        "Perfil deve ser ativado por moderador da plataforma",
                        "Vendedor é notificado por e-mail quando perfil for ativado"
                    }),

                new VersionEntry(
                    2, 17, 7, "Hotfixes dashboard e pedido", new DateTime(2018, 8, 8),
                    "Pequenas correções",
                    new []
                    {
                        "Corrigido erro ao exibir dashboard sem pedidos",
                        "Módulo de transação em garantia agora funciona com pedidos já existentes"
                    }),

                new VersionEntry(
                    2, 17, 6, "Feature - Painel de Métricas Avançadas", new DateTime(2018, 8, 7),
                    "Dois novos painéis e vários ajustes",
                    new []
                    {
                        "Adicionado painel de métricas avançadas do Admin",
                        "Disponível sob a config Dashboard > LevelTwo",
                        "Inclui métricas novas de produtos e orçamentos e filtro por data",
                        "Adicionado painel de métricas do vendedor",
                        "Opcional, habilitado na config Dashboard > SellerDashboard.Enabled",
                        "Quando habilitado, passa a ser a página padrão do Vendedor ao entrar na conta",
                        "Métricas modulares podendo ser habilitadas, desabilitadas e renomeadas uma a uma",
                        "É necessário concordar com os termos de uso via checkbox ao criar um cadastro",
                        "Nova opção para desligar Crop de imagens de itens de portfólio exibidos em modal",
                        "Alterada forma como categorias e subcategorias são exibidas no perfil do vendedor"
                    }),

                new VersionEntry(
                    2, 17, 5, "Hotfix Vitrine de Produtos", new DateTime(2018, 7, 31),
                    "Pequenas correções",
                    new []
                    {
                        "Corrigido bug na exibição de produtos na vitrine da home",
                        "Adicionada label separada para CPF de Comprador",
                        "Endereço do e-mail padrão modificado para usar a variavel Smtp.defaultFromAddress"
                    }),

                new VersionEntry(
                    2, 17, 4, "Hotfixes", new DateTime(2018, 7, 30),
                    "Pequenas correções",
                    new []
                    {
                        "Ajustes na página de orçamento individual",
                        "Corrigido erro que permitia que chaves duplicadas fossem inseridas na tabela de templates de email"
                    }),

                new VersionEntry(
                    2, 17, 3, "Feature - Busca de Portfolios", null,
                    "Agora é possivel pesquisar por itens de portfolio",
                    new []
                    {
                        "Novo modo de busca implementado : busca por itens de portfolio",
                        "Pode ser habilitado configurando o Search.SearchType e Search.ResultType para '2'",
                        "Quando habilitada, busca passa a retornar itens de portfolio",
                        "Itens redirecionam para perfil público do vendedor com foco no item selecionado",
                        "Filtros podem ser por busca na descrição e título, localização do vendedor e categoria/subcategoria"
                    }),

                new VersionEntry(
                    2, 17, 2, "Feature - Campos Personalizados Orçamento", null,
                    "Novos campos personalizados podem ser adicionados para casos específicos",
                    new []
                    {
                        "Estrutura pronta para implementar campos personalizados em outras Entities no futuro",
                        "No momento, implementado apenas para Orçamentos (Quotes)",
                        "Definição de campos precisa ser feita manualmente no banco de dados"
                    }),

                new VersionEntry(
                    2, 17, 1, "Feature - Seção Produtos Custom", null,
                    "Seção com categorias e itens personalizáveis",
                    new []
                    {
                        "Categorias são criadas na seção 'Home do Ecommerce' do painel admin",
                        "Podem ser exibidas em Carrossel ou de forma Extendida",
                        "Itens personalizado podem ter datas de ínicio e fim de exibição programáveis",
                        "Devem conter um título, uma imagem e um link"
                    }),

                new VersionEntry(
                    2, 17, 0, "Feature - Transação em Garantia", null,
                    "Nova feature opcional que quando ativada muda o fluxo de pagamento e recebimento",
                    new []
                    {
                        "A feature pode ser ativada na Config Payment.Common.EnableTransactionWarranty",
                        "Quando ligada, pagamento por compras de produtos ou cobrança de orçamentos é retido na plataforma",
                        "Pagamento passa a ser liberado para o vendedor mediante autorização do comprador ou automaticamente após certo prazo",
                        "Prazo de liberação automática é configurável - Payment.Common.DeadlineAutoExtensionDays - 14 dias por padrão",
                        "Comprador é notificado com antecedência quando a liberação automática está prestes a acontecer",
                        "Número de dias de antecedência configurável - Payment.Common.OrderConclusionNotificationDays - 3 dias por padrão",
                        "Em caso de irregularidade, as partes envolvidas podem iniciar uma disputa para conversar e resolver o problema",
                        "Na tela de disputa o pagamento pode ser liberado ou estornado, e mediação de admins da plataforma pode ser solicitada",
                        "Disputas em andamento podem ser visualizadas no painel admin"
                    }),

                new VersionEntry(
                    2, 16, 18, "Filtro por Cidade para Orçamento Geral", new DateTime(2018, 7, 24),
                    "Agora é possível filtrar vendedores por geolocalização",
                    new []
                    {
                        "Re-estruturação do formulário de orçamento geral",
                        "Novo campo opcional de filtro por cidade para orçamento geral",
                        "Quando preenchido, envia orçamento apenas para os vendedores dentro da região selecionada",
                        "Campo pode ser ligado na config Quote.GeneralQuoteCity"
                    }),

                new VersionEntry(
                    2, 16, 17, "Hotfixes Orçamentos", new DateTime(2018, 7, 23),
                    "Corrigidas pequenas inconsistências com módulo de Envio de Cobrança",
                    new []
                    {
                        "Seção de produtos no perfil do vendedor agora é ocultada juntamente com os menus de Produto",
                        "Perfil Incompleto de vendedor não é mais exibido na Home sob a seção 'Melhores Vendedores'"
                    }),

                new VersionEntry(
                    2, 16, 16, "Hotfix ActualNoShipping", new DateTime(2018, 7, 23),
                    "Corrigida validação de configuração",
                    new []
                    {
                        "Corrigido bug na validação de Config que não permitia que o ActualNoShipping fosse o único método de frete habilitado"
                    }),

                new VersionEntry(
                    2, 16, 15, "Redirect para HTTPS", new DateTime(2018, 7, 18),
                    "Redirecionametno automático para HTTPS",
                    new []
                    {
                        "Caso config.environment.appUrl especifique uma URL com HTTPS",
                        "No redirect, mantém todas as variáveis (Query String) da requisição"
                    }),

                new VersionEntry(
                    2, 16, 14, "Flag de Testes Pagar.me", new DateTime(2018, 7, 18),
                    "Nova Flag com modo de validação para o pagar.me e outros pequenos ajustes:",
                    new []
                    {
                        "Flag opcional em Config.Payment.Pagarme.UndergoingValidation",
                        "Quando ligada, ignora restrições de assinatura para usuários da plataforma",
                        "Usuários terão perfil e produtos visíveis e poderão receber pedidos e orçamentos normalmente mesmo sem assinatura",
                        "Exceto o usuário de teste configurado em Config.Payment.Pagarme.validationAccountEmail, que terá o fluxo normal"
                    }),

                new VersionEntry(
                    2, 16, 13, "Endpoints configuração remota", new DateTime(2018, 7, 16),
                    "Agiliza configurações automáticas no AWS",
                    new []
                    {
                        "Permite setar configurações como AppUrl, Cdn, Storage e SMTP",
                        "Inicializa configurações do Pagar.me com conta teste do INA",
                        "Corrige bug que não processava imagens como home bg pelo handler cloudimage"
                    }),

                new VersionEntry(
                    2, 16, 12, "Hotfix Ícones", new DateTime(2018, 7, 13),
                    "Pequenos ajustes:",
                    new []
                    {
                        "Corrigidos ícones invisíveis de editar imagem de perfil",
                        "Corrigido bug que fazia botão de aceitar proposta de orçamento sumir ao enviar uma mensagem"
                    }),

                new VersionEntry(
                    2, 16, 11, "Hotfix agendamento", new DateTime(2018, 7, 9),
                    "Corrigida exibição de avisos de restrição de locação",
                    new []
                    {
                        "Corrigido erro no script que impedia que textos fossem atualizados corretamente"
                    }),

                new VersionEntry(
                    2, 16, 10, "Importação de Planos de Assinatura", new DateTime(2018, 7, 5),
                    "Planos obsoletos são excluídos ao importar",
                    new []
                    {
                        "Ao importar planos de assinatura do Gateway pelo painel admin:",
                        "Planos que não existem mais no gateway são excluídos automaticamente"
                    }),

                new VersionEntry(
                    2, 16, 9, "Hotfix AWS storage", new DateTime(2018, 7, 10),
                    "Revisão para identificar se usa Azure Storage ou Amazon S3",
                    new []
                    {
                        "Se chaves de S3 estiverem preenchidas no appsettings.config, usa S3"
                    }),

                new VersionEntry(
                    2, 16, 8, "Hotfix método frete carrinho", new DateTime(2018, 7, 9),
                    "Método de frete era sempre setado para o primeiro disponível",
                    new []
                    {
                        "Se o frete atual for válido, mantém. Do contrário, aplica regra que troca para primeiro disponível"
                    }),

                new VersionEntry(
                    2, 16, 7, "Remove logo INA páginas de erro", new DateTime(2018, 7, 4),
                    "Remove logo INA páginas de erro",
                    new []
                    {
                        "Páginas /error.html, /error, /error/serviceunavailable.html, /error/pagenotfound"
                    }),

                new VersionEntry(
                    2, 16, 6, "Path do envio de cobranças", new DateTime(2018, 7, 6),
                    "Várias melhorias pequenas e correções",
                    new []
                    {
                        "Revisado o fechamento de pagamento de cobrança",
                        "Atrela corretamente o pedido à cobrança do orçamento",
                        "Cobrança é corretamente atualizada quando pedido é pago, cancelado, estornado"
                    }),

                new VersionEntry(
                    2, 16, 5, "Envio de cobrança de orçamentos", new DateTime(2018, 7, 6),
                    "O vendedor pode transformar o orçamento em uma cobrança",
                    new []
                    {
                        "Mediante uma proposta formal (preço e prazo) aceita pela comprador",
                        "Comprador recebe o link de cobrança por email e pode acessar tabém pelo site",
                        "Fluxo de pagamento usa o mesmo do módulo de produtos - cobrança é um produto estoque 1"
                    }),

                new VersionEntry(
                    2, 16, 4, "Hotfix excluir portfólio", new DateTime(2018, 7, 4),
                    "Erro ao excluir item de portfólio",
                    new []
                    {
                        "Erro estava relacionado às categorias associadas ao item de portfólio",
                        "Exclui associação do item sendo excluído com as categorias"
                    }),

                new VersionEntry(
                    2, 16, 3, "Hotfix pagar.me", new DateTime(2018, 7, 5),
                    "Correção para viabilizar cadastro",
                    new []
                    {
                        "Vendedor consegue cadastrar na plataforma mesmo se comunicação com gateway falhar",
                        "Forçado uso exclusivo de protocolo TLS 1.2 quando pagar.me habilitado ",
                        "Atualizada versão da biblioteca do pagar.me"
                    }),

                new VersionEntry(
                    2, 16, 2, "Modo frete \"sem entrega\"", new DateTime(2018, 7, 3),
                    "Permite configurar um produto sem envio (de verdade)",
                    new []
                    {
                        "Novo método de envio ActualNoShipping (sem entrega (de verdade))",
                        "Ao habilitar em config, fica disponível com checkbox no cadastro do produto",
                        "Funcionamento similar ao produto digital, mas sem necessidade de URL de download"
                    }),

                new VersionEntry(
                    2, 16, 1, "Hotfix", new DateTime(2018, 7, 4),
                    "Erro ao acessar portfólio no perfil público do vendedor",
                    new []
                    {
                        "Portfólio do vendedor gerava erro por perfil incompleto"
                    }),

                new VersionEntry(
                    2, 16, 0, "Feature: Comprador Aceita Orçamentos", new DateTime(2018, 6, 14),
                    "Nova funcionalidade opcional permite que comprador aceite ou cancele orçamentos",
                    new []
                    {
                        "Funcionalidade pode ser ligada ou desligada na config Quote.BuyerDecision",
                        "Quando ligada, o fluxo de orçamento muda para funcionar da seguinte forma:",
                        "Comprador solicita um orçamento",
                        "Vendedor formaliza uma proposta enviando uma resposta com Prazo e Preço",
                        "Vendedor não pode mais fechar o orçamento como ganho, mas ainda pode rejeitá-lo",
                        "Comprador pode cancelar o orçamento a qualquer momento, ou aceitar a última proposta enviada",
                        "Ao aceitar uma proposta, ela é fechada como 'ganha', e o vendedor é notificado",
                        "Caso seja um orçamento para a base, os orçamentos individuais para os outros vendedores são fechados como perdidos",
                        "Os vendedores são notificados por e-mail"
                    }),

                new VersionEntry(
                    2, 16, 0, "Feature: Comprador Aceita Orçamentos", new DateTime(2018, 6, 14),
                    "Nova funcionalidade opcional permite que comprador aceite ou cancele orçamentos",
                    new []
                    {
                        "Funcionalidade pode ser ligada ou desligada na config Quote.BuyerDecision",
                        "Quando ligada, o fluxo de orçamento muda para funcionar da seguinte forma:",
                        "Comprador solicita um orçamento",
                        "Vendedor formaliza uma proposta enviando uma resposta com Prazo e Preço",
                        "Vendedor não pode mais fechar o orçamento como ganho, mas ainda pode rejeitá-lo",
                        "Comprador pode cancelar o orçamento a qualquer momento, ou aceitar a última proposta enviada",
                        "Ao aceitar uma proposta, ela é fechada como 'ganha', e o vendedor é notificado",
                        "Caso seja um orçamento para a base, os orçamentos individuais para os outros vendedores são fechados como perdidos",
                        "Os vendedores são notificados por e-mail"
                    }),

                new VersionEntry(
                    2, 15, 34, "Categorias nos itens de portfólio", new DateTime(2017, 7, 2),
                    "Permite associar categorias aos itens de portfólio",
                    new []
                    {
                        "Adiciona os campos para cadastro de categorias no item de portfólio",
                        "Campos são alimentados pela mesma lista de categorias criadas para o vendedor",
                        "Categorias associadas aparecem na modal do item de portfólio no perfil público"
                    }),

                new VersionEntry(
                    2, 15, 33, "Refatoração de web.config", new DateTime(2018, 6, 28),
                    "Web.config dividido em dois arquivos adicionais",
                    new []
                    {
                        "Adicionados arquivos appsettings.config e connstrings.config complementares ao web.config",
                        "Arquivos não-versionados, com .defaults versionados",
                        "Correções ortográficas na tela de portfolio na view do Admin",
                        "Avatar do autor da última resposta de um orçamento agora é exibido corretamente",
                        "Links Adicionais agora aparecem no menu mobile"
                    }),

                new VersionEntry(
                    2, 15, 32, "Correções e otimizações diversas", new DateTime(2018, 6, 26),
                    "Diversas correções e otimizações",
                    new []
                    {
                        "Novo campo no Config.Intercom para informar Nome e Id da companhia ",
                        "Removidas referências e integrações com a Precode",
                        "Removida Config obsoleta 'Integrator'",
                        "Página de customização HTML pré-compilada",
                        "Regra de Redirect AWS no web.config"
                    }),

                new VersionEntry(
                    2, 15, 31, "Correções e ajustes: portfolio e busca", new DateTime(2018, 6, 22),
                    new []
                    {
                        "Descrição de portfólio agora aceita formatação por Markdown",
                        "Correção no layout de campos de busca",
                        "Página de produtos agora mostra o período relativo ao preço quando módulo de aluguel está habilitado"
                    }),

                new VersionEntry(
                    2, 15, 30, "Bug globalization helper", new DateTime(2018, 6, 20),
                    "Corrigido bug no helper de localização que impedia que algumas strings fossem exibidas corretamente",
                    new []
                    {
                        "Métodos ambíguos estavam causando comportamento inesperado e foram refatorados"
                    }),

                new VersionEntry(
                    2, 15, 29, "Bug custom link", new DateTime(2018, 6, 19),
                    "Corrigido bug que impedia links personalizados de serem mostrados apropriadamente",
                    new []
                    {
                        "Ajustes na partial de cabeçalho. Links agora sao exibidos corretamente"
                    }),

                new VersionEntry(
                    2, 15, 28, "Bugs módulo de aluguel", new DateTime(2018, 6, 14),
                    "Corrigido bug na exibição de agenda de bloqueios e estoque de produtos para aluguel",
                    new []
                    {
                        "Agenda de bloqueios não estava exibindo corretamente devido a exceção interna",
                        "Seleção de estoque de produtos para aluguel estava sendo exibido mesmo quando opção estava desligada"
                    }),

                new VersionEntry(
                    2, 15, 27, "Bug Seleção de TimeZones", new DateTime(2018, 6, 11),
                    "Corrigido bug que não listava corretamente as timezones",
                    new []
                    {
                        "Time Zones não estavam sendo listadas para conclusão de cadastro quando era feito login externo (facebook)"
                    }),

                new VersionEntry(
                    2, 15, 26, "Hotfix portfólio sem orçamento", new DateTime(2018, 6, 11),
                    "Corrigido bug que ocultava portfólio caso módulo Quote estivesse desligado",
                    new []
                    {
                        "Portfólio agora é exibido corretamente mesmo sem módulo de orçamento habilitado"
                    }),

                new VersionEntry(
                    2, 15, 25, "Hotfix registro de vendedores", new DateTime(2018, 6, 8),
                    "Corrigido bug no registro de vendedores",
                    new []
                    {
                        "Corrigido bug: URL amigável do vendedor não estava sendo gerada ao cadastrar"
                    }),

                new VersionEntry(
                    2, 15, 24, "Url Amigável do Vendedor", new DateTime(2018, 6, 8),
                    "Vendedor agora possui URL Amigável",
                    new []
                    {
                        "Agora é utilizada a URL amigável para referenciar o perfil do vendedor",
                        "Url antiga ainda funciona normalmente para fins de retrocompatibilidade"
                    }),

                new VersionEntry(
                    2, 15, 23, "Busca de Vendedor por descrição", new DateTime(2018, 6, 8),
                    "Query agora busca na descrição",
                    new []
                    {
                        "Agora os resultados de busca por vendedor incluem aqueles onde a query foi encontrada na descrição",
                        "Antes esta busca era feita levando em conta apenas o nome do vendedor"
                    }),

                new VersionEntry(
                    2, 15, 22, "Videos de outras fontes no Portfolio", new DateTime(2018, 6, 7),
                    "Adicionado suporte a diversas fontes de vídeos",
                    new []
                    {
                        "As seguintes fontes de vídeo agora tem suporte a Embed e geração automática de thumbnail",
                        "YouTube, Vimeo, Dailymotion, Facebook, Twitter, Instagram, Imgur, Twitch ",
                        "Também é possível especificar uma Thumbnail personalizada opcional",
                        "Fontes não reconhecidas exibirão apenas um link para visita externa"
                    }),

                new VersionEntry(
                    2, 15, 21, "Hotfix Ordenação Listas", new DateTime(2018, 6, 7),
                    "Corrigida ordenação de Subcategorias na home",
                    new []
                    {
                        "Bug ocorria em algumas marketplaces na hora de comparar elementos para ordenar lista"
                    }),

                new VersionEntry(
                    2, 15, 20, "Custom HTML", new DateTime(2018, 6, 8),
                    "Permite inserir tags HTML na plataforma",
                    new []
                    {
                        "Possibilidade de customizar a plataforma com códigos HTML no head, bem como início e fim do body",
                        "Configuração para mostrar parte da descrição do vendedor nos resultados da busca"
                    }),

                new VersionEntry(
                    2, 15, 19, "Hotfix Subcategoria Orçamentos", new DateTime(2018, 5, 30),
                    "Corrigida Seleção de Subcategorias na tela de Orçamentos",
                    new []
                    {
                        "Eram exibidas todas as subcategorias, e não só as habilitadas pelo vendedor",
                        "Agora na tela de solicitar orçamento são exibidas somente as subcategorias habilitadas pelo vendedor"
                    }),

                new VersionEntry(
                    2, 15, 18, "Hotfix Intercom", new DateTime(2018, 5, 29),
                    "Não aparecia mais em alguns admins",
                    new []
                    {
                        "Script movido para Partial View"
                    }),

                new VersionEntry(
                    2, 15, 17, "Links opcionais customizáveis", new DateTime(2018, 5, 28),
                    "Novos botões de links personalizáveis",
                    new []
                    {
                        "Adicionado campo AdditionalLinks na config",
                        "Pode conter até 3 links personalizados"
                    }),

                new VersionEntry(
                    2, 15, 16, "Revisão texto contas bancárias", new DateTime(2018, 5, 28),
                    "Conforme indicações do Pagar.me",
                    new []
                    {
                        "No cadastro de vendedor, textos explicativos e dos campos foram revisados"
                    }),

                new VersionEntry(
                    2, 15, 15, "Ajustes módulo de orçamento", new DateTime(2018, 5, 25),
                    "Novo campo no e-mail e informações de contato",
                    new []
                    {
                        "Adicionado campo [ANSWER] no e-mail de notificação de resposta de orçamento",
                        "Nova opção no Config: Quote.ShowBuyerContactInfo",
                        "Quando habilitada exibe os dados de contato do comprador para o vendedor"
                    }),

                new VersionEntry(
                    2, 15, 14, "Busca de produtos por geolocalização", new DateTime(2018, 5, 25),
                    "Busca aceita filtro de locação com autocomplete do Google Maps",
                    new []
                    {
                        "Busca de produtos por localização agora usa autocomplete do GoogleMaps",
                        "Resultados são filtrados de acordo com a localidade escolhida e a distância especificada na Config",
                        "Agora é possível ser mais específico na localização e obter resultados mais direcionados"
                    }),

                new VersionEntry(
                    2, 15, 13, "Resolução merges", new DateTime(2018, 5, 24),
                    "Conflitos problemáticos entre master e deployment",
                    new []
                    {
                        "Reset do .genared.cs de algumas views"
                    }),

                new VersionEntry(
                    2, 15, 12, "Hotfix busca", new DateTime(2018, 5, 24),
                    "Razor Generator não havia gerado todas as views",
                    new []
                    {
                        "11 views afetadas, busca entre elas"
                    }),

                new VersionEntry(
                    2, 15, 11, "Ajustes locação produtos", new DateTime(2018, 5, 23),
                    "",
                    new []
                    {
                        "Agrupa pedidos no carrinho de acordo com datas de frete (envio e retorno)",
                        "Ajuste no frete para o modo que opera frete por vendedor (ao invés de por produto)"
                    }),

                new VersionEntry(
                    2, 15, 10, "Hotfixes Diversos", new DateTime(2018, 5, 23),
                    "Corrigido bug em template de e-mail e ordenamento de tabela",
                    new []
                    {
                        "Adicionada a variavel [ORDER_LINK] em alguns templates de e-mail que estavam sem",
                        "Corrigido ordenamento incorreto por data na tabela de orçamentos do painel Admin"
                    }),

                new VersionEntry(
                    2, 15, 9, "Action Interna GatewayIds (22/05)", null,
                    "Criada nova action para recriar GatewayIds",
                    new []
                    {
                        "Nova action interna pode ser chamada através da rota /Internal/ReRegisterUsersOnGateway?key={chave} ",
                        "Recadastra todos os Vendedores e Compradores no Gateway de pagamento, gerando novos IDs",
                        "Para ser usada ao trocar a chave de API do Gateway"
                    }),

                new VersionEntry(
                    2, 15, 8, "Feature Taxas Personalizadas", new DateTime(2018, 5, 18),
                    "Novas Configurações que permitem definir taxas e chargeback personalizados ou globais",
                    new []
                    {
                        "Refatoramento das configurações. Variáveis compartilhadas foram movidas do PagarMe para Payment.Common",
                        "Agora é possível definir taxas e responsabilidade de chargeback globais",
                        "Nova opção de taxas personalizadas, quando ligada permite sobrescrever taxas globais com valores individuais por vendedor"
                    }),

                new VersionEntry(
                    2, 15, 7, "Hotfix Carrossel Home", new DateTime(2018, 5, 18),
                    "Corrigido bug que fazia imagens desaparecerem no Chrome",
                    new []
                    {
                        "Problema foi solucionado adicionando um snippet corretivo ao .css do Owl Carousel"
                    }),

                new VersionEntry(
                    2, 15, 6, "Hotfix busca vendedores", new DateTime(2018, 5, 16),
                    "Erro para renderizar resultados",
                    new []
                    {
                        "Problema era JSON com formato incorreto (escape de aspas)"
                    }),

                new VersionEntry(
                    2, 15, 5, "Hotfix login Facebook", new DateTime(2018, 5, 15),
                    "Parou de funcionar após campo TimeZone",
                    new []
                    {
                        "Tela precisava receber lista de timezones possíveis do servidor p/ renderizar form"
                    }),

                new VersionEntry(
                    2, 15, 4, "Ajustes da Locação de Produtos", new DateTime(2018, 5, 15),
                    "Conforme testes funcionais do cliente",
                    new []
                    {
                        "Configuração para determinar se permite locação por período fracionado",
                        "Sistema respeita bloqueios na agenda do vendedor",
                        "Frete \"Retirar no local\" indica que o cliente ainda poderá revisar local antes de confirmar",
                        "Agenda de locação mostra mesmo os produtos que tenham sido excluídos"
                    }),

                new VersionEntry(
                    2, 15, 3, "Refatoração da busca de produtos por cidade", new DateTime(2018, 5, 15),
                    "Busca de produtos por cidade",
                    new []
                    {
                        "A busca de produtos por cidade agora retorna resultados mais consistentes",
                        "É possivel filtrar resultados através de um parâmetro opcional de retirada no local",
                        "Habilitado Intercom no Painel Admin para todas as marketplaces",
                        "Otimizada e refatorada a busca de coordenadas por nome de cidade",
                        "Buscar um produto por cidade agora mostra todos os vendedores que entregam na cidade escolhida",
                        "Essa busca leva em consideração todos os métodos de entrega disponíveis"
                    }),

                new VersionEntry(
                    2, 15, 2, "Reabilitar módulo Vendas de Produtos", new DateTime(2018, 5, 14),
                    "Correção de bug",
                    new []
                    {
                        "Módulo de vendas de produtos não mostrava botão p/ Adicionar ao Carrinho"
                    }),

                new VersionEntry(
                    2, 15, 1, "Ajustes do Aluguel de Produtos", new DateTime(2018, 5, 9),
                    "Produtos agora podem ser alugados ao invés de vendidos",
                    new []
                    {
                        "Melhoria dos textos e descrições",
                        "Melhoria da interface de criação de bloqueios na agenda do vendedor",
                        "Agenda do vendedor usa cores para discrimidar locações OK, locações em atraso, e bloqueios",
                        "Agenda do vendedor aceita filtro por produto (somente locações e bloqueios do produto indicado)",
                        "Correções necessárias com base em testes da funcionalidade"
                    }),

                new VersionEntry(
                    2, 15, 0, "Aluguel de Produtos", new DateTime(2018, 5, 7),
                    "Produtos agora podem ser alugados ao invés de vendidos",
                    new []
                    {
                        "Novas configurações para definir modelo de negócio de produtos como aluguel",
                        "Novos campos no cadastro de produto e na compra para configurar o aluguel",
                        "Marketplace determina unidade de tempo das locações entre Horas, Dias e Meses",
                        "Vendedor tem agenda para visualizar locações",
                        "Vendedor pode configurar bloqueios de data para um produto ou toda a loja",
                        "Vendedor gerencia devolução dos produtos locados através dos status do pedido",
                        "Vendedor pode configurar frete duplicado, considerando envio e devolução dos itens",
                        "Comprador configura a locação por campos de data, no estilo Airbnb (início e fim)",
                        "Plataforma valida disponibilidade do produto, considerando estoque, locações e bloqueios",
                        "Em caso de indisponibilidade, plataforma sugere próxima data disponível",
                        "Plataforma separa itens da locação em pedidos distintos conforme datas de locação",
                        "Plataforma envia emails de lembrete da locação (início e fim) para o Comprador",
                        "Usuário informa TimeZone no cadastro, podendo alterar em Minha Conta",
                        "TimeZone padrão é Brasilia (UTC-03:00)",
                        "Locações e bloqueios aparecem no horário localizado à TimeZone do usuário"
                    }),

                new VersionEntry(
                    2, 14, 33, "Hotfix", new DateTime(2018, 5, 11),
                    "Última resposta de orçamento não aparecia na lista para o comprador",
                    new []
                    {
                        "Corrigida query que não retornava os IDs necessários para identificar a última resposta"
                    }),

                new VersionEntry(
                    2, 14, 32, "Hotfix", new DateTime(2018, 5, 10),
                    "Cadastro de e-mails com hífens",
                    new []
                    {
                        "Corrigido um bug no userManager que não permitia que e-mails com hífens fossem cadastrados"
                    }),

                new VersionEntry(
                    2, 14, 31, "Hotfix", new DateTime(2018, 5, 10),
                    "Tags HTML em variáveis de Template de Email",
                    new []
                    {
                        "Agora é possivel inserir tags HTML nas variáveis de template de e-mail",
                        "Tags de imagem agora são inseridas automaticamente para a variável APP_LOGO, quando aplicável"
                    }),

                new VersionEntry(
                    2, 14, 30, "Hotfix", new DateTime(2018, 5, 9),
                    "Mudança indesejada de status de pedido",
                    new []
                    {
                        "Não é mais possível definir o status 'retornado' a partir de 'esperando entrega' ou 'esperando retirada'"
                    }),

                new VersionEntry(
                    2, 14, 29, "Hotfix", new DateTime(2018, 5, 9),
                    "Impedir seleção de categoria que o vendedor não atende no orçamento",
                    new []
                    {
                        "Tela de orçamento não inclui mais categoria padrão no menu drop-down",
                        "Era possível visualizar todas as subcategorias, mesmo não relevantes, pela categoria padrão"
                    }),

                new VersionEntry(
                    2, 14, 28, "Campo de Telefone do Comprador no E-mail de novo pedido", new DateTime(2018, 5, 8),
                    new []
                    {
                        "Adicionado novo campo de variável ao template de e-mail \"Vendedor, novo pedido\""
                    }),

                new VersionEntry(
                    2, 14, 27, "Hotfix", new DateTime(2018, 5, 8),
                    "E-mail de Pagamento Processado não estava sendo enviado",
                    new []
                    {
                        "Faltava o respectivo event handler.",
                        "Comprador e vendedor serão devidamente notificados agora."
                    }),

                new VersionEntry(
                    2, 14, 26, "Hotfix", new DateTime(2018, 5, 7),
                    "Erro ao carregar imagens da busca",
                    new []
                    {
                        "Erro ocorria quando havia algum vendedor com um apóstrofe no nome (')"
                    }),

                new VersionEntry(
                    2, 14, 25, "Hotfix", new DateTime(2018, 4, 27),
                    "Erro ao adicionar produto no carrinho sem login",
                    new []
                    {
                        "Tentava adicionar o produto pela URL amigável, mas deve ser pelo ID",
                        "Código CSHTML estava correto, mas @Html.Hidden renderizava input incorretamente",
                        "Correção foi gerar tag input manualmente"
                    }),

                new VersionEntry(
                    2, 14, 24, "Frete único por vendedor via Correios", new DateTime(2018, 4, 26),
                    "Plataforma agrupa produtos do vendedor para envio pelos Correios",
                    new []
                    {
                        "Vendedor determina se realiza envio pelos Correios",
                        "Plataforma calcula peso cúbico para estimar o frete pelos Correios"
                    }),

                new VersionEntry(
                    2, 14, 23, "URL amigável produto", new DateTime(2018, 4, 25),
                    "Produtos passam a responder por URL amigável",
                    new []
                    {
                        "URL's amigáveis são calculadas automaticamente com base no título do produto",
                        "Uma vez geradas, não se alteram mais (ficam somente leitura)",
                        "Anúncio de produto continua respondendo por ID (p/ retro-compatibilidade)"
                    }),

                new VersionEntry(
                    2, 14, 22, "Hotfix", new DateTime(2018, 4, 20),
                    "Correções gerais",
                    new []
                    {
                        "ImageInterceptor passa a retentar acesso a arquivos de img bloqueados por outro processo",
                        "Bloqueia acesso do admin à página de orçamentos do Comprador e Vendedor",
                        "Correção do widget de editar img no Minha Conta de acordo com tamanho da tela",
                        "Correção na busca de produtos sem categoria (null)",
                        "Formatação de preço de produtos inclui separador de milhar (ex. 1890 -> 1.890)",
                        "Vendedor precisa registrar ao menos uma resposta de orçamento p/ que possa finalizá-lo"
                    }),

                new VersionEntry(
                    2, 14, 21, "Email unificado orçamento geral", new DateTime(2018, 4, 19),
                    "Unifica emails do comprador",
                    new []
                    {
                        "Antes, enviava ao Comprador um email para cada Vendedor envolvido, chegamos a 96 emails",
                        "Novo template de email para quando o pedido é para toda a base",
                        "O novo template aponta para a página de detalhes do grupo de orçamentos (lista de vendedores envolvidos)"
                    }),

                new VersionEntry(
                    2, 14, 20, "Deploy automático V2", new DateTime(2018, 4, 16),
                    "Ajuste da entrega contínua via Github",
                    new []
                    {
                        "Commits para o branch 'deployment' passam a ser notificados p/ um serviço intermediário",
                        "Configuração para desabilitar menu de Customização do Admin"
                    }),

                new VersionEntry(
                    2, 14, 19, "Deploy automático", new DateTime(2018, 4, 11),
                    "Configuração para entrega contínua",
                    new []
                    {
                        "Projeto movido para o Github, possibilitando configuração no Azure para entrega contínua",
                        "Commits para o branch 'deployment' passam a ser publicados automaticamente"
                    }),

                new VersionEntry(
                    2, 14, 18, "Melhorias", new DateTime(2018, 4, 11),
                    "Melhorias e correções",
                    new []
                    {
                        "Na busca por localização, retornar itens dentro dos limites geográficos da localização",
                        "Libera fechamento da compra em mktplace de produto sem gateway de pgto (pgto offline)"
                    }),

                new VersionEntry(
                    2, 14, 17, "Melhorias", new DateTime(2018, 4, 6),
                    "Melhorias e correções",
                    new []
                    {
                        "Separar as chaves de API do Google Maps entre server e client",
                        "Desbloquear campo Cidade UF no Minha Conta do Comprador",
                        "Restringir subcategorias nos filtros da busca de acordo c/ Categoria",
                        "Revisar layout do portfólio do Vendedor, melhorando destaque de informações"
                    }),

                new VersionEntry(
                    2, 14, 16, "Melhorias", new DateTime(2018, 4, 5),
                    "Melhorias e correções",
                    new []
                    {
                        "Agrupar pedido de orçamento geral, visão do Comprador",
                        "Abrir mais espaço no painel de Comprador e Vendedor",
                        "Validar email de suporte da plataforma, que deve estar preenchido",
                        "Resolver erro ao lançar DomainEventHandlerException",
                        "Configurar imagem do Desenvolvido por Ideia no Ar no rodapé"
                    }),

                new VersionEntry(
                    2, 14, 15, "Hotfix", new DateTime(2018, 3, 29),
                    "Correções gerais",
                    new []
                    {
                        "Alteração no cadastro de endereço do vendedor p/ geolocalizar corretamente",
                        "Limpa cache do Minha Conta após alterar assinatura (assinar, cancelar, etc)",
                        "Problemas ao editar categorias de produtos p/ home (parecia não atualizar)",
                        "Inclusão de todos os bancos brasileiros no cadastro de conta bancária"
                    }),

                new VersionEntry(
                    2, 14, 14, "Hotfix", new DateTime(2018, 3, 21),
                    "Dropdown de subcategorias não funcionava na página de busca",
                    new []
                    {
                        "Correção do bug"
                    }),

                new VersionEntry(
                    2, 14, 13, "Merge com feature de Editar Emails", new DateTime(2018, 3, 20),
                    "Habilita a funcionalidade de editar emails transacionais",
                    new []
                    {
                        "Merge com versão 2.14.10",
                        "Garante criação dos containers do Azure Storage após configuração",
                        "Lista de emails editáveis conforme módulos ativos no admin"
                    }),

                new VersionEntry(
                    2, 14, 12, "Hotfix para reconhecimento Clear Sale", new DateTime(2018, 3, 16),
                    "Trabalha com reconhecimento proativo",
                    new []
                    {
                        "Não estorna pedidos que já tenha sido entregues (tarde demais)",
                        "Consulta proativamente a Clear Sale várias vezes ao dia para pedidos pendentes de aprovação",
                        "Admin: melhorias na forma de mostrar lista e detalhes de pedidos",
                        "Admin: menu adota padrão slim, abrindo mais espaço para a visão principal/central",
                        "Melhoria na página de erros: possível visualizar o que houve no fonte da página"
                    }),

                new VersionEntry(
                    2, 14, 11, "Adaptação BD e Storage para AWS", new DateTime(2018, 3, 12),
                    "Esforço de migração de Azure para AWS",
                    new []
                    {
                        "Banco de dados passa a aceitar configuração para trabalhar com Amazon RDS",
                        "Cloud storage passa a aceitar configuração para trabalhar com Amazon S3",
                        "Correção para mostrar corretamente botão de pedido de orçamento direto p/ vendedor",
                        "Correção de dependências quando opera pagamentos com Pagarme",
                        "Correção do bug de upload de imagens de produto",
                        "Redefinição de configurações padrão de mapas e painel financeiro (desabilitados por padrão)"
                    }),

                new VersionEntry(
                    2, 14, 10, "Personalização de Emails V1", new DateTime(2018, 3, 19),
                    "Permite ao empreendedor personalizar emails transacionais",
                    new []
                    {
                        "Novo motor de templates de email, com variáveis fixas, sendo salvos no banco de dados",
                        "Nova opção no painel admin para personalização de emails"
                    }),

                new VersionEntry(
                    2, 14, 9, "Visão Orçamentos Admin", new DateTime(2018, 3, 6),
                    "Visão para usuário admin",
                    new []
                    {
                        "Admin pode visualizar pedidos de orçamento",
                        "Listagem traz visão geral; detalhes mostram mensagens e anexos trocados",
                        "Correção imagem compradores e vendedores sem foto no painel admin"
                    }),

                new VersionEntry(
                    2, 14, 8, "Painel Financeiro V1", new DateTime(2018, 3, 5),
                    "Visão para usuário admin",
                    new []
                    {
                        "Admin pode visualizar as movimentações financeiras (realizadas e previstas)",
                        "Listagem por parcela, por vendedor, por data",
                        "Transferências automáticas desativadas, pendentes de mais validações"
                    }),

                new VersionEntry(
                    2, 14, 7, "Hotfix", new DateTime(2018, 2, 28),
                    "Correções gerais",
                    new []
                    {
                        "Correção de bug ao acessar cálculo de frete sem login no anúncio do produto",
                        "Correção no carregamento de imagens e alinhamento de spots na busca de vendedores",
                        "Correção na validação de emails para o cadastro de vendedor e comprador",
                        "SEO: títulos na home transformados em tags H1 e H2",
                        "Revisão do upload de imagens em todas as áreas (produtos, portfolio, usuário, etc.)"
                    }),

                new VersionEntry(
                    2, 14, 6, "Hotfix", new DateTime(2018, 2, 23),
                    "Correções gerais",
                    new []
                    {
                        "Permite definir manualmente o pedido como Pago em caso de pgto offline",
                        "Envia corretamente o email de link p/ download após pgto de produtos digitais",
                        "Adiciona os bancos Banrisul, Sicredi e Sicoob para recebimento pelo Vendedor",
                        "Página de erro mais amigável, informando que fomos notificados por email",
                        "Carrinho mostra somente métodos de entrega possíveis para a compra",
                        "Oculta corretamente campos da busca que foram desabilitados",
                        "Correção no sistema de propagação e processamento de eventos no sistema",
                        "Adiciona automaticamente categorias de exemplo ao criar o banco de dados",
                        "Melhor formatação dos emails de erro que chegam para o sysadmin no Ideia no Ar",
                        "Refactoring de IBus para IDomainEventDispatcher",
                        "Refactoring da grande classe que tratava OrderStatusChanged para handlers menores",
                        "Refactoring de UnitOfWork para conter os repositórios",
                        "Refactoring de todos os eventos para serem roteados por IDomainEventDispatcher"
                    }),

                new VersionEntry(
                    2, 14, 5, "Correções gerais", new DateTime(2018, 2, 20),
                    "Correções gerais",
                    new []
                    {
                        "Problema de lentidão na busca em sites com muitos produtos",
                        "Imagens de produtos encavaldas ao final da busca",
                        "Filtra report de certas exceções por tipo e por user-agent (ex. facebot)",
                        "Inclui dados do cliente logado no admin para suporte Intercom",
                        "Imagem padrão na busca de produtos, até carregamento da imagem final"
                    }),

                new VersionEntry(
                    2, 14, 4, "Correções gerais", new DateTime(2018, 2, 16),
                    "Correções gerais",
                    new []
                    {
                        "Impede a mudança do status do pedido paga Pago e outros status críticos",
                        "Bug do responsivo que impedia editar categorias de portfolio no mobile",
                        "Erro ao abrir detalhes de determinados pedidos em Minhas Vendas (painel vendedor)",
                        "Envio de emails do sistema: garante assunto válido p/ evitar erro",
                        "Pedido de orçamento: sempre pede nome e email do cliente, mas autocompleta do login",
                        "Novos textos para os links de cadastro na tela de login",
                        "Troca cidade \"São Paulo - PR\" para \"São Paulo (Toledo) - PR\""
                    }),

                new VersionEntry(
                    2, 14, 3, "Logs de API", new DateTime(2018, 2, 14),
                    "Guarda logs de API eWebhookgs para fins de auditoria",
                    new []
                    {
                        "Logs de acessos de API",
                        "Logs de chamadas de Webhooks"
                    }),

                new VersionEntry(
                    2, 14, 2, "Hotfix Upload", new DateTime(2018, 1, 25),
                    "Erro ao realizar upload na api",
                    new []
                    {
                        "Movido o filtro de autorização de api 'globalmente' para 'por controller'"
                    }),

                new VersionEntry(
                    2, 14, 1, "Ajustes API", new DateTime(2018, 1, 25),
                    "Ajustes após merge com 2.14.0",
                    new []
                    {
                        "Merge para master",
                        "Correção erro localização resultados Ajax na busca",
                        "Correção erro renderização resultados Ajax na busca",
                        "Reorganização do projeto",
                        "Refactoring inicial de dependências",
                        "Importando classe Guard para rápida validação de parâmetros",
                        "Documento de objetivos de refactoring"
                    }),

                new VersionEntry(
                    2, 14, 0, "API", new DateTime(2018, 1, 24),
                    "Implementação da API V1",
                    new []
                    {
                        "Aterar propriedades básicas de produto",
                        "Listar pedidos",
                        "Detalhar pedido",
                        "Webhooks",
                        "Tokens são gerados no painel administrativo",
                        "Tokens podem ser restritos por vendedor ou gerais"
                    }),

                new VersionEntry(
                    2, 13, 10, "Hotfix", new DateTime(2018, 1, 24),
                    "Correções e melhorias gerais",
                    new []
                    {
                        "Inclusão plugin Intercom nas plataformas demo para testes internos",
                        "Correção erro envio orçamento toda a base",
                        "Correção de bug que não permitia limpar uma busca filtrada por local"
                    }),

                new VersionEntry(
                    2, 13, 9, "Hotfix Vendedor Invisível", new DateTime(2018, 1, 16),
                    "Erro de acesso perfis vendedores",
                    new []
                    {
                        "Perfil de vendedor invisível causava erro quando acessado por link direto",
                        "Nova página criada para indicar que perfil existe mas não está visível"
                    }),

                new VersionEntry(
                    2, 13, 8, "Anexos Respostas Orçamento", new DateTime(2018, 1, 15),
                    "Envio de anexo no pedido de orçamento",
                    new []
                    {
                        "Extende a funcionalidade para dentro das respostas do orçamento",
                        "Corrige bug em que vendedores inativos recebiam orçamentos p/ toda a base"
                    }),

                new VersionEntry(
                    2, 13, 7, "Hotfix Anexos Orçamento", new DateTime(2018, 1, 15),
                    "Envio de anexo no pedido de orçamento",
                    new []
                    {
                        "Bug no gerador de PasswordResetToken do OWIN no Azure",
                        "Inclusão de código RD Station no MS"
                    }),

                new VersionEntry(
                    2, 13, 6, "Anexos no Orçamento", new DateTime(2018, 1, 12),
                    "Envio de anexo no pedido de orçamento",
                    new []
                    {
                        "Adiciona campo para envio de anexo no pedido de orçamento",
                        "Vendedor agora informa orçamento gerou negócio (Ganho ou Perdido)",
                        "Correção link p/ resetar senha no email p/ comprador ao enviar orçamento"
                    }),

                new VersionEntry(
                    2, 13, 5, "Hotfix", new DateTime(2018, 1, 11),
                    "Correções Prioritárias",
                    new []
                    {
                        "Erro ao importar planos de assinatura via admin",
                        "Resultados da busca eram mostrados em mapa mesmo com mapa desligado",
                        "Links para GMaps eram mostrados no perfil mesmo com config desligada",
                        "Erro ao salvar atualização de perfil de vendedor"
                    }),

                new VersionEntry(
                    2, 13, 4, "Hotfix Gestão Planos", new DateTime(2018, 1, 9),
                    "Correções no painel admin, planos de assiantura",
                    new []
                    {
                        "Correção da gestão de status (habilitado/desabilitado)",
                        "Correção do relatório de assinaturas (admin)",
                        "Ajuste na página de compra de assinatura (divisão por zero)"
                    }),

                new VersionEntry(
                    2, 13, 3, "Reanálise Clear Sale", new DateTime(2018, 1, 4),
                    "Envia pedido repetido com status Reanálise",
                    new []
                    {
                        "Permite retentativa de pedido após pré-aprovação do mesmo no painel da Clear Sale"
                    }),

                new VersionEntry(
                    2, 13, 2, "Integração Pedido Venda", new DateTime(2017, 12, 27),
                    "Integração Pedido Venda via plataforma Precode",
                    new []
                    {
                        "Integração temporária do Pedido de Venda com sistema Precode",
                        "Envio de email relatando cada pedido integrado",
                        "Mostra qtde parcelas corretamente no site e admin"
                    }),

                new VersionEntry(
                    2, 13, 1, "Hotfix top sellers", new DateTime(2017, 12, 22),
                    "Seção Melhores Vendedores reaparece",
                    new []
                    {
                        "Seção melhores vendedores da home não aparecia mais"
                    }),

                new VersionEntry(
                    2, 13, 0, "Busca geolocalizada", new DateTime(2017, 12, 22),
                    "Resultados da busca em formato de mapa",
                    new []
                    {
                        "Configuração para habilitar mapa na busca de portfolio",
                        "Busca por locais com sugestões do Google Maps",
                        "Resultados em mapa ou lista",
                        "Ordenação de resultados por distância, relevância e outros",
                        "Preview do perfil no mapa",
                        "Links p/ abrir endereço no Google Maps e GPS",
                        "Início unificação da busca p/ Produto e Vendedor"
                    }),

                new VersionEntry(
                    2, 12, 18, "Hotfix", new DateTime(2017, 12, 18),
                    "Erro ao acessar produto sem estoque",
                    new []
                    {
                        "Passa a mostrar mensagem amigável ao invés de erro"
                    }),

                new VersionEntry(
                    2, 12, 17, "Hotfix", new DateTime(2017, 12, 14),
                    "Fechamento de compra sem dados completos",
                    new []
                    {
                        "Valida cadastro completo Comprador no checkout"
                    }),

                new VersionEntry(
                    2, 12, 16, "Hotfix", new DateTime(2017, 12, 14),
                    "Problemas no cadastro de produto",
                    new []
                    {
                        "Ajuste na validação de configurações",
                        "Passa a mostrar mensagem de erro que não aparecia",
                        "Exigia dimensões do produto mesmo com PAC não habilitado"
                    }),

                new VersionEntry(
                    2, 12, 15, "Ajustes gerais", new DateTime(2017, 12, 13),
                    "Clear Sale, cadastro, admin e orçamento",
                    new []
                    {
                        "Erro ao formatar números >= 1k p/ Clear Sale",
                        "Ajuste na captura de número telefone p/ Clear Sale",
                        "Valida cadastro telefone em formato brasileiro",
                        "Melhoria ao exibir detalhes da compra no admin",
                        "Diferencia cadastro e login com Facebook",
                        "Melhora mensagem erro login Facebook ainda s/ cadastro",
                        "Erro ao alterar status do pedido no painel vendedor",
                        "Aviso p/ vendedor informar categorias portfolio",
                        "Não permite envio orçamento vendedor s/ categoria"
                    }),

                new VersionEntry(
                    2, 12, 14, "Ajustes Clear Sale V4", new DateTime(2017, 12, 8),
                    "Ajustes de status do pedido",
                    new []
                    {
                        "Erro ao tratar certos status de pedido",
                        "Verifica cliente sem endereço no cálculo de frete",
                        "Verifica vendedor sem endereço no cálculo de frete"
                    }),

                new VersionEntry(
                    2, 12, 13, "Correção blog", new DateTime(2017, 12, 8),
                    "App bloqueava acesso dentro de /blog",
                    new []
                    {
                        "Alteração na forma como o site lida com rotas /blog"
                    }),

                new VersionEntry(
                    2, 12, 12, "Correção orçamento geral", new DateTime(2017, 12, 7),
                    "Mensagem categoria sem vendedores",
                    new []
                    {
                        "Erro envio orçamento geral p/ categoria sem vendedores"
                    }),

                new VersionEntry(
                    2, 12, 11, "Hotfix de frete", new DateTime(2017, 12, 7),
                    "Cálculo frete produto",
                    new []
                    {
                        "Erro pontual ao acessar carrinho com certos produtos"
                    }),

                new VersionEntry(
                    2, 12, 10, "Ajustes Clear Sale V3", new DateTime(2017, 12, 6),
                    "Ajustes conforme orientações da Clear Sale",
                    new []
                    {
                        "Erro ao receber webhook da Clear Sale",
                        "Erro na interpretação do status APM (Aprovação Manual)"
                    }),

                new VersionEntry(
                    2, 12, 9, "Ajustes Clear Sale V2", new DateTime(2017, 12, 4),
                    "Ajustes conforme orientações da Clear Sale",
                    new []
                    {
                        "Session ID mais consistente (um por compra)"
                    }),

                new VersionEntry(
                    2, 12, 8, "Hotfix", new DateTime(2017, 12, 1),
                    "Correções pontuais",
                    new []
                    {
                        "Erro ao abrir detalhes da compra após avaliação do vendedor",
                        "Lista de orçamentos no admin não mostra categoria, subcategoria, vendedor",
                        "Erro ao enviar email com assunto &gt; 78 caracteres"
                    }),

                new VersionEntry(
                    2, 12, 7, "Ajustes Clear Sale", new DateTime(2017, 11, 30),
                    "Conforme demanda de correções da Clear Sale",
                    new []
                    {
                        "Correções no XML enviado",
                        "Ajuste tamanho imagens produtos (performance)"
                    }),

                new VersionEntry(
                    2, 12, 6, "Tratamento 404", new DateTime(2017, 11, 30),
                    "Página personalizada de 404",
                    new []
                    {
                        "Impede 404 como exceção",
                        "Impede o envio de emails por causa de bots"
                    }),

                new VersionEntry(
                    2, 12, 5, "Ajustes template email", new DateTime(2017, 11, 29),
                    "Ajustes pontuais",
                    new []
                    {
                        "Não especifica prazo de entrega no email",
                        "Considera o cenário de pagamento offline",
                        "Correção bug configuração ImgHelper"
                    }),

                new VersionEntry(
                    2, 12, 4, "Hotfix", new DateTime(2017, 11, 29),
                    "Correções pontuais",
                    new []
                    {
                        "Correção query p/ carregar Minha Loja"
                    }),

                new VersionEntry(
                    2, 12, 3, "Hotfix", new DateTime(2017, 11, 28),
                    "Correções pontuais",
                    new []
                    {
                        "Não mostrava portfólio dos vendedores",
                        "Configuração de URL de imagens c/ problemas",
                        "SubCategorias não atualizava conforme Categoria",
                        "Msg erro por email passa a chegar formatada",
                        "Link por email p/ abrir orçamento estava quebrado"
                    }),

                new VersionEntry(
                    2, 12, 2, "Integração ClearSale", new DateTime(2017, 11, 28),
                    "Como opção de análise anti-fraude em compras",
                    new []
                    {
                        "Configuração para hibilitar análise via ClearSale",
                        "Implementação de estorno pela IUGU",
                        "Reembolso automático da compra em caso de recusa",
                        "Informa prazo de entrega no carrinho conforme tipo envio",
                        "Histórico do pedido adicionado em Minhas Vendas",
                        "Notificação de erros do site por email ao INA",
                        "Correção erro Google Bot",
                        "Atualização automática do BD p/ a última versão",
                        "Revisão tamanho imagens produtos (todas quadradas)"
                    }),

                new VersionEntry(
                    2, 12, 1, "Hotfix", new DateTime(2017, 11, 21),
                    "Correções pontuais",
                    new []
                    {
                        "Admin não abria detalhes de cadastro vendedor incompleto",
                        "Pedido de orçamento não era enviado, mesmo com OK do sistema"
                    }),

                new VersionEntry(
                    2, 12, 0, "Pagamento IUGU", new DateTime(2017, 11, 15),
                    "Nova opção de gateway de pagamentos",
                    new []
                    {
                        "Configurações da IUGU",
                        "Pagamento em boleto à vista ou cartão parcelado até 12x",
                        "Reconhecimento automático pgto boleto"
                    }),

                new VersionEntry(
                    2, 11, 0, "Pagamento offline", new DateTime(2017, 11, 1),
                    "Permite pagamento de pedido por fora da plataforma (offline)",
                    new []
                    {
                        "Pgto offline como novo método de pgto na configuração",
                        "Correção home mostrava produtos de vendedores sem gateway ID",
                        "Correção no painel admin, não listava produtos dos vendedores",
                        "Melhoria na geolocalização (ex. endereços Douradina - PR)",
                        "Anúncio do produto: verifica status do vendedor",
                        "Config. de cor da barra de busca ecommerce na home",
                        "Bloqueia alteração do CPF/CNPJ após cadastro",
                        "Máscaras corrigidas no mobile (cpf, cnpj, data, cartão crédito)",
                        "Refatoração do código de carrinho e fechamento do pedido"
                    }),

                new VersionEntry(
                    2, 10, 2, "Hotfix", new DateTime(2017, 10, 24),
                    "Problema no carregamento de categorias de produtos da Home do Ecommerce",
                    new []
                    {
                        "Correção do erro"
                    }),

                new VersionEntry(
                    2, 10, 1, "Melhorias feedback clientes", new DateTime(2017, 10, 23),
                    "Melhorias com base em feedbacks de clientes (MegaSoares, Lista1, HelpDriver)",
                    new []
                    {
                        "Melhoria menu principal (links mais relevantes e formatação)",
                        "Configuração para permitir frete gratuito por frota privada",
                        "Configuraçao para valor mínimo de compra por vendedor",
                        "Configuração para desativar seção \"HomeCallSellers\"",
                        "Bug no cadastro de perfil linkedin (no cadastro do vendedor)",
                        "Comprador para carrinho após login, se tiver produtos lá",
                        "Melhorias no sistema de configuração da plataforma",
                        "Otimização do perfil do vendedor, limitando qtde produtos",
                        "Anúncio do produto mostra opções de pgto e parcelamento",
                        "Otimizações de performance, alteração sistema repositórios",
                        "Introdução de sistema de cache orientado a eventos"
                    }),

                new VersionEntry(
                    2, 10, 1, "Melhorias feedback clientes", new DateTime(2017, 10, 23),
                    "Melhorias com base em feedbacks de clientes (MegaSoares, Lista1, HelpDriver)",
                    new []
                    {
                        "Melhoria menu principal (links mais relevantes e formatação)",
                        "Configuração para permitir frete gratuito por frota privada",
                        "Configuraçao para valor mínimo de compra por vendedor",
                        "Configuração para desativar seção \"HomeCallSellers\"",
                        "Bug no cadastro de perfil linkedin (no cadastro do vendedor)",
                        "Comprador para carrinho após login, se tiver produtos lá",
                        "Melhorias no sistema de configuração da plataforma",
                        "Otimização do perfil do vendedor, limitando qtde produtos",
                        "Anúncio do produto mostra opções de pgto e parcelamento",
                        "Otimizações de performance, alteração sistema repositórios",
                        "Introdução de sistema de cache orientado a eventos"
                    }),

                new VersionEntry(
                    2, 10, 0, "Personalização imagens", new DateTime(2017, 10, 3),
                    "Primeiros passos da personalização da plataforma pelo admin",
                    new []
                    {
                        "Envio de imagens pelo admin (logo, benefícios, bg, outras)",
                        "Envio de termos de uso e políticas de privacidade pelo admin",
                        "Correção para não mostrar produtos de vendedores inativos"
                    }),

                new VersionEntry(
                    2, 9, 3, "Melhorias no site", new DateTime(2017, 9, 28),
                    "Melhorias pontuais no marketplace de Produtos",
                    new []
                    {
                        "Id de produto personalizado (id próprio/nosso número)",
                        "Sem limite nas descrições do produto",
                        "Mais espaço no painel do Comprador e Vendedor",
                        "Melhorias de layout no painel do Comprador e Vendedor",
                        "Configuração para desabilitar login por Facebook",
                        "Configuração para desabilitar pgto por Cartão de Crédito"
                    }),

                new VersionEntry(
                    2, 9, 2, "Hotfix", new DateTime(2017, 9, 26),
                    "Correções gerais na plataforma conforme apontamentos de operação do Ebebê",
                    new []
                    {
                        "Ordenação de usuários no painel admin",
                        "Vendedores inativos não aparecem mais na busca",
                        "Configuração para produto estoque 0 sair da plataforma"
                    }),

                new VersionEntry(
                    2, 9, 1, "Responder orçamento geral", new DateTime(2017, 9, 28),
                    "Comprador visualiza e responde orçamento enviado para mais de um vendedor",
                    new []
                    {
                        "Alterações na visualização do Comprador",
                        "Agrupa orçamentos enviados para vários Vendedores"
                    }),

                new VersionEntry(
                    2, 9, 0, "Responder orçamento", new DateTime(2017, 9, 15),
                    "Comprador e Vendedor conversam sobre o orçamento por dentro da plataforma",
                    new []
                    {
                        "Vendedor e Comprador registram resposta via plataforma",
                        "Detalhamento orçamento via admin",
                        "Indicador de Valor do Negócio para o orçamentos",
                        "Notificações para Vendedor e Comprador por email"
                    }),

                new VersionEntry(
                    2, 8, 1, "Hotfix Detalhes Usuários", new DateTime(2017, 9, 20),
                    "Erro ao tentar visualizar Compradores e Vendedores sem endereço registrado no painel admin",
                    new []
                    {
                        "Correção do erro"
                    }),

                new VersionEntry(
                    2, 8, 0, "Melhorias no site e admin", new DateTime(2017, 9, 6),
                    "Detalhes no Admin, ordenação da busca de produtos e outros",
                    new []
                    {
                        "Busca de produtos pode ser ordenada",
                        "Comprador pode ser desativado pelo admin",
                        "Testes do pagamento de pedidos por boleto",
                        "Painel admin permite detalhar Compradores, Vendedores, e Pedidos"
                    }),

                new VersionEntry(
                    2, 7, 1, "Reset dos Migrations", new DateTime(2017, 8, 30),
                    "Reseta o migrations do EF por questões de compatibilidade",
                    new []
                    {
                        "Projetos com migrations reiniciados (todos para \"\"Initial\"\")",
                        "Initial representa o estado atual do banco de dados",
                        "Necessário aplicar projeto a projeto"
                    }),

                new VersionEntry(
                    2, 7, 0, "Home do Ecommerce", new DateTime(2017, 8, 28),
                    "Home em formato ecommerce, com banners e principais produtos de categorias selecionadas",
                    new []
                    {
                        "Personalização via admin: banners",
                        "Personalização via admin: até 3 categorias de produtos",
                        "Config. para ligar/desligar seções da home"
                    }),

                new VersionEntry(
                    2, 6, 0, "Pagamento com Boleto", new DateTime(2017, 8, 15),
                    "Pagamento de produtos pode ser por boleto ou cartão",
                    new []
                    {
                        "Configuração para habilitar boleto",
                        "Opção de pagar com boleto no checkout"
                    }),

                new VersionEntry(
                    2, 5, 1, "Imagens achatadas no responsivo", new DateTime(2017, 8, 9),
                    "Imagens de produtos e portfólio apareciam achatadas/esticadas em telas menores (tablets e telefones)",
                    new []
                    {
                        "Redimensionamento das imagens de acordo com o tamanho da tela por CSS + mediaquery"
                    }),

                new VersionEntry(
                    2, 5, 0, "Pagamentos Parcelados", new DateTime(2017, 8, 4),
                    "Pagamento de produtos pode ser parcelado em até 12x pelo Pagar.me",
                    new []
                    {
                        "Configurações de parcelamento",
                        "Pagamento parcelado de produtos"
                    }),

                new VersionEntry(
                    2, 4, 2, "Hotfix Cadastro Facebook", new DateTime(2017, 8, 7),
                    "Correção bug no cadastro de vendedores com Facebook",
                    new []
                    {
                        "Cadastro de vendedores com Facebook"
                    }),

                new VersionEntry(
                    2, 4, 1, "CDN Imagens", new DateTime(2017, 7, 31),
                    "Imagens enviadas pelos usuários são cachedas em CDN",
                    new []
                    {
                        "Configuração de CDN na parametrização da plataforma",
                        "Imagens recuperadas a partir do CDN"
                    }),

                new VersionEntry(
                    2, 4, 0, "Admin Revamp V1", new DateTime(2017, 7, 28),
                    "Melhoria de layout do painel admin e primeiros indicadores de negócio",
                    new []
                    {
                        "Totalizadores do negócio no painel admin, de acordo com modelo",
                        "Todos: total de usuários (vendedores e compradores)",
                        "Produto: total de pedidos, montante transacionado, faturamento",
                        "Orçamento: total de orçamentos requisitados",
                        "Mensalidade: total de assinaturas ativas, faturamento"
                    }),

                new VersionEntry(
                    2, 3, 0, "Frete Único por Vendedor", new DateTime(2017, 7, 21),
                    "Opção de calcular frete por vendedor, ao invés de por produto",
                    new []
                    {
                        "Configuração de modo de operação de frete p/ a plataforma",
                        "Vendedor configura se aceita retirada no local",
                        "Vendedor configura se realiza entrega com frota própria",
                        "Vendedor determina valores de frete por frota própria",
                        "Melhorias de layout no Produto, Carrinho, Checkout"
                    }),

                new VersionEntry(
                    2, 2, 0, "Variantes de Produto", new DateTime(2017, 6, 30),
                    "Classificação do produto em variantes (ex. Cores:Azul,Amarelo,Verde / Tamanhos:P,M,G)",
                    new []
                    {
                        "Admin configura variantes disponíveis",
                        "Vendedor associa variantes aos seus produtos",
                        "Filtros (facetas) disponíveis na busca de produtos",
                        "Cache da busca na camada de dados (desempenho)"
                    }),

                new VersionEntry(
                    2, 1, 0, "Frete pelos Correios", null,
                    "Cálculo automático de frete pelos correios (PAC e Sedex)",
                    new []
                    {
                        "Integração com API dos Correios"
                    }),

                new VersionEntry(
                    2, 0, 0, "Modelo Produto", null,
                    "Segunda versão do SaaS, operando venda de produtos (ex. Mercado Livre)",
                    new []
                    {
                        "Cadastro de comprador",
                        "Pagamento com cartão de crédito",
                        "Frete"
                    }),

                new VersionEntry(
                    1, 1, 0, "Correções", null,
                    "Ajustes e correções gerais da primeira versão com base em primeiros clientes",
                    new []
                    {
                        "Correções de bugs conforme Basecamp"
                    }),

                new VersionEntry(
                    1, 0, 0, "Modelo Orçamento", null,
                    "Primeira versão do SaaS",
                    new []
                    {
                        "Cadastro de vendedor",
                        "Portfólio de vendedor",
                        "Pedidos de orçamento",
                        "Busca de vendedores",
                        "Painel de parametrização da plataforma"
                    })
            };

            Entries = list.AsReadOnly();
            Current = list.First();
        }
    }

    public class VersionEntry
    {
        public DateTime? Date { get; }

        public string[] Details { get; }

        public int Major { get; }

        public int Minor { get; }

        public int Patch { get; }

        public string Subject { get; }

        public string Title { get; }

        public string VersionNumber => $"{Major}.{Minor}.{Patch}";

        public VersionEntry(int major, int minor, int patch, string title, DateTime? date, string[] details)
            : this(major, minor, patch, title, date, null, details)
        {
        }

        public VersionEntry(int major, int minor, int patch, string title, DateTime? date, string subject, string[] details)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Date = date == null ? (DateTime?)null : new DateTime(date.Value.Year, date.Value.Month, date.Value.Day);
            Subject = subject;
            Details = details ?? throw new ArgumentNullException(nameof(details));
        }
    }
}
