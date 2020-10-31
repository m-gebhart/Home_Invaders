using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "PlayerProjectile")
        {
            GameManager manager = (GameObject.Find("/GameManager")).gameObject.GetComponent<GameManager>();
            if (manager != null)
            {
                if (other.gameObject.name[0] == GameManager.playerID)
                { 
                    manager.Win();
                    manager.tcpConn.Send("dead:E");
                }
                Destroy(other);
                (this.gameObject.GetComponent<Animator>()).Play("EnemyDestroy");
                StartCoroutine(Fade());
            }
        }
    }

    private IEnumerator Fade() 
    {
        yield return new WaitForSeconds(1.5f);
        GameManager.enemy.sceneObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0);
    }
}
