using System.Collections.Generic;

namespace DnetIndexedDb.Models
{
    public class IndexedDbDatabaseModel
    {
      
        public string Name { get; set; }
     
        public int Version { get; set; }
      
        public List<IndexedDbStore> Stores { get; set; } = new List<IndexedDbStore>();

        public int DbModelId { get; set; }

        public string DbModelGuid { get; set; }

        public bool UseKeyGenerator { get; set; } = false;
    }
}
