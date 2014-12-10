using System;
using System.IO;

namespace ClearScript.Manager.Http.Helpers.Node
{
    public class NodeBuffer
    {

        public NodeBuffer(int size)
            : this(new MemoryStream(size))
        {

        }
        public NodeBuffer()
            : this(new MemoryStream())
        {

        }
        public NodeBuffer(Stream innerStream)
        {
            this.InnerStream = innerStream;
        }

        public static bool isBuffer(object obj)
        {
            return obj is NodeBuffer;
        }


        public NodeBuffer slice(int? start = null, int? end = null)
        {
            if (start == null && end == null)
            {
                return this;
            }
            throw new NotImplementedException();

        }

        protected Stream InnerStream { get; set; }

        public string toString(object enc = null, int? start = null, int? end = null)
        {
            string encoding = null;
            long pos = InnerStream.Position;
            InnerStream.Position = 0;
            string ret;

            if (string.IsNullOrWhiteSpace(encoding))
            {
                ret = new StreamReader(this.InnerStream, true).ReadToEnd();
            }
            else
            {
                ret = new StreamReader(this.InnerStream, System.Text.Encoding.GetEncoding(encoding)).ReadToEnd();
            }

            InnerStream.Position = pos;
            return ret;

        }

        public void copy(NodeBuffer target, int? targetStart = null, int? sourceStart = null, int? sourceEnd = null)
        {
            InnerStream.CopyTo(target.InnerStream);

        }

        public long length
        {
            get { return InnerStream.Length; }
        }


    }
}