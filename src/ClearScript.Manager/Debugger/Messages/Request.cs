namespace JavaScript.Manager.Debugger.Messages
{
    using Newtonsoft.Json;

    public class Request
    {
        public Request(string command)
        {
            this.Type = command;
        }


        /// <summary>
        /// Sequence number of the request.
        /// </summary>
        [JsonProperty("id")]
        public int Sequence
        {
            get;
            set;
        }

        /// <summary>
        /// The type of request (will always be "request");
        /// </summary>
        [JsonProperty("method")]
        public string Type
        {
            get;
            private set;
        }


        /// <summary>
        /// Gets the arguments associated with the request.
        /// </summary>
        [JsonProperty("params")]
        public object Arguments
        {
            get;
            set;
        }
    }
}