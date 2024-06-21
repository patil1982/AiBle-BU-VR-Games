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
            m_collider.enabled = false;

            //Debug.Log(m_collider.tag);
        }

        private void OnEnable()
        {
            initialPosition = transform.position;

            initialScale = transform.localScale;


            initialLocalPosition = transform.localPosition;
            //initialPosition = transform.position;

            //Debug.Log(initialPosition);

            
        }

        public void OnMouseDown()
        {
            if (!base.CanGoPlayerSlot())
            {
                return;
            }

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

                //BasicGameEvents.RaiseInstantiatePlaceHolder(transform.parent, initialPosition, gameObject);

                //Pickedgo = gameObject;
                //Pickedgo.transform.position = transform.position;
                //Pickedgo.transform.parent = transform.parent;

                gameObject.transform.position = initialPosition;
                gameObject.transform.localScale = initialScale;
                m_collider = gameObject.GetComponent<Collider>();
                m_collider.enabled = true;
                gameObject.SetActive(true);


            }
            StartCoroutine(AnimateGoingToSlot());
           
            
           
            

        }

        public void OnRegenerateProduct()
        {
            if (RegenerateProduct)
            {
                var go = GameObject.Instantiate(gameObject, transform.parent);
                go.transform.position = initialPosition;
                go.transform.localScale = initialScale;
                m_collider = go.GetComponent<Collider>();
                m_collider.enabled = true;
                go.SetActive(true);
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