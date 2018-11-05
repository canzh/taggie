function Store() {
    function retrieve(key) {
        var item = sessionStorage.getItem(key);

        if (item && item !== 'undefined') {
            return JSON.parse(sessionStorage.getItem(key));
        }

        return;
    }

    function store(key, value) {
        sessionStorage.setItem(key, JSON.stringify(value));
    }
}

var storage = new Store();
var IsAuthorized = false;
var authorityUrl = "http://localhost:7001";

storage.store("IdentityUrl", authorityUrl);

function ResetAuthorizationData() {
    this.storage.store('authorizationData', '');
    this.storage.store('authorizationDataIdToken', '');

    this.IsAuthorized = false;
    this.storage.store('IsAuthorized', false);
}

function login() {
    this.ResetAuthorizationData();

    var authorizationUrl = this.authorityUrl + '/connect/authorize';
    var client_id = 'js';
    var redirect_uri = location.origin + '/home/task';
    var response_type = 'id_token token';
    var scope = 'openid profile api1';
    var nonce = 'N' + Math.random() + '' + Date.now();
    var state = Date.now() + '' + Math.random();

    this.storage.store('authStateControl', state);
    this.storage.store('authNonce', nonce);

    var url =
        authorizationUrl + '?' +
        'response_type=' + encodeURI(response_type) + '&' +
        'client_id=' + encodeURI(client_id) + '&' +
        'redirect_uri=' + encodeURI(redirect_uri) + '&' +
        'scope=' + encodeURI(scope) + '&' +
        'nonce=' + encodeURI(nonce) + '&' +
        'state=' + encodeURI(state);

    window.location.href = url;
}

function logoff() {
    var authorizationUrl = this.authorityUrl + '/connect/endsession';
    var id_token_hint = this.storage.retrieve('authorizationDataIdToken');
    var post_logout_redirect_uri = location.origin + '/';

    var url =
        authorizationUrl + '?' +
        'id_token_hint=' + encodeURI(id_token_hint) + '&' +
        'post_logout_redirect_uri=' + encodeURI(post_logout_redirect_uri);

    this.ResetAuthorizationData();

    // emit observable
    //this.authenticationSource.next(false);
    window.location.href = url;
}

function authorizedCallback() {
    this.ResetAuthorizationData();

    var hash = window.location.hash.substr(1);

    var result = hash.split('&').reduce(function (result, item) {
        var parts = item.split('=');
        result[parts[0]] = parts[1];
        return result;
    }, {});

    console.log(result);

    var token = '';
    var id_token = '';
    var authResponseIsValid = false;

    if (!result.error) {

        if (result.state !== this.storage.retrieve('authStateControl')) {
            console.log('AuthorizedCallback incorrect state');
        } else {

            token = result.access_token;
            id_token = result.id_token;

            var dataIdToken = this.getDataFromToken(id_token);
            console.log(dataIdToken);

            // validate nonce
            if (dataIdToken.nonce !== this.storage.retrieve('authNonce')) {
                console.log('AuthorizedCallback incorrect nonce');
            } else {
                this.storage.store('authNonce', '');
                this.storage.store('authStateControl', '');

                authResponseIsValid = true;
                console.log('AuthorizedCallback state and nonce validated, returning access token');
            }
        }
    }

    if (authResponseIsValid) {
        this.SetAuthorizationData(token, id_token);
    }
}

function getDataFromToken(token) {
    var data = {};
    if (typeof token !== 'undefined') {
        var encoded = token.split('.')[1];
        data = JSON.parse(this.urlBase64Decode(encoded));
    }

    return data;
}

var UserData;

function SetAuthorizationData(token, id_token) {
    if (this.storage.retrieve('authorizationData') !== '') {
        this.storage.store('authorizationData', '');
    }

    this.storage.store('authorizationData', token);
    this.storage.store('authorizationDataIdToken', id_token);
    this.IsAuthorized = true;
    this.storage.store('IsAuthorized', true);

    getUserData(function (data) {
        this.UserData = data;
        this.storage.store('userData', data);
        window.location.href = location.origin;
    });
}

function urlBase64Decode(str) {
    var output = str.replace('-', '+').replace('_', '/');
    switch (output.length % 4) {
        case 0:
            break;
        case 2:
            output += '==';
            break;
        case 3:
            output += '=';
            break;
        default:
            throw 'Illegal base64url string!';
    }

    return window.atob(output);
}

function httpGetAsync(theUrl, callback) {
    var token = GetToken();

    var xmlHttp = new XMLHttpRequest();
    xmlHttp.onreadystatechange = function () {
        if (xmlHttp.readyState == 4 && xmlHttp.status == 200)
            callback(xmlHttp.responseText);
    }
    xmlHttp.open("GET", theUrl, true); // true for asynchronous 
    xmlhttp.setRequestHeader('Content-Type', 'application/json');
    xmlhttp.setRequestHeader('Accept', 'application/json');
    xmlhttp.setRequestHeader('Authorization', 'Bearer ' + token);
    xmlHttp.send(null);
}

function getUserData(callback) {
    if (this.authorityUrl === '')
        this.authorityUrl = this.storage.retrieve('IdentityUrl');

    httpGetAsync(this.authorityUrl + '/connect/userinfo', callback);
}

function GetToken() {
    return this.storage.retrieve('authorizationData');
}