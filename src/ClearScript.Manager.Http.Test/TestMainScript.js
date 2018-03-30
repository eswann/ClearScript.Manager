var test_aaa = require('./Config/TestIncludeScript.js');
var test_aaa2 = require('./Config/TestIncludeScript2.js');
var test_aaa3 = require('./TestIncludeScript2.js');

function bbbbbbbbbbbbbbbbbb() {

}


bbbbbbbbbbbbbbbbbb.hello = function () {
    return 'bbbbbbbbbbbbbbbbbb' + test_aaa.hello() + test_aaa2.hello();
}


this.exports = bbbbbbbbbbbbbbbbbb;