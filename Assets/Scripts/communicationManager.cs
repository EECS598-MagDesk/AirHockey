using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;



public class communicationManager : MonoBehaviour
{
	private string data = "";
	private int port = 5003;
	private int bufferSize = 1024;
	
	private TcpClient socketConnection;
	private Thread clientReceiveThread;
	private TcpListener socketListener;

	// Use this for initialization 	
	void Start()
	{
		ConnectToTcpServer();
	}

	// Update is called once per frame
	void Update()
	{
	}

	public string Get()
    {
		return data;
    }

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
			Debug.Log("On client connect exception " + e);
		}
	}
	  
	private void ListenForData()
	{
		try
		{
			socketListener = new TcpListener(IPAddress.Parse("0.0.0.0"), port);
			Console.WriteLine("listening ...");
			socketListener.Start();
			socketConnection = socketListener.AcceptTcpClient();
			NetworkStream stream = socketConnection.GetStream();

			byte[] bytes = new byte[bufferSize];

			while (true)
			{
				Console.WriteLine("connected");
				byte[] buffer = new byte[bufferSize];
				int length = stream.Read(buffer, 0, bufferSize);
				data = Encoding.UTF8.GetString(buffer);
			}

		}
		catch (SocketException socketException)
		{
			Debug.Log("Socket exception: " + socketException);
		}
	}

    private void OnApplicationQuit()
    {
		Debug.Log("ending socket");
		socketListener.Stop();
		socketConnection.Close();
	}
}
