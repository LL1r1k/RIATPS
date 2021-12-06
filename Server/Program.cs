using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Server
{
    class MainClass
    {
        public static string res { get; set; }

        private static DataSerializer ds = new DataSerializer("json");
        private const string serializedInput = @"{""K"":10,""Sums"":[1.01,2.02],""Muls"":[1,4]}";
        private const string serializedOutput = @"{""SumResult"":30.30,""MulResult"":4,""SortedInputs"":[1.0,1.01,2.02,4.0]}";
        public static void Main(string[] args)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://127.0.0.1:4000/");
            listener.Start();
            Console.WriteLine("Listening...");
            while (true)
            {
                if (!listener.IsListening) { break; }
                HttpListenerContext context = listener.GetContext();

                HandleContext(context, listener);
            }
        }

        private static void HandleContext(HttpListenerContext context, HttpListener listener)
        {     
            string responseString = "error";
            switch (context.Request.Url.AbsolutePath)
            {
                case "/Ping":
                    Console.WriteLine($"Ping\n");
                    responseString = $"{HttpStatusCode.OK}";
                    break;

                case "/GetInputData":
                    Console.WriteLine($"Отправлен клиенту input : {serializedInput}\n");
                    responseString = serializedInput;
                    break;

                case "/WriteAnswer":
                    string body = context.Request.QueryString["answer"].Trim('"');
                    Console.WriteLine($"Ответ с клиента: {body}");
                    Console.WriteLine($"Правильный ответ: {serializedOutput}\n");

                    if (body == serializedOutput)
                        responseString = @"{""answer"": true}";
                    else
                        responseString = @"{""answer"": false}";
                    break;

                case "/PostInputData":
                    using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                    {
                        res = reader.ReadToEnd();
                    }
                    Console.WriteLine($"Пришёл с клиента input: {res}  \n");
                    break;

                case "/GetAnswer":                    
                    if (res == null)
                        break;

                    responseString = ds.GetAnswer(res);
                    Console.WriteLine($"Отправлен на клиентoutput : {responseString} \n");
                    break;

                case "/Stop":
                    Console.WriteLine($"Stop\n");
                    listener.Stop();
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
            }

            byte[] responseBody = System.Text.Encoding.UTF8.GetBytes(responseString);
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentLength64 = responseBody.Length;
            context.Response.OutputStream.Write(responseBody, 0, responseBody.Length);

            context.Response.Close();
        }
    }
}
