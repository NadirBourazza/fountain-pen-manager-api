using System.Web.Http;
using Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Models.Requests.PenManagement;
using Newtonsoft.Json;

namespace API.PenCollectionManager.Func;

public class DeletePen(CollectionManagerContext dbContext, ILogger<DeletePen> logger)
{
    [Function("DeletePen")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "user/{userId}/fountain-pen/{entryId}")]
        HttpRequestData req, string userId, string entryId, FunctionContext context)
    {
        try
        {
            logger.LogInformation($"Request received to {context.FunctionDefinition.Name}");

            if (!Guid.TryParse(userId, out var parsedUserId))
            {
                return new BadRequestObjectResult(new { reason = "Invalid userId format." });
            }
            
            if (!Guid.TryParse(entryId, out var parsedEntryId))
            {
                return new BadRequestObjectResult(new { reason = "Invalid entryId format." });
            }
            
            var penData = await dbContext.Collections.FirstOrDefaultAsync(entry => entry.UserId == parsedUserId && entry.EntryId == parsedEntryId);
            
            if (penData == null)
            {
                return new NotFoundObjectResult( new { reason = "Fountain pen not found in collection." });
            }
            
            dbContext.Collections.Remove(penData);
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
        }
    }
}