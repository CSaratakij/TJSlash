using UnityEngine;

namespace TJ
{
    public class PlayerStat : MonoBehaviour
    {
        [SerializeField]
        StatusInt health;

        public StatusInt Health => health;
    }
}
