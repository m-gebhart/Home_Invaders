using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "PlayerProjectile")
        {
            Destroy(other);
            Die();
        }
    }

    private void Die()
    {
        (GameObject.Find("/GameManager")).gameObject.GetComponent<GameManager>().tcpConn.Send("dead:E");
        (this.gameObject.GetComponent<Animator>()).Play("EnemyDestroy");
        StartCoroutine(Fade());
    }

    private IEnumerator Fade() 
    {
        yield return new WaitForSeconds(1.5f);
        GameManager.enemy.sceneObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0);
    }
}
