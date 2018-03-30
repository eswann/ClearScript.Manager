var javascript_request_factory_http = javascript_request_factory_http || require('javascript_request_factory_http');

function requestFactory() {

    
}

requestFactory.create = function (option) {
    
    return new requestFactory.Request(option);
}

function Request(option) {
    this.options = option || {};
    if (!this.options.method) {
        this.options.method = 'GET';
    }
   
    if (this.options.method.toLowerCase() != 'get') {
        if (!this.options.accept) {
            this.options.accept = 'application/json';
        }
    }
}

Request.prototype.getString = function (option) {
    return javascript_request_factory_http.getResult(this.extend(option, this.options));
};

Request.prototype.getJson = function (option) {
    var body = javascript_request_factory_http.getResult(this.extend(option, this.options));
    try {
        body = JSON.parse(body);
    } catch (e) { }
    return body;
};

Request.prototype.extend = (function () {
    var isObjFunc = function (name) {
        var toString = Object.prototype.toString;
        return function () {
            return toString.call(arguments[0]) === '[object ' + name + ']';
        }
    }
    var isObject = isObjFunc('Object'),
        isArray = isObjFunc('Array'),
        isBoolean = isObjFunc('Boolean');
    return function extend() {
        var index = 0, isDeep = false, obj, copy, destination, source, i;
        if (isBoolean(arguments[0])) {
            index = 1;
            isDeep = arguments[0];
        }
        for (i = arguments.length - 1; i > index; i--) {
            destination = arguments[i - 1];
            source = arguments[i];
            if (isObject(source) || isArray(source)) {
                for (var property in source) {
                    obj = source[property];
                    if (isDeep && (isObject(obj) || isArray(obj))) {
                        copy = isObject(obj) ? {} : [];
                        var extended = extend(isDeep, copy, obj);
                        destination[property] = extended;
                    } else {
                        destination[property] = source[property];
                    }
                }
            } else {
                destination = source;
            }
        }
        return destination;
    }
})();

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
