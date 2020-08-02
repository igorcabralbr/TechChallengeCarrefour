# TechChallengeCarrefour
Criação de uma solução técnica que otimiza a comunicação entre clientes e Banco Carrefour

<u><b>1   - Variáveis</b></u><br>
Substituir ">changeme1" pelo token fornecido pelo BotFather<br>
Substituir ">changeme2" pela 'connection string' do seu DB<br>


<u><b>2   - Tabelas</b></u><br>

<b>2.1 - Tabela Faturas</b><br>
Possui as três últimas contas dos correntistas<br>

CREATE TABLE [dbo].[Faturas] (<br>
    [CONTA] NUMERIC (8)    NOT NULL,<br>
    [ST0]   BIT            NULL,<br>
    [FAT0]  DATE           NOT NULL,<br>
    [VAL0]  NUMERIC (8, 2) NULL,<br>
    [ST1]   BIT            NULL,<br>
    [FAT1]  DATE           NOT NULL,<br>
    [VAL1]  NUMERIC (8, 2) NULL,<br>
    [ST2]   BIT            NULL,<br>
    [FAT2]  DATE           NOT NULL,<br>
    [VAL2]  NUMERIC (8, 2) NULL,<br>
    [COD0]  NVARCHAR (50)  NULL,<br>
    [COD1]  NVARCHAR (50)  NULL,<br>
    [COD2]  NVARCHAR (50)  NULL,<br>
    PRIMARY KEY CLUSTERED ([CONTA] ASC)<br>
);<br>
<br>
onde<br>
Conta: Numero da conta, chave da tabela<br>
ST0,ST1,ST2: Status das faturas (true = paga / false = não paga)<br>
FAT0,FAT1,FAT2: Data de vencimento das faturas<br>
VAL1,VAL2,VAL3: Valor das faturas<br>
COD1,COD2,COD3: Número do código de barras das faturas<br>
<br>
<b>2.2 - Tabela Pessoal</b><br>
Possui as informações pessoais dos correntistas<br>
<br>
CREATE TABLE [dbo].[Pessoal] (<br>
    [CONTA] NUMERIC (8)  NOT NULL,<br>
    [NOME]  VARCHAR (30) NOT NULL,<br>
    [FONE]  VARCHAR (15) NOT NULL,<br>
    [SALDO] NUMERIC (6)  NULL,<br>
    PRIMARY KEY CLUSTERED ([CONTA] ASC)<br>
);<br>
<br>
Onde<br>
Conta: Numero da conta, chave da tabela<br>
Nome: Nome do correntista<br>
Fone: Número do telefone móvel do correntista<br>
Saldo: Saldo do correntista<br>



