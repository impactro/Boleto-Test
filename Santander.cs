﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Impactro.Cobranca;
using Impactro.Layout;
using System.IO;

namespace Test
{
    [TestClass]
    public class Santander
    {
        CedenteInfo Cedente;

        [TestInitialize]
        public void Init()
        {
            Cedente = new CedenteInfo();
            Cedente.Banco = "033";
            Cedente.Agencia = "1234-1";
            Cedente.Conta = "001234567-8";
            Cedente.CodCedente = "1231230";
            Cedente.CarteiraTipo = "5"; // Fundamental para homologação!
            Cedente.Carteira = "101";
            Cedente.CedenteCOD = "33333334892001304444"; // 20 digitos (note que o final, é o numero da conta, sem os ultios 2 digitos)
            Cedente.Convenio = "0000000000000000002222220"; // 25 digitos
            Cedente.Cedente = "TESTE QUALQUER LTDA";
            Cedente.CNPJ = "88.083.264/0001-05";
            Cedente.useSantander = true; //importante para gerar o código de barras correto (por questão de compatibilidade o padrão é false)
        }

        [TestMethod, TestCategory("Remessa")]
        public void Remessa_Santander240()
        {
            LayoutBancos lb = new LayoutBancos();
            Cedente.Layout = LayoutTipo.CNAB240; // O Santander tem os dois layouts, o CNAB400 é sempre o padrão se selecionar auto
            lb.Init(Cedente);
            lb.DataHoje = Util.DataTeste;
            Util.AddBoletos(lb);
            // lb.ShowDumpLine = true;
            string txt = lb.Remessa();
            Console.Write(txt);
            string cAnterior;
            File.WriteAllText(@"..\..\TXT\Teste_Santander240.txt", txt); // Gera um arquivo para testes de compraração
            // File.WriteAllText(@"..\..\TXT\Remessa_Santander240.txt", txt); // Gera um novo modelo
            cAnterior = File.ReadAllText(@"..\..\TXT\Remessa_Santander240.txt");

            // Isso necessáriamente não é um erro, pode ter sido uma correção ou melhoria que agora contemple mais casos
            Assert.IsTrue(cAnterior == txt, "O resultado da remessa mudou");
        }

        [TestMethod, TestCategory("Remessa")]
        public void Remessa_Santander400()
        {
            LayoutBancos lb = new LayoutBancos();
            Cedente.Layout = LayoutTipo.CNAB400; // layout padrão
            lb.Init(Cedente);
            lb.DataHoje = Util.DataTeste;
            Util.AddBoletos(lb);
            // lb.ShowDumpLine = true;
            string txt = lb.Remessa();
            Console.Write(txt);
            string cAnterior;
            File.WriteAllText(@"..\..\TXT\Teste_Santander400.txt", txt); // Gera um arquivo para testes de compraração
            // File.WriteAllText(@"..\..\TXT\Remessa_Santander400.txt", txt); // Gera um novo modelo
            cAnterior = File.ReadAllText(@"..\..\TXT\Remessa_Santander400.txt");

            // Isso necessáriamente não é um erro, pode ter sido uma correção ou melhoria que agora contemple mais casos
            Assert.IsTrue(cAnterior == txt, "O resultado da remessa mudou");
        }

        [TestMethod, TestCategory("Retorno")]
        public void Retorno_Santander()
        {
            LayoutBancos r = new LayoutBancos();
            r.Init(Cedente);

            string cFileRET = File.ReadAllText(@"..\..\TXT\Retorno_Santander.txt");
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
    }
}