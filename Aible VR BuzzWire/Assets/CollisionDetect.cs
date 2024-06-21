using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PW
{
    public class CollisionDetect : MonoBehaviour
    {
        public Text LogCollsiionEnter;
        public Text LogCollisionStay;
        public Text LogCollisionExit;

        AudioSource buzzaudio;

        public GameOverScript GameOverScript;

        public int gamescore = 100;
        public float gamesuccess = 0;

        public Color redcolor;
        public Color blackcolor;

        void Start()
        {
            buzzaudio = GetComponent<AudioSource>();
            gamescore = 100;
            gamesuccess = 100;
        }

        private void OnCollisionEnter(Collision collision)
        {
            //LogCollsiionEnter.text = "On Collision Enter: " + collision.collider.name;
            //Debug.Log(LogCollsiionEnter);
            //buzzaudio.Play();
            if (collision.collider.CompareTag("BuzzWire"))
            {
                print("collid BuzzWire0");
                buzzaudio.Play();
                transform.GetComponent<Renderer>().material.color = redcolor;
                gamescore -= 1;
            }

            if (collision.collider.CompareTag("EndPillar"))
            {
                print("End BuzzWireGame");

                float minutes = Time.realtimeSinceStartup / 60.0f;

                GameOverScript.Setup(gamescore, gamesuccess, minutes);
            }



            //collision.collider.gameObject.GetComponent<ReadyToServe>().OnMouseDown();
        }

        private void OnCollisionStay(Collision collision)
        {
            //LogCollisionStay.text = "On Collision stay: " + collision.collider.name;
            //Debug.Log(currentFrame);
            //buzzaudio.Play();

            if (collision.collider.CompareTag("BuzzWire"))
            {
                print("collid stay BuzzWire");
                //buzzaudio.Play();
                //transform.GetComponent<Renderer>().material.color = redcolor;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            //LogCollisionExit.text = "On Collision exit: " + collision.collider.name;
            //Debug.Log(currentFrame);

            if (collision.collider.CompareTag("BuzzWire"))
            {
                print("collid exit BuzzWire");
            }

            transform.GetComponent<Renderer>().material.color = blackcolor;
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("BuzzWire"))
            {
                print("collid BuzzWire0");
                buzzaudio.Play();
                gamescore -= 1;
            }



            if (collider.CompareTag("EndPillar"))
            {
                print("End BuzzWireGame");
                var nextpath = FindObjectOfType<ExoSimulatorInput2VR>();
                nextpath.FirstTime = false;
                nextpath.GotoHome = true;

                float minutes = Time.realtimeSinceStartup / 60.0f;

                GameOverScript.Setup(gamescore, gamesuccess, minutes);
            }

            if (collider.CompareTag("StartPillar"))
            {
                print("Start BuzzWireGame");
                //var nextpath = FindObjectOfType<ExoSimulatorInput2VR>();
                //nextpath.FirstTime = false;
                //nextpath.GotoHome = true;

                
            }


        }

        private void OnTriggerStay(Collider collider)
        {
            if (collider.CompareTag("BuzzWire"))
            {
                print("collid stay BuzzWire");
                buzzaudio.Play();
                transform.GetComponent<Renderer>().material.color = redcolor;
            }




        }

        private void OnTriggerExit(Collider collider)
        {
            if (collider.CompareTag("BuzzWire"))
            {
                print("collid exit BuzzWire");
            }

            transform.GetComponent<Renderer>().material.color = blackcolor;

        }
    }
}
