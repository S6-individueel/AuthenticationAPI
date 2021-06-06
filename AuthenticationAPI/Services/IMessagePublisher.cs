using AuthenticationAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationAPI.Services
{
    public interface IMessagePublisher
    {
        public Task Publish<T>(T obj, string topicName);
    }
}
