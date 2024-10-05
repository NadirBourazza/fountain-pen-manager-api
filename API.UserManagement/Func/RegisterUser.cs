using System.Web.Http;
using Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Requests.UserManagement;
using Newtonsoft.Json;
using Utilities;

namespace API.UserManagement.Func;

public class RegisterUser(CollectionManagerContext dbContext, ILogger<RegisterUser> logger)
{
    [Function("RegisterUser")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "user")]
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

            var registerUserRequest = JsonConvert.DeserializeObject<RegisterUserRequest>(requestBody);
            
            registerUserRequest.Validate();
            
            var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == registerUserRequest.Email);
            
            if (existingUser != null)
            {
                return new ConflictObjectResult( new { reason = "Account already exists" });
            }

            var hashedPassword = new PasswordHash(registerUserRequest.Password!);
            
            var newUser = new Models.Entities.User
            {
                UserId = Guid.NewGuid(),
                FirstName = registerUserRequest.FirstName!,
                Email = registerUserRequest.Email!,
                Salt = Convert.ToBase64String(hashedPassword.Salt),
                PasswordHash = Convert.ToBase64String(hashedPassword.Hash)
            };
            
            dbContext.Users.Add(newUser);
            await dbContext.SaveChangesAsync();
            
            return new CreatedResult("test", new { userId = newUser.UserId });
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