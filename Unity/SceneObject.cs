using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SceneObject
{
	public int id;
	public Vector3 pos;
    public GameObject sceneObject;

	public SceneObject(float xPos, float yPos)
	{
		pos = new Vector3(xPos, yPos, 0);
	}
}