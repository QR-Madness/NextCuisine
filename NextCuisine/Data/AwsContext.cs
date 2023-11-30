using System.Collections.ObjectModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using NextCuisine.Models;
using NextCuisine.Tools.ServiceTools;
using NextCuisine.Tools;

namespace NextCuisine.Data
{
    public class AwsContext
    {
        private readonly AwsDynamoWithS3 _aws = new();
        public AwsDynamoWithS3 Aws => _aws;

        /// <summary>
        /// Convert's a guest's upload 
        /// </summary>
        /// <param name="upload">Populated upload</param>
        /// <returns>Document for DynamoDB storage</returns>
        private Document ConvertUploadToDocument(GuestUpload upload)
        {
            List<Document> uploadFilesDocumentList = new List<Document>();
            foreach (GuestUploadFile file in upload.Files)
            {
                uploadFilesDocumentList.Add(new Document()
                {
                    ["Id"] = file.Id,
                    ["Filename"] = file.Filename,
                    ["FilenameS3"] = file.FilenameS3,
                    ["UploadDateTime"] = file.UploadDateTime
                });
            }
            List<Document> additionalContentDocumentList = new List<Document>();
            return new Document()
            {
                ["id"] = upload.Id,
                ["OwnerUid"] = upload.OwnerUid,
                ["Visibility"] = upload.Visibility,
                ["LastEditTime"] = upload.LastEditTime,
                ["UploadDate"] = upload.UploadDate,
                ["Title"] = upload.Title,
                ["ShortDescription"] = upload.ShortDescription,
                ["Content"] = upload.Content,
                ["Files"] = uploadFilesDocumentList,
                ["AdditionalContent"] = additionalContentDocumentList
            };
            foreach (KeyValuePair<string, string> textContentItem in upload.AdditionalContent)
            {
                uploadFilesDocumentList.Add(new Document()
                {
                    [textContentItem.Key] = textContentItem.Value
                });
            }
        }

