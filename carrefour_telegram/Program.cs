using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Telegram.Bot;

namespace carrefour_telegram
{
    class Program
    {
        private static TelegramBotClient botClient = new TelegramBotClient(">changeme1");
        private static SqlConnection conn = new SqlConnection(@">changeme2");

        public static byte step = 0;
        public static Boolean loop = false;
        public static String timeToken;
        public static String userToken;
        public static String contakey;


        static void Main(string[] args)
        {
            botClient.OnMessage += BotClient_OnMessage;
            botClient.StartReceiving();
            Thread.Sleep(Timeout.Infinite);
            botClient.StopReceiving();
        }


        public static String GetToken(DateTime value)
        {
            return value.ToString("sffff");
        }


        static async void BotClient_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {

// STEP 0 (MENU PRINCIPAL)
            if (step == 0)
            {
                if (e.Message.Text.ToUpper() == "OI")
                {
                    botClient.SendTextMessageAsync(e.Message.Chat.Id, $"Olá, tudo bem?" +
                    @"
Como posso te ajudar?
/conta (Para informações da conta) 
/help  (Para telefones de contato)
                 ");
                }

                else if (e.Message.Text.ToUpper() == "/HELP")
                {
                    botClient.SendTextMessageAsync(e.Message.Chat.Id, @" Entre em contato com nossa 
Central de Relacionamento:
* São Paulo e Regiões Metropolitanas
  3004 2222
* Demais Localidades
  0800 718 2222");
                }

                else if (e.Message.Text.ToUpper() == "/CONTA")
                {
                    await botClient.SendTextMessageAsync(e.Message.Chat.Id, @"Vamos lá, Digite o numero da sua conta");
                    step = 1; loop = false;
                }
                else
                {
                    botClient.SendTextMessageAsync(e.Message.Chat.Id, $"Hum, Opção inválida" +
                    @"
Como posso te ajudar?
/conta (Para informações da conta) 
/help  (Para demais informações)");
                }
            }

// STEP 1 (CONSULTA CONTA)
            if (step == 1 && loop)
            {
                string conta = e.Message.Text;
                contakey = conta;
                conn.Open();
                SqlCommand cmdo = conn.CreateCommand();
                cmdo.CommandType = CommandType.Text;
                cmdo.CommandText = "SELECT * FROM dbo.Pessoal WHERE conta='"+conta+"'";

                SqlDataReader dr;
                dr = cmdo.ExecuteReader();
                if(dr.HasRows)
                {
                dr.Read();
 //                   Console.WriteLine(dr.GetString(0));
                    await botClient.SendTextMessageAsync(e.Message.Chat.Id, @"Tudo bem, conta encontrada,
Por segurança vou enviar seu código de acesso por sms
Digite ele aqui quando chegar");
                
                 String timeStamp = GetToken(DateTime.Now);
                 timeToken = timeStamp;
                 Console.WriteLine($"Enviando código para o numero de celular +{dr.GetString(2)}");
                    Console.WriteLine($".........");
                 Console.WriteLine($"{dr.GetString(1)} Seu código é: {timeStamp}");

                    step = 2; loop = false;
                }
                else
                {
                await botClient.SendTextMessageAsync(e.Message.Chat.Id, @"Hum, não encontramos o número  
informado, vamos tentar denovo?
Digite: 
/conta (Para informações da conta) 
/help  (Para demais informações)");
                step = 0;
                }
 //               dr.Read();
                conn.Close();
            }

            // STEP 2 (VALIDA TOKEN)
            if (step == 2 && loop)
            {
                if (e.Message.Text == timeToken)
                {
                    await botClient.SendTextMessageAsync(e.Message.Chat.Id, @"Tudo certo, código validado,
O que você gostaria de consultar?
/fatura (Informações sobre pagamento de faturas)
/saldo (Consulta do saldo da conta)
/sair  (Encerra a consulta)");

                    step = 3; loop = false;
                }
                else
                {
                    await botClient.SendTextMessageAsync(e.Message.Chat.Id, @"Hum, o código informado é inválido  
Vamos tentar denovo?
Digite: 
/conta (Para informações da conta) 
/help  (Para demais informações)");
                    step = 0;
                }
            }

// STEP 3 (FATURAS / SALDO)
            if (step == 3 && loop)
            {
                if (e.Message.Text.ToUpper() == "/SALDO")
                {
                    conn.Open();
                    SqlCommand cmdo = conn.CreateCommand();
                    cmdo.CommandType = CommandType.Text;
                    cmdo.CommandText = "SELECT SALDO FROM dbo.Pessoal WHERE conta='" + contakey + "'";

                    SqlDataReader dr;
                    dr = cmdo.ExecuteReader();
                    dr.Read();

                    botClient.SendTextMessageAsync(e.Message.Chat.Id, $"Seu saldo é de R${dr.GetDecimal(0)},00");

                    conn.Close();
                    await Task.Delay(1000);
                    botClient.SendTextMessageAsync(e.Message.Chat.Id, @"Gostaria de consultar mais alguma coisa?,
/fatura (Informações sobre pagamento de faturas)
/saldo (Consulta do seu saldo)
/sair  (Encerra a consulta)");
                }

                else if (e.Message.Text.ToUpper() == "/FATURA")
                {
                    conn.Open();
                    SqlCommand cmdo = conn.CreateCommand();
                    cmdo.CommandType = CommandType.Text;
                    cmdo.CommandText = "SELECT * FROM dbo.FATURAS WHERE conta='" + contakey + "'";

                    SqlDataReader dr;
                    dr = cmdo.ExecuteReader();
                    dr.Read();

                    //fatura nao paga
                    if (dr.GetBoolean(1) == false || dr.GetBoolean(4) == false || dr.GetBoolean(7) == false)
                    {
                        if (dr.GetBoolean(1) == false)
                        {
                            await botClient.SendTextMessageAsync(e.Message.Chat.Id, $"Vencimento em {dr.GetDateTime(2).ToString("dd/MM/yyyy")}, R${dr.GetDecimal(3)} - Não Paga");
                        }
                        if (dr.GetBoolean(4) == false)
                        {
                            await botClient.SendTextMessageAsync(e.Message.Chat.Id, $"Vencimento em {dr.GetDateTime(5).ToString("dd/MM/yyyy")}, R${dr.GetDecimal(6)} - Não Paga");
                        }
                        if (dr.GetBoolean(7) == false)
                        {
                            await botClient.SendTextMessageAsync(e.Message.Chat.Id, $"Vencimento em {dr.GetDateTime(8).ToString("dd/MM/yyyy")}, R${dr.GetDecimal(9)} - Não Paga");
                        }
                        step = 4; loop = false;

                    }

                    //fatura paga
                    if (dr.GetBoolean(1) == true || dr.GetBoolean(4) == true || dr.GetBoolean(7) == true)
                    {
                        if (dr.GetBoolean(1) == true)
                        {
                            await botClient.SendTextMessageAsync(e.Message.Chat.Id, $"Vencimento em {dr.GetDateTime(2).ToString("dd/MM/yyyy")}, R${dr.GetDecimal(3)} - Paga");
                        }
                        if (dr.GetBoolean(4) == true)
                        {
                            await botClient.SendTextMessageAsync(e.Message.Chat.Id, $"Vencimento em {dr.GetDateTime(5).ToString("dd/MM/yyyy")}, R${dr.GetDecimal(6)} - Paga");
                        }
                        if (dr.GetBoolean(7) == true)
                        {
                            await botClient.SendTextMessageAsync(e.Message.Chat.Id, $"Vencimento em {dr.GetDateTime(8).ToString("dd/MM/yyyy")}, R${dr.GetDecimal(9)} - Paga");
                        }

                        //se tudo pago
                        if (dr.GetBoolean(1) == true && dr.GetBoolean(4) == true && dr.GetBoolean(7) == true)
                        {
                            await botClient.SendTextMessageAsync(e.Message.Chat.Id, @"Todas suas contas estão em dia!
Gostaria de consultar mais alguma coisa?,
/fatura (Informações sobre pagamento de faturas)
/saldo (Consulta do seu saldo)
/sair  (Encerra a consulta)");
                        }

                    }
                    conn.Close();
                }

                else if (e.Message.Text.ToUpper() == "/SAIR")
                {
                    await botClient.SendTextMessageAsync(e.Message.Chat.Id, "O Banco Carrefour agradece a sua preferência, volte sempre!");
                    step = 0;
                }
                else
                {
                    await botClient.SendTextMessageAsync(e.Message.Chat.Id, @"Hum, Opcão invalida,
O que você gostaria de consultar?
/fatura (Informações sobre pagamento de faturas)
/saldo (Consulta do seu saldo)
/sair  (Encerra a consulta)");
                }
            }



// CONSULTA DOS NUMEROS DE BOLETOS NAO PAGOS
            if (step == 4 && !loop)
            {
                await botClient.SendTextMessageAsync(e.Message.Chat.Id, @"Gostaria que eu te enviasse o número do código  
de barras da(s) fatura(s) não paga(s) ?
/sim 
/nao");
            }
            if (step == 4 && loop)
            {
                if (e.Message.Text.ToUpper() == "/SIM")
                {
                    // ENVIA NUMERO DOS BOLETOS

                    conn.Open();
                    SqlCommand cmdo = conn.CreateCommand();
                    cmdo.CommandType = CommandType.Text;
                    cmdo.CommandText = "SELECT * FROM dbo.FATURAS WHERE conta='" + contakey + "'";

                    SqlDataReader dr;
                    dr = cmdo.ExecuteReader();
                    dr.Read();

                    //fatura nao paga
                    if (dr.GetBoolean(1) == false || dr.GetBoolean(4) == false || dr.GetBoolean(7) == false)
                    {
                        if (dr.GetBoolean(1) == false)
                        {
                            await botClient.SendTextMessageAsync(e.Message.Chat.Id, $"Vencimento em {dr.GetDateTime(2).ToString("dd/MM/yyyy")}");
                            await botClient.SendTextMessageAsync(e.Message.Chat.Id, $"{dr.GetString(10)}");
                        }
                        if (dr.GetBoolean(4) == false)
                        {
                            await botClient.SendTextMessageAsync(e.Message.Chat.Id, $"Vencimento em {dr.GetDateTime(5).ToString("dd/MM/yyyy")}");
                            await botClient.SendTextMessageAsync(e.Message.Chat.Id, $"{dr.GetString(11)}");
                        }
                        if (dr.GetBoolean(7) == false)
                        {
                            await botClient.SendTextMessageAsync(e.Message.Chat.Id, $"Vencimento em {dr.GetDateTime(8).ToString("dd/MM/yyyy")}");
                            await botClient.SendTextMessageAsync(e.Message.Chat.Id, $"{dr.GetString(12)}");
                        }
                    }
                    conn.Close();
                    await botClient.SendTextMessageAsync(e.Message.Chat.Id, @"Gostaria de consultar mais alguma coisa?,
/fatura (Informações sobre pagamento de faturas)
/saldo (Consulta do seu saldo)
/sair  (Encerra a consulta)");
                    step = 3;
                }
                else if ((e.Message.Text.ToUpper() == "/NAO") || (e.Message.Text.ToUpper() == "/NÃO"))
                {
                    await botClient.SendTextMessageAsync(e.Message.Chat.Id, @"Hum, Gostaria de consultar mais alguma coisa?,
/fatura (Informações sobre pagamento de faturas)
/saldo (Consulta do seu saldo)
/sair  (Encerra a consulta)");
                    step = 3;
                }
                else
                {
                    await botClient.SendTextMessageAsync(e.Message.Chat.Id, @" Hum, opção inválida, 
Gostaria que eu te enviasse o número do   
código de barras da(s) fatura(s) não paga(s) ?
/sim 
/nao");
                }
            }
            loop = true;
        }
    }
}
