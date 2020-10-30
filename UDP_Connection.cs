using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;
using UnityEngine;

public class UDP_Connection
{
	public UdpClient Server = new UdpClient();
	UDP_Receiver recv;

    public UDP_Connection(String serverIP, Int32 port, GameObject player)
	{
		try
		{
			Server.Connect(serverIP, port);
			recv = new UDP_Receiver(Server, player);
		}
		catch (Exception e)
        {
			Debug.Log(e.ToString());
        }
	}

	public void Send(String message)
	{
		Byte[] sendBytes = System.Text.Encoding.ASCII.GetBytes(message);
		Server.Send(sendBytes, sendBytes.Length);
	}

	public void Quit()
	{
		recv.running = false;
		Server.Close();
		recv.rcvThread.Abort();
	}
}
