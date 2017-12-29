var http = require('http');

function requestFactory() {

    
}

requestFactory.create = function () {
    return new requestFactory.Request();
}

function Request() {
}

Request.prototype.getString = function (url, option) {
    if (!url) return '';
    if (!option) {
        option = {};
    }

    option.method = 'GET';
    option.url = url;
    return http.getResult(option);

};

Request.prototype.getJson = function (url, option) {
    if (!url) return {};
    if (!option) {
        option = {};
    }

    option.method = 'GET';
    option.url = url;
    var body = http.getResult(option);
    try {
        body = JSON.parse(body);
    } catch (e) { }
    return body;
};

Request.prototype.post = function (url, data, option) {
    if (!url) return '';
    if (!option) {
        option = {};
    }
    option.method = 'POST';
    option.url = url;
    option.data = data || '';
    if (!option.accept) {
        option.accept = 'application/json';
    }
    var body = http.getResult(option);
    try {
        body = JSON.parse(body);
    } catch (e) { }
    return body;
};

//Request.prototype.go = function () {
//    var me = this;

//    me.req = http.request(me.options, function (res) {
//        if (me.options.callback) {
//            res.on('end', function (body) {
//                me.onEnd(body, me.options.callback);
//            });
//            res.on('err', function (errmsg) {
//                me.options.callback(errmsg);
//            });
//        }
//    });

//    return me.req.end();

//};


//Request.prototype.onEnd = function (body, callback) {
//    var self = this;
   
//   // Console.WriteLine(body);
//    if (self.options.json) {
//        try {
//            body = JSON.parse(body);
//        } catch (e) { }
//    }
//    //Console.WriteLine(body[0].cargoTypeName);
//    callback.call(null, null, body);
//};

//if (typeof module != 'undefined' && module != null) {
//    module.exports = request;
//}

requestFactory.Request = Request;

this.exports = requestFactory;
