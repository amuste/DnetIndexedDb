using System;
using System.Collections.Generic;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using DnetIndexedDb.Models;

namespace DnetIndexedDb
{
    public class IndexedDbInterop
    {
        private readonly IJSRuntime _jsRuntime;

        private readonly IndexedDbOptions _indexedDbDatabaseOptions;

        private IndexedDbDatabaseModel _indexedDbDatabaseModel;

        public IndexedDbInterop(IJSRuntime jsRuntime, IndexedDbOptions indexedDbDatabaseOptions)
        {
            _jsRuntime = jsRuntime;
            _indexedDbDatabaseOptions = indexedDbDatabaseOptions;

            SetDbModel();
        }

        private void SetDbModel()
        {

            var option = _indexedDbDatabaseOptions.GetExtension<CoreOptionsExtension>();

            if (option != null)
            {
                _indexedDbDatabaseModel = option.IndexedDbDatabaseModel;

                _indexedDbDatabaseModel.DbModelGuid = Guid.NewGuid().ToString();
            }
            else
            {
                throw new NullReferenceException("IndexedDB Database Model not configured. Add one in AddIndexedDbDatabase method");
            }
        }

        public async ValueTask<int> OpenIndexedDb()
        {
            var dbModelId = await _jsRuntime.InvokeAsync<int>("dnetindexeddbinterop.openDb", _indexedDbDatabaseModel);

            if (dbModelId != -1) _indexedDbDatabaseModel.DbModelId = dbModelId;

            return dbModelId;
        }

        public async ValueTask<string> DeleteIndexedDb()
        {
            return await _jsRuntime.InvokeAsync<string>("dnetindexeddbinterop.deleteDb", _indexedDbDatabaseModel);
        }

        public async ValueTask<string> AddItems<TEntity>(string objectStoreName, List<TEntity> items)
        {
            return await _jsRuntime.InvokeAsync<string>("dnetindexeddbinterop.addItems", _indexedDbDatabaseModel, objectStoreName, items);
        }

        public async ValueTask<string> UpdateItems<TEntity>(string objectStoreName, List<TEntity> items)
        {
            return await _jsRuntime.InvokeAsync<string>("dnetindexeddbinterop.updateItems", _indexedDbDatabaseModel, objectStoreName, items);
        }

        public async ValueTask<string> UpdateItemsByKey<TEntity>(string objectStoreName, List<TEntity> items, List<int> keys)
        {
            return await _jsRuntime.InvokeAsync<string>("dnetindexeddbinterop.updateItemsByKey", _indexedDbDatabaseModel, objectStoreName, items, keys);
        }

        public async ValueTask<TEntity> GetByKey<TKey, TEntity>(string objectStoreName, TKey key)
        {
            return await _jsRuntime.InvokeAsync<TEntity>("dnetindexeddbinterop.getByKey", _indexedDbDatabaseModel, objectStoreName, key);
        }

        public async ValueTask<string> DeleteByKey<TKey>(string objectStoreName, TKey key)
        {
            return await _jsRuntime.InvokeAsync<string>("dnetindexeddbinterop.deleteByKey", _indexedDbDatabaseModel, objectStoreName, key);
        }

        public async ValueTask<string> DeleteAll(string objectStoreName)
        {
            return await _jsRuntime.InvokeAsync<string>("dnetindexeddbinterop.deleteAll", _indexedDbDatabaseModel, objectStoreName);
        }

        public async ValueTask<List<TEntity>> GetAll<TEntity>(string objectStoreName)
        {
            return await _jsRuntime.InvokeAsync<List<TEntity>>("dnetindexeddbinterop.getAll", _indexedDbDatabaseModel, objectStoreName);
        }

        public async ValueTask<List<TEntity>> GetRange<TKey, TEntity>(string objectStoreName, TKey lowerBound, TKey upperBound)
        {
            return await _jsRuntime.InvokeAsync<List<TEntity>>("dnetindexeddbinterop.getRange", _indexedDbDatabaseModel, objectStoreName, lowerBound, upperBound);
        }

        public async ValueTask<List<TEntity>> GetByIndex<TKey, TEntity>(string objectStoreName, TKey lowerBound, TKey upperBound, string dbIndex, bool isRange)
        {
            return await _jsRuntime.InvokeAsync<List<TEntity>>("dnetindexeddbinterop.getByIndex", _indexedDbDatabaseModel, objectStoreName, lowerBound, upperBound, dbIndex, isRange);
        }

        /// <summary>
        /// Returns the max value in the given data store's index
        /// </summary>
        /// <typeparam name="TIndex"></typeparam>
        /// <param name="objectStoreName"></param>
        /// <param name="dbIndex"></param>
        /// <returns></returns>
        public async ValueTask<TIndex> GetMaxIndex<TIndex>(string objectStoreName, string dbIndex)
        {
            return await _jsRuntime.InvokeAsync<TIndex>("dnetindexeddbinterop.getMaxIndex", _indexedDbDatabaseModel, objectStoreName, dbIndex);
        }

        /// <summary>
        /// Returns the max value in the given data store's key
        /// </summary>
        /// <typeparam name="TIndex"></typeparam>
        /// <param name="objectStoreName"></param>
        /// <returns></returns>
        public async ValueTask<TKey> GetMaxKey<TKey>(string objectStoreName)
        {
            return await _jsRuntime.InvokeAsync<TKey>("dnetindexeddbinterop.getMaxKey", _indexedDbDatabaseModel, objectStoreName);
        }

        /// <summary>
        /// Returns the minimum value in the given data store's index 
        /// </summary>
        /// <typeparam name="TIndex"></typeparam>
        /// <param name="objectStoreName"></param>
        /// <param name="dbIndex"></param>
        /// <returns></returns>
        public async ValueTask<TIndex> GetMinIndex<TIndex>(string objectStoreName, string dbIndex)
        {
            return await _jsRuntime.InvokeAsync<TIndex>("dnetindexeddbinterop.getMinIndex", _indexedDbDatabaseModel, objectStoreName, dbIndex);
        }

        /// <summary>
        /// Returns the minimum value in the given data store's key 
        /// </summary>
        /// <typeparam name="TIndex"></typeparam>
        /// <param name="objectStoreName"></param>
        /// <returns></returns>
        public async ValueTask<TKey> GetMinKey<TKey>(string objectStoreName)
        {
            return await _jsRuntime.InvokeAsync<TKey>("dnetindexeddbinterop.getMinKey", _indexedDbDatabaseModel, objectStoreName);
        }
    }
}
