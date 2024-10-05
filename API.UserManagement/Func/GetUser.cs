using System.Web.Http;
using Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Requests.UserManagement;
using Newtonsoft.Json;
using Utilities;

namespace API.UserManagement.Func;

public class GetUser(CollectionManagerContext dbContext, ILogger<GetUser> logger)
{
    [Function("GetUser")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "user")]
        HttpRequestData req, FunctionContext context)
    {
        try
        {
            logger.LogInformation($"Request received to {context.FunctionDefinition.Name}");
            
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(requestBody))
            {
                return new BadRequestObjectResult(new { reason = "Request body is empty." });
            }

            var getUserRequest = JsonConvert.DeserializeObject<GetUserRequest>(requestBody);
            
            getUserRequest.Validate();
            
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == getUserRequest.Email);
            
            if (user == null)
            {
                return new NotFoundObjectResult( new { reason = "User not found" });
            }
            
            var dbStoredUserPasswordHash = new PasswordHash(Convert.FromBase64String(user.Salt), Convert.FromBase64String(user.PasswordHash));

            var isCorrectPassword = dbStoredUserPasswordHash.Verify(getUserRequest.Password!);
            
            if(!isCorrectPassword)
            {
                return new UnauthorizedObjectResult(new { reason = "Invalid password" });
            }

            return new OkObjectResult(user.UserId);
        }
        catch (ArgumentNullException ex)
        {
            return new BadRequestObjectResult(new { reason = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating a user.");
            return new InternalServerErrorResult();
            //     return req.CreateResponse(HttpStatusCode.InternalServerError, new { reason = "An error occurred" });
        }
    }
}