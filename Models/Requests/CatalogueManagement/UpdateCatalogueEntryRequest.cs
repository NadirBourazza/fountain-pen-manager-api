using Newtonsoft.Json;

namespace Models.Requests.CatalogueManagement;

public class UpdateCatalogueEntryRequest
{
    [JsonProperty("manufacturer", NullValueHandling = NullValueHandling.Ignore)]
    public string? Manufacturer { get; init; }
    
    [JsonProperty("model", NullValueHandling = NullValueHandling.Ignore)]
    public string? Model { get; init; }
    
    public void Validate()
    {
        if (string.IsNullOrEmpty(Manufacturer)) throw new ArgumentNullException(nameof(Manufacturer), "manufacturer is required.");
        if (string.IsNullOrEmpty(Model)) throw new ArgumentNullException(nameof(Model), "model is required.");
    }
}