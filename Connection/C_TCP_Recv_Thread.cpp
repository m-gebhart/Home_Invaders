#include "C_TCP_Recv_Thread.h"
#include "C_TCP_Server.h"
#include <iostream>

C_TCP_Recv_Thread::C_TCP_Recv_Thread(C_TCP_Server* pServer, SOCKET Client)
{
	m_pServer = pServer;
	m_Client = Client;

	m_pThread = new std::thread(&C_TCP_Recv_Thread::ThreadProcess, this);
}

void C_TCP_Recv_Thread::ThreadProcess()
{
	char szBuffer[1024];
	ZeroMemory(&szBuffer, sizeof(szBuffer));

	do {
		int nResult = recv(m_Client, szBuffer, 1023, 0);

		if (nResult > 0) {
			if (strstr(szBuffer, "login:") != NULL)
				m_pServer->Login(szBuffer);
			else if (strstr(szBuffer, "dead:") != NULL)
				m_pServer->SetDead(szBuffer);
		}
	} while (true);
}