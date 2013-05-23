namespace DWH.Raven.Tests.Models
{
    public class Community
    {

        public enum Geolocations
        {
            All = 0,
            North,
            South,
            East,
            West
        }

        public enum CommunityTypes
        {
            Normal = 0,
            BuildOnYourLot
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public Geolocations Geolocation { get; set; }
        public CommunityTypes CommunityType { get; set; }
        public string[] SchoolIds { get; set; }

    }
}