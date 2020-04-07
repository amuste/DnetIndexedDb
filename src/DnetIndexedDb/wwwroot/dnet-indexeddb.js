
window.dnetindexeddbinterop = (function () {

    var sub = {};

    var dbModels = [];

    var dbModelIdCount = 0;

    const indexedDbMessages = {
        DB_OPEN: 'DB_OPEN',
        DB_ALREADY_OPEN: 'DB_ALREADY_OPEN',
        DB_OPEN_AND_UPGRADED: 'DB_OPEN_AND_UPGRADED',
        DB_OPEN_ERROR: 'DB_OPEN_ERROR',
        DB_DELETED: 'DB_DELETED',
        DB_DELETED_ERROR: 'DB_DELETED_ERROR',
        DB_DATA_ADDED: 'DB_DATA_ADDED',
        DB_DATA_ADD_ERROR: 'DB_DATA_ADD_ERROR',
        DB_DATA_UPDATED: 'DB_DATA_UPDATED',
        DB_DATA_UPDATE_ERROR: 'DB_DATA_UPDATE_ERROR',
        DB_CLOSE: 'DB_CLOSE',
        DB_TRANSACTION_ERROR: 'DB_TRANSACTION_ERROR',
        DB_GETBYKEY_ERROR: 'DB_GETBYKEY_ERROR',
        DB_DELETEOBJECT_ERROR: 'DB_DELETEOBJECT_ERROR',
        DB_DELETEOBJECT_SUCCESS: 'DB_DELETEOBJECT_SUCCESS'
    };

    const transactionTypes = {
        'readonly': 'readonly',
        'readwrite': 'readwrite'
    };

    const eventTypes = {
        'success': 'success',
        'error': 'error',
        'complete': 'complete',
        'upgradeneeded': 'upgradeneeded'
    };

    var Rx = window['rxjs'];

    function splitEvery(n, list) {

        if (n <= 0) {
            throw new Error('First argument to splitEvery must be a positive integer');
        }

        let result = [];
        let idx = 0;

        while (idx < list.length) {
            result.push(list.slice(idx, idx += n));
        }

        return result;
    }

    function open(dbModel) {

        var isUpgradeneeded = false;

        return Rx.Observable.create((observer) => {

            if (dbModel.instance > 0) {

                observer.next(-1);
                observer.complete();

            } else {

                var openRequest = indexedDB.open(dbModel.name, dbModel.version);

                var onSuccess = (event) => {

                    dbModel.instance = event.target.result;
                    dbModels.push({ 'dbModelId': dbModelIdCount++, 'dbModel': dbModel});

                    if (isUpgradeneeded) {
                        observer.next(dbModelIdCount);
                    } else {
                        observer.next(dbModelIdCount);
                    }

                    observer.complete();
                };

                var onError = (err) => {
                    observer.error(indexedDbMessages.DB_OPEN_ERROR);
                };

                var onUpgradeneeded = (event) => {

                    var newDbVersion = event.target.result;
                    var storeModel = dbModel.stores;

                    isUpgradeneeded = true;

                    var oldStores = newDbVersion.objectStoreNames;
                    upgradeDb(newDbVersion, storeModel, oldStores);
                };

                openRequest.addEventListener(eventTypes.success, onSuccess);
                openRequest.addEventListener(eventTypes.error, onError);
                openRequest.addEventListener(eventTypes.upgradeneeded, onUpgradeneeded);

                return () => {
                    openRequest.removeEventListener(eventTypes.success, onSuccess);
                    openRequest.removeEventListener(eventTypes.error, onError);
                    openRequest.removeEventListener(eventTypes.upgradeneeded, onUpgradeneeded);
                };

            }
        });
    }

    function upgradeDb(newDbVersion, stores, oldStores) {

        var objectStore;

        var index;
        if (oldStores.length > 0) {

            for (index = 0; index < oldStores.length; index++) {

                const name = oldStores[index];
                newDbVersion.deleteObjectStore(name);
            }
        }

        for (let store of stores) {

            objectStore = newDbVersion.createObjectStore(store.name, store.key);

            for (index of store.indexes) {

                objectStore.createIndex(index.name, index.name, index.definition);
            }
        }
    }

    function deleteDb(dbModel) {

        return Rx.Observable.create((observer) => {

            if (dbModel.instance !== null) {
                dbModel.instance.close();
            }

            var deleteRequest = indexedDB.deleteDatabase(dbModel.name);

            var onSuccess = (event) => {
                dbModel.instance = null;
                observer.next(indexedDbMessages.DB_DELETED);
                observer.complete();
            };

            var onError = (err) => {
                observer.error(indexedDbMessages.DB_DELETED_ERROR);
            };

            deleteRequest.addEventListener(eventTypes.success, onSuccess);
            deleteRequest.addEventListener(eventTypes.error, onError);

            return () => {
                deleteRequest.removeEventListener(eventTypes.success, onSuccess);
                deleteRequest.removeEventListener(eventTypes.error, onError);
            };

        });
    }

    function addItems(dbModel, objectStoreName, data, concurrentTranscations = 20) {

        return Rx.Observable.create((observer) => {

            if (dbModel.instance === null) {

                observer.error(indexedDbMessages.DB_CLOSE);

            } else {

                const transaction = dbModel.instance.transaction([objectStoreName], transactionTypes.readwrite);

                const onTransactionError = (error) => {
                    observer.error(indexedDbMessages.DB_TRANSACTION_ERROR);
                };

                const onComplete = (event) => {
                    observer.next(indexedDbMessages.DB_DATA_ADDED);
                    observer.complete();
                };

                const objectStore = transaction.objectStore(objectStoreName);

                const addItem = (item) => {
                    return new Rx.Observable((addReqObserver) => {

                        const addRequest = objectStore.add(item);

                        const onRequestError = (error) => {
                            addReqObserver.error(indexedDbMessages.DB_DATA_ADD_ERROR);
                        };

                        const onSuccess = (event) => {
                            addReqObserver.next(event);
                            addReqObserver.complete();
                        };

                        addRequest.addEventListener(eventTypes.success, onSuccess);
                        addRequest.addEventListener(eventTypes.error, onRequestError);

                        return () => {
                            addRequest.removeEventListener(eventTypes.success, onSuccess);
                            addRequest.removeEventListener(eventTypes.error, onRequestError);
                        };

                    });
                };

                transaction.addEventListener(eventTypes.error, onTransactionError);
                transaction.addEventListener(eventTypes.complete, onComplete);

                const splitArray = splitEvery(concurrentTranscations, data);
                const addRequestSubscriber = Rx.from(splitArray).pipe(

                    Rx.operators.concatMap((itemList) => {
                        return Rx.from(itemList).pipe(
                            Rx.operators.mergeMap((val) => {
                                return addItem(val);
                            })
                        );
                    })

                ).subscribe(() => { }, (error) => { observer.error(error); });

                return () => {
                    transaction.removeEventListener(eventTypes.complete, onComplete);
                    transaction.removeEventListener(eventTypes.error, onTransactionError);
                    addRequestSubscriber.unsubscribe();
                };

            }
        });
    }

    function updateItems(dbModel, objectStoreName, data, concurrentTranscations = 20) {

        return Rx.Observable.create((observer) => {

            if (dbModel.instance === null) {

                observer.error(indexedDbMessages.DB_CLOSE);

            } else {

                const transaction = dbModel.instance.transaction([objectStoreName], transactionTypes.readwrite);

                const onTransactionError = (error) => {
                    observer.error(indexedDbMessages.DB_TRANSACTION_ERROR);
                };

                const onComplete = (event) => {
                    observer.next(indexedDbMessages.DB_DATA_UPDATED);
                    observer.complete();
                };

                const objectStore = transaction.objectStore(objectStoreName);

                const add = (item) => {
                    return new Rx.Observable((addReqObserver) => {

                        const addRequest = objectStore.put(item);

                        const onRequestError = (error) => {
                            addReqObserver.error(indexedDbMessages.DB_DATA_UPDATE_ERROR);
                        };

                        const onSuccess = (event) => {
                            addReqObserver.next(event);
                            addReqObserver.complete();
                        };

                        addRequest.addEventListener(eventTypes.success, onSuccess);
                        addRequest.addEventListener(eventTypes.error, onRequestError);

                        return () => {
                            addRequest.removeEventListener(eventTypes.success, onSuccess);
                            addRequest.removeEventListener(eventTypes.error, onRequestError);
                        };

                    });
                };

                transaction.addEventListener(eventTypes.error, onTransactionError);
                transaction.addEventListener(eventTypes.complete, onComplete);

                const splitArray = splitEvery(concurrentTranscations, data);

                const addRequestSubscriber = Rx.from(splitArray).pipe(

                    Rx.operators.concatMap((itemList) => {
                        return Rx.from(itemList).pipe(
                            Rx.operators.mergeMap((val) => {
                                return add(val);
                            })
                        );
                    })

                ).subscribe(() => { }, (error) => { observer.error(error); });

                return () => {
                    transaction.removeEventListener(eventTypes.complete, onComplete);
                    transaction.removeEventListener(eventTypes.error, onTransactionError);
                    addRequestSubscriber.unsubscribe();
                };

            }
        });
    }

    function getByKey(dbModel, objectStoreName, key) {

        return Rx.Observable.create((observer) => {

            if (dbModel.instance === null) {

                observer.error(indexedDbMessages.DB_CLOSE);

            } else {

                const transaction = dbModel.instance.transaction([objectStoreName], transactionTypes.readonly);

                const onTransactionError = (error) => {
                    observer.error(indexedDbMessages.DB_TRANSACTION_ERROR);
                };

                const objectStore = transaction.objectStore(objectStoreName);

                const getByKey = () => {
                    return new Rx.Observable((addReqObserver) => {

                        const addRequest = objectStore.get(key);

                        const onRequestError = (error) => {
                            addReqObserver.error(indexedDbMessages.DB_GETBYKEY_ERROR);
                        };

                        const onSuccess = (event) => {
                            addReqObserver.next(addRequest.result);
                            addReqObserver.complete();
                        };

                        addRequest.addEventListener(eventTypes.success, onSuccess);
                        addRequest.addEventListener(eventTypes.error, onRequestError);

                        return () => {
                            addRequest.removeEventListener(eventTypes.success, onSuccess);
                            addRequest.removeEventListener(eventTypes.error, onRequestError);
                        };

                    });
                };

                transaction.addEventListener(eventTypes.error, onTransactionError);

                const addRequestSubscriber = getByKey().subscribe((item) => {
                    observer.next(item);
                    observer.complete();
                }, (error) => { observer.error(error); });

                return () => {
                    addRequestSubscriber.unsubscribe();
                };

            }
        });
    }

    function deleteByKey(dbModel, objectStoreName, key) {

        return Rx.Observable.create((observer) => {

            if (dbModel.instance === null) {

                observer.error(indexedDbMessages.DB_CLOSE);

            } else {

                const transaction = dbModel.instance.transaction([objectStoreName], transactionTypes.readwrite);

                const onTransactionError = (error) => {
                    observer.error(indexedDbMessages.DB_TRANSACTION_ERROR);
                };

                const objectStore = transaction.objectStore(objectStoreName);

                const deleteByKey = () => {
                    return new Rx.Observable((deleteReqObserver) => {

                        const deleteRequest = objectStore.delete(key);

                        const onRequestError = (error) => {
                            deleteReqObserver.error(indexedDbMessages.DB_DELETEOBJECT_ERROR);
                        };

                        const onSuccess = (event) => {
                            deleteReqObserver.next(indexedDbMessages.DB_DELETEOBJECT_SUCCESS);
                            deleteReqObserver.complete();
                        };

                        deleteRequest.addEventListener(eventTypes.success, onSuccess);
                        deleteRequest.addEventListener(eventTypes.error, onRequestError);

                        return () => {
                            deleteRequest.removeEventListener(eventTypes.success, onSuccess);
                            deleteRequest.removeEventListener(eventTypes.error, onRequestError);
                        };

                    });
                };

                transaction.addEventListener(eventTypes.success, onTransactionError);

                const deleteRequestSubscriber = deleteByKey().subscribe((message) => {
                    observer.next(message);
                    observer.complete();
                }, (error) => { observer.error(error); });

                return () => {
                    deleteRequestSubscriber.unsubscribe();
                    transaction.removeEventListener(eventTypes.error, onTransactionError);
                };

            }
        });
    }

    function deleteAll(dbModel, objectStoreName) {

        return Rx.Observable.create((observer) => {

            if (dbModel.instance === null) {

                observer.error(indexedDbMessages.DB_CLOSE);

            } else {

                const transaction = dbModel.instance.transaction([objectStoreName], transactionTypes.readwrite);

                const onTransactionError = (error) => {
                    observer.error(indexedDbMessages.DB_TRANSACTION_ERROR);
                };

                const objectStore = transaction.objectStore(objectStoreName);

                const deleteAll = () => {

                    return new Rx.Observable((deleteReqObserver) => {

                        const deleteRequest = objectStore.clear();

                        const onRequestError = (error) => {
                            deleteReqObserver.error(indexedDbMessages.DB_DELETEOBJECT_ERROR);
                        };

                        const onSuccess = (event) => {
                            deleteReqObserver.next(indexedDbMessages.DB_DELETEOBJECT_SUCCESS);
                            deleteReqObserver.complete();
                        };

                        deleteRequest.addEventListener(eventTypes.success, onSuccess);
                        deleteRequest.addEventListener(eventTypes.error, onRequestError);

                        return () => {
                            deleteRequest.removeEventListener(eventTypes.success, onSuccess);
                            deleteRequest.removeEventListener(eventTypes.error, onRequestError);
                        };

                    });
                };

                transaction.addEventListener(eventTypes.success, onTransactionError);

                const deleteRequestSubscriber = deleteAll().subscribe((message) => {
                    observer.next(message);
                    observer.complete();
                }, (error) => { observer.error(error); });

                return () => {
                    deleteRequestSubscriber.unsubscribe();
                    transaction.removeEventListener(eventTypes.error, onTransactionError);
                };
            }
        });
    }

    function getAll(dbModel, objectStoreName) {

        return Rx.Observable.create((observer) => {

            if (dbModel.instance === null) {
                observer.error(indexedDbMessages.DB_CLOSE);
            } else {

                const transaction = dbModel.instance.transaction([objectStoreName], transactionTypes.readonly);

                const onTransactionError = (error) => {
                    observer.error(indexedDbMessages.DB_TRANSACTION_ERROR);
                };

                const objectStore = transaction.objectStore(objectStoreName);

                const objectCount = () => {
                    return new Rx.Observable((objectCountObserver) => {

                        const objectCountRequest = objectStore.count();

                        const onRequestError = (error) => {
                            objectCountObserver.error(indexedDbMessages.DB_DELETEOBJECT_ERROR);
                        };

                        const onSuccess = (event) => {
                            let count = event.target.result;
                            objectCountObserver.next(count);
                            objectCountObserver.complete();
                        };

                        objectCountRequest.addEventListener(eventTypes.success, onSuccess);
                        objectCountRequest.addEventListener(eventTypes.error, onRequestError);

                        return () => {
                            objectCountRequest.removeEventListener(eventTypes.success, onSuccess);
                            objectCountRequest.removeEventListener(eventTypes.error, onRequestError);
                        };

                    });
                };

                let objects = [];
                const getAll = (count) => {
                    return new Rx.Observable((cursorObserver) => {

                        const cursorRequest = objectStore.openCursor();

                        const onRequestError = (error) => {
                            cursorObserver.error(indexedDbMessages.DB_DELETEOBJECT_ERROR);
                        };

                        const onSuccess = (event) => {

                            let result = event.target.result;

                            if (result !== null) {

                                let item = result.value;
                                objects.push(item);

                                if (objects.length === count) {
                                    cursorObserver.next(objects);
                                    cursorObserver.complete();
                                } else {
                                    result.continue();
                                }
                            } else {
                                cursorObserver.next(objects);
                                cursorObserver.complete();
                            }
                        };

                        cursorRequest.addEventListener(eventTypes.success, onSuccess);
                        cursorRequest.addEventListener(eventTypes.error, onRequestError);

                        return () => {
                            cursorRequest.removeEventListener(eventTypes.success, onSuccess);
                            cursorRequest.removeEventListener(eventTypes.error, onRequestError);
                        };

                    });
                };

                transaction.addEventListener(eventTypes.success, onTransactionError);

                const getAllSubscriber = objectCount().pipe(

                    Rx.operators.mergeMap((count) => {
                        return getAll(count);
                    })

                ).subscribe((data) => {

                    observer.next(data);
                    observer.complete();

                }, (error) => { observer.error(error); });

                return () => {
                    getAllSubscriber.unsubscribe();
                    transaction.removeEventListener(eventTypes.error, onTransactionError);
                };

            }
        });
    }

    function getRange(dbModel, objectStoreName, lowerBound, upperBound) {

        return Rx.Observable.create((observer) => {

            if (dbModel.instance === null) {
                observer.error(indexedDbMessages.DB_CLOSE);
            } else {

                const transaction = dbModel.instance.transaction([objectStoreName], transactionTypes.readonly);

                const onTransactionError = (error) => {
                    observer.error(indexedDbMessages.DB_TRANSACTION_ERROR);
                };

                const objectStore = transaction.objectStore(objectStoreName);

                let objects = [];

                const getRange = () => {
                    return new Rx.Observable((cursorObserver) => {

                        let boundedKeyRange = IDBKeyRange.bound(lowerBound, upperBound, false, false);
                        const cursorRequest = objectStore.openCursor(boundedKeyRange);

                        const onRequestError = (error) => {
                            cursorObserver.error(indexedDbMessages.DB_DELETEOBJECT_ERROR);
                        };

                        const onSuccess = (event) => {

                            let result = event.target.result;

                            if (result !== null) {

                                let item = result.value;
                                objects.push(item);
                                result.continue();

                            } else {
                                cursorObserver.next(objects);
                                cursorObserver.complete();
                            }
                        };

                        cursorRequest.addEventListener(eventTypes.success, onSuccess);
                        cursorRequest.addEventListener(eventTypes.error, onRequestError);

                        return () => {
                            cursorRequest.removeEventListener(eventTypes.success, onSuccess);
                            cursorRequest.removeEventListener(eventTypes.error, onRequestError);
                        };

                    });
                };

                transaction.addEventListener(eventTypes.success, onTransactionError);

                const getRangeSubscriber = getRange().subscribe((data) => {
                    observer.next(data);
                    observer.complete();
                }, (error) => { observer.error(error); });

                return () => {
                    transaction.removeEventListener(eventTypes.error, onTransactionError);
                    getRangeSubscriber.unsubscribe();
                };

            }
        });
    }

    function getByIndex(dbModel, objectStoreName, lowerBound, upperBound, dbIndex, isRange) {

        return Rx.Observable.create((observer) => {

            if (dbModel.instance === null) {
                observer.error(indexedDbMessages.DB_CLOSE);
            } else {

                const transaction = dbModel.instance.transaction([objectStoreName], transactionTypes.readonly);

                const onTransactionError = (error) => {
                    observer.error(indexedDbMessages.DB_TRANSACTION_ERROR);
                };

                const objectStore = transaction.objectStore(objectStoreName);

                let objects = [];

                const getRangeByIndex = () => {

                    return new Rx.Observable((cursorObserver) => {

                        let boundedKeyRange;

                        isRange = isRange || false;

                        if (isRange) {
                            boundedKeyRange = IDBKeyRange.bound(lowerBound, upperBound, false, false);
                        } else {
                            boundedKeyRange = IDBKeyRange.only(lowerBound);
                        }

                        const index = objectStore.index(dbIndex);
                        const cursorRequest = index.openCursor(boundedKeyRange);

                        const onRequestError = (error) => {
                            cursorObserver.error(indexedDbMessages.DB_DELETEOBJECT_ERROR);
                        };

                        const onSuccess = (event) => {

                            let result = event.target.result;

                            if (result !== null) {

                                let item = result.value;
                                objects.push(item);
                                result.continue();

                            } else {
                                cursorObserver.next(objects);
                                cursorObserver.complete();
                            }
                        };

                        cursorRequest.addEventListener(eventTypes.success, onSuccess);
                        cursorRequest.addEventListener(eventTypes.error, onRequestError);

                        return () => {
                            cursorRequest.removeEventListener(eventTypes.success, onSuccess);
                            cursorRequest.removeEventListener(eventTypes.error, onRequestError);
                        };

                    });
                };

                transaction.addEventListener(eventTypes.success, onTransactionError);

                const getRangeSubscriber = getRangeByIndex().subscribe((data) => {
                    observer.next(data);
                    observer.complete();
                }, (error) => { observer.error(error); });

                return () => {
                    transaction.removeEventListener(eventTypes.error, onTransactionError);
                    getRangeSubscriber.unsubscribe();
                };
            }
        });
    }


    function getDbModel(dbModelId) {

        const dbModel = dbModels.find(element => element.dbModelId = dbModelId);

        return dbModel;
    }

    return {

        openDb: async function (indexedDbDatabaseModel) {
          
            var dbModelId = await open(indexedDbDatabaseModel).pipe(Rx.operators.take(1)).toPromise();

            return dbModelId;
        },

        deleteDb: async function (indexedDbDatabaseModel) {

            let dbModel = getDbModel(indexedDbDatabaseModel.dbModelId).dbModel;

            return await deleteDb(dbModel).pipe(Rx.operators.take(1)).toPromise();
        },

        addItems: async function (indexedDbDatabaseModel, objectStoreName, items) {

            let dbModel = getDbModel(indexedDbDatabaseModel.dbModelId).dbModel;

            return await addItems(dbModel, objectStoreName, items).pipe(Rx.operators.take(1)).toPromise();
        },

        updateItems: async function (indexedDbDatabaseModel, objectStoreName, items) {

            let dbModel = getDbModel(indexedDbDatabaseModel.dbModelId).dbModel;

            return await updateItems(dbModel, objectStoreName, items).pipe(Rx.operators.take(1)).toPromise();
        },

        getByKey: async function (indexedDbDatabaseModel, objectStoreName, key) {

            let dbModel = getDbModel(indexedDbDatabaseModel.dbModelId).dbModel;

            return await getByKey(dbModel, objectStoreName, key).pipe(Rx.operators.take(1)).toPromise();
        },

        deleteByKey: async function (indexedDbDatabaseModel, objectStoreName, key) {

            let dbModel = getDbModel(indexedDbDatabaseModel.dbModelId).dbModel;

            return await deleteByKey(dbModel, objectStoreName, key).pipe(Rx.operators.take(1)).toPromise();
        },

        deleteAll: async function (indexedDbDatabaseModel, objectStoreName) {

            let dbModel = getDbModel(indexedDbDatabaseModel.dbModelId).dbModel;
           
            return await deleteAll(dbModel, objectStoreName).pipe(Rx.operators.take(1)).toPromise();
        },

        getAll: async function (indexedDbDatabaseModel, objectStoreName) {

            let dbModel = getDbModel(indexedDbDatabaseModel.dbModelId).dbModel;

            return await getAll(dbModel, objectStoreName).pipe(Rx.operators.take(1)).toPromise();
        },

        getRange: async function (indexedDbDatabaseModel, objectStoreName, lowerBound, upperBound) {

            let dbModel = getDbModel(indexedDbDatabaseModel.dbModelId).dbModel;

            return await getRange(dbModel, objectStoreName, lowerBound, upperBound).pipe(Rx.operators.take(1)).toPromise();
        },

        getByIndex: async function (indexedDbDatabaseModel, objectStoreName, lowerBound, upperBound, dbIndex, isRange) {

            let dbModel = getDbModel(indexedDbDatabaseModel.dbModelId).dbModel;

            return await getByIndex(dbModel, objectStoreName, lowerBound, upperBound, dbIndex, isRange).pipe(Rx.operators.take(1)).toPromise();
        }

    };
})();