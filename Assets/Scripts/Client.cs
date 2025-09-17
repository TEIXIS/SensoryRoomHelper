using UnityEngine;

using System;
using System.Net.Sockets;
using System.Text;

public class Client : MonoBehaviour
{
    public string serverIP = "127.0.0.1";
    public int serverPort = 5000;

    private TcpClient tcpClient;
    private NetworkStream stream;
    private byte[] buffer = new byte[1024];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ConnectToServer();    
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
            Debug.Log("Connection closed");
        }
        catch (Exception e) 
        {
            Debug.LogError("Error closing connection: " + e.Message);
        }
    }
}
