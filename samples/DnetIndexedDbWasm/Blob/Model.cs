using DnetIndexedDb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnetIndexedDb.Blob
{
    public static class Model
    {
        public static IndexedDbDatabaseModel GetBlobDatabaseModel()
        {
            var indexedDbDatabaseModel = new IndexedDbDatabaseModel
            {
                Name = "Blob",
                Version = 6,
                Stores = new List<IndexedDbStore>
                {
                    new IndexedDbStore
                    {
                        Name = "BlobStore",
                        Key = new IndexedDbStoreParameter
                        {
                            //KeyPath = "Id"
                        },
                        Indexes = new List<IndexedDbIndex>
                        {
                        }
                    }
                },
                DbModelId = 6,
                UseKeyGenerator = false
            };

            return indexedDbDatabaseModel;
        }
    }
}
