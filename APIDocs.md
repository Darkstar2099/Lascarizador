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


### 2.Retornando transações

Para retornar transações você deve realizar um GET usando a rota `/transactions`.
Retorna um *array* contendo objetos de transações, ordenadas a partir da transação realizada mais recentemente.
O layout do objeto de transação pode ser encontrado no item **2.1.Criando uma transação**.

`GET` `http://localhost:55787/api/transactions`

#### Layout da estrutura de dados de envio
parâmetros | tipo | descrição | valores_possíveis | obrigatório
---: | :--: | :--- | :--- | :---:
transaction_type | *string* | Tipo de transação solicitada. | `credito`, `credito_parcelado` | :heavy_check_mark:

### 3.Retornando uma transação

Para retornar uma transação você deve realizar um GET usando a rota `/transactions/[transaction_id]`.
Retorna os dados de uma transação em específico, com as informações em um único objeto.

`GET` `http://localhost:55787/api/transactions/[transaction_id]`

#### Layout da estrutura de dados de envio

parâmetros | tipo | descrição | obrigatório
---: | :--: | :--- | :---:
transaction_id | *string* | Número identificador da transação. | :heavy_check_mark:
