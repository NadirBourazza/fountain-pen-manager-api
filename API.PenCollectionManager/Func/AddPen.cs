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
using Newtonsoft.Json.Converters;

namespace API.PenCollectionManager.Func;

public class AddPen(CollectionManagerContext dbContext, ILogger<AddPen> logger)
{
    [Function("AddPen")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "user/{userId}/fountain-pen")]
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

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = { new StringEnumConverter() }
            };
            
            var addPenRequest = JsonConvert.DeserializeObject<AddPenRequest>(requestBody, settings);
            
            addPenRequest.Validate();
            
            Guid.TryParse(addPenRequest.PenId, out var parsedPenId);
            
            var penData = await dbContext.PenCatalog.FirstOrDefaultAsync(catalogue => catalogue.PenId == parsedPenId);
            
            if (penData == null)
            {
                return new NotFoundObjectResult( new { reason = "Fountain pen not found in catalogue." });
            }
            
            var newPen = new PenCollectionEntry
            {
                EntryId = Guid.NewGuid(),
                PenId = parsedPenId,
                UserId = parsedUserId,
                Color = addPenRequest.Color!,
                NibSize = addPenRequest.NibSize!.Value.GetDescription(),
                NibMaterial = addPenRequest.NibMaterial!.Value.GetDescription(),
                Nickname = addPenRequest.Nickname,
                PurchasePricePence = addPenRequest.PurchasePricePence ?? 0,
                DeliveryFeePence = addPenRequest.DeliveryFeePence ?? 0,
                ImportFeePence = addPenRequest.ImportFeePence ?? 0,
                CurrentValuePence = addPenRequest.CurrentValuePence ?? 0,
                PurchaseDate = addPenRequest.PurchaseDate
            };
            
            dbContext.Collections.Add(newPen);
            await dbContext.SaveChangesAsync();
            
            return new CreatedResult("", new { entryId = newPen.EntryId });
        }
        catch (ArgumentNullException ex)
        {
            return new BadRequestObjectResult(new { reason = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return new InternalServerErrorResult();
        }
    }
}