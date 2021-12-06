using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
{
    class MainClass
    {
        static readonly HttpClient client = new HttpClient();
        static async Task Main()
        {
            try
            {
                DataSerializer ds = new DataSerializer("json");
                HttpResponseMessage response = await client.GetAsync("http://127.0.0.1:4000/Ping");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Ответ сервера на /Ping: {responseBody}\n");

                response = await client.GetAsync("http://127.0.0.1:4000/GetInputData");
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Ответ сервера на /GetInputData: {responseBody}\n");
                string input_serialized = responseBody;
                string output_serialized = ds.GetAnswer(responseBody);

                response = await client.GetAsync($"http://127.0.0.1:4000/WriteAnswer?answer=\"{output_serialized}\"");
                response.EnsureSuccessStatusCode();
                Console.WriteLine($"Cерверу отправлен output: {output_serialized}\n");

                var byteContent = new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(input_serialized));
                response = await client.PostAsync("http://127.0.0.1:4000/PostInputData", byteContent);
                response.EnsureSuccessStatusCode();
                Console.WriteLine($"Серверу отправлен /PostInputData c {input_serialized}\n");

                response = await client.GetAsync("http://127.0.0.1:4000/GetAnswer");
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync();
                output_serialized = responseBody;
                Console.WriteLine($"Ответ серера на /GetAnswer: {output_serialized}\n");

                Console.Read();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Error ", e.Message);
            }
        }
    }
}
