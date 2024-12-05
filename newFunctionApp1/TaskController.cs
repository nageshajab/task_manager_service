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

namespace FunctionApp1
{
    public class TaskController
    {
        private DAL.TaskManager taskManager;

        [FunctionName("tasklist")]
        public async Task<IActionResult> TaskList(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            TaskSearch taskSearch = JsonConvert.DeserializeObject<TaskSearch>(requestBody);

            IActionResult response = new UnauthorizedResult();

            try
            {
                taskManager = new DAL.TaskManager();
                taskSearch = taskManager.ListTasksByUserId(taskSearch);              
            }
            catch (Exception ex)
            {
                throw ex;
            }
            TaskIndexViewModel indexViewModel = new TaskIndexViewModel()
            {
                ListOfTasks = taskSearch.Tasks,
                TaskSearch = taskSearch
            };

            return new OkObjectResult(indexViewModel);
        }

        [FunctionName("gettask")]
        public async Task<IActionResult> getTask([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            taskManager = new DAL.TaskManager();
            string requestbody = await new StreamReader(req.Body).ReadToEndAsync();
            int taskid = JsonConvert.DeserializeObject<TaskManager.Models.Task>(requestbody).Id;
            TaskManager.Models.Task taskFromDb = taskManager.Get(taskid);
            return new OkObjectResult(taskFromDb);
        }

        [FunctionName("addtask")]
        public async Task<IActionResult> AddTask([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            taskManager = new DAL.TaskManager();
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            TaskManager.Models.Task task = JsonConvert.DeserializeObject<TaskManager.Models.Task>(requestBody);
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
                            taskManager.Insert(newtask);
                            iteratingdate = iteratingdate.AddDays(1);
                        } while (iteratingdate < task.EndDate);
                        break;
                    case RepeatType.Weekly:
                        do
                        {
                            TaskManager.Models.Task newtask = CloneTask(task);
                            newtask.DueDate = iteratingdate;
                            taskManager.Insert(newtask);
                            iteratingdate = iteratingdate.AddDays(7);
                        } while (iteratingdate < task.EndDate);
                        break;
                    case RepeatType.Monthly:
                        do
                        {
                            TaskManager.Models.Task newtask = CloneTask(task);
                            newtask.DueDate = iteratingdate;//from start date to enddate
                            taskManager.Insert(newtask);
                            iteratingdate = iteratingdate.AddMonths(1);
                        } while (iteratingdate < task.EndDate);
                        break;

                    case RepeatType.Yearly:
                        do
                        {
                            TaskManager.Models.Task newtask = CloneTask(task);
                            newtask.DueDate = iteratingdate;//from start date to enddate
                            taskManager.Insert(newtask);
                            iteratingdate = iteratingdate.AddYears(1);
                        } while (iteratingdate < task.EndDate);
                        break;
                }
            }
            else
            {
                taskManager.Insert(task);
            }

            return new OkObjectResult(task);
        }

        private TaskManager.Models.Task CloneTask(TaskManager.Models.Task task)
        {
            TaskManager.Models.Task task1 = new();
            task1.Status = task.Status;
            task1.DueDate = task.DueDate;
            task1.EndDate = task.EndDate;
            task1.Description = task.Description;
            task1.RepeatType = task.RepeatType;
            task1.Priority = task.Priority;
            task1.Title = task.Title;
            task1.UserId = task.UserId;
            task1.Type = task.Type;
            task1.SubType = task.SubType;
            return task1;
        }

        [FunctionName("updatetask")]
        public async Task<IActionResult> UpdateTask([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            taskManager = new DAL.TaskManager();
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            TaskManager.Models.Task task = JsonConvert.DeserializeObject<TaskManager.Models.Task>(requestBody);
            taskManager.Update(task, task.Id);

            return new OkResult();
        }

        [FunctionName("deletetask")]
        public async Task<IActionResult> DeleteTask([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            taskManager = new DAL.TaskManager();
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            int taskid = JsonConvert.DeserializeObject<TaskManager.Models.Task>(requestBody).Id;

            taskManager.DeleteTask(taskid);

            return new OkResult();
        }

    }
}
