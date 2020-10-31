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
	GameManager m_manager;

	public UDP_Receiver(UdpClient udpClient, GameObject player)
	{
		playerObject = player;
		udpSocket = udpClient;
		RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
		m_manager = GameObject.Find("/GameManager").GetComponent<GameManager>();
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
					GameManager.enemy.pos.x = float.Parse(split[0], GameManager.culture);
					GameManager.enemy.pos.y = float.Parse(split[1], GameManager.culture);
				}

				//Receiving player position updates
				else if (returnData.IndexOf("pos") != -1)
				{
					returnData = returnData.Replace("pos:", string.Empty);
					split = returnData.Split(';');
					for (int p = 0; p < split.Length - 1; p++)
					{
						String[] id_x_y;
						id_x_y = split[p].Split(':');
						UpdatePlayerPosition(int.Parse(id_x_y[0]),
						float.Parse(id_x_y[1], GameManager.culture),
						float.Parse(id_x_y[2], GameManager.culture));
					}
				}

				//Receiving projectile position updates
				else if (returnData.IndexOf("pro") != -1)
				{
					returnData = returnData.Replace("pro:", string.Empty);
					split = returnData.Split(';');
					List<Projectile> tempList = GameManager.Projectiles;
					GameManager.inEdit = true;
					for (int p = 0; p < split.Length - 1; p++)
					{
						String[] id_x_y;
						id_x_y = split[p].Split(':');
						UpdateProjectileData(int.Parse(id_x_y[0]),
						float.Parse(id_x_y[1], GameManager.culture),
						float.Parse(id_x_y[2], GameManager.culture),
						tempList
						);
					}
					GameManager.Projectiles = tempList;
					GameManager.inEdit = false;
				}

				//receiving server time
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


	void UpdatePlayerPosition(int id, float xPos, float yPos)
	{
		//Create new Player in 'ghost-like' shapes
		if (!CheckPlayerExistence(id)) {
			GameManager.AllPlayers.Add(new Player(id, xPos, yPos));
		}

		foreach (Player p in GameManager.AllPlayers)
		{
			if (p.id == id || p.id == GameManager.playerID)
			{
				p.pos.x = xPos;
				p.pos.y = yPos;
				p.pos.z = 0;
			}
		}
	}


	void UpdateProjectileData(int id, float xPos, float yPos, List<Projectile> m_list)
	{
		if (!CheckProjectileExistence(id) && id > 10)
		{
			m_list.Add(new Projectile(id, xPos, yPos));
		}

		foreach (Projectile p in m_list)
		{
			if (p.pos.y < -10f || p.pos.y > 10f)
			{
				m_list.Remove(p);
				break;
			}
			else if (p.id == id)
			{
				p.pos.x = xPos;
				p.pos.y = yPos;
				p.pos.z = 0;
			}
		}
	}

	//Whether Player is already registered
	bool CheckPlayerExistence(int check_id)
    {
		foreach (Player p in GameManager.AllPlayers)
			if (p.id == check_id)
				return true;
		return false;
    }

	//Whether projectile is already registered
	bool CheckProjectileExistence(int check_id)
	{
		foreach (Projectile p in GameManager.Projectiles)
			if (p.id == check_id)
				return true;
		return false;
	}
}