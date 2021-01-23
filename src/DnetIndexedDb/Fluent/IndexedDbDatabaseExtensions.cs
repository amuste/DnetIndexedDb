using DnetIndexedDb.Models;
using System.Collections.Generic;

namespace DnetIndexedDb.Fluent
{
    public static class IndexedDbDatabaseExtensions
    {
        /// <summary>
        /// Sets Name of Database Model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IndexedDbDatabaseModel WithName(this IndexedDbDatabaseModel model, string name)
        {
            model.Name = name;
            return model;
        }

        /// <summary>
        /// Sets Version of Database Model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static IndexedDbDatabaseModel WithVersion(this IndexedDbDatabaseModel model, int version)
        {
            model.Version = version;
            return model;
        }

        /// <summary>
        /// Sets Database ModelId. Defaults to 0
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static IndexedDbDatabaseModel WithModelId(this IndexedDbDatabaseModel model, int id)
        {
            model.DbModelId = id;
            return model;
        }

        /// <summary>
        /// Creates a new IndexedDbStore and adds it to the Database Model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="name"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Adds a new store named after <T> and adds Key and Indexes based on IndexedDbKey and IndexedDbIndex Attributes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static IndexedDbStore AddStore<T>(this IndexedDbDatabaseModel model)
        {
            if (model.Stores == null)
            {
                model.Stores = new List<IndexedDbStore>();
            }

            var store = new IndexedDbStore();
            store.Name = typeof(T).Name;

            store.SetupFrom<T>();

            model.Stores.Add(store);

            return store;
        }
    }
}
