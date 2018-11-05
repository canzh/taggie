var archiveStoreMask;

var adminArchivedStore = Ext.create('Ext.data.Store', {
    //autoLoad: true,
    autoSync: true,
    pageSize: pageSize,
    model: contentFileItemDataModelName,
    proxy: {
        type: 'rest',
        url: getApiUrl('/api/contentfile'),
        reader: {
            root: 'data',
            totalProperty: 'total'
        }
    },
    listeners: {
        write: function (store, operation) {

        },
        beforeload: function () {
            archiveStoreMask.show();
        },
        load: function () {
            archiveStoreMask.hide();
        }
    }
});

mgr.getUser().then(function (user) {
    if (user) {
        console.log('set archive store access token');
        adminArchivedStore.getProxy().headers = {
            Authorization: 'Bearer ' + user.access_token
        };
    }
    else {
        console.log('failed to set archives store access token, user is null');
    }
});

var archiveAssignMenu = Ext.create('Ext.menu.Menu', {});

var adminArchiveContextMenu = Ext.create('Ext.menu.Menu', {
    items: [
        {
            text: 'Assign To',
            menu: archiveAssignMenu
        },
    ],
    listeners: {
        'beforeshow': function () {

            categoryStore.load(
                {
                    callback: function (r, o, s) {
                        if (s) {
                            archiveAssignMenu.removeAll();

                            for (var i = 0; i < categoryStore.data.length; i++) {

                                var menuAction = Ext.create('Ext.Action', {
                                    itemId: categoryStore.data.items[i].data.id,
                                    text: categoryStore.data.items[i].data.DisplayName,
                                    handler: function (widget, event) {
                                        var count = adminArchivedGrid.getSelectionModel().getCount();
                                        var record = adminArchivedGrid.getSelectionModel().getSelection();
                                        var ids = '';

                                        for (var i = 0; i < count; i++) {
                                            ids += record[i].data.id + ';';
                                        }

                                        Ext.Ajax.request({
                                            method: 'GET',
                                            url: '/api/ContentSources',
                                            params: {
                                                assignedToId: this.itemId,
                                                ids: ids
                                            },
                                            success: function () {
                                                adminArchivedStore.suspendAutoSync();
                                                adminArchivedStore.remove(record);
                                                adminArchivedStore.resumeAutoSync();
                                            },

                                            failure: function (r) {
                                                Ext.Msg.show({
                                                    title: 'Assign failed',
                                                    msg: r.responseText,
                                                    buttons: Ext.Msg.OK,
                                                    icon: Ext.Msg.ERROR
                                                });
                                            }
                                        });

                                    }
                                });

                                archiveAssignMenu.add(menuAction);
                            }
                        } else {
                            Ext.Msg.show({
                                title: 'Connection error.',
                                msg: 'Not able to pull sourcing data from server.',
                                buttons: Ext.Msg.OK,
                                icon: Ext.Msg.ERROR
                            });
                        }

                    }
                });
        }
    }
});

var Type_PagingToolBarArchive = Ext.extend(Ext.PagingToolbar, {
    constructor: function () {
        Type_PagingToolBarArchive.superclass.constructor.call(this, {
            store: adminArchivedStore,
            displayInfo: true,
            plugins: Ext.create('Ext.ux.ProgressBarPager', {}),
            listeners: {
                beforechange: function (t, p, o) {
                    adminArchivedStore.getProxy().extraParams = {
                        filePathId: filePathId,
                        itemStatus: itemStatus
                    };
                    return true;
                }
            }
        });
    }
});
var archivePagingToolBar = new Type_PagingToolBarArchive();

var adminArchivedGrid = Ext.create('Ext.grid.Panel', {
    autoWidth: true,
    autoHeight: true,
    layout: 'fit',
    frame: true,
    multiSelect: true,
    stripeRows: true,
    region: 'center',
    store: adminArchivedStore,
    bbar: archivePagingToolBar,
    columns: [
        Ext.create('Ext.grid.RowNumberer', {
            //autoWidth: true,
            width: 40
        }), {
            text: 'File Name',
            width: 200,
            sortable: true,
            dataIndex: 'Name',
            renderer: renderFileName,
        }, {
            header: 'Created Date',
            width: 120,
            sortable: true,
            dataIndex: 'CreatedDate',
            renderer: Ext.util.Format.dateRenderer('m-d-Y'),
        }, {
            text: 'Category',
            width: 260,
            sortable: true,
            dataIndex: 'Categories',
        }, {
            text: 'Sub Category',
            width: 260,
            sortable: true,
            dataIndex: 'Subcategories',
        },
        //Ext.create('Ext.ux.CheckColumn', {
        //    text: 'Headnote',
        //    dataIndex: 'Headnote',
        //    listeners: {
        //        checkchange: function (self, row, checked) {

        //        }
        //    }
        //    })
    ],
    listeners: {
        edit: function (editor, e) {

        },
        itemcontextmenu: function (view, rec, node, index, e) {
            e.stopEvent();
            //adminArchiveContextMenu.showAt(e.getXY());
            return false;
        }
    }
});

archiveStoreMask = new Ext.LoadMask(adminArchivedGrid, { msg: "Loading ..." });
