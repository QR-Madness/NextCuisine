using System.Collections.ObjectModel;
using Amazon.DynamoDBv2;
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

        private static List<GuestUploadFile> ConvertAttributeValuesToGuestUploadFiles(List<AttributeValue> files)
        {
            return files.Select(fileAttribute => new GuestUploadFile
            {
                Id = DataTools.GetValueOrDefault(fileAttribute.M, "Id"),
                Filename = DataTools.GetValueOrDefault(fileAttribute.M, "Filename"),
                UploadDateTime = DataTools.GetDateTimeValueOrDefault(fileAttribute.M, "UploadDateTime")
            }).ToList();
        }

        public static GuestUpload ConvertAttributeValuesToGuestUpload(Dictionary<string, AttributeValue> attributeValues)
        {
            return new GuestUpload
            {
                Id = DataTools.GetValueOrDefault(attributeValues, "id"),
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

        /// <summary>
        /// Parse/convert DB attribute values
        /// </summary>
        /// <param name="attributeValues"></param>
        /// <returns>Populated Model Object</returns>
        public static GuestProfile ConvertAttributeValuesToGuestProfile(Dictionary<string, AttributeValue> attributeValues)
        {
            return new GuestProfile
            {
                Uid = DataTools.GetValueOrDefault(attributeValues, "Uid"),
                Name = DataTools.GetValueOrDefault(attributeValues, "Name"),
                Bio = DataTools.GetValueOrDefault(attributeValues, "Bio"),
                FavoriteRecipes = DataTools.GetValueOrDefault(attributeValues, "FavoriteRecipes"),
                FavoriteSnacks = DataTools.GetValueOrDefault(attributeValues, "FavoriteSnacks"),
                GoodCombos = DataTools.GetValueOrDefault(attributeValues, "GoodCombos")
                //AdditionalContent = DataTools.ConvertAttributeValuesToDictionary(attributeValues["AdditionalContent"].M)
            };
        }

        /// <summary>
        /// Convert a profile model object 
        /// </summary>
        /// <param name="profile"></param>
        /// <returns>Document for AWS operations</returns>
        private Document ConvertProfileToDocument(GuestProfile profile)
        {
            List<Document> additionalContentDocument = new List<Document>();
            // ARCHIVED CODE
            //foreach (KeyValuePair<string, string> contentItem in profile.AdditionalContent)
            //{
            //    additionalContentDocument.Add(new Document()
            //    {
            //        [contentItem.Key] = contentItem.Value
            //    });
            //}
            // END OF ARCHIVED CODE
            return new Document()
            {
                //["Uid"] = profile.Uid,
                ["Name"] = profile.Name,
                ["Bio"] = profile.Bio,
                ["FavoriteSnacks"] = profile.FavoriteSnacks,
                ["FavoriteRecipes"] = profile.FavoriteRecipes,
                ["GoodCombos"] = profile.GoodCombos
                //["AdditionalContent"] = additionalContentDocument
            };
        }

        public async Task<List<GuestUpload>> GetUploads()
        {
            var uploads = await _aws.Db.ScanAsync(new ScanRequest
            {
                TableName = "NextCuisine"
            });
            return uploads.Items.Select(ConvertAttributeValuesToGuestUpload).ToList();
        }
        public async Task<List<GuestUpload>> GetGuestUploads(string uid)
        {
            var scanFilter = new ScanFilter();
            scanFilter.AddCondition("OwnerUid", ScanOperator.Equal, uid);
            var uploads = await _aws.Db.ScanAsync(new ScanRequest
            {
                TableName = "NextCuisine",
                ScanFilter = scanFilter.ToConditions()
            });
            return uploads.Items.Select(ConvertAttributeValuesToGuestUpload).ToList();
        }

        public async Task<List<GuestUpload>> GetPublicUploads()
        {
            var scanFilter = new ScanFilter();
            scanFilter.AddCondition("Visibility", ScanOperator.Equal, "Public");
            var search = await _aws.Db.ScanAsync(new ScanRequest()
            {
                ScanFilter = scanFilter.ToConditions(),
                TableName = "NextCuisine"
            });
            return search.Items.Select(ConvertAttributeValuesToGuestUpload).ToList();
        }

        public async Task<List<GuestUpload>> GetPrivateUploads(string uid)
        {
            var scanFilter = new ScanFilter();
            scanFilter.AddCondition("Visibility", ScanOperator.Equal, "Private");
            scanFilter.AddCondition("OwnerUid", ScanOperator.Equal, uid);
            var search = await _aws.Db.ScanAsync(new ScanRequest()
            {
                ScanFilter = scanFilter.ToConditions(),
                TableName = "NextCuisine"
            });
            var uploadsAttributes = search.Items;
            return uploadsAttributes.Select(ConvertAttributeValuesToGuestUpload).ToList();
        }

        public Task<PutItemResponse> CreateUpload(GuestUpload newUpload)
        {
            return _aws.Db.PutItemAsync(new PutItemRequest()
            {
                TableName = "NextCuisine",
                Item = ConvertUploadToDocument(newUpload).ToAttributeMap()
            });
        }

        public async Task<PutItemResponse> CreateProfile(GuestProfile guestProfile)
        {
            return await _aws.Db.PutItemAsync(new PutItemRequest()
            {
                TableName = "NextCuisineProfiles",
                Item = ConvertProfileToDocument(guestProfile).ToAttributeMap()
            });
        }

        public async Task<UpdateItemResponse> EditProfile(string uid, GuestProfile guestProfile)
        {
            return await _aws.Db.UpdateItemAsync(new UpdateItemRequest()
            {
                Key = new Dictionary<string, AttributeValue>()
                {
                    ["Uid"] = new AttributeValue(uid)
                },
                TableName = "NextCuisineProfiles",
                AttributeUpdates = ConvertProfileToDocument(guestProfile).ToAttributeUpdateMap(true)
            });
        }

        public async Task<GuestProfile?> GetProfile(string uid)
        {
            var scanFilter = new ScanFilter();
            scanFilter.AddCondition("Uid", ScanOperator.Equal, uid);
            var search = await _aws.Db.ScanAsync(new ScanRequest()
            {
                ScanFilter = scanFilter.ToConditions(),
                TableName = "NextCuisineProfiles"
            });
            var uploadsAttributes = search.Items;
            var upload = uploadsAttributes.FirstOrDefault();
            return upload == null ? null : ConvertAttributeValuesToGuestProfile(upload);
        }

        public async Task<GuestUpload?> GetUpload(string id)
        {
            var scanFilter = new ScanFilter();
            scanFilter.AddCondition("id", ScanOperator.Equal, id);
            var search = await _aws.Db.ScanAsync(new ScanRequest()
            {
                ScanFilter = scanFilter.ToConditions(),
                TableName = "NextCuisine"
            });
            var upload = search.Items.FirstOrDefault();
            return upload == null ? null : ConvertAttributeValuesToGuestUpload(upload);
        }

        public Task<UpdateItemResponse> EditUpload(GuestUpload modifiedUpload)
        {
            return _aws.Db.UpdateItemAsync(new UpdateItemRequest
            {
                TableName = "NextCuisine",
                Key = new Dictionary<string, AttributeValue>()
                {
                    ["id"] = new AttributeValue() { S = modifiedUpload.Id }
                },
                AttributeUpdates = ConvertUploadToDocument(modifiedUpload).ToAttributeUpdateMap(false)
            });
        }

        public Task<DeleteItemResponse> DeleteUpload(string id)
        {
            return _aws.Db.DeleteItemAsync(new DeleteItemRequest()
            {
                TableName = "NextCuisine",
                Key = new Dictionary<string, AttributeValue>() { { "id", new AttributeValue() { S = id } } }
            });
        }
    }
}
