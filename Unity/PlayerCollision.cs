using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerCollision : MonoBehaviour
{
    public GameObject PlayerExplosion;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "EnemyProjectile")
        {
            Destroy(other);
            StartCoroutine(Fade());
            GameManager.local.sceneObject.GetComponent<SpriteRenderer>().enabled = false;
            (GameObject.Find("/GameManager")).gameObject.GetComponent<GameManager>().LocalPlayerDie();
        }
    }

    //player sprite gets invisible and explosion is instantiated
    private IEnumerator Fade()
    {
        GameObject explosion = Instantiate(PlayerExplosion, this.transform.position, Quaternion.identity);
        if (this.GetComponent<SpriteRenderer>())
            this.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0);
        yield return new WaitForSeconds(0.75f);
        Destroy(explosion);
    }
}