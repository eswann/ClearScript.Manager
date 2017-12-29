var sql = require('sql');
var request = require('request');

function tabrisFactory() {

}


tabrisFactory.create = function (type, options) {
    if (!type) return undefined;
    type = type.trim().toLowerCase();
    if (type === 'sql') {
        if (!options) return undefined;
        if (!options.name || !options.type) return undefined;
        return sql.create(options.name, options.type);
    }else if (type === 'http') {
        return request.create();
    }

    return undefined;
}


this.exports = tabrisFactory;