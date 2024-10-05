using System.Web.Http;
using Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Requests.CatalogueManagement;
using Newtonsoft.Json;

namespace API.CatalogueManager.Func;

public class UpdateCatalogueEntry(CollectionManagerContext dbContext, ILogger<UpdateCatalogueEntry> logger)
{
    [Function("UpdatePenCatalogueEntry")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "catalogue/{penId}")]
        HttpRequestData req, string penId, FunctionContext context)
    {
        try
        {
            logger.LogInformation($"Request received to {context.FunctionDefinition.Name}");
            
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            
            if (!Guid.TryParse(penId, out var parsedPenId))
            {
                return new BadRequestObjectResult(new { reason = "Invalid penId format." });
            }

            if (string.IsNullOrWhiteSpace(requestBody))
            {
                return new BadRequestObjectResult(new { reason = "Request body is empty." });
            }

            var updateCatalogueEntryRequest = JsonConvert.DeserializeObject<UpdateCatalogueEntryRequest>(requestBody);
            
            updateCatalogueEntryRequest.Validate();
            
            var catalogueEntry = await dbContext.PenCatalog.FirstOrDefaultAsync(entry => entry.PenId == parsedPenId);
            
            if (catalogueEntry == null)
            {
                return new NotFoundObjectResult( new { reason = "Pen not found" });
            }
            
            dbContext.Entry(catalogueEntry).CurrentValues.SetValues(updateCatalogueEntryRequest);
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