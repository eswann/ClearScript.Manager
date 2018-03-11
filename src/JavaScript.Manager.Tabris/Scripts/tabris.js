var javascript_sql_factory = require('javascript_sql_factory');
var javascript_request_factory = require('javascript_request_factory');
var javascript_log_factory = require('javascript_log_factory');
var javascript_webview_factory = require('javascript_webview_factory');

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
    }
    return undefined;
}


this.exports = tabrisFactory;