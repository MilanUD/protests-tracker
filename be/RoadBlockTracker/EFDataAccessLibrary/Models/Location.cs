using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccessLibrary.Models
{
    public class Location
    {
        public string Address { get; }
        public string City { get; }
        public string Country { get; }
        public double Latitude { get; }
        public double Longitude { get; }
        public string AdditionalInfo { get; }

        public Location(string address, string city, string country, double latitude, double longitude, string additionalInfo)
        {
            Address = address;
            City = city;
            Country = country;
            Latitude = latitude;
            Longitude = longitude;
            AdditionalInfo = additionalInfo;
        }
    }
}
