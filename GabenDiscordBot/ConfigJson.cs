using Newtonsoft.Json;

internal struct ConfigJson
{
    [JsonProperty("token")] public string Token { get; private set; }

    [JsonProperty("prefix")] public string Prefix { get; private set; }

    [JsonProperty("eventStoreConnectionString")] public string EventStoreConnectionString { get; private set; }
}