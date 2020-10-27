#include <winsock2.h>
#include <ws2tcpip.h>
#include <list>
#include <mutex>

class C_TCP_Recv_Thread;
class C_GameData;

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

	C_GameData* m_Game;

public:
	C_TCP_Server();
	~C_TCP_Server();

	bool Init(char* szPort, C_GameData* m_data);
	void ThreadProc();
	bool CheckLogin(char* szBuffer);
};