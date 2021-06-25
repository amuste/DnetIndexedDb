master - .Net5 version

master_net31 - 3.x version

# DnetIndexedDb
This is a Blazor library to work with IndexedDB DOM API. It allows query multiple IndexedDb databases simultaneously 

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
* Retrieve max key value
* Retrieve max index value
* Retrieve minimum key value
* Retrieve minimum index value

Compatibility
*Server-side Blazor and client-side Blazor

## Using the library

### Installation

1. Install the Nuget package DnetIndexedDb
2. Add the following script reference to your Index.html after the blazor.webassembly.js or blazor.server.js reference: 

```Html
<script src="_content/DnetIndexedDb/rxjs.min.js"></script>
<script src="_content/DnetIndexedDb/dnet-indexeddb.js"></script>
```

3. Create a derived class from ```IndexedDbInterop```.  
   a. This is the class you will use to interact with IndexedDb JavaScript for this database

```CSharp
  public class GridColumnDataIndexedDb : IndexedDbInterop
  {
    public GridColumnDataIndexedDb(IJSRuntime jsRuntime, IndexedDbOptions<GridColumnDataIndexedDb> options) 
    :base(jsRuntime, options)
    {
    }
  }
```

5. Create a new instance of ```IndexedDbDatabaseModel```.  
Configure this using one of the options in the next section.
4. Register your derived class in ```ConfiguredServices``` in ```Startup.cs```.  
When registering, use options to add the `IndexedDbDatabaseModel` instance.

```CSharp
  services.AddIndexedDbDatabase<GridColumnDataIndexedDb>(options =>
  {
    options.UseDatabase(GetGridColumnDatabaseModel());
  });
```

6. Inject the created instance of your derived class into the component or page.


### Detailed configuration and use

#### Step 1 - Create the database model
To configure the database model, use the following classes. You can find more info about the IndexedDB database here: https://www.w3.org/TR/IndexedDB-2/#database-construct

```IndexedDbDatabaseModel``` 
```IndexedDbStore```
```IndexedDbIndex``` 
```IndexedDbStoreParameter``` 

```CSharp
    public class IndexedDbDatabaseModel
    {      
        public string Name { get; set; }     
        public int Version { get; set; }      
        public List<IndexedDbStore> Stores { get; set; } = new List<IndexedDbStore>();
        public int DbModelId { get; set; }
    }
```

##### Manual Configuration

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

##### Fluent Manual Configuration

Fluent-based Extension methods to make the configuration of the database simplier.
It will create the same model as manual configuration but in a more concise syntax.

```CSharp
using DnetIndexedDb.Fluent;
...
    services.AddIndexedDbDatabase<SecuritySuiteDataIndexedDb>(options =>
    {
        var model = new IndexedDbDatabaseModel()
            .WithName("Security")
            .WithVersion(1)
            .WithModelId(0);

        model.AddStore("tableFieldDtos")
            .WithAutoIncrementingKey("tableFieldId")
            .AddUniqueIndex("tableFieldId")
            .AddIndex("attachedProperty")
            .AddIndex("fieldVisualName")
            .AddIndex("hide")
            .AddIndex("isLink")
            .AddIndex("memberOf")
            .AddIndex("tableName")
            .AddIndex("textAlignClass")
            .AddIndex("type")
            .AddIndex("width");

        options.UseDatabase(model);
    });
```

##### Configure From Class Attributes by @Kylar182

`[IndexDbKey]` and `[IndexDbIndex]` Property Attributes can be used to configure the database based on the given class.  
A DataStore will be created matching the name of the class.

```CSharp
    public class TableFieldDto
    {
        [IndexDbKey(AutoIncrement = true)]
        public int? TableFieldId { get; set; }
        [IndexDbIndex]
        public string TableName { get; set; }
        [IndexDbIndex]
        public string FieldVisualName { get; set; }
        [IndexDbIndex]
        public string AttachedProperty { get; set; }
        [IndexDbIndex]
        public bool IsLink { get; set; }
        [IndexDbIndex]
        public int MemberOf { get; set; }
        [IndexDbIndex]
        public int Width { get; set; }
        [IndexDbIndex]
        public string TextAlignClass { get; set; }
        [IndexDbIndex]
        public bool Hide { get; set; }
        [IndexDbIndex]
        public string Type { get; set; }
    }

...

    var indexedDbDatabaseModel = new IndexedDbDatabaseModel()
        .WithName("TestAttributes")
        .WithVersion(1);

    indexedDbDatabaseModel.AddStore<TableFieldDto>();
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


## API examples

The following examples have two overloads of each DataStore level function.

The first is used when you need to manually specify the DataStore name and the store name does not match the Class Name.

The second is used when the Class name and DataStore name match, such as when using the Class Attribute based configuration.

### Open and upgrade an instance of IndexedDB

```ValueTask<int> OpenIndexedDb()```

```CSharp
 var result = await GridColumnDataIndexedDb.OpenIndexedDb();
```
 
### Add an items to a given store

```ValueTask<TEntity> AddItems<TEntity>(string objectStoreName, List<TEntity> items)```

```ValueTask<TEntity> AddItems<TEntity>(List<TEntity> items)```

```CSharp
// Manually set DataStore name
 var result = await GridColumnDataIndexedDb.AddItems<TableFieldDto>("tableField", items);
OR
// DataStore name inferred from class 
 var result = await GridColumnDataIndexedDb.AddItems<TableFieldDto>(items);
```

### Retrieve an item by key

```ValueTask<TEntity> GetByKey<TKey, TEntity>(string objectStoreName, TKey key)```

```ValueTask<TEntity> GetByKey<TKey, TEntity>(TKey key)```

```CSharp
// Manually set DataStore name
 var result = await GridColumnDataIndexedDb.GetByKey<int, TableFieldDto>("tableField", 11);
