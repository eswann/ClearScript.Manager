var javascript_sqlExecutor = require('javascript_sqlExecutor');

function dbFactory() {

   
}

dbFactory.create = function (mapping, type) {
    return new dbFactory.DbContext(mapping, type);
}

function DbContext(mapping,type) {
    this.options = {
        "connectionString": mapping,
        "DbType": type || "sqlserver"
    };
}

DbContext.prototype.extend = (function () {
    var isObjFunc = function(name) {
        var toString = Object.prototype.toString;
        return function() {
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


DbContext.prototype.exec = function (sql,options) {
    var sqlType = sql.trim().toLowerCase().slice(0, 6);
    if (sqlType === 'insert') {
        return this.insert(sql, options);
    } else if (sqlType === 'update') {
        return this.update(sql,options);
    } else if (sqlType === 'delete') {
        return this.delete(sql, options);
    } else {
        return this.query(sql, options);
    }
}

DbContext.prototype.newOption = function (sql, options) {
    var me = this;
    var pp = {
        "connectionString": me.options.connectionString,
        "DbType": me.options.DbType,
        "sql": sql
    };
    if (!options) {
        return pp;
    }
    return this.extend(pp, options);
}
DbContext.prototype.insert = function (sql, options) {
    return javascript_sqlExecutor.DbExecutorNonQuery(this.newOption(sql, options));
}

DbContext.prototype.update = function (sql, options) {
    return javascript_sqlExecutor.DbExecutorNonQuery(this.newOption(sql, options));
}

DbContext.prototype.delete = function (sql, options) {
    return javascript_sqlExecutor.DbExecutorNonQuery(this.newOption(sql, options));
}

DbContext.prototype.insertWithIdentity = function (sql, options) {
    return javascript_sqlExecutor.DbExecutorScalar(this.newOption(sql, options));
}

DbContext.prototype.query = function (sql, options) {
    var List = xHost.type('System.Collections.Generic.List');
    var objList = xHost.type('System.Collections.Generic.List');
    var obj = xHost.type('System.Object');
    var week = xHost.newObj(List(objList(obj)));
    week = javascript_sqlExecutor.DbExecutorQuery(this.newOption(sql, options));
    var first = week[0][0].key;
    //Console.WriteLine(first);
    if (first == 'null_key_') {
        return week[0][0].value;
    }
    var ary = [];
    for (var i = 0; i < week.Count; i++) {
        var obj2 = {};
        for (var j = 0; j < week[i].Count; j++) {
            var obj = week[i][j];
            //Console.WriteLine(obj);
            obj2[obj.key] = obj.value;
        }
        ary.push(obj2);
    }
    return ary;
}

DbContext.prototype.useTransaction = function (callback,options) {
    if (!callback) return;
    javascript_sqlExecutor.UseTransaction(callback, options);
}
dbFactory.DbContext = DbContext;
this.exports = dbFactory;