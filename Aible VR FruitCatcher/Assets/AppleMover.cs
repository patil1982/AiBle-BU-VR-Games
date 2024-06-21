using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PW
{
    public class AppleMover : MonoBehaviour
    {

        // Update is called once per frame
        void Update()
        {
            if (transform.position.y < -2f)
            {
                //Destroy(gameObject);
            }
        }
    }
}
