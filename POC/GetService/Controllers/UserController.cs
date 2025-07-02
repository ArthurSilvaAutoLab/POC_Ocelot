using Microsoft.AspNetCore.Mvc;
using System.Data.SQLite;
namespace GetService.Controllers{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase{
        [HttpGet("{id}")]
        public IActionResult GetUserById(int id) {
            using (var connection = new SQLiteConnection(@"Data Source=C:\AutoLabGithub\POC_Ocelot\poc.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT IDENTIFIER, NAME, AGE FROM USERS WHERE IDENTIFIER = @id";
                command.Parameters.AddWithValue("@id", id);
                using (var reader = command.ExecuteReader()){
                    if (reader.Read()){var user = new User{
                        Id = reader.GetInt32(0),Name = reader.GetString(1),Age = reader.GetInt32(2)
                    };
                        return Ok(user);
                    }
                }
            }
            return NotFound();
        }
    }
}