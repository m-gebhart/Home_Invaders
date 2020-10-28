#include "C_UDP_Server.h"
#include "C_GameObject.h"
#include "C_GameData.h"
#include <iostream>
#include <string>
#include <clocale>

C_UDP_Server::C_UDP_Server(){}

C_UDP_Server::~C_UDP_Server()
{
	timeKillEvent(m_idEvent);

	freeaddrinfo(result);
	closesocket(Server);
	WSACleanup();
}

void CALLBACK TimerFunction(UINT wTimerID, UINT msg, DWORD dwUser, DWORD dw1, DWORD dw2)
{
	C_UDP_Server* obj = (C_UDP_Server*)dwUser;
	obj->TimerProc();
}

bool C_UDP_Server::Init(char* szPort, C_GameData* m_data)
{
	std::setlocale(LC_NUMERIC, "de_DE.UTF-8");

	// GetInfo
	m_Game = m_data;

	struct	addrinfo info;

	ZeroMemory(&info, sizeof(info));
	info.ai_family = AF_INET;
	info.ai_socktype = SOCK_DGRAM;
	info.ai_protocol = IPPROTO_UDP;
	info.ai_flags = AI_PASSIVE;

	nResult = getaddrinfo(NULL, szPort, &info, &result);

	// Create Socket
	Server = socket(result->ai_family, result->ai_socktype, result->ai_protocol);
	if (Server == INVALID_SOCKET) {
		freeaddrinfo(result);
		WSACleanup();
		return false;
	}

	// Receive
	ZeroMemory(&sender, sizeof(sender));
	sender.sin_family = AF_INET;
	sender.sin_port = htons(atoi(szPort));
	sender.sin_addr.s_addr = INADDR_ANY;
	nSize = sizeof(sender);

	nResult = ::bind(Server, (struct sockaddr*)&sender, nSize);
	if (Server == INVALID_SOCKET) {
		freeaddrinfo(result);
		closesocket(Server);
		WSACleanup();
		return false;
	}

	//Receive Thread
	m_pThread = new std::thread(&C_UDP_Server::ThreadProc, this);


	TIMECAPS tc;
	timeGetDevCaps(&tc, sizeof(TIMECAPS));
	int resolution = min(max(tc.wPeriodMin, 0), tc.wPeriodMax);
	timeBeginPeriod(resolution);

	//Sent Data by timed Events
	m_idEvent = timeSetEvent(
		100,
		resolution,
		TimerFunction,
		(DWORD)this,
		TIME_PERIODIC
	);

	return true;
}

//Receiving...
void C_UDP_Server::ThreadProc()
{
	int	 nResult;
	char szBuffer[256];

	while (true) {
		nResult = recvfrom(Server, szBuffer, sizeof(szBuffer), 0, (struct sockaddr*)&sender, &nSize);
		if (nResult > 0) {
			if (nResult < 256) szBuffer[nResult] = '\0';
			//cout << szBuffer;
			std::list<C_GameObject*>::iterator	i_object;

			//id requests
			if (strstr(szBuffer, "id:") != NULL) {
				for (i_object = m_Game->m_list_GameObjects.begin(); i_object != m_Game->m_list_GameObjects.end(); ++i_object) {
					if (atoi((char*)&szBuffer[3]) == (*i_object)->m_id) {
						(*i_object)->udp_sender = sender;
					}
				}
			}

			//position requests
			if (strstr(szBuffer, "pos:") != NULL) {
				char* szFirst = strstr(szBuffer, ":");
				char* szSec = strstr(szFirst + 1, ":");
				szSec[0] = '\0';
				int id = atoi(szFirst + 1);

				//change position for requested id
				for (i_object = m_Game->m_list_GameObjects.begin(); i_object != m_Game->m_list_GameObjects.end(); ++i_object) {
					if (id == (*i_object)->m_id) {
						szSec++;
						szFirst = strstr(szSec, ":");
						szFirst[0] = '\0';

						(*i_object)->xPos = atof(szSec);
						(*i_object)->yPos = atof(szFirst+1);
						break;
					}
				}
			}
		}
	}
}


void C_UDP_Server::TimerProc()
{
	SendPositions();
}

void C_UDP_Server::SendPositions(void)
{
	char szBuffer[2048];
	char szClient[128];

	szBuffer[0] = '\0';
	strcpy(szBuffer, "pos:");
	std::list<C_GameObject*>::iterator	i;
	const std::lock_guard<std::mutex> lock(m_Mutex);
	for (i = m_Game->m_list_GameObjects.begin(); i != m_Game->m_list_GameObjects.end(); ++i) {
		sprintf_s(szClient, 128, "%d:%.2f:%.2f;", (*i)->m_id, (*i)->xPos, (*i)->yPos);
		strcat_s(szBuffer, 2048, szClient);
	}

	for (i = m_Game->m_list_GameObjects.begin(); i != m_Game->m_list_GameObjects.end(); ++i) {
		sendto(Server, szBuffer, strlen(szBuffer), 0, (struct sockaddr*)&(*i)->udp_sender, sizeof((*i)->udp_sender));
	}
}