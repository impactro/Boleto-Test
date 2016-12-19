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
            Cedente.Layout = LayoutTipo.CNAB400;
        }

        [TestMethod, TestCategory("Remessa")]
        public void Remessa_Sicredi()
        {
            LayoutBancos lb = new LayoutBancos();
            lb.Init(Cedente);
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
            // File.WriteAllText(@"..\..\TXT\Remessa_Sicredi.txt", txt); // Gera um novo modelo
            string cAnterior = File.ReadAllText(@"..\..\TXT\Remessa_Sicredi.txt");

            // Isso necessáriamente não é um erro, pode ter sido uma correção ou melhoria que agora contemple mais casos
            Assert.IsTrue(cAnterior == txt, "O resultado da remessa mudou");
        }

        [TestMethod, TestCategory("Retorno")]
        public void Retorno_Sicredi()
        {
            LayoutBancos r = new LayoutBancos(); 
            r.Init(Cedente);

            string cFileRET = File.ReadAllText(@"..\..\TXT\Retorno_Sicredi.txt");
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
                Console.Write("{0} {1:dd/MM/yyyy} {2:C}\r\n", Boleto.NossoNumero, Boleto.DataPagamento, Boleto.ValorDocumento);
            }

            // por causa do tipo (r.ErroType) pode haver duplicidade de dados
            // pois um boleto pode ter sido baixado e protestado ou pago, 
            // e com alguma ocorrencia e assim cada registro informa algo
            Console.WriteLine("Duplicados:");
            foreach (var Boleto in r.Boletos.Duplicados)
                Console.Write("{0} {1:dd/MM/yyyy} {2:C}\r\n", Boleto.NossoNumero, Boleto.DataPagamento, Boleto.ValorDocumento);
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

            /* Do exemplo da documentação página 16 (https://github.com/impactro/Boleto-Test/blob/master/DOC/sicredi_cnab400.pdf)
            Linha digitável: 74891.11422 00001.03544 02000.921078 9 618700000010000 (usando http://exemplos.boletoasp.com.br/BoletoNet/FuncTeste_DecodIPTE.aspx para obter o código de barras)
            Código de barras: 748.9.6.1870.0000010000-1114200001035442000921078 
            Temos o Campo Livre: 1 1 14 2 00001 0 3544 20 00921 0 7 8
                           onde: R C yy b nnnnn d AAAA PP CCCCC V 0 D
                                                                  X => Note que este campo deveria ser ZERO e é 7!
                                                                       Isso é estrano, pois não está de acordo com a documentação, assim no exemplo estou assumindo como 0
                                                                       Logico que o deigito verificar seguinte muda com isso
            */
            blt = new Boleto();
            blt.DataVencimento = new DateTime(2014, 9, 15); // O ano de vencimento entra na formação do nosso numero
            cl = Banco_Sicredi.CampoLivre(blt, "3544", "20", "00921", "00001", "1");
            Console.WriteLine(
                "Campo Livre: " + cl +
                " Nosso Número: " + blt.NossoNumeroExibicao);
            Assert.IsTrue(cl == "1114200001035442000921000", "Erro"); 
        }
    }
}