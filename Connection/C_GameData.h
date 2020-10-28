#include <list>

class C_GameObject;

class C_GameData {

public:
	int playerCount = 0;

	std::list<C_GameObject*>	m_list_GameObjects;
	std::list<C_GameObject*>	m_list_Players;

public:
	C_GameData();
	~C_GameData();
};