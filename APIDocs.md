## Usando a API

A *rota básica* utilizada nos casos abaixo será: `http://localhost:55787/api`.
Todas as *rotas* descritas partem da *rota básica*.
- **Por exempo**: Ao enviar uma transação use a rota `/transactions`, significa: use `http://localhost:55787/api/transactions`

### 1.Criando uma transação

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

#### a)Objeto Requisição de Transação
Ao criar uma transação, este é o objeto que você recebe como resposta do processo de efetivação da transação, bem como o link para a transação criada.

campo | tipo | descrição | valores_possíveis
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
transaction_log_id | *int* | Número identificador da requisição da transação. | 


### 2.Retornando transações

Para retornar transações você deve realizar um GET usando a rota `/transactions`.
Retorna um *array* contendo **b)Objetos Transação**, ordenadas a partir da transação realizada mais recentemente.
O layout do objeto de transação pode ser encontrado logo abaixo.

`GET` `http://localhost:55787/api/transactions`

#### b)Objeto Transação
Ao solicitar transações, este é o objeto que você recebe como resposta.

campo | tipo | descrição | valores_possíveis
---: | :--: | :--- | :--- 
amount | *int* | Valor da transação (em centavos) |
**card** | *objeto* | Dados sobre o Cartão. | 
**card**/**card_brand** | *objeto* | Dados sobre a Banderia do Cartão. |
**card**/**card_brand**/id | *int* | Número identificador da Bandeira. |
**card**/**card_brand**/name | *string* | Bandeira do Cartão. | `bedrock_visa`, `bedrock_master`, `bedrock_express`
**card**/**client** | *objeto* | Dados sobre o Cliente do Cartão |
**card**/**client**/_cpf_ | *string* | CPF do Cliente do Cartão |
**card**/**client**/_email_ | *string* | E-mail do Cliente do Cartão |
**card**/**client**/_name_ | *string* | Nome do Cliente do Cartão |
**card**/_expiration_month_ | *int* | Mês de expiração do Cartão. |
**card**/_expiration_year_ | *int* | Ano de expiração do Cartão. |
**card**/first_digits | *string* | Quatro(4) primeiros números do cartão |
**card**/last_digits | *string* | Quatro(4) últimos números do cartão |
creation_timestamp | *timestamp* | Data e hora da criação da transação |
id | *int* | Número identificador da Transação. | 
installments | *int* | Quantidade de parcelas (somente válido para transações do tipo parcelado. Atualmente somente para transaction_type = `credito_parcelado` |
transaction_log_id | *int* | Número identificador da requisição da transação. | 
**transaction_type** | *objeto* | Dados sobre o Tipo de transação. |
**transaction_type**/_id_ | *int* | Número identificador do Tipo de transação. |
**transaction_type**/_type_ | *string* | Descrição do Tipo de transação. | `credito`, `credito_parcelado`


### 3.Retornando uma transação

Para retornar uma transação você deve realizar um GET usando a rota `/transactions/[transaction_id]`.
Retorna os dados de uma transação específica, com as informações em um único **b)Objeto Transação**.

`GET` `http://localhost:55787/api/transactions/[transaction_id]`

#### Layout da estrutura de dados de envio

parâmetros | tipo | descrição | obrigatório
---: | :--: | :--- | :---:
transaction_id | *string* | Número identificador da transação. | :heavy_check_mark:

### 4.Retornando o Log de Requisições de Transações

Para retornar o log de requisições de transações você deve realizar um GET usando a rota `/transactionslog`.
Retorna um *array* contendo **a)Objeto Requisição de Transação**, ordenadas a partir da transação realizada mais recentemente.
O layout do objeto pode ser encontrado logo acima.

`GET` `http://localhost:55787/api/transactionslog`

### 5.Retornando um registro do Log de Requisições de Transações

Para retornar um registro do log de requisições de transação você deve realizar um GET usando a rota `/transactionslog/[transaction_id]`.
Retorna os dados de um registro de requisição de transação específico, com as informações em um único **a)Objeto Requisição de Transação**.

`GET` `http://localhost:55787/api/transactionslog/[transaction_log_id]`

#### Layout da estrutura de dados de envio

parâmetros | tipo | descrição | obrigatório
---: | :--: | :--- | :---:
transaction_log_id | *string* | Número identificador da requisição de transação. | :heavy_check_mark:
