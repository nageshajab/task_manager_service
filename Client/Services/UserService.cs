// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Client.Models;

namespace TodoListClient.Services
{
    public static class UserServiceExtensions
    {
        public static void AddUserService(this IServiceCollection services, IConfiguration configuration)
        {
            // https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
            services.AddHttpClient<IUserService, UserService>();
        }
    }

    /// <summary></summary>
    /// <seealso cref="TodoListClient.Services.ITodoListService" />
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly HttpClient _httpClient;
        private readonly string _userScope = string.Empty;
        private readonly string _userBaseAddress = string.Empty;
        private readonly ITokenAcquisition _tokenAcquisition;

        public UserService(ITokenAcquisition tokenAcquisition, HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _httpClient = httpClient;
            _tokenAcquisition = tokenAcquisition;
            _contextAccessor = contextAccessor;
            _userScope = configuration["TodoList:UserScope"];
            _userBaseAddress = configuration["TodoList:TodoListBaseAddress"];
        }

        public async Task DeleteAsync(string id)
        {
            await PrepareAuthenticatedClient();

            var response = await this._httpClient.DeleteAsync($"{ _userBaseAddress}/api/userlist/{id}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task<User> EditAsync(User user)
        {
            await PrepareAuthenticatedClient();

            var jsonRequest = JsonConvert.SerializeObject(user);
            var jsoncontent = new StringContent(jsonRequest, Encoding.UTF8, "application/json-patch+json");

            var response = await _httpClient.PatchAsync($"{ _userBaseAddress}/api/user", jsoncontent);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                user = JsonConvert.DeserializeObject<User>(content);

                return user;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task<IEnumerable<User>> GetAsync()
        {
            await PrepareAuthenticatedClient();

            var response = await _httpClient.GetAsync($"{ _userBaseAddress}/api/user");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                IEnumerable<User> userlist = JsonConvert.DeserializeObject<IEnumerable<User>>(content);

                return userlist;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        private async Task PrepareAuthenticatedClient()
        {
            var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { _userScope });
            Debug.WriteLine($"access token-{accessToken}");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<User> GetAsync(string id)
        {
            await PrepareAuthenticatedClient();

            var response = await _httpClient.GetAsync($"{ _userBaseAddress}/api/user/{id}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                User user= JsonConvert.DeserializeObject<User>(content);

                return user;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }
    }
}