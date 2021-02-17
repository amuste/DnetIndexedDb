using DnetIndexedDb.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

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
            return store.CreateKey(name, autoIncrement: true);
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

        private static string ToCamelCase(this string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > 1)
            {
                return char.ToLowerInvariant(str[0]) + str.Substring(1);
            }
            return str;
        }

        /// <summary>
        /// Adds Key and Indexes to Store based on IndexedDbKey and IndexedDbIndex Attributes on properties in Type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="store"></param>
        /// <returns></returns>
        public static IndexedDbStore SetupFrom<T>(this IndexedDbStore store)
        {
            store.Indexes = null;

            var type = typeof(T);
            var props = type.GetProperties();

            foreach (var prop in props)
            {
                var keyAttribute = prop.GetCustomAttribute<IndexDbKeyAttribute>();
                var indexAttribute = prop.GetCustomAttribute<IndexDbIndexAttribute>();

                if (keyAttribute is not null)
                {
                    store.CreateKey(prop.Name, keyAttribute.AutoIncrement);
                    store.CreateIndex(prop.Name, new IndexedDbIndexParameter() { Unique = keyAttribute.Unique });
                }

                if (indexAttribute is not null)
                {
                    store.CreateIndex(prop.Name, new IndexedDbIndexParameter() { Unique = indexAttribute.Unique });
                }
            }

            if(store.Key == null)
            {
                throw new System.Exception($"No IndexDbKey Found on Property Attributes in Class {type.Name}");
            }

            return store;
        }
    }
}
