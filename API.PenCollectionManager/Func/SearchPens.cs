using Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;

namespace API.PenCollectionManager.Func;

public class SearchPens(CollectionManagerContext dbContext, ILogger<SearchPens> logger)
{
    [Function("SearchPens")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "user/{userId}/fountain-pen/search")]
        HttpRequestData req, string userId, FunctionContext context)
    {
        try
        {
            logger.LogInformation($"Request received to {context.FunctionDefinition.Name}");

            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            var searchTerm = query.Get("query") ?? string.Empty;
            
            if (!Guid.TryParse(userId, out var parsedUserId))
            {
                return new BadRequestObjectResult(new { reason = "userId cannot be empty." });
            }
            
            var collectionEntries = await dbContext.Collections
                                               .Include(uc => uc.Pen)  // Include related pen catalog entry
                                               .Where(u => u.UserId == parsedUserId)
                                               .ToListAsync();  // Pulls data into memory for in-memory string operations

            List<PenCollectionEntry> result = [];
            
            if (collectionEntries.Count != 0)
            {
                result = collectionEntries
                    .Where(uc => (uc.Pen.Manufacturer + " " + uc.Pen.Model)
                                 .Contains(searchTerm, StringComparison.OrdinalIgnoreCase))  // String comparison done in memory
                    .ToList();
            }

            return new OkObjectResult(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while searching the catalogue.");
            return new StatusCodeResult(500);
        }
    }
}

