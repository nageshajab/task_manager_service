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
using MongoDB.Driver.Linq;

namespace FunctionApp1
{
    public class TaskController
    {
        private readonly MongoDbContext _context;

        public TaskController(MongoDbContext context)
        {
            _context = context;
        }

        [FunctionName("tasklist")]
        public async Task<IActionResult> TaskList(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            TaskSearch taskSearch = JsonConvert.DeserializeObject<TaskSearch>(requestBody);

            IActionResult response = new UnauthorizedResult();

            IQueryable<TaskManager.Models.Task> lstTasks;
            try
            {
                lstTasks = _context.Tasks.Where(t => t.UserId == taskSearch.UserId);

                if (taskSearch.Status != string.Empty && taskSearch.Status != Status.None.ToString())
                {
                    if (Enum.TryParse(taskSearch.Status, out Status parsedStatus))
                    {
                        lstTasks = lstTasks.Where(t => t.Status == parsedStatus);
                    }
                }

                if (taskSearch.DueFromDate != DateTime.MinValue || taskSearch.DueToDate != DateTime.MinValue)
                {
                    if (taskSearch.DueFromDate != DateTime.MinValue)
                    {
                        lstTasks = lstTasks.Where(t => t.DueDate > taskSearch.DueFromDate);
                    }
                    if (taskSearch.DueToDate != DateTime.MinValue)
                    {
                        lstTasks = lstTasks.Where(t => t.DueDate < taskSearch.DueToDate);
                    }
                }

                if (taskSearch.SortBy == string.Empty)
                {
                    taskSearch.SortBy = "duedate asc";
                }
                
                switch (taskSearch.SortBy.ToLower())
                {
                    case "duedate asc":
                        lstTasks = lstTasks.OrderBy(t => t.DueDate);
                        break;
                    case "duedate desc":
                        lstTasks = lstTasks.OrderByDescending(t => t.DueDate);
                        break;
                    case "status asc":
                        lstTasks = lstTasks.OrderBy(t => t.Status);
                        break;
                    case "status desc":
                        lstTasks = lstTasks.OrderByDescending(t => t.Status);
                        break;
                    default:
                        break;
                }

                taskSearch.TotalRecords = lstTasks.Count();
                //pagination at work
                lstTasks = lstTasks.Skip((taskSearch.PageNumber - 1) * 10).Take(10);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            TaskIndexViewModel indexViewModel = new TaskIndexViewModel()
            {
                ListOfTasks = lstTasks.ToList(),
                TaskSearch = taskSearch
            };

            return new OkObjectResult(indexViewModel);
        }

        [FunctionName("gettask")]
        public async Task<IActionResult> getTask([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestbody = await new StreamReader(req.Body).ReadToEndAsync();
            string taskid = JsonConvert.DeserializeObject<TaskManager.Models.Task1>(requestbody).Id.ToString();
            TaskManager.Models.Task taskFromDb = _context.Tasks.FirstOrDefault(t => t.Id.ToString() == taskid);
            return new OkObjectResult(taskFromDb);
        }

        [FunctionName("addtask")]
        public async Task<IActionResult> AddTask([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            TaskManager.Models.Task task = JsonConvert.DeserializeObject<TaskManager.Models.Task>(requestBody);
            task.RepeatId = Guid.NewGuid();
            if (task.RepeatType != RepeatType.None)
            {
                DateTime iteratingdate = task.DueDate;
                switch (task.RepeatType)
                {
                    case RepeatType.Daily:
                        do
                        {
                            TaskManager.Models.Task newtask = CloneTask(task);
                            newtask.DueDate = iteratingdate;//from start date to enddate
                            _context.Tasks.Add(newtask);
                            iteratingdate = iteratingdate.AddDays(1);
                        } while (iteratingdate < task.EndDate);
                        break;
                    case RepeatType.Weekly:
                        do
                        {
                            TaskManager.Models.Task newtask = CloneTask(task);
                            newtask.DueDate = iteratingdate;
                            _context.Tasks.Add(newtask);
                            iteratingdate = iteratingdate.AddWeeks(1);
                        } while (iteratingdate < task.EndDate);
                        break;
                    case RepeatType.Monthly:
                        do
                        {
                            TaskManager.Models.Task newtask = CloneTask(task);
                            newtask.DueDate = iteratingdate;//from start date to enddate
                            _context.Tasks.Add(newtask);
                            iteratingdate = iteratingdate.AddMonths(1);
                        } while (iteratingdate < task.EndDate);
                        break;

                    case RepeatType.Yearly:
                        do
                        {
                            TaskManager.Models.Task newtask = CloneTask(task);
                            newtask.DueDate = iteratingdate;//from start date to enddate
                            _context.Tasks.Add(newtask);
                            iteratingdate = iteratingdate.AddYears(1);
                        } while (iteratingdate < task.EndDate);
                        break;
                }
            }
            else
            {
                _context.Tasks.Add(task);
            }

            _context.SaveChanges();
            return new OkObjectResult(task);
        }

        private TaskManager.Models.Task CloneTask(TaskManager.Models.Task task)
        {
            TaskManager.Models.Task task1 = new();
            task1.Status = task.Status;
            task1.DueDate = task.DueDate;
            task1.EndDate = task.EndDate;
            task1.Description = task.Description;
            task1.CanRepeat = task.CanRepeat;
            task1.RepeatType = task.RepeatType;
            task1.Id = new MongoDB.Bson.ObjectId();
            task1.Priority = task.Priority;
            task1.RepeatId = task.RepeatId;
            task1.Title = task.Title;
            task1.UserId = task.UserId;
            return task1;
        }

        [FunctionName("updatetask")]
        public async Task<IActionResult> UpdateTask([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            TaskManager.Models.Task1 task = JsonConvert.DeserializeObject<TaskManager.Models.Task1>(requestBody);
            TaskManager.Models.Task taskFromDb = _context.Tasks.FirstOrDefault(t => t.Id.ToString() == task.Id.ToString());
            if (taskFromDb == null)
            {
                return new NotFoundResult();
            }
            taskFromDb.Title = task.Title;
            taskFromDb.Description = task.Description;
            taskFromDb.DueDate = task.DueDate;
            taskFromDb.Priority = task.Priority;
            taskFromDb.Status = task.Status;
            taskFromDb.CanRepeat = task.CanRepeat;
            taskFromDb.RepeatType = task.RepeatType;
            _context.SaveChanges();
            return new OkObjectResult(taskFromDb);
        }

        [FunctionName("deletetask")]
        public async Task<IActionResult> DeleteTask([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            string taskid = JsonConvert.DeserializeObject<TaskManager.Models.Task1>(requestBody).Id.ToString();

            TaskManager.Models.Task taskFromDb = _context.Tasks.FirstOrDefault(t => t.Id.ToString() == taskid.ToString());
            if (taskFromDb == null)
            {
                return new NotFoundResult();
            }
            _context.Tasks.Remove(taskFromDb);
            _context.SaveChanges();
            return new OkObjectResult(taskFromDb);
        }

    }
}
