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

     public GameObject toastC;

        private void OnCollisionEnter(Collision collision)
    {
        LogCollsiionEnter.text = "On Collision Enter: " + collision.collider.name;
        Debug.Log(LogCollsiionEnter);

        //collision.collider.gameObject.GetComponent<ReadyToServe>().OnMouseDown();
    }

    private void OnCollisionStay(Collision collision)
    {
        LogCollisionStay.text = "On Collision stay: " + collision.collider.name;
        //Debug.Log(currentFrame);
    }

    private void OnCollisionExit(Collision collision)
    {
        //LogCollisionExit.text = "On Collision exit: " + collision.collider.name;
        //Debug.Log(currentFrame);
    }

    private void OnTriggerEnter(Collider collider)
    {
            //if (collider.CompareTag("Player"))
            //{
            //    //var PlayerToast = FindObjectOfType<GrabDrop3D>();
            //    var PlayerToast = FindObjectOfType<ExoSimulatorInput2VR>();
            //    if (PlayerToast != null)
            //    {
            //        Debug.Log(PlayerToast.toast.gameObject.name);

            //        print("collid enter");

            //        toastC = PlayerToast.toast;
            //        //if (toastC != null)
            //        toastC.gameObject.GetComponent<ReadyToServe>().OnMouseDown();

            //    }


                
            //}

        }

    private void OnTriggerStay(Collider collider)
    {
        

    }

    private void OnTriggerExit(Collider collider)
    {

            if (collider.CompareTag("Player"))
            {
                //var PlayerToast = FindObjectOfType<GrabDrop3D>();
                var PlayerToast = FindObjectOfType<ExoSimulatorInput2VR>();
                if (PlayerToast != null)
                {
                    //Debug.Log(PlayerToast.toast.gameObject.name);

                    print("collid exit");

                    toastC = PlayerToast.toast;
                    if (toastC != null)
                    toastC.gameObject.GetComponent<ReadyToServe>().OnMouseDown();

                    //var VRPlayer = GameObject.FindGameObjectWithTag("Player");
                    //VRPlayer.GetComponent<BoxCollider>().enabled = false;

                    if(toastC.gameObject.GetComponent<ReadyToServe>().RegenerateProduct)
                    {
                        toastC.gameObject.GetComponent<ReadyToServe>().OnRegenerateProduct();
                    }
                        


                }


                //    //toast = GameObject.FindWithTag("StraberryToast");
                //    //if (toast != null)
                //    //{
                //    //    print("collid enter toast1");
                //    //    toast.gameObject.GetComponent<ReadyToServe>().OnMouseDown();



                //    //}

                //    //toast = GameObject.FindWithTag("EggToast");
                //    //if (toast != null && GameObject.FindWithTag("StraberryToast")==null)
                //    ////if (toast != null)
                //    //{
                //    //    print("collid enter toast2");
                //    //    toast.gameObject.GetComponent<ReadyToServe>().OnMouseDown();

                //    //}
                }

            }
}
}
