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
	public GameObject LocalPlayer, OtherPlayer, ProjectilePrefab;
	static Player local = new Player(playerID, 0, 0);
	public static List<Player> AllPlayers = new List<Player>();
	public static List<Projectile> Projectiles = new List<Projectile>();
	public static Enemy enemy;
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

		GameManager.enemy = new Enemy(0, -4);
		enemy.sceneObject = GameObject.FindGameObjectWithTag("Enemy");

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

		CheckInput(v);
	}

    void FixedUpdate()
    {
		//moving objects according to server data
		MoveLocalPlayer();
		MoveOtherPlayers();
		MoveProjectiles();
		MoveEnemy();
	}

	private void CheckInput(Vector3 v)
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
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (local.bCanShoot)
			{
				StartCoroutine(SetCooldown());
				local.bCanShoot = false;
				SendProjectile(v.x, v.y);
			}
		}
	}

	private void SendPosition(float xPos, float yPos) 
	{
		if (playerID > -1)
			udpConn.Send("pos:" + playerID + ":" + xPos + ":" + yPos + "\r\n");
	}

	private void SendProjectile(float xPos, float yPos)
	{
		if (playerID > -1) 
		{
			udpConn.Send("pro:" + playerID + ":" + xPos + ":" + yPos + "\r\n");
			Projectile newProjectile = new Projectile(playerID, xPos, yPos);
			Projectiles.Add(newProjectile);
			GameObject go = Instantiate(ProjectilePrefab, new Vector3(xPos, yPos, 0), Quaternion.identity);
			newProjectile.sceneObject = go;
		}
	}

	IEnumerator SetCooldown()
	{
		yield return new WaitForSeconds(Player.attackCooldown);
		local.bCanShoot = true;
	}

	private void SetEnemy() 
	{
		GameManager.enemy = new Enemy(0, 0);
		enemy.sceneObject = GameObject.FindGameObjectWithTag("Enemy");
	}

	private void MoveEnemy()
	{
		Vector3 newPos = new Vector3(GameManager.enemy.pos.x, GameManager.enemy.pos.y, 0);
		if (!GameManager.enemy.bPosInitialized)
		{
			enemy.sceneObject.transform.position = newPos;
			GameManager.enemy.bPosInitialized = true;
		}
		else
			enemy.sceneObject.transform.position = Vector3.Lerp(enemy.sceneObject.transform.position, newPos, 0.05f);
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
					CreateGhost(p, OtherPlayer);
				}
				p.sceneObject.transform.position = p.pos;
			}
		}
	}

	void MoveProjectiles()
	{
		foreach (Projectile p in Projectiles)
		{
			if (p.sceneObject == null)
				CreateGhost(p, ProjectilePrefab);
			p.sceneObject.transform.position = p.pos;
		}
	}

	void CreateGhost(SceneObject s, GameObject prefab) 
	{
		s.sceneObject = Instantiate(prefab, s.pos, Quaternion.identity);
		(s.sceneObject.GetComponent<SpriteRenderer>()).color = new Color(1, 1, 1, 0.75f);
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

