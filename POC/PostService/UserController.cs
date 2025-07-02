using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace PostService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] User user)
        {
            var factory = new ConnectionFactory() { 
                HostName = "localhost",
                UserName = "user",
                Password = "password"
                };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "user_queue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var jsonString = JsonSerializer.Serialize(user);
                var body = Encoding.UTF8.GetBytes(jsonString);

                channel.BasicPublish(exchange: "",
                                     routingKey: "user_queue",
                                     basicProperties: null,
                                     body: body);
            }

            return Ok();
        }
    }
}