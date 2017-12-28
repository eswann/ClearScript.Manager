var http = require('http');

function requestFactory(options, callback) {

    var cfg = options;
    if (typeof options === 'string') {
        cfg = { uri: options };
    }

    cfg.callback = callback || options.callback;

    return new requestFactory.Request(cfg);
}


function Request(options) {
    this.options = options;
    this.go();

}

Request.prototype.go = function () {
    var me = this;

    me.req = http.request(me.options, function (res) {
        if (me.options.callback) {
            res.on('end', function (body) {
                me.onEnd(body, me.options.callback);
            });
            res.on('err', function (errmsg) {
                me.options.callback(errmsg);
            });
        }
    });

    me.req.end();
    //todo sedn data on req.

};


Request.prototype.onEnd = function (body, callback) {
    var self = this;
   
   // Console.WriteLine(body);
    if (self.options.json) {
        try {
            body = JSON.parse(body);
        } catch (e) { }
    }
    //Console.WriteLine(body[0].cargoTypeName);
    callback.call(null, null, body);
};

if (typeof module != 'undefined' && module != null) {
    module.exports = request;
}

requestFactory.Request = Request;

this.exports = requestFactory;
