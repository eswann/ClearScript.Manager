var javascript_log_factory_logExecutor = require('javascript_log_factory_logExecutor');

function logFactory() {

   
}

logFactory.create = function (options) {
    this.options = options || {};
    return new logFactory.LogContext();
}

function LogContext() {
  
}

LogContext.prototype.extractLocation = function (urlLike) {
    // Fail-fast but return locations like "(native)"
    if (urlLike.indexOf(':') === -1) {
        return [urlLike];
    }

    var regExp = /(.+?)(?:\:(\d+))?(?:\:(\d+))?$/;
    var parts = regExp.exec(urlLike.replace(/[\(\)]/g, ''));
    return [parts[1], parts[2] || undefined, parts[3] || undefined];
}

LogContext.prototype.parseV8OrIE = function (error) {
    var CHROME_IE_STACK_REGEXP = /^\s*at .*(\S+\:\d+|\(native\))/m;
    var filtered = error.stack.split('\n').filter(function (line) {
        return !!line.match(CHROME_IE_STACK_REGEXP);
    }, this);

    return filtered.map(function (line) {
        if (line.indexOf('(eval ') > -1) {
            // Throw away eval information until we implement stacktrace.js/stackframe#8
            line = line.replace(/eval code/g, 'eval').replace(/(\(eval at [^\()]*)|(\)\,.*$)/g, '');
        }
        var tokens = line.replace(/^\s+/, '').replace(/\(eval code/g, '(').split(/\s+/).slice(1);
        var locationParts = this.extractLocation(tokens.pop());
        var functionName = tokens.join(' ') || undefined;
        //var fileName = ['eval', '<anonymous>'].indexOf(locationParts[0]) > -1 ? undefined : locationParts[0];

        return {
            functionName: functionName,
            lineNumber: locationParts[1],
            columnNumber: locationParts[2],
            source: line
        };
    }, this);
}

LogContext.prototype.info = function (err) {
    if (typeof err === 'string') {
        javascript_log_factory_logExecutor.Info(err, '');
    } else {
        javascript_log_factory_logExecutor.Info(err.message, this.getTrace(err));
    }
}

LogContext.prototype.warn = function (err) {
    if (typeof err === 'string') {
        javascript_log_factory_logExecutor.Warn(err, '');
    } else {
        javascript_log_factory_logExecutor.Warn(err.message, this.getTrace(err));
    }
}

LogContext.prototype.error = function (err) {
    if (typeof err === 'string') {
        javascript_log_factory_logExecutor.Error(err, '');
    } else {
        javascript_log_factory_logExecutor.Error(err.message, this.getTrace(err));
    }
}

LogContext.prototype.debug = function (err) {
    
    if (typeof err === 'string') {
        javascript_log_factory_logExecutor.Debug(err, '');
    } else {
        javascript_log_factory_logExecutor.Debug(err.message, this.getTrace(err));
    }
}
LogContext.prototype.getTrace= function (err)
{
    if (this.trace) {
        try {
            var tracew = JSON.stringify(this.parseV8OrIE(err));
            return tracew;
        } catch (e) {
            return '';
        }
    } else {
        return '';
    }
    
}

logFactory.LogContext = LogContext;
this.exports = logFactory;