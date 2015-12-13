using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Impactro.Cobranca;
using Impactro.Layout;
using System.IO;

namespace Test
{
    [TestClass]
    public class Caixa
    {
        const string fileTest = @"..\..\TXT\Remessa_Caixa.txt"; // para deixar na pasta TXT/ do projeto

        CedenteInfo Cedente;

        [TestInitialize]
        public void Init()
        {
            Cedente = new CedenteInfo();
            Cedente.Banco = "104";
            Cedente.Agencia = "123-4";
            Cedente.Conta = "5678-9";
            Cedente.Carteira = "2";          // Código da Carteira
            Cedente.Convenio = "02";         // CNPJ do PV da conta do cliente
            Cedente.CodCedente = "455932";   // Código do Cliente(cedente)
            Cedente.Modalidade = "14";       // G069 - CC = 14 (título Registrado emissão Cedente)
            Cedente.Endereco = "Rua Sei la aonde";
            Cedente.Informacoes =
                "SAC CAIXA: 0800 726 0101 (informações, reclamações, sugestões e elogios)<br/>" +
                "Para pessoas com deficiência auditiva ou de fala: 0800 726 2492<br/>" +
                "Ouvidoria: 0800 725 7474 (reclamações não solucionadas e denúncias)<br/>" +
                "<a href='http://caixa.gov.br' target='_blank'>caixa.gov.br</a>";
            BoletoTextos.LocalPagamento = "PREFERENCIALMENTE NAS CASAS LOTÉRICAS ATÉ O VALOR LIMITE";
            Cedente.Cedente = "TESTE QUALQUER LTDA";
            Cedente.CNPJ = "88.083.264/0001-05";
        }

        [TestMethod, TestCategory("Remessa")]
        public void Remessa_Caixa()
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
        public void Retorno_Caixa()
        {
        }

        [TestMethod, TestCategory("CampoLivre")]
        public void Campo_Livre()
        {
            Boleto blt = new Boleto();
            string cl;

            // Logica 1
            cl = Banco_Caixa.CampoLivre(blt, "", "123456789012345", "", "", "9876543210");
            Console.WriteLine(
                "Campo Livre para código do cedente de 15 digitos: " + cl + 
                " Agencia/Conta: "+ blt.AgenciaConta + 
                " Nosso Número: " + blt.NossoNumeroExibicao );
            Assert.IsTrue(cl == "9876543210123456789012345", "Erro");

            // Logica 2
            cl = Banco_Caixa.CampoLivre(blt, "5555", "123456", "2", "3", "543210987654321");
            Console.WriteLine(
                "Campo Livre para código de cededente de 6 digitos: " + cl +
                " Agencia/Conta: " + blt.AgenciaConta +
                " Nosso Número: " + blt.NossoNumeroExibicao);
            Assert.IsTrue(cl == "1234560543321049876543219", "Erro");

            // Logica 3
            cl = Banco_Caixa.CampoLivre(blt, "", "12345", "7777", "8", "7654321");
            Console.WriteLine(
                "Campo Livre para carteira 8 ara código de cedente de 5 posições: " + cl +
                " Agencia/Conta: " + blt.AgenciaConta +
                " Nosso Número: " + blt.NossoNumeroExibicao);
            Assert.IsTrue(cl == "1234577778700000007654321", "Erro");

            // Logica 4
            cl = Banco_Caixa.CampoLivre(blt, "", "333333", "", "1", "76543210987654321");
            Console.WriteLine(
                "Campo Livre para caso generico: " + cl +
                " Agencia/Conta: " + blt.AgenciaConta +
                " Nosso Número: " + blt.NossoNumeroExibicao);
            Assert.IsTrue(cl == "3333337543121049876543214", "Erro");
        }
    }
}