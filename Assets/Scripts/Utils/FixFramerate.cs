using UnityEngine;

namespace TJ
{
    public class FixFramerate : MonoBehaviour
    {
        [SerializeField]
        int targetFrameRate = 60;

        void Awake()
        {
            Application.targetFrameRate = targetFrameRate;
        }
    }
}
