using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace NotesApiWithUI.Services
{
    public class TelegramService
    {
        private readonly string _botToken;
        private readonly string _chatId;
        private readonly TelegramBotClient _botClient;

        public TelegramService(IConfiguration configuration)
        {
            _botToken = configuration["Telegram:BotToken"] ?? throw new ArgumentNullException("BotToken не указан");
            _chatId = configuration["Telegram:ChatId"] ?? throw new ArgumentNullException("ChatId не указан");
            _botClient = new TelegramBotClient(_botToken);
        }

        public async Task SendMessageAsync(string message)
        {
            try
            {
                await _botClient.SendTextMessageAsync(_chatId, message);
            }
            catch (ApiRequestException ex)
            {
                // Логируйте ошибки
                Console.WriteLine($"Telegram API Error: {ex.Message}");
            }
        }
    }
}