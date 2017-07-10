using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Impactro.Cobranca;

namespace Test
{
    [TestClass]
    public class UtilFunc
    { 
        [TestMethod, TestCategory("Util")]
        public void RemoveAcentos()
        {
            string c1= "Acentos Atenção 1ºSei lá; ok";
            Console.WriteLine(c1);

            string c2=CobUtil.RemoveAcentos(c1);
            Console.WriteLine(c2);

            Assert.IsTrue(c1.Length == c2.Length);
        }
    }
}