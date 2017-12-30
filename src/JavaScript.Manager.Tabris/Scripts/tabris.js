var sql = require('javascript_sql');
var request = require('javascript_request');

function tabrisFactory() {

}


tabrisFactory.create = function (type, options) {
    if (!type) return undefined;
    if (!options) return undefined;
    type = type.trim().toLowerCase();
    if (type === 'sql') {
        if (!options.name || !options.type) return undefined;
        return sql.create(options.name, options.type);
    }else if (type === 'http') {
        return request.create(options);
    }
    return undefined;
}


this.exports = tabrisFactory;