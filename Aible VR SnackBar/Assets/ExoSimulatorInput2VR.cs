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
        public Vector4 nextRotation;

        public Vector3 nextTargetPosition;


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

        public GameObject toast;
        public GameObject TargetPlate;

        public GameObject playerarm;

        public GameOverScript GameOverScript;
        public bool EndGame = true;

        public Text LogScoreEnter;
        public Text LogSuccessEnter;

        public int Servescore = 0;
        public float Servesuccess = 0.0f;

        public int VRgamescore = 0;
        public double VRgameduration = 0;
        public int VRgameLA = 0;
        public int VRgameStatusCmd = 0;


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
        // Start is called before the first frame update

        bool m_Started;

        public LayerMask m_LayerMask;
        AudioSource audioSource;
        public float sphereRadius;
        public float hitDistance;


        public RaycastHit[] hits;

        float previousTimeScale = 1;
        public Text pauseLabel;
        public static bool isPaused;


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
            m_yoffset = 0.00f;
            m_zoffset = -0.10f;

            Servescore = 0;
            Servesuccess = 0.0f;



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
                if (string.Equals(msgCommand, "ExoData"))
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


            TargetPlate = GameObject.FindGameObjectWithTag("OrderPlate");
            TargetPlate.GetComponent<BoxCollider>().enabled = false;
            FirstTime = true;

            m_Started = true;
            audioSource = GetComponent<AudioSource>();
        }


        void CheckForVRArmUpdate()
        {

            vrPosition.x = -nextPosition.x + m_xoffset;
            vrPosition.y = nextPosition.y + m_yoffset;
            vrPosition.z = nextPosition.z + m_zoffset;

            Debug.Log(vrPosition);

            string msgvr = vrPosition.z.ToString() + ' ' + vrPosition.x.ToString() + ' ' + vrPosition.y.ToString() +
                            ' ' + vrRotation.x.ToString() + ' ' + vrRotation.y.ToString() + ' ' + vrRotation.z.ToString() + ' ' + vrRotation.w.ToString();

            if (!GameOverScript.isActiveAndEnabled)
            {
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



        //Right Arm
        private Quaternion FindRotationOffetShoulderCurrentVirtualToZeroRealQuaternionPUnity(Quaternion virtualQuaternion)
        {
            qRotation = new Quaternion((float)0.5000, (float)-0.5000, (float)-0.5000, (float)-0.5000);
            qRotationOffset = ConvertRightHandedToLeftHandedQuaternionSpace(qRotation);
            Quaternion qRotationTmp = Quaternion.identity;
            qRotationTmp = Quaternion.Inverse(qRotationOffset) * virtualQuaternion;
            return qRotationTmp; //working 
        }

        //RightArm
        private Quaternion FindRotationOffetElbowCurrentVirtualToZeroRealQuaternionPUnity(Quaternion virtualQuaternion)
        {
            qRotation = new Quaternion((float)0.0, (float)0.0, (float)0.0, (float)1.0);
            qRotationOffset = ConvertRightHandedToLeftHandedQuaternionShoulder(qRotation);
            Quaternion qRotationTmp = Quaternion.identity;
            qRotationTmp = Quaternion.Inverse(qRotationOffset) * virtualQuaternion;
            return qRotationTmp; //working 
        }

        //RightArm
        private Quaternion FindRotationOffetWristCurrentVirtualToZeroRealQuaternionPUnity(Quaternion virtualQuaternion)
        {
            qRotation = new Quaternion((float)0.5000, (float)-0.5000, (float)-0.5000, (float)0.5000);
            qRotationOffset = ConvertRightHandedToLeftHandedQuaternionElbow(qRotation);
            Quaternion qRotationTmp = Quaternion.identity;
            qRotationTmp = Quaternion.Inverse(qRotationOffset) * virtualQuaternion;
            return qRotationTmp; //working 
        }

        //RightArm
        private Quaternion ConvertRightHandedToLeftHandedQuaternionSpace(Quaternion rightHandedQuaternion)
        {
            return new Quaternion(-rightHandedQuaternion.y, rightHandedQuaternion.z, rightHandedQuaternion.x, -rightHandedQuaternion.w); //working 
        }
        private Quaternion ConvertRightHandedToLeftHandedQuaternionShoulder(Quaternion rightHandedQuaternion)
        {
            return new Quaternion(rightHandedQuaternion.z, -rightHandedQuaternion.x, rightHandedQuaternion.y, -rightHandedQuaternion.w); //working 
        }
        private Quaternion ConvertRightHandedToLeftHandedQuaternionElbow(Quaternion rightHandedQuaternion)
        {
            return new Quaternion(rightHandedQuaternion.z, -rightHandedQuaternion.x, rightHandedQuaternion.y, -rightHandedQuaternion.w); //working 
        }

        ////LeftArm
        //private Quaternion FindRotationOffetShoulderCurrentVirtualToZeroRealQuaternionPUnity(Quaternion virtualQuaternion)
        //{
        //    qRotation = new Quaternion((float)-0.5000, (float)0.5000, (float)0.5000, (float)0.5000);
        //    qRotationOffset = ConvertRightHandedToLeftHandedQuaternionSpace(qRotation);
        //    Quaternion qRotationTmp = Quaternion.identity;
        //    qRotationTmp = Quaternion.Inverse(qRotationOffset) * virtualQuaternion;
        //    return qRotationTmp; //working 
        //}




        ////LeftArm
        //private Quaternion FindRotationOffetElbowCurrentVirtualToZeroRealQuaternionPUnity(Quaternion virtualQuaternion)
        //{
        //    qRotation = new Quaternion((float)0.0, (float)0.0, (float)0.0, (float)1.0);
        //    qRotationOffset = ConvertRightHandedToLeftHandedQuaternionShoulder(qRotation);
        //    Quaternion qRotationTmp = Quaternion.identity;
        //    qRotationTmp = Quaternion.Inverse(qRotationOffset) * virtualQuaternion;
        //    return qRotationTmp; //working 
        //}



        ////LeftArm
        //private Quaternion FindRotationOffetWristCurrentVirtualToZeroRealQuaternionPUnity(Quaternion virtualQuaternion)
        //{
        //    qRotation = new Quaternion((float)0.5000, (float)-0.5000, (float)-0.5000, (float)0.5000);
        //    qRotationOffset = ConvertRightHandedToLeftHandedQuaternionElbow(qRotation);
        //    Quaternion qRotationTmp = Quaternion.identity;
        //    qRotationTmp = Quaternion.Inverse(qRotationOffset) * virtualQuaternion;
        //    return qRotationTmp; //working 
        //}

        ////LeftArm
        //private Quaternion ConvertRightHandedToLeftHandedQuaternionSpace(Quaternion rightHandedQuaternion)
        //{
        //    return new Quaternion(-rightHandedQuaternion.y, rightHandedQuaternion.z, rightHandedQuaternion.x, -rightHandedQuaternion.w); //working 
        //}
        //private Quaternion ConvertRightHandedToLeftHandedQuaternionShoulder(Quaternion rightHandedQuaternion)
        //{
        //    return new Quaternion(rightHandedQuaternion.y, rightHandedQuaternion.z, -rightHandedQuaternion.x, -rightHandedQuaternion.w); //working 
        //}
        //private Quaternion ConvertRightHandedToLeftHandedQuaternionElbow(Quaternion rightHandedQuaternion)
        //{
        //    return new Quaternion(rightHandedQuaternion.x, rightHandedQuaternion.z, rightHandedQuaternion.y, rightHandedQuaternion.w); //working 
        //}



        void SendVRPositionUpdate(string msgvr)
        {
#if DEGUG_INFOS
        Debug.Log(msgvr);
#endif
 
            this.client.SendMoreFrame("VRData").SendFrame(msgvr);

        }

        private void OnApplicationQuit()
        {

            NetMQConfig.Cleanup(false);
            Debug.Log("VRArmJointController exited.");
        }

        void TogglePause()
        {
            if(Time.timeScale > 0)
            {

                previousTimeScale = Time.timeScale;
                Time.timeScale = 0;
                AudioListener.pause = true;
                pauseLabel.enabled = true;

                isPaused = true;


            }
            else if(Time.timeScale == 0)
            {
                Time.timeScale = previousTimeScale;
                AudioListener.pause = false;
                pauseLabel.enabled = false;

                isPaused = false;

            }


        }

        // Update is called once per frame
        void Update()
        {

            deltaTime = Time.time - timeSinceLerpStart;

            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    TogglePause();
            //}

            if (deltaTime >= interval && Time.timeScale > 0)
            {

                timeSinceLerpStart = Time.time;

               

                if (m_objTrans1)
                {
                    m_ObjectTransfrom[0].position = m_JointTransfrom[3].position;

                    //Debug.Log("Toast1 Go Target Plate");

                    var dis = Vector3.Distance(m_JointTransfrom[3].position, HomePosition); // Calculating Distance
                    if (dis <= 0.09f) // checking if distance is less than required distance.
                    //if (GotoTargetPlate)
                    {
                        Debug.Log("Toast1 Go Target Plate");
                        TargetPlate.GetComponent<BoxCollider>().enabled = true;
                        VRWrist = GameObject.FindWithTag("Player");
                        VRWrist.GetComponent<BoxCollider>().enabled = true;

                        //m_xoffset = 0.00f;
                        //m_yoffset = 0.00f;
                        //m_zoffset = -0.10f;

                        nextPosition = m_ObjectTransfrom[12].position;
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
                        Debug.Log("Toast2 Go Target Plate");
                        TargetPlate.GetComponent<BoxCollider>().enabled = true;
                        VRWrist = GameObject.FindWithTag("Player");
                        VRWrist.GetComponent<BoxCollider>().enabled = true;

                        //m_xoffset = 0.00f;
                        //m_yoffset = 0.00f;
                        //m_zoffset = -0.10f;

                        nextPosition = m_ObjectTransfrom[12].position;
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
                        Debug.Log("Toast3 Go Target Plate");
                        TargetPlate.GetComponent<BoxCollider>().enabled = true;
                        VRWrist = GameObject.FindWithTag("Player");
                        VRWrist.GetComponent<BoxCollider>().enabled = true;

                        //m_xoffset = 0.00f;
                        //m_yoffset = 0.00f;
                        //m_zoffset = -0.10f;

                        nextPosition = m_ObjectTransfrom[12].position;
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
                        Debug.Log("Toast4 Go Target Plate");
                        TargetPlate.GetComponent<BoxCollider>().enabled = true;
                        VRWrist = GameObject.FindWithTag("Player");
                        VRWrist.GetComponent<BoxCollider>().enabled = true;

                        //m_xoffset = 0.00f;
                        //m_yoffset = 0.00f;
                        //m_zoffset = -0.10f;

                        nextPosition = m_ObjectTransfrom[12].position;
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
                        Debug.Log("Toast5 Go Target Plate");
                        TargetPlate.GetComponent<BoxCollider>().enabled = true;
                        VRWrist = GameObject.FindWithTag("Player");
                        VRWrist.GetComponent<BoxCollider>().enabled = true;

                        //m_xoffset = 0.00f;
                        //m_yoffset = 0.00f;
                        //m_zoffset = -0.10f;

                        nextPosition = m_ObjectTransfrom[12].position;
                        Debug.Log(nextPosition);
                        GotoTargetPlate = false;
                    }


                }

                if (m_objTrans6)
                {
                    m_ObjectTransfrom[5].position = m_JointTransfrom[3].position;

                    var dis = Vector3.Distance(m_JointTransfrom[3].position, HomePosition); // Calculating Distance
                    if (dis <= 0.09f) // checking if distance is less than required distance.
                    //if (GotoTargetPlate)
                    {
                        Debug.Log("Toast6 Go Target Plate");
                        TargetPlate.GetComponent<BoxCollider>().enabled = true;
                        VRWrist = GameObject.FindWithTag("Player");
                        VRWrist.GetComponent<BoxCollider>().enabled = true;

                        //m_xoffset = 0.00f;
                        //m_yoffset = 0.00f;
                        //m_zoffset = -0.10f;

                        nextPosition = m_ObjectTransfrom[12].position;
                        Debug.Log(nextPosition);
                        GotoTargetPlate = false;
                    }


                }

                if (m_objTrans7)
                {
                    m_ObjectTransfrom[6].position = m_JointTransfrom[3].position;

                    var dis = Vector3.Distance(m_JointTransfrom[3].position, HomePosition); // Calculating Distance
                    if (dis <= 0.09f) // checking if distance is less than required distance.
                    //if (GotoTargetPlate)
                    {
                        Debug.Log("Toast7 Go Target Plate");
                        TargetPlate.GetComponent<BoxCollider>().enabled = true;
                        VRWrist = GameObject.FindWithTag("Player");
                        VRWrist.GetComponent<BoxCollider>().enabled = true;

                        //m_xoffset = 0.00f;
                        //m_yoffset = 0.00f;
                        //m_zoffset = -0.10f;

                        nextPosition = m_ObjectTransfrom[12].position;
                        Debug.Log(nextPosition);
                        GotoTargetPlate = false;
                    }


                }

                if (m_objTrans8)
                {
                    m_ObjectTransfrom[7].position = m_JointTransfrom[3].position;

                    var dis = Vector3.Distance(m_JointTransfrom[3].position, HomePosition); // Calculating Distance
                    if (dis <= 0.09f) // checking if distance is less than required distance.
                    //if (GotoTargetPlate)
                    {
                        Debug.Log("Toast8 Go Target Plate");
                        TargetPlate.GetComponent<BoxCollider>().enabled = true;
                        VRWrist = GameObject.FindWithTag("Player");
                        VRWrist.GetComponent<BoxCollider>().enabled = true;

                        //m_xoffset = 0.00f;
                        //m_yoffset = 0.00f;
                        //m_zoffset = -0.10f;

                        nextPosition = m_ObjectTransfrom[12].position;
                        Debug.Log(nextPosition);
                        GotoTargetPlate = false;
                    }


                }

                if (m_objTrans9)
                {
                    m_ObjectTransfrom[8].position = m_JointTransfrom[3].position;

                    var dis = Vector3.Distance(m_JointTransfrom[3].position, HomePosition); // Calculating Distance
                    if (dis <= 0.09f) // checking if distance is less than required distance.
                    //if (GotoTargetPlate)
                    {
                        Debug.Log("Toast9 Go Target Plate");
                        TargetPlate.GetComponent<BoxCollider>().enabled = true;
                        VRWrist = GameObject.FindWithTag("Player");
                        VRWrist.GetComponent<BoxCollider>().enabled = true;

                        //m_xoffset = 0.00f;
                        //m_yoffset = 0.00f;
                        //m_zoffset = -0.10f;

                        nextPosition = m_ObjectTransfrom[12].position;
                        Debug.Log(nextPosition);
                        GotoTargetPlate = false;
                    }


                }

                if (m_objTrans10)
                {
                    m_ObjectTransfrom[9].position = m_JointTransfrom[3].position;

                    var dis = Vector3.Distance(m_JointTransfrom[3].position, HomePosition); // Calculating Distance
                    if (dis <= 0.09f) // checking if distance is less than required distance.
                    //if (GotoTargetPlate)
                    {
                        Debug.Log("Toast10 Go Target Plate");
                        TargetPlate.GetComponent<BoxCollider>().enabled = true;
                        VRWrist = GameObject.FindWithTag("Player");
                        VRWrist.GetComponent<BoxCollider>().enabled = true;

                        //m_xoffset = 0.00f;
                        //m_yoffset = 0.00f;
                        //m_zoffset = -0.10f;

                        nextPosition = m_ObjectTransfrom[12].position;
                        Debug.Log(nextPosition);
                        GotoTargetPlate = false;
                    }


                }

                if (m_objTrans11)
                {
                    m_ObjectTransfrom[10].position = m_JointTransfrom[3].position;

                    var dis = Vector3.Distance(m_JointTransfrom[3].position, HomePosition); // Calculating Distance
                    if (dis <= 0.09f) // checking if distance is less than required distance.
                    //if (GotoTargetPlate)
                    {
                        Debug.Log("Toast11 Go Target Plate");
                        TargetPlate.GetComponent<BoxCollider>().enabled = true;
                        VRWrist = GameObject.FindWithTag("Player");
                        VRWrist.GetComponent<BoxCollider>().enabled = true;

                        //m_xoffset = 0.00f;
                        //m_yoffset = 0.00f;
                        //m_zoffset = -0.10f;

                        nextPosition = m_ObjectTransfrom[12].position;
                        Debug.Log(nextPosition);
                        GotoTargetPlate = false;
                    }


                }

                if (m_objTrans12)
                {
                    m_ObjectTransfrom[11].position = m_JointTransfrom[3].position;

                    var dis = Vector3.Distance(m_JointTransfrom[3].position, HomePosition); // Calculating Distance
                    if (dis <= 0.09f) // checking if distance is less than required distance.
                    //if (GotoTargetPlate)
                    {
                        Debug.Log("Toast12 Go Target Plate");
                        TargetPlate.GetComponent<BoxCollider>().enabled = true;
                        VRWrist = GameObject.FindWithTag("Player");
                        VRWrist.GetComponent<BoxCollider>().enabled = true;

                        //m_xoffset = 0.00f;
                        //m_yoffset = 0.00f;
                        //m_zoffset = -0.10f;

                        nextPosition = m_ObjectTransfrom[12].position;
                        Debug.Log(nextPosition);
                        GotoTargetPlate = false;
                    }


                }

                if (FirstTime)
                {
                    Debug.Log("First Toast");

                    //m_xoffset = 0.05f;
                    //m_yoffset = 0.05f;
                    //m_zoffset = -0.05f;
                    toast = GameObject.FindGameObjectWithTag("Toast1");
                    if (toast != null)
                    {
                        nextPosition = m_ObjectTransfrom[0].position;
                        Debug.Log(nextPosition);
                    }

                    //nextPosition = HomePosition;
                }

                if (GotoHome)
                {
                    Debug.Log("Goto Home");
                    nextPosition = HomePosition;
                    GotoHome = false;

                    //var VRPlayer = GameObject.FindGameObjectWithTag("Player");
                    //VRPlayer.GetComponent<BoxCollider>().enabled = true;

                }

                VRCube = GameObject.FindWithTag("GameController");
                if (VRCube != null)
                {

                    HomePosition = VRCube.gameObject.transform.position;
                    Debug.Log(HomePosition);
                    //nextPosition = HomePosition;
                }


            

                VRCube = GameObject.FindWithTag("GameController");
                if (VRCube != null)
                {

                    //vrPosition = VRCube.gameObject.transform.position;

                   // vrRotation = VRCube.gameObject.transform.localRotation;



                    //LogCubeEnter.text = "Cube Position: " + vrCubePosition.ToString();
                    //LogShoulderEnter.text = "Shoulder Position: " + VRShoulder.gameObject.transform.position.ToString();
                    //LogElbowEnter.text = "Elbow Position: " + VRElbow.gameObject.transform.position.ToString();
                    //LogWristEnter.text = "Wrist Position: " + VRWrist.gameObject.transform.position.ToString();




                }

                Invoke("CheckForVRArmUpdate", 0.1f);

                var PlayerScore = FindObjectOfType<ServeOrder>();
                if (PlayerScore != null)
                {
                    Servescore = PlayerScore.GetServeScore();
                    Servesuccess = PlayerScore.GetServeSuccess();
                }

            }


            //LogScoreEnter.text = "Points: " + Servescore.ToString();
            //LogSuccessEnter.text = "Success Rate: " + Servesuccess.ToString("F2") + "%";
        }

        private void OnTriggerEnter(Collider collider)
        {
            //LogCollsiionEnter.text = "Hand Collision With: " + collider.name;

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
            

            if (collider.CompareTag("Toast1"))
            {
                Debug.Log("Trigger Toast1 Goto Home");
                m_objTrans1 = true;
                FirstTime = false;
    
                toast = GameObject.FindGameObjectWithTag("Toast1");
  
                m_xoffset = 0.00f;
                m_yoffset = 0.00f;
                m_zoffset = -0.10f;

                nextPosition = HomePosition;
                Debug.Log(nextPosition);

            }

            if (collider.CompareTag("Toast2"))
            {
                Debug.Log("Trigger Toast2 Goto Home");

                m_objTrans2 = true;
   
                toast = GameObject.FindGameObjectWithTag("Toast2");

                m_xoffset = 0.00f;
                m_yoffset = 0.00f;
                m_zoffset = -0.10f;

                nextPosition = HomePosition;
                Debug.Log(nextPosition);

            }

            if (collider.CompareTag("Toast3"))
            {
                Debug.Log("Trigger Toast3 Goto Home");

                m_objTrans3 = true;
               
                toast = GameObject.FindGameObjectWithTag("Toast3");

                m_xoffset = 0.00f;
                m_yoffset = 0.00f;
                m_zoffset = -0.10f;

                nextPosition = HomePosition;
                Debug.Log(nextPosition);

            }

            if (collider.CompareTag("Toast4"))
            {
                Debug.Log("Trigger Toast4 Goto Home");

                m_objTrans4 = true;
               
                toast = GameObject.FindGameObjectWithTag("Toast4");

                m_xoffset = 0.00f;
                m_yoffset = 0.00f;
                m_zoffset = -0.10f;

                nextPosition = HomePosition;
                Debug.Log(nextPosition);

            }

            if (collider.CompareTag("Toast5"))
            {
                Debug.Log("Trigger Toast5 Goto Home");

                m_objTrans5 = true;

                toast = GameObject.FindGameObjectWithTag("Toast5");

                m_xoffset = 0.00f;
                m_yoffset = 0.00f;
                m_zoffset = -0.10f;

                nextPosition = HomePosition;
                Debug.Log(nextPosition);

            }

            if (collider.CompareTag("Toast6"))
            {
                Debug.Log("Trigger Toast6 Goto Home");

                m_objTrans6 = true;

                toast = GameObject.FindGameObjectWithTag("Toast6");

                m_xoffset = 0.00f;
                m_yoffset = 0.00f;
                m_zoffset = -0.10f;

                nextPosition = HomePosition;
                Debug.Log(nextPosition);

            }

            if (collider.CompareTag("Toast7"))
            {
                Debug.Log("Trigger Toast7 Goto Home");

                m_objTrans7 = true;

                toast = GameObject.FindGameObjectWithTag("Toast7");

                m_xoffset = 0.00f;
                m_yoffset = 0.00f;
                m_zoffset = -0.10f;

                nextPosition = HomePosition;
                Debug.Log(nextPosition);

            }

            if (collider.CompareTag("Toast8"))
            {
                Debug.Log("Trigger Toast8 Goto Home");

                m_objTrans8 = true;

                toast = GameObject.FindGameObjectWithTag("Toast8");

                m_xoffset = 0.00f;
                m_yoffset = 0.00f;
                m_zoffset = -0.10f;

                nextPosition = HomePosition;
                Debug.Log(nextPosition);

            }

            if (collider.CompareTag("Toast9"))
            {
                Debug.Log("Trigger Toast9 Goto Home");

                m_objTrans9 = true;

                toast = GameObject.FindGameObjectWithTag("Toast9");

                m_xoffset = 0.00f;
                m_yoffset = 0.00f;
                m_zoffset = -0.10f;

                nextPosition = HomePosition;
                Debug.Log(nextPosition);

            }

            if (collider.CompareTag("Toast10"))
            {
                Debug.Log("Trigger Toast10 Goto Home");

                m_objTrans10 = true;

                toast = GameObject.FindGameObjectWithTag("Toast10");

                m_xoffset = 0.00f;
                m_yoffset = 0.00f;
                m_zoffset = -0.10f;

                nextPosition = HomePosition;
                Debug.Log(nextPosition);

            }

            if (collider.CompareTag("Toast11"))
            {
                Debug.Log("Trigger Toast11 Goto Home");

                m_objTrans11 = true;

                toast = GameObject.FindGameObjectWithTag("Toast11");

                m_xoffset = 0.00f;
                m_yoffset = 0.00f;
                m_zoffset = -0.10f;

                nextPosition = HomePosition;
                Debug.Log(nextPosition);

            }

            if (collider.CompareTag("Toast12"))
            {
                Debug.Log("Trigger Toast12 Goto Home");

                m_objTrans12 = true;

                toast = GameObject.FindGameObjectWithTag("Toast12");

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
                    m_ObjectTransfrom[0].position = m_ObjectTransfrom[12].position;
                    m_objTrans1 = false;
                    GotoHome = true;

                    toast = GameObject.FindGameObjectWithTag("Toast1");
                    toast.GetComponent<BoxCollider>().enabled = false;
                   
                    m_xoffset = 0.00f;
                    m_yoffset = 0.00f;
                    m_zoffset = -0.10f;

                    Debug.Log("Goto Home");

                    



                }

                if (m_objTrans2)
                {
                    m_ObjectTransfrom[1].position = m_ObjectTransfrom[12].position;
                    m_objTrans2 = false;
                    GotoHome = true;

                    toast = GameObject.FindGameObjectWithTag("Toast2");
                    toast.GetComponent<BoxCollider>().enabled = false;

                    m_xoffset = 0.00f;
                    m_yoffset = 0.00f;
                    m_zoffset = -0.10f;

                    Debug.Log("Goto Home");

                }

                if (m_objTrans3)
                {
                    m_ObjectTransfrom[2].position = m_ObjectTransfrom[12].position;
                    m_objTrans3 = false;

                    GotoHome = true;

                    toast = GameObject.FindGameObjectWithTag("Toast3");
                    toast.GetComponent<BoxCollider>().enabled = false;

                    m_xoffset = 0.00f;
                    m_yoffset = 0.00f;
                    m_zoffset = -0.10f;

                    Debug.Log("Goto Home");

     
                }

                if (m_objTrans4)
                {
                    m_ObjectTransfrom[3].position = m_ObjectTransfrom[12].position;
                    m_objTrans4 = false;

                    GotoHome = true;

                    toast = GameObject.FindGameObjectWithTag("Toast4");
                    toast.GetComponent<BoxCollider>().enabled = false;

                    m_xoffset = 0.00f;
                    m_yoffset = 0.00f;
                    m_zoffset = -0.10f;

                    Debug.Log("Goto Home");

                }

                if (m_objTrans5)
                {
                    m_ObjectTransfrom[4].position = m_ObjectTransfrom[12].position;
                    m_objTrans5 = false;

                    GotoHome = true;

                    toast = GameObject.FindGameObjectWithTag("Toast5");
                    toast.GetComponent<BoxCollider>().enabled = false;

                    m_xoffset = 0.00f;
                    m_yoffset = 0.00f;
                    m_zoffset = -0.10f;

                    Debug.Log("Goto Home");

                }

                if (m_objTrans6)
                {
                    m_ObjectTransfrom[5].position = m_ObjectTransfrom[12].position;
                    m_objTrans6 = false;

                    GotoHome = true;

                    toast = GameObject.FindGameObjectWithTag("Toast6");
                    toast.GetComponent<BoxCollider>().enabled = false;

                    m_xoffset = 0.00f;
                    m_yoffset = 0.00f;
                    m_zoffset = -0.10f;

                    Debug.Log("Goto Home");

                }

                if (m_objTrans7)
                {
                    m_ObjectTransfrom[6].position = m_ObjectTransfrom[12].position;
                    m_objTrans7 = false;

                    GotoHome = true;

                    toast = GameObject.FindGameObjectWithTag("Toast7");
                    toast.GetComponent<BoxCollider>().enabled = false;

                    m_xoffset = 0.00f;
                    m_yoffset = 0.00f;
                    m_zoffset = -0.10f;

                    Debug.Log("Goto Home");

                }

                if (m_objTrans8)
                {
                    m_ObjectTransfrom[7].position = m_ObjectTransfrom[12].position;
                    m_objTrans8 = false;

                    GotoHome = true;

                    toast = GameObject.FindGameObjectWithTag("Toast8");
                    toast.GetComponent<BoxCollider>().enabled = false;

                    m_xoffset = 0.00f;
                    m_yoffset = 0.00f;
                    m_zoffset = -0.10f;

                    Debug.Log("Goto Home");

                }

                if (m_objTrans9)
                {
                    m_ObjectTransfrom[8].position = m_ObjectTransfrom[12].position;
                    m_objTrans9 = false;

                    GotoHome = true;

                    toast = GameObject.FindGameObjectWithTag("Toast9");
                    toast.GetComponent<BoxCollider>().enabled = false;

                    m_xoffset = 0.00f;
                    m_yoffset = 0.00f;
                    m_zoffset = -0.10f;

                    Debug.Log("Goto Home");

                }

                if (m_objTrans10)
                {
                    m_ObjectTransfrom[9].position = m_ObjectTransfrom[12].position;
                    m_objTrans10 = false;

                    GotoHome = true;

                    toast = GameObject.FindGameObjectWithTag("Toast10");
                    toast.GetComponent<BoxCollider>().enabled = false;

                    m_xoffset = 0.00f;
                    m_yoffset = 0.00f;
                    m_zoffset = -0.10f;

                    Debug.Log("Goto Home");

                }

                if (m_objTrans11)
                {
                    m_ObjectTransfrom[10].position = m_ObjectTransfrom[12].position;
                    m_objTrans11 = false;

                    GotoHome = true;

                    toast = GameObject.FindGameObjectWithTag("Toast11");
                    toast.GetComponent<BoxCollider>().enabled = false;

                    m_xoffset = 0.00f;
                    m_yoffset = 0.00f;
                    m_zoffset = -0.10f;

                    Debug.Log("Goto Home");

                }

                if (m_objTrans12)
                {
                    m_ObjectTransfrom[11].position = m_ObjectTransfrom[12].position;
                    m_objTrans12 = false;

                    GotoHome = true;

                    toast = GameObject.FindGameObjectWithTag("Toast12");
                    toast.GetComponent<BoxCollider>().enabled = false;

                    m_xoffset = 0.00f;
                    m_yoffset = 0.00f;
                    m_zoffset = -0.10f;

                    Debug.Log("Goto Home");

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
