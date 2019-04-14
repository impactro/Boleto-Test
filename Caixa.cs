using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Impactro.Cobranca;
using Impactro.Layout;
using System.IO;

namespace Test
{
    [TestClass]
    public partial class Caixa
    {
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
            Cedente.Layout = LayoutTipo.CNAB240;
        }
        
        [TestMethod, TestCategory("Remessa")]
        public void Remessa_Caixa()
        {
            LayoutBancos lb = new LayoutBancos();
            lb.Init(Cedente);
            lb.DataHoje = DateTime.Parse("13/12/2015 16:34:08");

            Util.AddBoletos(lb);

            // Exemplo de definições de dados não calculados no componente, ou não existente inicialmente
            BoletoInfo boleto = lb.Boletos[Util.NossoNumeroInicial.ToString()]; // Captura apenas o primeiro boleto adicionado pela minha rotina padrão de geração
            boleto.SetRegEnumValue(CNAB240SegmentoPCaixa.Juros, 1);                   // 118
            boleto.SetRegEnumValue(CNAB240SegmentoPCaixa.JurosData, Util.DataTeste);  // 119-126
            boleto.SetRegEnumValue(CNAB240SegmentoPCaixa.JurosMora, 0.26);            // 127-141
            boleto.SetRegEnumValue(CNAB240SegmentoPCaixa.ProtestoPrazo, 15);          // 222-223

            // Exibir as informações de DUMP ajuda a char os erros e diferenças
            // lb.ShowDumpLine = true;

            string txt = lb.Remessa();
            Console.Write(txt);

            File.WriteAllText(@"..\..\TXT\Teste_Caixa.txt", txt); // Gera um arquivo para testes de compraração
            //File.WriteAllText(@"..\..\TXT\Remessa_Caixa.txt", txt); // Gera um novo modelo
            string cAnterior = File.ReadAllText(@"..\..\TXT\Remessa_Caixa.txt");

            // Isso necessáriamente não é um erro, pode ter sido uma correção ou melhoria que agora contemple mais casos
            Assert.IsTrue(cAnterior == txt);
        }

        [TestMethod, TestCategory("Retorno")]
        public void Retorno_Caixa()
        {
            LayoutBancos r = new LayoutBancos();
            r.Init(Cedente);

            string cFileRET = File.ReadAllText(@"..\..\TXT\Retorno_Caixa.txt");
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
                Console.Write("{0} {1:dd/MM/yyyy} {2:C} {3:C} {4:C} {5:C} {6:C} {7:C} {8:C} {9:dd/MM/yyyy} {10:dd/MM/yyyy} {11:dd/MM/yyyy}\r\n",
                    Boleto.NossoNumero,         // 0 reg[CNAB240SegmentoTCaixa.NossoNumero]
                    Boleto.DataPagamento,       // 1 reg[CNAB240SegmentoUCaixa.DataOcorrencia];
                    Boleto.ValorDocumento,      // 2 reg[CNAB240SegmentoTCaixa.ValorDocumento],
                    Boleto.ValorAcrescimo,      // 3 reg[CNAB240SegmentoUCaixa.ValorAcrescimos];
                    Boleto.ValorDesconto,       // 4 reg[CNAB240SegmentoUCaixa.ValorDesconto];
                    Boleto.ValorDesconto2,      // 5 reg[CNAB240SegmentoUCaixa.ValorAbatimento];
                    Boleto.ValorIOF,            // 6 reg[CNAB240SegmentoUCaixa.ValorIOF];
                    Boleto.ValorPago,           // 7 reg[CNAB240SegmentoUCaixa.ValorPago];
                    Boleto.ValorLiquido,        // 8 reg[CNAB240SegmentoUCaixa.ValorLiquido];
                    Boleto.DataProcessamento,   // 9 reg[CNAB240SegmentoUCaixa.DataOcorrencia];
                    Boleto.DataCredito,         //10 reg[CNAB240SegmentoUCaixa.DataCredito];
                    Boleto.DataTarifa);         //11 reg[CNAB240SegmentoUCaixa.DataTarifa];
            }

