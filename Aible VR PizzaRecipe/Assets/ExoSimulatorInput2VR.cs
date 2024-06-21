#define DEBUG_INFOS


using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;
using System.Threading;


namespace PW
{
    public class ExoSimulatorInput2VR : ProductGameObject
    {

        [Header("Tracking Points")]
        public Transform[] m_JointTransfrom;
        public Transform[] m_ObjectTransfrom;
        //public Transform m_ObjectTransfrom;

        public Vector3 armPosition;
        public Vector4 armRotation;

        public Vector3 elbowPosition;
        public Vector4 elbowRotation;

        public Vector3 wristPosition;
        public Vector4 wristRotation;

        public Vector3 elbowPosition_pBase;
        public Vector4 elbowRotation_pBase;

        public Vector3 wristPosition_pBase;
        public Vector4 wristRotation_pBase;

        public Vector3 HomePosition;
        public Vector4 HomeRotation;

        public Vector3 nextPosition = new Vector3(-0.0f, 0.0f, 0.0f);
        public Quaternion nextRotation = Quaternion.identity;



        public Quaternion qRotation = Quaternion.identity;

        public Quaternion qArmRotation = Quaternion.identity;
        public Quaternion qElbowRotation_pBase = Quaternion.identity;
        public Quaternion qWristRotation_pBase = Quaternion.identity;
        public Quaternion qElbowRotation = Quaternion.identity;
        public Quaternion qWristRotation = Quaternion.identity;

        public Quaternion qRotationOffset = Quaternion.identity;



        public Quaternion qArmRotation_Unity = Quaternion.identity;
        public Quaternion qElbowRotation_Unity = Quaternion.identity;
        public Quaternion qWristRotation_Unity = Quaternion.identity;




        Quaternion offsetArmExo = Quaternion.identity;
        Quaternion offsetElbowExo = Quaternion.identity;
        Quaternion offsetWristExo = Quaternion.identity;



        public float timeSinceLerpStart;
        public float interval;
        public float inverseInterval;
        public float deltaTime;

        public GameObject VRCube;
        public GameObject VRShoulder;
        public GameObject VRElbow;
        public GameObject VRWrist;

        public Text LogCubeEnter;
        public Text LogShoulderEnter;
        public Text LogElbowEnter;
        public Text LogWristEnter;


        System.Globalization.CultureInfo invC = System.Globalization.CultureInfo.InvariantCulture;


        public float m_xoffset;
        public float m_yoffset;
        public float m_zoffset;

        public Text LogCollsiionEnter;
        public bool m_objTrans1 = false;
        public bool m_objTrans2 = false;
        public bool m_objTrans3 = false;
        public bool m_objTrans4 = false;

        public bool m_objTrans5 = false;
        public bool m_objTrans6 = false;
        public bool m_objTrans7 = false;
        public bool m_objTrans8 = false;

        public bool m_objTrans9 = false;
        public bool m_objTrans10 = false;
        public bool m_objTrans11 = false;
        public bool m_objTrans12 = false;

        public bool FirstTime = false;
        public bool GotoHome = false;

        public bool GotoTargetPlate = false;

        public GameObject recipe;
        public GameObject TargetDough;

        public GameObject playerarm;

        public GameOverScript GameOverScript;
        public GameReceipeMenu GameReceipeScript;

        public int gamescore = 0;
        public float gamesuccess = 0;

        public ReadyToServe GO;

        public Text LogScoreEnter;
        public Text LogSuccessEnter;

        public int Servescore = 0;
        public float Servesuccess = 0.0f;

        public Text Item1Text;
        public int Item1Count;

        public Text Item2Text;
        public int Item2Count;

        public Text Item3Text;
        public int Item3Count;

        public Text Item4Text;
        public int Item4Count;

        public Text Item5Text;
        public int Item5Count;


        public float armCheckFrequency = 1f; // seconds between checking the ZMQ queue for another arm update
        public string armServerAddr = "tcp://localhost:8899";
        
        private RequestSocket client;
        private string msgString;


        public Transform vrArmShoulderPos, vrArmElbowPos, vrArmWristPos;
        public Transform vrObjectPos;
        public Transform vrHomePos;
        public Transform vrTargetPos;

        public Vector3 vrPosition = new Vector3(-0.0f, 0.0f, 0.0f);
        public Quaternion vrRotation = Quaternion.identity;

        public Vector3 vrCubePosition = new Vector3(-0.0f, 0.0f, 0.0f);
        public Quaternion vrcubeRotation = Quaternion.identity;

