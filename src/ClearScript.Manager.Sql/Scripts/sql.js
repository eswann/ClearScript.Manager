var sqlExecutor = require('sqlExecutor');



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
                console.log(source);
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


DbContext.prototype.Exec = function (sql,options) {
    var sqlType = sql.trim().toLowerCase().slice(0, 6);
    if (sqlType === 'insert') {
        return this.Insert(sql, options);
    } else if (sqlType === 'update') {
        return this.Update(sql,options);
    } else if (sqlType === 'delete') {
        return this.Delete(sql, options);
    } else {
        return this.Query(sql, options);
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
DbContext.prototype.Insert = function (sql, options) {
    return sqlExecutor.DbExecutorNonQuery(this.newOption(sql,options));
}

DbContext.prototype.Update = function (sql, options) {
    return sqlExecutor.DbExecutorNonQuery(this.newOption(sql, options));
}

DbContext.prototype.Delete = function (sql, options) {
    return sqlExecutor.DbExecutorNonQuery(this.newOption(sql,options));
}

DbContext.prototype.InsertWithIdentity = function (sql, options) {
    return sqlExecutor.DbExecutorScalar(this.newOption(sql, options));
}

DbContext.prototype.Query = function (sql, options) {
    var List = xHost.type('System.Collections.Generic.List');
    var objList = xHost.type('System.Collections.Generic.List');
    var obj = xHost.type('System.Object');
    var week = xHost.newObj(List(objList(obj)));
    week = sqlExecutor.DbExecutorQuery(this.newOption(sql, options));
    var first = week[0][0].key;
    //Console.WriteLine(first);
    if (first == 'null_key_') {
        //Console.WriteLine(week[0][0].value);
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

DbContext.prototype.UseTransaction = function (callback,options) {
    if (!callback) return;
    sqlExecutor.UseTransaction(callback, options);
}

this.exports = DbContext;