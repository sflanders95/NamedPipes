using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Pipes;
using System.Xml.Serialization;
using System.Data;
using System.Threading;

namespace NamedPipes
{
    public class pipedServer : pipedBase
    {
        public new static string DebugMessagePrefix => "SVR";

        public pipedServer() : base("RED") { CreateServerPipe(); }
        public pipedServer(string pipeName):base(pipeName)
        {
            CreateServerPipe();
        }

        public bool CreateServerPipe()
        {
            Debug($"Starting Server {DateTime.Now} Name: {_PipeName}");

            var pipe = new NamedPipeServerStream(_PipeName, PipeDirection.InOut);
            Debug($"Waiting for connection on pipe Named: {_PipeName}");
            pipe.WaitForConnection();

            Debug("Connected");

            XmlSerializer ser = new XmlSerializer(typeof(DataSet));

            DataSet ds;
            using (StreamReader sr = new StreamReader(pipe)) // Note, when using closes it also closes the Pipe
            {
                using (MemoryStream ms = new MemoryStream(ReadToZed(sr).ToArray()))
                {
                    ds = (DataSet)ser.Deserialize(ms);
                    Debug(MemStreamToString(ms));
                }
                Debug("After Reading Info");


                try
                {
                    if (ds.Tables[0].Rows[0][0].Equals("1"))
                        ds.Tables[0].Rows[0][0] = "Success";
                    else
                        ds.Tables[0].Rows[0][0] = "Try Again";
                }
                catch (Exception ex) { Debug(ex); }

                // Send back info
                ser.Serialize(pipe, ds);
                pipe.Write(new byte[] { 0 }, 0, 1); // End current msg
                Debug($"After Sending Info: {DateTime.Now}");
                pipe.WaitForPipeDrain();
                TryWait(sr);
            }

            // Thread.Sleep(1000);
            try { pipe.Disconnect(); } catch { ; }
            return true;
        }

        
    }
}
