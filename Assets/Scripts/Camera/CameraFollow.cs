using UnityEngine;

namespace TJ
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField]
        Transform target;

        [SerializeField]
        float smoothDamp;


        Vector3 offset;


        void Awake()
        {
            offset = (target.position - transform.position);
        }

        void LateUpdate()
        {
            Vector3 newTarget = target.position;
            newTarget.y = 0.0f;
            newTarget.z -= offset.z;
            transform.position = Vector3.Lerp(transform.position, newTarget, smoothDamp);
        }
    }
}
