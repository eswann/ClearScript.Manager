namespace JavaScript.Manager.Debugger
{
    using System;
    using System.IO;

    public interface INetworkClient : IDisposable
    {

        /// <summary>
        /// Gets a value indicating whether client is connected to a remote host.
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// Returns the <see cref="T:System.IO.Stream" /> used to send and receive data.
        /// </summary>
        /// <returns>The underlying <see cref="T:System.IO.Stream" /></returns>
        Stream GetStream();

        string TargetId { get; set; }
    }
}