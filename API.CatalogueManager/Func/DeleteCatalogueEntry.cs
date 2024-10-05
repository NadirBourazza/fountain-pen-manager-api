using System.Web.Http;
using Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.CatalogueManager.Func;

public class DeleteCatalogueEntry(CollectionManagerContext dbContext, ILogger<DeleteCatalogueEntry> logger)
{
    [Function("DeleteCatalogueEntry")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "catalogue/{penId}")]
        HttpRequestData req, string penId, FunctionContext context)
    {
        try
        {
            logger.LogInformation($"Request received to {context.FunctionDefinition.Name}");
            
            if (string.IsNullOrWhiteSpace(penId))
            {
                return new BadRequestObjectResult(new { reason = "penId is null or empty." });
            }

            if (!Guid.TryParse(penId, out var parsedPenId))
            {
                return new BadRequestObjectResult(new { reason = "penId is not a valid GUID." });
            }
            
            var penEntry = await dbContext.PenCatalog.FirstOrDefaultAsync(catalogue => catalogue.PenId == parsedPenId);
            
            if (penEntry == null)
            {
                return new NotFoundObjectResult( new { reason = "Fountain pen entry not found." });
            }
            
            dbContext.PenCatalog.Remove(penEntry);
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