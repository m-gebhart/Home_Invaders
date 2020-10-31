#include <list>
#include <chrono>

class C_GameObject;

class C_Session {

public:
	int playerCount = 0;
	int enemyProjectileCount = 0;

	std::chrono::time_point<std::chrono::high_resolution_clock> time_start;
	std::chrono::time_point<std::chrono::high_resolution_clock> time_current;
	std::list<C_GameObject*>	m_list_SessionPlayers;
	std::list<C_GameObject*>	m_list_Projectiles;
	C_GameObject* m_enemy;
	float time_passed;

public:
	C_Session();
	void CreateEnemyProjectile();
};