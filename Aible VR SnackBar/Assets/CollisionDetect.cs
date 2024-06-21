using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PW
{
    public class CollisionDetect : ProductGameObject
    {
        public Text LogCollsiionEnter;
        public Text LogCollisionStay;
        public Text LogCollisionExit;

        private void OnCollisionEnter(Collision collision)
        {
            LogCollsiionEnter.text = "On Collision Enter: " + collision.collider.name;
            Debug.Log(LogCollsiionEnter);

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
            if(collider.CompareTag("StraberryToast"))
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
            if (collider.CompareTag("StraberryToast"))
            {
                print("collid stay");
            }

            //if (collider.CompareTag("Player"))
            //{
             //   print("collid enter");

                //collider.gameObject.GetComponent<ReadyToServe>().OnMouseDown();
            //}



        }

        private void OnTriggerExit(Collider collider)
        {
            if (collider.CompareTag("StraberryToast"))
            {
                print("collid exit");
            }

        }
    }
}
