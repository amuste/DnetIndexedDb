using DnetIndexedDb.Models;
using System.Collections.Generic;

namespace DnetIndexedDb.Fluent
{
    public static class IndexedDbStoreExtensions
    {

        // Key Creation

        /// <summary>
        /// Add non-auto-incrementing key to store.
        /// </summary>
        /// <param name="store">IndexedDb Store</param>
        /// <param name="name"></param>
        /// <returns>Given IndexedDbStore Instance</returns>
        public static IndexedDbStore WithKey(this IndexedDbStore store, string name)
        {
            return store.CreateKey(name, autoIncrement: false);
        }

        /// <summary>
        /// Add auto-incrementing key to store.
        /// </summary>
        /// <param name="store">IndexedDb Store</param>
        /// <param name="name"></param>
        /// <returns>Given IndexedDbStore Instance</returns>
        public static IndexedDbStore WithAutoIncrementingKey(this IndexedDbStore store, string name)
        {
            return store.CreateKey(name, autoIncrement:true);
        }

        private static IndexedDbStore CreateKey(this IndexedDbStore store, string name, bool autoIncrement)
        {
            store.Key = new IndexedDbStoreParameter
            {
                KeyPath = name.ToCamelCase(),
                AutoIncrement = autoIncrement
            };

            return store;
        }

        // Index Creation

        /// <summary>
        /// Add unique index to store.
        /// </summary>
        /// <param name="store">IndexedDb Store</param>
        /// <param name="name"></param>
        /// <returns>Given IndexedDbStore Instance</returns>
        public static IndexedDbStore AddUniqueIndex(this IndexedDbStore store, string name)
        {
            return store.CreateIndex(name, new IndexedDbIndexParameter() { Unique = true });
        }

        /// <summary>
        /// Add non-unique index to store.
        /// </summary>
        /// <param name="store">IndexedDb Store</param>
        /// <param name="name"></param>
        /// <returns>Given IndexedDbStore Instance</returns>
        public static IndexedDbStore AddIndex(this IndexedDbStore store, string name)
        {
            return store.CreateIndex(name, new IndexedDbIndexParameter() { Unique = false });
        }

        private static IndexedDbStore CreateIndex(this IndexedDbStore store, string name, IndexedDbIndexParameter definition)
        {
            if (store.Indexes == null)
            {
                store.Indexes = new List<IndexedDbIndex>();
            }

            var index = new IndexedDbIndex();
            index.Name = name.ToCamelCase();
            index.Definition = definition;

            store.Indexes.Add(index);

            return store;
        }

        public static string ToCamelCase(this string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > 1)
            {
                return char.ToLowerInvariant(str[0]) + str.Substring(1);
            }
            return str;
        }
    }
}
