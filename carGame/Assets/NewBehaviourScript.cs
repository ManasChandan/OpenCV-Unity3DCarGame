using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    #region private members 	
    private TcpClient socketConnection;
    private Thread clientReceiveThread;
    #endregion
    // Use this for initialization 	
    public GameObject player,cars;
    float x = 0.0f, z = 0.0f; // 0 , 10 , 19.5 , -10 , -19.5
    float speed = 4f; 
    int control; 
    string controller;
    public Material style,pink;
    MeshRenderer car; 
    void Start()
    {
        car = cars.GetComponent<MeshRenderer>();    
    }
    // Update is called once per frame
    void Update()
    {
        ConnectToTcpServer();
        get_Movement(); 
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("obstacle"))
        {
            x = 0;
            player.transform.position = new Vector3(4, 0, 0);
        }
        else if (other.gameObject.CompareTag("Goal"))
        {
            Application.Quit(); 
        }
        else if (other.gameObject.CompareTag("colorStyle"))
        {
            car.material = style; 
        }
        else if (other.gameObject.CompareTag("pink"))
        {
            car.material = pink; 
        }
    }
    /// <summary> 	
    /// Setup socket connection. 	
    /// </summary> 	
    private void ConnectToTcpServer()
    {
        try
        {
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
           
        }
    }
    /// <summary> 	
    /// Runs in background clientReceiveThread; Listens for incomming data. 	
    /// </summary>     
    private void ListenForData()
    {
        try
        {
            socketConnection = new TcpClient("localhost", 8888);
            Byte[] bytes = new Byte[1024];
            while (true)
            {
                // Get a stream object for reading 				
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    // Read incomming stream into byte arrary. 					
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        // Convert byte array to string message. 						
                        string serverMessage = Encoding.UTF8.GetString(incommingData);
                        Debug.Log("server message received as: " + serverMessage);
                        controller = serverMessage;
                        
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            
        }
    }
    /// <summary> 	
    /// Send message to server using socket connection. 	
    /// </summary> 	
    private void SendMessage()
    {
        if (socketConnection == null)
        {
            return;
        }
        try
        {
            // Get a stream object for writing. 			
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                string clientMessage = "This is a message from one of your clients.";
                // Convert string message to byte array.                 
                byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
                // Write byte array to socketConnection stream.                 
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                Debug.Log("Client sent his message - should be received by server");
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
    void get_Movement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            player.transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
            x = x + -speed * Time.deltaTime;
        }
        control = int.Parse(controller);
        control = control / 100;
        if (control < 1.44f)
        {
            player.transform.position = new Vector3(x, 0, -19.5f);
        }
        else if(control < 2.88f)
        {
            player.transform.position = new Vector3(x, 0, -10f);
        }
        else if(control  < 4.32f)
        {
            player.transform.position = new Vector3(x, 0, 0);
        }
        else if(control < 5.76f)
        {
            player.transform.position = new Vector3(x, 0, 10f);
        }
        else if(control < 7.00f)
        {
            player.transform.position = new Vector3(x, 0, 19.5f);
        }
        
    }
}