using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Handlers
{
    public class Save
    {
        public APIGatewayProxyResponse Run(APIGatewayProxyRequest request)
        {
            var body = JsonConvert.DeserializeObject<Request>(request.Body);
            var Response = new Response("Helllooooooo nuurse!", body);
            return new APIGatewayProxyResponse { StatusCode = 200, Body = JsonConvert.SerializeObject(Response) };
        }
    }

    public class Response
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("request")]
        public Request Request { get; set; }

        public Response(string message, Request request)
        {
            Message = message;
            Request = request;
        }
    }

    public class Request
    {
        [JsonProperty("key1")]
        public string Key1 { get; set; }
        [JsonProperty("key2")]
        public string Key2 { get; set; }
        [JsonProperty("key3")]
        public string Key3 { get; set; }

        public Request(string key1, string key2, string key3)
        {
            Key1 = key1;
            Key2 = key2;
            Key3 = key3;
        }
    }
}
