using Models.Enums;
using Newtonsoft.Json;

namespace Models.Requests.CatalogueManagement;

public class AddCatalogueEntryRequest
{
    [JsonProperty("manufacturer", NullValueHandling = NullValueHandling.Ignore)]
    public string? Manufacturer { get; init; }
    
    [JsonProperty("model", NullValueHandling = NullValueHandling.Ignore)]
    public string? Model { get; init; }
    
    [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
    public string? Color { get; init; }
    
    [JsonProperty("nibSize", NullValueHandling = NullValueHandling.Ignore)]
    public NibSize? NibSize { get; init; }
    
    [JsonProperty("nibMaterial", NullValueHandling = NullValueHandling.Ignore)]
    public NibMaterial? NibMaterial { get; init; }
    
    public void Validate()
    {
        if (string.IsNullOrEmpty(Manufacturer)) throw new ArgumentNullException(nameof(Manufacturer), "manufacturer is required.");
        if (string.IsNullOrEmpty(Model)) throw new ArgumentNullException(nameof(Model), "model is required.");
        // if (string.IsNullOrEmpty(Color)) throw new ArgumentNullException(nameof(Color), "color is required.");
        // if(!NibSize.HasValue || !Enum.IsDefined(typeof(NibSize), NibSize.Value)) throw new ArgumentNullException(nameof(NibSize), "nibSize is invalid or missing.");
        // if(!NibMaterial.HasValue || !Enum.IsDefined(typeof(NibMaterial), NibMaterial.Value)) throw new ArgumentNullException(nameof(NibMaterial), "nibMaterial is invalid or missing.");
    }
}