using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Impactro.Cobranca;
using Impactro.Layout;
using System.IO;

namespace Test
{
    [TestClass]
    public class BRB
    {
        const string fileTest = @"..\..\TXT\Remessa_BRB.txt"; // para deixar na pasta TXT/ do projeto

        CedenteInfo Cedente;

        [TestInitialize]
        public void Init()
        {
            Cedente = new CedenteInfo();
            Cedente.Cedente = "TESTE QUALQUER LTDA";
            Cedente.CNPJ = "12.345.678/0001-99";
            Cedente.Banco = "070-1";
            Cedente.Agencia = "222";
            Cedente.Conta = "3333333-7";
        }

        [TestMethod, TestCategory("Remessa")]
        public void Remessa_BRB()
        {
            LayoutBancos lb = new LayoutBancos();
            lb.Init(Cedente, LayoutTipo.CNAB400);
            // O CNB240 tem a referencia da data de geração do arquivo, 
            // portato se não for passado a data e hora da gração do anterior nunca irá dar igual
            // Mas atenção é preciso primeiro definir o cedente
            lb.DataHoje = DateTime.Parse("06/06/2016 23:34:08");
            
            Util.AddBoletos(lb);
            
            // Exibir as informações de DUMP ajuda a char os erros e diferenças
            // lb.ShowDumpLine = true;

            string txt = lb.Remessa();
            Console.Write(txt);

            File.WriteAllText(@"..\..\TXT\Teste_BRB.txt", txt); // Gera um arquivo para testes
            //File.WriteAllText(fileTest, txt); // Gera um novo modelo
            string cAnterior = File.ReadAllText(fileTest);

            // Isso necessáriamente não é um erro, pode ter sido uma correção ou melhoria que agora contemple mais casos
            Assert.IsTrue(cAnterior == txt, "O resultado da remessa mudou");
        }

        [TestMethod, TestCategory("CampoLivre")]
        public void CampoLivre_BRB()
        {
            Boleto blt = new Boleto();
            string cl;

            // Logica Unica!
            // Mas vale lembrar que um dos requisitos no NossoNumero é iniciar sempre em '1'
            cl = Banco_BRB.CampoLivre(blt, "058", "6002006", "1", "1");
            Console.WriteLine(
                "Campo Livre: " + cl + 
                " Nosso Número: " + blt.NossoNumeroExibicao );
            Assert.IsTrue(cl == "0000586002006100000107045", "Erro");
        }
    }
}