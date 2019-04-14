using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Impactro.Cobranca;
using Impactro.Layout;
using System.IO;

namespace Test
{
    public partial class BB
    {
        [TestMethod, TestCategory("Retorno")]
        public void Retorno_BB()
        {
            LayoutBancos r = new LayoutBancos();
            r.Init(Cedente);

            // Registro tipo 7
            string cFileRET = File.ReadAllText(@"..\..\TXT\Retorno_BB.txt");
            r.ErroType = BoletoDuplicado.Lista;
            Layout ret = r.Retorno(cFileRET);

            // Ou usa-se o array de boletos
            foreach (string nn in r.Boletos.NossoNumeros)
            {
                BoletoInfo Boleto = r.Boletos[nn];
                Console.Write("{0} {1:C} {2:dd/MM/yyyy} {3:dd/MM/yyyy}\r\n",
                    Boleto.NossoNumero,
                    Boleto.ValorDocumento,
                    Boleto.DataVencimento,
                    Boleto.DataPagamento);
            }

            // por causa do tipo (r.ErroType) pode haver duplicidade de dados
            // pois um boleto pode ter sido baixado e protestado ou pago, 
            // e com alguma ocorrencia e assim cada registro informa algo
            Console.WriteLine("Duplicados:");
            foreach (var Boleto in r.Boletos.Duplicados)
                Console.Write("{0} {1:dd/MM/yyyy} {2:C}\r\n", Boleto.NossoNumero, Boleto.DataPagamento, Boleto.ValorDocumento);
        }
    }

    
}