#include <stdio.h>
#include <winsock2.h>
#include <ws2tcpip.h>

enum ObjectType {Enemy, Player, Projectile};

class C_GameObject{

	/*
	* m_id: 
		0-9: Players 1-10;
		10 / E: Enemy
		ObjectType::Projectiles are possessive (e.g. projectiles with id:10 belong to Enemy)
	*/

public:
	int		m_id;
	float	xPos, yPos, movementRange = 0.f, speed = 0.f;
	ObjectType m_type;
	struct sockaddr_in udp_sender;
	bool isAlive = true;

public:
	C_GameObject();
	C_GameObject(int nID, ObjectType m_type, const struct sockaddr_in pSender);
	C_GameObject(int nID, ObjectType m_type);
};