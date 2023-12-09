using System.Collections.ObjectModel;
using System.Diagnostics;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.S3;
using Amazon.S3.Model;
using NextCuisine.Models;
using NextCuisine.Tools.ServiceTools;
using NextCuisine.Tools;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Microsoft.AspNetCore.StaticFiles;

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
            // transfer upload files
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
            // transfer feedback 
            List<Document> uploadFeedbackList = new List<Document>();
            foreach (GuestUploadFeedback file in upload.Feedback)
            {
                uploadFeedbackList.Add(new Document()
                {
                    ["Id"] = file.Id,
                    ["OwnerUid"] = file.OwnerUid,
                    ["OwnerName"] = file.OwnerName,
                    ["Content"] = file.Content,
                    ["Rating"] = file.Rating.ToString(),
                    ["CreationTime"] = file.CreationTime,
                });
            }
            // transfer additional content list
            List<Document> additionalContentDocumentList = new List<Document>();
            foreach (KeyValuePair<string, string> textContentItem in upload.AdditionalContent)
            {
                uploadFilesDocumentList.Add(new Document()
                {
                    [textContentItem.Key] = textContentItem.Value
                });
            }
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
                ["Feedback"] = uploadFeedbackList,
                ["AdditionalContent"] = additionalContentDocumentList
            };
        }

        private static List<GuestUploadFile> ConvertAttributeValuesToGuestUploadFiles(List<AttributeValue> files)
        {
            return files.Select(fileAttribute => new GuestUploadFile
            {
                Id = DataTools.GetValueOrDefault(fileAttribute.M, "Id"),
                Filename = DataTools.GetValueOrDefault(fileAttribute.M, "Filename"),
                FilenameS3 = DataTools.GetValueOrDefault(fileAttribute.M, "FilenameS3"),
                UploadDateTime = DataTools.GetDateTimeValueOrDefault(fileAttribute.M, "UploadDateTime")
            }).ToList();
        }

        private static List<GuestUploadFeedback> ConvertAttributeValuesToGuestUploadFeedback(List<AttributeValue> feedback)
        {
            return feedback.Select(fileAttribute => new GuestUploadFeedback
            {
                Id = DataTools.GetValueOrDefault(fileAttribute.M, "Id"),
                OwnerUid = DataTools.GetValueOrDefault(fileAttribute.M, "OwnerUid"),
                OwnerName = DataTools.GetValueOrDefault(fileAttribute.M, "OwnerName"),
                Content = DataTools.GetValueOrDefault(fileAttribute.M, "Content"),
                Rating = DataTools.GetValueOrDefault(fileAttribute.M, "Rating"),
                CreationTime = DataTools.GetDateTimeValueOrDefault(fileAttribute.M, "CreationTime"),
            }).ToList();
        }

        public static GuestUpload ConvertAttributeValuesToGuestUpload(
                Dictionary<string, AttributeValue> attributeValues)
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
                Feedback = ConvertAttributeValuesToGuestUploadFeedback(attributeValues["Feedback"].L),
                AdditionalContent = DataTools.ConvertAttributeValuesToDictionary(attributeValues["AdditionalContent"].M)
            };
        }

        /// <summary>
        /// Parse/convert DB attribute values
        /// </summary>
        /// <param name="attributeValues"></param>
        /// <returns>Populated Model Object</returns>
        public static GuestProfile ConvertAttributeValuesToGuestProfile(
            Dictionary<string, AttributeValue> attributeValues)
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

        public async Task<PutObjectResponse> UploadGuestFile(GuestUploadFile guestUploadFile, Stream fileReadStream)
        {
            return await _aws.S3.PutObjectAsync(new PutObjectRequest()
            {
                BucketName = _aws.UploadsBucketName,
                InputStream = fileReadStream,
                Key = guestUploadFile.FilenameS3
            });
        }

        public async Task<IActionResult> GetGuestFile(GuestUploadFile guestUploadFile)
        {
            var transferTools = new TransferUtility(_aws.S3);
            var streamingRequest = new TransferUtilityOpenStreamRequest()
            {
                BucketName = _aws.UploadsBucketName,
                Key = guestUploadFile.FilenameS3,
            };
            using var guestFileStream =
                transferTools.OpenStreamAsync(_aws.UploadsBucketName, guestUploadFile.FilenameS3);
            var provider = new FileExtensionContentTypeProvider();
            var fileContentType = "application/octet-stream";
            if (provider.TryGetContentType(guestUploadFile.Filename, out string contentType))
            {
                fileContentType = contentType;
            }
            Debug.WriteLine(fileContentType);
            var contents = new byte[] { };
            await guestFileStream.Result.ReadAsync(contents);
            return new FileContentResult(contents, fileContentType);
        }

        public IActionResult DownloadGuestFile(GuestUploadFile guestUploadFile)
        {
            var transferTools = new TransferUtility(_aws.S3);
            var streamingRequest = new TransferUtilityOpenStreamRequest()
            {
                BucketName = _aws.UploadsBucketName,
                Key = guestUploadFile.FilenameS3,
            };
            using (var guestFileStream =
                   transferTools.OpenStreamAsync(_aws.UploadsBucketName, guestUploadFile.FilenameS3))
            {
                return new FileStreamResult(guestFileStream.Result, "application/octet-stream")
                {
                    FileDownloadName = guestUploadFile.Filename
                };
            }
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
            var attributes = ConvertUploadToDocument(modifiedUpload).ToAttributeUpdateMap(false);
            attributes.Remove("id");
            return _aws.Db.UpdateItemAsync(new UpdateItemRequest
            {
                TableName = "NextCuisine",
                Key = new Dictionary<string, AttributeValue>()
                {
                    ["id"] = new AttributeValue() { S = modifiedUpload.Id }
                },
                AttributeUpdates = attributes
            });
        }

        public async Task<bool> DeleteUpload(GuestUpload? upload)
        {
            try
            {
                await _aws.Db.DeleteItemAsync(new DeleteItemRequest()
                {
                    TableName = "NextCuisine",
                    Key = new Dictionary<string, AttributeValue>() { { "id", new AttributeValue() { S = upload.Id } } }
                });
                foreach (GuestUploadFile file in upload.Files)
                {
                    await _aws.S3.DeleteObjectAsync(new DeleteObjectRequest()
                    {
                        BucketName = _aws.UploadsBucketName,
                        Key = file.FilenameS3
                    });
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<GuestUpload?> GetUploadByFileId(string fileId)
        {
            try
            {
                GuestUpload? uploadMatch = null;
                // Retrieve the upload with the matching file ID
                // TODO Implement a query for better processing
                var uploads = await GetPublicUploads();
                uploads.ForEach(upload =>
                {
                    upload.Files.ForEach(file =>
                    {
                        if (file.Id == fileId)
                        {
                            uploadMatch = upload;
                        }
                    });
                });
                // return object
                return uploadMatch;
            }
            catch (Exception ex)
            {
                // issue during search
                Debug.WriteLine(ex);
                return null;
            }
        }

        public async Task<bool> DeleteUploadFile(string fileId)
        {
            try
            {
                // find upload
                var upload = await GetUploadByFileId(fileId);
                if (upload == null) throw new AggregateException("Cannot find candidate file deletion upload.");
                // remove from blob storage
                var s3Filename = upload.Files.First(uf => uf.Id == fileId).FilenameS3;
                Debug.WriteLine($"Deleting: {s3Filename}");
                await _aws.S3.DeleteObjectAsync(new DeleteObjectRequest
                {
                    BucketName = _aws.UploadsBucketName,
                    Key = s3Filename,
                });
                // remove from upload list
                upload.Files.Remove(upload.Files.Find(uf => uf.Id == fileId) ?? throw new InvalidOperationException());
                // commit changes
                await EditUpload(upload);
                return true;
            }
            catch (Exception e)
            {
                // failed file deletion
                Debug.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> PostUploadFeedback(string uploadId, GuestUploadFeedback newFeedback)
        {
            try
            {
                var modifiedUpload = await GetUpload(uploadId) ?? throw new InvalidOperationException();
                modifiedUpload.Feedback.Add(newFeedback);
                await EditUpload(modifiedUpload);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }
    }
}
