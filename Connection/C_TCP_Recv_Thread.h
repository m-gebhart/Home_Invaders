#include <winsock2.h>
#include <ws2tcpip.h>
#include <thread>

class C_TCP_Server;

class C_TCP_Recv_Thread
{
	C_TCP_Server*	m_pServer;
	SOCKET			m_Client;
	std::thread*	m_pThread;

public:
	C_TCP_Recv_Thread(C_TCP_Server* pServer, SOCKET Client);
	void ThreadProc();
};