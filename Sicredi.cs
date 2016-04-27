using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Impactro.Cobranca;
using Impactro.Layout;
using System.IO;

namespace Test
{
    [TestClass]
    public class Sicredi
    {
        const string fileTest = @"..\..\TXT\Remessa_Sicredi.txt"; // para deixar na pasta TXT/ do projeto

        CedenteInfo Cedente;

        [TestInitialize]
        public void Init()
        {
            Cedente = new CedenteInfo();
            Cedente.Cedente = "TESTE QUALQUER LTDA";
            Cedente.CNPJ = "12.345.678/0001-99";
            Cedente.Cedente = "Exemplo de empresa cedente";
            Cedente.Banco = "748-2";
            Cedente.Agencia = "4444";
            Cedente.Conta = Cedente.CodCedente = "55555";
            Cedente.Modalidade = "04"; // posto
        }

        [TestMethod, TestCategory("Remessa")]
        public void Remessa_Sicredi()
        {
            LayoutBancos lb = new LayoutBancos();
            lb.Init(Cedente, LayoutTipo.CNAB400);
            // Somente apos inicializar, pode-se definir alguns valores!
            lb.DataHoje = Util.DataTeste; // Data a ser usada no header da remessa, para dar sempre o mesmo resultado nos testes
            lb.Lote = 1234; // apenas para sempre gerar o mesmo numero de lote a cada teste

            // E em seguida adiciona os boletos
            Util.AddBoletos(lb);
            
            // Exibir as informações de DUMP ajuda a char os erros e diferenças
            // lb.ShowDumpLine = true;

            string txt = lb.Remessa();
            Console.Write(txt);

            File.WriteAllText(@"..\..\TXT\Teste_Sicredi.txt", txt); // Gera um arquivo para testes
            // File.WriteAllText(fileTest, txt); // Gera um novo modelo
            string cAnterior = File.ReadAllText(fileTest);

            // Isso necessáriamente não é um erro, pode ter sido uma correção ou melhoria que agora contemple mais casos
            Assert.IsTrue(cAnterior == txt, "O resultado da remessa mudou");
        }

        [TestMethod, TestCategory("Retorno")]
        public void Retorno_Sicredi()
        {
        }

        [TestMethod, TestCategory("CampoLivre")]
        public void CampoLivre_Sicredi()
        {
            Boleto blt = new Boleto();
            string cl;

            // Logica Unica!
            // Mas vale lembrar que um dos requisitos no NossoNumero é iniciar sempre em '1'
            cl = Banco_Sicredi.CampoLivre(blt, "1111", "22", "33333", "44444", "3");
            Console.WriteLine(
                "Campo Livre: " + cl + 
                " Nosso Número: " + blt.NossoNumeroExibicao );
            Assert.IsTrue(cl == "3101244444011112233333001", "Erro");
        }
    }
}