using System.Collections.Generic;

namespace Lab1WebApp.Models
{
    public class Response<T>
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
        public string ResponseText { get; set; }
        public T Metadata { get; set; }
    }
}
