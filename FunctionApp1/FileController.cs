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

namespace FunctionApp1
{
    public class FileController
    {
        private readonly MongoDbContext _context;

        public FileController(MongoDbContext context)
        {
            _context = context;
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

            IQueryable<TaskManager.Models.File> lstFiles;
            try
            {
                lstFiles = _context.Files.Where(t => t.UserId == fileSearch.UserId);
                fileSearch.TotalRecords = lstFiles.Count();

                //pagination at work
                lstFiles = lstFiles.Skip((fileSearch.PageNumber - 1) * 10).Take(10);

                if (fileSearch.SortBy != string.Empty)
                {
                    switch (fileSearch.SortBy.ToLower())
                    {
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            FileIndexViewModel indexViewModel = new FileIndexViewModel()
            {
                ListOfFiles= lstFiles.ToList(),
                FileSearch= fileSearch
            };

            return new OkObjectResult(indexViewModel);
        }

        [FunctionName("getfile")]
        public async Task<IActionResult> getfile([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestbody = await new StreamReader(req.Body).ReadToEndAsync();
            string id = JsonConvert.DeserializeObject<TaskManager.Models.File1>(requestbody).Id.ToString();
            TaskManager.Models.File filefromdb = _context.Files.FirstOrDefault(t => t.Id.ToString() == id);
            return new OkObjectResult(filefromdb);
        }

        [FunctionName("addfile")]
        public async Task<IActionResult> AddFile([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            TaskManager.Models.File file= JsonConvert.DeserializeObject<TaskManager.Models.File>(requestBody);
            _context.Files.Add(file);
            _context.SaveChanges();
            return new OkObjectResult(file);
        }

        [FunctionName("updatefile")]
        public async Task<IActionResult> UpdateTask([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            File1 file1= JsonConvert.DeserializeObject<File1>(requestBody);
            TaskManager.Models.File filefromdb = _context.Files.FirstOrDefault(t => t.Id.ToString() == file1.Id.ToString());
            
            if (filefromdb == null)
            {
                return new NotFoundResult();
            }

            filefromdb.Name= file1.Name;
            filefromdb.Description = file1.Description;
            filefromdb.Tags= file1.Tags;
            filefromdb.ParentFolder= file1.ParentFolder;
            filefromdb.GoogleDrivePath= file1.GoogleDrivePath;
            filefromdb.AzurePath= file1.AzurePath;

            _context.SaveChanges();
            return new OkObjectResult(filefromdb);
        }

        [FunctionName("deletefile")]
        public async Task<IActionResult> DeleteFile([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            string id = JsonConvert.DeserializeObject<TaskManager.Models.File1>(requestBody).Id.ToString();

            TaskManager.Models.File filefromdb= _context.Files.FirstOrDefault(t => t.Id.ToString() == id.ToString());
            
            if (filefromdb== null)
            {
                return new NotFoundResult();
            }

            _context.Files.Remove(filefromdb);
            _context.SaveChanges();

            return new OkObjectResult(filefromdb);
        }

    }
}
