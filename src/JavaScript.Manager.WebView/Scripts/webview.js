var javascript_webview_factory_webviewExecutor = javascript_webview_factory_webviewExecutor || require('javascript_webview_factory_webviewExecutor');

function webviewFactory() {


}

WebViewContext.prototype.extend = (function () {
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

webviewFactory.create = function (options) {
    if (!options) {
        options = {};
    }
    return new webviewFactory.WebViewContext(options);
}

function WebViewContext(options) {
    this.options = options;
}


WebViewContext.prototype.show = function (ops) {
    javascript_webview_factory_webviewExecutor.show(this.extend({ param: ops }, this.options));
}

WebViewContext.prototype.close = function () {
     javascript_webview_factory_webviewExecutor.close();
}

WebViewContext.prototype.on = function (name,callback) {
    javascript_webview_factory_webviewExecutor.on(name, callback);
}

WebViewContext.prototype.getInitCookieString = function () {
    return javascript_webview_factory_webviewExecutor.getInitCookieString();
}

WebViewContext.prototype.getInitCookieContainer = function () {
    return javascript_webview_factory_webviewExecutor.getInitCookieContainer();
}

WebViewContext.prototype.execJs = function (js) {
    return javascript_webview_factory_webviewExecutor.execJs(js);
}

WebViewContext.prototype.getDomHtml = function () {
    return javascript_webview_factory_webviewExecutor.getDomHtml();
}

WebViewContext.prototype.createTimer = function (interval) {
    return javascript_webview_factory_webviewExecutor.createTimer(interval);
}

//WebViewContext.prototype.getMillisecondTime = function (type) {
//    return javascript_webview_factory_webviewExecutor.getMillisecondTime(type);
//}

webviewFactory.WebViewContext = WebViewContext;
this.exports = webviewFactory;