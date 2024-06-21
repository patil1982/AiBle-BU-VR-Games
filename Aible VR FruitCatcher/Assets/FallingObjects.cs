using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace PW
{
    public class FallingObjects : MonoBehaviour
    {
        
        [Tooltip("How many objects do we want to spawn.")]
        public int numberOfObjects = 20;
        public Transform[] myObjects;

        public GameOverScript GameOverScript;

        public CollisionDetect GameOver;

        bool endgame = true;

        private float nextSpawnTime = 0.0f;
        public float spawnRate = 30f;
        public float dragRate = 70f;
        private int ballsCount = 0;

        public bool dropleft = false;
        public bool dropright = false;


        void Update()
        {
            if (nextSpawnTime < Time.time)
            {
                SpawnBalls();
                nextSpawnTime = Time.time + spawnRate;

                //spawnRate = Random.Range(10f, 20f);
                //numberOfBalls = Mathf.RoundToInt(Random.Range(1f, 10f));
            }
        }

        void SpawnBalls()
        {

            

            //if (ballPrefab && cubePrefab && ballsCount < numberOfObjects)
            if (ballsCount < numberOfObjects)
             {
                
                Vector3 posUser = new Vector3(0.0f, 0.0f, 0.0f);
                float xOfs = 0.0f;
                if (dropleft)
                  xOfs = Random.Range(-0.0f, 0.1f);

                if(dropright)
                  xOfs = Random.Range(0.2f, 0.45f);

                if(!dropleft && !dropright)
                  xOfs = Random.Range(-0.05f, 0.45f);

                 float zOfs = Random.Range(0.2f, 0.45f);
                 float yOfs = Random.Range(-0.23f, 0.23f);
                Vector3 spawnPos = new Vector3(posUser.x + xOfs, posUser.y + yOfs, posUser.z + zOfs);

                //int ballOrCube = Mathf.RoundToInt(Random.Range(0f, 1f));
                int randomIndex = Random.Range(0,myObjects.Length);

                Transform ballTransform = Instantiate(myObjects[randomIndex], spawnPos, Quaternion.identity) as Transform;
                //Transform ballTransform = Instantiate(ballOrCube > 0 ? ballPrefab : cubePrefab, spawnPos, Quaternion.identity) as Transform;
                ballTransform.GetComponent<Renderer>().material.color = new Color(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), 1f);
                //ballTransform.GetComponent<Rigidbody>().drag = Random.Range(18f, 45f);
                ballTransform.GetComponent<Rigidbody>().drag = dragRate;

                ballTransform.parent = transform;

                var FruitPos = FindObjectOfType<ExoSimulatorInput2VR>();

                //FruitPos.m_ObjectTransfrom.position = spawnPos;
                //FruitPos.nextPosition = FruitPos.m_ObjectTransfrom.position;

                //FruitPos.nextPosition = ballTransform.position;

                FruitPos.Fruit = ballTransform.GetComponent<Rigidbody>().gameObject;
                FruitPos.nextPosition = FruitPos.Fruit.transform.position;
                FruitPos.FirstTime = true;

                Debug.Log(FruitPos.nextPosition);

                Debug.Log(ballTransform.position);

                ballsCount++;
            }
            else
            {
                print("End Game");

                var FruitPos = FindObjectOfType<ExoSimulatorInput2VR>();
                FruitPos.nextPosition = FruitPos.HomePosition;
                FruitPos.GotoHome = true;
                FruitPos.FirstTime = false;

                float minutes = Time.realtimeSinceStartup / 60.0f;

                float successrate = ((GameOver.gamesuccess / 10)/numberOfObjects)*100;


                if (endgame)
                {
                    GameOverScript.Setup(GameOver.gamescore, successrate, minutes);
                    endgame = !endgame;
                }
                


            }
        }

        void OnDrawGizmos()
        {
            float xMin = 0.45f;
            float yMin = -0.23f;
            float zMin = 0.20f;

            float xMax = -0.05f;
            float yMax = 0.23f;
            float zMax = 0.45f;



            Gizmos.color = Color.yellow;


            // Now we can simply calculate our 8 vertices of the bounding box
            Vector3 A = new Vector3(xMin, yMin, zMin);
            Vector3 B = new Vector3(xMin, yMin, zMax);
            Vector3 C = new Vector3(xMin, yMax, zMin);
            Vector3 D = new Vector3(xMin, yMax, zMax);

            Vector3 E = new Vector3(xMax, yMin, zMin);
            Vector3 F = new Vector3(xMax, yMin, zMax);
            Vector3 G = new Vector3(xMax, yMax, zMin);
            Vector3 H = new Vector3(xMax, yMax, zMax);


            // And finally visualize it
            Gizmos.DrawLine(A, B);
            Gizmos.DrawLine(B, D);
            Gizmos.DrawLine(D, C);
            Gizmos.DrawLine(C, A);

            Gizmos.DrawLine(E, F);
            Gizmos.DrawLine(F, H);
            Gizmos.DrawLine(H, G);
            Gizmos.DrawLine(G, E);

            Gizmos.DrawLine(A, E);
            Gizmos.DrawLine(B, F);
            Gizmos.DrawLine(D, H);
            Gizmos.DrawLine(C, G);

        }

        public void DropLeftButton()
        {

            Debug.Log("Drop Left");
            dropleft = true;
            dropright = false;
        }

        public void DropRightButton()
        {
            
            Debug.Log("Drop Right");
            dropleft = false;
            dropright = true;

        }

        public void BasketLeftButton()
        {

            var Fruit_GroundBasket = GameObject.FindWithTag("FruitBasket");
            if (Fruit_GroundBasket != null)
            {

                Vector3 fruitBasketPos = new Vector3(-1.0f, -1.0f, 1.2f);
                Fruit_GroundBasket.gameObject.transform.position = fruitBasketPos;

            }

        }

        public void BasketRightButton()
        {

            var Fruit_GroundBasket = GameObject.FindWithTag("FruitBasket");
            if (Fruit_GroundBasket != null)
            {

                Vector3 fruitBasketPos = new Vector3(1.0f, -1.0f, 1.2f);
                Fruit_GroundBasket.gameObject.transform.position = fruitBasketPos;

            }

        }

        public void DropRandomButton()
        {

            Debug.Log("Drop Random");
            dropleft = false;
            dropright = false;

        }




    }
}


