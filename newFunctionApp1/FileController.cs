using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TaskManager.Models;
using System.Linq;
using System.Collections.Generic;
using DAL;

namespace FunctionApp1
{
    public class FileController
    {
        [FunctionName("taglist")]
        public async Task<IActionResult> TagList(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
           ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            FileSearch fileSearch = JsonConvert.DeserializeObject<FileSearch>(requestBody);

            IActionResult response = new UnauthorizedResult();

            List<TaskManager.Models.File> returnlist = new();
            List<string> tags = new();
            try
            {
                FileManager fileManager= new FileManager();
                tags = fileManager.GetTagsByUserId( fileSearch.UserId);                
            }
            catch (Exception)
            {
                throw;
            }

            return new OkObjectResult(tags);
        }

        [FunctionName("filelist")]
        public async Task<IActionResult> FileList(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            FileSearch fileSearch = JsonConvert.DeserializeObject<FileSearch>(requestBody);

            IActionResult response = new UnauthorizedResult();

            List<TaskManager.Models.File> lstFiles;
            List<TaskManager.Models.File> returnlist = new();
            List<string> tags = new();
            try
            {
                FileManager fileManager= new FileManager();
                lstFiles =fileManager.ListFilesByUserId(fileSearch);

                fileSearch.TotalRecords = returnlist.Count;

                //pagination at work
                returnlist = returnlist.Skip((fileSearch.PageNumber - 1) * 10).Take(10).ToList();

                if (fileSearch.SortBy != string.Empty)
                {
                    switch (fileSearch.SortBy.ToLower())
                    {
                        default:
                            break;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            FileIndexViewModel indexViewModel = new FileIndexViewModel()
            {
                ListOfFiles = returnlist,
                FileSearch = fileSearch,
                Tags = tags
            };

            return new OkObjectResult(indexViewModel);
        }

        [FunctionName("getfile")]
        public async Task<IActionResult> getfile([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestbody = await new StreamReader(req.Body).ReadToEndAsync();
            int id = JsonConvert.DeserializeObject<TaskManager.Models.File>(requestbody).Id;
            TaskManager.Models.File filefromdb =new FileManager().Get (id);
            return new OkObjectResult(filefromdb);
        }

        private List<string> RemoveEmptyTags(TaskManager.Models.File file)
        {
            //remove emtpy tag from array
            List<string> tags = new List<string>();
            for (int i = 0; i < file.Tags.Count; i++)
            {
                if (file.Tags[i].Trim().Length != 0)
                {
                    tags.Add(file.Tags[i].Trim());
                }
            }
            return tags;
        }

        [FunctionName("addfile")]
        public async Task<IActionResult> AddFile([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            TaskManager.Models.File file = JsonConvert.DeserializeObject<TaskManager.Models.File>(requestBody);

            file.Tags = RemoveEmptyTags(file);
            new FileManager().Insert(file);
            return new OkObjectResult(file);
        }

        [FunctionName("updatefile")]
        public async Task<IActionResult> UpdateTask([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            TaskManager.Models.File file1 = JsonConvert.DeserializeObject<TaskManager.Models.File>(requestBody);
           string result = new FileManager().Update(file1, file1.Id);

            return new OkObjectResult(result);
        }

        [FunctionName("deletefile")]
        public async Task<IActionResult> DeleteFile([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            int id = JsonConvert.DeserializeObject<TaskManager.Models.File>(requestBody).Id;

            new FileManager().DeleteFile(id);

            return new OkResult();
        }

    }
}
