var userCompletedStoreMask;

var userCompletedStore = Ext.create('Ext.data.Store', {
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
            userCompletedStoreMask.show();
        },
        load: function () {
            userCompletedStoreMask.hide();
        }
    }
});

var userInCompleteAction = Ext.create('Ext.Action', {
    icon: '',
    text: 'Incomplete',
    disabled: false,
    handler: function (widget, event) {
        var count = userCompletedGrid.getSelectionModel().getCount();
        var record = userCompletedGrid.getSelectionModel().getSelection();
        var ids = '';

        for (var i = 0; i < count; i++) {
            ids += record[i].data.id + ';';
        }

        Ext.Ajax.request({
            method: 'GET',
            url: '/api/ContentSources',
            params: {
                ids: ids,
                actionType: 'Incomplete'
            },
            success: function () {
                userCompletedStore.suspendAutoSync();
                userCompletedStore.remove(record);
                userCompletedStore.resumeAutoSync();
            },

            failure: function (r) {
                Ext.Msg.show({
                    title: 'Incomplete failed',
                    msg: r.responseText,
                    buttons: Ext.Msg.OK,
                    icon: Ext.Msg.ERROR
                });
            }
        });
    }
});

var userCompletedContextMenu = Ext.create('Ext.menu.Menu', {
    items: [
            userInCompleteAction,
    ]
});

var Type_PagingToolBarUserCompleted = Ext.extend(Ext.PagingToolbar, {
    constructor: function () {
        Type_PagingToolBarUserCompleted.superclass.constructor.call(this, {
            store: userCompletedStore,
            displayInfo: true,
            plugins: Ext.create('Ext.ux.ProgressBarPager', {}),
            listeners: {
                beforechange: function (t, p, o) {
                    userCompletedStore.getProxy().extraParams = {
                        filePathId: filePathId,
                        itemStatus: itemStatus
                    };
                    return true;
                }
            }
        });
    }
});
var userCompletedPagingToolBar = new Type_PagingToolBarUserCompleted();

var userCompletedGrid = Ext.create('Ext.grid.Panel', {
    autoWidth: true,
    autoHeight: true,
    layout: 'fit',
    frame: true,
    multiSelect: true,
    stripeRows: true,
    region: 'center',
    store: userCompletedStore,
    bbar: userCompletedPagingToolBar,
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
    }, {
        text: 'Completed Date',
        width: 130,
        sortable: true,
        dataIndex: 'FinishedDate',
        renderer: Ext.util.Format.dateRenderer('m-d-Y g:i:s A'),
    }],
    listeners: {
        edit: function (editor, e) {

        },
        itemcontextmenu: function (view, rec, node, index, e) {
            e.stopEvent();
            userCompletedContextMenu.showAt(e.getXY());
            return false;
        }
    }
});

userCompletedStoreMask = new Ext.LoadMask(userCompletedGrid, { msg: "Loading ..." });
