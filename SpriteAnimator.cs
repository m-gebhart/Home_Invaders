using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    public Sprite baseSprite, secondarySprite;
    public float frequency = 0.5f;
    public static float universalDelay = 0.2f;
    bool baseSpriteDisplayed = true, isPlaying = false;
    SpriteRenderer renderer;

    void Start()
    {
        renderer = this.gameObject.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (universalDelay < Time.deltaTime && !isPlaying)
        {
            if (secondarySprite != null)
                StartCoroutine("AnimateSprite");
        }
    }

    IEnumerator AnimateSprite()
    {
        this.enabled = false;
        isPlaying = true;
        do
        {
            yield return new WaitForSeconds(frequency);
            if (baseSpriteDisplayed)
                renderer.sprite = secondarySprite;
            else
                renderer.sprite = baseSprite;
            baseSpriteDisplayed = !baseSpriteDisplayed;
        } while (true);
    }
}
