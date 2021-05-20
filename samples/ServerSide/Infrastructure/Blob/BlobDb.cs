using DnetIndexedDb;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnetIndexedDbServer.Infrastructure.Blob
{
    public class BlobDb : IndexedDbInterop
    {
        public BlobDb(IJSRuntime jsRuntime, IndexedDbOptions<BlobDb> options) : base(jsRuntime, options)
        {
        }
    }
}