        public bool moveWrist = true;
        public bool revrsWrist = false;

        public int numPathChildren = 0;
        public int MaxPathChildren = 0;
        public Vector3  nxtPathPos ;
        public Quaternion nxtPathQu;

        // Start is called before the first frame update
        void Start()
        {


            AsyncIO.ForceDotNet.Force();
            //NetMQConfig.Cleanup();

            this.client = new RequestSocket();
            this.client.Connect(this.armServerAddr);


            if (this.client != null)
                Debug.Log("VRArmController :: Connected to ZMQ Server at " + this.armServerAddr);
            if (this.client == null)
            {


                Debug.Log("VRArmController :: Please Connect to ZMQ Server at " + this.armServerAddr);
                NetMQConfig.Cleanup(false);
            }


            //Invoke("CheckForVRArmUpdate", 0.1f);


            m_xoffset = 0.00f;
            m_yoffset = -0.00f;
            m_zoffset = -0.10f;

            Servescore = 0;
            Servesuccess = 0.0f;

            Item1Count = 4;
            Item2Count = 4;
            Item3Count = 5;
            Item4Count = 9;
            Item5Count = 6;
            

            gamescore = 100;
            gamesuccess = 100;

            m_objTrans1 = false;
            m_objTrans2 = false;

            interval = 0.05f;
            inverseInterval = 1 / interval;


            offsetArmExo = FindRotationOffetShoulderCurrentVirtualToZeroRealQuaternionPUnity(m_JointTransfrom[0].localRotation);

            offsetElbowExo = FindRotationOffetElbowCurrentVirtualToZeroRealQuaternionPUnity(m_JointTransfrom[1].localRotation);

            offsetWristExo = FindRotationOffetWristCurrentVirtualToZeroRealQuaternionPUnity(m_JointTransfrom[2].localRotation);

 

            VRWrist = GameObject.FindWithTag("Player");

            if (VRWrist != null)
            {

                this.client.SendMoreFrame("VRInit").SendFrame(HomePosition.ToString());

                NetMQMessage msgServer = this.client.ReceiveMultipartMessage();
                //Debug.Log(msgServer.FrameCount);
                //Debug.Log(msgServer[0].ConvertToString());
                //Debug.Log(msgServer[1].ConvertToString());

                var msgCommand = msgServer[0].ConvertToString();
                if(string.Equals(msgCommand, "ExoData"))
                {
                    string msg2VRarm = msgServer[1].ConvertToString();
                    ProcessArmUpdate(msg2VRarm);

 
                    VRCube = GameObject.FindWithTag("GameController");
                    if (VRCube != null)
                    {

                        HomePosition = VRCube.gameObject.transform.position;
                        Debug.Log(HomePosition);
                        nextPosition = HomePosition;
                    }


                }
   

            }

            TargetDough = GameObject.FindGameObjectWithTag("OrderPlate");
            TargetDough.GetComponent<BoxCollider>().enabled = false;
            FirstTime = true;

            

        }

        IEnumerator GetResponse()
        {
            yield return null;
            Thread.Sleep(100);
            if (client.TryReceiveFrameString(out var message))
            {
                Debug.Log("Received " + message);
            }
            else
            {
                Debug.Log("Received no message.");
            }
        }

        void CheckForVRArmUpdate()
        {

            vrPosition.x = -nextPosition.x + m_xoffset;
            vrPosition.y = nextPosition.y + m_yoffset;
            vrPosition.z = nextPosition.z + m_zoffset;


            //qRotationOffset = new Quaternion((float)0.0, (float)0.0, (float)0.0, (float)1.0);
            ////qRotationOffset = new Quaternion((float)0.5000, (float)-0.5000, (float)-0.5000, (float)0.5000);
            
            //Quaternion qRotationTmp = Quaternion.identity;
            //qRotationTmp = ConvertLeftHandedToRightHandedQuaternion(nextRotation);
            //vrRotation = Quaternion.Inverse(qRotationOffset) * qRotationTmp;





            Debug.Log(vrPosition);

            string msgvr = vrPosition.z.ToString() + ' ' + vrPosition.x.ToString() + ' ' + vrPosition.y.ToString() +
                            ' ' + vrRotation.x.ToString() + ' ' + vrRotation.y.ToString() + ' ' + vrRotation.z.ToString() + ' ' + vrRotation.w.ToString();

            SendVRPositionUpdate(msgvr);



            NetMQMessage msgServer = this.client.ReceiveMultipartMessage();
            //Debug.Log(msgServer.FrameCount);
            //Debug.Log(msgServer[0].ConvertToString());
            //Debug.Log(msgServer[1].ConvertToString());

            var msgCommand = msgServer[0].ConvertToString();
            if (string.Equals(msgCommand, "ExoData"))
            {
                string msg2VRarm = msgServer[1].ConvertToString();
                ProcessArmUpdate(msg2VRarm);

            }


            // now schedule the next check (otherwise we would stop checking)
            //Invoke("CheckForVRArmUpdate", this.armCheckFrequency);
        }

