using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TYPSA.SharedLib.Autocad.Metrics
{
    public class cls_00_MetricsSender
    {
        public static async Task SendMetricsAsync(string user, string accion, object[] executed_process, Object additionalData)
        {
            var url = "https://dlab.typsa.net/register-record-v2/api/v2/send/metrics";

            var data = new
            {
                user = user,
                accion = accion,
                executed_process = executed_process,
                data = additionalData
            };

            string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
            var httpClient = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseBody);
                    // Aquí puedes deserializar y usar la respuesta si es necesario
                }
                else
                {
                    Console.WriteLine($"Error al enviar métricas. Estado: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar métricas: {ex.Message}");
            }
        }
    }
}
