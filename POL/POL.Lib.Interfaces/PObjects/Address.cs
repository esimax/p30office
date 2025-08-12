using System;

namespace POL.Lib.Interfaces.PObjects
{
    [Serializable]
    public class Address
    {
        public string Title { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        public string Street { get; set; }
        public string POBox { get; set; }
        public string ZipCode { get; set; }
        public string Note { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
