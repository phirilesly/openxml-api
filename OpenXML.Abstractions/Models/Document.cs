using Microsoft.AspNetCore.Http;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;


namespace OpenXML.Abstractions.Models
{
    [DataContract]
    public class UploadedDocument : ICommandData, IEventData
    {
        public Guid? FolderId { get; set; }
        [DataMember(Name = "fileName")]
        public string FileName { get; set; }
        [DataMember(Name = "blobUrl")]
        public string BlobUrl { get; set; }
        [DataMember(Name = "entityName")]
        public string EntityName { get; set; }
        [DataMember(Name = "entityId")]
        public Guid EntityId { get; set; }
        [DataMember(Name = "classId")]
        public Guid ClassId { get; set; }

        [DataMember(Name = "classCode")]
        public string ClassCode { get; set; }

        [JsonIgnore]
        public IFormFile File { get; set; }

        [DataMember(Name = "fileType")]
        public string FileType { get; set; }
    }
}
