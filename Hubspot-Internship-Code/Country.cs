using Newtonsoft.Json;
using System.Collections.Generic;

namespace Hubspot_Internship_Code
{
    class Country
    {
        [JsonProperty("attendeeCount")]
        public int AttendeeCount { get; set; }
        
        [JsonProperty("attendees")]
        public List<string> Attendees { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("startDate")]
        public string StartDate { get; set; }

        public Country(string Name)
        {
            this.Name = Name;
        }
    }

    class CountryRoot
    {
        [JsonProperty("countries")]
        public List<Country> Countries { get; set; }

        public CountryRoot()
        {
            this.Countries = new List<Country>();
        }
    }
}
