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
            //Cedente.Cedente = "TESTE QUALQUER LTDA";
            //Cedente.CNPJ = "88.083.264/0001-05";
            Cedente.Cedente = "Via Corte Indústria e Comercio de Oxicorte e Aços LTDA CNPJ:08.539.790/0001-90 ";
            Cedente.CNPJ = "8539790000190";
            Cedente.Banco = "756-0";
            Cedente.Agencia = "3010";
            Cedente.Conta = "109157-3";
            Cedente.Convenio = "982296";
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
            
            Util.AddBoletos(lb);
            
            // Exibir as informações de DUMP ajuda a char os erros e diferenças
            // lb.ShowDumpLine = true;

            string txt = lb.Remessa();
            Console.Write(txt);
            
            File.WriteAllText(@"..\..\TXT\Teste.txt", txt); // Gera um arquivo para testes
            // File.WriteAllText(fileTest, txt); // Gera um novo modelo
            string cAnterior = File.ReadAllText(fileTest);

            // Isso necessáriamente não é um erro, pode ter sido uma correção ou melhoria que agora contemple mais casos
            Assert.IsTrue(cAnterior == txt, "O resultado da remessa mudou");
        }

        [TestMethod, TestCategory("Retorno")]
        public void Retorno_Sicoob()
        {
        }

        [TestMethod, TestCategory("CampoLivre")]
        public void CampoLivre_Sicoob()
        {
            Boleto blt = new Boleto();
            string cl;

            // Logica Unica
            cl = Banco_SICOOB.CampoLivre(blt, "1", "2222", "33", "7654321", "1234567");
            Console.WriteLine(
                "Campo Livre: " + cl + 
                " Agencia/Conta: "+ blt.AgenciaConta + 
                " Nosso Número: " + blt.NossoNumeroExibicao );
            Assert.IsTrue(cl == "1222233765432112345670000", "Erro");
        }
    }
}