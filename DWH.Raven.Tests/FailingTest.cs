using System;
using System.Linq;
using System.Threading;
using DWH.Raven.Tests.Indexes;
using DWH.Raven.Tests.Models;
using NUnit.Framework;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using SharpTestsEx;

namespace DWH.Raven.Tests
{
    [TestFixture]
    public class FailingTest
    {



        private IDocumentStore _store;

        private void SetupDocumentStore()
        {
            _store = new DocumentStore()
            {
                ConnectionStringName = "RavenDB"
            }.Initialize();

            IndexCreation.CreateIndexes(GetType().Assembly, _store);

            _store.Conventions.CustomizeJsonSerializer =
                serializer =>
                {
                    serializer.Converters.Add(new HtmlStringConverter());
                    serializer.Converters.Add(new UriConverter());
                };
        }

        private void ClearAllDocuments()
        {
            WaitForNonStaleIndexes();
            _store.DatabaseCommands.DeleteByIndex(AllDocumentsById.TheIndexName, new IndexQuery());
        }

        private void AddDocuments()
        {
            using (var session = _store.OpenSession())
            {
                var school1 = new School()
                    {
                        SchoolDistrictName = "School District for School 1"
                    };
                session.Store(school1);

                var school2 = new School()
                    {
                        SchoolDistrictName = "School District for School 2"
                    };
                session.Store(school2);

                var comm1 = new Community()
                    {
                        Name = "Community 1",
                        SchoolIds = new[] {school1.Id, school2.Id}
                    };
                session.Store(comm1);

                var fp1 = new FloorPlan()
                    {
                        Community = comm1,
                        Name = "Floor Plan 1"
                    };
                session.Store(fp1);

                var fp2 = new FloorPlan()
                    {
                        Community = comm1,
                        Name = "Floor Plan 2"
                    };
                session.Store(fp2);

                var sc1 = new Showcase()
                    {
                        Community = comm1,
                        Name = "Showcase 1"
                    };
                session.Store(sc1);

                var sc2 = new Showcase()
                    {
                        Community = comm1,
                        Name = "Showcase 2"
                    };
                session.Store(sc2);

                session.SaveChanges();
            }
        }

        public void WaitForNonStaleIndexes()
        {
            Console.WriteLine("Waiting for indexes");
            while (_store.DatabaseCommands.GetStatistics().StaleIndexes.Length != 0)
                Thread.Sleep(10);
            Console.WriteLine("All indexes are caught up.");
        }


        [TestFixtureSetUp]
        public void Setup()
        {
            SetupDocumentStore();
            ClearAllDocuments();
            AddDocuments();
            WaitForNonStaleIndexes();
        }

        [Test]
        public void IndexHasNoMoreResultsThanCommunityCount()
        {
            using (var session = _store.OpenSession())
            {
                var communityCount = session.Query<Community>().Count();
                var indexResultCount = session.Query<CommunitySearchIndex.MapResult>(CommunitySearchIndex.TheIndexName)
                                              .ToList()
                                              .Count;

                indexResultCount.Should().Be.EqualTo(communityCount);
            }
        }

    }
}
