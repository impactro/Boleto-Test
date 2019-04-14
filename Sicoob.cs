using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Impactro.Cobranca;
using Impactro.Layout;
using System.IO;

namespace Test
{
    [TestClass]
    public class Sicoob
    {
        const string fileTest = @"..\..\TXT\Remessa_Sicoob.txt"; // para deixar na pasta TXT/ do projeto

        CedenteInfo Cedente;

        [TestInitialize]
        public void Init()
        {
            Cedente = new CedenteInfo();
            Cedente.Cedente = "TESTE QUALQUER LTDA";
            Cedente.CNPJ = "12.345.678/0001-99";
            Cedente.Banco = "756-0";
            Cedente.Agencia = "2222";
            Cedente.Conta = "333333-3";
            Cedente.Convenio = "555555";
            Cedente.Modalidade = "01";
            Cedente.Layout = LayoutTipo.CNAB240;
        }

        [TestMethod, TestCategory("Remessa")]
        public void Remessa_Sicoob()
        {
            LayoutBancos lb = new LayoutBancos();
            lb.Init(Cedente);
            // O CNB240 tem a referencia da data de geração do arquivo, 
            // portato se não for passado a data e hora da gração do anterior nunca irá dar igual
            // Mas atenção é preciso primeiro definir o cedente
            lb.DataHoje = DateTime.Parse("13/12/2015 16:34:08");
            //lb.Lote = 1234;
            //lb.SequencialLote = 5678;
            
            Util.AddBoletos(lb);
            
            // Exibir as informações de DUMP ajuda a char os erros e diferenças
            // lb.ShowDumpLine = true;

            string txt = lb.Remessa();
            Console.Write(txt);

            File.WriteAllText(@"..\..\TXT\Teste_Sicoob.txt", txt); // Gera um arquivo para testes
            //File.WriteAllText(fileTest, txt); // Gera um novo modelo
            string cAnterior = File.ReadAllText(fileTest);

            // Isso necessáriamente não é um erro, pode ter sido uma correção ou melhoria que agora contemple mais casos
            Assert.IsTrue(cAnterior == txt, "O resultado da remessa mudou");
        }

        [TestMethod, TestCategory("CampoLivre")]
        public void CampoLivre_Sicoob()
        {
            Boleto blt = new Boleto();
            string cl;

            // Logica Unica!
            // Mas vale lembrar que um dos requisitos no NossoNumero é iniciar sempre em '1'
            cl = Banco_SICOOB.CampoLivre(blt, "1", "1", "2222", "33", "7654321", "1234567");
            Console.WriteLine(
                "Campo Livre: " + cl + 
                " Agencia/Conta: "+ blt.AgenciaConta + 
                " Nosso Número: " + blt.NossoNumeroExibicao );
            Assert.IsTrue(cl == "1222233765432112345670001", "Erro");
        }
    }
}