var sqlExecutor = require('sqlExecutor');



function DbContext(mapping,type) {
    this.options = {
        "connectionString": mapping,
        "DbType": type || "sqlserver"
    };
}

DbContext.prototype.Exec = function (sql, timeout) {
    var sqlType = sql.trim().toLowerCase().slice(0, 6);
    if (sqlType === 'select') {
        return this.Insert(sql, timeout);
    } else if (sqlType === 'update') {
        return this.Update(sql, timeout);
    } else if (sqlType === 'delete') {
        return this.Delete(sql, timeout);
    } else {
        return this.Query(sql, timeout);
    }
}

DbContext.prototype.newOption = function (sql, timeout) {
    var me = this;
    return {
        "connectionString": me.options.connectionString,
        "DbType": me.options.DbType,
        "sql": sql,
        "timeout": timeout || 0
    };
}
DbContext.prototype.Insert = function (sql,timeout) {
    return sqlExecutor.DbExecutorNonQuery(this.newOption(sql,timeout));
}

DbContext.prototype.Update = function (sql, timeout) {
    return sqlExecutor.DbExecutorNonQuery(this.newOption(sql, timeout));
}

DbContext.prototype.Delete = function (sql, timeout) {
    return sqlExecutor.DbExecutorNonQuery(this.newOption(sql, timeout));
}

DbContext.prototype.InsertWithIdentity = function (sql, timeout) {
    return sqlExecutor.DbExecutorScalar(this.newOption(sql, timeout));
}

DbContext.prototype.Query = function (sql, timeout) {
    var List = xHost.type('System.Collections.Generic.List');
    var objList = xHost.type('System.Collections.Generic.List');
    var obj = xHost.type('System.Object');
    var week = xHost.newObj(List(objList(obj)));
    week = sqlExecutor.DbExecutorQuery(this.newOption(sql, timeout));
    //var first = week[0][0].key;
    Console.WriteLine(first);
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

this.exports = DbContext;