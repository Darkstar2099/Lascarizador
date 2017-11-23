# Lascarizador

Projeto para atender ao enunciado proposto pela Stone Pagamentos.
Esse produto foi patrocinado por:
- __**Bedrock Bank**__ :copyright:
- __**Pedregulho S/A**__ :copyright:
- __**Pterodactyl Airlines**__ :copyright:.

## Recursos utilizados

- **bootbox** v4.3.0 by Nick Payne
- **bootstrap** v3.3.7 by Mark Otto and Jacob Thornton
  - **bootstrap-sandstone.css** theme by Thomas Park (bootstrap theme available in bootswatch.com/sandstone)
- **Entity Framework** v6.2.0 by Microsoft
- **jQuery** v3.2.1 by jQuery Foundation
- **jquery.datatables** v1.10.15 by Allan Jardine (www.spymedia.co.uk)
- **Microsoft.aspNet.Mvc** v5.2.3 by Microsoft
- **Visual Studio 2017 Community** v15.4.4 by Microsoft

## Para rodar o projeto
1. Copiar o projeto para uma pasta e abrí-lo no Visual Studio.
2. Executar as migrações para criar os dados básicos e alguns usuários.
- Como fazer isso???

- Os seguintes dados serão criados para as tabelas abaixo listadas:
  - CardBrand(Bandeiras de Cartão): bedrock visa, bedrock master e bedrock express.
  - CardType(Tipos de Cartão): chip e tarja magnética.
  - TransactionType(Tipos de Transação): crédito e crédito parcelado.
  - Clients(Clientes): alguns clientes _conhecidos_ para podermos começar a usar o Lascarizador.

## Usando o aplicativo
O aplicativos possue quatro(4) itens básicos de menu.
- Dois(2) fazem o acesso direto ao servidor por meio unicamente da estrutura MVC.
- Dois(2) acessam o servidor através da API criada para o projeto.

### Clientes e Transações (acesso aos controllers - MVC)
Na opção de **Clientes** pode-se:
 - Listar Clientes,
 - Criar Novos Clientes,
 - Listar os Cartões de um Cliente selecionado,
 - Criar Novos Cartões para o Cliente selecionado,
 - Listar as Transações aprovadas de um Cartão selecionado e
 - Ver o Detalhe do Log de Requisição de Transação que originou a Transação selecionada.

Pode-se ver que a navegação aqui é bem amarrada aos itens selecionados possibilitando uma visão mais fechada e específica da base de dados.
 
Na opção de **Transações** somente transações que retornarem do processo de validação com status-code `aprovada` e status-reason `sucesso` serão criadas na base de dados e terão seu registro de requisição de transação criado no Log de Transações.
 
Todos os erros retornados pelo processo de validação são mostrados na tela de solicitação para que o usuário Eesteja ciente dos problemas e providencie as devidas soluções.
 
### Menu API (Log Requisições e Nova Transação)
Na opção **Log Requisições** uma listagem de todas as requisições realizadas através da API, aprovadas ou recusadas, será mostrada, bem como as requisições realizadas aprovadas realizadas na opção **Transações**.
Os Detalhes do registro no Log de Transações também pode ser visualizado para a requisição selecionada. Nos detalhes pode-se ver a lista de erros gerados pelo processo de validação que foram os responsáveis pelo status-code `recusada`.
Na opção **Nova Transação**, uma solicitação via API é simulada, com os campos informados não verificados antes do envio.

## 1.Telas Internas (MVC-Controllers)

### 1.1.Listagem de Clientes (Clientes)
![Tela de Listagem de Clientes](/images/Clientes.jpg)
Aqui são mostrados todos os clientes cadastrados na base de dados.
- A tela de edição de dados de um cliente pode ser acessada ao se clicar no link do `Nome` do cliente.
- O botão `Novo Cliente` possibilita o acesso à tela de cadastro de Novo Cliente.
- A opção `ver cartões` possibilita o acesso á tela de Listagem de Cartões.
- A opção `remover` apaga os dados do cliente, seus cartões, suas transações e o log de transações (CUIDADO)

### 1.2.Novo Cliente ou Edição de Cliente
![Tela de Novo Cliente](/images/NovoCliente.jpg)
Essa tela possibilita a entrada e edição dos dados do cliente

A auditoria das alterações de dados do cliente não está implementada... ainda.

### 1.3.Listagem de Cartões (Cartões)
![Tela de Listagem de Cartões](/images/Cartoes.jpg)
Aqui pode-se ver todos os cartões cadastrados para o cliente selecionado na tela de Listagem de Clientes.
- A tela de edição de dados de um cartão pode ser acessada ao ser clicar o link do `Número` do cartão.
- O botão `Novo Cartão` possibilita o acesso à tela de cadastro de Novo Cartão para o cliente selecionado.
- A opção `ver transações` possibilita o acesso á tela de Listagem de Cartões.
- A opção `remover` apaga os dados do cartão, suas transações e o log de transações (CUIDADO)

### 1.4.Novo Cartão ou Edição de Cartão
![Tela de Novo Cartão](/images/NovoCartao.jpg)
Essa tela possibilita a entrada e edição dos dados do cartão do cliente.

### 1.5.Listagem de Transações
![Tela de Listagem de Transações](/images/Transacoes.jpg)
Aqui pode-se ver todas as transações requisitadas e _aprovadas_ referentes a um cartão selecionado na Tela de Listagem de Cartões.
- A opção `ver requisição` possibilita o acesso á tela de Detalhe de Log de Requisições de Transação e possibilita ver os detalhes da requisição que gerou a transação.

### 1.6.Detalhe do Log de Requisições de Transação
![Tela de Detalhe de Log de Transação](/images/DetalheLogTransacao.jpg)
Essa tela possibilita visualizar os detalhes, registrados no Log de Requisição de transações, da Transação selecionada na tela de Listagem de Transações.

### 1.7.Nova Transação
![Tela de Nova Transação](/images/NovaTransacao.jpg)
Essa tela possibilita a entrada de dados necessários para se requisitar uma transação financeira junto ao validador.
Nesta tela todos os erros encontrado pelo validador são exibidos para que o usuário possa tomar as devidas providências para saná-los.
Através desta tela, somente transações aprovadas serão guardadas na base de dados. As requisições recusadas que gerariam registros no Log de Requisições de Transação com status-code `recusada` não são gerados por essa tela.
Os erros aqui são tratados como informação para correções da entrada de dados do usuário.


## 2.Telas que usam a API (Menu API)

### 2.1.Log de Requisições
![Tela de Log de Requisições](/images/LogRequisicoes.jpg)
Essa tela possibilita a visualização de todas as requisições de transação realizadas através da API.
- A tela de Detalhe do Log de Requisição de Transação pode ser acessada ao ser clicar o link do `Req#` da requisição. Na tela de detalhe os erros encontrados durante a validação estarão visíveis.
Essa tela simula uma solicitação de listagem de transações requisitas à API através de um `GET` no endereço `http://localhost:55787/api/transactionslog`

### 2.2.(API)Nova Transação
![Tela de Requisição de Transações](/images/API_NovaTransacao.jpg)
Essa tela possibilita a entrada de dados necessários para se requisitar uma transação financeira junto ao validador.
Diferente da Tela de **Transações** aqui nenhuma validação é realizada nos campos antes de enviá-los para a API.
Essa tela simula uma solicitação de aprovação de transação financeira através de um `POST` no endereço `http://localhost:55787/api/transactions`

Para mais informações em como __**consumir a API**__ acesse o documento [APIDocs](https://github.com/Darkstar2099/Lascarizador/blob/master/APIDocs.md)



