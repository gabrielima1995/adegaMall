using System.ComponentModel.DataAnnotations;

namespace PaymentIntegrator.Core.Common
{
    /// <summary>
    /// Representa uma entidade bancária.
    /// </summary>
    public enum Bank
    {
        [Display(Name = "001 - Banco do Brasil")]
        BancodoBrasil = 1,

        [Display(Name = "002 - Banco Central do Brasil")]
        BancoCentraldoBrasil = 2,

        [Display(Name = "003 - Banco da Amazônia")]
        BancodaAmazonia = 3,

        [Display(Name = "004 - Banco do Nordeste do Brasil")]
        BancodoNordestedoBrasil = 4,

        [Display(Name = "007 - Banco Nacional de Desenvolvimento Econômico e Social")]
        BancoNacionaldeDesenvolvimentoEconomicoeSocial = 7,

        [Display(Name = "020 - Banco do Estado de Alagoas")]
        BancodoEstadodeAlagoas = 20,

        [Display(Name = "021 - Banco do Estado do Espírito Santo")]
        BancodoEstadodoEspiritoSanto = 21,

        [Display(Name = "022 - Banco de Crédito Real de Minas Gerais")]
        BancodeCreditoRealdeMinasGerais = 22,

        [Display(Name = "023 - Banco de Desenvolvimento de Minas Gerais")]
        BancodeDesenvolvimentodeMinasGerais = 23,

        [Display(Name = "024 - Banco do Estado de Pernambuco")]
        BancodoEstadodePernambuco = 24,

        [Display(Name = "025 - Banco Alfa")]
        BancoAlfa = 25,

        [Display(Name = "026 - Banco do Estado do Acre")]
        BancodoEstadodoAcre = 26,

        [Display(Name = "027 - Banco do Estado de Santa Catarina")]
        BancodoEstadodeSantaCatarina = 27,

        [Display(Name = "028 - Banco do Estado da Bahia")]
        BancodoEstadodaBahia = 28,

        [Display(Name = "029 - Banco do Estado do Rio de Janeiro")]
        BancodoEstadodoRiodeJaneiro = 29,

        [Display(Name = "030 - Banco do Estado da Paraíba")]
        BancodoEstadodaParaiba = 30,

        [Display(Name = "031 - Banco do Estado de Goiás")]
        BancodoEstadodeGoias = 31,

        [Display(Name = "032 - Banco do Estado de Mato Grosso")]
        BancodoEstadodeMatoGrosso = 32,

        [Display(Name = "033 - Banco Santander")]
        BancoSantander = 33,

        [Display(Name = "034 - Banco do Estado do Amazonas")]
        BancodoEstadodoAmazonas = 34,

        [Display(Name = "035 - Banco do Estado do Ceará")]
        BancodoEstadodoCeara = 35,

        [Display(Name = "036 - Banco do Estado do Maranhão")]
        BancodoEstadodoMaranhao = 36,

        [Display(Name = "037 - Banco do Estado do Pará")]
        BancodoEstadodoPara = 37,

        [Display(Name = "038 - Banco do Estado do Paraná")]
        BancodoEstadodoParana = 38,

        [Display(Name = "039 - Banco do Estado do Piauí")]
        BancodoEstadodoPiaui = 39,

        [Display(Name = "041 - Barnisul - Banco do Estado do Rio Grande do Sul")]
        BancodoEstadodoRioGrandedoSul = 41,

        [Display(Name = "043 - Banco do Estado de Rio Grande do Norte")]
        BancodoEstadodeRioGrandedoNorte = 43,

        [Display(Name = "045 - Banco Opportunity")]
        BancoOpportunity = 45,

        [Display(Name = "046 - Banco Regional de Desenvolvimento do Extremo Sul")]
        BancoRegionaldeDesenvolvimentodoExtremoSul = 46,

        [Display(Name = "047 - Banco do Estado de Sergipe")]
        BancodoEstadodeSergipe = 47,

        [Display(Name = "048 - Banco do Estado de Minas Gerais")]
        BancodoEstadodeMinasGerais = 48,

        [Display(Name = "059 - Banco do Estado de Rondônia")]
        BancodoEstadodeRondonia = 59,

        [Display(Name = "070 - Banco de Brasília")]
        BancodeBrasilia = 70,

        [Display(Name = "073 - Banco Popular do Brasil")]
        BancoPopulardoBrasil = 73,

        [Display(Name = "075 - Banco ABN Amro S.A.")]
        BancoABNAmroSA = 75,

        [Display(Name = "077 - Banco Inter")]
        BancoInter = 77,

        [Display(Name = "082 - Banco Topázio")]
        BancoTopazio = 82,

        [Display(Name = "085 - Banco Cecred")]
        BancoCecredi = 85,

