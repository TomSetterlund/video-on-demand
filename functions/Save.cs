using Amazon;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Handlers
{
    public class Save
    {
        private readonly string _bucketName = Environment.GetEnvironmentVariable("S3_PREPROCESSED_NAME");
        private readonly RegionEndpoint _bucketRegion;
        private readonly IAmazonS3 _s3Client;
        public Save()
        {
            _bucketRegion = RegionEndpoint.USWest2;
            _s3Client = new AmazonS3Client(_bucketRegion);
        }

        public async Task<APIGatewayProxyResponse> Run(APIGatewayProxyRequest request)
        {
            try
            {
                var body = JsonConvert.DeserializeObject<Request>(request.Body);

                var putRequest = new PutObjectRequest 
                {
                    BucketName = _bucketName,
                    Key = body.ChallengeName,
                    ContentBody = body.EncodedImg
                };
                await _s3Client.PutObjectAsync(putRequest);
                return new APIGatewayProxyResponse { StatusCode = 200, Body = "Success!" };
            }
            catch (AmazonS3Exception e)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = 500,
                    Body = $"Error encountered on server. Message: '{e.Message}' when writing an object"
                };
            }
            catch (Exception e)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = 500,
                    Body = $"Error encountered on server. Message: '{e.Message}' when writing an object"
                };
            }
        }
    }

    public class Request
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        [JsonProperty("challenge_name")]
        public string ChallengeName { get; set; }
        [JsonProperty("encoded_image")]
        public string EncodedImg { get; set; }

        public Request(string userId, string encodedImage)
        {
            UserId = userId;
            EncodedImg = encodedImage;
        }
    }
}
