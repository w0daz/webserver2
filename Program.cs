using System;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpListener server = new HttpListener();
            server.Prefixes.Add("http://127.0.0.1:8888/");
            server.Prefixes.Add("http://localhost:8888/");

            server.Start();

            Console.WriteLine("Listening...");

            while (true)
            {
                HttpListenerContext context = server.GetContext();

                // Handle each request in a separate thread
                ThreadPool.QueueUserWorkItem((state) =>
                {
                    HttpListenerResponse response = context.Response;

                    // in this example there is a index.html file in the (root) project folder. 
                    string page = @"../../index.html";

                    TextReader tr = new StreamReader(page);
                    string msg = tr.ReadToEnd();

                    byte[] buffer = Encoding.UTF8.GetBytes(msg);

                    response.ContentLength64 = buffer.Length;
                    Stream st = response.OutputStream;
                    st.Write(buffer, 0, buffer.Length);

                    context.Response.Close();
                    Console.WriteLine("Thread complete, sleeping.");
                    Thread.Sleep(5000);
                });
            }
        }
    }

}