using Azure;
using System.Text.Json.Serialization;

namespace QuizEngineBE.DTO
{
    public class UserResponse : ResponseBase<UserResponse>
    {
        public int? Id { get; set; }

        public string? Token { get; set; } 


        [JsonIgnore]
        new public UserResponse WrongFields => new()
        {
            Success = false,
            Message = "username o Password sbagliate"


        };
        [JsonIgnore]
        public UserResponse ExistingUser => new()
        {
            Success = false,
            Message = "Username già esistente"

        };


    }
}
