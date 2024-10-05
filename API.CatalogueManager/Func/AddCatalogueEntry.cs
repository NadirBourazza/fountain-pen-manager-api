using System.Web.Http;
using Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Models.Requests.CatalogueManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace API.CatalogueManager.Func;

public class AddCatalogueEntry(CollectionManagerContext dbContext, ILogger<AddCatalogueEntry> logger)
{
    [Function("AddCatalogueEntry")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "catalogue")]
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
            
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = { new StringEnumConverter() }
            };

            var addCatalogueEntryRequest = JsonConvert.DeserializeObject<AddCatalogueEntryRequest>(requestBody, settings);
            
            addCatalogueEntryRequest.Validate();
            
            var existingEntry = await dbContext.PenCatalog.FirstOrDefaultAsync(catalogue => catalogue.Manufacturer == addCatalogueEntryRequest.Manufacturer && catalogue.Model == addCatalogueEntryRequest.Model);
            
            if (existingEntry != null)
            {
                return new ConflictObjectResult( new { reason = "Fountain pen entry is already in catalogue." });
            }
            
            var newEntry = new PenCatalogueEntry
            {
                PenId = Guid.NewGuid(),
                Manufacturer = addCatalogueEntryRequest.Manufacturer!,
                Model = addCatalogueEntryRequest.Model!,
            };
            
            dbContext.PenCatalog.Add(newEntry);
            await dbContext.SaveChangesAsync();
            
            return new CreatedResult("", new { entryId = newEntry.PenId });
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