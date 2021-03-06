#include <iostream>
#include "C_Session.h"
#include "C_GameObject.h"
#include "C_UDP_Server.h"
#include "C_TCP_Server.h"

int main(int argc,char *argv[])
{
	WSADATA		wsData;
	int nResult = WSAStartup(MAKEWORD(2,2),&wsData);

	C_Session* m_InitData = new C_Session();

	C_UDP_Server* p_UDP_Server = new C_UDP_Server();
	if (p_UDP_Server->Init("20000", m_InitData) == 0)
		std::cout << "UDP Connection failed" << std::endl;

	C_TCP_Server* p_TCP_Server = new C_TCP_Server();
	if (p_TCP_Server->Init("20001", m_InitData) == 0)
		std::cout << "TCP Connection failed" << std::endl;

	getchar();

	return 0;
}