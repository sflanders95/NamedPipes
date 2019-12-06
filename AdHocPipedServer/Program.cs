using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace NamedPipes
{
    class SinglePipedServer_Stress
    {
        public static string DebugFile = "debugc.txt";

        static void Main(string[] args)
        {
            DoTest(4);
        }

        public static void DoTest(int num)
        {
            // Debug($"Program Start {DateTime.Now}");
            List<Process> procs = new List<Process>();

            for (int i = 0; i < num; i++)
            {
                string pid = Guid.NewGuid().ToString().Substring(24);
                ProcessStartInfo startInfo = new ProcessStartInfo("SingleCallAndAnswer.exe")
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    Arguments = pid
                };
                procs.Add(Process.Start(startInfo));
                try
                {
                    pipedClient pc = new pipedClient(pid);
                }
                catch (Exception e) { Debug(e, pipedClient.DebugMessagePrefix); }
            }
            
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
            // Console.WriteLine($"{prefix}{text}");

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
