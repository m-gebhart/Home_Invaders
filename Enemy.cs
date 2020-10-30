using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Enemy : SceneObject
{
	public bool bIsAlive = true;
	public float movementRange = 3.2f;
	[HideInInspector]
	public Vector3 lowLimit, highLimit;
	public static float speed = 2;

	public Enemy(float xPos, float yPos) : base(xPos, yPos)
	{
		lowLimit = new Vector3(xPos - movementRange, yPos, 0);
		highLimit = new Vector3(xPos + movementRange, yPos, 0);
	}
};