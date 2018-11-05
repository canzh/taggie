
//var categoryStore = Ext.create('Ext.data.Store', {
//    autoLoad: true,
//    autoSync: true,
//    model: keywordDataModelName,
//    proxy: {
//        type: 'rest',
//        url: getApiUrl('/api/categories'),
//    },
//    listeners: {
//        write: function (store, operation) {

//        }
//    }
//});


var categoryRowEditing = Ext.create('Ext.grid.plugin.RowEditing', {
    clicksToMoveEditor: 1,
    autoCancel: true,
    listeners: {
        cancelEdit: function (rowEditing, context) {
            // Canceling editing of a locally added, unsaved record: remove it
            if (context.record.phantom) {
                categoryStore.remove(context.record);
            }
        }
    }
});


var categoryGrid = Ext.create('Ext.grid.Panel', {
    plugins: [categoryRowEditing],
    autoWidth: true,
    autoHeight: true,
    layout: 'fit',
    frame: true,
    //multiSelect: true,
    stripeRows: true,
    region: 'center',
    store: categoryStore,
    columns: [
    //Ext.create('Ext.grid.RowNumberer', {
    //    //autoWidth: true,
    //    width: 40
    //}),
    {
        text: 'Name',
        width: 200,
        sortable: true,
        dataIndex: 'Name',
        field: {
            xtype: 'textfield'
        }
    },
    Ext.create('Ext.ux.CheckColumn', {
        text: 'Is Active',
        dataIndex: 'Status',
        listeners: {
            checkchange: function (self, row, checked) {

            }
        }
    })],
    dockedItems: [{
        xtype: 'toolbar',
        items: [{
            text: 'Add',
            iconCls: 'icon-add',
            handler: function () {
                // empty record
                categoryStore.insert(0, {Status: true});
                categoryRowEditing.startEdit(0, 0);
            }
        }, '-', {
            itemId: 'delete',
            text: 'Delete',
            iconCls: 'icon-delete',
            disabled: true,
            handler: function () {
                var selection = categoryGrid.getView().getSelectionModel().getSelection()[0];
                if (selection) {
                    categoryStore.remove(selection);
                }
            }
        }]
    }],
    listeners: {
        edit: function (editor, e) {
            e.grid.getView().refresh();
        },
        itemcontextmenu: function (view, rec, node, index, e) {

        }
    }
});

categoryGrid.getSelectionModel().on('selectionchange', function (selModel, selections) {
    categoryGrid.down('#delete').setDisabled(selections.length === 0);
});

