using Azure;
using System.Text.Json.Serialization;

namespace QuizEngineBE.DTO
{
    public class ResponseBase<T> where T : ResponseBase<T>
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        
        [JsonIgnore]
        public T MissingFields
        {
            get
            {
                Success = false;
                Message = "campi mancanti";
                return (T)this;
            }
        }
    }

}
