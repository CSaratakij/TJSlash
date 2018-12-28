﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TJ
{
    public class Gun : MonoBehaviour
    {
        [SerializeField]
        [Range(1, 100)]
        int maxBulletPooling;

        [SerializeField]
        float bulletSpeed;

        [SerializeField]
        GameObject bulletPrefabs;

        [SerializeField]
        Transform barrelPoint;


        bool isAllowShoot;

        Bullet[] bullets;
        Timer fireDelay;


        void Awake()
        {
            Initialize();
            SubscribeEvent();
        }

        void OnDestroy()
        {
            UnSubscribeEvent();
        }

        void Initialize()
        {
            isAllowShoot = true;

            fireDelay = GetComponent<Timer>();
            bullets = new Bullet[maxBulletPooling];

            for (int i = 0; i < maxBulletPooling; ++i) {
                bullets[i] = Instantiate(bulletPrefabs, barrelPoint.position, Quaternion.identity).GetComponent<Bullet>();
                bullets[i].gameObject.SetActive(false);
            }
        }

        void SubscribeEvent()
        {
            fireDelay.OnTimerStop += fireDelay_OnStop;
        }

        void UnSubscribeEvent()
        {
            fireDelay.OnTimerStop -= fireDelay_OnStop;
        }

        void fireDelay_OnStop()
        {
            isAllowShoot = true;
        }

        void Shoot(Vector2 direction)
        {
            foreach (Bullet bullet in bullets)
            {
                if (bullet.gameObject.activeSelf) {
                    continue;
                }
                else {
                    bullet.Move(direction, bulletSpeed);
                    bullet.transform.position = barrelPoint.position;
                    bullet.gameObject.SetActive(true);

                    isAllowShoot = false;
                    fireDelay.Countdown();
                    break;
                }
            }
        }

        public void BeginShoot(Vector2 direction)
        {
            if (isAllowShoot)
                Shoot(direction);
        }
    }
}
