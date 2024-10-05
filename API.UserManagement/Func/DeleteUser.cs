using System.Web.Http;
using Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.UserManagement.Func;

public class DeleteUser(CollectionManagerContext dbContext, ILogger<DeleteUser> logger)
{
    [Function("DeleteUser")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "user/{userId}")]
        HttpRequestData req, string userId, FunctionContext context)
    {
        try
        {
            logger.LogInformation($"Request received to delete user with ID: {userId}");

            if (!Guid.TryParse(userId, out var parsedUserId))
            {
                return new BadRequestObjectResult(new { reason = "Invalid userId format." });
            }

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == parsedUserId);

            if (user == null)
            {
                return new NotFoundObjectResult(new { reason = "User not found." });
            }

            dbContext.Users.Remove(user);
            await dbContext.SaveChangesAsync();

            return new OkObjectResult(new { message = "User deleted successfully." });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while deleting the user.");
            return new InternalServerErrorResult();
        }
    }
}