using System.Linq;
using DWH.Raven.Tests.Models;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace DWH.Raven.Tests.Indexes
{
    public class CommunitySearchIndex : AbstractMultiMapIndexCreationTask<CommunitySearchIndex.MapResult>
    {

        public class MapResult
        {
            public Community Community { get; set; }
            public FloorPlan FloorPlan { get; set; }
            public Showcase Showcase { get; set; }

            public string Id { get; set; }
            public string MarketId { get; set; }
            public string CityId { get; set; }
            public Community.Geolocations Geolocation { get; set; }
            public string[] SchoolDistricts { get; set; }
            public string Name { get; set; }
            public string CommunityName { get; set; }
            public string CommunityId { get; set; }
            public Community.CommunityTypes CommunityType { get; set; }
        }

        public const string TheIndexName = "Communities/Search";

        public override string IndexName
        {
            get { return TheIndexName; }
        }


        public CommunitySearchIndex()
        {

            AddMap<FloorPlan>(floorPlans =>
                              from floorPlan in floorPlans
                              let community = LoadDocument<Community>(floorPlan.Community.Id)
                              where MetadataFor(community).Value<string>("Raven-Entity-Name") == "Communities"
                              let schoolDistricts = community.SchoolIds.Select(id => LoadDocument<School>(id).SchoolDistrictName).ToArray()
                              select new MapResult
                              {
                                  Id = floorPlan.Id,
                                  Community = community,
                                  CommunityId = community.Id,
                                  CommunityName = community.Name,
                                  CommunityType = community.CommunityType,
                                  Geolocation = community.Geolocation,
                                  SchoolDistricts = schoolDistricts
                              }
                              );

            AddMap<Showcase>(showcases =>
                              from showcase in showcases
                              let community = LoadDocument<Community>(showcase.Community.Id)
                              where MetadataFor(community).Value<string>("Raven-Entity-Name") == "Communities"
                              let schoolDistricts = community.SchoolIds.Select(id => LoadDocument<School>(id).SchoolDistrictName).ToArray()
                              select new MapResult
                              {
                                  Id = showcase.Id,
                                  Community = community,
                                  CommunityId = community.Id,
                                  CommunityName = community.Name,
                                  CommunityType = community.CommunityType,
                                  Geolocation = community.Geolocation,
                                  SchoolDistricts = schoolDistricts
                              }
                              );

            Reduce = results =>
                     from result in results
                     group result by result.CommunityId
                         into g
                         let firstResult = g.First()
                         select new
                         {
                             Id = firstResult.CommunityId,
                             Geolocation = firstResult.Community.Geolocation,
                             SchoolDistricts = firstResult.SchoolDistricts,
                             Community = firstResult.Community,
                             CommunityId = firstResult.Community.Id,
                             CommunityName = firstResult.Community.Name,
                             CommunityType = firstResult.Community.CommunityType
                         };


            Index(r => r.Geolocation, FieldIndexing.NotAnalyzed);
            Index(r => r.SchoolDistricts, FieldIndexing.NotAnalyzed);

        }



    }
}
