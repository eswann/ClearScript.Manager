namespace JavaScript.Manager.Debugger.Messages
{
    using Newtonsoft.Json;

    public class Response
    {
        /// <summary>
        /// Sequence number of the response.
        /// </summary>
        [JsonProperty("id")]
        public int Sequence
        {
            get;
            set;
        }


        /// <summary>
        /// If the request was not successful, returns a message.
        /// </summary>
        [JsonProperty("result")]
        public dynamic Result
        {
            get;
            set;
        }
    }
}
