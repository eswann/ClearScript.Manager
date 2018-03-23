using JavaScript.Manager.Loaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace JavaScript.Manager.WebView
{
    public class TimerPackageExecutor : RequiredPackage
    {
        public TimerPackageExecutor()
        {
            PackageId = "javascript_timer_factory_timerExecutor";
            HostObjects.Add(new HostObject
            {
                Name = "javascript_timer_factory_timerExecutor",
                Target = new TimerHelper()
            });
        }
    }

    public class TimerPackage : RequiredPackage
    {
        public TimerPackage()
        {
            PackageId = "javascript_timer_factory";
            ScriptUri = "JavaScript.Manager.WebView.Scripts.timer.js";
            RequiredPackageType = RequiredPackageType.EmbeddedFile;
        }
    }

    public class TimerHelper
    {
        public MyTimer create(int interval)
        {
            if (interval == 0) interval = 1000;
            return new MyTimer(interval);
        }
    }

    public class JsArg
    {
        public int Result { get; set; }
    }
    public class MyTimer:IDisposable
    {
        System.Timers.Timer systemTimer;
        private Dictionary<string, List<dynamic>> _listeners = new Dictionary<string, List<dynamic>>();
        public MyTimer(int interval)
        {
            systemTimer = new System.Timers.Timer(interval);
            systemTimer.AutoReset = true;
            systemTimer.Elapsed += SystemTimerOnElapsed;
        }



        private void SystemTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (systemTimer == null)
            {
                return;
            }

            systemTimer.Stop();

            List<dynamic> listeners;
            if (_listeners.TryGetValue("timer", out listeners))
            {

                Task.Factory.StartNew(() =>
                {
                    if (listeners.Count > 0)
                    {
                        var result = new JsArg { Result = 0 };
                        listeners[0].call(null, result);

                        if (result.Result == 2)
                        {
                            //去掉
                            _listeners["timer"] = listeners.Skip(1).ToList();
                            systemTimer.Start();
                        }
                        else
                        {
                            systemTimer.Start();
                        }
                    }

                });
            }
            else
            {
                systemTimer.Start();
            }
        }

        public int RESTART { get; set; } = 1;
        public int DONEXT { get; set; } = 2;
        public MyTimer pushCallback(dynamic callbackFn)
        {

            this.@on("timer", callbackFn);
            return this;
        }
        private void on(string eventName, dynamic callbackFn)
        {

            List<dynamic> events;
            if (!_listeners.TryGetValue(eventName, out events))
            {
                events = _listeners[eventName] = new List<dynamic>();
            }

            events.Add(callbackFn);
        }


        public void start()
        {
            systemTimer.Start();
        }

        public void stop()
        {
            systemTimer.Stop();
        }

        public void destroy()
        {
           this.Dispose();
        }
        public void Dispose()
        {
            systemTimer?.Stop();
            systemTimer = null;
        }

        ~MyTimer()
        {
            try
            {
                systemTimer?.Stop();
                systemTimer = null;
            }
            catch (Exception)
            {
                //ignore
            }
        }
    }
}
