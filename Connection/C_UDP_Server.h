#include <winsock2.h>
#include <ws2tcpip.h>
#include <thread>
#include <mutex>
#include <list>

class C_Session;

class C_UDP_Server
{
	SOCKET			Server;
	struct			sockaddr_in	sender;
	int				nSize;
	int				nResult;
	struct			addrinfo* result;
	std::thread*	m_pThread;
	std::mutex		m_Mutex;
	MMRESULT		m_idEvent;
	C_Session *m_Session;


public:
	C_UDP_Server();
	~C_UDP_Server();

	bool Init(char* szPort, C_Session* m_data);
	void ThreadProcess();
	void TimerProcess();
	void SendPositions(void);
	void SendServerTime(void);
	void SendToAllClients(const char* message);
};