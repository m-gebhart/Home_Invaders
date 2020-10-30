using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System.Diagnostics;

public class GameManager : MonoBehaviour
{
	public static int playerID = -1;
	public static float SessionTimeSeconds = 0;
	public GameObject LocalPlayer, OtherPlayer;
	static Player local = new Player(playerID, 0, 0);
	public static List<Player> AllPlayers = new List<Player>();
	public static List<SceneObject> SceneObjects = new List<SceneObject>();
	static Enemy enemy;
	public float enemyRange = 6f, enemySpeed = 3.7f;
	public static CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
	UDP_Connection udpConn;
	TCP_Connection tcpConn;

	void Start()
	{
		culture.NumberFormat.NumberDecimalSeparator = ",";
		udpConn = new UDP_Connection("127.0.0.1", 20000, LocalPlayer);
		tcpConn = new TCP_Connection("127.0.0.1", 20001, udpConn, LocalPlayer);

		if (LocalPlayer == null)
			LocalPlayer = GameObject.FindWithTag("PlayerLocal");

		SetEnemy();

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
		Vector3 newPos = Vector3.Lerp(enemy.lowLimit, enemy.highLimit, (Mathf.Sin(GameManager.SessionTimeSeconds)) + 1 * 0.5f);
		if (enemy.bPosInitialized)
			enemy.pos = Vector3.MoveTowards(enemy.pos, newPos, Time.fixedDeltaTime * enemy.speed);
		else
		{
			enemy.pos = newPos;
			enemy.bPosInitialized = true;
		}
		enemy.sceneObject.transform.position = enemy.pos;
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

	void SetEnemy() 
	{
		UnityEngine.Debug.Log(Time.fixedDeltaTime);
		//Enemy should already be located in the level
		GameObject levelEnemy = GameObject.FindWithTag("Enemy");
		if (levelEnemy != null)
		{
			enemy = new Enemy(levelEnemy.transform.position.x, levelEnemy.transform.position.y, enemyRange, enemySpeed);
			if (GameObject.FindGameObjectsWithTag("Enemy").Length != 0)
				enemy.sceneObject = levelEnemy;
			SceneObjects.Add(enemy);
		}
	}

	void OnApplicationQuit()
	{
		if (udpConn != null)
			udpConn.Quit();

		if (tcpConn != null)
			tcpConn.Quit();
		UnityEngine.Debug.Log("Quit...");
	}
}

