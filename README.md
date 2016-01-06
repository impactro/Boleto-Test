Boleto-Test
===========

Testes funcionais de compatibilidade para validação das rotinas de remessa e retorno do componente [BoletoASP DLL .Net](http://www.boletoasp.com.br)

1. Exibição de Boleto
	* HTML (ASP.Net WebControl)
	* Imagem (PNG,BMP,GDI:WindowsControl)
	* Formatos: Normal ou Carne
	* Bancos Suportados
		* 001-Banco do Brasil
        * 021-Banestes
        * 027-Besc
        * 033-Banespa Santander
        * 041-Barinsul
        * 047-Banese
        * 047-BRB
        * 091-UniCred
        * 104-Caixa Económica Federal
        * 151-Nossa Caixa
        * 237-Bradesco
        * 341-Itaú
        * 347-Sudameris
        * 353-Santander
        * 356-Real
        * 389-Mercantil
        * 399-HSBC
        * 409-Unibanco
        * 422-Safra
        * 745-CitiBank
        * 748-Sicredi
        * 756-SICOOB
	
2. Remessa/Retorno (Arquivos)
	* CNAB240 Seguimentos P/Q
		* 104-Caixa - OK
		* 756-SICOOB - Remessa em teste

	* CNAB400 - Registros Tipo 1
		* 001-Banco do Brasil - OK
		* 237-Bradesco - OK (Registro Tipo 2 implementado)
		* 341-Itaú - OK
		* 353-Santander - OK
		* 748-Sicredi - Em Homologação
		* 091-UniCred - Em Homologação

3. Recursos em desenvolvimento
	* Geração em PDF - [Usando iTextSharp](http://sourceforge.net/projects/itextsharp/)