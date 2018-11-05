var userTaskStoreMask;

var userTaskStore = Ext.create('Ext.data.Store', {
    //autoLoad: true,
    autoSync: true,
    pageSize: pageSize,
    model: contentFileItemDataModelName,
    proxy: {
        type: 'rest',
        url: '/api/contentsources',
        reader: {
            root: 'data',
            totalProperty: 'total'
        },
    },
    listeners: {
        write: function (store, operation) {

        },
        beforeload: function () {
            userTaskStoreMask.show();
        },
        load: function () {
            userTaskStoreMask.hide();
        }
    }
});

var userTaskCompleteAction = Ext.create('Ext.Action', {
    icon: '',
    text: 'Complete',
    disabled: false,
    handler: function (widget, event) {
        var count = userTaskGrid.getSelectionModel().getCount();
        var record = userTaskGrid.getSelectionModel().getSelection();
        var ids = '';

        for (var i = 0; i < count; i++) {
            ids += record[i].data.id + ';';
        }

        Ext.Ajax.request({
            method: 'GET',
            url: '/api/ContentSources',
            params: {
                ids: ids,
                actionType: 'Complete'
            },
            success: function () {
                userTaskStore.suspendAutoSync();
                userTaskStore.remove(record);
                userTaskStore.resumeAutoSync();
            },

            failure: function (r) {
                Ext.Msg.show({
                    title: 'Complete failed',
                    msg: r.responseText,
                    buttons: Ext.Msg.OK,
                    icon: Ext.Msg.ERROR
                });
            }
        });
    }
});

var userTaskContextMenu = Ext.create('Ext.menu.Menu', {
    items: [
            userTaskCompleteAction,
    ]
});

var Type_PagingToolBarUserTask = Ext.extend(Ext.PagingToolbar, {
    constructor: function () {
        Type_PagingToolBarUserTask.superclass.constructor.call(this, {
            store: userTaskStore,
            displayInfo: true,
            plugins: Ext.create('Ext.ux.ProgressBarPager', {}),
            listeners: {
                beforechange: function (t, p, o) {
                    userTaskStore.getProxy().extraParams = {
                        filePathId: filePathId,
                        itemStatus: itemStatus
                    };
                    return true;
                }
            }
        });
    }
});
var userTaskPagingToolBar = new Type_PagingToolBarUserTask();

var userTaskGrid = Ext.create('Ext.grid.Panel', {
    autoWidth: true,
    autoHeight: true,
    layout: 'fit',
    frame: true,
    multiSelect: true,
    stripeRows: true,
    region: 'center',
    store: userTaskStore,
    bbar: userTaskPagingToolBar,
    columns: [
    Ext.create('Ext.grid.RowNumberer', {
        //autoWidth: true,
        width: 40
    }), {
        text: 'File Name',
        width: 200,
        sortable: true,
        dataIndex: 'FileName',
        renderer: renderFileName,
    }, {
        header: 'Assigned Date',
        width: 130,
        sortable: true,
        dataIndex: 'AssignedDate',
        renderer: Ext.util.Format.dateRenderer('m-d-Y g:i:s A'),
    }, {
        text: 'Subject',
        width: 260,
        sortable: true,
        dataIndex: 'Subjects',
    }, {
        text: 'Keyword',
        width: 260,
        sortable: true,
        dataIndex: 'Keywords',
    }],
    listeners: {
        edit: function (editor, e) {

        },
        itemcontextmenu: function (view, rec, node, index, e) {
            e.stopEvent();
            userTaskContextMenu.showAt(e.getXY());
            return false;
        }
    }
});



var tabPanel = Ext.create('Ext.tab.Panel', {
    layout: 'fit',
    autoWidth: true,
    autoHeight: true,
    region: 'center',
    enableTabScroll: true,
    split: true,
    items: [
    {
        title: 'My Task',
        items: userTaskGrid,
        xtype: 'container',
        layout: 'border',
        listeners: {
            deactivate: function () {

            },
            activate: function () {

                itemStatus = "Assigned";
                currentStore = userTaskStore;

                loadStore();
            }
        }
    },
     {
         title: 'Completed',
         items: userCompletedGrid,
         xtype: 'container',
         layout: 'border',
         listeners: {
             deactivate: function () {

             },
             activate: function () {
                 itemStatus = "Finished";
                 currentStore = userCompletedStore;

                 loadStore();
             }
         }
     }
    ]
});

userTaskStoreMask = new Ext.LoadMask(userTaskGrid, { msg: "Loading ..." });
