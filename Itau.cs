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
        }

        [TestMethod, TestCategory("Remessa")]
        public void Remessa_Itau()
        {
            LayoutBancos lb = new LayoutBancos();
            lb.Init(Cedente);
            // O CNB240 tem a referencia da data de geração do arquivo, 
            // portato se não for passado a data e hora da gração do anterior nunca irá dar igual
            // Mas atenção é preciso primeiro definir o cedente
            lb.DataHoje = DateTime.Parse("13/12/2015 16:34:08");

            Util.AddBoletos(lb);

            // Exibir as informações de DUMP ajuda a char os erros e diferenças
            // lb.ShowDumpLine = true;

            string txt = lb.Remessa();
            Console.Write(txt);

            File.WriteAllText(@"..\..\TXT\Teste_Itau.txt", txt); // Gera um arquivo para testes de compraração
            // File.WriteAllText(fileTest, txt); // Gera um novo modelo
            string cAnterior = File.ReadAllText(fileTest);

            // Isso necessáriamente não é um erro, pode ter sido uma correção ou melhoria que agora contemple mais casos
            Assert.IsTrue(cAnterior == txt);
        }

        [TestMethod, TestCategory("Retorno")]
        public void Retorno_Caixa()
        {
        }
    }
}