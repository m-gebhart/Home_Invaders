#include "C_TCP_Server.h"
#include "C_GameObject.h"
#include "C_Session.h"
#include "C_TCP_Recv_Thread.h"
#include <iostream>

C_TCP_Server::C_TCP_Server()
{
	m_bRunning = true;
}

C_TCP_Server::~C_TCP_Server(){}

bool C_TCP_Server::Init(char* szPort, C_Session* m_data)
{
	m_Session = m_data;

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

bool C_TCP_Server::Login(char* szBuffer)
{
	const std::lock_guard<std::mutex> lock(m_Mutex);
	if (m_Session->playerCount < 10)
		CheckPlayerID(szBuffer);
	else
		send(Client, "Error: Login Failed - PlayerLimitReached", 40, 0);
	return true;
}

bool C_TCP_Server::CheckPlayerID(char* szBuffer)
{
	szBuffer[strlen(szBuffer) - 2] = '\0';

	m_Session->playerCount++;
	C_GameObject* pPlayer = new C_GameObject(m_Session->playerCount - 1, Player);
	m_Session->m_list_SessionPlayers.push_back(pPlayer);
	char szID[24];
	sprintf(szID, "id:%d", m_Session->playerCount - 1);
	send(Client, szID, strlen(szID), 0);
	return true;
}

bool C_TCP_Server::SetDead(char* szBuffer)
{
	char szDead[24];
	char* deadObject;

	if (strstr(szBuffer, "E") != NULL)
	{
		deadObject = "E";
		m_Session->m_enemy->isAlive = false;
	}
	else if (atoi)
		deadObject = "";
	
	
	sprintf(szDead, "dead:%s", deadObject);
	const std::lock_guard<std::mutex> lock(m_Mutex);
	send(Client, szDead, strlen(szDead), 0);
	return true;
}


bool C_TCP_Server::Logout(char* szBuffer) { return true; }