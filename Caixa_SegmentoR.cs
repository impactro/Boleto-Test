using Impactro.Cobranca;
using Impactro.Layout;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Test
{
    public partial class Caixa
    {
        [TestMethod, TestCategory("Remessa Customizada")]
        public void Remessa_CaixaR()
        {
            LayoutBancos lb = new LayoutBancos();
            lb.Init(Cedente); // A inicialização define a instancia 'cnab' apropriada de acordo com o cedente
            lb.cnab.AddOpcionalType(typeof(CNAB240SegmentoRCaixa));
            lb.onRegOpcional = Caixa_SegmentoR_Opcional; // o evento pertence a instancia do 'cnab', é é apenas redirecionado internamente
                                                         // lb.cnab.onRegOpcional += Caixa_SegmentoR_Opcional;

            Util.AddBoletos(lb);

            string txt = lb.Remessa();
            Console.Write(txt);

        }

        private int Caixa_SegmentoR_Opcional(CNAB cnab, BoletoInfo boleto)
        {
            // Apenas mostrando de forma simples, que nem todos os boletos precisariam ter o registro opcional
            if (boleto.BoletoID % 2 == 0) // no caso somente os boletos de numero impar teriam o registro
                return 0; // informa que não foi inserido nenhum registro opcional, assim a numero de sequencia do registro permanece o mesmo

            // Define as informações do segmento opcional R (da mesma forma pode ser criado qualquer tipo de registro opcional)
            var regR = new Reg<CNAB240SegmentoRCaixa>();

            regR[CNAB240SegmentoRCaixa.Lote] = cnab.SequencialLote; // o número da sequencia só é alterado quando sair deste evento
            regR[CNAB240SegmentoRCaixa.Nregistro] = cnab.SequencialRegistro;
            regR[CNAB240SegmentoRCaixa.email] = "teste@email.com";
            regR[CNAB240SegmentoRCaixa.Informacao] = "info..." ;
            cnab.Add(regR);

            // regX[CNAB240SegmentoXCaixa.Lote] = cnab.SequencialLote + 1; // Se houver mais registros o numero sequencial deve ser incrementadp
            // cnab.Add(regX); // E neste caso o retorno teria que ser 2 (numero de itens adicionados)

            return 1; // Nomero de registro opcionais incluidos
        }
    }

    // https://github.com/impactro/Boleto-Test/blob/master/DOC/caixa_cnab240.pdf
    // Os registros opcionais dos bancos eu não implemento no componente padrão pois na maioria dos casos o uso é mais expecífico dificultando a homologação inicial
    // Mas se precisar crie estruturas auxiliares como esta abaixo, se use conforme o exemplo
    [RegLayout(@"^10400013\d{5}R", DateFormat8 = "ddMMyyyy")]
    public enum CNAB240SegmentoRCaixa
    {
        #region "Controle"

        /// <summary>
        /// Código do Banco na Compensação
        /// </summary>
        [RegFormat(RegType.P9, 3, Default = "104")] // 1-3
        Banco,

        /// <summary>
        /// Lote de Serviço
        /// </summary>
        [RegFormat(RegType.P9, 4, Default = "1")] // 4-7
        Lote,

        /// <summary>
        /// Tipo de Registro
        /// </summary>
        [RegFormat(RegType.P9, 1, Default = "3")] // 8-8
        Registro,

        #endregion

        #region "Serviço"

        /// <summary>
        /// Nº Sequencial do Registro no Lote
        /// </summary>
        [RegFormat(RegType.P9, 5)] // 9-13
        Nregistro,

        /// <summary>
        /// Cód. Segmento do Registro Detalhe
        /// </summary>
        [RegFormat(RegType.PX, 1, Default = "R")] // 14-14
        Segmento,

        /// <summary>
        /// Uso Exclusivo FEBRABAN/CNAB
        /// </summary>
        [RegFormat(RegType.PX, 1)] // 15-15
        CNAB,

        /// <summary>
        /// Código de Movimento Remessa
        /// </summary>
        [RegFormat(RegType.P9, 2, Default = "1")] // 16-17
        CodMov,

        #endregion

        /// <summary>
        /// Código do Desconto 2 *C021 
        /// </summary>
        [RegFormat(RegType.P9, 1)] // 18
        DescontoCodigo2,

        /// <summary>
        /// Data do Desconto 2 *C022 
        /// </summary>
        [RegFormat(RegType.PD, 8)] // 19-26
        DescontoData2,

        /// <summary>
        /// Valor/Percentual a ser Concedido *C023 
        /// </summary>
        [RegFormat(RegType.PV, 15)] // 27-41
        DescontoValor2,

        /// <summary>
        /// Código do Desconto 3 *C021 
        /// </summary>
        [RegFormat(RegType.P9, 1)] // 42
        DescontoCodigo3,

        /// <summary>
        /// Data do Desconto 3 *C022 
        /// </summary>
        [RegFormat(RegType.PD, 8)] // 43-50
        DescontoData3,

        /// <summary>
        /// Valor/Percentual a ser Concedido *C023 
        /// </summary>
        [RegFormat(RegType.PV, 15)] // 51-65
        DescontoValor3,

        /// <summary>
        /// Código da multa *C073 
        /// </summary>
        [RegFormat(RegType.P9, 1)] // 66
        MultaCodigo,

        /// <summary>
        /// Data da multa *C074 
        /// </summary>
        [RegFormat(RegType.PD, 8)] // 67-74
        MultaData,

        /// <summary>
        /// Valor/Percentual a ser aplicado C075 
        /// </summary>
        [RegFormat(RegType.PV, 15)] // 75-89
        MultaValor,

        /// <summary>
        /// Informação ao sacado G036
        /// </summary>
        [RegFormat(RegType.PX, 10)] // 90-99
        Informacao,

        /// <summary>
        /// Informação ao sacado G037
        /// </summary>
        [RegFormat(RegType.PX, 40)] // 100-139
        Mensagem3,

        /// <summary>
        /// Informação ao sacado G037
        /// </summary>
        [RegFormat(RegType.PX, 40)] // 140-179
        Mensagem4,

        /// <summary>
        /// email do sacado para envio de informações G032
        /// </summary>
        [RegFormat(RegType.PX, 50)] // 180-229
        email,

        [RegFormat(RegType.PX, 11)] // 2330-240
        branco
    }
}