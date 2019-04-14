using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Impactro.Cobranca;
using Impactro.Layout;
using System.IO;

namespace Test
{
    [TestClass]
    public partial class BB
    {
        const string fileTest = @"..\..\TXT\Remessa_BB.txt"; // para deixar na pasta TXT/ do projeto

        CedenteInfo Cedente;

        [TestInitialize]
        public void Init()
        {
            Cedente = new CedenteInfo();
            Cedente.Cedente = "TESTE QUALQUER LTDA";
            Cedente.CNPJ = "88.999.222/0001-33";
            Cedente.Banco = "001";
            Cedente.Agencia = "1234-5";
            Cedente.Conta = "56785678-1";
            Cedente.Carteira = "16";
            Cedente.Modalidade = "18";
            Cedente.Convenio = "123456";
            Cedente.Layout = LayoutTipo.CNAB400;
        }

        
        [TestMethod, TestCategory("Remessa")]
        public void Remessa_BB()
        {
            LayoutBancos lb = new LayoutBancos();
            lb.Init(Cedente);
            lb.DataHoje = DateTime.Parse("19/06/2016 20:31:08");

            Util.AddBoletos(lb);

            // Exibir as informações de DUMP ajuda a char os erros e diferenças
            // lb.ShowDumpLine = true;

            string txt = lb.Remessa();
            Console.Write(txt);

            File.WriteAllText(@"..\..\TXT\Teste_BB.txt", txt); // Gera um arquivo para testes de compraração
            //File.WriteAllText(fileTest, txt); // Gera um novo modelo
            string cAnterior = File.ReadAllText(fileTest);

            // Isso necessáriamente não é um erro, pode ter sido uma correção ou melhoria que agora contemple mais casos
            Assert.IsTrue(cAnterior == txt);
        }
        
        /*
        [TestMethod, TestCategory("Retorno")]
        public void Retorno_BB()
        {
            LayoutBancos r = new LayoutBancos();
            r.Init(Cedente);

            string cFileRET = File.ReadAllText(@"..\..\TXT\Retorno_BB240.txt");
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
                Console.Write("{0} {1:dd/MM/yyyy} {2} {3:C} {4:dd/MM/yyyy} {5:dd/MM/yyyy} {6:dd/MM/yyyy}\r\n",
                    Boleto.NossoNumero,     // 0 reg[CNAB400Retorno1BB.NossoNumero]
                    Boleto.DataDocumento,   // 1 reg[CNAB400Retorno1BB.OcorrenciaData]
                    Boleto.NumeroDocumento, // 2 reg[CNAB400Retorno1BB.NumeroDocumento]
                    Boleto.ValorDocumento,  // 3 reg[CNAB400Retorno1BB.ValorDocumento]
                    Boleto.DataVencimento,  // 4 reg[CNAB400Retorno1BB.Vencimento]
                    Boleto.DataPagamento,   // 5 reg[CNAB400Retorno1BB.DataPagamento]
                    Boleto.ValorPago);      // 6 reg[CNAB400Retorno1BB.ValorPago]
            }

            // por causa do tipo (r.ErroType) pode haver duplicidade de dados
            // pois um boleto pode ter sido baixado e protestado ou pago, 
            // e com alguma ocorrencia e assim cada registro informa algo
            Console.WriteLine("Duplicados:");
            foreach (var Boleto in r.Boletos.Duplicados)
                Console.Write("{0} {1:dd/MM/yyyy} {2:C}\r\n", Boleto.NossoNumero, Boleto.DataPagamento, Boleto.ValorDocumento);
        }
        */

        [TestMethod, TestCategory("CampoLivre")]
        public void CampoLivre_BB()
        {
            Boleto blt = new Boleto();
            string cl;

            // Logica 1
            cl = Banco_do_Brasil.CampoLivre(blt, "", "", "2719715", "19", "18", "27197150000000014");
            Console.WriteLine(
                "Campo Livre para código do cedente de 7 dígitos e nosso número de 17 dígitos: " + cl +
                " Agência/Conta: " + blt.AgenciaConta +
                " Nosso Número: " + blt.NossoNumeroExibicao);

            Assert.IsTrue(cl == "0000002719715000000001418");
            // Nosso Numero      ------12345678901234567--

            // Exemplo documentação pagina 22
            string cDAC = Banco_do_Brasil.NossoNumeroDV("05009401448");
            Console.WriteLine("DAC: " + cDAC);
            Assert.IsTrue(cDAC == "1");
        }
    }
}