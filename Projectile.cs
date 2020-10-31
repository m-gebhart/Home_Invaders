using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Projectile : SceneObject
{
	public static float speed = 1;
	public bool onHit = false;

	public Projectile(int init_id, float xPos, float yPos) : base(xPos, yPos)
	{
		id = init_id;
	}
};