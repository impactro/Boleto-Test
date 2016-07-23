using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Impactro.Cobranca;
using Impactro.Layout;
using System.IO;

namespace Test
{
    [TestClass]
    public class BB
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
            // File.WriteAllText(fileTest, txt); // Gera um novo modelo
            string cAnterior = File.ReadAllText(fileTest);

            // Isso necessáriamente não é um erro, pode ter sido uma correção ou melhoria que agora contemple mais casos
            Assert.IsTrue(cAnterior == txt);
        }

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