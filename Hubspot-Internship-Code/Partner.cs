using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Hubspot_Internship_Code
{
    class Partner
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("availableDates")]
        public List<DateTime> AvailableDates { get; set; }
    }

    class PartnerRoot
    {
        [JsonProperty("partners")]
        public List<Partner> Partners { get; set; }
    }
}
