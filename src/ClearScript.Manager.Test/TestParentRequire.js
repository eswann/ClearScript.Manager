
var testObject = require('testRequire');

function TestParentType() {
    
}

TestParentType.prototype.getText = testObject.getText;
TestParentType.prototype.getNumber = function () { return 500; }

testParentRequire.exports = new TestParentType();