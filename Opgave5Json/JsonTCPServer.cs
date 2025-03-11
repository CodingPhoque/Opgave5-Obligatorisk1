using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace Opgave5Json
{
    class JsonTCPServer
    {
        public void HandleClient(TcpClient client)
        {
            using (client)
            {
                NetworkStream ns = client.GetStream();
                StreamReader reader = new StreamReader(ns, Encoding.UTF8);
                StreamWriter writer = new StreamWriter(ns, new UTF8Encoding(false)) { AutoFlush = true };

                try
                {
                    // Læs én linje JSON fra klienten
                    string requestJson = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(requestJson))
                    {
                        SendError(writer, "Empty or invalid JSON request.");
                        return;
                    }

                    // Parse JSON til vores RequestData
                    RequestData requestData;
                    try
                    {
                        requestData = JsonSerializer.Deserialize<RequestData>(requestJson);

                        // Tjek at vigtige felter er udfyldt
                        if (requestData == null ||
                            string.IsNullOrWhiteSpace(requestData.Method) ||
                            requestData.Tal1 == null ||
                            requestData.Tal2 == null)
                        {
                            SendError(writer, "JSON missing required fields: Method, Tal1, Tal2");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        SendError(writer, $"JSON parse error: {ex.Message}");
                        return;
                    }

                    // Udfør handling
                    int result;
                    switch (requestData.Method)
                    {
                        case "Random":
                            result = new Random().Next(requestData.Tal1.Value, requestData.Tal2.Value + 1);
                            break;

                        case "Add":
                            result = requestData.Tal1.Value + requestData.Tal2.Value;
                            break;

                        case "Subtract":
                            result = requestData.Tal1.Value - requestData.Tal2.Value;
                            break;

                        default:
                            SendError(writer, $"Unknown method: {requestData.Method}");
                            return;
                    }

                    // Returnér svar som JSON
                    var responseObj = new
                    {
                        Result = result,
                        Status = "OK"
                    };
                    string responseJson = JsonSerializer.Serialize(responseObj);
                    writer.WriteLine(responseJson);
                }
                finally
                {
                    Console.WriteLine("Forbindelse til klient lukket.");
                }
            }
        }

        private void SendError(StreamWriter writer, string errorMessage)
        {
            var errorObj = new
            {
                Error = errorMessage,
                Status = "Error"
            };
            string errorJson = JsonSerializer.Serialize(errorObj);
            writer.WriteLine(errorJson);
        }

        // Hjælpeklasse til at matche JSON-request
        private class RequestData
        {
            public string Method { get; set; }
            public int? Tal1 { get; set; }
            public int? Tal2 { get; set; }
        }
    }
}
