# Lascarizador

Projeto para atender ao enunciado proposto pela Stone
Esse produto foi patrocinado por __**Bedrock Bank**__ :copyright:

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

## 1.Usando o aplicativo

### 1.1.Listagem de clientes

### 1.2.Novo Cliente

### 1.3.Listagem de cartões

### 1.4.Novo Cartão

### 1.5.Listagem de Transações

### 1.6.Nova Transação

## 2.Usando a API

A *rota básica* utilizada nos casos abaixo será: `http://localhost:55787/api`.
Todas as *rotas* descritas partem da *rota básica*.
- **Por exempo**: Ao enviar uma transação use a rota `/transactions`, significa: use `http://localhost:55787/api/transactions`

### 2.1.Criando uma transação

Para fazer uma cobrança você deve realizar um POST usando a rota `/transactions` para criar a sua transação.

`POST` `http://localhost:55787/api/transactions`

#### Layout da estrutura de dados de envio
parâmetros | tipo | descrição | valores_possíveis | obrigatório
---: | :--: | :--- | :--- | :---:
transaction_type | *string* | Tipo de transação solicitada. | `credito`, `credito_parcelado` | :heavy_check_mark:
card_brand | *string* | Bandeira do cartão informado. | `bedrock_visa`, `bedrock_master`, `bedrock_express` | :heavy_check_mark:
card_number | *string* | Número do cartão informado. | númericos de 12 a 16 de comprimento | :heavy_check_mark:
card_holder_name | *string* | Nome do portador do cartão informado. | caracteres alfanuméricos em caixa alta | :heavy_check_mark:
expiration_month | *string* | Mês de expiração do cartão informado. | númericos de  1 até 12 | :heavy_check_mark:
expiration_year | *string* | Ano de expiração do cartão informado. | numéricos de 2017 até 2100 | :heavy_check_mark:
cvv | *string* | cvv do cartão informado. | numéricos de 3 de comprimento | :heavy_check_mark:
amount | *int* | Valor (em centavos) da transação solicitada. | numéricos maiores ou igual a 10 (mínimo de 10 centavos) |  :heavy_check_mark:
installments | *int* | Quantidade de parcelas da transação. | numéricos de 1 até 12 (somente obrigatórios para transaction_type = `credito_parcelado` | 
password | *string* | Senha do cartão informado. | numéricos de 4 até 6 de comprimento (somente obrigatório se o cartão informado exigir senha) |

#### Objeto Transação
Ao criar uma transação, este é o objeto que você recebe como resposta do processo de efetivação da transação.

propiedade | tipo | descrição | valores_possíveis
---: | :--: | :--- | :--- 
transaction_id | *int* | Número identificador da transação. | 
transaction_type | *string* | Tipo de transação solicitada. | `credito`, `credito_parcelado`
card_brand | *string* | Bandeira do cartão informado. | `bedrock_visa`, `bedrock_master`, `bedrock_express`
card_number_first | *string* | Quatro(4) primeiros números do cartão |
card_number_last | *string* | Quatro(4) últimos números do cartão |
card_holder_name | *string* | Nome do portador do cartão |
amount | *int* | Valor da transação (em centavos) |
installments | *int* | Quantidade de parcelas (somente válido para transações do tipo parcelado. Atualmente somente para transaction_type = `credito_parcelado` |
creation_timestamp | *timestamp* | Data e hora da criação da transação |
status_code | *string* | Código de status da transação | `paid`, `refused`
status_reason | *string* | Razão do código de status |


### 2.2.Retornando transações

Para retornar transações você deve realizar um GET usando a rota `/transactions`.
Retorna um *array* contendo objetos de transações, ordenadas a partir da transação realizada mais recentemente.
O layout do objeto de transação pode ser encontrado no item **2.1.Criando uma transação**.

`GET` `http://localhost:55787/api/transactions`

#### Layout da estrutura de dados de envio
parâmetros | tipo | descrição | valores_possíveis | obrigatório
---: | :--: | :--- | :--- | :---:
transaction_type | *string* | Tipo de transação solicitada. | `credito`, `credito_parcelado` | :heavy_check_mark:

### 2.3.Retornando uma transação

Para retornar uma transação você deve realizar um GET usando a rota `/transactions/[transaction_id]`.
Retorna os dados de uma transação em específico, com as informações em um único objeto.

`GET` `http://localhost:55787/api/transactions/[transaction_id]`

#### Layout da estrutura de dados de envio

parâmetros | tipo | descrição | obrigatório
---: | :--: | :--- | :---:
transaction_id | *string* | Número identificador da transação. | :heavy_check_mark:




