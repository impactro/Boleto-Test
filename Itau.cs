using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Impactro.Cobranca;
using Impactro.Layout;
using System.IO;

namespace Test
{
    [TestClass]
    public class Itau
    {
        const string fileTest = @"..\..\TXT\Remessa_Itau.txt"; // para deixar na pasta TXT/ do projeto

        CedenteInfo Cedente;

        [TestInitialize]
        public void Init()
        {
            Cedente = new CedenteInfo();
            Cedente.Banco = "341";
            Cedente.Agencia = "1234";
            Cedente.Conta = "56789-0";
            Cedente.Carteira = "123";          // Código da Carteira
            Cedente.Cedente = "TESTE QUALQUER LTDA";
            Cedente.CNPJ = "88.999.222/0001-33";
            Cedente.Layout = LayoutTipo.CNAB400;
        }

        [TestMethod, TestCategory("Retorno")]
        public void Retorno_Itau()
        {
            LayoutBancos r = new LayoutBancos();
            r.Init(Cedente);

            string cFileRET = File.ReadAllText(@"..\..\TXT\Retorno_Itau.txt");
            r.ErroType = BoletoDuplicado.Lista;
            Layout ret = r.Retorno(cFileRET);

            // O resultado pode vir completo em uma tabela
            // var tb = ret.Table(typeof(CNAB400Retorno1Bradesco)); 
            //string cErros = r.ErroLinhas;
            //Assert.IsTrue(string.IsNullOrEmpty(cErros), cErros);

            // Ou usa-se o array de boletos
            foreach (string nn in r.Boletos.NossoNumeros)
            {
                BoletoInfo Boleto = r.Boletos[nn];
                Console.Write("{0} {1:dd/MM/yyyy} {2} {3:C} {4:dd/MM/yyyy} {5:dd/MM/yyyy} {6:C} {7}\r\n",
                    Boleto.NossoNumero,     // 0 reg[CNAB400Retorno1Itau.NossoNumero],
                    Boleto.DataDocumento,   // 1 reg[CNAB400Retorno1Itau.OcorrenciaData],
                    Boleto.NumeroDocumento, // 2 reg[CNAB400Retorno1Itau.NumeroDocumento],
                    Boleto.ValorDocumento,  // 3 reg[CNAB400Retorno1Itau.ValorDocumento],
                    Boleto.DataVencimento,  // 4 reg[CNAB400Retorno1Itau.Vencimento],
                    Boleto.DataPagamento,   // 5 reg[CNAB400Retorno1Itau.DataPagamento],
                    Boleto.ValorPago,       // 6 reg[CNAB400Retorno1Itau.ValorPago]
                    Boleto.OcorrenciaRetorno); // 7 Ocorencia de retorno
            }

            // por causa do tipo (r.ErroType) pode haver duplicidade de dados
            // pois um boleto pode ter sido baixado e protestado ou pago, 
            // e com alguma ocorrencia e assim cada registro informa algo
            Console.WriteLine("Duplicados:");
            foreach (var Boleto in r.Boletos.Duplicados)
                Console.Write("{0} {1:dd/MM/yyyy} {2:C}\r\n", Boleto.NossoNumero, Boleto.DataPagamento, Boleto.ValorDocumento);
        }

        [TestMethod, TestCategory("Remessa")]
        public void Remessa_Itau()
        {
            LayoutBancos lb = new LayoutBancos();
            lb.Init(Cedente);
            lb.DataHoje = DateTime.Parse("13/12/2015 16:34:08");

            Util.AddBoletos(lb);

            // Exibir as informações de DUMP ajuda a char os erros e diferenças
            // lb.ShowDumpLine = true;

            string txt = lb.Remessa();
            Console.Write(txt);

            File.WriteAllText(@"..\..\TXT\Teste_Itau.txt", txt); // Gera um arquivo para testes de compraração
            //File.WriteAllText(fileTest, txt); // Gera um novo modelo
            string cAnterior = File.ReadAllText(fileTest);

            // Isso necessáriamente não é um erro, pode ter sido uma correção ou melhoria que agora contemple mais casos
            Assert.IsTrue(cAnterior == txt);
        }
    }
}