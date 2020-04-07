# DnetIndexedDb
This is a Blazor library for work with IndexedDB DOM API. It allows query multiple IndexedDb databases simultaneously 

The actual API expose the following methods:

* Open and upgrade an instance of IndexedDB
* Close an instance of IndexedDB
* Delete an instance of IndexedDB
* Add an item to a given store
* Update an item in a given store
* Delete an item from a store by key
* Delete all items from a store
* Retrieve all items from a given store
* Retrieve an items by index
* Retrieve a range of items by key
* Retrieve a range of items by index

Compatibility
*Server-side Blazor and client-side Blazor

## Using the library

1. Install the Nuget package DnetIndexedDb
2. Add the following script reference to your Index.html after the blazor.webassembly.js reference: 
```<script src="_content/DnetIndexedDb/rxjs.min.js"></script>```
```<script src="_content/DnetIndexedDb/dnet-indexeddb.js"></script>```
2. create a new instance of ```IndexedDbDatabaseModel```
3. Create a derive class from ```IndexedDbInterop```
```CSharp
  public class GridColumnDataIndexedDb : IndexedDbInterop
  {
    public GridColumnDataIndexedDb(IJSRuntime jsRuntime, IndexedDbOptions<GridColumnDataIndexedDb> options) 
    :base(jsRuntime, options)
    {
    }
  }
```
5. Register your derive class in ```ConfiguredServices``` on ```Startup.cs```. When register use options to add the IndexedDbDatabaseModel instance.
```CSharp
  services.AddIndexedDbDatabase<GridColumnDataIndexedDb>(options =>
  {
    options.UseDatabase(GetGridColumnDatabaseModel());
  });
```
6. Inject the created instance of your derived class into the component or page.


### Detailed configuration and use

#### Step 1 - Create the database model
To create the database model use the following classes. You can find more info about the IndexedDB database here: https://www.w3.org/TR/IndexedDB-2/#database-construct

```IndexedDbDatabase```
```IndexedDbDatabaseModel```
```IndexedDbIndex```
```IndexedDbStore```
```IndexedDbStoreParameter```

See the example below
```CSharp
 var indexedDbDatabaseModel = new IndexedDbDatabaseModel
            {
                Name = "GridColumnData",
                Version = 1,
                Stores = new List<IndexedDbStore>
                {
                new IndexedDbStore
                {
                    Name = "tableField",
                    Key = new IndexedDbStoreParameter
                    {
                        KeyPath = "tableFieldId",
                        AutoIncrement = true
                    },
                    Indexes = new List<IndexedDbIndex>
                    {
                        new IndexedDbIndex
                        {
                            Name = "tableFieldId",
                            Definition = new IndexedDbIndexParameter
                            {
                                Unique = true
                            }
                        },
                        new IndexedDbIndex
                        {
                            Name = "attachedProperty",
                            Definition = new IndexedDbIndexParameter
                            {
                                Unique = false
                            }
                        },
                        new IndexedDbIndex
                        {
                            Name = "fieldVisualName",
                            Definition = new IndexedDbIndexParameter
                            {
                                Unique = false
                            }
                        }, new IndexedDbIndex
                        {
                            Name = "hide",
                            Definition = new IndexedDbIndexParameter
                            {
                                Unique = false
                            }
                        },
                        new IndexedDbIndex
                        {
                            Name = "isLink",
                            Definition = new IndexedDbIndexParameter
                            {
                                Unique = false
                            }
                        },
                        new IndexedDbIndex
                        {
                            Name = "memberOf",
                            Definition = new IndexedDbIndexParameter
                            {
                                Unique = false
                            }
                        },
                        new IndexedDbIndex
                        {
                            Name = "tableName",
                            Definition = new IndexedDbIndexParameter
                            {
                                Unique = false
                            }
                        },
                        new IndexedDbIndex
                        {
                            Name = "textAlignClass",
                            Definition = new IndexedDbIndexParameter
                            {
                                Unique = false
                            }
                        },
                        new IndexedDbIndex
                        {
                            Name = "type",
                            Definition = new IndexedDbIndexParameter
                            {
                                Unique = false
                            }
                        },
                        new IndexedDbIndex
                        {
                            Name = "width",
                            Definition = new IndexedDbIndexParameter
                            {
                                Unique = false
                            }
                        }
                    }
                }
            },
                DbModelId = 0
            };
```

#### Step 2 - Creating a service
You can create a service for any indexedDB's database that you want to use in your application. Use the base class ```IndexedDbInterop``` to create your derived class. See code below.

