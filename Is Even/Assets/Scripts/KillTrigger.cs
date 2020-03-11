using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jack.IsEven
{
    public class KillTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<BallIdentifier>())
            {
                Destroy(other.gameObject);
            }
        }
    }
}