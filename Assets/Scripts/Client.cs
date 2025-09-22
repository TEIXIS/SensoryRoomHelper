using UnityEngine;

using System;
using System.Net.Sockets;
using System.Text;

public class Client : MonoBehaviour
{
    public int serverPort = 5000;

    public LANDiscovery lanDiscovery;

    private TcpClient tcpClient;
    private NetworkStream stream;
    private byte[] buffer = new byte[1024];
    private string serverIP;

    private bool connected = false;

    private void Update()
    {
        if (!connected && lanDiscovery.DiscoveredServer()) 
        {
            serverIP = lanDiscovery.discoveredIP;
            ConnectToServer();
        }
    }

    private void OnApplicationQuit()
    {
        CloseConnection();
    }

    void ConnectToServer()
    {
        try 
        {
            tcpClient = new(serverIP, serverPort);
            stream = tcpClient.GetStream();

            Debug.Log("Connected to server");

            SendMessageToServer("Hello, server!");

            ReceiveMessageFromServer();

            connected = true;
        }
        catch (Exception e)
        {
            Debug.LogError("Error connecting to server: " + e.Message);
        }
    }

    void SendMessageToServer(string msg) 
    {
        try 
        {
            byte[] msgBytes = Encoding.ASCII.GetBytes(msg);
            stream.Write(msgBytes, 0, msgBytes.Length);
            Debug.Log("Sent message to server: " + msg);
        }
        catch (Exception e)
        {
            Debug.LogError("Error sending message to server: " + e.Message);
        }
    }

    void ReceiveMessageFromServer() 
    {
        try 
        {
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            if (bytesRead > 0)
            {
                string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Debug.Log("Received message from server: " + response);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error receiving message from server: " + e.Message);
        }
    }

    void CloseConnection()
    {
        try 
        {
            stream.Close();
            tcpClient.Close();
            connected = false;
            Debug.Log("Connection closed");
        }
        catch (Exception e) 
        {
            Debug.LogError("Error closing connection: " + e.Message);
        }
    }
}
