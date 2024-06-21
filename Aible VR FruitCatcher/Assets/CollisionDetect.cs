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
        public GameObject Fruit_GroundBasket;

        public int gamescore = 0;
        public float gamesuccess = 0;
        public float gameduration = 0;

        //public Color redcolor;
        //public Color blackcolor;

        void Start()
        {
            buzzaudio = GetComponent<AudioSource>();
            gamescore = 0;
            gamesuccess = 0;
        }

        private void OnCollisionEnter(Collision collision)
        {
            //LogCollsiionEnter.text = "On Collision Enter: " + collision.collider.name;
            //Debug.Log(LogCollsiionEnter);
            if (collision.collider.CompareTag("BuzzWire"))
            {
                //print("collid BuzzWire0");
                //buzzaudio.Play();
                //gamescore += 25;
            }




            //collision.collider.gameObject.GetComponent<ReadyToServe>().OnMouseDown();
        }

        private void OnCollisionStay(Collision collision)
        {
            //LogCollisionStay.text = "On Collision stay: " + collision.collider.name;
            //Debug.Log(currentFrame);
            //buzzaudio.Play();
            //Debug.Log(LogCollisionStay);

        }

        private void OnCollisionExit(Collision collision)
        {
            //LogCollisionExit.text = "On Collision exit: " + collision.collider.name;
            //Debug.Log(currentFrame);
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("Catcher"))
            {
                //print("collid BuzzWire0");
                buzzaudio.Play();
                gamescore += 10;
                Debug.Log("collid Catcher");

                gamesuccess = gamescore;

               

                Fruit_GroundBasket = GameObject.FindWithTag("FruitBasket");
                if (Fruit_GroundBasket != null)
                {
                    float xOfs = Random.Range(-0.0f, 0.1f);
                    float zOfs = Random.Range(-0.0f, 0.1f);
                    float yOfs = Random.Range(0.2f, 0.25f);

                    Vector3 vrBasket = Fruit_GroundBasket.gameObject.transform.position;

                    Vector3 fruitPos = new Vector3(vrBasket.x + xOfs, vrBasket.y + yOfs, vrBasket.z + zOfs);


                    collider.gameObject.transform.position = fruitPos;
                }

                var FruitPos = FindObjectOfType<ExoSimulatorInput2VR>();


                FruitPos.FirstTime = false;
                FruitPos.GotoHome = true;
                FruitPos.nextPosition = FruitPos.HomePosition;

                
                //Destroy(collider.gameObject);
            }

        }

        private void OnTriggerStay(Collider collider)
        {
           

        }

        private void OnTriggerExit(Collider collider)
        {
         
        }
    }
}