        void ProcessArmUpdate(string msgarm)
        {
#if DEGUG_INFOS
            Debug.Log(msgarm);
#endif
            //Debug.Log(msgarm);
            var armpt = msgarm.Split(' ');
            armPosition = new Vector3(float.Parse(armpt[0], invC), float.Parse(armpt[1], invC), float.Parse(armpt[2], invC));
            //qArmRotation = new Quaternion(float.Parse(armpt[3], invC), float.Parse(armpt[4], invC), float.Parse(armpt[5], invC), float.Parse(armpt[6], invC));
            qArmRotation = new Quaternion(float.Parse(armpt[3]), float.Parse(armpt[4]), float.Parse(armpt[5]), float.Parse(armpt[6]));
#if DEGUG_INFOS
        Debug.Log(qArmRotation.x.ToString());Debug.Log(qArmRotation.y.ToString());Debug.Log(qArmRotation.z.ToString());Debug.Log(qArmRotation.w.ToString());
            //Debug.Log(float.Parse(armpt[3])); Debug.Log(float.Parse(armpt[4], invC)); Debug.Log(float.Parse(armpt[5], invC)); Debug.Log(float.Parse(armpt[6], invC));
            //Debug.Log(armpt[3]); Debug.Log(armpt[4]); Debug.Log(armpt[5]); Debug.Log(armpt[6]);
#endif
            var elbowpt = msgarm.Split(' ');
            elbowPosition_pBase = new Vector3(float.Parse(elbowpt[7], invC), float.Parse(elbowpt[8], invC), float.Parse(elbowpt[9], invC));
            //qElbowRotation_pBase = new Quaternion(float.Parse(elbowpt[10], invC), float.Parse(elbowpt[11], invC), float.Parse(elbowpt[12], invC), float.Parse(elbowpt[13], invC));
            qElbowRotation_pBase = new Quaternion(float.Parse(elbowpt[10]), float.Parse(elbowpt[11]), float.Parse(elbowpt[12]), float.Parse(elbowpt[13]));
            qElbowRotation = Quaternion.Inverse(qArmRotation) * qElbowRotation_pBase;
#if DEGUG_INFOS
            Debug.Log(qElbowRotation.x.ToString());Debug.Log(qElbowRotation.y.ToString()); Debug.Log(qElbowRotation.z.ToString()); Debug.Log(qElbowRotation.w.ToString());
            //Debug.Log(float.Parse(armpt[10], invC)); Debug.Log(float.Parse(armpt[11], invC)); Debug.Log(float.Parse(armpt[12], invC)); Debug.Log(float.Parse(armpt[13], invC));
            //Debug.Log(armpt[10]); Debug.Log(armpt[11]); Debug.Log(armpt[12]); Debug.Log(armpt[13]);
#endif
            var wristpt = msgarm.Split(' ');
            wristPosition_pBase = new Vector3(float.Parse(wristpt[14], invC), float.Parse(wristpt[15], invC), float.Parse(wristpt[16], invC));
            //qWristRotation_pBase = new Quaternion(float.Parse(wristpt[17], invC), float.Parse(wristpt[18], invC), float.Parse(wristpt[19], invC), float.Parse(wristpt[20], invC));
            qWristRotation_pBase = new Quaternion(float.Parse(wristpt[17]), float.Parse(wristpt[18]), float.Parse(wristpt[19]), float.Parse(wristpt[20]));
            qWristRotation = Quaternion.Inverse(qElbowRotation_pBase) * qWristRotation_pBase;
#if DEGUG_INFOS
            Debug.Log(qWristRotation.ToString());
#endif
            qArmRotation_Unity = ConvertRightHandedToLeftHandedQuaternionSpace(qArmRotation);
            m_JointTransfrom[0].localRotation = Quaternion.Slerp(m_JointTransfrom[0].localRotation, qArmRotation_Unity * offsetArmExo, inverseInterval * deltaTime);

            qElbowRotation_Unity = ConvertRightHandedToLeftHandedQuaternionShoulder(qElbowRotation);
            m_JointTransfrom[1].localRotation = Quaternion.Slerp(m_JointTransfrom[1].localRotation, qElbowRotation_Unity * offsetElbowExo, inverseInterval * deltaTime);

            qWristRotation_Unity = ConvertRightHandedToLeftHandedQuaternionElbow(qWristRotation);
            m_JointTransfrom[2].localRotation = Quaternion.Slerp(m_JointTransfrom[2].localRotation, qWristRotation_Unity * offsetWristExo, inverseInterval * deltaTime);
        }


