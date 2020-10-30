#include <winsock2.h>
#include <ws2tcpip.h>
#include <list>
#include <mutex>

class C_TCP_Recv_Thread;
class C_Session;

class C_TCP_Server
{
	SOCKET							Server;
	SOCKET							Client;
	int								nSize;
	int								nResult;
	struct							addrinfo* result;
	std::thread*					m_pThread;
	std::mutex						m_Mutex;
	bool							m_bRunning;
	std::list<C_TCP_Recv_Thread*>	m_Threads;

	C_Session* m_Session;

public:
	C_TCP_Server();
	~C_TCP_Server();

	bool Init(char* szPort, C_Session* m_data);
	void ThreadProcess();
	bool Login(char* szBuffer);
	bool CheckPlayerID(char* szBuffer);
	bool CheckPlayerLives(char* szBuffer);
	bool Logout(char* szBuffer);
};