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
	public GameObject LocalPlayer, OtherPlayer, ProjectilePrefab, EnemyProjectilePrefab;
	public static Player local = new Player(playerID, 0, 0);
	public static List<Player> AllPlayers = new List<Player>();
	public static List<Projectile> Projectiles = new List<Projectile>();
	public static bool inEdit = false;
	public static Enemy enemy;
	public static CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
	public UDP_Connection udpConn;
	public TCP_Connection tcpConn;

	void Start()
	{
		culture.NumberFormat.NumberDecimalSeparator = ",";
		udpConn = new UDP_Connection("127.0.0.1", 20000, LocalPlayer);
		tcpConn = new TCP_Connection("127.0.0.1", 20001, udpConn, LocalPlayer);


		//setting up enemy, player ID and player gameobject
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
				local.bCanShoot = false;
				StartCoroutine(SetCooldown());
				SendProjectile(v.x, v.y);
			}
		}
	}

	//On Movemenent Input
	private void SendPosition(float xPos, float yPos) 
	{
		if (playerID > -1)
			udpConn.Send("pos:" + playerID + ":" + xPos + ":" + yPos + "\r\n");
	}

	//On Attack / Shoot Input
	private void SendProjectile(float xPos, float yPos)
	{
		if (playerID > -1) 
		{
			udpConn.Send("pro:" + playerID + ":" + xPos + ":" + yPos + "\r\n");
			Projectile newProjectile = new Projectile(playerID, xPos, yPos);
			while (GameManager.inEdit) { }
			Projectiles.Add(newProjectile);
			GameObject go = Instantiate(ProjectilePrefab, new Vector3(xPos, yPos, 0), Quaternion.identity);
			newProjectile.sceneObject = go;
			newProjectile.sceneObject.name = (playerID.ToString()+"_PlayerProjectile");
		}
	}

	IEnumerator SetCooldown()
	{
		yield return new WaitForSeconds(Player.attackCooldown);
		local.bCanShoot = true;
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

	//move player of respective user
	void MoveLocalPlayer()
	{
		foreach (Player p in AllPlayers)
		{
			if (p.id == GameManager.playerID && p.sceneObject != null)
				p.sceneObject.transform.position = p.pos;
		}
	}

	//setting positions all the other players' replications in the scene
	void MoveOtherPlayers() 
	{
		foreach (Player p in AllPlayers)
		{
			if (p.id != GameManager.playerID)
			{
				if (p.sceneObject == null)
				{
					CreateOtherPrefab(p, OtherPlayer, new Color(1, 1, 1, 0.75f));
				}
				p.sceneObject.transform.position = p.pos;
			}
		}
	}

	//setting positions of all projectiles in the scene (by players and the enemy)
	void MoveProjectiles()
	{
		foreach (Projectile p in Projectiles)
		{
			if (p.sceneObject == null)
			{
				if (p.id > 10)
					CreateOtherPrefab(p, EnemyProjectilePrefab, new Color(1, 0, 0, 0.5f));
				else
					CreateOtherPrefab(p, ProjectilePrefab, new Color(1, 1, 1, 0.75f));
			}
			p.sceneObject.transform.position = p.pos;
		}
	}

	void CreateOtherPrefab(SceneObject s, GameObject prefab, Color color) 
	{
		s.sceneObject = Instantiate(prefab, s.pos, Quaternion.identity);
		(s.sceneObject.GetComponent<SpriteRenderer>()).color = color;
	}

	public void Win() 
	{
		local.bHasWon = true;
		UnityEngine.Debug.Log("You stroke the enemy!");
	}

	public void LocalPlayerDie() 
	{
		local.bIsAlive = false;
		tcpConn.Send("dead:"+playerID + "\r\n");
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

