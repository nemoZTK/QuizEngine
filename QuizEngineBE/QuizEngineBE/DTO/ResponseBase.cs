using Azure;
using System.Text.Json.Serialization;

namespace QuizEngineBE.DTO
{
    /// <summary>
    /// Classe base delle risposte, estesa dalle varie risposte specifiche.
    /// Contiene:
    /// - un messaggio (Message)
    /// - un flag booleano (Success)
    /// - metodi di risposta standard: MissingFields, IdNotFound, WrongFields.
    /// </summary>
    /// <typeparam name="T">Tipo della classe derivata</typeparam>
    public abstract class ResponseBase<T> where T : ResponseBase<T>
    {
        public bool Success { get; set; } = false;
        public string? Message { get; set; }



        protected internal T MissingFields(string? field = null)
        {
            Success = false;
            Message = "Campi mancanti " + (field ?? "");
            return (T)this;
        }

        [JsonIgnore]
        protected internal T IdNotFound
        {
            get
            {
                Success = false;
                Message = "Id non trovato";
                return (T)this;
            }
        }

        protected internal T WrongFields(string? field = null)
        {
            Success = false;
            Message = "Campi errati " + (field ?? "");
            return (T)this;
        }
    }
}
