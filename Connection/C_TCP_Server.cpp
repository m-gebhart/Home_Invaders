#include "C_TCP_Server.h"
#include "C_GameObject.h"
#include "C_GameData.h"
#include "C_TCP_Recv_Thread.h"

C_TCP_Server::C_TCP_Server()
{
	m_bRunning = true;
}

C_TCP_Server::~C_TCP_Server()
{
}

bool C_TCP_Server::Init(char* szPort, C_GameData* m_data)
{
	m_Game = m_data;

	// GetInfo

	struct		addrinfo info;
	struct		addrinfo* result;

	ZeroMemory(&info, sizeof(info));
	info.ai_family = AF_INET;
	info.ai_socktype = SOCK_STREAM;
	info.ai_protocol = IPPROTO_TCP;
	info.ai_flags = AI_PASSIVE;

	nResult = getaddrinfo(NULL, szPort, &info, &result);

	// Create socket

	Server = socket(result->ai_family, result->ai_socktype, result->ai_protocol);
	if (Server == INVALID_SOCKET) {
		//cout << "Socket Error: " << WSAGetLastError() + "\n";
		freeaddrinfo(result);
		WSACleanup();
		return 0;
	}

	// Bind socket to ip

	nResult = ::bind(Server, result->ai_addr, (int)result->ai_addrlen);
	if (nResult == SOCKET_ERROR) {
		//cout << "Bind Error: " << WSAGetLastError() + "\n";
		freeaddrinfo(result);
		closesocket(Server);
		WSACleanup();
		return false;
	}

	// Listen on socket

	if (listen(Server, SOMAXCONN) == SOCKET_ERROR) {
		//cout << "Listen Error: " << WSAGetLastError() + "\n";
		closesocket(Server);
		return false;
	}

	// Start the accepting thread

	m_pThread = new std::thread(&C_TCP_Server::ThreadProc, this);

	return true;
}

void C_TCP_Server::ThreadProc()
{
	do {
		Client = accept(Server, NULL, NULL);
		if (Client == INVALID_SOCKET) {
			//cout << "Socket Error: " << WSAGetLastError() + "\n";
			freeaddrinfo(result);
			closesocket(Server);
			WSACleanup();
			return;
		}

		C_TCP_Recv_Thread* pThread = new C_TCP_Recv_Thread(this, Client);
		m_Threads.push_back(pThread);
	} while (m_bRunning);

}

bool C_TCP_Server::CheckLogin(char* szBuffer)
{
	//cout << "CheckLogin: " << szBuffer << endl;

	std::list<C_GameObject*>::iterator	i;

	for (i = m_Game->m_list_GameObjects.begin(); i != m_Game->m_list_GameObjects.end(); ++i) {
		if (strcmp((char*)&szBuffer[6], (*i)->m_szName) == 0) return false;
	}

	szBuffer[strlen(szBuffer) - 2] = '\0';

	m_Mutex.lock();
	m_Game->playerCount++;
	C_GameObject* pItem = new C_GameObject(m_Game->playerCount, (char*)&szBuffer[6]);
	m_Game->m_list_GameObjects.push_back(pItem);
	char szID[24];
	sprintf(szID, "id:%d", pItem->m_nID);

	if (send(Client, szID, strlen(szID), 0) == SOCKET_ERROR) {
		//cout << "Socket Error: " << WSAGetLastError() + "\n";
	}
	m_Mutex.unlock();
	return true;
}