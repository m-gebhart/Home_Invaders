using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Enemy : SceneObject
{
	public bool bIsAlive = true, bPosInitialized = false;
	public Vector3 lowLimit, highLimit;
	public float speed;

	public Enemy(float xPos, float yPos, float movementRange, float movementSpeed) : base(xPos, yPos)
	{
		InitMovement(xPos, yPos, movementRange, movementSpeed);
	}

	public void InitMovement(float xPos, float yPos, float movementRange, float movementSpeed) 
	{
		speed = movementSpeed;
		lowLimit = new Vector3(xPos - movementRange, yPos, 0);
		highLimit = new Vector3(xPos + movementRange, yPos, 0);

	}
};