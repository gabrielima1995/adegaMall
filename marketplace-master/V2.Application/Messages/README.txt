Autor: Phillippe Santana
Data Criação: 19/09/2017
Última revisão: 12/03/2018

COMO CRIAR UMA NOVA MENSAGEM/EMAIL/NOTIFICAÇÃO

O sistema de envio de mensagens (email é um tipo de mensagem) consiste em:
- Um template para o corpo da mensagem e assunto (via banco de dados);
- Uma classe que especifica o template, variáveis e model;
- ITemplateEngine, implementado pela infra e responsável por obter e processar o template;
- IMessageSender, implementado pela infra e responsável pelo envio (.Send);

Passos:

1. Criar o template:
	- v2.Application/Modules/[NOME_DO_MODULO]/Messages/Templates;
	- Arquivo HTML com sintaxe Razor.
	- Build Action do arquivo: Embedded Resource

2. Criar a classe da model:
	- v2.Application/Modules/[NOME_DO_MODULO]/Messages;
	- Classe herda de MessageModel.

3. Criar a classe de especificação do template
	- v2.Application/Modules/[NOME_DO_MODULO]/Messages/Specs;
	- Classe herda de MessageSpecification.
	- Nome da classe deve terminar com Specification.

A partir das especificações, ao iniciar a aplicação, o sistema irá popular o template no banco de dados (caso seja novo) e 
habilitá para edição do template na admin

--------------------

Para o envio:

- Usar MessagesContext
	- Generate; ou
	- GenerateAndSend

--------------------


