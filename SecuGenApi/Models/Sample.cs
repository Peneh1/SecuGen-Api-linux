using System.Text.Json.Serialization;

namespace SecuGenApi.Models
{
    public class Sample
    {
        /// <summary>
        /// Clock object
        /// </summary>
        public class Clock
        {
            [JsonPropertyName("device")]
            public string? Device { get; set; }

            [JsonPropertyName("badge")]
            public string? Badge { get; set; }

            [JsonPropertyName("name")]
            public string? Name { get; set; }

            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("type")]
            public string? Type { get; set; }

            [JsonPropertyName("clockTime")]
            public DateTime ClockTime { get; set; }

            [JsonPropertyName("workCode")]
            public string? WorkCode { get; set; }

        }

        public class BioTimeClock
        {
            [JsonPropertyName("device")]
            public string? Device { get; set; }

            [JsonPropertyName("badge")]
            public string? Badge { get; set; }

            [JsonPropertyName("name")]
            public string? Name { get; set; }

            [JsonPropertyName("lastname")]
            public string? LastName { get; set; }

            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("punchstate")]
            public string? PunchState { get; set; }

            [JsonPropertyName("punchtime")]
            public DateTime PunchTime { get; set; }

            [JsonPropertyName("workcode")]
            public string? WorkCode { get; set; }

            [JsonPropertyName("longitude")]
            public string? Longitude { get; set; }

            [JsonPropertyName("latitude")]
            public string? Latitude { get; set; }

            [JsonPropertyName("gps_location")]
            public string? Gps_Location { get; set; }

        }

    }
}
