var pageSize = 30;

var itemStatus;
var filePathId;
var currentStore;

var contentFileItemDataModelName = 'ContentFileItem';
var categoryDataModelName = 'Category';
var subcategoryDataModelName = 'SubCategory';

//var jwtHeader = {
//    'Authorization': 'Bearer ' + accessToken
//};

function getApiUrl(relativePath) {
    return 'http://localhost:7002' + relativePath;
}

Ext.define(contentFileItemDataModelName, {
    extend: 'Ext.data.Model',
    idProperty: 'Id',
    fields: [
            'Id',
            'Name',
            'CreatedDate',
            'Status',
            'FilePathId',
            'FinishedBy',
            'VerifiedBy',
            'Categories',
            'Subcategories',
            'FinishedDate',
            'VerifiedDate'
    ]
});

Ext.define(categoryDataModelName, {
    extend: 'Ext.data.Model',
    idProperty: 'Id',
    fields: [
            'Id',
            'Name',
            'Status'
    ]
});

Ext.define(subcategoryDataModelName, {
    extend: 'Ext.data.Model',
    idProperty: 'Id',
    fields: [
            'Id',
            'Name',
            'Status'
    ]
});

function download(fileContentId) {
    //mgr.getUser().then(function (user) {
    //    var url = getApiUrl("/api/download") + "/" + fileContentId;

    //    var xmlHttp = new XMLHttpRequest();
    //    xmlHttp.onreadystatechange = function () {
    //        if (xmlHttp.readyState == 4 && xmlHttp.status == 200) {
    //            //(xmlHttp.responseText); TODO
    //        }
    //    };
    //    xmlHttp.open("GET", url, true); // true for asynchronous 
    //    xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
    //    xmlHttp.send(null);
    //});
}

function renderFileName(v, p, r) {

    //return Ext.String.format(
    //    '<a href ="#" alt="target file" target="_blank" onclick="download({0})")>{1}</a>',
    //    r.get('Id'), v);

    return Ext.String.format(
        '<a href ="{2}/{0}" alt="target file" target="_blank")>{1}</a>',
        r.get('Id'), v, getApiUrl('/api/download'));

}

function renderUser(v, p, r) {
    if (v) {
        //if (categoryStore.data.length == 0) {
        //    //adminUserStore.load(
        //    //        {
        //    //            callback: function (r, o, s) {
        //    //                if (s) {
        //    //                    var usr = adminUserStore.getById(v);
        //    //                    return usr.data.DisplayName;
        //    //                }
        //    //            }
        //    //        });
        //    return;
        //}

        //var usr = categoryStore.getById(v);
        //return usr.data.DisplayName;
    }
}

function loadStore() {
    if (filePathId && itemStatus) {
        currentStore.load({
            params: {
                filePathId: filePathId,
                status: itemStatus
            },
            callback: function (r, o, s) {
                if (s) {
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

