using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Impactro.Cobranca;
using Impactro.Layout;
using System.IO;

namespace Test
{
    public partial class BB
    {
        [TestMethod, TestCategory("Retorno")]
        public void Retorno_BB7()
        {
            string cFileRET = File.ReadAllText(@"..\..\TXT\Retorno_BB7.txt");

            // A classe Layout interpreta qualquer layout definido por um enumerador estruturado
            Layout r = new Layout(typeof(CNAB400Retorno7BB));
            r.Conteudo = cFileRET;

            // Uso a mesma lógica da rotina de retorno padrão para armazenar tudo em um array de boletos
            var Boletos = new BoletoItens();

            // Para cada registro do tipo especificado executa-se a ação
            r.ForEach<CNAB400Retorno7BB>(reg =>
            {
                // É possivel ler qualquer campo!
                Console.Write("{0}-{1} {2}-{3}\r\n", 
                    reg[CNAB400Retorno7BB.Agencia], reg[CNAB400Retorno7BB.AgenciaDV], 
                    reg[CNAB400Retorno7BB.Conta], reg[CNAB400Retorno7BB.ContaDV]);

                // E o que for relevante coloca-se no array de boletos
                //Boletos.Add(new BoletoInfo()
                //{
                //    ValorPago = 0 // (double)reg[CNAB400Retorno1BB.ValorPago]
                //}, reg.OriginalLine);
            });

            // Uso o mesmo tipo de loop 
            //foreach (string nn in Boletos.NossoNumeros)
            //{
            //    BoletoInfo Boleto = Boletos[nn];
            //    Console.Write("{0} {1:dd/MM/yyyy} {2} {3:C} {4:dd/MM/yyyy} {5:dd/MM/yyyy} {6:dd/MM/yyyy}\r\n",
            //        Boleto.NossoNumero,     // 0 reg[CNAB400Retorno1BB.NossoNumero]
            //        Boleto.DataDocumento,   // 1 reg[CNAB400Retorno1BB.OcorrenciaData]
            //        Boleto.NumeroDocumento, // 2 reg[CNAB400Retorno1BB.NumeroDocumento]
            //        Boleto.ValorDocumento,  // 3 reg[CNAB400Retorno1BB.ValorDocumento]
            //        Boleto.DataVencimento,  // 4 reg[CNAB400Retorno1BB.Vencimento]
            //        Boleto.DataPagamento,   // 5 reg[CNAB400Retorno1BB.DataPagamento]
            //        Boleto.ValorPago);      // 6 reg[CNAB400Retorno1BB.ValorPago]
            //}

        }
    }

    /// <summary>
    /// Estrutura de Retorno BB: Layout de Arquivo Retorno para convênios na faixa numérica entre 1.000.000 a 9.999.999 (Convênios de 7 posições)    /// banco_do_brasil_cnab400-retorno.pdf - Página 4
    /// </summary>
    [RegLayout(@"^7", DateFormat6 = "ddMMyy")]
    public enum CNAB400Retorno7BB
    {
        /// <summary>
        /// Tipo de Registro
        /// </summary>
        [RegFormat(RegType.P9, 1)] // 1
        Controle_Registro,

        [RegFormat(RegType.P9, 2)] // 2
        Zeros1,

        [RegFormat(RegType.P9, 14)] // 4
        Zeros2,

        /// <summary>
        /// Prefixo da Agência
        /// </summary>
        [RegFormat(RegType.P9, 4)] // 18
        Agencia,

        [RegFormat(RegType.PX, 1)] // 22
        AgenciaDV,

        /// <summary>
        /// Número da Conta Corrente do Cedente
        /// </summary>
        [RegFormat(RegType.P9, 8)] // 23
        Conta,

        [RegFormat(RegType.PX, 1)] // 31
        ContaDV,

        /// <summary>
        /// Número do Convênio de Cobrança do Cedente
        /// </summary>
        [RegFormat(RegType.P9, 7)] // 32
        Convenio
    }
}