

var tabPanel = Ext.create('Ext.tab.Panel', {
    layout: 'fit',
    autoWidth: true,
    autoHeight: true,
    region: 'center',
    enableTabScroll: true,
    split: true,
    items: [
    {
        title: 'New Content',
        items: grid,
        xtype: 'container',
        layout: 'border',
        listeners: {
            deactivate: function () {

            },
            activate: function () {

                itemStatus = "New";
                currentStore = adminTaskStore;

                loadStore();
            }
        }
    },
    {
        title: 'Archived Content',
        items: adminArchivedGrid,
        xtype: 'container',
        layout: 'border',
        listeners: {
            deactivate: function () {

            },
            activate: function () {

                itemStatus = "Archived";
                currentStore = adminArchivedStore;

                loadStore();
            }
        }
    },
    {
        title: 'Category',
        items: categoryGrid,
        xtype: 'container',
        layout: 'border',
        listeners: {
            deactivate: function () {

            },
            activate: function () {
                itemStatus = undefined;

                //categoryStore.load();
            }
        }
    },
    {
        title: 'Sub Category',
        items: subcategoryGrid,
        xtype: 'container',
        layout: 'border',
        listeners: {
            deactivate: function () {

            },
            activate: function () {
                itemStatus = undefined;

                subcategoryStore.load();
            }
        }
    }
    ]
});

