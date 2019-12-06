using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace NamedPipes
{
    public class pipedBase
    {
        protected string _PipeName = "RED"; // Guid.NewGuid().ToString();

        public pipedBase() { ; }
        public pipedBase(string pipeName)
        {
            _PipeName = pipeName;
        }

        protected string DebugMessagePrefix = "";

        public virtual string Debug(object text)
        {
            return Debug(text, DebugMessagePrefix);
        }

        public static string Debug(object text, string prefix)
        {
            return SinglePipedServer.Debug(text, prefix);
        }

        /// <summary>
        /// Reads the stream until '\0' is found, then returns.
        /// </summary>
        /// <param name="rdr"></param>
        /// <param name="zed">optional way to change the byte that signifies the end.</param>
        /// <returns></returns>
        public static List<byte> ReadToZed(StreamReader rdr, int zed = 0)
        {
            List<byte> bytes = new List<byte>();
            int b;
            try
            {
                while ((b = rdr.Read()) != zed || b == -1) // Read can return -1 (~EOF)
                {
                    bytes.Add((byte)b);
                }
            } catch { ; }
            return bytes;
        }

        /// <summary>
        /// A Way to let the client close the connection when it is done.
        /// </summary>
        /// <remarks>Interesting that on many calls, this never gets called, but it 
        /// does ever once in a while. Maybe 10 can be increased to cover huge latencies? 10 = wait max 1 second</remarks>
        /// <param name="rdr"></param>
        /// <param name="NumReadTimes"></param>
        public static void TryWait(StreamReader rdr, int NumReadTimes = 10)
        {
            for (int i = 0; i < NumReadTimes; i++)
            {
                Thread.Sleep(100);
                try { i = rdr.Read() == 0 ? 10 : i; } catch { i = NumReadTimes; }
                Debug($"TryWait {i}", "");
            }
        }

        /// <summary>
        /// Convert Stream to String
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static string MemStreamToString(MemoryStream ms)
        {
            long pos = ms.Position;
            ms.Position = 0;
            string s = new StreamReader(ms).ReadToEnd();
            ms.Position = pos;
            return s;
        }
    }
}
