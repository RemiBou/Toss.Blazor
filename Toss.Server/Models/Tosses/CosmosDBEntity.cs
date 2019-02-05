namespace Toss.Server.Data
{
    public abstract class RavenDBDocument
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}
