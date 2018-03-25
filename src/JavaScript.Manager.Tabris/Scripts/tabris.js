var javascript_sql_factory = require('javascript_sql_factory');
var javascript_request_factory = require('javascript_request_factory');
var javascript_log_factory = require('javascript_log_factory');
var javascript_webview_factory = require('javascript_webview_factory');
var javascript_timer_factory = require('javascript_timer_factory');

function tabrisFactory() {

}


tabrisFactory.create = function (type, options) {
    if (!type) return undefined;
    type = type.trim().toLowerCase();
    if (type === 'sql') {
        if (!options.name || !options.type) return undefined;
        return javascript_sql_factory.create(options);
    }else if (type === 'http') {
        return javascript_request_factory.create(options);
    } else if (type === 'log') {
        return javascript_log_factory.create(options);
    }
    else if (type === 'view') {
        return javascript_webview_factory.create(options);
    } else if (type === 'timer') {
        return javascript_timer_factory.create(options);
    } else if (type === 'clr') {
        if (!options) return xHost.lib('mscorlib', 'System', 'System.Core', 'System.Data');
        return xHost.lib(options);
    }
    return undefined;
}


Array.prototype.toCsharpList = function(type) {
    var List = xHost.type('System.Collections.Generic.List');
    if (!type) {
        type = "string";
    } else {
        type = type.toLowerCase();
    }

    var objString;
    if (type == "string") {
        objString = xHost.type('System.String');
    } else if (type == "int") {
        objString = xHost.type('System.Int32');
    } else if (type == "byte") {
        objString = xHost.type('System.Byte');
    }else if (type == "double") {
        objString = xHost.type('System.Double');
    } else if (type == "decimal") {
        objString = xHost.type('System.Decimal');
    } else {
        throw new Error('convert to charp list error.');
    }
    
    var total = xHost.newObj(List(objString));
    for (var i = 0; i < this.length; i++) {
        var value;
        if (type == "string") {
            value = this[i].toString();
        } else if (type == "int") {
            value = xHost.toInt32(this[i]);
        } else if (type == "double") {
            value = xHost.toDouble(this[i]);
        } else if (type == "decimal") {
            value = xHost.toDecimal(this[i]);
        } else if (type == "byte") {
            value = xHost.toByte(this[i]);
        }
        total.Add(value);
    }
    return total;
};


this.exports = tabrisFactory;