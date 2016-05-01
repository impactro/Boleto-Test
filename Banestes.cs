using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Impactro.Cobranca;
using Impactro.Layout;
using System.IO;

namespace Test
{
    [TestClass]
    public class Banestes
    {
        const string fileTest = @"..\..\TXT\Remessa_Banestes.txt"; // para deixar na pasta TXT/ do projeto

        CedenteInfo Cedente;

        [TestInitialize]
        public void Init()
        {
            Cedente = new CedenteInfo();
            Cedente.Banco = "021-3";
            Cedente.CodCedente = "6573315";
            Cedente.Modalidade = "4";
            // Os campos abaixo não são usados no calculo da linha digitável mas são visivel no boleto
            Cedente.Agencia = "123-4";
            Cedente.Conta = "45678-9";
            Cedente.Endereco = "Rua Sei la aonde";
            Cedente.Cedente = "TESTE QUALQUER LTDA";
            Cedente.CNPJ = "12.345.678/0001-12";
        }

        [TestMethod, TestCategory("CampoLivre")]
        public void CampoLivre_Banestes()
        {
            // Leia mais sobre esse teste: https://github.com/impactro/Boleto-Test/wiki/Criando-Layouts/_edit
            Boleto blt = new Boleto();
            string cl = Banco_Banestes.CampoLivre(blt, Cedente.CodCedente, Cedente.Modalidade, "178");
            Console.WriteLine( "Linha Digitável Formatada: " + CobUtil.CampoLivreFormatado(cl, new int[] { 8, 11, 1, 3, 1, 1 })); // maximo 25 digitos
            Assert.IsTrue(cl == "0000017800006573315402141", "Linha invalida");
            // Veja página 31 e 38 da documentação
        }

        [TestMethod, TestCategory("Remessa")]
        public void Remessa_Banestes()
        {
            // (em homologação: 27/04/2016)

            LayoutBancos lb = new LayoutBancos();
            lb.Init(Cedente, LayoutTipo.CNAB400);
            lb.DataHoje = DateTime.Parse("23/04/2016 11:30:00");

            Util.AddBoletos(lb);

            //// Exemplo de definições de dados não calculados no componente, ou não existente inicialmente
            BoletoInfo boleto = lb.Boletos[Util.NossoNumeroInicial.ToString()]; // Captura apenas o primeiro boleto adicionado pela minha rotina padrão de geração
            boleto.SetRegEnumValue(CNAB400Remessa1Banestes.Mensagem, "Mensagem a ser impressa..."); // 352-391

            // Exibir as informações de DUMP ajuda a char os erros e diferenças
            // lb.ShowDumpLine = true;

            string txt = lb.Remessa();
            Console.Write(txt);

            File.WriteAllText(@"..\..\TXT\Teste_Banestes.txt", txt); // Gera um arquivo para testes de compraração
            //File.WriteAllText(fileTest, txt); // Gera um novo modelo
            string cAnterior = File.ReadAllText(fileTest);

            // Isso necessáriamente não é um erro, pode ter sido uma correção ou melhoria que agora contemple mais casos
            Assert.IsTrue(cAnterior == txt);
        }

        [TestMethod, TestCategory("Boleto")]
        public void Boleto_Banestes()
        {
            // Ainda de acordo com o exemplo em: https://github.com/impactro/Boleto-Test/wiki/Criando-Layouts/_edit

            // Dados do Pagador
            SacadoInfo s = new SacadoInfo() { Sacado = "Fabio Ferreira" };

            // Informações do Boleto
            BoletoInfo b = new BoletoInfo()
            {
                DataVencimento = DateTime.Parse("30/07/2000"),
                ValorDocumento = 75,
                NossoNumero = "178"
            };

            // Cria uma instancia do objeto que calcula e monta um boleto
            Boleto bol = new Boleto();

            // Seta as variáveis (parametros) com os dados do recebedor (c), pagador (s), e as informações do boleto (b)
            bol.MakeBoleto(Cedente, s, b);

            // Calcula efetivamente o boleto
            bol.CalculaBoleto();

            // Imprime a linha digitável no console e alguns outros dados para conferencia
            Console.WriteLine("Linha Digitável: " + bol.LinhaDigitavel);
            Console.WriteLine("Agência/Conta: " + bol.AgenciaConta);
            Console.WriteLine("Nosso Número: " + bol.NossoNumeroExibicao);
            Console.WriteLine("Fator Vencimento: " + CobUtil.CalcFatVenc(bol.DataVencimento));

            // De acordo com a página 13 deve gerar exatamente a linha abaixo
            Assert.IsTrue(bol.LinhaDigitavel == "02190.00007 17800.006573 33154.021415 7 10270000007500"); // Página 38 da documentação
                                                                                                           // 02190.00007 17800.006573 33154.021415 3 10270000007500    // O exemplo está com o digito errado!
                                                                                                           // Salva a imagem do boleto para conferencia visual
                                                                                                           //bol.Save("boleto.png");

            // Baseado no segundo exemplo da página 34 (só vou especificar o que é realmente necessário)
            Console.WriteLine();
            bol = new Boleto(); // reseta tudo!
            CedenteInfo c = new CedenteInfo()
            {
                Banco = "021-3",
                CodCedente = "7730070",
                Modalidade = "4"
            };

            // E reaproveiro a instancia (Alterando)
            b.DataVencimento = DateTime.Parse("09/12/2000");
            b.ValorDocumento = 131.50;
            b.NossoNumero = "10297";

            bol.MakeBoleto(c, s, b);

            // Calcula efetivamente o boleto
            bol.CalculaBoleto();

            // Imprime a linha digitável no console e alguns outros dados para conferencia
            Console.WriteLine("Linha Digitável: " + bol.LinhaDigitavel);
            Console.WriteLine("Agência/Conta: " + bol.AgenciaConta);
            Console.WriteLine("Nosso Número: " + bol.NossoNumeroExibicao);
            Console.WriteLine("Fator Vencimento: " + CobUtil.CalcFatVenc(bol.DataVencimento));

            // De acordo com a página 13 deve gerar exatamente a linha abaixo
            Assert.IsTrue(bol.LinhaDigitavel == "02190.00106 29700.007734 00704.021823 3 11590000013150");
            // Note que o digito verificador é 3... acho que houve confusão na digitação da documentação
            // E este número retorna exatamente o informado na página 32: 021.9.3.1159.0000013150-0001029700007730070402182
        }
    }
}