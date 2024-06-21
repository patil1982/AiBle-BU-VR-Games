using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace PW
{
    public class GameReceipeMenu : MonoBehaviour
    {
        public void Setup()
        {

            gameObject.SetActive(true);
            
        }

        public void Disable()
        {

            gameObject.SetActive(false);

        }
    }
}
