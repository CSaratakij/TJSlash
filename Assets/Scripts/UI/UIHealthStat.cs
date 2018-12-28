using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TJ
{
    public class UIHealthStat : MonoBehaviour
    {
        [SerializeField]
        Stat stat;

        [SerializeField]
        Sprite heartFull;

        [SerializeField]
        Sprite heartEmpty;

        [SerializeField]
        Image[] imgHearts;


        void Awake()
        {
            SubscribeEvent();
        }

        void Start()
        {
            UpdateUI(stat.health.current);
        }

        void OnDestroy()
        {
            UnSubscribeEvent();
        }

        void SubscribeEvent()
        {
            stat.health.OnValueChanged += health_OnValueChanged;
        }

        void UnSubscribeEvent()
        {
            stat.health.OnValueChanged -= health_OnValueChanged;
        }

        void health_OnValueChanged(int value)
        {
            UpdateUI(value);
        }

        void UpdateUI(int value)
        {
            for (int i = 0; i < imgHearts.Length; ++i)
            {
                if (i < value) {
                    imgHearts[i].sprite = heartFull;
                }
                else {
                    imgHearts[i].sprite = heartEmpty;
                }
            }
        }
    }
}
