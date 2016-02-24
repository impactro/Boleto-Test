using Impactro.Cobranca;
using Impactro.Layout;
using System;

namespace Test
{
    internal class Util
    {

        public const int NossoNumeroInicial = 1000;
        public const double ValorInicial = 1000d;
        public static DateTime DataTeste = DateTime.Parse("13/12/2015");
        public static DateTime VencimentoTeste = DateTime.Parse("20/10/2015");

        static internal void AddBoletos(LayoutBancos lb)
        {
            //Definição dos dados do sacado
            SacadoInfo Sacado = new SacadoInfo();
            Sacado.Sacado = "Fábio F S";
            Sacado.Documento = "922.719.724-93";
            Sacado.Endereco = "rua qualquer lugar, S/N";
            Sacado.Cidade = "São Paulo";
            Sacado.Bairro = "Centro";
            Sacado.Cep = "55555-333";
            Sacado.UF = "SP";
            Sacado.Email = "email@provedor.com";
            Sacado.Avalista = "Avalista";

            for (int n = 0; n < 10; n++)
            {
                //Definição das Variáveis do boleto
                BoletoInfo Boleto = new BoletoInfo();
                Boleto.BoletoID = n;
                Boleto.NossoNumero = (NossoNumeroInicial + n).ToString();
                Boleto.NumeroDocumento = Boleto.NossoNumero;
                Boleto.ValorDocumento = ValorInicial + n;
                Boleto.DataDocumento = DataTeste;
                Boleto.DataVencimento = VencimentoTeste.AddDays(n);
                Boleto.Instrucoes = "Todas as informações deste bloqueto são de exclusiva responsabilidade do cedente";

                // outros campos opcionais
                Boleto.ValorMora = Boleto.ValorDocumento * 0.2 / 30; // Vale lembrar que o juros pode ser tão pequeno que as vezes pode sair como isento
                Boleto.PercentualMulta = 0.03;
                Boleto.ValorDesconto = n;
                Boleto.DataDesconto = DataTeste;
                Boleto.ValorOutras = -n; // abatimentos 
                Boleto.ParcelaNumero = 1 + (n % 3);

                lb.Add(Boleto, Sacado);
            }
        }
    }
}
