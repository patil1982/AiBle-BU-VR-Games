// 
// ReadyToServe.cs
//
// Author:
//       K.Sinan Acar <ksa@puzzledwizard.com>
//
// Copyright (c) 2019 PuzzledWizard
//
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace PW
{
    public class ReadyToServe : ProductGameObject
    {

        public GameObject platePrefab;
        public GameObject Pickedgo;

        public Collider m_collider;

        private Vector3 initialPosition;

        private Vector3 initialScale;

        private Vector3 initialLocalPosition;

        private Vector4 initialLocalRPosition;

        private void Awake()
        {
            m_collider = GetComponent<Collider>();
            m_collider.enabled = true;
        }

        private void OnEnable()
        {
            initialPosition = transform.position;

            initialScale = transform.localScale;

            initialLocalPosition = transform.localPosition;
            //initialPosition = transform.position;
        }

        public void OnMouseDown()
        {
            //if (!base.CanGoPlayerSlot())
            //{
            //    return;
           // }

            if (AddToPlateBeforeServed)
            {
                var plate = GameObject.Instantiate(platePrefab, transform.position, Quaternion.identity);
                plate.transform.SetParent(transform);
                if (plateOffset.magnitude > 0)
                {
                    plate.transform.localPosition = plateOffset;
                }
                plate.transform.SetAsFirstSibling();//so we know what to delete later

            }
            if (RegenerateProduct)
            {
                //BasicGameEvents.RaiseInstantiatePlaceHolder(transform.parent, transform.position, gameObject);
                Pickedgo = gameObject;
                Pickedgo.transform.position = initialPosition;
                Pickedgo.transform.parent = transform.parent;
                //gameObject.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);


                //Pickedgo = GameObject.Instantiate(gameObject, transform.position, Quaternion.identity, transform.parent);
                //Pickedgo.transform.position = transform.position;

                //m_collider = Pickedgo.GetComponent<Collider>();
                //m_collider.enabled = true;

                //Pickedgo.SetActive(false);
            }
            //StartCoroutine(AnimateGoingToSlot());

        }

        public void OnRegenerateProduct()
            {
            //BasicGameEvents.RaiseInstantiatePlaceHolder(transform.parent, transform.position, gameObject);

            //Pickedgo.transform.position = transform.position;


            //var go = GameObject.Instantiate(Pickedgo, initialPosition, Quaternion.identity, Pickedgo.transform.parent);
            //go.transform.position = initialPosition;

            // m_collider = go.GetComponent<Collider>();
            // m_collider.enabled = true;

            //go.SetActive(true);

                var PizzaBase = GameObject.FindWithTag("OrderPlate");
                if (PizzaBase != null)
                {
                    float xOfs = Random.Range(-0.07f, 0.07f);
                    float zOfs = Random.Range(-0.07f, 0.07f);
                    float yOfs = Random.Range(0.007f, 0.03f);

                    Vector3 vrBasket = PizzaBase.gameObject.transform.position;

                    Vector3 fruitPos = new Vector3(vrBasket.x + xOfs, vrBasket.y + yOfs, vrBasket.z + zOfs);


                    var go2 = GameObject.Instantiate(Pickedgo, fruitPos, Quaternion.identity);
                    go2.transform.position = fruitPos;
                    go2.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    m_collider = go2.GetComponent<Collider>();
                    m_collider.enabled = false;
                    go2.SetActive(true);
                }



            gameObject.transform.position = initialPosition;
            gameObject.transform.localScale = initialScale;
            m_collider = gameObject.GetComponent<Collider>();
            m_collider.enabled = true;
            gameObject.SetActive(true);

            var PlayerRecipe = FindObjectOfType<ExoSimulatorInput2VR>();
            if (PlayerRecipe.Item1Count != 0)
            {

                PlayerRecipe.recipe = GameObject.FindGameObjectWithTag("Anchovy");
                PlayerRecipe.recipe.GetComponent<MeshCollider>().enabled = false;

                PlayerRecipe.recipe = GameObject.FindGameObjectWithTag("RedPepper");
                PlayerRecipe.recipe.GetComponent<MeshCollider>().enabled = false;

                PlayerRecipe.recipe = GameObject.FindGameObjectWithTag("GreenOlive");
                PlayerRecipe.recipe.GetComponent<MeshCollider>().enabled = false;

                PlayerRecipe.recipe = GameObject.FindGameObjectWithTag("BasilLeaf");
                PlayerRecipe.recipe.GetComponent<MeshCollider>().enabled = false;

                PlayerRecipe.recipe = GameObject.FindGameObjectWithTag("LemonSlice");
                PlayerRecipe.recipe.GetComponent<MeshCollider>().enabled = true;

                if (PlayerRecipe.recipe != null)
                {
                    //PlayerRecipe.gameObject.SetActive(true);
                    PlayerRecipe.nextPosition = PlayerRecipe.m_ObjectTransfrom[0].position;

                }
            }
            else if (PlayerRecipe.Item2Count != 0)
            {
                print("Goto Recipe 2");

                
                PlayerRecipe.recipe = GameObject.FindGameObjectWithTag("RedPepper");
                PlayerRecipe.recipe.GetComponent<MeshCollider>().enabled = false;

                PlayerRecipe.recipe = GameObject.FindGameObjectWithTag("GreenOlive");
                PlayerRecipe.recipe.GetComponent<MeshCollider>().enabled = false;

                PlayerRecipe.recipe = GameObject.FindGameObjectWithTag("BasilLeaf");
                PlayerRecipe.recipe.GetComponent<MeshCollider>().enabled = false;

                PlayerRecipe.recipe = GameObject.FindGameObjectWithTag("LemonSlice");
                PlayerRecipe.recipe.GetComponent<MeshCollider>().enabled = false;

                PlayerRecipe.recipe = GameObject.FindGameObjectWithTag("Anchovy");
                PlayerRecipe.recipe.GetComponent<MeshCollider>().enabled = true;

                if (PlayerRecipe.recipe != null)
                {
                    //PlayerRecipe.gameObject.SetActive(true);
                    PlayerRecipe.nextPosition = PlayerRecipe.m_ObjectTransfrom[1].position;
                    //Debug.Log(PlayerRecipe.nextPosition);

                }

            }
            else if (PlayerRecipe.Item3Count != 0)
            {
                print("Goto Recipe 3");

                
                PlayerRecipe.recipe = GameObject.FindGameObjectWithTag("GreenOlive");
                PlayerRecipe.recipe.GetComponent<MeshCollider>().enabled = false;

                PlayerRecipe.recipe = GameObject.FindGameObjectWithTag("BasilLeaf");
                PlayerRecipe.recipe.GetComponent<MeshCollider>().enabled = false;

                PlayerRecipe.recipe = GameObject.FindGameObjectWithTag("LemonSlice");
                PlayerRecipe.recipe.GetComponent<MeshCollider>().enabled = false;

                PlayerRecipe.recipe = GameObject.FindGameObjectWithTag("Anchovy");
                PlayerRecipe.recipe.GetComponent<MeshCollider>().enabled = false;

                PlayerRecipe.recipe = GameObject.FindGameObjectWithTag("RedPepper");
                PlayerRecipe.recipe.GetComponent<MeshCollider>().enabled = true;

                if (PlayerRecipe.recipe != null)
                {
                    //PlayerRecipe.gameObject.SetActive(true);
                    PlayerRecipe.nextPosition = PlayerRecipe.m_ObjectTransfrom[2].position;
                    //Debug.Log(PlayerRecipe.nextPosition);

                }

            }
            else if (PlayerRecipe.Item4Count != 0)
            {
                print("Goto Recipe 4");

 
                PlayerRecipe.recipe = GameObject.FindGameObjectWithTag("BasilLeaf");
                PlayerRecipe.recipe.GetComponent<MeshCollider>().enabled = false;

                PlayerRecipe.recipe = GameObject.FindGameObjectWithTag("LemonSlice");
                PlayerRecipe.recipe.GetComponent<MeshCollider>().enabled = false;

                PlayerRecipe.recipe = GameObject.FindGameObjectWithTag("Anchovy");
                PlayerRecipe.recipe.GetComponent<MeshCollider>().enabled = false;

                PlayerRecipe.recipe = GameObject.FindGameObjectWithTag("RedPepper");
                PlayerRecipe.recipe.GetComponent<MeshCollider>().enabled = false;

                PlayerRecipe.recipe = GameObject.FindGameObjectWithTag("GreenOlive");
                PlayerRecipe.recipe.GetComponent<MeshCollider>().enabled = true;

                if (PlayerRecipe.recipe != null)
                {
                    //PlayerRecipe.gameObject.SetActive(true);
                    PlayerRecipe.nextPosition = PlayerRecipe.m_ObjectTransfrom[3].position;
                    //Debug.Log(PlayerRecipe.nextPosition);

                }

            }
            else if (PlayerRecipe.Item5Count != 0)
            {
                print("Goto Recipe 5");

 
                PlayerRecipe.recipe = GameObject.FindGameObjectWithTag("LemonSlice");
                PlayerRecipe.recipe.GetComponent<MeshCollider>().enabled = false;

                PlayerRecipe.recipe = GameObject.FindGameObjectWithTag("Anchovy");
                PlayerRecipe.recipe.GetComponent<MeshCollider>().enabled = false;

                PlayerRecipe.recipe = GameObject.FindGameObjectWithTag("RedPepper");
                PlayerRecipe.recipe.GetComponent<MeshCollider>().enabled = false;

                PlayerRecipe.recipe = GameObject.FindGameObjectWithTag("GreenOlive");
                PlayerRecipe.recipe.GetComponent<MeshCollider>().enabled = false;

                PlayerRecipe.recipe = GameObject.FindGameObjectWithTag("BasilLeaf");
                PlayerRecipe.recipe.GetComponent<MeshCollider>().enabled = true;

                if (PlayerRecipe.recipe != null)
                {
                    //PlayerRecipe.gameObject.SetActive(true);
                    PlayerRecipe.nextPosition = PlayerRecipe.m_ObjectTransfrom[4].position;
                    //Debug.Log(PlayerRecipe.nextPosition);

                }

            }


        }

    public override IEnumerator AnimateGoingToSlot()
        {

            yield return base.AnimateGoingToSlot();

            var PlayerServe = FindObjectOfType<ServeOrder>();

            PlayerServe.ServeMe();

            gameObject.SetActive(false);
        }

        
    }
}