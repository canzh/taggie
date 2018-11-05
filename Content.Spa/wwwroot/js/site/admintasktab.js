var taskStoreMask;

var adminTaskStore = Ext.create('Ext.data.Store', {
    //autoLoad: true,
    autoSync: true,
    pageSize: pageSize,
    model: contentFileItemDataModelName,
    proxy: {
        url: getApiUrl('/api/contentfile'),
        type: 'rest',
        reader: {
            root: 'data',
            totalProperty: 'total'
        },
    },
    listeners: {
        write: function (store, operation) {

        },
        beforeload: function () {
            taskStoreMask.show();
        },
        load: function () {
            taskStoreMask.hide();
        },
    }
});

mgr.getUser().then(function (user) {
    if (user) {
        console.log('set task store access token');
        adminTaskStore.getProxy().headers = {
            Authorization: 'Bearer ' + user.access_token
        };
    }
    else {
        console.log('failed to set task store access token, user is null');
    }
});

var adminTaskArchiveAction = Ext.create('Ext.Action', {
    icon: '',
    text: 'Archive',
    disabled: false,
    handler: function (widget, event) {
        var count = grid.getSelectionModel().getCount();
        var record = grid.getSelectionModel().getSelection();
        var ids = '';

        for (var i = 0; i < count; i++) {
            ids += record[i].data.Id + ';';
        }

        mgr.getUser().then(function (user) {
            if (user) {
                Ext.Ajax.request({
                    method: 'PUT',
                    url: getApiUrl('/api/contentfile/action'),
                    headers: {
                        Authorization: 'Bearer ' + user.access_token
                    },
                    params: {
                        ids: ids,
                        actionType: 'Archive'
                    },
                    success: function () {
                        adminTaskStore.suspendAutoSync();
                        adminTaskStore.remove(record);
                        adminTaskStore.resumeAutoSync();
                    },

                    failure: function (r) {
                        Ext.Msg.show({
                            title: 'Archive failed',
                            msg: r.responseText,
                            buttons: Ext.Msg.OK,
                            icon: Ext.Msg.ERROR
                        });
                    }
                });
            }
            else {
                console.log('failed to do archive action, user is null');
            }
        });
        
    }
});


var assignMenu = Ext.create('Ext.menu.Menu', {});

var adminTaskContextMenu = Ext.create('Ext.menu.Menu', {
    items: [
            //{
            //    text: 'Assign To',
            //    menu: assignMenu
            //},
            adminTaskArchiveAction,
    ],
    listeners: {
        'beforeshow': function () {
            
        }
    }
});

var rowEditing = Ext.create('Ext.grid.plugin.RowEditing', {
    //clicksToEdit: 0,
    clicksToMoveEditor: 2,
    autoCancel: true,
    listeners: {
        cancelEdit: function (rowEditing, context) {
        }
    }
});

var Type_PagingToolBarTask = Ext.extend(Ext.PagingToolbar, {
    constructor: function () {
        Type_PagingToolBarTask.superclass.constructor.call(this, {
            store: adminTaskStore,
            displayInfo: true,
            plugins: Ext.create('Ext.ux.ProgressBarPager', {}),
            listeners: {
                beforechange: function (t, p, o) {
                    adminTaskStore.getProxy().extraParams = {
                        filePathId: filePathId,
                        itemStatus: itemStatus
                    };
                    return true;
                }
            }
        });
    }
});
var taskPagingToolBar = new Type_PagingToolBarTask();

var grid = Ext.create('Ext.grid.Panel', {
    plugins: [rowEditing],
    autoWidth: true,
    autoHeight: true,
    autoScroll: true,
    layout: 'fit',
    frame: true,
    multiSelect: true,
    stripeRows: true,
    region: 'center',
    store: adminTaskStore,
    bbar: taskPagingToolBar,
    columns: [
    Ext.create('Ext.grid.RowNumberer', {
        autoWidth: true,
        //width: 40
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
        editor: Ext.create('Ext.ux.form.field.BoxSelect', {
            style: 'margin-left:10px',
            store: categoryStore,
            delimiter: ';',
            valueField: 'Name',
            displayField: 'Name',
            typeAhead: true,
            queryMode: 'local',
            multiSelect: true,
            forceSelection: true,
            selectOnTab: true,
            filterPickList: true,
            pinList: false
        })
    }, {
        text: 'Sub Category',
        width: 260,
        sortable: true,
        dataIndex: 'Subcategories',
        editor: Ext.create('Ext.ux.form.field.BoxSelect', {
            style: 'margin-left:10px',
            store: subcategoryStore,
            delimiter: ';',
            valueField: 'Name',
            displayField: 'Name',
            typeAhead: true,
            queryMode: 'remote',
            multiSelect: true,
            forceSelection: true,
            selectOnTab: true,
            filterPickList: true,
            pinList: false,
            editable: true,
            hideTrigger: true,
            enableKeyEvents: true,
            minChars: 2,
            triggerAction: 'query',
            triggerOnClick: false
        })
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
            adminTaskContextMenu.showAt(e.getXY());
            return false;
        }
    }
});

taskStoreMask = new Ext.LoadMask(grid, { msg: "Loading ..." });

