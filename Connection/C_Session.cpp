#include "C_Session.h"
#include "C_GameObject.h"


C_Session::C_Session() 
{
	time_start = std::chrono::high_resolution_clock::now();

	m_enemy = new C_GameObject(10, ObjectType::Enemy);
}