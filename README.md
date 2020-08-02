# TechChallengeCarrefour
Criação de uma solução técnica que otimiza a comunicação entre clientes e Banco Carrefour

1   - Variáveis
Substituir ">changeme1" pelo token fornecido pelo BotFather
Substituir ">changeme2" pela 'connection string' do seu DB


2   - Tabelas

2.1 - Tabela Faturas
Possui as três últimas contas dos correntistas

CREATE TABLE [dbo].[Faturas] (
    [CONTA] NUMERIC (8)    NOT NULL,
    [ST0]   BIT            NULL,
    [FAT0]  DATE           NOT NULL,
    [VAL0]  NUMERIC (8, 2) NULL,
    [ST1]   BIT            NULL,
    [FAT1]  DATE           NOT NULL,
    [VAL1]  NUMERIC (8, 2) NULL,
    [ST2]   BIT            NULL,
    [FAT2]  DATE           NOT NULL,
    [VAL2]  NUMERIC (8, 2) NULL,
    [COD0]  NVARCHAR (50)  NULL,
    [COD1]  NVARCHAR (50)  NULL,
    [COD2]  NVARCHAR (50)  NULL,
    PRIMARY KEY CLUSTERED ([CONTA] ASC)
);

onde
Conta: Numero da conta, chave da tabela
ST0,ST1,ST2: Status das faturas (true = paga / false = não paga)
FAT0,FAT1,FAT2: Data de vencimento das faturas
VAL1,VAL2,VAL3: Valor das faturas
COD1,COD2,COD3: Número do código de barras das faturas

2.2 - Tabela Pessoal
Possui as informações pessoais dos correntistas

CREATE TABLE [dbo].[Pessoal] (
    [CONTA] NUMERIC (8)  NOT NULL,
    [NOME]  VARCHAR (30) NOT NULL,
    [FONE]  VARCHAR (15) NOT NULL,
    [SALDO] NUMERIC (6)  NULL,
    PRIMARY KEY CLUSTERED ([CONTA] ASC)
);

Onde
Conta: Numero da conta, chave da tabela
Nome: Nome do correntista
Fone: Número do telefone móvel do correntista
Saldo: Saldo do correntista



