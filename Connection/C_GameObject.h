#include <stdio.h>
#include <winsock2.h>
#include <ws2tcpip.h>

enum ObjectType {Enemy, Player, Obstacle, Projectile, Other};

class C_GameObject{

public:
	int		m_id;
	float	xPos, yPos, movementRange = 0.f, speed = 0.f;
	ObjectType m_type;
	struct sockaddr_in udp_sender;

public:
	C_GameObject();
	C_GameObject(int nID, ObjectType m_type, const struct sockaddr_in pSender);
	C_GameObject(int nID, ObjectType m_type);
};