        ////RightArm
        //private Quaternion FindRotationOffetShoulderCurrentVirtualToZeroRealQuaternionPUnity(Quaternion virtualQuaternion)
        //{
        //    qRotation = new Quaternion((float)0.5000, (float)-0.5000, (float)-0.5000, (float)-0.5000);
        //    qRotationOffset = ConvertRightHandedToLeftHandedQuaternionSpace(qRotation);
        //    Quaternion qRotationTmp = Quaternion.identity;
        //    qRotationTmp = Quaternion.Inverse(qRotationOffset) * virtualQuaternion;
        //    return qRotationTmp; //working 
        //}

        ////RightArm
        //private Quaternion FindRotationOffetElbowCurrentVirtualToZeroRealQuaternionPUnity(Quaternion virtualQuaternion)
        //{
        //    qRotation = new Quaternion((float)0.0, (float)0.0, (float)0.0, (float)1.0);
        //    qRotationOffset = ConvertRightHandedToLeftHandedQuaternionShoulder(qRotation);
        //    Quaternion qRotationTmp = Quaternion.identity;
        //    qRotationTmp = Quaternion.Inverse(qRotationOffset) * virtualQuaternion;
        //    return qRotationTmp; //working 
        //}

        ////RightArm
        //private Quaternion FindRotationOffetWristCurrentVirtualToZeroRealQuaternionPUnity(Quaternion virtualQuaternion)
        //{
        //    qRotation = new Quaternion((float)0.5000, (float)-0.5000, (float)-0.5000, (float)0.5000);
        //    qRotationOffset = ConvertRightHandedToLeftHandedQuaternionElbow(qRotation);
        //    Quaternion qRotationTmp = Quaternion.identity;
        //    qRotationTmp = Quaternion.Inverse(qRotationOffset) * virtualQuaternion;
        //    return qRotationTmp; //working 
        //}

        ////RightArm
        //private Quaternion ConvertRightHandedToLeftHandedQuaternionSpace(Quaternion rightHandedQuaternion)
        //{
        //    return new Quaternion(-rightHandedQuaternion.y, rightHandedQuaternion.z, rightHandedQuaternion.x, -rightHandedQuaternion.w); //working 
        //}
        //private Quaternion ConvertRightHandedToLeftHandedQuaternionShoulder(Quaternion rightHandedQuaternion)
        //{
        //    return new Quaternion(rightHandedQuaternion.z, -rightHandedQuaternion.x, rightHandedQuaternion.y, -rightHandedQuaternion.w); //working 
        //}
        //private Quaternion ConvertRightHandedToLeftHandedQuaternionElbow(Quaternion rightHandedQuaternion)
        //{
        //    return new Quaternion(rightHandedQuaternion.z, -rightHandedQuaternion.x, rightHandedQuaternion.y, -rightHandedQuaternion.w); //working 
        //}

        private Quaternion ConvertLeftHandedToRightHandedQuaternion(Quaternion leftHandedQuaternion)
        {
            return new Quaternion(-leftHandedQuaternion.y, leftHandedQuaternion.z, leftHandedQuaternion.x, leftHandedQuaternion.w);
        }


        //LeftArm
        private Quaternion FindRotationOffetShoulderCurrentVirtualToZeroRealQuaternionPUnity(Quaternion virtualQuaternion)
        {
            qRotation = new Quaternion((float)-0.5000, (float)0.5000, (float)0.5000, (float)0.5000);
            qRotationOffset = ConvertRightHandedToLeftHandedQuaternionSpace(qRotation);
            Quaternion qRotationTmp = Quaternion.identity;
            qRotationTmp = Quaternion.Inverse(qRotationOffset) * virtualQuaternion;
            return qRotationTmp; //working 
        }




