using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PW
{
    public class CollisionDetectPlate : ProductGameObject
    {
    public Text LogCollsiionEnter;
    public Text LogCollisionStay;
    public Text LogCollisionExit;

     public GameObject R_recipe;

        private void OnCollisionEnter(Collision collision)
    {
        //LogCollsiionEnter.text = "On Collision Enter: " + collision.collider.name;
        //Debug.Log(LogCollsiionEnter);

        //collision.collider.gameObject.GetComponent<ReadyToServe>().OnMouseDown();
    }

    private void OnCollisionStay(Collision collision)
    {
        //LogCollisionStay.text = "On Collision stay: " + collision.collider.name;
        //Debug.Log(currentFrame);
    }

    private void OnCollisionExit(Collision collision)
    {
        //LogCollisionExit.text = "On Collision exit: " + collision.collider.name;
        //Debug.Log(currentFrame);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("StraberryToast"))
        {
            print("collid enter");
        }

        if (collider.CompareTag("Player"))
        {
            print("collid enter");

            //collider.gameObject.GetComponent<ReadyToServe>().OnMouseDown();
        }

        if (collider.CompareTag("OrderPlate"))
        {
            print("collid enter");

            //collider.gameObject.GetComponent<ReadyToServe>().OnMouseDown();
        }


    }

    private void OnTriggerStay(Collider collider)
    {
       
            if (collider.CompareTag("Player"))
            {
               print("collid stay");


            }



    }

    private void OnTriggerExit(Collider collider)
    {

            print("Player exit");

            var PlayerRecipe = FindObjectOfType<ExoSimulatorInput2VR>();
            if (PlayerRecipe != null)
            {
                Debug.Log(PlayerRecipe.recipe.gameObject.name);

                print("Player exit");

                R_recipe = PlayerRecipe.recipe;
                if (R_recipe != null)
                    R_recipe.gameObject.GetComponent<ReadyToServe>().OnRegenerateProduct();


            }


        }
}
}