        [Display(Name = "104 - Caixa Econômica Federal")]
        CaixaEconomicaFederal = 104,

        [Display(Name = "106 - Banco Crefisul")]
        BancoCrefisul = 106,

        [Display(Name = "107 - Banco BBM")]
        BancoBBM = 107,

        [Display(Name = "121 - Agibank")]
        Agibank = 121,

        [Display(Name = "136 - Unicred")]
        Unicred = 136,

        [Display(Name = "150 - Caixa Econômica do Estado de Minas Gerais")]
        CaixaEconomicadoEstadodeMinasGerais = 150,

        [Display(Name = "151 - Nossa Caixa")]
        NossaCaixa = 151,

        [Display(Name = "152 - Caixa Econômica do Estado de Goiás")]
        CaixaEconomicadoEstadodeGoias = 152,

        [Display(Name = "153 - Caixa Econômica Estadual do Rio Grande do Sul")]
        CaixaEconomicaEstadualdoRioGrandedoSul = 153,

        [Display(Name = "154 - Caixa Econômica do Estado de Santa Catarina")]
        CaixaEconomicadoEstadodeSantaCatarina = 154,

        [Display(Name = "206 - Banco Martinelli")]
        BancoMartinelli = 206,

        [Display(Name = "208 - Banco BTG Pactual")]
        BancoBTGPactual = 208,

        [Display(Name = "212 - Banco Original")]
        BancoOriginal = 212,

        [Display(Name = "215 - Banco América do Sul")]
        BancoAmericadoSul = 215,

        [Display(Name = "218 - Banco Bonsucesso")]
        BancoBonsucesso = 218,

        [Display(Name = "219 - Banco Zogbi")]
        BancoZogbi = 219,

        [Display(Name = "220 - Banco Queiroz")]
        BancoQueiroz = 220,

        [Display(Name = "223 - Banco Interunion")]
        BancoInterunion = 223,

        [Display(Name = "224 - Banco Fibra")]
        BancoFibra = 224,

        [Display(Name = "226 - Banco Auxiliar")]
        BancoAuxiliar = 226,

        [Display(Name = "228 - Banco Icatu")]
        BancoIcatu = 228,

        [Display(Name = "229 - Banco Cruzeiro do Sul")]
        BancoCruzeirodoSul = 229,

        [Display(Name = "230 - Banco Bandeirantes")]
        BancoBandeirantes = 230,

        [Display(Name = "231 - Banco Boavista")]
        BancoBoavista = 231,

        [Display(Name = "232 - Banco Interpart")]
        BancoInterpart = 232,

        [Display(Name = "233 - Banco da Capital")]
        BancodaCapital = 233,

        [Display(Name = "234 - Banco Lavra")]
        BancoLavra = 234,

        [Display(Name = "235 - Banco Liberal")]
        BancoLiberal = 235,

        [Display(Name = "237 - Bradesco")]
        Bradesco = 237,

        [Display(Name = "244 - Banco Cidade")]
        BancoCidade = 244,

        [Display(Name = "245 - Banco Empresarial")]
        BancoEmpresarial = 245,

        [Display(Name = "251 - Banco São Jorge")]
        BancoSaoJorge = 251,

        [Display(Name = "255 - Milbanco")]
        Milbanco = 255,

        [Display(Name = "260 - Nu Pagamentos S.A.")]
        NuPagamentosSA = 260,

        [Display(Name = "261 - Banco da América")]
        BancodaAmerica = 261,

        [Display(Name = "263 - Banco Cacique")]
        BancoCacique = 263,

        [Display(Name = "265 - Banco Fator")]
        BancoFator = 265,

        [Display(Name = "275 - Banco Real")]
        BancoReal = 275,

        [Display(Name = "291 - Pagseguro Internet S.A")]
        PagSeguro = 290,

        [Display(Name = "291 - Banco de Crédito Nacional")]
        BancodeCreditoNacional = 291,

        [Display(Name = "304 - Banco Pontual")]
        BancoPontual = 304,

        [Display(Name = "314 - Banco do Comércio e Indústria de São Paulo")]
        BancodoComercioeIndustriadeSaoPaulo = 314,

        [Display(Name = "318 - Banco BMG")]
        BancoBMG = 318,

        [Display(Name = "320 - Banco Industrial e Comercial")]
        BancoIndustrialeComercial = 320,

        [Display(Name = "326 - Banco Royal de Investimento")]
        BancoRoyaldeInvestimento = 326,

        [Display(Name = "334 - Banco Econômico")]
        BancoEconomico = 334,

        [Display(Name = "336 - Banco C6 S.A.")]
        BancoC6SA = 336,

