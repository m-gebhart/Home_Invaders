using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;
using UnityEngine;


public class UDP_Receiver
{
	public UdpClient udpSocket;
	public Thread rcvThread;
	public Boolean running = true;
	IPEndPoint RemoteIpEndPoint;
	GameObject playerObject;

	public UDP_Receiver(UdpClient udpClient, GameObject player)
	{
		playerObject = player;
		udpSocket = udpClient;
		RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
		rcvThread = new Thread(ThreadProcess);
		rcvThread.Start();
	}

	public void ThreadProcess()
	{
		while (running)
		{
			try
			{
				String[] split;
				Byte[] receiveBytes = udpSocket.Receive(ref RemoteIpEndPoint);
				string returnData = System.Text.Encoding.ASCII.GetString(receiveBytes);
					
				//Debug.Log("UDP >> " + returnData);
				if (returnData.IndexOf("pos:E") != -1) 
				{
					returnData = returnData.Replace("pos:E:", string.Empty);
					split = returnData.Split(':');
					UpdateEnemyPosition(float.Parse(split[0], GameManager.culture), float.Parse(split[1], GameManager.culture));
				}

				else if (returnData.IndexOf("pos") != -1)
				{
					returnData = returnData.Replace("pos:", string.Empty);
					split = returnData.Split(';');
					for (int p = 0; p < split.Length-1; p++)
					{
						//New Player Ghosts
						String[] id_x_y;
						id_x_y = split[p].Split(':');
							UpdatePlayerPosition(int.Parse(id_x_y[0]), 
							float.Parse(id_x_y[1], GameManager.culture), 
							float.Parse(id_x_y[2], GameManager.culture));
					}
				}
				else if (returnData.IndexOf("pro") != -1)
				{
					Debug.Log(returnData);
					returnData = returnData.Replace("pro:", string.Empty);
					split = returnData.Split(';');
					for (int p = 0; p < split.Length - 1; p++)
					{
						//New Projectiles
						String[] id_x_y;
						id_x_y = split[p].Split(':');
						UpdateProjectilePositions(int.Parse(id_x_y[0]),
						float.Parse(id_x_y[1], GameManager.culture),
						float.Parse(id_x_y[2], GameManager.culture));
					}
				}
				else if (returnData.IndexOf("time:") != -1)
                {
					split = returnData.Split(':');
					GameManager.SessionTimeSeconds = float.Parse(split[1], GameManager.culture);
				}
			}
			catch (Exception e)
			{
				Debug.Log("Thread ended:" + e);
				break;
			}
		}
	}

	void UpdateEnemyPosition(float xPos, float yPos)
    {
		GameManager.enemy.pos.x = xPos;
		GameManager.enemy.pos.y = yPos;
	}


	void UpdatePlayerPosition(int id, float xPos, float yPos)
	{
		if (!CheckPlayerExistence(id)) {
			//Create new Ghost Player
			GameManager.AllPlayers.Add(new Player(id, xPos, yPos));
		}

		foreach (Player p in GameManager.AllPlayers)
		{
			if (p.id == id)
			{
				p.pos.x = xPos;
				p.pos.y = yPos;
				p.pos.z = 0;
			}
		}
	}

	bool CheckPlayerExistence(int check_id)
    {
		foreach (Player p in GameManager.AllPlayers)
		{
			if(p.id == check_id)
			return true;
		}
		return false;
    }

	void UpdateProjectilePositions(int id, float xPos, float yPos)
	{
		foreach (Projectile p in GameManager.Projectiles)
		{
			p.pos.x = xPos;
			p.pos.y = yPos;
			p.pos.z = 0;
			if (p.pos.y < -10f)
			{
				GameManager.Projectiles.Remove(p);
				break;
			}
		}
	}
}
