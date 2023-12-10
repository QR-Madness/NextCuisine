using Amazon;
using Amazon.DynamoDBv2;
using Amazon.S3;
using NextCuisine.Secrets;
using System.Net.Sockets;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace NextCuisine.Tools.ServiceTools
{
    public class AwsDynamoWithS3
    {
        public string UploadsBucketName = "nc-uploads-306";
        //public string StaticBucketName = "nc-static-306";
        private readonly RegionEndpoint _s3Region = RegionEndpoint.USEast1;
        public AmazonDynamoDBClient Db;
        public AmazonS3Client S3;

        public AwsDynamoWithS3()
        {
            Db = new AmazonDynamoDBClient(AwsParameters.Iam().Key, AwsParameters.Iam().Value, new AmazonDynamoDBConfig
            {
                RegionEndpoint = RegionEndpoint.USEast1
            });
            S3 = new AmazonS3Client(AwsParameters.Iam().Key, AwsParameters.Iam().Value, _s3Region);
        }
    }
}
