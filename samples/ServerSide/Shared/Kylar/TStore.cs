using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using DnetIndexedDb.Models;

namespace DnetIndexedDbServer.Shared.Kylar
{
    public class TStore<T> : IndexedDbStore where T : class
    {
        private IndexedDbStoreParameter _key;

        private readonly List<IndexedDbIndex> _indexes = new List<IndexedDbIndex>();

        public TStore()
        {
            BuildStore();
            Name = typeof(T).Name.ToPlural();
            Key = _key;
            Indexes = _indexes;
        }

        private void BuildStore()
        {
            foreach (PropertyInfo info in typeof(T).GetProperties())
            {
                var classId = info.Name.Substring(info.Name.Length - 2);

                var classAttrs = info.GetCustomAttributes(true);

                var keyAttr = classAttrs.Select(p => p as KeyAttribute).FirstOrDefault();

                if (classId.ToLower() == "Id" || keyAttr != null)
                {
                    _key = new IndexedDbStoreParameter
                    {
                        KeyPath = info.Name.ToCamelCase(),
                        AutoIncrement = true
                    };
                }

                _indexes.Add(new IndexedDbIndex
                {
                    Name = info.Name.ToCamelCase(),
                    Definition = new IndexedDbIndexParameter { Unique = false }
                });
            }
        }
    }
}
