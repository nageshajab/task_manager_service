using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models;

namespace TaskManager.Models
{
    public class File
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("userid")]
        public string UserId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("tags")]
        public string[] Tags { get; set; }

        [BsonElement("parentfolder")]
        public string ParentFolder { get; set; }

        [BsonElement("googledrivepath")]
        public string GoogleDrivePath { get; set; }

        [BsonElement("azurepath")]
        public string AzurePath { get; set; }
    }

    public class File1
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string[] Tags { get; set; }
        public string ParentFolder { get; set; }
        public string GoogleDrivePath { get; set; }
        public string AzurePath { get; set; }   
    }

    public class FileSearch
    {
        public string UserId { get; set; } = string.Empty;
        public string SortBy { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int TotalRecords { get; set; } = 0;
    }

    public class FileIndexViewModel
    {
        public List<File> ListOfFiles { get; set; }
        public FileSearch FileSearch{ get; set; }
        public int TotalPages { get; set; }
    }
}
