
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

    const extentTypes = {
        Max: "prev",
        Min: "next"
    }

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

                const openRequest = indexedDB.open(dbModel.name, dbModel.version);

                const onSuccess = (event) => {

                    dbModel.instance = event.target.result;

                    dbModels.push({ 'dbModel': dbModel });

                    if (isUpgradeneeded) {
                        observer.next(dbModelIdCount);
                    } else {
                        observer.next(dbModelIdCount);
                    }

                    observer.complete();
                };

                const onError = (err) => {
                    observer.error(indexedDbMessages.DB_OPEN_ERROR);
                };

                const onUpgradeneeded = (event) => {

                    const currentDbVersion = event.target.result;
                    const storeModel = dbModel.stores;

                    isUpgradeneeded = true;

                    const oldStores = currentDbVersion.objectStoreNames;
                    upgradeDb(currentDbVersion, storeModel, oldStores, event.target.transaction);
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

   

    function upgradeDb(currentDbVersion, stores, oldStores, transaction) {

        const oldStoresArray = [...oldStores];

        if (oldStoresArray.length > 0) {

            /* Check every current store against the old version. If it is not in the old version, create the store.
             * If it exists in the old, add or delete indexes where necessary.
             */
            for (let store of stores) {

                const haveThisStore = oldStoresArray.indexOf(store.name) > -1;

                if (haveThisStore) {

                    const objectStore = transaction.objectStore(store.name);

                    const oldStoreIndexes = objectStore.indexNames;

                    const oldStoreIndexesArray = [...oldStoreIndexes];
                    const currentIndexesArray = store.indexes.map(p => p.name);

                    const itemsToDeleteNames = oldStoreIndexesArray.filter(e => currentIndexesArray.indexOf(e) === -1);
                    const itemsToAddNames = currentIndexesArray.filter(e => oldStoreIndexesArray.indexOf(e) === -1);

                    for (let itemToAddName of itemsToAddNames) {

                        const newIndex = store.indexes.find(p => p.name === itemToAddName);

                        if (newIndex != null) objectStore.createIndex(newIndex.name, newIndex.name, newIndex.definition);

                    }

                    for (let itemToDeleteName of itemsToDeleteNames) {

                        objectStore.deleteIndex(itemToDeleteName);

                    }

                } else {
                    createStore(currentDbVersion, store);
                }
            }

            /* Check every old store against the current version. If it is not in the current version, delete the old store.
             * This is necessary as old stores appear to remain even if they are not anymore defined in the current version.
             */
            const currentStoreNames = stores.map(p => p.name);

            for (let oldStore of oldStoresArray) {

                const notInCurrentVersion = currentStoreNames.indexOf(oldStore) == -1;

                if (notInCurrentVersion) {
                    currentDbVersion.deleteObjectStore(oldStore);
                }

            }

        } else {

            for (let store of stores) {
                createStore(currentDbVersion, store);
            }
        }
    }

    function toBytesInt32(num) {
        arr = new ArrayBuffer(4); // an Int32 takes 4 bytes
        view = new DataView(arr);
        view.setUint32(0, num, false); // byteOffset = 0; litteEndian = false
        return arr;
    }

    function createStore(currentDbVersion, store) {

        const key = store.key.keyPath === "" ? { autoIncrement: true } : store.key;

        const objectStore = currentDbVersion.createObjectStore(store.name, key);

        for (let index of store.indexes) {

            objectStore.createIndex(index.name, index.name, index.definition);
        }
    }

    function deleteDb(dbModel) {

        return Rx.Observable.create((observer) => {

            if (dbModel.instance !== null) {
                dbModel.instance.close();
            }

            var deleteRequest = indexedDB.deleteDatabase(dbModel.name);

            var onSuccess = (event) => {

                const index = dbModels.findIndex(item => item.dbModelGuid === dbModel.dbModelGuid);

                if (index !== -1) {
                    dbModels.splice(index, 1);
                };

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

    function addBlobItem(dbModel, objectStoreName, data, key = null) {

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

                const addItem = (item,key) => {

                    return new Rx.Observable((addReqObserver) => {

                        const store = dbModel.stores.find(p => p.name === objectStoreName);

                        const keyPath = store.key.keyPath;

                        if (keyPath !== "" && dbModel.useKeyGenerator) delete item[keyPath];

                        const addRequest = objectStore.add(item,key);

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

                const addRequestSubscriber = addItem(data,key)
                .subscribe(() => { }, (error) => { observer.error(error); });

                return () => {
                    transaction.removeEventListener(eventTypes.complete, onComplete);
                    transaction.removeEventListener(eventTypes.error, onTransactionError);
                    addRequestSubscriber.unsubscribe();
                };

            }
        });
    }


    function updateBlobItem(dbModel, objectStoreName, data, key = null) {

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

                const update = (item, key) => {

                    return new Rx.Observable((addReqObserver) => {

                        const store = dbModel.stores.find(p => p.name === objectStoreName);

                        const keyPath = store.key.keyPath;

                        if (keyPath !== "" && dbModel.useKeyGenerator) delete item[keyPath];

                        const updateRequest = objectStore.put(item, key);

                        const onRequestError = (error) => {
                            addReqObserver.error(indexedDbMessages.DB_DATA_ADD_ERROR);
                        };

                        const onSuccess = (event) => {
                            addReqObserver.next(event);
                            addReqObserver.complete();
                        };

                        updateRequest.addEventListener(eventTypes.success, onSuccess);
                        updateRequest.addEventListener(eventTypes.error, onRequestError);

                        return () => {
                            updateRequest.removeEventListener(eventTypes.success, onSuccess);
                            updateRequest.removeEventListener(eventTypes.error, onRequestError);
                        };

                    });
                };

                transaction.addEventListener(eventTypes.error, onTransactionError);
                transaction.addEventListener(eventTypes.complete, onComplete);

                const updateRequestSubscriber = update(data, key)
                    .subscribe(() => { }, (error) => { observer.error(error); });

                return () => {
                    transaction.removeEventListener(eventTypes.complete, onComplete);
                    transaction.removeEventListener(eventTypes.error, onTransactionError);
                    updateRequestSubscriber.unsubscribe();
                };

            }
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

                        const store = dbModel.stores.find(p => p.name === objectStoreName);

                        const keyPath = store.key.keyPath;

                        if (keyPath !== "" && dbModel.useKeyGenerator) delete item[keyPath];

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

                const update = (item) => {

                    return new Rx.Observable((updateReqObserver) => {

                        const updateRequest = objectStore.put(item);

                        const onRequestError = (error) => {
                            updateReqObserver.error(indexedDbMessages.DB_DATA_UPDATE_ERROR);
                        };

                        const onSuccess = (event) => {
                            updateReqObserver.next(event);
                            updateReqObserver.complete();
                        };

                        updateRequest.addEventListener(eventTypes.success, onSuccess);
                        updateRequest.addEventListener(eventTypes.error, onRequestError);

                        return () => {
                            updateRequest.removeEventListener(eventTypes.success, onSuccess);
                            updateRequest.removeEventListener(eventTypes.error, onRequestError);
                        };

                    });
                };

                transaction.addEventListener(eventTypes.error, onTransactionError);
                transaction.addEventListener(eventTypes.complete, onComplete);

                const splitArray = splitEvery(concurrentTranscations, data);

                const updateRequestSubscriber = Rx.from(splitArray).pipe(

                    Rx.operators.concatMap((itemList) => {

                        return Rx.from(itemList).pipe(

                            Rx.operators.mergeMap((val) => {

                                return update(val);
                            })
                        );
                    })

                ).subscribe(() => { }, (error) => { observer.error(error); });

                return () => {
                    transaction.removeEventListener(eventTypes.complete, onComplete);
                    transaction.removeEventListener(eventTypes.error, onTransactionError);
                    updateRequestSubscriber.unsubscribe();
                };

            }
        });
    }

    function updateItemsByKey(dbModel, objectStoreName, data, keys) {

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

                const update = (item, key) => {

                    return new Rx.Observable((updateReqObserver) => {

                        const updateRequest = objectStore.put(item, key);

                        const onRequestError = (error) => {
                            updateReqObserver.error(indexedDbMessages.DB_DATA_UPDATE_ERROR);
                        };

                        const onSuccess = (event) => {
                            updateReqObserver.next(event);
                            updateReqObserver.complete();
                        };

                        updateRequest.addEventListener(eventTypes.success, onSuccess);
                        updateRequest.addEventListener(eventTypes.error, onRequestError);

                        return () => {
                            updateRequest.removeEventListener(eventTypes.success, onSuccess);
                            updateRequest.removeEventListener(eventTypes.error, onRequestError);
                        };

                    });
                };

                transaction.updateEventListener(eventTypes.error, onTransactionError);
                transaction.updateEventListener(eventTypes.complete, onComplete);

                const items$ = Rx.from(data);
                const keys$ = Rx.from(keys);

                const updateRequestSubscriber = Rx.zip(items$, keys$).pipe(

                    Rx.operators.switchMap((values) => {

                        return update(values[0], values[1]);

                    })

                ).subscribe(() => { }, (error) => { observer.error(error); });

                return () => {
                    transaction.removeEventListener(eventTypes.complete, onComplete);
                    transaction.removeEventListener(eventTypes.error, onTransactionError);
                    updateRequestSubscriber.unsubscribe();
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
                    return new Rx.Observable((getReqObserver) => {

                        const getRequest = objectStore.get(key);

                        const onRequestError = (error) => {
                            getReqObserver.error(indexedDbMessages.DB_GETBYKEY_ERROR);
                        };

                        const onSuccess = (event) => {
//                            console.log("Got Record:");     // FSIGAP
//                            console.log(getRequest.result); // FSIGAP 
                            getReqObserver.next(getRequest.result);
                            getReqObserver.complete();
                        };

                        getRequest.addEventListener(eventTypes.success, onSuccess);
                        getRequest.addEventListener(eventTypes.error, onRequestError);

                        return () => {
                            getRequest.removeEventListener(eventTypes.success, onSuccess);
                            getRequest.removeEventListener(eventTypes.error, onRequestError);
                        };

                    });
                };

                transaction.addEventListener(eventTypes.error, onTransactionError);

                const getRequestSubscriber = getByKey().subscribe((item) => {
                    observer.next(item);
                    observer.complete();
                }, (error) => { observer.error(error); });

                return () => {
                    getRequestSubscriber.unsubscribe();
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

                const objects = [];
                const getAllLocal = (count) => {
                    return new Rx.Observable((cursorObserver) => {

                        const cursorRequest = objectStore.openCursor();

                        const onRequestError = (error) => {
                            cursorObserver.error(indexedDbMessages.DB_DELETEOBJECT_ERROR);
                        };

                        const onSuccess = (event) => {

                            const result = event.target.result;

                            if (result !== null) {

                                const item = result.value;
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
                        return getAllLocal(count);
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

                const objects = [];

                const getRangeLocal = () => {
                    return new Rx.Observable((cursorObserver) => {

                        const boundedKeyRange = IDBKeyRange.bound(lowerBound, upperBound, false, false);
                        const cursorRequest = objectStore.openCursor(boundedKeyRange);

                        const onRequestError = (error) => {
                            cursorObserver.error(indexedDbMessages.DB_DELETEOBJECT_ERROR);
                        };

                        const onSuccess = (event) => {

                            const result = event.target.result;

                            if (result !== null) {

                                const item = result.value;
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

                const getRangeSubscriber = getRangeLocal().subscribe((data) => {
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

                const objects = [];

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

                            const result = event.target.result;

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

    function getExtent(dbModel, objectStoreName, dbIndex, extentType) {

        return Rx.Observable.create((observer) => {

            if (dbModel.instance === null) {

                observer.error(indexedDbMessages.DB_CLOSE);

            } else {

                const transaction = dbModel.instance.transaction([objectStoreName], transactionTypes.readonly);

                const onTransactionError = (error) => {
                    observer.error(indexedDbMessages.DB_TRANSACTION_ERROR);
                };

                const objectStore = transaction.objectStore(objectStoreName);

                const getKeyOrIndexExtent = () => {
                    return new Rx.Observable((getReqObserver) => {

                        let cursorRequest = null;

                        if (dbIndex) {
                            // Search By Index
                            const index = objectStore.index(dbIndex);
                            cursorRequest = index.openCursor(null, extentType);
                        } else {
                            // Search By Key
                            cursorRequest = objectStore.openCursor(null, extentType);
                        }

                        const onRequestError = (error) => {
                            getReqObserver.error(indexedDbMessages.DB_GETBYKEY_ERROR);
                        };

                        const onSuccess = (event) => {
                            if (cursorRequest.result) {
                                getReqObserver.next(cursorRequest.result.key);
                            } else {
                                getReqObserver.next(null);
                            }

                            getReqObserver.complete();
                        };

                        cursorRequest.addEventListener(eventTypes.success, onSuccess);
                        cursorRequest.addEventListener(eventTypes.error, onRequestError);

                        return () => {
                            cursorRequest.removeEventListener(eventTypes.success, onSuccess);
                            cursorRequest.removeEventListener(eventTypes.error, onRequestError);
                        };

                    });
                };

                transaction.addEventListener(eventTypes.error, onTransactionError);

                const getRequestSubscriber = getKeyOrIndexExtent().subscribe((item) => {
                    observer.next(item);
                    observer.complete();
                }, (error) => { observer.error(error); });

                return () => {
                    getRequestSubscriber.unsubscribe();
                };

            }
        });
    }


    function getDbModel(dbModelGuid) {

        const dbModel = dbModels.find(element => element.dbModel.dbModelGuid === dbModelGuid);

        return dbModel;
    }

    
    return {

        openDb: async function (indexedDbDatabaseModel) {

            const dbModelId = await open(indexedDbDatabaseModel).pipe(Rx.operators.take(1)).toPromise();

            return dbModelId;
        },

        deleteDb: async function (indexedDbDatabaseModel) {

            const dbModel = getDbModel(indexedDbDatabaseModel.dbModelGuid).dbModel;

            return await deleteDb(dbModel).pipe(Rx.operators.take(1)).toPromise();
        },

        addBlobItem: async function (fields, item) {
            // extract unmarshalled fields from struct
            const dbModelGuid = Blazor.platform.readStringField(fields, 0);
            const objectStoreName = Blazor.platform.readStringField(fields, 8);
            let key = Blazor.platform.readStringField(fields, 16);
            const mimeType = Blazor.platform.readStringField(fields, 24);
            console.log(`Adding file with mime type ${mimeType}`);
            if (key === "") key = null;
            const dbModel = getDbModel(dbModelGuid).dbModel;

            // create blob from array
            const dataPtr = Blazor.platform.getArrayEntryPtr(item, 0, 4);
            const length = Blazor.platform.getArrayLength(item);
            var blob = new Blob([new Uint8Array(Module.HEAPU8.buffer, dataPtr, length)], { type: mimeType });

            return await addBlobItem(dbModel, objectStoreName, blob, key).pipe(Rx.operators.take(1)).toPromise();
        },

        updateBlobItem: async function (fields, item) {
            // extract unmarshalled fields from struct
            const dbModelGuid = Blazor.platform.readStringField(fields, 0);
            const objectStoreName = Blazor.platform.readStringField(fields, 8);
            let key = Blazor.platform.readStringField(fields, 16);
            const mimeType = Blazor.platform.readStringField(fields, 24);
            console.log(`Updating file with mime type ${mimeType}`);
            if (key === "") key = null;
            const dbModel = getDbModel(dbModelGuid).dbModel;

            // create blob from array
            const dataPtr = Blazor.platform.getArrayEntryPtr(item, 0, 4);
            const length = Blazor.platform.getArrayLength(item);
            var blob = new Blob([new Uint8Array(Module.HEAPU8.buffer, dataPtr, length)], { type: mimeType });

            return await updateBlobItem(dbModel, objectStoreName, blob, key).pipe(Rx.operators.take(1)).toPromise();
        },

        addItems: async function (indexedDbDatabaseModel, objectStoreName, items) {

            const dbModel = getDbModel(indexedDbDatabaseModel.dbModelGuid).dbModel;

            return await addItems(dbModel, objectStoreName, items).pipe(Rx.operators.take(1)).toPromise();
        },

        updateItems: async function (indexedDbDatabaseModel, objectStoreName, items) {

            const dbModel = getDbModel(indexedDbDatabaseModel.dbModelGuid).dbModel;

            return await updateItems(dbModel, objectStoreName, items).pipe(Rx.operators.take(1)).toPromise();
        },

        updateItemsByKey: async function (indexedDbDatabaseModel, objectStoreName, items, keys) {

            const dbModel = getDbModel(indexedDbDatabaseModel.dbModelGuid).dbModel;

            return await updateItemsByKey(dbModel, objectStoreName, items, keys).pipe(Rx.operators.take(1)).toPromise();
        },

        assignBlobToElement: async function (indexedDbDatabaseModel, objectStoreName, key, elementId, attribute) {
            const dbModel = getDbModel(indexedDbDatabaseModel.dbModelGuid).dbModel;
            const res = await getByKey(dbModel, objectStoreName, key).pipe(Rx.operators.take(1)).toPromise();
            var el = document.getElementById(elementId);
            var url=URL.createObjectURL(res);
            el.setAttribute(attribute, url);
        },

        // This is really slow since it has to base64 encode anything going back to blazor.
        // You could directly reference the blob using something like this which should be faster:
        /// var preview = document.getElementById('preview');
        //  URL.createObjectURL(blobvar); 
        //  preview.setAttribute('src', url);
        //https://schibsted.com/blog/the-magic-of-createobjecturl/
        //https://developer.mozilla.org/en-US/docs/Web/API/File/Using_files_from_web_applications#example_using_object_urls_to_display_images
        getBlobByKey: async function (indexedDbDatabaseModel, objectStoreName, key) {

            const dbModel = getDbModel(indexedDbDatabaseModel.dbModelGuid).dbModel;
            const res = await getByKey(dbModel, objectStoreName, key).pipe(Rx.operators.take(1)).toPromise();
            const reader = new FileReader();
            reader.readAsDataURL(res);
            return new Promise(resolve => {
                reader.onloadend = () => {
                    resolve(reader.result);
                };
            });
        },

        //// Faster version of above by writing into .net memory buffer. 
        ////
        //// See:
        //// https://github.com/SteveSandersonMS/BlazorInputFile/blob/master/BlazorInputFile/SharedMemoryFileListEntryStream.cs
        //// https://github.com/SteveSandersonMS/BlazorInputFile/blob/master/BlazorInputFile/wwwroot/inputfile.js
        //// https://github.com/SteveSandersonMS/BlazorInputFile/blob/master/BlazorInputFile/FileListEntryStream.cs
        ////
        //// Unmarshalled invoke doesn't seem to have an async version, so return value synchronously
        ////
        getBlobByKey2: function (fields) {
            var bytesReturned = Blazor.platform.readInt32Field(fields, 32);
            var bytesReturnedArray = Blazor.platform.toUint8Array(bytesReturned);

            return (async () => {
                const dbModelGuid = Blazor.platform.readStringField(fields, 0);
                const objectStoreName = Blazor.platform.readStringField(fields, 8);
                const key = Blazor.platform.readStringField(fields, 16);
                var destination = Blazor.platform.readInt32Field(fields, 24);
                var maxBytes = Blazor.platform.readInt32Field(fields, 28);
                var bytesReturned = Blazor.platform.readInt32Field(fields, 32);
                const dbModel = getDbModel(dbModelGuid).dbModel;
                const res = await getByKey(dbModel, objectStoreName, key).pipe(Rx.operators.take(1)).toPromise();
                //console.log(`got blob: ${res}`);
                if (res == undefined) {
                    throw "Blob with key " + key +" not found";
                }
                else {
                    const reader = new FileReader();
                    reader.readAsArrayBuffer(res);
                    //console.log("getBlobByKey2() returning promise");

                    //let result = await
                    return new Promise(resolve => {
                        reader.onloadend = () => {
                            //console.log("getBlobByKey2() load ended");
                            var sourceArrayBuffer = reader.result;
                            var bytesToRead = Math.min(maxBytes, sourceArrayBuffer.byteLength);
                            //console.log(`getBlobByKey2() bytes to read ${bytesToRead}`);
                            var sourceUint8Array = new Uint8Array(sourceArrayBuffer, 0, bytesToRead);
                            //console.log(`Source array first three bytes=${sourceUint8Array[0]} ${sourceUint8Array[1]} ${sourceUint8Array[2]} `)
                            var destinationUint8Array = Blazor.platform.toUint8Array(destination);
                            destinationUint8Array.set(sourceUint8Array, 0);
                            //console.log(`Dest array first three bytes=${destinationUint8Array[0]} ${destinationUint8Array[1]} ${destinationUint8Array[2]} `)
                            //console.log(`getBlobByKey2() promise returning ${bytesToRead}`);
                            var bytesReturnedArray = Blazor.platform.toUint8Array(bytesReturned);                            
                            var readout = new Uint8Array(toBytesInt32(bytesToRead));
                            //console.log(`getBlobByKey2() readout[0] ${readout[0]}`);
                            bytesReturnedArray.set(readout, 0);
                            resolve(bytesToRead);

                        };
                    });
                }
            })().catch(e => {
                console.error(e);
                var abuf = toBytesInt32(-1);
                var arr = new Uint8Array(abuf);
                console.log(arr);
                bytesReturnedArray.set(arr, 0);// set bytes returned to -1 on error
            });
        },
        // Faster version of above by writing into .net memory buffer. 
        //
        // See:
        // https://github.com/SteveSandersonMS/BlazorInputFile/blob/master/BlazorInputFile/SharedMemoryFileListEntryStream.cs
        // https://github.com/SteveSandersonMS/BlazorInputFile/blob/master/BlazorInputFile/wwwroot/inputfile.js
        // https://github.com/SteveSandersonMS/BlazorInputFile/blob/master/BlazorInputFile/FileListEntryStream.cs
        //
        //getBlobByKey2: function (fields) {
        //    (async () => await getBlobByKey2b(fields))();
        //},

        getByKey: async function (indexedDbDatabaseModel, objectStoreName, key) {

            const dbModel = getDbModel(indexedDbDatabaseModel.dbModelGuid).dbModel;
            return await getByKey(dbModel, objectStoreName, key).pipe(Rx.operators.take(1)).toPromise();
        },

        deleteByKey: async function (indexedDbDatabaseModel, objectStoreName, key) {

            const dbModel = getDbModel(indexedDbDatabaseModel.dbModelGuid).dbModel;

            return await deleteByKey(dbModel, objectStoreName, key).pipe(Rx.operators.take(1)).toPromise();
        },

        deleteAll: async function (indexedDbDatabaseModel, objectStoreName) {

            const dbModel = getDbModel(indexedDbDatabaseModel.dbModelGuid).dbModel;

            return await deleteAll(dbModel, objectStoreName).pipe(Rx.operators.take(1)).toPromise();
        },

        getAll: async function (indexedDbDatabaseModel, objectStoreName) {

            const dbModel = getDbModel(indexedDbDatabaseModel.dbModelGuid).dbModel;

            return await getAll(dbModel, objectStoreName).pipe(Rx.operators.take(1)).toPromise();
        },

        getRange: async function (indexedDbDatabaseModel, objectStoreName, lowerBound, upperBound) {

            const dbModel = getDbModel(indexedDbDatabaseModel.dbModelGuid).dbModel;

            return await getRange(dbModel, objectStoreName, lowerBound, upperBound).pipe(Rx.operators.take(1)).toPromise();
        },

        getByIndex: async function (indexedDbDatabaseModel, objectStoreName, lowerBound, upperBound, dbIndex, isRange) {

            const dbModel = getDbModel(indexedDbDatabaseModel.dbModelGuid).dbModel;

            return await getByIndex(dbModel, objectStoreName, lowerBound, upperBound, dbIndex, isRange).pipe(Rx.operators.take(1)).toPromise();
        },

        getExtent: async function (indexedDbDatabaseModel, objectStoreName, dbIndex, extentType) {

            const dbModel = getDbModel(indexedDbDatabaseModel.dbModelGuid).dbModel;

            return await getExtent(dbModel, objectStoreName, dbIndex, extentTypes[extentType]).pipe(Rx.operators.take(1)).toPromise();
        }

    };
})();