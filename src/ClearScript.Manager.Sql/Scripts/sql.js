var javascript_sql_factory_sqlExecutor = javascript_sql_factory_sqlExecutor || require('javascript_sql_factory_sqlExecutor');

function dbFactory() {

   
}

dbFactory.create = function (option) {
    return new dbFactory.DbContext(option);
}

function DbContext(option) {
    this.options = option;
}


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


DbContext.prototype.extend = (function () {
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


DbContext.prototype.insert = function (sql, option) {
    return javascript_sql_factory_sqlExecutor.DbExecutorNonQuery(sql, this.extend({ param: option}, this.options));
}

DbContext.prototype.update = function (sql, option) {
    return javascript_sql_factory_sqlExecutor.DbExecutorNonQuery(sql, this.extend({ param: option }, this.options));
}

DbContext.prototype.delete = function (sql, option) {
    return javascript_sql_factory_sqlExecutor.DbExecutorNonQuery(sql, this.extend({ param: option }, this.options));
}

DbContext.prototype.insertWithIdentity = function (sql, option) {
    return javascript_sql_factory_sqlExecutor.DbExecutorScalar(sql, this.extend({ param: option }, this.options));
}

DbContext.prototype.query = function (sql, option) {
    var List = xHost.type('System.Collections.Generic.List');
    var objList = xHost.type('System.Collections.Generic.List');
    var obj = xHost.type('System.Object');
    var week = xHost.newObj(List(objList(obj)));
    week = javascript_sql_factory_sqlExecutor.DbExecutorQuery(sql, this.extend({ param: option }, this.options));
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

DbContext.prototype.useTransaction = function (callback,option) {
    if (!callback) return;
    javascript_sql_factory_sqlExecutor.UseTransaction(callback, option);
}
dbFactory.DbContext = DbContext;
this.exports = dbFactory;