        //LeftArm
        private Quaternion FindRotationOffetElbowCurrentVirtualToZeroRealQuaternionPUnity(Quaternion virtualQuaternion)
        {
            qRotation = new Quaternion((float)0.0, (float)0.0, (float)0.0, (float)1.0);
            qRotationOffset = ConvertRightHandedToLeftHandedQuaternionShoulder(qRotation);
            Quaternion qRotationTmp = Quaternion.identity;
            qRotationTmp = Quaternion.Inverse(qRotationOffset) * virtualQuaternion;
            return qRotationTmp; //working 
        }



        //LeftArm
        private Quaternion FindRotationOffetWristCurrentVirtualToZeroRealQuaternionPUnity(Quaternion virtualQuaternion)
        {
            qRotation = new Quaternion((float)0.5000, (float)-0.5000, (float)-0.5000, (float)0.5000);
            qRotationOffset = ConvertRightHandedToLeftHandedQuaternionElbow(qRotation);
            Quaternion qRotationTmp = Quaternion.identity;
            qRotationTmp = Quaternion.Inverse(qRotationOffset) * virtualQuaternion;
            return qRotationTmp; //working 
        }

        //LeftArm
        private Quaternion ConvertRightHandedToLeftHandedQuaternionSpace(Quaternion rightHandedQuaternion)
        {
            return new Quaternion(-rightHandedQuaternion.y, rightHandedQuaternion.z, rightHandedQuaternion.x, -rightHandedQuaternion.w); //working 
        }
        private Quaternion ConvertRightHandedToLeftHandedQuaternionShoulder(Quaternion rightHandedQuaternion)
        {
            return new Quaternion(rightHandedQuaternion.y, rightHandedQuaternion.z, -rightHandedQuaternion.x, -rightHandedQuaternion.w); //working 
        }
        private Quaternion ConvertRightHandedToLeftHandedQuaternionElbow(Quaternion rightHandedQuaternion)
        {
            return new Quaternion(rightHandedQuaternion.x, rightHandedQuaternion.z, rightHandedQuaternion.y, -rightHandedQuaternion.w); //working ok for rotation in left wrist
        }

        void SendVRPositionUpdate(string msgvr)
        {
#if DEGUG_INFOS
        Debug.Log(msgvr);
#endif
            //client.SendFrame(msgvr);

            this.client.SendMoreFrame("VRData").SendFrame(msgvr);

        }

        private void OnApplicationQuit()
        {
            //client.SendFrame("End");
            NetMQConfig.Cleanup(false);
            Debug.Log("VRArmJointController exited.");
        }

