using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static ITopicClient topicClient;
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }
        static async Task MainAsync()
        {
            string ServiceBusConnectionString = "Endpoint=sb://publish-sub.servicebus.windows.net/;SharedAccessKeyName=user-send;SharedAccessKey=G0yPfFo3qTNPyCxxZUxSOwtPejlcCbL1DOqox4nqbYE=";
            string TopicName = "topic1";

            topicClient = new TopicClient(ServiceBusConnectionString, TopicName);

            // Send messages.
            await SendUserMessage();

            Console.ReadKey();

            await topicClient.CloseAsync();
        }
        static async Task SendUserMessage()
        {
            List<User> users = GetDummyDataForUser();

            var serializeUser = JsonConvert.SerializeObject(users);

            string messageType = "userData";

            string messageId = Guid.NewGuid().ToString();

            var message = new ServiceBusMessage
            {
                Id = messageId,
                Type = messageType,
                Content = serializeUser
        };
            var serializeBody = JsonConvert.SerializeObject(message);

            var busMessage = new Message(Encoding.UTF8.GetBytes(serializeBody));
            busMessage.UserProperties.Add("Type", messageType);
            busMessage.MessageId = messageId;

            await topicClient.SendAsync(busMessage);

            Console.WriteLine("Message has been send");
        }

        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }          
        }

        public class ServiceBusMessage
        {
            public string Id { get; set; }
            public string Type { get; set; }
            public string Content { get; set; }
        }
        private static List<User> GetDummyDataForUser()
        {
            User user = new User();
            List<User> lstUser = new List<User>();

            for(int i = 1; i < 3; ++i)
            {
                user = new User();
                user.Id = i;
                user.Name = "sdfgsdfg" + 1;

                lstUser.Add(user);
            }

            return lstUser;

        }

    }
}
