using AuthenticationAPI.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AuthenticationAPI.Services
{
    public class MessagePublisher : IMessagePublisher
    {
        private IConfiguration _config;

        public MessagePublisher(IConfiguration config)
        {
            _config = config;
        }

        public Task Publish<T>(T obj, string topicName)
        {
            var topicClient = new TopicClient(_config.GetConnectionString("AzureServiceBus"), topicName);
            String messageBody = JsonSerializer.Serialize(obj);
            var message = new Message(Encoding.UTF8.GetBytes(messageBody));
            message.UserProperties["messageType"] = typeof(T).Name;
            return topicClient.SendAsync(message);
        }
    }
}