        // Update is called once per frame
        void Update()
        {

            deltaTime = Time.time - timeSinceLerpStart;

            if (deltaTime >= interval)
            {

                timeSinceLerpStart = Time.time;

               

                if (m_objTrans1)
                {
                    m_ObjectTransfrom[0].position = m_JointTransfrom[3].position;

                    var dis = Vector3.Distance(m_JointTransfrom[3].position, HomePosition); // Calculating Distance
                    if (dis <= 0.09f) // checking if distance is less than required distance.
                    //if (GotoTargetPlate)
                    {
                        Debug.Log("Item1 Go TargetDough");
                        TargetDough.GetComponent<BoxCollider>().enabled = true;
                        VRWrist = GameObject.FindWithTag("Player");
                        VRWrist.GetComponent<BoxCollider>().enabled = true;

                        //m_xoffset = 0.00f;
                        //m_yoffset = 0.00f;
                        //m_zoffset = -0.10f;

                        nextPosition = m_ObjectTransfrom[5].position;
                        Debug.Log(nextPosition);
                        GotoTargetPlate = false;

                    }

                }

                if (m_objTrans2)
                {
                    m_ObjectTransfrom[1].position = m_JointTransfrom[3].position;

                    var dis = Vector3.Distance(m_JointTransfrom[3].position, HomePosition); // Calculating Distance
                    if (dis <= 0.09f) // checking if distance is less than required distance.
                    //if (GotoTargetPlate)
                    {
                        Debug.Log("Item2 Go TargetDoug");

                        TargetDough.GetComponent<BoxCollider>().enabled = true;
                        VRWrist = GameObject.FindWithTag("Player");
                        VRWrist.GetComponent<BoxCollider>().enabled = true;

                        //m_xoffset = 0.00f;
                        //m_yoffset = 0.00f;
                        //m_zoffset = -0.10f;

                        nextPosition = m_ObjectTransfrom[5].position;
                        Debug.Log(nextPosition);
                        GotoTargetPlate = false;
                    }


                }

                if (m_objTrans3)
                {
                    m_ObjectTransfrom[2].position = m_JointTransfrom[3].position;

                    var dis = Vector3.Distance(m_JointTransfrom[3].position, HomePosition); // Calculating Distance
                    if (dis <= 0.09f) // checking if distance is less than required distance.
                    //if (GotoTargetPlate)
                    {
                        Debug.Log("Item3 Go TargetDoug");

                        TargetDough.GetComponent<BoxCollider>().enabled = true;
                        VRWrist = GameObject.FindWithTag("Player");
                        VRWrist.GetComponent<BoxCollider>().enabled = true;

                        //m_xoffset = 0.00f;
                        //m_yoffset = 0.00f;
                        //m_zoffset = -0.10f;

                        nextPosition = m_ObjectTransfrom[5].position;
                        Debug.Log(nextPosition);
                        GotoTargetPlate = false;
                    }


                }

                if (m_objTrans4)
                {
                    m_ObjectTransfrom[3].position = m_JointTransfrom[3].position;

                    var dis = Vector3.Distance(m_JointTransfrom[3].position, HomePosition); // Calculating Distance
                    if (dis <= 0.09f) // checking if distance is less than required distance.
                    //if (GotoTargetPlate)
                    {
                        Debug.Log("Item4 Go TargetDoug");

                        TargetDough.GetComponent<BoxCollider>().enabled = true;
                        VRWrist = GameObject.FindWithTag("Player");
                        VRWrist.GetComponent<BoxCollider>().enabled = true;

                        //m_xoffset = 0.00f;
                        //m_yoffset = 0.00f;
                        //m_zoffset = -0.10f;

                        nextPosition = m_ObjectTransfrom[5].position;
                        Debug.Log(nextPosition);
                        GotoTargetPlate = false;
                    }


                }

                if (m_objTrans5)
                {
                    m_ObjectTransfrom[4].position = m_JointTransfrom[3].position;

                    var dis = Vector3.Distance(m_JointTransfrom[3].position, HomePosition); // Calculating Distance
                    if (dis <= 0.09f) // checking if distance is less than required distance.
                    //if (GotoTargetPlate)
                    {
                        Debug.Log("Item5 Go TargetDoug");

                        TargetDough.GetComponent<BoxCollider>().enabled = true;
                        VRWrist = GameObject.FindWithTag("Player");
                        VRWrist.GetComponent<BoxCollider>().enabled = true;

                        //m_xoffset = 0.00f;
                        //m_yoffset = 0.00f;
                        //m_zoffset = -0.10f;

                        nextPosition = m_ObjectTransfrom[5].position;
                        Debug.Log(nextPosition);
                        GotoTargetPlate = false;
                    }


                }

                
                if (FirstTime)
                {
                    Debug.Log("First Pos");

                    
                    recipe = GameObject.FindGameObjectWithTag("Anchovy");
                    recipe.GetComponent<MeshCollider>().enabled = false;

                    recipe = GameObject.FindGameObjectWithTag("RedPepper");
                    recipe.GetComponent<MeshCollider>().enabled = false;

                    recipe = GameObject.FindGameObjectWithTag("GreenOlive");
                    recipe.GetComponent<MeshCollider>().enabled = false;

                    recipe = GameObject.FindGameObjectWithTag("BasilLeaf");
                    recipe.GetComponent<MeshCollider>().enabled = false;

                    recipe = GameObject.FindGameObjectWithTag("LemonSlice");
                    if (recipe != null)
                    {
                        nextPosition = m_ObjectTransfrom[0].position;
                        Debug.Log(nextPosition);
                    }



                }

                if (GotoHome)
                {
                    Debug.Log("Goto Home");
                    nextPosition = HomePosition;
                    GotoHome = false;

                    

                }

                VRCube = GameObject.FindWithTag("GameController");
                if (VRCube != null)
                {

                    HomePosition = VRCube.gameObject.transform.position;
                    
                    ////nextPosition = HomePosition;
                }


            

            VRCube = GameObject.FindWithTag("GameController");
            if (VRCube != null)
            {

                //vrPosition = VRCube.gameObject.transform.position;

                //vrRotation = VRCube.gameObject.transform.localRotation;

                //LogCubeEnter.text = "Cube Position: " + vrCubePosition.ToString();
                //LogShoulderEnter.text = "Shoulder Position: " + VRShoulder.gameObject.transform.position.ToString();
                //LogElbowEnter.text = "Elbow Position: " + VRElbow.gameObject.transform.position.ToString();
                //LogWristEnter.text = "Wrist Position: " + VRWrist.gameObject.transform.position.ToString();

            }

            Invoke("CheckForVRArmUpdate", 0.1f);

            
            //LogScoreEnter.text = "Points: " + Servescore.ToString();
            //LogSuccessEnter.text = "Success Rate: " + Servesuccess.ToString("F2") + "%";
        }
    }


