using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Data.SQLite;
using System.Text;
using System.Text.Json;
using QueueConsumer;

var factory = new ConnectionFactory() { HostName = "localhost", UserName = "user", Password = "password" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.QueueDeclare(queue: "user_queue",
                         durable: true,
                         exclusive: false,
                         autoDelete: false,
                         arguments: null);

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var user = JsonSerializer.Deserialize<User>(message);

        var age = DateTime.Today.Year - user.Birth.Year;
        if (user.Birth.Date > DateTime.Today.AddYears(-age)) age--;

        using (var dbConnection = new SQLiteConnection(@"Data Source=C:\AutoLabGithub\POC_Ocelot\poc.db"))
        {
            dbConnection.Open();
            var command = dbConnection.CreateCommand();
            command.CommandText = "INSERT INTO Users (IDENTIFIER, NAME, AGE) VALUES (@Id, @Name, @Age)";
            command.Parameters.AddWithValue("@Id", user.Id);
            command.Parameters.AddWithValue("@Name", user.Name);
            command.Parameters.AddWithValue("@Age", age);
            command.ExecuteNonQuery();
        }

        Console.WriteLine($" [x] Received and processed {message}");
    };
    channel.BasicConsume(queue: "user_queue",
                         autoAck: true,
                         consumer: consumer);

    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();
}