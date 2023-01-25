using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Octokit;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace GithubHook
{
    public class HttpGitHubWorkflowExecute
    {
        private readonly IConfiguration _configuration;

        public HttpGitHubWorkflowExecute(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [FunctionName("HttpGitHubWorkflowExecute")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "execute/{app_name}")] HttpRequest req,
            string app_name,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var creds = new Credentials(_configuration["GitHubToken"]);
            var client = new GitHubClient(new ProductHeaderValue("xximjasonxx"));
            client.Credentials = creds;

            try
            {
                var dispatchEvent = new CreateWorkflowDispatch("main");
                dispatchEvent.Inputs = new Dictionary<string, object>();
                dispatchEvent.Inputs.Add("app_name", app_name);

                var app_roles = new List<AppRole>()
                {
                    new AppRole() { display_name = "Admin", role_name = "admin" },
                    new AppRole() { display_name = "Content Writer", role_name = "content.write" }
                };
                
                dispatchEvent.Inputs.Add("app_roles", JsonConvert.SerializeObject(app_roles));

                await client.Actions.Workflows.CreateDispatch("xximjasonxx", "GitHubWebHookExample", "blank.yml", dispatchEvent);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error executing workflow");
            }

            return new OkObjectResult("Hello World");
        }
    }
}
