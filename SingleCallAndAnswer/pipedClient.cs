using System;
using System.IO;
using System.IO.Pipes;
using System.Xml.Serialization;
using System.Data;

namespace NamedPipes
{
    public class pipedClient : pipedBase
    {
        public new static string DebugMessagePrefix => "CLIENT";

        public pipedClient():base("RED") { ; }
        public pipedClient(string pipeName):base(pipeName)
        {
            StartClientPipe(SinglePipedServer.CreateTestDataSet());
        }

        public void StartClientPipe(DataSet inputDataSet)
        {
            using (var pipe = new NamedPipeClientStream(".", _PipeName, PipeDirection.InOut, PipeOptions.None))
            {
                Debug($"Connecting, PipeName = {_PipeName}\r\n");
                pipe.Connect();

                XmlSerializer ser = new XmlSerializer(typeof(DataSet));
                // DataSet ds = SinglePipedServer.CreateTestDataSet("TestTable");
                
                ser.Serialize(pipe, inputDataSet); // Send DataSet
                pipe.Write(new byte[] { 0 }, 0, 1); // Send End current msg char
                
                Debug("DataSet Sent\r\n");

                // Read Info:
                DataSet respDataSet;
                using (StreamReader sr = new StreamReader(pipe))
                {
                    using (MemoryStream ms = new MemoryStream(ReadToZed(sr).ToArray()))
                    {
                        respDataSet = (DataSet)ser.Deserialize(ms);
                        Debug(MemStreamToString(ms));
                    }
                }

                Debug($"Response was: {respDataSet?.Tables[0]?.Rows[0]?.ItemArray[0]??"error"}");
            }
        }
    }
}
