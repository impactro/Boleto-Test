using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Impactro.Cobranca;
using Impactro.Layout;

namespace Test
{
    public partial class BB
    {
        // Este exemplo funciona de forma parecida com o BB_Retorno7.cs ou Caixa_SegmentoR.cs
        [TestMethod, TestCategory("Remessa Customizada")]
        public void Remessa_BB5()
        {
            LayoutBancos lb = new LayoutBancos();
            lb.Init(Cedente); 
            lb.cnab.AddOpcionalType(typeof(CNAB400Remessa5BB)); // Tipagem do registro opcional definido neste exemplo (externo aos fontes)
            lb.onRegOpcional = BB_Registro5_Opcional;           // o evento para processar o registro opcional

            Util.AddBoletos(lb);

            string txt = lb.Remessa();
            Console.Write(txt);

        }

        private int BB_Registro5_Opcional(CNAB cnab, BoletoInfo boleto)
        {
            // Apenas mostrando de forma simples, que nem todos os boletos precisariam ter o registro opcional
            if (boleto.BoletoID % 2 == 0) // no caso somente os boletos de numero impar teriam o registro
                return 0; // informa que não foi inserido nenhum registro opcional, assim a numero de sequencia do registro permanece o mesmo

            // Define as informações do registro opcional 5 para registro de valores de multa
            var regR = new Reg<CNAB400Remessa5BB>();

            // Os valores default nem precisa definir
            regR[CNAB400Remessa5BB.CodigoMulta] = 1; // Coloque o que quiser(logico de acordo com a documentação)
            regR[CNAB400Remessa5BB.DataMulta] = boleto.DataVencimento.AddDays(15);  // 15 dias após o vencimento
            regR[CNAB400Remessa5BB.ValorMulta] = boleto.ValorDocumento * 0.02;      // 2% do valor do documento
            regR[CNAB400Remessa5BB.Sequencial] = cnab.SequencialRegistro;           // Já como esse evento é chamado por ultimo, ele está no valor correto, mas se precisar adicionar mais que 1 registro ai tem que adicionar +N, e retornar sempre o numero de registros incluidos

            cnab.Add(regR);

            return 1; // Nomero de registro opcionais incluidos
        }
    }

    // http://prnt.sc/dzd2mc
    // Os registros opcionais dos bancos eu não implemento no componente padrão pois na maioria dos casos é expecífico dificultando a homologação inicial
    // Mas se precisar crie estruturas auxiliares como esta abaixo, se use conforme o exemplo
    [RegLayout(@"^10400013\d{5}R", DateFormat8 = "ddMMyyyy")]
    public enum CNAB400Remessa5BB
    {
        [RegFormat(RegType.P9, 1, Default = "5")] // 1-1
        Identificacao,

        [RegFormat(RegType.PX, 2, Default = "99")] // 2-3
        Servico,

        [RegFormat(RegType.P9, 1)] // 4-4
        CodigoMulta,

        [RegFormat(RegType.PD, 6)] // 5-10
        DataMulta,

        [RegFormat(RegType.PV, 12)] // 11-22
        ValorMulta,

        [RegFormat(RegType.PX, 372)] // 23-394
        Brancos,

        [RegFormat(RegType.P9, 6)] // 395-400
        Sequencial
    }
}