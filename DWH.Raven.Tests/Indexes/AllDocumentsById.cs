using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace DWH.Raven.Tests.Indexes
{
    public class AllDocumentsById : AbstractIndexCreationTask
    {
        public const string TheIndexName = "AllDocumentsById";

        public override string IndexName
        {
            get { return TheIndexName; }
        }

        public override IndexDefinition CreateIndexDefinition()
        {
            return new IndexDefinition
            {
                Name = TheIndexName,
                Map = "from doc in docs let DocId = doc[\"@metadata\"][\"@id\"] select new {DocId};"
            };
        }
    }
}
