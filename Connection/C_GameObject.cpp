#include "C_GameObject.h"
#include <iostream>

C_GameObject::C_GameObject(){}

//Players
C_GameObject::C_GameObject(int nID, ObjectType m_type, const struct sockaddr_in pSender)
{
	m_id = nID;
	xPos = yPos = 0.0f;
	udp_sender = pSender;
}

//Other Objects
C_GameObject::C_GameObject(int nID, ObjectType m_type)
{
	m_id = nID;
	xPos = yPos = 0.0f;
	if (m_type == ObjectType::Enemy)
	{
		movementRange = 5.f;
		yPos = -4;
		speed = 1;
	}
	else if (m_type == ObjectType::Projectile)
		speed = -2;
}