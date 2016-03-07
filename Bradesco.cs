using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Impactro.Cobranca;
using Impactro.Layout;
using System.IO;

namespace Test
{
    [TestClass]
    public class Bradesco
    {
        const string fileTest = @"..\..\TXT\Remessa_Bradesco.txt"; // para deixar na pasta TXT/ do projeto

        CedenteInfo Cedente;

        [TestInitialize]
        public void Init()
        {
            Cedente = new CedenteInfo();
            Cedente.Banco = "237-2";
            Cedente.Agencia = "1510";
            Cedente.Conta = "001466-4";
            Cedente.Carteira = "09";
            Cedente.CedenteCOD = "00000000000001111111"; // 20 digitos
        }

        [TestMethod, TestCategory("Remessa")]
        public void Remessa_Bradesco()
        {
            LayoutBancos lb = new LayoutBancos();
            lb.Init(Cedente);
            lb.DataHoje = Util.DataTeste;
            Util.AddBoletos(lb);
            string txt = lb.Remessa();
            Console.Write(txt);

            File.WriteAllText(@"..\..\TXT\Teste_Bradesco.txt", txt); // Gera um arquivo para testes de compraração
            // File.WriteAllText(fileTest, txt); // Gera um novo modelo
            string cAnterior = File.ReadAllText(fileTest);

            // Isso necessáriamente não é um erro, pode ter sido uma correção ou melhoria que agora contemple mais casos
            Assert.IsTrue(cAnterior == txt, "O resultado da remessa mudou");
        }

        [TestMethod, TestCategory("Retorno")]
        public void Retorno_Bradesco()
        {
        }
    }
}