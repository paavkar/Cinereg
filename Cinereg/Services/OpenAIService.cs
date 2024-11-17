using Azure.AI.OpenAI;
using Cinereg.Models;
using OpenAI.Chat;
using System.ClientModel;

namespace Cinereg.Services
{
    public class OpenAIService : IOpenAIService
    {
        private OpenAIChat _openAIChat;
        private AzureOpenAIClient _openAIClient;
        private ChatClient _chatClient;

        public OpenAIService(IConfiguration configuration)
        {
            _openAIChat = configuration.GetSection("OpenAI").Get<OpenAIChat>()!;
            _openAIClient = new(
                new Uri(_openAIChat.Endpoint),
                new ApiKeyCredential(_openAIChat.ApiKey));
            _chatClient = _openAIClient.GetChatClient(_openAIChat.Model);
        }

        public ChatCompletion GetChatCompletion(List<ChatMessage> chatHistory)
        {
            ChatCompletion chatCompletion = _chatClient.CompleteChat(chatHistory);
            return chatCompletion;
        }
    }
}
