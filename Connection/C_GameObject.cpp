#include "C_GameObject.h"

C_GameObject::C_GameObject()
{
}

C_GameObject::C_GameObject(int nID, char* szName, const struct sockaddr_in pSender)
{
	m_nID = nID;
	strcpy_s(m_szName, 256, szName);
	x = y = 0.0f;
	sender = pSender;
}

C_GameObject::C_GameObject(int nID, char* szName)
{
	m_nID = nID;
	strcpy_s(m_szName, 256, szName);
	x = y = 0.0f;
}