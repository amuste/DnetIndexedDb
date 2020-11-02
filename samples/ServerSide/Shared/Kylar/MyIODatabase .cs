using System.Collections.Generic;
using DnetIndexedDb.Models;
using DnetIndexedDbServer.Infrastructure.Entities;

namespace DnetIndexedDbServer.Shared.Kylar
{
    /// <summary>
    /// Sets which Objects will Be added to the Database
    /// </summary>
    public class MyIODatabase : IndexedDbDatabaseModel
    {
        public MyIODatabase()
        {
            Name = "GridColumnData";
            Version = 1;
            Stores = _stores;
        }

        /// <summary>
        /// List of Object Stores
        /// </summary>
        private List<IndexedDbStore> _stores => new List<IndexedDbStore>
        {
            _transferLogStore,
        };

        private IndexedDbStore _transferLogStore => new TStore<TableFieldDto>();
    }
}
