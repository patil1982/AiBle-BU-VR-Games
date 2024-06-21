// ******------------------------------------------------------******
// OrderGenerator.cs
//
// Author:
//       K.Sinan Acar <ksa@puzzledwizard.com>
//
// Copyright (c) 2019 PuzzledWizard
//
// ******------------------------------------------------------******

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace PW
{
    public class OrderGenerator : MonoBehaviour
    {


        public GameOverScript GameOverScript;

        public int gamescore = 0;
        public float gamesuccess = 0;

        //This limits generating orders constantly
        public int MaxConcurrentOrder=1;

        public int currentOrderCount;

        public int spriteIndex = 0;

        public Sprite[] orderSprites;

        //[HideInInspector]
        public int[] orderedProducts;

        public Transform UIParentForOrders;

        public GameObject orderRepPrefab;//The general prefab for order represantation

        public GameObject[] gameObjects;


        private void OnEnable()
        {
            //We'll listen for order events;
            BasicGameEvents.onOrderCancelled += BasicGameEvents_onOrderCancelled;
            BasicGameEvents.onOrderCompleted += BasicGameEvents_onOrderCompleted;

        }
        private void OnDisable()
        {
            //Don't forget to remove listeners from events on disable.
            BasicGameEvents.onOrderCancelled -= BasicGameEvents_onOrderCancelled;
            BasicGameEvents.onOrderCompleted -= BasicGameEvents_onOrderCompleted;

        }

        private void BasicGameEvents_onOrderCancelled(int ID)
        {
            //We could also do something with the ID of the product,
            //Or we could pass other things as parameters,
            //but for demo purposes this is fine.
            currentOrderCount--;

        }

        private void BasicGameEvents_onOrderCompleted(int ID,float percentageSucccess)
        {
            currentOrderCount--;
            //In a common gameplay logic,
            //We would add money, play effects, maybe check our list of products to complete here,
            //by raising an another event or calling a method of a gamemanager like script.
            //i.e. GameManager.CheckMilestonesForOrderID(ID)
            //or BasicGameEvents.onMoneyIncreased(ID,percentageSuccess)
            //percentage of Success can define the xp we got, or money multiplier and so forth.


            //You could also use another float as a third parameter to check if an order is overcooked,
            //or just perfect.
            //You could also check combo multipliers for multiple fast deliveries
        }


        void Start()
        {
            //In a demo only manner we start calling the coroutine here on Start.

             StartCoroutine(GenerateOrderRoutine(0.9f));

        }


        public IEnumerator GenerateOrderRoutine(float intervalTime)
        {
            //We assume we don't pause the game or something,
            //You should check if your game state is playing here
            //while(true)
            while(!GameOverScript.isActiveAndEnabled)
            //while(spriteIndex < orderSprites.Length)
            {
                if (currentOrderCount < MaxConcurrentOrder)
                {
                    Debug.Log(spriteIndex);
                    GenerateOrder(spriteIndex++);
                    yield return new WaitForSeconds(intervalTime);
                    
                }
                else
                {
                    yield return new WaitForEndOfFrame();
                }
            }
            
            

            


        }

        public void GenerateOrder(int spriteIndex)
        {
            //Debug.Log("Generating order");

            //Get a random ID from sprites list
            //We could store the ID of the object to track last generated orders,
            //Totally random generation may create the same order in row repeatedly.

            //int spriteIndex = Random.Range(0, orderSprites.Length);

            if (spriteIndex < orderSprites.Length)
            {
                int orderID = orderedProducts[spriteIndex];

                var newOrder = GameObject.Instantiate(orderRepPrefab, UIParentForOrders).GetComponent<ServeOrder>();

                newOrder.SetOrder(orderID, Random.Range(40f, 60f));

                newOrder.SetSprite(orderSprites[spriteIndex]);

                SpawnToast(spriteIndex);


                // var ToastServe = FindObjectOfType<ExoSimulatorInput2VR>();

                // ToastServe.nextPosition = ToastServe.m_ObjectTransfrom[0].position;

                //Debug.Log(ToastServe.m_ObjectTransfrom[0].position);

                currentOrderCount++;



               

            }
            else
            {

                float minutes = Time.realtimeSinceStartup / 60.0f;

                var PlayerScore = FindObjectOfType<ServeOrder>();
                if (PlayerScore != null)
                {
                    gamescore = PlayerScore.GetGameScore();
                    gamesuccess = PlayerScore.GetGameSuccess() / orderSprites.Length;

                    //GameOverScript.Setup(gamescore, gamesuccess);

                    var PlayerAssistance = FindObjectOfType<ExoSimulatorInput2VR>();
                    var gameassistance = PlayerAssistance.VRgameLA;
                    GameOverScript.Setup(gamescore, gameassistance, minutes);


                    PlayerScore.SetGameScore(0);
                }

                
            }

            


        }

        public void SpawnToast(int orderIndex)
        {

                Vector3 posUser = new Vector3(0.0f, 0.0f, 0.0f);
                float xOfs = Random.Range(-0.05f, 0.45f);
                float yOfs = Random.Range(-0.23f, 0.23f);
                float zOfs = Random.Range(0.2f, 0.45f);
                
                Vector3 spawnPos = new Vector3(posUser.x + xOfs, posUser.y + yOfs, posUser.z + zOfs);

                




            ////Instantiate(gameObjects[orderIndex], spawnPos, Quaternion.identity);

            var ToastServe = FindObjectOfType<ExoSimulatorInput2VR>();

            //ToastServe.m_ObjectTransfrom[orderIndex].position = spawnPos;
            
            ToastServe.nextPosition = ToastServe.m_ObjectTransfrom[orderIndex].position;

            if (orderIndex == 0)
            {
                ToastServe.toast = GameObject.FindGameObjectWithTag("Toast1");
                ToastServe.toast.GetComponent<BoxCollider>().enabled = true;
                Debug.Log(ToastServe.nextPosition);
            }
            if (orderIndex == 1)
            {
                ToastServe.toast = GameObject.FindGameObjectWithTag("Toast2");
                ToastServe.toast.GetComponent<BoxCollider>().enabled = true;
                Debug.Log(ToastServe.nextPosition);
            }
            if (orderIndex == 2)
            {
                ToastServe.toast = GameObject.FindGameObjectWithTag("Toast3");
                ToastServe.toast.GetComponent<BoxCollider>().enabled = true;
                Debug.Log(ToastServe.nextPosition);
            }
            if (orderIndex == 3)
            {
                ToastServe.toast = GameObject.FindGameObjectWithTag("Toast4");
                ToastServe.toast.GetComponent<BoxCollider>().enabled = true;
                Debug.Log(ToastServe.nextPosition);
            }
            if (orderIndex == 4)
            {
                ToastServe.toast = GameObject.FindGameObjectWithTag("Toast5");
                ToastServe.toast.GetComponent<BoxCollider>().enabled = true;
                Debug.Log(ToastServe.nextPosition);
            }
            if (orderIndex == 5)
            {
                ToastServe.toast = GameObject.FindGameObjectWithTag("Toast6");
                ToastServe.toast.GetComponent<BoxCollider>().enabled = true;
                Debug.Log(ToastServe.nextPosition);
            }
            if (orderIndex == 6)
            {
                ToastServe.toast = GameObject.FindGameObjectWithTag("Toast7");
                ToastServe.toast.GetComponent<BoxCollider>().enabled = true;
                Debug.Log(ToastServe.nextPosition);
            }
            if (orderIndex == 7)
            {
                ToastServe.toast = GameObject.FindGameObjectWithTag("Toast8");
                ToastServe.toast.GetComponent<BoxCollider>().enabled = true;
                Debug.Log(ToastServe.nextPosition);
            }
            if (orderIndex == 8)
            {
                ToastServe.toast = GameObject.FindGameObjectWithTag("Toast9");
                ToastServe.toast.GetComponent<BoxCollider>().enabled = true;
                Debug.Log(ToastServe.nextPosition);
            }
            if (orderIndex == 9)
            {
                ToastServe.toast = GameObject.FindGameObjectWithTag("Toast10");
                ToastServe.toast.GetComponent<BoxCollider>().enabled = true;
                Debug.Log(ToastServe.nextPosition);
            }
            if (orderIndex == 10)
            {
                ToastServe.toast = GameObject.FindGameObjectWithTag("Toast11");
                ToastServe.toast.GetComponent<BoxCollider>().enabled = true;
                Debug.Log(ToastServe.nextPosition);
            }
            if (orderIndex == 11)
            {
                ToastServe.toast = GameObject.FindGameObjectWithTag("Toast12");
                ToastServe.toast.GetComponent<BoxCollider>().enabled = true;
                Debug.Log(ToastServe.nextPosition);
            }

            ToastServe.nextTargetPosition = ToastServe.m_ObjectTransfrom[orderIndex].position;


            //var PlayerToast = FindObjectOfType<ExoSimulatorInput2VR>();
            //if (PlayerToast != null)
            //{
            //    //gameObject.GetComponent<ReadyToServe>().OnMouseDown();
            //    ToastServe.m_ObjectTransfrom[orderIndex].gameObject.GetComponent<Collider>().enabled = true;
                
            //}

            float xOs = Random.Range(-0.05f, 0.4f);
            float yOs = Random.Range(-0.1f, 0.23f);
            float zOs = Random.Range(0.2f, 0.45f);

            //Vector3 spawnPosPlate = new Vector3(posUser.x + xOs, ToastServe.m_ObjectTransfrom[12].position.y, posUser.z + zOs);

            //ToastServe.m_ObjectTransfrom[12].position = spawnPosPlate;


        }

        void OnDrawGizmos()
        {
            //RightArm
            float xMin = -0.05f;
            float yMin = -0.23f;
            float zMin = 0.2f;

            float xMax = 0.45f;
            float yMax = 0.23f;
            float zMax = 0.45f;


            ////LeftArm
            //float xMin = 0.05f;
            //float yMin = -0.23f;
            //float zMin = 0.2f;

            //float xMax = -0.45f;
            //float yMax = 0.23f;
            //float zMax = 0.45f;



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




        public Sprite GetSpriteForOrder(int orderID)
        {
            var spriteIndex = Array.IndexOf(orderedProducts, orderID);
            if (spriteIndex<0)
                return null;
            return orderSprites[spriteIndex];
        }


       
    }
}