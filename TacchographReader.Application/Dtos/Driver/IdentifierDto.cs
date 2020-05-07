using System;
using Newtonsoft.Json;

namespace TachographReader.Application.Dtos.Driver
{
    public class IdentifierDto
    {
        [JsonProperty(PropertyName = "cardIssueDate")]
        public DateTime CardIssueDate { get; set; }
        [JsonProperty(PropertyName = "cardNumber")]
        public string CardNumber { get; set; }
        [JsonProperty(PropertyName = "cardExpiryDate")]
        public DateTime CardExpiryDate { get; set; }
    }
}
