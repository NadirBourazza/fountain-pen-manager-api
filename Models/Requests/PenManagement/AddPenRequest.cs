using Models.Enums;
using Newtonsoft.Json;

namespace Models.Requests.PenManagement;

public class AddPenRequest
{
    [JsonProperty("penId", NullValueHandling = NullValueHandling.Ignore)]
    public string? PenId { get; init; }
    
    [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
    public string? Color { get; init; }
    
    [JsonProperty("nibSize", NullValueHandling = NullValueHandling.Ignore)]
    public NibSize? NibSize { get; init; }
    
    [JsonProperty("nibMaterial", NullValueHandling = NullValueHandling.Ignore)]
    public NibMaterial? NibMaterial { get; init; }

    [JsonProperty("nickname", NullValueHandling = NullValueHandling.Ignore)]
    public string? Nickname { get; init; }
    
    [JsonProperty("purchasePricePence", NullValueHandling = NullValueHandling.Ignore)]
    public int? PurchasePricePence { get; init; }
    
    [JsonProperty("deliveryFeePence", NullValueHandling = NullValueHandling.Ignore)]
    public int? DeliveryFeePence { get; init; }
    
    [JsonProperty("importFeePence", NullValueHandling = NullValueHandling.Ignore)]
    public int? ImportFeePence { get; init; }
    
    [JsonProperty("currentValuePence", NullValueHandling = NullValueHandling.Ignore)]
    public int? CurrentValuePence { get; init; }
    
    [JsonProperty("purchaseDate", NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? PurchaseDate { get; init; }
    
    public void Validate()
    {
        if (string.IsNullOrEmpty(PenId)) throw new ArgumentNullException(nameof(PenId), "penId is required.");
        if (string.IsNullOrEmpty(Color)) throw new ArgumentNullException(nameof(Color), "color is required.");
        if(!NibSize.HasValue || !Enum.IsDefined(typeof(NibSize), NibSize.Value)) throw new ArgumentNullException(nameof(NibSize), "nibSize is invalid or missing.");
        if(!NibMaterial.HasValue || !Enum.IsDefined(typeof(NibMaterial), NibMaterial.Value)) throw new ArgumentNullException(nameof(NibMaterial), "nibMaterial is invalid or missing.");
    }
}