using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class LANDiscovery : MonoBehaviour
{
    public int port = 56566;
    public float broadcastInterval = 1.0f;
    public string serverName = "test";

    public enum Side {
        Server,
        Client
    };

    public Side side;

    private UdpClient client;
    private Thread listener;

    private bool listening;
    private bool broadcasting;

    private volatile bool discovered = false;
    public string discoveredIP = "";
    public string discoveredName = "";

    public void ResetState() 
    {
        discovered = false;
        if (side == Side.Client)
            StartListening();
        else
            StartBroadcast();
    }

    public bool DiscoveredServer() 
    {
        if (side == Side.Server)
            return false;

        return discovered;
    }

    private void Start()
    {
        ResetState();
    }

    private void OnApplicationQuit()
    {
        if (side == Side.Client)
            StopListening();
        else
            StopBroadcast();
    }

    public void StartBroadcast() 
    {
        if (broadcasting)
            return;

        client = new();
        client.EnableBroadcast = true;

        broadcasting = true;

        InvokeRepeating(nameof(Broadcast), 0.0f, broadcastInterval);
        Debug.Log("Started LAN broadcast");
    }

    public void StopBroadcast() 
    {
        if (!broadcasting)
            return;

        broadcasting = false;
        CancelInvoke(nameof(Broadcast));
        client?.Close();
        client = null;
        Debug.Log("Stopped LAN broadcast");
    }

    private void Broadcast() 
    {
        // Send server details, for now IP and a "name". This name should be unique to tell multiple VR users apart
        try 
        {
            string localIP = GetLocalIPAddress();
            string msg = $"{serverName}|{localIP}";
            byte[] data = Encoding.UTF8.GetBytes(msg);

            IPEndPoint endPoint = new(IPAddress.Broadcast, port);
            client.Send(data, data.Length, endPoint);
        }
        catch (Exception e) 
        {
            Debug.LogError("Failed LAN broadcast: " + e.Message);
        }

    }

    public void StartListening() 
    {
        if (listening)
            return;

        listening = true;
        listener = new(Listen);
        listener.IsBackground = true;
        listener.Start();
        Debug.Log("Listening...");
    }

    public void StopListening() 
    {
        if (!listening)
            return;

        listening = false;
        listener?.Abort();
        listener = null;

        Debug.Log("Stopped listening");
    }

    private void Listen() 
    {
        using UdpClient listener = new(port);
        IPEndPoint endPoint = new(IPAddress.Any, port);

        try
        {
            while (listening)
            {
                byte[] bytes = listener.Receive(ref endPoint);
                string msg = Encoding.UTF8.GetString(bytes);

                string[] parts = msg.Split("|");
                if (parts.Length == 2)
                {
                    string name = parts[0];
                    string address = parts[1];

                    Debug.Log("Discovered " + name + " as " + address);
                    discovered = true;
                    discoveredName = name;
                    discoveredIP = address;

                    StopListening();
                    return;
                }
            }
        }
        catch (System.Threading.ThreadAbortException e) 
        {
            
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to listen to LAN broadcast: " + e.Message + " (" + e.GetType() + ")");
        }
    }

    private string GetLocalIPAddress() 
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var address in host.AddressList) 
        {
            if (address.AddressFamily == AddressFamily.InterNetwork)
                return address.ToString();
        }
        return "0.0.0.0";
    }
}
