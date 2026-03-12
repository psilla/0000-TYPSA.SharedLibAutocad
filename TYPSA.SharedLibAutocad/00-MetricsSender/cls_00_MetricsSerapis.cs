using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public static class SerapisMetrics
{
    private static readonly HttpClient client = new HttpClient();

    private static string BackendUrl =>
        Environment.GetEnvironmentVariable("SERAPIS_BACKEND_URL")
        ?? "https://serapis.api.typsa.com:3000";

    private static string UrlRegisterHash => $"{BackendUrl}/api/metrics/register-user-hash";
    private static string UrlMetrics => $"{BackendUrl}/api/metrics/register";
    private static string ErrorUrl => $"{BackendUrl}/api/metrics/client-error";

    static SerapisMetrics()
    {
        client.DefaultRequestHeaders.Add("Origin", "http://localhost:4200");
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public static async Task ReportErrorAsync(Exception ex, string context = "unknown")
    {
        var payload = new
        {
            function = context,
            error_message = ex.Message
        };

        try
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await client.PostAsync(ErrorUrl, content);
        }
        catch
        {
            // No romper ejecución si falla el logging
        }
    }

    public static string GetUserHash(string versionId)
    {
        string filePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".serapis_user_id"
        );

        try
        {
            if (File.Exists(filePath))
                return File.ReadAllText(filePath).Trim();

            return CreateNewUserHash(filePath, versionId).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            ReportErrorAsync(ex, "GetUserHash").Wait();
            throw;
        }
    }

    private static async Task<string> CreateNewUserHash(string filePath, string versionId)
    {
        string userHash = Guid.NewGuid().ToString();
        File.WriteAllText(filePath, userHash);

        string systemUser = Environment.UserName;

        var payload = new
        {
            user_hash = userHash,
            systemUser = systemUser,
            versionId = versionId
        };

        try
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(UrlRegisterHash, content);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error registering user_hash: {response.StatusCode}");

            return userHash;
        }
        catch (Exception ex)
        {
            await ReportErrorAsync(ex, "CreateNewUserHash");
            throw;
        }
    }

    public static async Task TrackUsageAsync(string userHash, string versionId)
    {
        string systemUser = Environment.UserName;

        var payload = new
        {
            user_hash = userHash,
            versionId = versionId,
            systemUser = systemUser,
            status = "success"
        };

        try
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(UrlMetrics, content);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Backend error: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            await ReportErrorAsync(ex, "TrackUsageAsync");
            throw;
        }
    }

    public static async Task<string> InitializeMetricsAsync(string versionId)
    {
        try
        {
            string userHash = GetUserHash(versionId);
            await TrackUsageAsync(userHash, versionId);
            return "Metrics sent successfully.";
        }
        catch (Exception ex)
        {
            await ReportErrorAsync(ex, "InitializeMetricsAsync");
            return $"[ERROR] Could not initialize metrics: {ex.Message}";
        }
    }
}
