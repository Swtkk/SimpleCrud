using System.Text.Json.Serialization;

namespace SimpleCrudProject.Model;

public class City
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CountryId { get; set; }
    [JsonIgnore]
    public virtual Country? Country { get; set; }
    public double? Temperature { get; set; }
}