        private void OnTriggerEnter(Collider collider)
        {


            if (collider.CompareTag("GameController"))
            {
                Debug.Log("Trigger GameController");
                //LogCollsiionEnter.text = "On Collision Enter: " + collider.name;

                VRCube = GameObject.FindWithTag("GameController");
                if (VRCube != null)
                {
                    VRCube.GetComponent<Renderer>().material.color = Color.red;
                }
            }

            if (collider.CompareTag("LemonSlice"))
            {
                Debug.Log("Trigger 1 Goto Home");
                m_objTrans1 = true;
                FirstTime = false;

                Item1Count--;
                Item1Text.text = Item1Count.ToString();
                collider.gameObject.GetComponent<ReadyToServe>().OnMouseDown();

                
                //recipe = GameObject.FindGameObjectWithTag("Anchovy");
                //recipe.GetComponent<MeshCollider>().enabled = false;
                VRWrist.GetComponent<BoxCollider>().enabled = false;

                recipe = GameObject.FindGameObjectWithTag("LemonSlice");
                recipe.GetComponent<MeshCollider>().enabled = false;

                m_xoffset = 0.00f;
                m_yoffset = 0.00f;
                m_zoffset = -0.10f;

                nextPosition = HomePosition;
                Debug.Log(nextPosition);

            }

            if (collider.CompareTag("Anchovy"))
            {
                Debug.Log("Trigger 2 Goto Home");

                m_objTrans2 = true;

                Item2Count--;
                Item2Text.text = Item2Count.ToString();
                collider.gameObject.GetComponent<ReadyToServe>().OnMouseDown();
                VRWrist.GetComponent<BoxCollider>().enabled = false;


                recipe = GameObject.FindGameObjectWithTag("Anchovy");
                recipe.GetComponent<MeshCollider>().enabled = false;

                m_xoffset = 0.00f;
                m_yoffset = 0.00f;
                m_zoffset = -0.10f;

                nextPosition = HomePosition;
                Debug.Log(nextPosition);

            }

            if (collider.CompareTag("RedPepper"))
            {
                Debug.Log("Trigger 3 Goto Home");

                m_objTrans3 = true;

                Item3Count--;
                Item3Text.text = Item3Count.ToString();
                collider.gameObject.GetComponent<ReadyToServe>().OnMouseDown();
                VRWrist.GetComponent<BoxCollider>().enabled = false;


                recipe = GameObject.FindGameObjectWithTag("RedPepper");
                recipe.GetComponent<MeshCollider>().enabled = false;

                m_xoffset = 0.00f;
                m_yoffset = 0.00f;
                m_zoffset = -0.10f;

                nextPosition = HomePosition;
                Debug.Log(nextPosition);

            }

            if (collider.CompareTag("GreenOlive"))
            {
                Debug.Log("Trigger 4 Goto Home");

                m_objTrans4 = true;

                Item4Count--;
                Item4Text.text = Item4Count.ToString();
                collider.gameObject.GetComponent<ReadyToServe>().OnMouseDown();
                VRWrist.GetComponent<BoxCollider>().enabled = false;


                recipe = GameObject.FindGameObjectWithTag("GreenOlive");
                recipe.GetComponent<MeshCollider>().enabled = false;

                m_xoffset = 0.00f;
                m_yoffset = 0.00f;
                m_zoffset = -0.10f;

                nextPosition = HomePosition;
                Debug.Log(nextPosition);

            }

            if (collider.CompareTag("BasilLeaf"))
            {
                Debug.Log("Trigger 5 Goto Home");

                m_objTrans5 = true;

                Item5Count--;
                Item5Text.text = Item5Count.ToString();
                collider.gameObject.GetComponent<ReadyToServe>().OnMouseDown();
                VRWrist.GetComponent<BoxCollider>().enabled = false;


                recipe = GameObject.FindGameObjectWithTag("BasilLeaf");
                recipe.GetComponent<MeshCollider>().enabled = false;

                m_xoffset = 0.00f;
                m_yoffset = 0.00f;
                m_zoffset = -0.10f;

                nextPosition = HomePosition;
                Debug.Log(nextPosition);

            }

           
            if (collider.CompareTag("OrderPlate"))
            {
                Debug.Log("Trigger Plate");

                if (m_objTrans1)
                {
                    m_ObjectTransfrom[0].position = m_ObjectTransfrom[5].position;
                    m_objTrans1 = false;
                    GotoHome = true;

                    recipe = GameObject.FindGameObjectWithTag("LemonSlice");
                    recipe.GetComponent<MeshCollider>().enabled = false;
                    //VRWrist.GetComponent<BoxCollider>().enabled = false;

                    m_xoffset = 0.00f;
                    m_yoffset = 0.00f;
                    m_zoffset = -0.10f;

                    Debug.Log("Goto Home");

                    


                }

                if (m_objTrans2)
                {
                    m_ObjectTransfrom[1].position = m_ObjectTransfrom[5].position;
                    m_objTrans2 = false;
                    GotoHome = true;

                    

                    recipe = GameObject.FindGameObjectWithTag("Anchovy");
                    recipe.GetComponent<MeshCollider>().enabled = false;
                    //VRWrist.GetComponent<BoxCollider>().enabled = false;

                    m_xoffset = 0.00f;
                    m_yoffset = 0.00f;
                    m_zoffset = -0.10f;

                    Debug.Log("Goto Home");

                    
                    

                }

                if (m_objTrans3)
                {
                    m_ObjectTransfrom[2].position = m_ObjectTransfrom[5].position;
                    m_objTrans3 = false;

                    GotoHome = true;

                    recipe = GameObject.FindGameObjectWithTag("RedPepper");
                    recipe.GetComponent<MeshCollider>().enabled = false;

                    m_xoffset = 0.00f;
                    m_yoffset = 0.00f;
                    m_zoffset = -0.10f;

                    Debug.Log("Goto Home");


                }

                if (m_objTrans4)
                {
                    m_ObjectTransfrom[3].position = m_ObjectTransfrom[5].position;
                    m_objTrans4 = false;

                    GotoHome = true;

                    recipe = GameObject.FindGameObjectWithTag("GreenOlive");
                    recipe.GetComponent<MeshCollider>().enabled = false;

                    m_xoffset = 0.00f;
                    m_yoffset = 0.00f;
                    m_zoffset = -0.10f;

                    Debug.Log("Goto Home");

                }

                if (m_objTrans5)
                {
                    m_ObjectTransfrom[4].position = m_ObjectTransfrom[5].position;
                    m_objTrans5 = false;

                    GotoHome = true;

                    recipe = GameObject.FindGameObjectWithTag("BasilLeaf");
                    recipe.GetComponent<MeshCollider>().enabled = false;

                    m_xoffset = 0.00f;
                    m_yoffset = 0.00f;
                    m_zoffset = -0.10f;

                    Debug.Log("Goto Home");

                    if (Item5Count == 0)
                    {
                        print("End Game");

                        float minutes = Time.realtimeSinceStartup / 60.0f;

                        var PlayerScore = FindObjectOfType<ServeOrder>();
                        if (PlayerScore != null)
                        {
                            PlayerScore.OrderCompleted();

                            gamescore = PlayerScore.GetGameScore();
                            gamesuccess = PlayerScore.GetGameSuccess();

                        }

                        //GameOverScript.Setup(gamescore, gamesuccess);
                        GameOverScript.Setup(gamescore, gamesuccess, minutes);

                        PlayerScore.SetGameScore(0);
                    }

                }

               
            }

        }


        private void OnTriggerStay(Collider collider)
        {
            if (collider.CompareTag("GameController"))
            {
                Debug.Log("Trigger GameController");
                //LogCollsiionEnter.text = "On Collision Enter: " + collider.name;

                VRCube = GameObject.FindWithTag("GameController");
                if (VRCube != null)
                {
                    VRCube.GetComponent<Renderer>().material.color = Color.green;
                    GotoTargetPlate = true;
                }
            }

        }

        private void OnTriggerExit(Collider collider)
        {

            if (collider.CompareTag("GameController"))
            {
                Debug.Log("Trigger GameController");
                //LogCollsiionEnter.text = "On Collision Enter: " + collider.name;

                VRCube = GameObject.FindWithTag("GameController");
                if (VRCube != null)
                {
                    VRCube.GetComponent<Renderer>().material.color = Color.red;
                }
            }

        }



    }
}
