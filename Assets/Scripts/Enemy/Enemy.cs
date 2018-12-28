using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TJ
{
    [RequireComponent(typeof(EnemyStat))]
    public class Enemy : MonoBehaviour
    {
        WaitForSeconds turnNormalWait;
        SpriteRenderer spriteRenderer;
        EnemyStat stat;

        void Awake()
        {
            Initialize();
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Bullet")) {
                stat.health.Remove(1);

                if (stat.health.current > 0)
                    StartCoroutine(hitCallback());
                else
                    gameObject.SetActive(false);
            }
        }

        void Initialize()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            turnNormalWait = new WaitForSeconds(0.12f);
            stat = GetComponent<EnemyStat>();
        }

        IEnumerator hitCallback()
        {
            spriteRenderer.color = Color.red;
            yield return turnNormalWait;
            spriteRenderer.color = Color.white;
        }
    }
}
