using OpenAI.Chat;

namespace Cinereg.Services
{
    public interface IOpenAIService
    {
        ChatCompletion GetChatCompletion(List<ChatMessage> chatHistory);
    }
}