using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;
using UnityEngine;

public class TCP_Connection
{
	public TcpClient Server = new TcpClient();
	TCP_Receiver recv;

	public TCP_Connection(String serverIP, Int32 port, UDP_Connection udpCon, GameObject player)
	{
		try
		{
			Server.Connect(serverIP, port);
			recv = new TCP_Receiver(Server, udpCon, player);
		}
		catch (Exception e)
		{
			Debug.Log(e.ToString());
		}
	}

	public void Send(String message)
	{
		Byte[] sendBytes = System.Text.Encoding.ASCII.GetBytes(message);
		NetworkStream stream = Server.GetStream();
		stream.Write(sendBytes, 0, sendBytes.Length);
	}

	public void Quit()
	{
		recv.running = false;
		Server.Close();
		recv.rcvThread.Abort();
	}
};