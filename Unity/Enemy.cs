using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Enemy : SceneObject
{
	public bool bIsAlive = true, bPosInitialized = false;

	public Enemy(float xPos, float yPos) : base(xPos, yPos)
	{
		pos.x = xPos; 
		pos.y = yPos;
	}
};