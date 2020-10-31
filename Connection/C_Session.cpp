#include "C_Session.h"
#include "C_GameObject.h"
#include <cmath>

C_Session::C_Session() 
{
	time_start = std::chrono::high_resolution_clock::now();

	m_enemy = new C_GameObject(10, ObjectType::Enemy);
}

void C_Session::CreateEnemyProjectile()
{
	C_GameObject* newProjectile = new C_GameObject(10+(++enemyProjectileCount), ObjectType::Projectile);
	newProjectile->xPos = m_enemy->xPos;
	newProjectile->yPos = m_enemy->yPos + 0.25f;
	newProjectile->speed = abs(newProjectile->speed);
	m_list_Projectiles.push_back(newProjectile);
}