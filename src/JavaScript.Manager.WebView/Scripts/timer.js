var javascript_timer_factory_timerExecutor = javascript_timer_factory_timerExecutor || require('javascript_timer_factory_timerExecutor');

function timerFactory() {


}

timerFactory.create = function (options) {
    if (!options) {
        options = 1000;
    }
    return javascript_timer_factory_timerExecutor.create(options);
}


this.exports = timerFactory;