```CSharp
  public class GridColumnDataIndexedDb : IndexedDbInterop
  {
    public GridColumnDataIndexedDb(IJSRuntime jsRuntime, IndexedDbOptions<GridColumnDataIndexedDb> options) 
    :base(jsRuntime, options)
    {
    }
  }
```

This follows a similar pattern to what you do when you create a new DbContext in EF core. 

#### Step 3 - Registering a service
You can use the ```AddIndexedDbDatabase``` extension method to register your service in ```ConfiguredServices``` on ```Startup.cs```. Use the ```options``` builder to add the service Database. See code below.

```CSharp
  services.AddIndexedDbDatabase<GridColumnDataIndexedDb>(options =>
  {
    options.UseDatabase(GetGridColumnDatabaseModel());
  });
```

```GetGridColumnDatabaseModel()``` return an instance of ```IndexedDbDatabaseModel```

You can also use multiple database, declaring as many service as you want.
```CSharp
   services.AddIndexedDbDatabase<SecuritySuiteDataIndexedDb>(options =>
   {
     options.UseDatabase(GetSecurityDatabaseModel());
   });
```

## Using the service in your application

To use IndexedDB service in a component or page first inject the IndexedDB servicer instance.

```CSharp
@inject GridColumnDataIndexedDb GridColumnDataIndexedDb
``` 

IndexedDB store are the equivalent of table in SQL Database. For the API demostrations will use the following class as our store model.

```CSharp
    public class TableFieldDto
    {
        public int TableFieldId { get; set; }

        public string TableName { get; set; }

        public string FieldVisualName { get; set; }

        public string AttachedProperty { get; set; }

        public bool IsLink { get; set; }

        public int MemberOf { get; set; }

        public int Width { get; set; }

        public string TextAlignClass { get; set; }

        public bool Hide { get; set; }

        public string Type { get; set; }
    }
```


### API examples

### Open and upgrade an instance of IndexedDB

```ValueTask<int> OpenIndexedDb()```

```CSharp
 var result = await GridColumnDataIndexedDb.OpenIndexedDb();
```
 
### Add an items to a given store

```ValueTask<TEntity> AddItems<TEntity>(string objectStoreName, List<TEntity> items)```

```CSharp
 var result = await GridColumnDataIndexedDb.AddItems<TableFieldDto>("tableField", items);
```

### Retrieve an item by key

```ValueTask<TEntity> GetByKey<TKey, TEntity>(string objectStoreName, TKey key)```

```CSharp
 var result = await GridColumnDataIndexedDb.GetByKey<int, TableFieldDto>("tableField", 11);
```

### Delete an item from a store by key

```ValueTask<string> DeleteByKey<TKey>(string objectStoreName, TKey key)```

```CSharp
 var result = await GridColumnDataIndexedDb.DeleteByKey<int>("tableField", 11);
```

### Retrieve all items from a given store

```ValueTask<List<TEntity>> GetAll<TEntity>(string objectStoreName)```

```CSharp
  var result = await GridColumnDataIndexedDb.GetAll<TableFieldDto>("tableField");
```

### Retrieve a range of items by key

```ValueTask<List<TEntity>> GetRange<TKey, TEntity>(string objectStoreName, TKey lowerBound, TKey upperBound)```

```CSharp
   var result = await GridColumnDataIndexedDb.GetRange<int, TableFieldDto>("tableField", 15, 20);
```

### Retrieve a range of items by index

```ValueTask<List<TEntity>> GetByIndex<TKey, TEntity>(string objectStoreName, TKey lowerBound, TKey upperBound, string dbIndex, bool isRange)```

```CSharp
    var result = await GridColumnDataIndexedDb.GetByIndex<string, TableFieldDto>("tableField", "Name", null, "fieldVisualName", false);
```

### Update items in a given store

```ValueTask<string> UpdateItems<TEntity>(string objectStoreName, List<TEntity> items)```

```CSharp
    foreach (var item in items)
    {
        item.FieldVisualName = item.FieldVisualName + "Updated";
    }
   var result = await GridColumnDataIndexedDb.UpdateItems<TableFieldDto>("tableField", items);
```

### Delete all items from a store

```ValueTask<string> DeleteAll(string objectStoreName)```

```CSharp
  var result = await GridColumnDataIndexedDb.DeleteAll("tableField");
```

### Delete an instance of IndexedDB

```ValueTask<string> DeleteIndexedDb()```

```CSharp
  var result = await GridColumnDataIndexedDb.DeleteIndexedDb();
```
 
 
