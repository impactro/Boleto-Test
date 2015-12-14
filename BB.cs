using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Impactro.Cobranca;
using Impactro.Layout;
using System.IO;

namespace Test
{
    [TestClass]
    public class BB
    {
        [TestMethod, TestCategory("CampoLivre")]
        public void CampoLivre_BB()
        {
            Boleto blt = new Boleto();
            string cl;

            // Logica 1
            cl = Banco_do_Brasil.CampoLivre(blt, "", "", "2719715", "19", "18", "27197150000000014");
            Console.WriteLine(
                "Campo Livre para código do cedente de 7 dígitos e nosso número de 17 dígitos: " + cl +
                " Agência/Conta: " + blt.AgenciaConta +
                " Nosso Número: " + blt.NossoNumeroExibicao);

            Assert.IsTrue(cl == "0000002719715000000001418");
            // Nosso Numero      ------12345678901234567--
        }
    }
}