using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace CashierExternal;

public class ApiWrapper
{
    public static int Workingip = 0;
    private static string _cachedSubnet = null;
    private static readonly HttpClient _sharedClient = new HttpClient 
    { 
       
    };
    
    public static async Task<HttpResponseMessage> Post(string json, string path)
    {
        HttpContent content = json != null 
            ? new StringContent(json, Encoding.UTF8, "application/json") 
            : null;
        
        string subnet = GetLocalSubnet();
        int port = 7084;
        
        Debug.WriteLine($"[DEBUG] Detected subnet: {subnet}");
        
        // Try cached IP first
        if (Workingip != 0)
        {
            Debug.WriteLine($"[DEBUG] Trying cached IP: {subnet}{Workingip}");
            var result = await TryPostToIp(Workingip, subnet, port, path, content);
            if (result != null && result.IsSuccessStatusCode) 
            {
                Debug.WriteLine($"[DEBUG] Cached IP worked!");
                return result;
            }
            
            Debug.WriteLine($"[DEBUG] Cached IP failed");
            Workingip = 0;
        }
        
        // Scan all IPs for open port 7084
        Debug.WriteLine($"[DEBUG] Scanning subnet {subnet}1-254 for port {port}...");
        
        using var cts = new CancellationTokenSource();
        var successResult = new TaskCompletionSource<(int ip, HttpResponseMessage response)>();
        
        var tasks = Enumerable.Range(1, 254)
            .Select(async i => 
            {
                try
                {
                    var result = await ScanAndTryPost(i, subnet, port, path, content, cts.Token);
                    if (result.response != null && result.response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine($"[DEBUG] SUCCESS! Server found at {subnet}{result.ip}");
                        successResult.TrySetResult(result);
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[DEBUG] Scan error on {subnet}{i}: {ex.Message}");
                    return (i, null);
                }
            })
            .ToList();
        
        // Wait for either a success or all tasks to complete
        var allTasksCompleted = Task.WhenAll(tasks);
        var firstSuccess = successResult.Task;
        
        var completed = await Task.WhenAny(firstSuccess, allTasksCompleted);
        
        if (completed == firstSuccess)
        {
            var (ip, response) = await firstSuccess;
            Workingip = ip;
            cts.Cancel(); // Cancel remaining work
            Debug.WriteLine($"[DEBUG] Returning successful response from {subnet}{ip}");
            return response;
        }
        
        Debug.WriteLine($"[DEBUG] All tasks completed, no server found");
        return null;
    }
    
    private static async Task<(int ip, HttpResponseMessage response)> ScanAndTryPost(
        int ip, string subnet, int port, string path, HttpContent content, CancellationToken ct)
    {
        // First check if port is open (fast TCP check)
        string ipAddress = $"{subnet}{ip}";
        
        using var tcpClient = new TcpClient();
        try
        {
            // Quick port check with 100ms timeout
            var connectTask = tcpClient.ConnectAsync(ipAddress, port);
            if (await Task.WhenAny(connectTask, Task.Delay(100, ct)) != connectTask)
            {
                return (ip, null); // Timeout
            }
            
            if (!tcpClient.Connected)
                return (ip, null);
            
            Debug.WriteLine($"[DEBUG] Port {port} open on {ipAddress}, trying HTTP...");
        }
        catch
        {
            return (ip, null); // Port closed or unreachable
        }
        
        // Port is open, try HTTP POST
        try
        {
            string url = $"http://{ipAddress}:{port}/api/{path}/";
            var response = await _sharedClient.PostAsync(url, content, ct);
            Debug.WriteLine($"[DEBUG] {ipAddress} responded: {response.StatusCode}");
            return (ip, response);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[DEBUG] {ipAddress} HTTP failed: {ex.Message}");
            return (ip, null);
        }
    }
    
    /// <summary>
    /// Gets the local subnet (e.g., "192.168.8." from IP "192.168.8.187")
    /// </summary>
    private static string GetLocalSubnet()
    {
        if (_cachedSubnet != null)
            return _cachedSubnet;
        
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var localIP = host.AddressList.FirstOrDefault(ip => 
                ip.AddressFamily == AddressFamily.InterNetwork && 
                !IPAddress.IsLoopback(ip) &&
                (ip.ToString().StartsWith("192.168.") || 
                 ip.ToString().StartsWith("10.") ||
                 ip.ToString().StartsWith("172.")));
            
            if (localIP != null)
            {
                var parts = localIP.ToString().Split('.');
                _cachedSubnet = $"{parts[0]}.{parts[1]}.{parts[2]}.";
                Debug.WriteLine($"[DEBUG] Local IP detected: {localIP}, subnet: {_cachedSubnet}");
                return _cachedSubnet;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[DEBUG] Error detecting subnet: {ex.Message}");
        }
        
        _cachedSubnet = "192.168.1.";
        Debug.WriteLine($"[DEBUG] Using fallback subnet: {_cachedSubnet}");
        return _cachedSubnet;
    }
    
    private static async Task<HttpResponseMessage> TryPostToIp(
        int ip, string subnet, int port, string path, HttpContent content)
    {
        string url = $"http://{subnet}{ip}:{port}/api/{path}/";
        try
        {
            return await _sharedClient.PostAsync(url, content);
        }
        catch
        {
            return null;
        }
    }
}