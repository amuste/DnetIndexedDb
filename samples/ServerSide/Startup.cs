using System.Collections.Generic;
using DnetIndexedDb;
using DnetIndexedDb.Models;
using DnetIndexedDbServer.Infrastructure;
using DnetIndexedDbServer.Shared.Kylar;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DnetIndexedDbServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            services.AddServerSideBlazor();

            services.AddHttpClient();

            services.AddIndexedDbDatabase<GridColumnDataIndexedDb>(options =>
            {
                options.UseDatabase(GetGridColumnDatabaseModel());
            });

            //var model = new TableFieldDatabase();
            //services.AddIndexedDbDatabase<GridColumnDataIndexedDb>(options =>
            //{
            //    options.UseDatabase(model);
            //});

            var model1 = new SecurityDatabase();
            services.AddIndexedDbDatabase<SecuritySuiteDataIndexedDb>(options =>
            {
                options.UseDatabase(model1);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {  
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        private IndexedDbDatabaseModel GetGridColumnDatabaseModel()
        {
            var indexedDbDatabaseModel = new IndexedDbDatabaseModel
            {
                Name = "GridColumnData",
                Version = 1,
                Stores = new List<IndexedDbStore>
                {
                new IndexedDbStore
                {
                    Name = "TableFieldDtos",
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
                DbModelId = 0,
                UseKeyGenerator = true
            };

            return indexedDbDatabaseModel;
        }

        private IndexedDbDatabaseModel GetSecurityDatabaseModel()
        {
            var indexedDbDatabaseModel = new IndexedDbDatabaseModel
            {
                Name = "Security",
                Version = 1,
                Stores = new List<IndexedDbStore>
        {
                new IndexedDbStore
                {
                    Name = "tableFieldDtos",
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

            return indexedDbDatabaseModel;
        }


    }


}
