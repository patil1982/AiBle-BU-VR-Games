// ******------------------------------------------------------******
// ServeOrder.cs
//
// Author:
//       K.Sinan Acar <ksa@puzzledwizard.com>
//
// Copyright (c) 2019 PuzzledWizard
//
// ******------------------------------------------------------******
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PW
{
    public class ServeOrder : MonoBehaviour
    {


        //public GameOverScript GameOverScript;

        int order_count = 1;
        //for demo we only check for one product
        //by order but you can add more products for a serving.
        int total_order_served = 0;

        public float totalServingTime;//Total serving time for order

        public float curServeTime; //How much time left

        Image m_Image;//Image to hold product sprite.

        public int orderID; //what is MyProduct ID

        public Image serveTimeRepresentation;

        public Text LogOrderStatus;

        public static int gamescore = 0;

        public static int servescore = 0;

        public static float gamesuccess = 0;

        public static float servesuccess = 0;

        public Text LogScoreEnter = null;

        public GameObject toastOrder;

        public void ServeMe()
        {
            var PlayerSlots = FindObjectOfType<PlayerSlots>();

            if (PlayerSlots != null)
            {
                //If player currently don't have the product ready to serve, return
                if (!PlayerSlots.HoldsItem(orderID))
                {
                    return;
                }

                total_order_served++;

                Debug.Log("Served one thing");

                Debug.Log("Served in Time!!!");
                //LogOrderStatus.text = "Served one thing";

                if (order_count == total_order_served)
                {
                    Debug.Log("customer is happy");
                    //LogOrderStatus.text = "customer is happy";
                    //We served the order, we need to raise the relevant event;
                    OrderCompleted();


                }
            }

            
        }

        public void OrderCompleted()
        {
            //We completed the order,
            //For demo purposes we will just calculate our success based on the serve-time we got
            //float score = curServeTime / totalServingTime;
            //float score = totalServingTime - curServeTime;
            float score = curServeTime;

            float success = totalServingTime / curServeTime;

            //Debug.Log(curServeTime.ToString());


            //LogOrderStatus.text = success.ToString();
            Debug.Log((totalServingTime - curServeTime).ToString());
            //Debug.Log(totalServingTime.ToString());
            Debug.Log(curServeTime.ToString());
            Debug.Log((success * 100).ToString());
            //Debug.Log((score).ToString());


            if (score < 0)
            {


                servescore = 0;
                gamescore += 0;
            }
            else
            {
                servescore = (int)(score);
                gamescore += (int)(score);
                //gamescore += (int)(score * 100);

            }


            if (success < 0)
            {
                servesuccess = 0;
                gamesuccess += 0;
            }
            else
            {
                servesuccess = (success * 100);
                gamesuccess += (success * 100);
            }
            //Debug.Log(gamescore.ToString());
            

            //LogOrderStatus.text = success.ToString();
            //we could of course calculate this on various parameters affecting
            //this success value i.e. cooking amount, speed, combo multiplier,
            //combo multipliers of same product in a row or customer happiness

            BasicGameEvents.RaiseOnOrderCompleted(orderID, success);
            StartCoroutine(DoEmphasize());
            
        }


       

        public IEnumerator DoEmphasize()
        {
            //You can do a better version of this with DOTween punchscale;
            var outline = m_Image.GetComponent<Outline>();
            Color outlineColor = outline.effectColor;
            float totalTime = .6f;
            float curTime = totalTime;
            var alphaVal = 1f;
            while (curTime > 0)
            {
                curTime -= Time.deltaTime;

                transform.localScale += Vector3.one * 0.1f * -1f * Mathf.Sin(totalTime - 2 * curTime);
                //animate outline alpha
                alphaVal += 0.1f * -1f * Mathf.Sin(totalTime - 2 * curTime);
                outline.effectColor = new Color(outlineColor.r, outlineColor.g, outlineColor.b, alphaVal);
                yield return null;
            }
            transform.localScale = Vector3.one;
            outline.effectColor = new Color(outlineColor.r, outlineColor.g, outlineColor.b, 0f);
            Destroy(gameObject);

        }

        public void SetOrder(int ID, float serveTime)
        {
            orderID = ID;
            totalServingTime = serveTime;
            curServeTime = totalServingTime;

            Debug.Log(totalServingTime.ToString());
            //If totalserving time has passed we cancel the order
            //So we set a delayed invoke with serveTime here.
            //Invoke("ServeMe", serveTime);
            //Invoke("CancelOrder", serveTime); 


           // toastOrder = GameObject.FindWithTag("StraberryToast");
           // if (toastOrder != null)
           // {

            //    StartCoroutine(DoEmphasizeOrder(toastOrder));

            //}

            
        }

        public void SetSprite(Sprite sprite)
        {
            //Because this is a one time only approach , we dont need to get reference to the sprite,
            //If you have another use case for the sprite and you'll use it more than once
            //Its a better idea to hold a reference to the sprite and get it on OnEnable or Start functions;

            m_Image = transform.GetChild(0).GetComponent<Image>();
            //m_Image.SetNativeSize();
            m_Image.sprite = sprite;
        }

        public void Update()
        {
            //we can also check for cancel time here but we don't need to.
            curServeTime -= Time.deltaTime;
            //Debug.Log(totalServingTime.ToString());
            //Debug.Log(curServeTime.ToString());
            //Update the UI progress bar
            //by finding how much time we have and divide by total time
            //Fill amounts are great for progress bars,
            //You can do a lot of UI tricks, just by using the fill mode of a UI sprite.

            

            if (serveTimeRepresentation != null)
            {
                serveTimeRepresentation.fillAmount = curServeTime / totalServingTime;
            }
        }

        public void CancelOrder()
        {
            BasicGameEvents.RaiseOnOrderCancelled(orderID);
            //Order is canncelled so destroy the UI object.
            Destroy(gameObject);
            //Debug.Log("Not Served in Time!!!");


        }

        public int  GetGameScore()
        {
            return gamescore;

        }

        public int GetServeScore()
        {
            return servescore;

        }

        public float GetGameSuccess()
        {
            return gamesuccess;

        }

        public float GetServeSuccess()
        {
            return servesuccess;

        }


        public void SetGameScore(int score)
        {
            gamescore = score;
            //servescore = score;
            gamesuccess = 0.0f;
            //servesuccess = 0.0f;

        }



        public IEnumerator DoEmphasizeOrder(GameObject toast)
        {

            GameObject child = toast.transform.GetChild(0).gameObject;
            //You can do a better version of this with DOTween punchscale;
            var outline = child.GetComponent<Outline>();
            Color outlineColor = outline.effectColor;
            float totalTime = .6f;
            float curTime = totalTime;
            var alphaVal = 1f;
            while (curTime > 0)
            {
                curTime -= Time.deltaTime;

                transform.localScale += Vector3.one * 0.1f * -1f * Mathf.Sin(totalTime - 2 * curTime);
                //animate outline alpha
                alphaVal += 0.1f * -1f * Mathf.Sin(totalTime - 2 * curTime);
                outline.effectColor = new Color(outlineColor.r, outlineColor.g, outlineColor.b, alphaVal);
                yield return null;
            }
            transform.localScale = Vector3.one;
            outline.effectColor = new Color(outlineColor.r, outlineColor.g, outlineColor.b, 0f);
            //Destroy(gameObject);

        }



    }
}