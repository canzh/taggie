// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

Oidc.Log.logger = window.console;
Oidc.Log.level = Oidc.Log.INFO;

var config = {
    authority: "http://localhost:7001",
    client_id: "js",
    redirect_uri: "http://localhost:7003/callback.html",
    response_type: "token id_token",
    scope: "openid profile api1",
    post_logout_redirect_uri: "http://localhost:7003",
};
var mgr = new Oidc.UserManager(config);

function login() {
    mgr.signinRedirect();
}

function api() {
    mgr.getUser().then(function (user) {
        var url = "http://localhost:7002/api/categories";

        var xhr = new XMLHttpRequest();
        xhr.open("GET", url);
        xhr.onload = function () {
            log(xhr.status, JSON.parse(xhr.responseText));
        }
        xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
        xhr.send();
    });
}

function logout() {
    mgr.signoutRedirect();
}

function log() {
    document.getElementById('results').innerText = '';

    Array.prototype.forEach.call(arguments, function (msg) {
        if (msg instanceof Error) {
            msg = "Error: " + msg.message;
        }
        else if (typeof msg !== 'string') {
            msg = JSON.stringify(msg, null, 2);
        }
        document.getElementById('results').innerHTML += msg + '\r\n';
    });
}

//var config = {
//    authority: "http://localhost:7001",
//    client_id: "js",
//    redirect_uri: window.location.origin + "/home/task",
//    post_logout_redirect_uri: window.location.origin + "/",

//    // if we choose to use popup window instead for logins
//    popup_redirect_uri: window.location.origin + "/popup.html",
//    popupWindowFeatures: "menubar=yes,location=yes,toolbar=yes,width=1200,height=800,left=100,top=100;resizable=yes",

//    // these two will be done dynamically from the buttons clicked, but are
//    // needed if you want to use the silent_renew
//    response_type: "id_token token",
//    scope: "openid profile email api1 api2.read_only",

//    // this will toggle if profile endpoint is used
//    loadUserInfo: true,

//    // silent renew will get a new access_token via an iframe 
//    // just prior to the old access_token expiring (60 seconds prior)
//    silent_redirect_uri: window.location.origin + "/silent.html",
//    automaticSilentRenew: true,

//    // will revoke (reference) access tokens at logout time
//    revokeAccessTokenOnSignout: true,

//    // this will allow all the OIDC protocol claims to be visible in the window. normally a client app 
//    // wouldn't care about them or want them taking up space
//    filterProtocolClaims: false
//};

//Oidc.Log.logger = window.console;
//Oidc.Log.level = Oidc.Log.INFO;

//var mgr = new Oidc.UserManager(config);

//mgr.events.addUserLoaded(function (user) {
//    log("User loaded");
//    showTokens();
//});
//mgr.events.addUserUnloaded(function () {
//    log("User logged out locally");
//    showTokens();
//});
//mgr.events.addAccessTokenExpiring(function () {
//    log("Access token expiring...");
//});
//mgr.events.addSilentRenewError(function (err) {
//    log("Silent renew error: " + err.message);
//});
//mgr.events.addUserSignedOut(function () {
//    log("User signed out of OP");
//});

//function login(scope, response_type) {
//    var use_popup = false;
//    if (!use_popup) {
//        mgr.signinRedirect({ scope: scope, response_type: response_type });
//    }
//    else {
//        mgr.signinPopup({ scope: scope, response_type: response_type }).then(function () {
//            log("Logged In");
//        });
//    }
//}

//function logout() {
//    mgr.signoutRedirect();
//}

//function revoke() {
//    mgr.revokeAccessToken();
//}

//function callApi() {
//    mgr.getUser().then(function (user) {
//        var xhr = new XMLHttpRequest();
//        xhr.onload = function (e) {
//            if (xhr.status >= 400) {
//                display("#ajax-result", {
//                    status: xhr.status,
//                    statusText: xhr.statusText,
//                    wwwAuthenticate: xhr.getResponseHeader("WWW-Authenticate")
//                });
//            }
//            else {
//                display("#ajax-result", xhr.response);
//            }
//        };
//        xhr.open("GET", "http://localhost:7002/api/categories", true);
//        xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
//        xhr.send();
//    });
//}

//if (window.location.hash) {
//    handleCallback();
//}

//[].forEach.call(document.querySelectorAll(".request"), function (button) {
//    button.addEventListener("click", function () {
//        login(this.dataset["scope"], this.dataset["type"]);
//    });
//});

//document.querySelector(".call").addEventListener("click", callApi, false);
//document.querySelector(".revoke").addEventListener("click", revoke, false);
//document.querySelector(".logout").addEventListener("click", logout, false);


//function log(data) {
//    document.getElementById('response').innerText = '';

//    Array.prototype.forEach.call(arguments, function (msg) {
//        if (msg instanceof Error) {
//            msg = "Error: " + msg.message;
//        }
//        else if (typeof msg !== 'string') {
//            msg = JSON.stringify(msg, null, 2);
//        }
//        document.getElementById('response').innerHTML += msg + '\r\n';
//    });
//}

//function display(selector, data) {
//    if (data && typeof data === 'string') {
//        try {
//            data = JSON.parse(data);
//        }
//        catch (e) { }
//    }
//    if (data && typeof data !== 'string') {
//        data = JSON.stringify(data, null, 2);
//    }
//    document.querySelector(selector).textContent = data;
//}

//function showTokens() {
//    mgr.getUser().then(function (user) {
//        if (user) {
//            display("#id-token", user);
//        }
//        else {
//            log("Not logged in");
//        }
//    });
//}
//showTokens();

//function handleCallback() {
//    mgr.signinRedirectCallback().then(function (user) {
//        var hash = window.location.hash.substr(1);
//        var result = hash.split('&').reduce(function (result, item) {
//            var parts = item.split('=');
//            result[parts[0]] = parts[1];
//            return result;
//        }, {});

//        log(result);
//        showTokens();

//        window.history.replaceState({},
//            window.document.title,
//            window.location.origin + window.location.pathname);

//    }, function (error) {
//        log(error);
//    });
//}