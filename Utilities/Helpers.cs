using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Utilities;

public static class Helpers
{
    public static TResult ReadJson<TResult>(string path, JsonSerializerSettings? settings = null)
    {
        using var reader = new StreamReader(path);
        var data = JsonConvert.DeserializeObject<TResult>(reader.ReadToEndAsync().Result, settings: settings);
        return data!;
    }

    public static async Task<T> ReadRequestBody<T>(this HttpRequestData request)
    {
        var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
        var result = JsonConvert.DeserializeObject<T>(requestBody)!;
        return result;
    }
    
    
}