            // por causa do tipo (r.ErroType) pode haver duplicidade de dados
            // pois um boleto pode ter sido baixado e protestado ou pago, 
            // e com alguma ocorrencia e assim cada registro informa algo
            Console.WriteLine("Duplicados:");
            foreach (var Boleto in r.Boletos.Duplicados)
                Console.Write("{0} {1:dd/MM/yyyy} {2:C}\r\n", Boleto.NossoNumero, Boleto.DataPagamento, Boleto.ValorDocumento);
        }

        [TestMethod, TestCategory("CampoLivre")]
        public void CampoLivre_Caixa()
        {
            Boleto blt = new Boleto();
            string cl;

            // Logica 1
            cl = Banco_Caixa.CampoLivre(blt, "", "123456789012345", "", "", "9876543210");
            Console.WriteLine(
                "Campo Livre para código do cedente de 15 digitos: " + cl +
                " Agencia/Conta: " + blt.AgenciaConta +
                " Nosso Número: " + blt.NossoNumeroExibicao);
            Assert.IsTrue(cl == "9876543210123456789012345");

            // Logica 2
            cl = Banco_Caixa.CampoLivre(blt, "5555", "123456", "2", "3", "543210987654321");
            Console.WriteLine(
                "Campo Livre para código de cededente de 6 digitos: " + cl +
                " Agencia/Conta: " + blt.AgenciaConta +
                " Nosso Número: " + blt.NossoNumeroExibicao);
            Assert.IsTrue(cl == "1234560543321049876543219");

            // Logica 3
            cl = Banco_Caixa.CampoLivre(blt, "", "12345", "7777", "8", "7654321");
            Console.WriteLine(
                "Campo Livre para carteira 8 ara código de cedente de 5 posições: " + cl +
                " Agencia/Conta: " + blt.AgenciaConta +
                " Nosso Número: " + blt.NossoNumeroExibicao);
            Assert.IsTrue(cl == "1234577778700000007654321");

            // Logica 4
            cl = Banco_Caixa.CampoLivre(blt, "", "333333", "", "1", "76543210987654321");
            Console.WriteLine(
                "Campo Livre para caso generico: " + cl +
                " Agencia/Conta: " + blt.AgenciaConta +
                " Nosso Número: " + blt.NossoNumeroExibicao);
            Assert.IsTrue(cl == "3333337543121049876543214");

            // Teste Livre
            cl = Banco_Caixa.CampoLivre(blt, "", "123456789012345", "", "", "9000003225"); // o DV do Nosso numero tem que dar Zero!
            Console.WriteLine(
                "Linha Digitável Formatada: " + CobUtil.CampoLivreFormatado(cl, new int[] { 10, 15 }) + // maximo 25 digitos
                " Agencia/Conta: " + blt.AgenciaConta +
                " Nosso Número: " + blt.NossoNumeroExibicao);

            // Linha Digitável Formatada: 9000003225.123456789012345
            // ---------------------------1234567890
            // Agencia / Conta: 1234.567.89012345.2 
            // Nosso Número: 9000003225 - 0

        }

        [TestMethod, TestCategory("Boleto")]
        public void Boleto_Caixa()
        {
            // Exemplo 100% de acordo com a documentação usando os parametros mínimos
            // https://github.com/impactro/Boleto-ASP.NET/files/44866/ESPCODBARR_SICOB.pdf
            // Página 8, item 5.1.1

            /* Dados usados para cálculo:
                 104 Banco ...............................Posição: 01 - 03
                   9 Moeda ...............................Posição: 04 - 04
    1099(10/10/2000) Fator de Vencimento .................Posição: 06 - 09
              160,00 Valor ...............................Posição: 10 - 19
          9001200200 Nosso Número (sem DV) ...............Posição: 20 - 29
     001287000000012 Código do Cedente no SICOB(sem DV) ..Posição: 30 - 44 */

            // Dados do Recebedor
            CedenteInfo c = new CedenteInfo()
            {
                Banco = "104-0",
                CodCedente = "001287000000012"
            };

            // Dados do Pagador
            SacadoInfo s = new SacadoInfo();

            // Informações do Boleto
            BoletoInfo b = new BoletoInfo()
            {
                DataVencimento = DateTime.Parse("10/10/2000"),
                ValorDocumento = 160,
                NossoNumero = "9001200200" // Exemplo do caso especial onde o Dv dá Zero (caso critico)
            };

            // Cria uma instancia do objeto que calcula e monta um boleto
            Boleto bol = new Boleto();

            // Seta as variáveis (parametros) com os dados do recebedor (c), pagador (s), e as informações do boleto (b)
            bol.MakeBoleto(c, s, b);

            // Calcula efetivamente o boleto
            bol.CalculaBoleto();

            // Imprime a linha digitável no console e alguns outros dados para conferencia
            Console.WriteLine("Linha Digitável: " + bol.LinhaDigitavel);
            Console.WriteLine("Agência/Conta: " + bol.AgenciaConta);
            Console.WriteLine("Nosso Número: " + bol.NossoNumeroExibicao);
            Console.WriteLine("Fator Vencimento: " + CobUtil.CalcFatVenc(bol.DataVencimento));

            // De acordo com a página 13 deve gerar exatamente a linha abaixo
            Assert.IsTrue(bol.LinhaDigitavel == "10499.00127 00200.001287 70000.000128 1 10990000016000");

            // Outro exemplo qualquer:
            b.NossoNumero = "9000003225";
            bol.MakeBoleto(c, s, b); // atualizo os dados (não é recomendado, mas para simplificar funciona)
            bol.CalculaBoleto(); // se não chamar esta rotina, virá comos resultados calculados anteriormente
            Console.WriteLine("Exemplo Livre: Nosso Número: " + bol.NossoNumeroExibicao);
            Assert.IsTrue(bol.NossoNumeroExibicao == "9000003225-0"); // no caso o sistema sempre preenche com os digitos zeros a esquerda

            // Salva a imagem do boleto para conferencia visual
            bol.Save("boleto.png");

        }
    }
}