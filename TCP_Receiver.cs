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

				//receiving ip requests
				if (returnData.IndexOf("id") != -1)
				{
					split = returnData.Split(':');
					SetPlayerID(Int32.Parse(split[1]));
					udpConn.Send("id:" + GameManager.playerID);
				}

				//receiving 'enemy or player is dead' requests
				else if (returnData.IndexOf("dead:") != -1)
				{
					SetDead(returnData);
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

	//Set either enemy E or Player with id to isAlive = false
	void SetDead(string dyingObject)
	{
		if (dyingObject.EndsWith("E"))
			GameManager.enemy.bIsAlive = false;

		else if (dyingObject.IndexOf(":P:") != -1)
        {
			int id = dyingObject[dyingObject.Length - 1];
			foreach (Player p in GameManager.AllPlayers) 
			{
				if (p.id == id)
					p.bIsAlive = false;
			}
        }
	}
};
