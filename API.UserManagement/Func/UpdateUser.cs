using System.Web.Http;
using Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Requests;
using Models.Requests.UserManagement;
using Newtonsoft.Json;

namespace API.UserManagement.Func;

public class UpdateUser(CollectionManagerContext dbContext, ILogger<UpdateUser> logger)
{
    [Function("UpdateUser")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "user/{userId}")]
        HttpRequestData req, string userId, FunctionContext context)
    {
        try
        {
            logger.LogInformation($"Request received to {context.FunctionDefinition.Name}");
            
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            
            if (!Guid.TryParse(userId, out var parsedUserId))
            {
                return new BadRequestObjectResult(new { reason = "Invalid userId format." });
            }

            if (string.IsNullOrWhiteSpace(requestBody))
            {
                return new BadRequestObjectResult(new { reason = "Request body is empty." });
            }

            var updateUserRequest = JsonConvert.DeserializeObject<UpdateUserRequest>(requestBody);
            
            updateUserRequest.Validate();
            
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == parsedUserId);
            
            if (user == null)
            {
                return new NotFoundObjectResult( new { reason = "User not found" });
            }
            
            dbContext.Entry(user).CurrentValues.SetValues(updateUserRequest);
            await dbContext.SaveChangesAsync();
            
            return new NoContentResult();
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