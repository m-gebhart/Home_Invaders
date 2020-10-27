#include <stdio.h>
#include <winsock2.h>
#include <ws2tcpip.h>

class C_GameObject{

public:
	int		m_nID;
	char	m_szName[256];
	float	x, y;
	struct sockaddr_in sender;

public:
	C_GameObject();
	C_GameObject(int nID, char* szName, const struct sockaddr_in pSender);
	C_GameObject(int nID, char* szName);
};