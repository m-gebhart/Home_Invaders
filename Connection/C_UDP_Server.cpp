#include "C_UDP_Server.h"
#include "C_GameObject.h"
#include "C_Session.h"
#include <iostream>
#include <stdlib.h>
#include <list>
#include <string>
#include <clocale>
#include "math.h"
#include <chrono>

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
	obj->TimerProcess();
}

bool C_UDP_Server::Init(char* szPort, C_Session* m_data)
{
	std::setlocale(LC_NUMERIC, "de_DE.UTF-8");

	// GetInfo
	m_Session = m_data;

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
	m_pThread = new std::thread(&C_UDP_Server::ThreadProcess, this);


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
void C_UDP_Server::ThreadProcess()
{
	int	 nResult;
	char szBuffer[256];

	while (true) {
		nResult = recvfrom(Server, szBuffer, sizeof(szBuffer), 0, (struct sockaddr*)&sender, &nSize);
		if (nResult > 0) {
			if (nResult < 256) szBuffer[nResult] = '\0';
			
				std::list<C_GameObject*>::iterator	i_object;

				//id requests - beggining of session
				if (strstr(szBuffer, "id:") != NULL) {
					for (i_object = m_Session->m_list_SessionPlayers.begin(); i_object != m_Session->m_list_SessionPlayers.end(); ++i_object) {
						if (atoi((char*)&szBuffer[3]) == (*i_object)->m_id) {
							(*i_object)->udp_sender = sender;
						}
					}
				}

				//position requests - continous
				else if (strstr(szBuffer, "pos:") != NULL) {
					char* szFirst = strstr(szBuffer, ":");
					char* szSec = strstr(szFirst + 1, ":");
					szSec[0] = '\0';
					int id = atoi(szFirst + 1);

					for (i_object = m_Session->m_list_SessionPlayers.begin(); i_object != m_Session->m_list_SessionPlayers.end(); ++i_object) {
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

			//projectile position requests - continous
			else if (strstr(szBuffer, "pro:") != NULL) {
				char* szFirst = strstr(szBuffer, ":");
				char* szSec = strstr(szFirst + 1, ":");
				szSec[0] = '\0';
				int id = atoi(szFirst + 1);

				C_GameObject* newProjectile = new C_GameObject(id, Projectile);
				szSec++;
				szFirst = strstr(szSec, ":");
				szFirst[0] = '\0';
				newProjectile->xPos = atof(szSec);
				newProjectile->yPos = atof(szFirst + 1);
				m_Session->m_list_Projectiles.push_back(newProjectile);
			}
		}
	}
}


void C_UDP_Server::TimerProcess()
{
	if (m_Session->m_list_SessionPlayers.size() > 0)
		new std::thread(&C_UDP_Server::SendPositions, this);

	if (m_Session->m_enemy->isAlive)
		new std::thread(&C_UDP_Server::SendEnemyPosition, this);

	//5% chance of spawning an enemy projectile in each call of TimeProcess()
	if (rand() % 20 == 0) {
		m_Session->CreateEnemyProjectile();
	}

	if (m_Session->m_list_Projectiles.size() > 0)
		new std::thread(&C_UDP_Server::SendProjectilePositions, this);

	new std::thread(&C_UDP_Server::SendServerTime, this);
}

void C_UDP_Server::SendPositions(void)
{
	SendToAllClients(GetDataListAsChar("pos:", m_Session->m_list_SessionPlayers));
}

void C_UDP_Server::SendEnemyPosition(void)
{
	char szBuffer[2048];

	m_Session->m_enemy->xPos = Lerp(
		-1*(m_Session->m_enemy->movementRange), 
		m_Session->m_enemy->movementRange, 
		((sin(m_Session->time_passed)) + m_Session->m_enemy->speed) * 0.5f);

	szBuffer[0] = '\0';
	strcpy(szBuffer, "pos:E:");
	sprintf_s(szBuffer, 128, "pos:E:%.2f:%.2f", m_Session->m_enemy->xPos, m_Session->m_enemy->yPos);

	SendToAllClients(szBuffer);
}

void C_UDP_Server::SendProjectilePositions(void)
{
	char szBuffer[128];

	std::list<C_GameObject*>::iterator	i;
	for (i = m_Session->m_list_Projectiles.begin(); i != m_Session->m_list_Projectiles.end(); ++i) {
		if ((*i)->yPos < -10.f || (*i)->yPos > 10.f)
		{
			//pop_front() only possible due to the same speed
			m_Session->m_list_Projectiles.pop_front();
			break;
		}
		(*i)->yPos += (*i)->speed*0.1f;
	}
	SendToAllClients(GetDataListAsChar("pro:", m_Session->m_list_Projectiles));
}

void C_UDP_Server::SendServerTime(void)
{
	char szBuffer[128];

	//Send in Seconds
	m_Session->time_current = std::chrono::high_resolution_clock::now();
	m_Session->time_passed = std::chrono::duration_cast<std::chrono::duration<float>>(m_Session->time_current - m_Session->time_start).count();
	sprintf_s(szBuffer, 128, "time:%.3f", m_Session->time_passed);
	//std::cout << "Server running for " << szBuffer << " seconds" << "\r";

	SendToAllClients(szBuffer);
}

void C_UDP_Server::SendToAllClients(const char* message)
{
	const std::lock_guard<std::mutex> lock(m_Mutex);
	std::list<C_GameObject*>::iterator	client;
	for (client = m_Session->m_list_SessionPlayers.begin(); client != m_Session->m_list_SessionPlayers.end(); ++client) {
		sendto(Server, message, strlen(message), 0, (struct sockaddr*)&(*client)->udp_sender, sizeof((*client)->udp_sender));
	}
}


float C_UDP_Server::Lerp(float a, float b, float t)
{
	return a + t * (b - a);
}

const char* C_UDP_Server::GetDataListAsChar(const char* key, std::list<C_GameObject*> m_list)
{
	char szBuffer[2048];
	char szClient[128];

	szBuffer[0] = '\0';
	strcpy(szBuffer, key);
	std::list<C_GameObject*>::iterator	i;
	for (i = m_list.begin(); i != m_list.end(); ++i) {
		if ((*i)->isAlive) 
		{
			sprintf_s(szClient, 128, "%d:%.2f:%.2f;", (*i)->m_id, (*i)->xPos, (*i)->yPos);
			strcat_s(szBuffer, 2048, szClient);
		}
	}
	return szBuffer;
}