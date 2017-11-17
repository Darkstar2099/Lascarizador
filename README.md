# Lascarizador

Projeto para atender ao enunciado proposto pela Stone Pagamentos.
Esse produto foi patrocinado por:
- __**Bedrock Bank**__ :copyright:
- __**Pedregulho S/A**__ :copyright:
- __**Pterodactyl Airlines**__ :copyright:.

## Recursos utilizados

- **bootbox** v4.3.0 by Nick Payne
- **bootstrap** v3.0.0 by Mark Otto and Jacob Thornton
  - **bootstrap-sandstone.css** by Thomas Park (bootstrap theme available in bootswatch.com/sandstone)
- **Entity Framework** v6.2.0 by Microsfot
- **jQuery** v1.10.2 by jQuery Foundatino
- **jquery.datatables** v1.10.15 by Allan Jardine (www.spymedia.co.uk)
- **Microsoft.aspNet.Mvc** v5.2.3 by Microsoft
- **Visual Studio 2017 Community** v15.4.3 by Microsoft

## Para rodar o projeto
1. Copiar o projeto para uma pasta e abrí-lo no Visual Studio.
2. Executar as migrações.

## Usando o aplicativo
O aplicativos possue quatro(4) itens básicos de menu. Dois(2) fazem o acesso direto ao servidor por meio unicamenta da estrutura MVC. Dois(2) acessam o servidor através da API criada para o projeto.

### Clientes e Nova Transação (acesso aos controllers - MVC)
Na opção de **Clientes** pode-se:
 - Listar Clientes,
 - Criar Novos Clientes,
 - Listar os Cartões de um Cliente selecionado,
 - Criar Novos Cartões para o Cliente selecionado,
 - Listar as Transações aprovadas de um Cartão selecionado e
 - Ver o Detalhe do Log de Requisição de Transação que originou a Transação selecionada.

Pode-se ver que a navegação é bem amarrada aos itens selecionados possibilitando uma visão mais fechada e específica da base de dados.
 
Na opção de **Nova Transação** somente transações que retornarem do processo de validação com status-code `aprovada` e status-reason `sucesso` serão criadas na base de dados e terão seu registro de requisição de transação criado no Log de Transações.
 
Todos os erros retornados pelo processo de validação são mostrados na tela de solicitação para que o usuário possa estar ciente dos problemas e providenciar as devidas soluções
 
 
### (API)Lista Transações e (API)Nova Transação
Na opção **(API)Lista Transações** uma listagem de todas as requisições realizadas através da API, aprovadas ou recusadas, será mostrada, bem como as requisições realizadas aprovadas realizadas na opção **Nova Transação**.
Os Detalhes do registros no Log de Transações também pode ser visualizado para a requisição selecionada. Nos detalhes pode-se ver a lista de erros gerados pelo processo de validação que foram os responsáveis pelo status-code `recusada`.

## 1.Telas Internas (MVC-Controllers)

### 1.1.Listagem de clientes
![Tela de Listagem de Clientes](/images/Clientes.jpg)
Aqui são mostrados todos os clientes cadastrados na base de dados.
- A tela de edição de dados de um cliente pode ser acessada ao se clicar no link do `Nome` do cliente.
- O botão `Novo Cliente` possibilita o acesso à tela de cadastro de novo cliente.
- A opção `ver cartões` possibilita o acesso á tela de Listagem de Cartões.
- A opção `remover` apaga os dados do cliente, seus cartões, suas transações e o log de transações (CUIDADO)

### 1.2.Novo Cliente ou Edição de Cliente
![Tela de Novo Cliente](/images/NovoCliente.jpg)
Essa tela possibilita a entrada e edição dos dados do cliente

A auditoria das alterações de dados do cliente não está implementada... ainda.

### 1.3.Listagem de cartões
![Tela de Listagem de Cartões](/images/Cartoes.jpg)
- A tela de edição de dados de um cartão pode ser acessada ao ser clicar o link do `Número` do cartão.
- O botão `Novo Cartão` possibilita o acesso à tela de cadastro de novo cartão para o cliente em questão.
- A opção `ver transações` possibilita o acesso á tela de Listagem de Cartões.
- A opção `remover` apaga os dados do cliente, seus cartões, suas transações e o log de transações (CUIDADO)

### 1.4.Novo Cartão
![Tela de Novo Cartão](/images/NovoCartao.jpg)

### 1.5.Listagem de Transações
![Tela de Listagem de Transações](/images/Transacoes.jpg)

### 1.6.Nova Transação
![Tela de Nova Transação](/images/NovaTransacao.jpg)

### 1.7.Detalhe do Log de Requisições de Transação
![Tela de Detalhe de Log de Transação](/images/DetalheLogTransacao.jpg)


## 2.Telas que consomem a API

### 2.1.(API)Lista Transações

### 2.2.(API)Nova Transação

Para mais informações em como consumir a API acesse o documento [APIDocs](https://github.com/Darkstar2099/Lascarizador/edit/master/APIDocs.md)



