using UnityEngine;

using TMPro;
using UnityEngine.UI;

public class ChangeCube : MonoBehaviour
{
    public Button button;
    public Client client;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button.onClick.AddListener(SendMessage);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SendMessage() 
    {
        if (client.Connected()) 
        {
            client.SendMessageToServer("changecolor");
        }
        else 
        {
            Debug.LogWarning("Cannot send message: not connected to the server.");
        }
    }
}
