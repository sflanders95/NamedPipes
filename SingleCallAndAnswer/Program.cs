using System;
using System.IO;
using System.Data;
using System.Threading.Tasks;

namespace NamedPipes
{
    class SinglePipedServer
    {
        public static string DebugFile = "debug.txt";

        static void Main(string[] argv)
        {
            if (argv.Length != 0)
            {
                StartServer(argv[0]);
            }
            else
                DoTest();
        }

        public static void StartServer(string pipeID = "")
        {
            Debug($"Program Start {pipeID} - {DateTime.Now}", "PROG");
            // Start Server
            //Task.Run(async () => {
                try
                {
                    pipedServer svr = new pipedServer(pipeID);
                }
                catch (Exception e) { Debug(e, pipedServer.DebugMessagePrefix); }
            //    await Task.Delay(10); // Task.run needs an await.
            //});
        }

        public static void DoTest()
        {
            StartServer("RED");

            // Client
            pipedClient pc = new pipedClient("RED");
        }





        private static object __oLock = new object();
        public static string Debug(object text, string prefix = "")
        {
            lock (__oLock)
            {
                bool notWritten = true;
                while (notWritten)
                {
                    try
                    {
                        prefix = prefix.Length > 0 ? $"{prefix} " : prefix ?? "";
                        File.AppendAllText(DebugFile, $"{prefix}{text}\r\n");
                        notWritten = false;
                    }
                    catch { ; }
                }
            }
            Console.WriteLine($"{prefix}{text}");

            return $"{prefix}{text}";
        }

        public static DataSet CreateTestDataSet(string TableName = "Table1")
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable(TableName);
            dt.Columns.Add(new DataColumn("Col1"));
            dt.Columns.Add(new DataColumn("Col2"));
            DataRow dr = dt.NewRow();
            dr[0] = "1";
            dr[1] = DateTime.Now.ToString();
            dt.Rows.Add(dr);
            ds.Tables.Add(dt);
            return ds;
        }

    }
}
