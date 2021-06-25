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
        /// Sets Database UseKeyGenerator Property to true
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static IndexedDbDatabaseModel UseKeyGenerator(this IndexedDbDatabaseModel model)
        {
            model.UseKeyGenerator = true;
            return model;
        }

        /// <summary>
        /// Sets Database UseKeyGenerator Property to given value
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static IndexedDbDatabaseModel UseKeyGenerator(this IndexedDbDatabaseModel model, bool value)
        {
            model.UseKeyGenerator = value;
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
        /// Adds a new store named TEntity.Name and adds Key and Indexes based on IndexedDbKey and IndexedDbIndex Attributes
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static IndexedDbStore AddStore<TEntity>(this IndexedDbDatabaseModel model)
        {
            if (model.Stores == null)
            {
                model.Stores = new List<IndexedDbStore>();
            }

            var store = new IndexedDbStore();
            store.Name = typeof(TEntity).Name;

            store.SetupFrom<TEntity>();

            model.Stores.Add(store);

            return store;
        }

        /// <summary>
        /// Adds a new store and adds Key and Indexes based on IndexedDbKey and IndexedDbIndex Attributes
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="model"></param>
        /// <param name="storeName"></param>
        /// <returns></returns>
        public static IndexedDbStore AddStore<TEntity>(this IndexedDbDatabaseModel model, string storeName)
        {
            if (model.Stores == null)
            {
                model.Stores = new List<IndexedDbStore>();
            }

            var store = new IndexedDbStore();
            store.Name = storeName;

            store.SetupFrom<TEntity>();

            model.Stores.Add(store);

            return store;
        }
    }
}
