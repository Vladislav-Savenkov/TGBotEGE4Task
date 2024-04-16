using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotEGERus4;
using static TelegramBotEGERus4.words;

namespace TelegramBotEGERus4
{
    public class TaskManager
    {
        public async void OnMessage(ITelegramBotClient botClient, MessageEventArgs e)
        {
            User user = (new DataManager()).FindUserAsync(e.Message.From.Id).Result;
            
            if (user.UserTaskId == -1)
            {
                await SendTask(botClient, e, user);
            }
            else
            {
                await CheckAnswer(botClient, e, user);
                await SendTask(botClient, e, user);
            }
        }

        public async Task SendTask(ITelegramBotClient botClient, MessageEventArgs e, User user)
        {
            int newTaskId = (new Random()).Next(0, words.w.Length - 1);
            if (newTaskId == user.UserTaskId)
            {
                newTaskId = (new Random()).Next(0, words.w.Length - 1);
            }
            user.UserTaskId = newTaskId;

            await (new DataManager()).UpdateUserTaskAsync(user.UserId, user.UserTaskId);

            Message message = await botClient.SendTextMessageAsync(
                chatId: user.UserId,
                text: $"Укажите букву, на которую падает ударение в данном слове:\n\n{words.w[user.UserTaskId].ToLower()}\n\nПример:\nСлово: \"свекла\"\nОтвет: \"свЕкла\"",
                parseMode: ParseMode.Html,
                disableNotification: true);
        }
        public async Task CheckAnswer(ITelegramBotClient botClient, MessageEventArgs e, User user)
        {
            if (e.Message.Text == words.w[user.UserTaskId])
            {
                Message message = await botClient.SendTextMessageAsync(
                    chatId: user.UserId,
                    text: "&#9989Верно!&#9989",
                    parseMode: ParseMode.Html,
                    disableNotification: true,
                    replyToMessageId: e.Message.MessageId);
            }
            else
            {
                Message message = await botClient.SendTextMessageAsync(
                    chatId: user.UserId,
                    text: $"&#10060Неверно!&#10060\nПравильный ответ: \"{words.w[user.UserTaskId]}\"",
                    parseMode: ParseMode.Html,
                    disableNotification: true,
                    replyToMessageId: e.Message.MessageId);
            }
        }
    }
}
