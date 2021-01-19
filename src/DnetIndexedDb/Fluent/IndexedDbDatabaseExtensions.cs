using DnetIndexedDb.Models;
using System.Collections.Generic;

namespace DnetIndexedDb.Fluent
{
    public static class IndexedDbDatabaseExtensions
    {
        public static IndexedDbDatabaseModel WithName(this IndexedDbDatabaseModel model, string name)
        {
            model.Name = name;
            return model;
        }

        public static IndexedDbDatabaseModel WithVersion(this IndexedDbDatabaseModel model, int version)
        {
            model.Version = version;
            return model;
        }

        public static IndexedDbDatabaseModel WithModelId(this IndexedDbDatabaseModel model, int id)
        {
            model.DbModelId = id;
            return model;
        }


        public static IndexedDbStore AddStore(this IndexedDbDatabaseModel model, string name)
        {
            if (model.Stores == null)
            {
                model.Stores = new List<IndexedDbStore>();
            }

            var store = new IndexedDbStore();
            store.Name = name;

            model.Stores.Add(store);

            return store;
        }
    }
}
