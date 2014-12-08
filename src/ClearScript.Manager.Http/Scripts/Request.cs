using System.Net.Http;
using System.Threading.Tasks;

namespace ClearScript.Manager.Helpers
{
    public class Http
    {
        public Task get(string uri, dynamic callback)
        {
            var client = new HttpClient();

            var task = client.GetStreamAsync(uri)
                .ContinueWith(res => callback.call(null,res.Result), TaskContinuationOptions.NotOnFaulted)
                .ContinueWith(res => callback.call(null, res.Result), TaskContinuationOptions.OnlyOnFaulted)
                ;

            return task;
        }
    }

    public class HttpAwaiter
    {

    }
}