        [Display(Name = "338 - Banco F. Barreto")]
        BancoFBarreto = 338,

        [Display(Name = "341 - Itaú Unibanco")]
        ItauUnibanco = 341,

        [Display(Name = "344 - Banco Mercantil de Pernambuco")]
        BancoMercantildePernambuco = 344,

        [Display(Name = "347 - Banco Sudameris")]
        BancoSudameris = 347,

        [Display(Name = "351 - Banco Bozano, Simonsen")]
        BancoBozanoSimonsen = 351,

        [Display(Name = "356 - ABN AMRO Bank")]
        ABNAMROBank = 356,

        [Display(Name = "388 - Banco BMD")]
        BancoBMD = 388,

        [Display(Name = "389 - Banco Mercantil do Brasil")]
        BancoMercantildoBrasil = 389,

        [Display(Name = "394 - Banco BMC")]
        BancoBMC = 394,

        [Display(Name = "409 - Unibanco")]
        Unibanco = 409,

        [Display(Name = "415 - Banco Nacional")]
        BancoNacional = 415,

        [Display(Name = "420 - Banco Nacional do Norte")]
        BancoNacionaldoNorte = 420,

        [Display(Name = "422 - Banco Safra")]
        BancoSafra = 422,

        [Display(Name = "424 - Banco Noroeste")]
        BancoNoroeste = 424,

        [Display(Name = "434 - Banco Fortaleza")]
        BancoFortaleza = 434,

        [Display(Name = "453 - Banco Rural")]
        BancoRural = 453,

        [Display(Name = "473 - Banco Caixa Geral- Brasil")]
        BancoCaixaGeralBrasil = 473,

        [Display(Name = "491 - Banco Maisonnave")]
        BancoMaisonnave = 491,

        [Display(Name = "504 - Banco Multiplic")]
        BancoMultiplic = 504,

        [Display(Name = "505 - Banco Credit Suisse")]
        BancoCreditSuisse = 505,

        [Display(Name = "604 - Banco Industrial do Brasil")]
        BancoIndustrialdoBrasil = 604,

        [Display(Name = "607 - Banco Santos Neves")]
        BancoSantosNeves = 607,

        [Display(Name = "611 - Banco Paulista")]
        BancoPaulista = 611,

        [Display(Name = "612 - Banco Guanabara")]
        BancoGuanabara = 612,

        [Display(Name = "623 - Banco Pan")]
        BancoPan = 623,

        [Display(Name = "625 - Banco Araucária")]
        BancoAraucaria = 625,

        [Display(Name = "635 - Banco do Estado de Amapá")]
        BancodoEstadodeAmapa = 635,

        [Display(Name = "637 - Banco Sofisa")]
        BancoSofisa = 637,

        [Display(Name = "638 - Banco Prosper")]
        BancoProsper = 638,

        [Display(Name = "643 - Banco Pine")]
        BancoPine = 643,

        [Display(Name = "645 - Banco do Estado de Roraima")]
        BancodoEstadodeRoraima = 645,

        [Display(Name = "647 - Banco Marka")]
        BancoMarka = 647,

        [Display(Name = "653 - Banco Indusval")]
        BancoIndusval = 653,

        [Display(Name = "654 - Banco Renner")]
        BancoRenner = 654,

        [Display(Name = "655 - Banco Votorantim")]
        BancoVotorantim = 655,

        [Display(Name = "702 - Banco Santos")]
        BancoSantos = 702,

        [Display(Name = "710 - Banco Hexabanco")]
        BancoHexabanco = 710,

        [Display(Name = "715 - Banco Vega")]
        BancoVega = 715,

        [Display(Name = "719 - Banco Banif")]
        BancoBanif = 719,

        [Display(Name = "721 - Banco Credibel")]
        BancoCredibel = 721,

        [Display(Name = "722 - Banco Interior de São Paulo")]
        BancoInteriordeSaoPaulo = 722,

        [Display(Name = "735 - Banco Neon")]
        BancoNeon = 735,

        [Display(Name = "738 - Banco Morada")]
        BancoMorada = 738,

        [Display(Name = "739 - Banco BGN")]
        BancoBGN = 739,

        [Display(Name = "741 - Banco Ribeirão Preto")]
        BancoRibeiraoPreto = 741,

        [Display(Name = "745 - Citibank")]
        Citibank = 745,

        [Display(Name = "746 - Banco Modal")]
        BancoModal = 746,

        [Display(Name = "748 - Banco Cooperativo Sicredi - BANSICREDI")]
        BancoCooperativoSicrediBANSICREDI = 748,

        [Display(Name = "756 - Banco Cooperativo do Brasil - BANCOOB")]
        BancoCooperativodoBrasilBANCOOB = 756
    }
}