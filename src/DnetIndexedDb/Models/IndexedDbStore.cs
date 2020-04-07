using System.Collections.Generic;

namespace DnetIndexedDb.Models
{
    public class IndexedDbStore
    {
        public string Name { get; set; }

        public IndexedDbStoreParameter Key { get; set; }

        public List<IndexedDbIndex> Indexes { get; set; }
    }
}
