
//var subcategoryStore = Ext.create('Ext.data.Store', {
//    autoLoad: true,
//    autoSync: true,
//    model: subjectDataModelName,
//    proxy: {
//        type: 'rest',
//        url: getApiUrl('/api/subcategories'),
//    },
//    listeners: {
//        write: function (store, operation) {

//        }
//    }
//});


var subcategoryRowEditing = Ext.create('Ext.grid.plugin.RowEditing', {
    clicksToMoveEditor: 1,
    autoCancel: true,
    listeners: {
        cancelEdit: function (rowEditing, context) {
            // Canceling editing of a locally added, unsaved record: remove it
            if (context.record.phantom) {
                subcategoryStore.remove(context.record);
            }
        }
    }
});


var subcategoryGrid = Ext.create('Ext.grid.Panel', {
    plugins: [subcategoryRowEditing],
    autoWidth: true,
    autoHeight: true,
    layout: 'fit',
    frame: true,
    //multiSelect: true,
    stripeRows: true,
    region: 'center',
    store: subcategoryStore,
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
                subcategoryStore.insert(0, { Status: true });
                subcategoryRowEditing.startEdit(0, 0);
            }
        }, '-', {
            itemId: 'delete',
            text: 'Delete',
            iconCls: 'icon-delete',
            disabled: true,
            handler: function () {
                var selection = subcategoryGrid.getView().getSelectionModel().getSelection()[0];
                if (selection) {
                    subcategoryStore.remove(selection);
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

subcategoryGrid.getSelectionModel().on('selectionchange', function (selModel, selections) {
    subcategoryGrid.down('#delete').setDisabled(selections.length === 0);
});

