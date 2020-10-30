using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class GameManager : MonoBehaviour
{
	public static int playerID = -1;
	public static float SessionTimeSeconds = 0;
	public GameObject LocalPlayer, OtherPlayer;
	static Player local = new Player(playerID, 0, 0);
	public static List<Player> AllPlayers = new List<Player>();
	public static List<GameObject> LevelObjects = new List<GameObject>();
	public static CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
	UDP_Connection udpConn;
	TCP_Connection tcpConn;

	void Start()
	{
		culture.NumberFormat.NumberDecimalSeparator = ",";
		udpConn = new UDP_Connection("127.0.0.1", 20000, LocalPlayer);
		tcpConn = new TCP_Connection("127.0.0.1", 20001, udpConn, LocalPlayer);

		if (LocalPlayer == null)
			LocalPlayer = GameObject.Find("/Player");

		/*ENEMY POS HERE*/
		LevelObjects.Add(GameObject.Find("Enemy"));

		tcpConn.Send("login:"+ playerID);
		local.pos = Vector3.zero;
		AllPlayers.Add(local);
		local.sceneObject = LocalPlayer;

		Application.runInBackground = true;

	}

	void Update()
	{
		local.id = GameManager.playerID;
		Vector3 v = local.pos;

		CheckMovementInput(v);
	}

    void FixedUpdate()
    {
		MoveLocalPlayer();
		MoveOtherPlayers();
		MoveEnemy();
	}

	private void CheckMovementInput(Vector3 v)
    {
		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
		{
			v.x -= 0.1f * Player.speed;
			SendPosition(v.x, v.y);
		}
		if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
		{
			v.x += 0.1f * Player.speed;
			SendPosition(v.x, v.y);
		}
		if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
		{
			v.y += 0.1f * Player.speed;
			SendPosition(v.x, v.y);
		}
		if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
		{
			v.y -= 0.1f * Player.speed;
			SendPosition(v.x, v.y);
		}
	}

	private void SendPosition(float xPos, float yPos) 
	{
		if (playerID > -1)
			udpConn.Send("pos:" + playerID + ":" + xPos + ":" + yPos + "\r\n");
	}

	private void MoveEnemy()
	{
		foreach (GameObject go in LevelObjects)
		{
			//go.transform.position = Mathf.Lerp(go.transform)
		}
	}

	void MoveLocalPlayer()
	{
		foreach (Player p in AllPlayers)
		{
			if (p.id == GameManager.playerID && p.sceneObject != null)
				p.sceneObject.transform.position = p.pos;
		}
	}


	void MoveOtherPlayers() 
	{
		foreach (Player p in AllPlayers)
		{
			if (p.id != GameManager.playerID)
			{
				if (p.sceneObject == null)
				{
					CreateGhostPlayerSprite(p);
				}
				p.sceneObject.transform.position = p.pos;
			}
		}
	}

	void CreateGhostPlayerSprite(Player p) 
	{
		p.sceneObject = Instantiate(OtherPlayer, p.pos, Quaternion.identity);
		(p.sceneObject.GetComponent<SpriteRenderer>()).color = new Color(1, 1, 1, 0.75f);
	}

	void OnApplicationQuit()
	{
		if (udpConn != null)
			udpConn.Quit();

		if (tcpConn != null)
			tcpConn.Quit();
		Debug.Log("Quit...");
	}
}

