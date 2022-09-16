using System;
using System.Collections.Generic;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using DnetIndexedDb.Models;
using System.Text.Json;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;

namespace DnetIndexedDb
{
    public class IndexedDbInterop
    {
        private const string MaxExtent = "Max";
        private const string MinExtent = "Min";
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

        /// <summary>
        /// Create, Open or Upgrade IndexedDb Database
        /// </summary>
        /// <returns></returns>
        public async ValueTask<int> OpenIndexedDb()
        {
            var dbModelId = await _jsRuntime.InvokeAsync<int>("dnetindexeddbinterop.openDb", _indexedDbDatabaseModel);

            if (dbModelId != -1) _indexedDbDatabaseModel.DbModelId = dbModelId;

            return dbModelId;
        }

        /// <summary>
        /// Delete IndexedDb Database
        /// </summary>
        /// <returns></returns>
        public async ValueTask<string> DeleteIndexedDb()
        {
            return await _jsRuntime.InvokeAsync<string>("dnetindexeddbinterop.deleteDb", _indexedDbDatabaseModel);
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct AddBlobStruct
        {
            [FieldOffset(0)]
            public string DbModelGuid;

            [FieldOffset(8)]
            public string objectStoreName;

            [FieldOffset(16)]
            public string key;

            [FieldOffset(24)]
            public string mimeType;
        }

        /// <summary>
        /// Add records to a given data store
        /// </summary>
        /// <typeparam name="TEntity">Type of Objects in Data Store</typeparam>
        /// <param name="objectStoreName"></param>
        /// <param name="item"></param>
        /// <param name="key"></param>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        public string AddBlobItem<TEntity>(string objectStoreName, TEntity item, string mimeType, string key = "")
        {
            var addblob = new AddBlobStruct
            {
                DbModelGuid = _indexedDbDatabaseModel.DbModelGuid,
                objectStoreName = objectStoreName,
                key = key,
                mimeType = mimeType
            };
            var unmarshalledRuntime = (IJSUnmarshalledRuntime)_jsRuntime;
            return unmarshalledRuntime.InvokeUnmarshalled<AddBlobStruct, TEntity, string> ("dnetindexeddbinterop.addBlobItem", addblob, item);
        }

        /// <summary>
        /// Add records to a given data store
        /// </summary>
        /// <typeparam name="TEntity">Type of Objects in Data Store</typeparam>
        /// <param name="objectStoreName"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public async ValueTask<string> AddItems<TEntity>(string objectStoreName, List<TEntity> items)
        {
            return await _jsRuntime.InvokeAsync<string>("dnetindexeddbinterop.addItems", _indexedDbDatabaseModel, objectStoreName, items);
        }

        /// <summary>
        /// Add records to a data store matching TEntity.Name
        /// </summary>
        /// <typeparam name="TEntity">Type of Objects in Data Store</typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public async ValueTask<string> AddItems<TEntity>(List<TEntity> items)
        {
            return await _jsRuntime.InvokeAsync<string>("dnetindexeddbinterop.addItems", _indexedDbDatabaseModel, typeof(TEntity).Name, items);
        }

        /// <summary>
        /// Update records in a given data store
        /// </summary>
        /// <typeparam name="TEntity">Type of Objects in Data Store</typeparam>
        /// <param name="objectStoreName"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public async ValueTask<string> UpdateItems<TEntity>(string objectStoreName, List<TEntity> items)
        {
            return await _jsRuntime.InvokeAsync<string>("dnetindexeddbinterop.updateItems", _indexedDbDatabaseModel, objectStoreName, items);
        }

        /// <summary>
        /// Update records in a data store matching TEntity.Name
        /// </summary>
        /// <typeparam name="TEntity">Type of Objects in Data Store</typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public async ValueTask<string> UpdateItems<TEntity>(List<TEntity> items)
        {
            return await _jsRuntime.InvokeAsync<string>("dnetindexeddbinterop.updateItems", _indexedDbDatabaseModel, typeof(TEntity).Name, items);
        }

        /// <summary>
        /// Update records in a given data store by key
        /// </summary>
        /// <typeparam name="TEntity">Type of Objects in Data Store</typeparam>
        /// <param name="objectStoreName"></param>
        /// <param name="items"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public async ValueTask<string> UpdateItemsByKey<TEntity>(string objectStoreName, List<TEntity> items, List<int> keys)
        {
            return await _jsRuntime.InvokeAsync<string>("dnetindexeddbinterop.updateItemsByKey", _indexedDbDatabaseModel, objectStoreName, items, keys);
        }

        /// <summary>
        /// Update records in a data store matching TEntity.Name by key
        /// </summary>
        /// <typeparam name="TEntity">Type of Objects in Data Store</typeparam>
        /// <param name="items"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public async ValueTask<string> UpdateItemsByKey<TEntity>(List<TEntity> items, List<int> keys)
        {
            return await _jsRuntime.InvokeAsync<string>("dnetindexeddbinterop.updateItemsByKey", _indexedDbDatabaseModel, typeof(TEntity).Name, items, keys);
        }

        /// <summary>
        /// Return a record in a given data store by its key
        /// </summary>
        /// <typeparam name="TEntity">Type of Objects in Data Store</typeparam>
        /// <typeparam name="TKey">Type of Key Field</typeparam>
        /// <param name="key"></param>
        /// <param name="objectStoreName"></param>
        /// <returns></returns>
        public async ValueTask<TEntity> GetByKey<TKey, TEntity>(string objectStoreName, TKey key)
        {
            return await _jsRuntime.InvokeAsync<TEntity>("dnetindexeddbinterop.getByKey", _indexedDbDatabaseModel, objectStoreName, key);
        }


        /// <summary>
        /// Add records to a given data store
        /// </summary>
        /// <typeparam name="TEntity">Type of Objects in Data Store</typeparam>
        /// <param name="objectStoreName"></param>
        /// <param name="item"></param>
        /// <param name="key"></param>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        public string UpdateBlobByKey<TEntity>(string objectStoreName, TEntity item, string mimeType, string key)
        {
            var addblob = new AddBlobStruct
            {
                DbModelGuid = _indexedDbDatabaseModel.DbModelGuid,
                objectStoreName = objectStoreName,
                key = key,
                mimeType = mimeType
            };
            var unmarshalledRuntime = (IJSUnmarshalledRuntime)_jsRuntime;
            return unmarshalledRuntime.InvokeUnmarshalled<AddBlobStruct, TEntity, string>("dnetindexeddbinterop.updateBlobItem", addblob, item);
        }


        /// <summary>
        /// Return a record in a given data store by its key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="objectStoreName"></param>
        /// <returns></returns>
        public async ValueTask<string> GetBlobByKey(string objectStoreName, string key)
        {
            return await _jsRuntime.InvokeAsync<string>("dnetindexeddbinterop.getBlobByKey", _indexedDbDatabaseModel, objectStoreName, key);
        }

        /// <summary>
        /// Return a record in a given data store by its key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="objectStoreName"></param>
        /// <param name="destination"></param>
        /// <param name="maxBytes"></param>
        /// <returns></returns>
        //[Obsolete("This function is only compatible with .NET 5")]
        //public async ValueTask<int> GetBlobByKeyNet5(string objectStoreName, string key, byte[] destination, int maxBytes)
        //{
        //    //return await _jsRuntime.InvokeAsync<string>("dnetindexeddbinterop.getBlobByKey2", _indexedDbDatabaseModel, objectStoreName, key);

        //    var unmarshalledRuntime = (IJSUnmarshalledRuntime)_jsRuntime;

        //    var bytesReturned = new byte[4];
        //    var getblob = new GetBlobStruct
        //    {
        //        DbModelGuid = _indexedDbDatabaseModel.DbModelGuid,
        //        Destination = destination,
        //        MaxBytes = maxBytes,
        //        Key = key,
        //        ObjectStoreName = objectStoreName,
        //        BytesReturned = bytesReturned
        //    };


        //    var res= unmarshalledRuntime.InvokeUnmarshalled<GetBlobStruct, int>("dnetindexeddbinterop.getBlobByKey2", getblob);

        //    // invoke umarshalled seems to return immediately, wait for result to get written
        //    await Task.Delay(3000);
        //    while (bytesReturned[0] == 0 && bytesReturned[1] == 0 && bytesReturned[2] == 0 && bytesReturned[3] == 0)
        //    {
        //        await Task.Delay(10);
        //    }
        //    var b1 = getblob.Destination[0];
        //    var b2 = getblob.Destination[1];
        //    var b3 = getblob.Destination[2];
        //    var b4 = getblob.Destination[3];
            
        //    if (BitConverter.IsLittleEndian) Array.Reverse(getblob.BytesReturned);
        //    var br = BitConverter.ToInt32(getblob.BytesReturned, 0);
        //    return br;
        //}


        /// <summary>
        /// Return a record in a given data store by its key
        /// 
        /// This function ONLY works in .NET 6+
        /// https://docs.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/6.0/byte-array-interop
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="objectStoreName"></param>
        /// <returns></returns>
        public async ValueTask<byte[]> GetBlobByKey(string objectStoreName, string key, int maxBytes)
        {
            var res = await _jsRuntime.InvokeAsync<byte[]>("dnetindexeddbinterop.getBlobByKey3", _indexedDbDatabaseModel, objectStoreName, key, maxBytes);
            return res;
        }



        [StructLayout(LayoutKind.Explicit)]
        public struct InteropStruct
        {
            [FieldOffset(0)]
            public string Name;

            [FieldOffset(8)]
            public int Year;
        }

 


        [StructLayout(LayoutKind.Explicit)]
        private struct GetBlobStruct
        {
            [FieldOffset(0)]
            public string DbModelGuid;

            [FieldOffset(8)]
            public string ObjectStoreName;

            [FieldOffset(16)]
            public string Key;

            [FieldOffset(24)]
            public byte[] Destination;

            [FieldOffset(28)]
            public int MaxBytes;

            [FieldOffset(32)]
            public byte[] BytesReturned;

        }


        /// <summary>
        /// Directly references blob from html element without having to marshall it into .NET
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="objectStoreName"></param>
        /// <param name="key"></param>
        /// <param name="elementId"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public async ValueTask<string> AssignBlobToElement(string objectStoreName, string key, string elementId, string attribute)
        {
            return await _jsRuntime.InvokeAsync<string>("dnetindexeddbinterop.assignBlobToElement", _indexedDbDatabaseModel, objectStoreName, key, elementId, attribute);
        }


        /// <summary>
        /// Directly updates blob from html element without having to marshall it into .NET
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="objectStoreName"></param>
        /// <param name="key"></param>
        /// <param name="elementId"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public async ValueTask<string> UpdateBlobFromElement(string objectStoreName, string key, string elementId, string attribute)
        {
            return await _jsRuntime.InvokeAsync<string>("dnetindexeddbinterop.updateBlobFromElement", _indexedDbDatabaseModel, objectStoreName, key, elementId, attribute);
        }


        /// <summary>
        /// Return a record in a data store matching TEntity.Name by key
        /// </summary>
        /// <typeparam name="TEntity">Type of Objects in Data Store</typeparam>
        /// <typeparam name="TKey">Type of Key Field</typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async ValueTask<TEntity> GetByKey<TKey, TEntity>(TKey key)
        {
            return await _jsRuntime.InvokeAsync<TEntity>("dnetindexeddbinterop.getByKey", _indexedDbDatabaseModel, typeof(TEntity).Name, key);
        }

        /// <summary>
        /// Delete a record in a given data store by its key
        /// </summary>
        /// <typeparam name="TKey">Type of Key Field</typeparam>
        /// <param name="key"></param>
        /// <param name="objectStoreName"></param>
        /// <returns></returns>
        public async ValueTask<string> DeleteByKey<TKey>(string objectStoreName, TKey key)
        {
            return await _jsRuntime.InvokeAsync<string>("dnetindexeddbinterop.deleteByKey", _indexedDbDatabaseModel, objectStoreName, key);
        }

        /// <summary>
        /// Delete a record in a data store matching TEntity.Name by key
        /// </summary>
        /// <typeparam name="TKey">Type of Key Field</typeparam>
        /// <typeparam name="TEntity">Type of Objects in Data Store</typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async ValueTask<string> DeleteByKey<TKey, TEntity>(TKey key)
        {
            return await _jsRuntime.InvokeAsync<string>("dnetindexeddbinterop.deleteByKey", _indexedDbDatabaseModel, typeof(TEntity).Name, key);
        }

        /// <summary>
        /// Delete all records in a given data store
        /// </summary>
        /// <param name="objectStoreName"></param>
        /// <returns></returns>
        public async ValueTask<string> DeleteAll(string objectStoreName)
        {
            return await _jsRuntime.InvokeAsync<string>("dnetindexeddbinterop.deleteAll", _indexedDbDatabaseModel, objectStoreName);
        }

        /// <summary>
        /// Delete all records in a data store matching TEntity.Name
        /// </summary>
        /// <typeparam name="TEntity">Type of Objects in Data Store</typeparam>
        /// <returns></returns>
        public async ValueTask<string> DeleteAll<TEntity>()
        {
            return await _jsRuntime.InvokeAsync<string>("dnetindexeddbinterop.deleteAll", _indexedDbDatabaseModel, typeof(TEntity).Name);
        }

        /// <summary>
        /// Return all records in a given data store
        /// </summary>
        /// <typeparam name="TEntity">Type of Objects in Data Store</typeparam>
        /// <param name="objectStoreName"></param>
        /// <returns></returns>
        public async ValueTask<List<TEntity>> GetAll<TEntity>(string objectStoreName)
        {
            return await _jsRuntime.InvokeAsync<List<TEntity>>("dnetindexeddbinterop.getAll", _indexedDbDatabaseModel, objectStoreName);
        }

        /// <summary>
        /// Return all records in a data store matching TEntity.Name
        /// </summary>
        /// <typeparam name="TEntity">Type of Objects in Data Store</typeparam>
        /// <returns></returns>
        public async ValueTask<List<TEntity>> GetAll<TEntity>()
        {
            return await _jsRuntime.InvokeAsync<List<TEntity>>("dnetindexeddbinterop.getAll", _indexedDbDatabaseModel, typeof(TEntity).Name);
        }

        /// <summary>
        /// Return some records in a given data store by key
        /// </summary>
        /// <typeparam name="TEntity">Type of Objects in Data Store</typeparam>
        /// <typeparam name="TKey">Type of Key Field</typeparam>
        /// <param name="objectStoreName"></param>
        /// <param name="lowerBound"></param>
        /// <param name="upperBound"></param>
        /// <returns></returns>
        public async ValueTask<List<TEntity>> GetRange<TKey, TEntity>(string objectStoreName, TKey lowerBound, TKey upperBound)
        {
            return await _jsRuntime.InvokeAsync<List<TEntity>>("dnetindexeddbinterop.getRange", _indexedDbDatabaseModel, objectStoreName, lowerBound, upperBound);
        }

        /// <summary>
        /// Return some records in a data store matching TEntity.Name by key
        /// </summary>
        /// <typeparam name="TEntity">Type of Objects in Data Store</typeparam>
        /// <typeparam name="TKey">Type of Key Field</typeparam>
        /// <param name="lowerBound"></param>
        /// <param name="upperBound"></param>
        /// <returns></returns>
        public async ValueTask<List<TEntity>> GetRange<TKey, TEntity>(TKey lowerBound, TKey upperBound)
        {
            return await _jsRuntime.InvokeAsync<List<TEntity>>("dnetindexeddbinterop.getRange", _indexedDbDatabaseModel, typeof(TEntity).Name, lowerBound, upperBound);
        }

        /// <summary>
        /// Return some records in a given data store by index
        /// </summary>
        /// <typeparam name="TKey">Type of Key Field</typeparam>
        /// <typeparam name="TEntity">Type of Objects in Data Store</typeparam>
        /// <param name="objectStoreName"></param>
        /// <param name="lowerBound"></param>
        /// <param name="upperBound"></param>
        /// <param name="dbIndex"></param>
        /// <param name="isRange"></param>
        /// <returns></returns>
        public async ValueTask<List<TEntity>> GetByIndex<TKey, TEntity>(string objectStoreName, TKey lowerBound, TKey upperBound, string dbIndex, bool isRange)
        {
            return await _jsRuntime.InvokeAsync<List<TEntity>>("dnetindexeddbinterop.getByIndex", _indexedDbDatabaseModel, objectStoreName, lowerBound, upperBound, dbIndex, isRange);
        }

        /// <summary>
        /// Return some records in a data store matching TEntity.Name by index
        /// </summary>
        /// <typeparam name="TKey">Type of Key Field</typeparam>
        /// <typeparam name="TEntity">Type of Objects in Data Store</typeparam>
        /// <param name="lowerBound"></param>
        /// <param name="upperBound"></param>
        /// <param name="dbIndex"></param>
        /// <param name="isRange"></param>
        /// <returns></returns>
        public async ValueTask<List<TEntity>> GetByIndex<TKey, TEntity>(TKey lowerBound, TKey upperBound, string dbIndex, bool isRange)
        {
            return await _jsRuntime.InvokeAsync<List<TEntity>>("dnetindexeddbinterop.getByIndex", _indexedDbDatabaseModel, typeof(TEntity).Name, lowerBound, upperBound, dbIndex, isRange);
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
            return await GetExtent<TIndex>(objectStoreName, dbIndex, MaxExtent);
        }

        /// <summary>
        /// Returns the max value in a data store matching TEntity.Name by index
        /// </summary>
        /// <typeparam name="TIndex"></typeparam>
        /// <typeparam name="TEntity">Type of Objects in Data Store</typeparam>
        /// <param name="dbIndex"></param>
        /// <returns></returns>
        public async ValueTask<TIndex> GetMaxIndex<TIndex, TEntity>(string dbIndex)
        {
            return await GetExtent<TIndex>(typeof(TEntity).Name, dbIndex, MaxExtent);
        }

        /// <summary>
        /// Returns the max value in the given data store's key
        /// </summary>
        /// <typeparam name="TKey">Type of Key Field</typeparam>
        /// <param name="objectStoreName"></param>
        /// <returns></returns>
        public async ValueTask<TKey> GetMaxKey<TKey>(string objectStoreName)
        {
            return await GetExtent<TKey>(objectStoreName, null, MaxExtent);
        }

        /// <summary>
        /// Returns the max value in a data store matching TEntity.Name 
        /// </summary>
        /// <typeparam name="TKey">Type of Key Field</typeparam>
        /// <typeparam name="TEntity">Type of Objects in Data Store</typeparam>
        /// <returns></returns>
        public async ValueTask<TKey> GetMaxKey<TKey, TEntity>()
        {
            return await GetExtent<TKey>(typeof(TEntity).Name, null, MaxExtent);
        }

        /// <summary>
        /// Returns the minimum value in the given data store's index by key 
        /// </summary>
        /// <typeparam name="TIndex"></typeparam>
        /// <param name="objectStoreName"></param>
        /// <param name="dbIndex"></param>
        /// <returns></returns>
        public async ValueTask<TIndex> GetMinIndex<TIndex>(string objectStoreName, string dbIndex)
        {
            return await GetExtent<TIndex>(objectStoreName, dbIndex, MinExtent);
        }

        /// <summary>
        /// Returns the minimum value in a data store matching TEntity.Name by index
        /// </summary>
        /// <typeparam name="TIndex"></typeparam>
        /// <typeparam name="TEntity">Type of Objects in Data Store</typeparam>
        /// <param name="dbIndex"></param>
        /// <returns></returns>
        public async ValueTask<TIndex> GetMinIndex<TIndex, TEntity>(string dbIndex)
        {
            return await GetExtent<TIndex>(typeof(TEntity).Name, dbIndex, MinExtent);
        }

        /// <summary>
        /// Returns the minimum value in the given data store's key 
        /// </summary>
        /// <typeparam name="TKey">Type of Key Field</typeparam>
        /// <param name="objectStoreName"></param>
        /// <returns></returns>
        public async ValueTask<TKey> GetMinKey<TKey>(string objectStoreName)
        {
            return await GetExtent<TKey>(objectStoreName, null, MinExtent);
        }

        /// <summary>
        /// Returns the minimum value in a data store matching TEntity.Name by key 
        /// </summary>
        /// <typeparam name="TKey">Type of Key Field</typeparam>
        /// <typeparam name="TEntity">Type of Objects in Data Store</typeparam>
        /// <returns></returns>
        public async ValueTask<TKey> GetMinKey<TKey, TEntity>()
        {
            return await GetExtent<TKey>(typeof(TEntity).Name, null, MinExtent);
        }

        private async ValueTask<T> GetExtent<T>(string objectStoreName, string dbIndex, string extentType)
        {
            var result = await _jsRuntime.InvokeAsync<JsonElement>("dnetindexeddbinterop.getExtent", _indexedDbDatabaseModel, objectStoreName, dbIndex, extentType);

            if (result.ValueKind == JsonValueKind.Null)
            {
                return default;
            }
            else
            {
                return JsonSerializer.Deserialize<T>(result.GetRawText());
            }
        }
    }
}
