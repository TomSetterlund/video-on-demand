using Amazon;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace Handlers
{
    public class PresignedPostData
    {
        private readonly string _bucketName;
        private readonly RegionEndpoint _bucketRegion;
        private readonly IAmazonS3 _s3Client;
        public PresignedPostData()
        {
            if (String.IsNullOrEmpty(Environment.GetEnvironmentVariable("S3_PREPROCESSED_NAME")))
            {
                throw new Exception("Your Preprocessd bucket name doesn't exist as an environment variable!");
            }
            _bucketName = Environment.GetEnvironmentVariable("S3_PREPROCESSED_NAME");
            _bucketRegion = RegionEndpoint.USWest2;
            _s3Client = new AmazonS3Client(_bucketRegion);
        }

        public APIGatewayProxyResponse Get(APIGatewayProxyRequest request)
        {
            try
            {
                var body = JsonConvert.DeserializeObject<Request>(request.Body);
                var presignedUrlRequest = new GetPreSignedUrlRequest {
                    BucketName = _bucketName,
                    Key = body.ChallengeName,
                    ContentType = "image/*",
                    Expires = DateTime.Now.AddMinutes(3)
                };

                var response = _s3Client.GetPreSignedURL(presignedUrlRequest);
                var respHeaders = new Dictionary<string, string> {
                    { "Access-Control-Allow-Origin", "*" },
                    { "Access-Control-Allow-Credentials", "true" }
                };

                return new APIGatewayProxyResponse { StatusCode = 200, Body = response, Headers = respHeaders };
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

        public Request(string userId, string challengeName)
        {
            UserId = userId;
            ChallengeName = challengeName;
        }
    }
}