OR
// DataStore name inferred from class 
 var result = await GridColumnDataIndexedDb.GetByKey<int, TableFieldDto>(11);
```

### Delete an item from a store by key

```ValueTask<string> DeleteByKey<TKey>(string objectStoreName, TKey key)```

```ValueTask<string> DeleteByKey<TKey, TEntity>(TKey key)```

```CSharp
// Manually set DataStore name
 var result = await GridColumnDataIndexedDb.DeleteByKey<int>("tableField", 11);
OR
// DataStore name inferred from class 
 var result = await GridColumnDataIndexedDb.DeleteByKey<int, TableFieldDto>(11);
```

### Retrieve all items from a given store

```ValueTask<List<TEntity>> GetAll<TEntity>(string objectStoreName)```

```ValueTask<List<TEntity>> GetAll<TEntity>()```

```CSharp
// Manually set DataStore name
  var result = await GridColumnDataIndexedDb.GetAll<TableFieldDto>("tableField");
OR
// DataStore name inferred from class 
  var result = await GridColumnDataIndexedDb.GetAll<TableFieldDto>();
```

### Retrieve a range of items by key

```ValueTask<List<TEntity>> GetRange<TKey, TEntity>(string objectStoreName, TKey lowerBound, TKey upperBound)```

```ValueTask<List<TEntity>> GetRange<TKey, TEntity>(TKey lowerBound, TKey upperBound)```

```CSharp
// Manually set DataStore name
   var result = await GridColumnDataIndexedDb.GetRange<int, TableFieldDto>("tableField", 15, 20);
OR
// DataStore name inferred from class 
   var result = await GridColumnDataIndexedDb.GetRange<int, TableFieldDto>(15, 20);
```

### Retrieve a range of items by index

```ValueTask<List<TEntity>> GetByIndex<TKey, TEntity>(string objectStoreName, TKey lowerBound, TKey upperBound, string dbIndex, bool isRange)```

```ValueTask<List<TEntity>> GetByIndex<TKey, TEntity>(TKey lowerBound, TKey upperBound, string dbIndex, bool isRange)```

```CSharp
// Manually set DataStore name
    var result = await GridColumnDataIndexedDb.GetByIndex<string, TableFieldDto>("tableField", "Name", null, "fieldVisualName", false);
OR
// DataStore name inferred from class 
    var result = await GridColumnDataIndexedDb.GetByIndex<string, TableFieldDto>("Name", null, "fieldVisualName", false);
```

### Retrieve Max Key

```ValueTask<TKey> GetMaxKey<TKey>(string objectStoreName)```

```ValueTask<TKey> GetMaxKey<TKey, TEntity>()```

```CSharp
// Manually set DataStore name
    var result = await GridColumnDataIndexedDb.GetMaxKey<string>("tableField");
OR
// DataStore name inferred from class 
    var result = await GridColumnDataIndexedDb.GetMaxKey<string, TableFieldDto>();
```

### Retrieve Minimum Key

```ValueTask<TKey> GetMinKey<TKey>(string objectStoreName)```

```ValueTask<TKey> GetMinKey<TKey, TEntity>()```

```CSharp
// Manually set DataStore name
    var result = await GridColumnDataIndexedDb.GetMinKey<string>("tableField");
OR
// DataStore name inferred from class 
    var result = await GridColumnDataIndexedDb.GetMinKey<string, TableFieldDto>();
```

### Retrieve Max value by index

```ValueTask<TIndex> GetMaxIndex<TIndex>(string objectStoreName, string dbIndex)```

```ValueTask<TIndex> GetMaxIndex<TIndex, TEntity>(string dbIndex)```

```CSharp
// Manually set DataStore name
    var result = await GridColumnDataIndexedDb.GetMaxIndex<string>("tableField", "fieldVisualName");
OR
// DataStore name inferred from class 
    var result = await GridColumnDataIndexedDb.GetMaxIndex<string, TableFieldDto>("fieldVisualName");
```

### Retrieve Minimum value by index

```ValueTask<TIndex> GetMinIndex<TIndex>(string objectStoreName, string dbIndex)```

```ValueTask<TIndex> GetMinIndex<TIndex, TEntity>(string dbIndex)```

```CSharp
// Manually set DataStore name
    var result = await GridColumnDataIndexedDb.GetMinIndex<string>("tableField", "fieldVisualName");
OR
// DataStore name inferred from class 
    var result = await GridColumnDataIndexedDb.GetMinIndex<string, TableFieldDto>("fieldVisualName");
```

### Update items in a given store

```ValueTask<string> UpdateItems<TEntity>(string objectStoreName, List<TEntity> items)```

```ValueTask<string> UpdateItems<TEntity>(List<TEntity> items)```

```CSharp
    foreach (var item in items)
    {
        item.FieldVisualName = item.FieldVisualName + "Updated";
    }
    // Manually set DataStore name
   var result = await GridColumnDataIndexedDb.UpdateItems<TableFieldDto>("tableField", items);
OR
// DataStore name inferred from class    
   var result = await GridColumnDataIndexedDb.UpdateItems<TableFieldDto>(items);
```

### Delete all items from a store

```ValueTask<string> DeleteAll(string objectStoreName)```

```ValueTask<string> DeleteAll<TEntity>()```

```CSharp
// Manually set DataStore name
  var result = await GridColumnDataIndexedDb.DeleteAll("tableField");
OR
// DataStore name inferred from class 
  var result = await GridColumnDataIndexedDb.DeleteAll<TableFieldDto>();
```

### Delete an instance of IndexedDB

```ValueTask<string> DeleteIndexedDb()```

```CSharp
  var result = await GridColumnDataIndexedDb.DeleteIndexedDb();
```
 
 
