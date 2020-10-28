#include "C_TCP_Server.h"
#include "C_GameObject.h"
#include "C_GameData.h"
#include "C_TCP_Recv_Thread.h"

C_TCP_Server::C_TCP_Server()
{
	m_bRunning = true;
}

C_TCP_Server::~C_TCP_Server(){}

bool C_TCP_Server::Init(char* szPort, C_GameData* m_data)
{
	m_Game = m_data;

	// Get Info
	struct	addrinfo info;
	struct	addrinfo* result;

	ZeroMemory(&info, sizeof(info));
	info.ai_family = AF_INET;
	info.ai_socktype = SOCK_STREAM;
	info.ai_protocol = IPPROTO_TCP;
	info.ai_flags = AI_PASSIVE;
	nResult = getaddrinfo(NULL, szPort, &info, &result);

	// Create Socket
	Server = socket(result->ai_family, result->ai_socktype, result->ai_protocol);
	if (Server == INVALID_SOCKET) {
		freeaddrinfo(result);
		WSACleanup();
		return 0;
	}

	// Bind Socket to IP
	nResult = ::bind(Server, result->ai_addr, (int)result->ai_addrlen);
	if (nResult == SOCKET_ERROR) {
		freeaddrinfo(result);
		closesocket(Server);
		WSACleanup();
		return false;
	}

	// Listen on Socket
	if (listen(Server, SOMAXCONN) == SOCKET_ERROR) {
		closesocket(Server);
		return false;
	}

	// Start the accepting Thread
	m_pThread = new std::thread(&C_TCP_Server::ThreadProcess, this);

	return true;
}

void C_TCP_Server::ThreadProcess()
{
	do {
		Client = accept(Server, NULL, NULL);
		if (Client == INVALID_SOCKET) {
			freeaddrinfo(result);
			closesocket(Server);
			WSACleanup();
			return;
		}
		C_TCP_Recv_Thread* pThread = new C_TCP_Recv_Thread(this, Client);
		m_Threads.push_back(pThread);
	} while (m_bRunning);

}

bool C_TCP_Server::CheckPlayerID(char* szBuffer)
{
	szBuffer[strlen(szBuffer) - 2] = '\0';

	//10 player limit
	const std::lock_guard<std::mutex> lock(m_Mutex);
	m_Game->playerCount++;
	C_GameObject* pPlayer = new C_GameObject(m_Game->playerCount-1, Player);
	m_Game->m_list_GameObjects.push_back(pPlayer);
	m_Game->m_list_Players.push_back(pPlayer);
	char szID[24];
	sprintf(szID, "id:%d", m_Game->playerCount-1);
	send(Client, szID, strlen(szID), 0);

	return true;
}

bool C_TCP_Server::CheckPlayerLives(char* szBuffer)
{
	return true;
}

bool C_TCP_Server::CheckLevelObjects(char* szBuffer)
{
	return true;
}