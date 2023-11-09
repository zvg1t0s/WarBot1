using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.VisualBasic;
using System.Configuration;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Polling;
using System.Data.SqlTypes;
using System.Timers;

using System.Text;

namespace WarBot1
{
    internal class Program
    {
        private static System.Timers.Timer aTimer;
        public static string gameName;
        private static System.Timers.Timer PayDayTimer;
        private static TelegramBotClient client;

        static void Main(string[] args)
        {


            String token = "6825545252:AAF_hRuw_yy9g4rPjxZfj77D8n5I8riKFs8";
            client = new TelegramBotClient(token);
            using var cts = new CancellationTokenSource();
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }
            };

            client.StartReceiving(Update, Error, receiverOptions, cts.Token);

            Console.ReadLine();
            cts.Cancel();


        }

        async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            string S1 = "";

            int number1 = 0;
            int number13 = 0;
            var message = update?.Message;
            var me = botClient.GetMeAsync();





            if (update?.Message?.Text != null)
            {
                User user3 = Game.keyValuePairs.Where(z => z.Value.userId == 0).FirstOrDefault().Value;
                User usr3 = new User();
                if (update.Message.Text == "/st" && Game.isStarted && Game.keyValuePairs.ContainsKey(message.From.ToString()) && Game.keyValuePairs.TryGetValue(user3.name, out usr3))
                {


                    if (usr3.userId == 0)
                    {
                        PayDayTimer = new System.Timers.Timer(30000);
                        // Hook up the Elapsed event for the timer. 
                        PayDayTimer.Elapsed += OnTimedEvent;

                        PayDayTimer.Enabled = true;
                        PayDayTimer.AutoReset = true;


                        async void OnTimedEvent(object source, ElapsedEventArgs e)
                        {
                            await botClient.SendTextMessageAsync(-4090139549, $"{DateTime.Now}");
                            return;
                        }
                    }

                }
                if (message.Text.ToLower().Contains("/startgame"))
                {
                    S1 = update.Message.From.ToString();



                    if (!Game.isStarted)
                    {
                        InlineKeyboardMarkup keyboard = new(new[]
                        {
                        InlineKeyboardButton.WithCallbackData("Африка"),
                        InlineKeyboardButton.WithCallbackData("Европа")
                    });

                        await botClient.SendTextMessageAsync(message.Chat.Id, $"выберите локацию {update.Message.From}", replyMarkup: keyboard);

                        return;

                    }
                    else { await botClient.SendTextMessageAsync(message.Chat.Id, "Игра уже идет!"); return; }

                }
                if (message.Text.ToLower().Contains("/deletegame"))
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Игра окончена!");
                    Game.isStarted = false;
                    Game.keyValuePairs.Clear();



                    return;


                }
                if (message.Text.ToLower().Contains("/go") && Game.isStarted && !Game.keyValuePairs.ContainsKey(message.From.ToString()))
                {

                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Генерал {message.From} присоединился к игре");
                    Game.keyValuePairs.Add(message.From.ToString(), new User(message.From.ToString()));
                    number13 += 1;
                    return;


                }
                else if (message.Text.ToLower().Contains("/go") && (!Game.isStarted || Game.keyValuePairs.ContainsKey(message.From.ToString())))
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Игра началась, либо вы уже зарегистрированы!");
                }
                if (message.Text.ToLower().Contains("/users"))
                {

                    string users = "";
                    foreach (var person in Game.keyValuePairs)
                    {

                        users += $" {person.Value.name} [{person.Value.userId}] с деньгами {person.Value.money}$\n";
                    }
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Список генералов в текущей игре:\n{users}");
                }
                if (message.Text.ToLower().Contains("/help"))
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Список команд: \n /startgame [название] - создать игру\n /go - присоединиться к игре \n /копать - добывать камень\n /продать [название] [количество] - продать что-то");

                }
                if ((message.Text.ToLower().Contains("/mine") || message.Text.ToLower().Contains("/копать")) && Game.keyValuePairs.ContainsKey(message.From.ToString()))
                {

                    User usr = new User();
                    if (Game.keyValuePairs.TryGetValue(message?.From.ToString(), out usr))
                    {
                        if (usr.Kirka)
                        {
                            int minedNow = Game.MinePlus();
                            await botClient.SendTextMessageAsync(message.Chat.Id, $"{usr.name} вы добыли {minedNow.ToString()} кубов камня");
                            usr.rocks += minedNow;
                        }
                        else if (!usr.Kirka)
                        {
                            int minedNow = Game.Mine();
                            await botClient.SendTextMessageAsync(message.Chat.Id, $"{usr.name} вы добыли {minedNow.ToString()} кубов камня");
                            usr.rocks += minedNow;
                        }

                    }
                }
                if ((message.Text.ToLower().Contains("/rocks") || message.Text.ToLower().Contains("/камни")) && Game.keyValuePairs.ContainsKey(message.From.ToString()))
                {
                    User usr = new User();
                    if (Game.keyValuePairs.TryGetValue(message.From.ToString(), out usr))
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"{usr.name} у вас {usr.rocks} кубов камня");
                    }
                }
                if ((message.Text.ToLower().Contains("/sell ") || message.Text.ToLower().Contains("/продать ") || message.Text.ToLower().Contains("/пр ")) && Game.keyValuePairs.ContainsKey(message?.From.ToString()))
                {

                    User usr = new User();
                    if (Game.keyValuePairs.TryGetValue(message.From.ToString(), out usr))
                    {
                        string[] s = message.Text.ToLower().Split(' ');
                        int num = 0;
                        if (Int32.TryParse(s[2], out num))
                        {
                            if (s[1].ToLower().Contains("кам") && num <= usr.rocks)
                            {
                                usr.money += 2000 * Int32.Parse(s[2]);
                                usr.rocks -= Int32.Parse(s[2]);
                                await botClient.SendTextMessageAsync(message.Chat.Id, $"{usr.name} Вы продали {Int32.Parse(s[2])} кубов камня за {2000 * Int32.Parse(s[2])}");
                            }
                            else if (s[1].ToLower().Contains("кам") && Int32.Parse(s[2]) > usr.rocks)
                            {
                                await botClient.SendTextMessageAsync(message.Chat.Id, $"{usr.name} У вас нет столько камня!");
                            }
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, $"{usr.name} введите корректное число!");
                        }
                    }

                }
                if ((message.Text.ToLower().Contains("/stat") || message.Text.ToLower().Contains("/стат")) && Game.keyValuePairs.ContainsKey(message.From.ToString()))
                {

                    User usr = new User();
                    if (Game.keyValuePairs.TryGetValue(message.From.ToString(), out usr))
                    {
                        int minedNow = Game.Mine();

                        //await botClient.SendTextMessageAsync(message.Chat.Id, $"статистика {usr.name}\n денег: {usr.money.ToString()} \n камня: {usr.rocks.ToString()} \n войск {usr.soldiers.ToString()} \n рейтинга {usr.rating}");
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"Статистика\r\n\r\n\r\n👤 {usr.name}\r\n\r\n💵 Деньги: {usr.money.ToString()}\r\n💎 Камни: {usr.rocks.ToString()}\r\n\r\n🛡 войско: {usr.soldiers.ToString()}");
                    }
                }
                if ((message.Text.ToLower().Contains("/магаз") || message.Text.ToLower().Contains("/shop") || message.Text.ToLower().Contains("/buy")) && Game.keyValuePairs.ContainsKey(message.From.ToString()))
                {


                    await botClient.SendTextMessageAsync(message.Chat.Id, $"🛍Магазин🛍:\n 1) кирка⛏ - вы добываете больше камня - 300.000$\n 2)🪖нанять солдата - 500.000$ \n 3) 🏆купить рейтинг - 1.000.000$\n\n Чтобы купить введите /buy [номер]‼️");


                }
                if ((message.Text.ToLower().Contains("/buy ") || message.Text.ToLower().Contains("/купить ")) && Game.keyValuePairs.ContainsKey(message.From.ToString()))
                {
                    User usr = new User();
                    if (Game.keyValuePairs.TryGetValue(message.From.ToString(), out usr))
                    {
                        string[] s = message.Text.ToLower().Split(' ');
                        if (s[1].ToLower().Contains("1") && 300000 <= usr.money && !usr.Kirka)
                        {
                            usr.money -= 300000;
                            usr.Kirka = true;
                            await botClient.SendTextMessageAsync(message.Chat.Id, $"{usr.name} Вы купили кирку за 300.000$!\n теперь вы будете добывать больше камня");
                        }
                        else if (s[1].ToLower().Contains("1") && 300000 > usr.money)
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, $"{usr.name} Вам не хватает денег на покупку кирки🚫🚫🚫");
                        }
                        else if (s[1].ToLower().Contains("2") && 500000 <= usr.money)
                        {
                            usr.soldiers += 1;
                            await botClient.SendTextMessageAsync(message.Chat.Id, $"{usr.name} Вы наняли солдата теперь численность вашей армии {usr.soldiers}");
                            usr.money -= 500000;
                        }
                        else if (s[1].ToLower().Contains("2") && 500000 > usr.money)
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, $"{usr.name} Вам не хватает денег на наемника🚫🚫🚫");
                        }
                        else if (s[1].ToLower().Contains("3") && 1000000 <= usr.money)
                        {
                            usr.rating += 1;
                            await botClient.SendTextMessageAsync(message.Chat.Id, $"{usr.name} Вы купили рейтинг\n ваш текущий рейтинг: 🏆{usr.rating}");
                            usr.money -= 1000000;
                        }
                        else if (s[1].ToLower().Contains("3") && 1000000 > usr.money)
                        {

                            await botClient.SendTextMessageAsync(message.Chat.Id, $"{usr.name} Тебе не хватает денег на покупку рейтинга🚫🚫🚫");
                        }

                    }

                }
                if ((message.Text.ToLower().Contains("/bonus") || message.Text.ToLower().Contains("/бонус")) && Game.keyValuePairs.ContainsKey(message.From.ToString()))
                {
                    User usr = new User();
                    if (Game.keyValuePairs.TryGetValue(message.From.ToString(), out usr))
                    {
                        usr.money = 100000000;
                    }
                }
                if (message.Text.ToLower() == "/war")
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Правила ведения войны💥:\n1) Нападающий должен иметь войско минимум в 5 человек❗️\n2) Нападаемый должен иметь капитал минимум в 1.000.000$❗️\n3) Нападать можно только в общем чате,\nВ случае нарушения правила №3 вы получите штраф❗️\n\n Для нападения введите /war [ID] \nID вы можете узнать в /users");

                }
                if (message.Text.ToLower().Contains("/war ") && message.Text.ToLower() != "/war" && message.Chat.Id == -4090139549 && Game.keyValuePairs.ContainsKey(message.From.ToString()))
                {
                    string[] text1 = message.Text.ToLower().Split(" ");
                    bool b1 = Int32.TryParse(text1[1], out int val);
                    User user2 = Game.keyValuePairs.Where(z => z.Value.userId == val).FirstOrDefault().Value;
                    if (b1)
                    {
                        User usr = new User();
                        User usr2 = new User();

                        if (Game.keyValuePairs.TryGetValue(message.From.ToString(), out usr))
                        {
                            if (Game.keyValuePairs.TryGetValue(user2.name, out usr2))
                            {
                                if (usr2.isOnWar) { await botClient.SendTextMessageAsync(message.Chat.Id, $"Игрок уже в состоянии войны!"); return; }
                                else
                                {
                                    if (user2.userId == val && usr.soldiers >= 5 && usr2.money >= 1000000)
                                    {
                                        await botClient.SendTextMessageAsync(message.Chat.Id, $"{usr.name} и его {usr.soldiers} солдат напали на игрока {user2.name}. {user2.soldiers} его солдат идут в ответ\n Результаты столкновения через 30 секунд!");
                                        usr.isOnWar = true;
                                        usr2.isOnWar = true;

                                        aTimer = new System.Timers.Timer(30000);
                                        // Hook up the Elapsed event for the timer. 
                                        aTimer.Elapsed += OnTimedEvent;

                                        aTimer.Enabled = true;

                                        async void OnTimedEvent(object source, ElapsedEventArgs e)
                                        {
                                            //код, который должен срабатывать раз в 30 сек.


                                            if (usr2.soldiers == 0)
                                            {
                                                await botClient.SendTextMessageAsync(message.Chat.Id, $"У {usr2.name} не было военных на базе, и {usr.name} забрал половину его денег(");
                                                usr.isOnWar = false;
                                                usr2.isOnWar = false;
                                                usr.money += (usr2.money / 2);
                                                usr2.money /= 2;
                                                aTimer.Stop();


                                            }
                                            else if (usr.soldiers < usr2.soldiers)
                                            {
                                                usr.isOnWar = false;
                                                usr2.isOnWar = false;
                                                await botClient.SendTextMessageAsync(message.Chat.Id, $"{usr2.name} успешно отстоял свою территорию!\n армия {usr.name} разгромлена\n {usr2.name} сохранил {usr2.soldiers - usr.soldiers} солдат");
                                                usr2.soldiers -= usr.soldiers;
                                                usr.soldiers = 0;
                                                usr2.money += (usr.money / 2);
                                                usr.money /= 2;
                                                aTimer.Stop();



                                            }
                                            else if (usr.soldiers > usr2.soldiers)
                                            {
                                                usr.isOnWar = false;
                                                usr2.isOnWar = false;
                                                await botClient.SendTextMessageAsync(message.Chat.Id, $"{usr.name} успешно выполнил наступление!\n армия {usr2.name} разгромлена\n {usr.name} потерял {(usr2.soldiers + usr.soldiers) - usr.soldiers} солдат");
                                                usr.soldiers -= usr2.soldiers;
                                                usr2.soldiers = 0;
                                                usr.money += (usr2.money / 2);
                                                usr2.money /= 2;
                                                aTimer.Stop();


                                            }
                                            else if (usr.soldiers == usr2.soldiers)
                                            {
                                                usr.isOnWar = false;
                                                usr2.isOnWar = false;
                                                await botClient.SendTextMessageAsync(message.Chat.Id, $"Война не принесла результата, погибли люди, но никто не выйграл из-за равенства сил(");
                                                usr.soldiers = 0;
                                                usr2.soldiers = 0;
                                                aTimer.Stop();



                                            }
                                            return;
                                        }

                                    }
                                    else if (user2.userId == val && usr.soldiers < 5 && usr2.money < 1000000)
                                    {
                                        await botClient.SendTextMessageAsync(message.Chat.Id, $"{usr.name} Вы не можете напасть на игрока {user2.name},возможно у вас не хватает армии или у игрока слишком мало денег\n /war - правила ведения войны!");
                                    }

                                    return;
                                }
                            }


                        }


                    }
                }

            }






            if (update?.CallbackQuery != null && update?.CallbackQuery.Message.From != null)
            {
                if (update.CallbackQuery.Data == "Африка" && !Game.isStarted)
                {

                    Game.StartGame(gameName, update.CallbackQuery.Data);

                    InlineKeyboardMarkup keyboard2 = new(new[]
                        {
                        InlineKeyboardButton.WithCallbackData("Присоединиться")

                    });


                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"Началась игра на карте {Game.mapName} \n Вы можете присоединиться:", replyMarkup: keyboard2);

                    return;
                }


                else if (update.CallbackQuery.Data == "Европа" && !Game.isStarted)
                {

                    InlineKeyboardMarkup keyboard2 = new(new[]
                        {
                        InlineKeyboardButton.WithCallbackData("Присоединиться")

                    });
                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"Началась игра на карте {Game.mapName} \n Вы можете присоединиться:", replyMarkup: keyboard2);

                    return;
                }


            }
            if (update?.CallbackQuery != null)
            {
                if (update.CallbackQuery.Data == "Присоединиться" && !Game.keyValuePairs.ContainsKey(update.CallbackQuery.From.ToString()) && Game.isStarted)
                {

                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"Генерал {update.CallbackQuery.From} присоединился к игре");
                    Game.keyValuePairs.Add(update.CallbackQuery.From.ToString(), new User(update.CallbackQuery.From.ToString()));
                    number13 += 1;
                    await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, $"{update.CallbackQuery.From} Вы присоединились к игре!");
                    return;

                }
                else if (update.CallbackQuery.Data == "Присоединиться" && (Game.keyValuePairs.ContainsKey(update.CallbackQuery.From.ToString()) || !Game.isStarted))
                {

                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"{update.CallbackQuery.From} Игра началась, либо вы уже зарегистрированы!");
                    update.CallbackQuery = null;
                    return;
                }
            }
        }

        private static Task Error(ITelegramBotClient arg1, Exception exception, CancellationToken arg3)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}