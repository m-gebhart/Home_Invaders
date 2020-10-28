#include <winsock2.h>
#include <ws2tcpip.h>
#include <thread>
#include <mutex>
#include <list>

class C_GameData;

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

	C_GameData *m_Game;


public:
	C_UDP_Server();
	~C_UDP_Server();

	bool Init(char* szPort, C_GameData* m_data);
	void ThreadProc();
	void SendPositions(void);
	void TimerProc();
};