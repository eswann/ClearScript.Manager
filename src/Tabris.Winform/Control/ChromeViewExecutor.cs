using CefSharp;
using CefSharp.WinForms;
using JavaScript.Manager.Extensions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tabris.Winform.Control
{
    public class CookieVisitor : CefSharp.ICookieVisitor
    {
        public event Action<CefSharp.Cookie> SendCookie;
        public bool Visit(CefSharp.Cookie cookie, int count, int total, ref bool deleteCookie)
        {
            deleteCookie = false;
            if (SendCookie != null)
            {
                SendCookie(cookie);
            }

            return true;
        }

        public void Dispose()
        {
        }
    }

   
    public class ChromeViewExecutor
    {
        private ChromiumWebBrowser browser;
        private CookieContainer initCookieContainer = new CookieContainer();
        private string cookies = string.Empty;

        public Action<ChromiumWebBrowser, Action> AddChrome { get; set; }
        public Action Closeing { get; set; }

        private Dictionary<string, List<dynamic>> _listeners = new Dictionary<string, List<dynamic>>();

        public void show(dynamic options)
        {
            var _options = options as DynamicObject;
            var url = _options.GetMember<string>("url");
            if (string.IsNullOrEmpty(url))
            {
                return;
            }
            var param = _options.GetMember<dynamic>("param");
            if (param != null)
            {
                this.@on("ready", param);
            }
            browser = new ChromiumWebBrowser(url)
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.White,
            };
            browser.FrameLoadEnd += OnFrameLoadEnd;

            AddChrome(browser, close);
        }



        public string getInitCookieString()
        {
            return cookies;
        }

        public async Task<string> execJs(string js)
        {
            try
            {
                if (browser == null) return string.Empty;
                var task = await browser.GetMainFrame().EvaluateScriptAsync(js, null);
                if (task.Success)
                {
                    if (task.Result != null)
                    {
                        return task.Result.ToString();
                    }
                    else
                    {
                        return "null";
                    }
                }

                return task.Message;
            }
            catch (Exception)
            {

                return String.Empty;
            }
        }
        public CookieContainer getInitCookieContainer()
        {
            try
            {
                return CopyContainer(this.initCookieContainer);
            }
            catch (Exception)
            {

                return null;
            }
        }

        public async Task<string> getDomHtml()
        {
            try
            {
                if (browser == null) return string.Empty;
                var result = await browser.GetSourceAsync();
                return result;
            }
            catch (Exception)
            {

                return String.Empty;
            }
        }

        public void close()
        {
            closebrowser();


            OnClose();

            _listeners = new Dictionary<string, List<dynamic>>();
            initCookieContainer = new CookieContainer();
            cookies = string.Empty;

        }


        public void on(string eventName, dynamic callbackFn)
        {

            List<dynamic> events;
            if (!_listeners.TryGetValue(eventName, out events))
            {
                events = _listeners[eventName] = new List<dynamic>();
            }

            events.Add(callbackFn);
        }

        private void OnReady()
        {
            List<dynamic> listeners;
            if (_listeners.TryGetValue("ready", out listeners))
            {
                listeners.ForEach(listener =>
                {
                    Task.Factory.StartNew( () =>
                    {
                         listener.call(null, null);
                    });
                });
            }
        }
        private void OnClose()
        {
            List<dynamic> listeners;
            if (_listeners.TryGetValue("close", out listeners))
            {
                listeners.ForEach(listener => listener.call(null, null));
            }
        }


        public void closebrowser()
        {

            try
            {
                browser?.CloseDevTools();
            }
            catch { }
            try
            {
                browser?.GetBrowser().CloseBrowser(true);
            }
            catch { }

            try
            {
                browser?.Dispose();
            }
            catch { }

        }


        private void OnFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            var cookieManager = CefSharp.Cef.GetGlobalCookieManager();
            CookieVisitor visitor = new CookieVisitor();
            visitor.SendCookie += visitor_SendCookie;
            cookieManager.VisitAllCookies(visitor);
            OnReady();
        }
        private void visitor_SendCookie(CefSharp.Cookie obj)
        {
            try
            {
                initCookieContainer.Add(new System.Net.Cookie(obj.Name, obj.Value) { Domain = obj.Domain });
                cookies += obj.Domain.TrimStart('.') + "^" + obj.Name + "^" + obj.Value + "$";
            }
            catch (Exception)
            {

            }
        }
        private CookieContainer CopyContainer(CookieContainer container)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, container);
                stream.Seek(0, SeekOrigin.Begin);
                return (CookieContainer)formatter.Deserialize(stream);
            }
        }

    }
}
