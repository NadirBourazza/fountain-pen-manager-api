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

public class UpdatePen(CollectionManagerContext dbContext, ILogger<UpdatePen> logger)
{
    [Function("UpdatePen")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "user/{userId}/fountain-pen/{entryId}")]
        HttpRequestData req, string userId, string entryId, FunctionContext context)
    {
        try
        {
            logger.LogInformation($"Request received to {context.FunctionDefinition.Name}");
            
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            if (!Guid.TryParse(userId, out var parsedUserId))
            {
                return new BadRequestObjectResult(new { reason = "Invalid userId format." });
            }
            
            if (!Guid.TryParse(entryId, out var parsedEntryId))
            {
                return new BadRequestObjectResult(new { reason = "Invalid entryId format." });
            }
            
            if (string.IsNullOrWhiteSpace(requestBody))
            {
                return new BadRequestObjectResult(new { reason = "Request body is empty." });
            }

            var updatePenRequest = JsonConvert.DeserializeObject<UpdatePenRequest>(requestBody);
            
            updatePenRequest.Validate();
            
            if (!Guid.TryParse(updatePenRequest.PenId, out var parsedPenId))
            {
                return new BadRequestObjectResult(new { reason = "Invalid penId format." });
            }
            
            // Check penId is known
            var penData = await dbContext.PenCatalog.FirstOrDefaultAsync(catalogueEntry => catalogueEntry.PenId == parsedPenId);
            
            if (penData == null)
            {
                return new NotFoundObjectResult( new { reason = "Fountain pen not found in catalogue." });
            }
            
            var entryData = await dbContext.Collections.FirstOrDefaultAsync(entry => entry.UserId == parsedUserId && entry.EntryId == parsedEntryId);
            
            if (entryData == null)
            {
                return new NotFoundObjectResult( new { reason = "Fountain pen not found in collection." });
            }
            
            var newPen = new PenCollectionEntry
            {
                EntryId = Guid.NewGuid(),
                PenId = parsedPenId,
                UserId = parsedUserId,
                Color = updatePenRequest.Color!,
                NibSize = updatePenRequest.NibSize!.Value.GetDescription(),
                NibMaterial = updatePenRequest.NibMaterial!.Value.GetDescription(),
                Nickname = updatePenRequest.Nickname,
                PurchasePricePence = updatePenRequest.PurchasePricePence ?? 0,
                DeliveryFeePence = updatePenRequest.DeliveryFeePence ?? 0,
                ImportFeePence = updatePenRequest.ImportFeePence ?? 0,
                CurrentValuePence = updatePenRequest.CurrentValuePence ?? 0,
                PurchaseDate = updatePenRequest.PurchaseDate
            };
            
            dbContext.Collections.Add(newPen);
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