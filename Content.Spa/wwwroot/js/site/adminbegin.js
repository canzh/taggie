
var categoryStore = Ext.create('Ext.data.Store', {
    //autoLoad: true,
    autoSync: true,
    model: categoryDataModelName,
    proxy: {
        type: 'rest',
        url: getApiUrl('/api/categories'),
    },
    listeners: {
        write: function (store, operation) {
        },
        beforeload: function () {
        },
        load: function () {
        }
    }
});


var subcategoryStore = Ext.create('Ext.data.Store', {
    //autoLoad: true,
    autoSync: true,
    model: subcategoryDataModelName,
    proxy: {
        type: 'rest',
        url: getApiUrl('/api/subcategories'),
    },
    listeners: {
        write: function (store, operation) {
        },
        beforeload: function () {
        },
        load: function () {
        }
    }
});

mgr.getUser().then(function (user) {
    if (user) {
        console.log('set subcategoryStore access token');
        categoryStore.getProxy().headers = {
            Authorization: 'Bearer ' + user.access_token
        };

        categoryStore.load();

        subcategoryStore.getProxy().headers = {
            Authorization: 'Bearer ' + user.access_token
        };
    }
    else {
        console.log('failed to set subcategoryStore access token, user is null');
    }
});


function keywordStoreContainsAllKeyword(keyword) {

    for (var k in keyword) {
        var r = categoryStore.findRecord('Name', kws[k]);

        if (!r) {
            return false;
        }
    }

    return true;
}

function subjectStoreContainsAllSubject(subject) {

    for (var s in subject) {
        var r = subcategoryStore.findRecord('Name', subs[s]);

        if (!r) {
            return false;
        }
    }

    return true;
}