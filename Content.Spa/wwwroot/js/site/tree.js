
Ext.define('Directory', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'id', type: 'int' },
        { name: 'name', type: 'string' },
        { name: 'path', type: 'string' }
    ]
});

var treeStore = Ext.create('Ext.data.TreeStore', {
    model: 'Directory',
    root: {
        text: 'root',

        //name: 'root',
        //expanded: true
    },
    proxy: {
        type: 'ajax',
        url: getApiUrl('/api/ContentFilePath'),
    },
    //folderSort: true
});

mgr.getUser().then(function (user) {
    if (user) {
        console.log('set tree store access token');
        treeStore.getProxy().headers = {
            Authorization: 'Bearer ' + user.access_token
        };

        treeStore.load();
    }
    else {
        console.log('failed to set tree store access token, user is null');
    }
});

var treePanel = Ext.create('Ext.tree.Panel', {
    width: 200,
    minWidth: 80,
    autoheight: true,
    title: 'Directory',
    region: 'west',
    split: true,
    collapsible: true,
    useArrows: true,
    store: treeStore,
    rootVisible: false,
    columns: [
        {
            xtype: 'treecolumn',
            header: 'Name',
            dataIndex: 'name',
            flex: 1
        }
    ],
    listeners: {
        itemclick: function (s, r) {
            var record = treePanel.getSelectionModel().getSelection();

            filePathId = record[0].data.id;

            loadStore();
        }
    }
});