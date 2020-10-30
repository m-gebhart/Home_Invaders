using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;
using UnityEngine;


public class TCP_Receiver
{
	public TcpClient tcpSocket;
	public Thread rcvThread;
	public Boolean running = true;
	GameObject PlayerObject;
	IPEndPoint RemoteIpEndPoint;
	UDP_Connection udpConn;

	public TCP_Receiver(TcpClient tcpClient, UDP_Connection udpCon, GameObject player)
	{
		PlayerObject = player;
		udpConn = udpCon;
		tcpSocket = tcpClient;
		RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
		rcvThread = new Thread(ThreadProc);
		rcvThread.Start();
	}

	public void ThreadProc()
	{
		while (running)
		{
			try
			{
				NetworkStream stream = tcpSocket.GetStream();

				String[] split;
				Byte[] receiveBytes = new Byte[1024];
				stream.Read(receiveBytes, 0, receiveBytes.Length);
				string returnData = System.Text.Encoding.ASCII.GetString(receiveBytes);

				//Debug.Log("TCP >>" + returnData);

				if (returnData.IndexOf("id") != -1)
				{
					split = returnData.Split(':');
					SetPlayerID(Int32.Parse(split[1]));
					udpConn.Send("id:" + GameManager.playerID);
				}
			}
			catch (Exception e)
			{
				Debug.Log("Thread ended:" + e);
				break;
			}
		}
	}

	void SetPlayerID(int init_id) 
	{
		GameManager.playerID = init_id;
	}

	void SetPlayerLives(int init_id){}
};
