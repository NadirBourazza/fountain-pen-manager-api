using Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.CatalogueManager.Func
{
    public class SearchCatalogueEntries(CollectionManagerContext dbContext, ILogger<SearchCatalogueEntries> logger)
    {
        [Function("SearchCatalogueEntries")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "catalogue/search")]
            HttpRequestData req, FunctionContext context)
        {
            try
            {
                logger.LogInformation($"Request received to {context.FunctionDefinition.Name}");

                var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
                
                var searchTerm = query.Get("query") ?? string.Empty;

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return new BadRequestObjectResult(new { reason = "Search term cannot be empty." });
                }

                // Step 1: Fetch all potential matches from the database (with basic filtering, if necessary)
                var potentialMatches = await dbContext.PenCatalog.ToListAsync();

                // Step 2: Perform the case-insensitive search in-memory
                var matchingEntries = potentialMatches
                    .Where(pen => (pen.Manufacturer + " " + pen.Model).Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (matchingEntries.Count == 0)
                {
                    return new NotFoundObjectResult(new { reason = "No matching catalogue entries found." });
                }

                // Serialize and return matching results
                return new OkObjectResult(matchingEntries);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while searching the catalogue.");
                return new StatusCodeResult(500);
            }
        }
    }
}
