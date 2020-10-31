using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Player : SceneObject
{
	public int  lives = 3;
	public static float speed = 1, attackCooldown = 3.3f;
	public bool bIsAlive = true, bCanShoot = true, bHasWon = false;

	public Player(int init_id, float xPos, float yPos) : base(xPos, yPos)
	{
		id = init_id;
	}
};