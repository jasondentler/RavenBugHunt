namespace DWH.Raven.Tests.Models
{
    public class CommunityReference
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public static implicit operator CommunityReference(Community community)
        {
            return new CommunityReference()
                {
                    Id = community.Id,
                    Name = community.Name
                };
        }

    }
}