        private static Collection<GuestUploadFile> ConvertAttributeValuesToGuestUploadFiles(List<AttributeValue> files)
        {
            return files.Select(fileAttribute => new GuestUploadFile(
            {
                Id = DataTools.GetValueOrDefault(fileAttribute.M, "Id"),
                Filename = DataTools.GetValueOrDefault(fileAttribute.M, "Filename"),
                UploadDateTime = DataTools.GetDateTimeValueOrDefault(fileAttribute.M, "UploadDateTime")
            });
        }

        public static GuestUpload ConvertAttributeValuesToGuestUpload(Dictionary<string, AttributeValue> attributeValues)
        {
            return new GuestUpload
            {
                Id = DataTools.GetValueOrDefault(attributeValues, "Id"),
                OwnerUid = DataTools.GetValueOrDefault(attributeValues, "OwnerUid"),
                Visibility = DataTools.GetValueOrDefault(attributeValues, "Visibility"),
                LastEditTime = DataTools.GetDateTimeValueOrDefault(attributeValues, "LastEditTime"),
                UploadDate = DataTools.GetDateTimeValueOrDefault(attributeValues, "UploadDate"),
                Title = DataTools.GetValueOrDefault(attributeValues, "Title"),
                ShortDescription = DataTools.GetValueOrDefault(attributeValues, "ShortDescription"),
                Content = DataTools.GetValueOrDefault(attributeValues, "Content"),
                Files = ConvertAttributeValuesToGuestUploadFiles(attributeValues["Files"].L),
                AdditionalContent = DataTools.ConvertAttributeValuesToDictionary(attributeValues["AdditionalContent"].M)
            };
        }

        public static GuestProfile ConvertAttributeValuesToGuestProfile(Dictionary<string, AttributeValue> attributeValues)
        {
            return new GuestProfile
            {
                Uid = DataTools.GetValueOrDefault(attributeValues, "Uid"),
                Name = DataTools.GetValueOrDefault(attributeValues, "Name"),
                Bio = DataTools.GetValueOrDefault(attributeValues, "Bio"),
                AdditionalContent = DataTools.ConvertAttributeValuesToDictionary(attributeValues["AdditionalContent"].M)
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        private Document ConvertProfileToDocument(GuestProfile profile)
        {
            List<Document> additionalContentDocument = new List<Document>();
            foreach (KeyValuePair<string, string> contentItem in profile.AdditionalContent)
            {
                additionalContentDocument.Add(new Document()
                {
                    [contentItem.Key] = contentItem.Value
                });
            }
            return new Document()
            {
                ["Uid"] = profile.Uid,
                ["Name"] = profile.Name,
                ["Bio"] = profile.Bio,
                ["AdditionalContent"] = additionalContentDocument
            };
        }

        public Task<PutItemResponse> CreateUpload(GuestUpload newUpload)
        {
            return _aws.Db.PutItemAsync(new PutItemRequest()
            {
                TableName = "NextCuisine",
                Item = ConvertUploadToDocument(newUpload).ToAttributeMap()
            });
        }

        public Task<PutItemResponse> CreateProfile(GuestProfile guestProfile)
        {
            return _aws.Db.PutItemAsync(new PutItemRequest()
            {
                TableName = "NextCuisineProfiles",
                Item = ConvertProfileToDocument(guestProfile).ToAttributeMap()
            });
        }

        public Task<UpdateItemResponse> EditProfile(GuestProfile guestProfile)
        {
            return _aws.Db.UpdateItemAsync(new UpdateItemRequest()
            {
                TableName = "NextCuisineProfiles",
                AttributeUpdates = ConvertProfileToDocument(guestProfile).ToAttributeUpdateMap(false)
            });
        }

        public GuestUpload? GetProfile(string uid)
        {
            var scanFilter = new ScanFilter();
            scanFilter.AddCondition("Uid", ScanOperator.Equal, uid);
            var search = _aws.Db.ScanAsync(new ScanRequest()
            {
                ScanFilter = scanFilter.ToConditions(),
                TableName = "NextCuisineProfiles"
            });
            var uploadsAttributes = search.Result.Items;
            var upload = uploadsAttributes.FirstOrDefault();
            return upload == null ? null : ConvertAttributeValuesToGuestUpload(upload);
        }

        public GuestUpload? GetUpload(string id)
        {
            var scanFilter = new ScanFilter();
            scanFilter.AddCondition("id", ScanOperator.Equal, id);
            var search = _aws.Db.ScanAsync(new ScanRequest()
            {
                ScanFilter = scanFilter.ToConditions(),
                TableName = "NextCuisine"
            });
            var uploadsAttributes = search.Result.Items;
            var upload = uploadsAttributes.FirstOrDefault();
            return upload == null ? null : ConvertAttributeValuesToGuestUpload(upload);
        }

        public Task<UpdateItemResponse> EditUpload(GuestUpload modifiedUpload)
        {
            return _aws.Db.UpdateItemAsync(new UpdateItemRequest
            {
                TableName = "NextCuisine",
                Key = new Dictionary<string, AttributeValue>()
                {
                    ["Id"] = new AttributeValue() { S = modifiedUpload.Id }
                },
                AttributeUpdates = ConvertUploadToDocument(modifiedUpload).ToAttributeUpdateMap(false)
            });
        }

        public List<GuestUpload?> GetPublicUploads()
        {
            var scanFilter = new ScanFilter();
            scanFilter.AddCondition("Visibility", ScanOperator.Equal, "Public");
            var search = _aws.Db.ScanAsync(new ScanRequest()
            {
                ScanFilter = scanFilter.ToConditions(),
                TableName = "NextCuisine"
            });
            var uploadsAttributes = search.Result.Items;
            return uploadsAttributes.Select(ConvertAttributeValuesToGuestUpload).ToList();
        }

        public List<GuestUpload?> GetPrivateUploads(string uid)
        {
            var scanFilter = new ScanFilter();
            scanFilter.AddCondition("Visibility", ScanOperator.Equal, "Private");
            scanFilter.AddCondition("OwnerUid", ScanOperator.Equal, uid);
            var search = _aws.Db.ScanAsync(new ScanRequest()
            {
                ScanFilter = scanFilter.ToConditions(),
                TableName = "NextCuisine"
            });
            var uploadsAttributes = search.Result.Items;
            return uploadsAttributes.Select(ConvertAttributeValuesToGuestUpload).ToList();
        }
    }
}
