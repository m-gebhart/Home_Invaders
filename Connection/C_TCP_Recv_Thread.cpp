#include "C_TCP_Recv_Thread.h"
#include "C_TCP_Server.h"


C_TCP_Recv_Thread::C_TCP_Recv_Thread(C_TCP_Server* pServer, SOCKET Client)
{
	m_pServer = pServer;
	m_Client = Client;

	m_pThread = new std::thread(&C_TCP_Recv_Thread::ThreadProc, this);
}

void C_TCP_Recv_Thread::ThreadProc()
{
	// Reading

	char szBuffer[1024];
	ZeroMemory(&szBuffer, sizeof(szBuffer));

	do {
		int nResult = recv(m_Client, szBuffer, 1023, 0);

		if (nResult > 0) {
			//cout << szBuffer << "\n";

			if (strstr(szBuffer, "login:") != NULL) {
				if (m_pServer->CheckLogin(szBuffer) == true) {
					//SendPosition();	
				}
			}
		}
	} while (true);
}