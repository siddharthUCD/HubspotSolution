using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hubspot_Internship_Code
{
    class Program
    {
        private static readonly HttpClient Client = new HttpClient();

        static async Task Main(string[] args)
        {
            string ApiKey = "3693c6667cc824cc339e03c5f8da";
            string GetURL = "https://candidate.hubteam.com/candidateTest/v3/problem/dataset?userKey=" + ApiKey;
            string PostURL = "https://candidate.hubteam.com/candidateTest/v3/problem/result?userKey=" + ApiKey;
            Program Main = new Program();

            Dictionary<string, List<DateTime>> DatesPerCountry = new Dictionary<string, List<DateTime>>();
            CountryRoot FinalDates;

            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                PartnerRoot DeserializedPartnerClass = JsonConvert.DeserializeObject<PartnerRoot>(await Client.GetStringAsync(GetURL));

                //Populates Dictionary with the survey dates for each country
                DatesPerCountry = Main.GetAvailableDates(DeserializedPartnerClass, DatesPerCountry);

                //Retrieves the optimal date for the event for each country along with attendee details
                FinalDates = Main.GetFinalDatesData(DatesPerCountry, DeserializedPartnerClass);


                var SerializedFinalDates = JsonConvert.SerializeObject(FinalDates);
                var Content = new StringContent(SerializedFinalDates, Encoding.UTF8, "application/json");
                var Result = Client.PostAsync(PostURL, Content).Result;

                Console.WriteLine(Result.Content.ReadAsStringAsync().Result);
            }

            catch(Exception ex)
            {
                Console.WriteLine("Try Again!" + ex.StackTrace);
            }
        }

        /// <summary>
        /// Consolidates all the survey dates for each country into a dictionary variable
        /// </summary>
        /// <param name="deserializedPartnerClass"></param>
        /// <param name="DatesPerCountry"></param>
        private Dictionary<string, List<DateTime>> GetAvailableDates(PartnerRoot DeserializedPartnerClass, Dictionary<string, List<DateTime>> DatesPerCountry)
        {
            try
            {
                foreach (Partner Partner in DeserializedPartnerClass.Partners)
                {
                    foreach (DateTime AvailableDate in Partner.AvailableDates)
                    {
                        if (!DatesPerCountry.ContainsKey(Partner.Country))
                        {
                            DatesPerCountry.Add(Partner.Country, new List<DateTime>());
                        }

                        if (Partner.AvailableDates.Contains(AvailableDate.AddDays(1)))
                        {
                            DatesPerCountry[Partner.Country].Add(AvailableDate);
                        }
                    }
                }
                return DatesPerCountry;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Picks the best date for the event for each country and based on that populates the attendee details
        /// </summary>
        /// <param name="DatesPerCountry"></param>
        /// <param name="DeserializedPartnerClass"></param>
        /// <returns>Returns a list of country objects with attendee details and event start date</returns>
        private CountryRoot GetFinalDatesData(Dictionary<string, List<DateTime>> DatesPerCountry, PartnerRoot DeserializedPartnerClass)
        {
            CountryRoot FinalList = new CountryRoot();

            try
            {
                foreach (string CountryName in DatesPerCountry.Keys)
                {
                    Country Country = new Country(CountryName);

                    DateTime StartDate = DatesPerCountry[CountryName].GroupBy(i => i)
                                                                    .OrderByDescending(grp => grp.Count())
                                                                    .ThenBy(grp => grp.Key) //To pick the earliest date during a tie
                                                                    .Select(grp => grp.Key)
                                                                    .FirstOrDefault();

                    Country.Attendees = DeserializedPartnerClass.Partners.Where(x => x.Country == CountryName)
                                                                            .Where(x => x.AvailableDates.Contains(StartDate) && x.AvailableDates.Contains(StartDate.AddDays(1)))
                                                                            .Select(x => x.Email)
                                                                            .ToList();

                    Country.AttendeeCount = Country.Attendees.Count();

                    Country.StartDate = (StartDate == DateTime.MinValue) ? null : StartDate.ToString("yyyy-MM-dd");

                    FinalList.Countries.Add(Country);
                }

                return FinalList;
            }

            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
