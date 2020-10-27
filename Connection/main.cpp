#include <stdio.h>
#include <iostream>
#include <thread>
#include <mutex>
#include <list>
#include <winsock2.h>
#include <ws2tcpip.h>
#include "C_GameObject.h"
#include "C_GameData.h"
#include "C_UDP_Server.h"
#include "C_TCP_Server.h"

using namespace	std;

// -----------------------------------------------------------------------------------------

int main(int argc,char *argv[])
{
	WSADATA		wsData;
	int nResult = WSAStartup(MAKEWORD(2,2),&wsData);

	C_GameData* m_GameData = new C_GameData();

	C_UDP_Server* pUDPServer = new C_UDP_Server();
	if (pUDPServer->Init("20000", m_GameData) == 0)
		cout << "UDP failed" << endl;

	C_TCP_Server* pTCPServer = new C_TCP_Server();
	if (pTCPServer->Init("20001", m_GameData) == 0)
		cout << "TCP failed" << endl;

	getchar();


	return 0;
}