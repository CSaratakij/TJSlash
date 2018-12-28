using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TJ
{
    public class Bullet : MonoBehaviour
    {
        float force;
        Vector2 direction;
        Rigidbody2D rigid;

        void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            rigid.velocity = (direction * force) * Time.fixedDeltaTime;
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.CompareTag("Bullet") || collider.gameObject.CompareTag("Potion"))
                return;

            gameObject.SetActive(false);
        }

        public void Move(Vector2 direction, float force)
        {
            this.direction = direction;
            this.force = force;
        }
    }
}
