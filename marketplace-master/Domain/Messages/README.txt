**************************
**
** OBSOLETO
** Usar instruções em V2.Application
**
**************************

Autor: Phillippe Santana
Data Criação: 19/09/2017
Última revisão: -

COMO CRIAR UMA NOVA MENSAGEM/EMAIL/NOTIFICAÇÃO

O sistema de envio de mensagens (email é um tipo de mensagem) consiste em:
- Um template para o corpo da mensagem (v1 = embedded resource, v2 = banco de dados (ainda não implementado));
- Uma classe que especifica o template, título e model;
- ITemplateService, implementado pela infra e responsável por obter e processar o template;
- IMessageService, implementado pela infra e resposnável pelo envio (.Send);

Passos:

1. Criar o template:
	- Domain/Messages/Templates;
	- Arquivo HTML com sintaxe Razor.

2. Criar a classe da model:
	- Domain/Messages/Model;
	- Classe herda de BaseMessageModel.

3. Criar a classe de especificação do template
	- Domain/Messages;
	- Classe herda de MessageSpecificationBase<TModel>.

--------------------

Para o envio:

- Usar Domain.Helpers.MessageHelper
	- Create; ou
	- CreateAndSend

--------------------

No momento de alterar o sistema de templates para vir do banco de dados:

1. Criar a nova implementação em Infra;

2. Cadastrar os templates na base de dados mantendo o ID (templateId) que é usado hoje;

3. Associar a nova implementação pelo Unity em Presentation.
