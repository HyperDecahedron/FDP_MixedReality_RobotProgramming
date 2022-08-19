using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using QRTracking;
using Microsoft.MixedReality.QR;
using Microsoft.MixedReality.OpenXR;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityGLTF;
using RK;

public class myMainMenu : MonoBehaviour
{
    #region Serialize fields and variables
    [SerializeField] 
    private GameObject InfoCubeText = null;         // text to show in the InfoCube
    [SerializeField]
    private GameObject InfoCube;
    [SerializeField]
    private GameObject myMainMenuObject;            // main menu object
    [SerializeField]
    private QRCodesManager qrCodesManager;          // QR codes manager
    [SerializeField]
    private Transform Pointer_Origin = null;        // yellow pointer for the origin
    [SerializeField]
    private Transform Table_Workobject = null;      // left workobject of the table that contains the targets.
    [SerializeField]
    private Material Target_Material;               // normal material in blue
    [SerializeField]
    private Material Highlight_Material;            // yellow material to change when highlighting
    [SerializeField]
    private Material GunActive_Material;            // pink material to change when the weld gun is active
    [SerializeField]
    private RobotMechanismBuilder robotBuilder;     // script to build the robot
    [SerializeField]
    private Transform Station;                      // station containing the robot and targets
    [SerializeField]
    private Transform Station_QR;                   // station QR position
    [SerializeField]
    private GameObject Cube_QR;
    [SerializeField]
    private GameObject EH_QR;
    [SerializeField]
    private GameObject JS_QR;
    [SerializeField]
    private GameObject PieceB_QR;
    [SerializeField]
    private GameObject Cube_offset;
    [SerializeField]
    private GameObject EH_offset;
    [SerializeField]
    private GameObject JS_offset;
    [SerializeField]
    private GameObject PieceB_offset;
    [SerializeField]
    private GameObject robot;
    [SerializeField]
    private GameObject tool;
    [SerializeField]
    private GameObject table;
    [SerializeField]
    private GameObject numberPad;

    // QR CODES ********************************************************************************
    private bool _QRTrackingEnabled = false;    // if the QR tracking is enabled or not
    private bool _QR_PointerOriginFound = false;                // si se ha encontrado el QR del origen del Workobject (Amarillo)
    private bool _QR_PointerXFound = false;                     // si se ha encontrado el QR del pointer X (Rojo)
    private bool _QR_PointerXYFound = false;                    // si se ha encontrado el QR del pointer XY (Verde)
    private System.Collections.Generic.IList<Microsoft.MixedReality.QR.QRCode> qrCodesList = null;
    public SpatialGraphNode nodeWeldPiece; 

    // TCP/IP COMMUNICATION*********************************************************************
    [SerializeField]
    private string IP_ADDRESS;                   // IP Adress of the PC
    private const int LOCAL_PORT = 3000;         // Local Port of the Local IP Adress
    private const int MAX_NUM_TARGETS = 60;      // maximum number of targets
    private string message_received;             // message received from RS
    private bool updateRobPos = false;           // update position of the virtual robot during the simulation
    private TcpClient client2;
    private NetworkStream nwStream2;
    private bool stopRobot = false;

    // CHECK REACHABILITY **********************************************************************
    private bool reachability_problem = false;   // if there is a reachability problem in a specific target

    // VIEW ROBOT AT TARGET **********************************************************************
    private int target_pos = 1; // last target of "Robot at target"
    private bool targets_sent = false; // if the targets were sent or not
    private bool updateTool = true; // update the tool in the loop update

    // UPDATE TARGET POS FROM TOOL POS ***********************************************************
    private GameObject Tool;
    private GameObject Tool_Components;
    private GameObject Target;

    // MOVE TOOL ********************************************************************************
    private bool enable_manipulation_tool = false;
    private bool components_added = false;  // components added from Tool_Components to Tool
    private BoundsControl boundControl; // component
    private ObjectManipulator objManipulator; // component

    // OTHER ********************************************************************************
    private GameObject GLTFNode;
    private GameObject Link;
    private int i = 1;
    public bool EnabledInfoCube = false;

    #endregion

    #region Start and Update
    // Start is called before the first frame update
    void Start()
    {
        // search for residual Gltf nodes
        //do
        //{
        //    GLTFNode = GameObject.Find("GLTFNode" + i.ToString());
        //    i++;
        //} while (GLTFNode == null && i < 27);
        //// delete
        //if (GLTFNode != null)
        //{
        //    Destroy(GLTFNode);
        //}
        //// Search for residual links
        //i = 1;
        //do
        //{
        //    Link = GameObject.Find("Link" + i.ToString());
        //    i++;
        //} while (Link == null && i < 27);
        //// delete
        //if (Link != null)
        //{
        //    Destroy(Link);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (updateTool)
            robotBuilder.UpdateToolPos();

        if (updateRobPos)
        {
            if (!stopRobot)
            {
                SendMessageToRS_Op("up"); // random message, will not be used
                if (message_received != "stop")
                    MoveRobotToTarget(message_received);
                else
                {
                    updateRobPos = false; // stops updating its position
                    updateTool = false;
                    client2.Close();
                }
            }
            else
            {
                SendMessageToRS_Op("Stop");
                updateRobPos = false; 
                updateTool = false;
                stopRobot = false;
            }
            
        }
    }

    public string ipValue;
    public void GetIP()
    {
        numberPad.SetActive(true);
        numberPad.GetComponent<NumberPad>().ip = true;
        numberPad.transform.position = myMainMenuObject.transform.position + new Vector3(0.4f, 0f, -0.2f);
        numberPad.GetComponent<RadialView>().enabled = true;
        numberPad.GetComponent<FollowMeToggle>().enabled = true;

    }
    public void UpdateIP(string ipValue)
    {
        numberPad.SetActive(false);
        IP_ADDRESS = ipValue;
        numberPad.GetComponent<NumberPad>().ip = false;
    }

    #endregion

    #region MAIN MENU BUTTONS


    public void ToggleQRCodeTracking()
    {
        _QRTrackingEnabled = !_QRTrackingEnabled;                   // toggle del estado del tracking
        if (_QRTrackingEnabled)                                          // antes no estaba activado y al pulsar el botón se activa
        {
            if (qrCodesManager != null)                                     // este es el manager de QR que hemos puesto en las variables públicas,
                                                                            // comprueba que existe y que lo hemos puesto
            {
                qrCodesManager.QRCodeAdded += QrCodesManager_QRCodeAdded;   // cuando se ejecuta la función de que se ha añadido un QR, entonces
                                                                            // se llama a la función de QRCodeAdded, que está más abajo
                qrCodesManager.StartQRTracking();                           // empieza el tracking
                PrintInfoText("Enabled QR_Code Tracking");                      // por el DebugText
            }
        }
        else
        {
            qrCodesManager.QRCodeAdded -= QrCodesManager_QRCodeAdded;
            UpdateStationBasedOnQR_Codes();
            PrintInfoText("Disabled QR_Code Tracking");
        }
    }

    private void UpdateStationBasedOnQR_Codes()
    {
        if (qrCodesManager != null)
        {
            // set active the robot, tool and table
            robot.SetActive(true);
            tool.SetActive(true);   
            //table.SetActive(true);

            qrCodesList = qrCodesManager.GetList();
            PrintInfoText("Found: " + qrCodesList.Count.ToString() + " QR Codes ");
            Microsoft.MixedReality.QR.QRCode code = TryGetQrCode(qrCodesList, "Calib"); // QR del Calib Origin.

            if ((code != null) && (Station != null))
            {
                SpatialGraphNode node = SpatialGraphNode.FromStaticNodeId(code.SpatialGraphNodeId);
                if (node != null && node.TryLocate(FrameTime.OnUpdate, out Pose pose))
                {
                    PrintInfoText("Placed: Station");
                    Matrix4x4 m = Matrix4x4.LookAt(pose.position, pose.position + Vector3.up, pose.right);
                    Station_QR.transform.SetPositionAndRotation(pose.position, m.rotation); //set to last qr code location
                }
            }
            else
                PrintInfoText("Could not place station");

            code = TryGetQrCode(qrCodesList, "X", true);
            //code = TryGetQrCode(qrCodesList, "TestPart_X"); // QR TestPart_X

            if ((Station != null) && (code != null))
            {
                SpatialGraphNode node = SpatialGraphNode.FromStaticNodeId(code.SpatialGraphNodeId);
                if (node != null && node.TryLocate(FrameTime.OnUpdate, out Pose pose))
                {
                    PrintInfoText("Placed: Workobject");
                    Matrix4x4 m = Matrix4x4.LookAt(pose.position, pose.position + Vector3.up, pose.right);
                    Table_Workobject.transform.SetPositionAndRotation(pose.position, m.rotation); //set to last qr code location
                    // send message to RS to update the position of the workobject in robot studio
                    //float Xpos = Table_Workobject.transform.localPosition.x * 1000;
                    //float Ypos = Table_Workobject.transform.localPosition.y * (-1000);
                    //float Zpos = Table_Workobject.transform.localPosition.z * 1000;

                    //string strPosition = "CW" + Xpos.ToString() + "|" + Ypos.ToString() + "|" + Zpos.ToString() + "|";
                    //SendMessageToRS(strPosition);
                }
                else
                    PrintInfoText("Could not place Workobject");

            }
            else
                PrintInfoText("Workobject code or station not found");

            // send message to RS to update the position of the workobject in robot studio
            float Xpos = Table_Workobject.transform.localPosition.x * 1000;
            float Ypos = Table_Workobject.transform.localPosition.y * (-1000);
            float Zpos = Table_Workobject.transform.localPosition.z * 1000;
            Quaternion rot = Table_Workobject.transform.localRotation;
            rot.y = rot.y * (-1);
            rot.z = rot.z * (-1);

            string strPosition = "CW" + Xpos.ToString() + "|" + Ypos.ToString() + "|" + Zpos.ToString() + "|" 
                + rot.w.ToString() + "|" + rot.x.ToString() + "|" + rot.y.ToString() + "|" + rot.z.ToString() + "|"; ;
            SendMessageToRS(strPosition);

            code = TryGetQrCode(qrCodesList, "XY", true);
            if (code != null)
            {
                nodeWeldPiece = SpatialGraphNode.FromStaticNodeId(code.SpatialGraphNodeId);
            }
            else
            {
                PrintInfoText("Code XY for Weld Pice is null");
            }

            OnClickSelectWeldPiece();

            //_QR_PointerOriginFound = TryPlacePointer(qrCodesList, "Origin", Pointer_Origin); // busca todos estos QR
            //_QR_PointerXFound = TryPlacePointer(qrCodesList, "X", Pointer_X);
            //_QR_PointerXYFound = TryPlacePointer(qrCodesList, "XY", Pointer_XY);

            qrCodesManager.StopQRTracking();
            PrintInfoText("Stopped QR code Tracking");
        }
    }

    public void OnClickSelectWeldPiece()
    {
        if (qrCodesManager != null && qrCodesList != null )
        {
            //System.Collections.Generic.IList<Microsoft.MixedReality.QR.QRCode> qrCodesList = qrCodesManager.GetList();
            PrintInfoText("Found: " + qrCodesList.Count.ToString() + " QR Codes ");
            Microsoft.MixedReality.QR.QRCode code = TryGetQrCode(qrCodesList, "XY", true);

            if (code != null)
            {
                // Get the current weld piece
                GameObject[] arrWeldPieces = GameObject.FindGameObjectsWithTag("WeldPiece"); // the component 0 is the welding piece

                if (arrWeldPieces.Length != 0)
                {
                    //SpatialGraphNode node = SpatialGraphNode.FromStaticNodeId(code.SpatialGraphNodeId);
                    if (nodeWeldPiece != null && nodeWeldPiece.TryLocate(FrameTime.OnUpdate, out Pose pose))
                    {
                        Matrix4x4 m = Matrix4x4.LookAt(pose.position, pose.position + Vector3.up, pose.right);
                        if (arrWeldPieces[0].name == "Cube")
                        {
                            Cube_QR.transform.SetPositionAndRotation(pose.position, pose.rotation); //set to last qr code location
                            arrWeldPieces[0].transform.SetPositionAndRotation(Cube_offset.transform.position, Cube_offset.transform.rotation);
                        }
                        else if (arrWeldPieces[0].name == "EH197202_02")
                        {
                            //EH_QR.transform.SetPositionAndRotation(pose.position, m.rotation); //set to last qr code location
                            EH_QR.transform.SetPositionAndRotation(pose.position, pose.rotation);
                            arrWeldPieces[0].transform.SetPositionAndRotation(EH_offset.transform.position, EH_offset.transform.rotation);
                        }
                        else if (arrWeldPieces[0].name == "JS016014_01")
                        {
                            JS_QR.transform.SetPositionAndRotation(pose.position, pose.rotation);
                            arrWeldPieces[0].transform.SetPositionAndRotation(JS_offset.transform.position, JS_offset.transform.rotation);
                        }
                        else if (arrWeldPieces[0].name == "Piece B")
                        {
                            PieceB_QR.transform.SetPositionAndRotation(pose.position, pose.rotation);
                            arrWeldPieces[0].transform.SetPositionAndRotation(PieceB_offset.transform.position, PieceB_offset.transform.rotation);
                        }

                        PrintInfoText("Placed: " + arrWeldPieces[0].name);

                    }
                    else if (nodeWeldPiece == null)
                        PrintInfoText("NodeWeldPiece is null");
                    else
                        PrintInfoText("Could not place " + arrWeldPieces[0].name);
                }
                else
                    PrintInfoText("No objects with tag WeldPiece");
            }
            else
                PrintInfoText("Welding piece code not found");
        }
        else
        {
            PrintInfoText("qr Code List null");
        }
    }

    public void OnToggleInfoCube() {
        if (!EnabledInfoCube)
        {
            InfoCube.SetActive(true);
            InfoCube.transform.position = myMainMenuObject.transform.position + new Vector3(0.4f, 0f, 0.1f);
            InfoCube.transform.rotation = myMainMenuObject.transform.rotation;
        }
        else
        {
            InfoCube.SetActive(false);
        }

        EnabledInfoCube = !EnabledInfoCube;
    
    }

    #region QR Codes Managing

    private bool TryPlacePointer(System.Collections.Generic.IList<Microsoft.MixedReality.QR.QRCode> list, string PointerNameEnd, Transform Pointer)
    {
        bool result = false;
        Microsoft.MixedReality.QR.QRCode code = TryGetQrCode(list, PointerNameEnd, true);
        if ((code != null))
        {
            SpatialGraphNode node = SpatialGraphNode.FromStaticNodeId(code.SpatialGraphNodeId);
            if (node != null && node.TryLocate(FrameTime.OnUpdate, out Pose pose))
            {
                if (Pointer != null)
                {
                    PrintInfoText("Placed: " + Pointer.name);
                    Pointer.SetPositionAndRotation(pose.position, pose.rotation); //set to last qr code location
                    result = true;
                }
            }
        }
        return result;
    }

    private Microsoft.MixedReality.QR.QRCode TryGetQrCode(System.Collections.Generic.IList<Microsoft.MixedReality.QR.QRCode> list, string TestString, bool EndsWith = false)
    {
        DateTimeOffset timeStampLast = new DateTimeOffset();
        Microsoft.MixedReality.QR.QRCode result = null;
        foreach (Microsoft.MixedReality.QR.QRCode code in list)
        {
            string[] stSplit = code.Data.Split('_');
            string stringPart = code.Data.Trim();
            if (stSplit.Length > 1)
                stringPart = EndsWith ? stSplit[stSplit.Length - 1].Trim() : stSplit[0].Trim();
            if (TestString.ToLower() == stringPart.ToLower())
            {
                //  PrintText("timeStampLast " + timeStampLast.Ticks.ToString());
                DateTimeOffset timeStamp = code.LastDetectedTime;
                //  PrintText("timeStamp " + timeStamp.Ticks.ToString());
                if (timeStamp.Ticks > timeStampLast.Ticks)
                {
                    result = code;
                    timeStampLast = timeStamp;
                }
            }
        }
        return result;
    }

    private Microsoft.MixedReality.QR.QRCode TryGetQrCode(System.Collections.Generic.IList<Microsoft.MixedReality.QR.QRCode> list, string String1, string String2)
    {
        DateTimeOffset timeStampLast = new DateTimeOffset();
        Microsoft.MixedReality.QR.QRCode result = null;
        foreach (Microsoft.MixedReality.QR.QRCode code in list)
        {
            string[] stSplit = code.Data.Split('_');
            if ((stSplit.Length > 1) && (stSplit[0].ToLower() == String1.ToLower()) && (stSplit[1].ToLower() == String2.ToLower()))
            {
                DateTimeOffset timeStamp = code.LastDetectedTime;
                if (timeStamp.Ticks > timeStampLast.Ticks)
                {
                    result = code;
                    timeStampLast = timeStamp;
                    PrintInfoText("Found " + code.Data);
                }
            }
        }
        return result;
    }

    private void QrCodesManager_QRCodeAdded(object sender, QRCodeEventArgs<Microsoft.MixedReality.QR.QRCode> e)
    {
        // Si se ha añadido un QR y no está puesta la station origin, pone el station origin ahí. 
        //if ((Station != null) && (e.Data.Data.StartsWith("Calib_Origin")))
        //{
        //    SpatialGraphNode node = SpatialGraphNode.FromStaticNodeId(e.Data.SpatialGraphNodeId);
        //    if (node != null && node.TryLocate(FrameTime.OnUpdate, out Pose pose))
        //    {
        //        PrintInfoText("Placed: " + Station.name);
        //        Matrix4x4 m = Matrix4x4.LookAt(pose.position, pose.position + Vector3.up, pose.right);
        //        pose.position.x -= 0.234f; // m
        //        pose.position.y -= 0.085f; // m
        //        Station.transform.SetPositionAndRotation(pose.position, m.rotation); //set to last qr code location
        //    }
        //    else
        //        PrintInfoText("Could not place Station");
        //}
        //else if ((Station != null) && (e.Data.Data.StartsWith("TestPart_X")))
        //{
        //    SpatialGraphNode node = SpatialGraphNode.FromStaticNodeId(e.Data.SpatialGraphNodeId);
        //    if (node != null && node.TryLocate(FrameTime.OnUpdate, out Pose pose))
        //    {
        //        PrintInfoText("Placed: Workobject");
        //        Matrix4x4 m = Matrix4x4.LookAt(pose.position, pose.position + Vector3.up, pose.right);
        //        Table_Workobject.transform.SetPositionAndRotation(pose.position, m.rotation); //set to last qr code location
        //    }
        //    else
        //        PrintInfoText("Could not place Workobject");
        //}
    }

    /*public void ToggleQRCodeTracking()
    {
        _QRTrackingEnabled = !_QRTrackingEnabled; // toggle the state of the button
        if (_QRTrackingEnabled) // Enabled
        {
            if(qrCodesManager != null) // to ensure that there is a qr manager put
            {
                qrCodesManager.QRCodeAdded += QrCodesManager_QRCodeAdded;   // cuando se ejecuta la función de que se ha añadido un QR, entonces
                qrCodesManager.StartQRTracking();
                PrintInfoText("Enabled QR Tracking");
            }
        }
        else // Not enabled
        {
          qrCodesManager.QRCodeAdded -= QrCodesManager_QRCodeAdded;
          qrCodesManager.StopQRTracking();
          UpdateStationBasedOnQR_Codes();
          PrintInfoText("Disabled QR_Code Tracking");
        }

    }

    private void QrCodesManager_QRCodeAdded(object sender, QRCodeEventArgs<Microsoft.MixedReality.QR.QRCode> e)
    {
        // Si se ha añadido un QR y no está puesta la station origin, pone el station origin ahí. 
        if ((Station != null) && (e.Data.Data.StartsWith("Calib_Origin")))
        {
            SpatialGraphNode node = SpatialGraphNode.FromStaticNodeId(e.Data.SpatialGraphNodeId);
            if (node != null && node.TryLocate(FrameTime.OnUpdate, out Pose pose))
            {
                PrintInfoText("Placed: " + Station.name);
                Matrix4x4 m = Matrix4x4.LookAt(pose.position, pose.position + Vector3.up, pose.right);
                Station.transform.SetPositionAndRotation(pose.position, m.rotation); //set to last qr code location
            }
        }
    }

    private void UpdateStationBasedOnQR_Codes() // updates the position of the pointer_origin based in the QR Calib_Origin
    {
        if (qrCodesManager != null)
        {
            System.Collections.Generic.IList<Microsoft.MixedReality.QR.QRCode> qrCodesList = qrCodesManager.GetList(); // to get the list of QR Codes
            PrintInfoText("Found: " + qrCodesList.Count.ToString() + " QR Codes "); // print how many QR codes were found
            // Calib_Origin
            Microsoft.MixedReality.QR.QRCode code = TryGetQrCode(qrCodesList, "Calib_Origin"); // try to get that QR code in the list
            if ((code != null) && (Pointer_Origin != null))
            {
                SpatialGraphNode node = SpatialGraphNode.FromStaticNodeId(code.SpatialGraphNodeId); // extract the position of the QR
                if (node != null && node.TryLocate(FrameTime.OnUpdate, out Pose pose))
                {
                     PrintInfoText("Placed: " + Pointer_Origin.name);
                     Pointer_Origin.SetPositionAndRotation(pose.position, pose.rotation); // update the position to that QR 
                     Station.SetPositionAndRotation(pose.position, pose.rotation); // FOR DEBUGGING
                }
            }
            else if(code==null)
            {
                PrintInfoText("Calib_Origin not found");
            }
            else
            {
                PrintInfoText("Pointer_Origin is null");
            }

            qrCodesManager.StopQRTracking();
            PrintInfoText("Stopped QR code Tracking");
        }
    }

    private Microsoft.MixedReality.QR.QRCode TryGetQrCode(System.Collections.Generic.IList<Microsoft.MixedReality.QR.QRCode> list, string TestString, bool EndsWith = false)
    {
        DateTimeOffset timeStampLast = new DateTimeOffset();
        Microsoft.MixedReality.QR.QRCode result = null;
        foreach (Microsoft.MixedReality.QR.QRCode code in list)
        {
            string[] stSplit = code.Data.Split('_');
            string stringPart = code.Data.Trim();
            if (stSplit.Length > 1)
                stringPart = EndsWith ? stSplit[stSplit.Length - 1].Trim() : stSplit[0].Trim();
            if (TestString.ToLower() == stringPart.ToLower())
            {
                DateTimeOffset timeStamp = code.LastDetectedTime;
                if (timeStamp.Ticks > timeStampLast.Ticks)
                {
                    result = code;
                    timeStampLast = timeStamp;
                }
            }
        }
        return result;
    }

    private Microsoft.MixedReality.QR.QRCode TryGetQrCode(System.Collections.Generic.IList<Microsoft.MixedReality.QR.QRCode> list, string String1, string String2)
    {
        DateTimeOffset timeStampLast = new DateTimeOffset();
        Microsoft.MixedReality.QR.QRCode result = null;
        foreach (Microsoft.MixedReality.QR.QRCode code in list)
        {
            string[] stSplit = code.Data.Split('_');
            if ((stSplit.Length > 1) && (stSplit[0].ToLower() == String1.ToLower()) && (stSplit[1].ToLower() == String2.ToLower()))
            {
                DateTimeOffset timeStamp = code.LastDetectedTime;
                if (timeStamp.Ticks > timeStampLast.Ticks)
                {
                    result = code;
                    timeStampLast = timeStamp;
                    PrintInfoText("Found " + code.Data);
                }
            }
        }
        return result;
    }*/
    #endregion

    #endregion

    #region ROBOT MENU BUTTONS
    public void OnClickSendTargets()
    {
        updateTool = false; // optimization

        // to convert the targets from Unity to RS it is only needed to multiply by (-1) the y position and convert to mm. 
        Transform current_target = null;                        // the target that will be converted into a string
        Vector3 current_pos = Vector3.zero;                     // position of the target that is being computed
        Quaternion current_rot;                                 // rotation in quaternions
        //int numChildren = Table_Workobject.childCount;          // number of children the workobject contains 
        string targets;                                         // string with the targets to sendo to RS

        // target configuration variables
        string target_type;
        string target_speed;
        string target_zone;
        string target_conf;
        TargetConfiguration component;

        GameObject[] arrTargets = GameObject.FindGameObjectsWithTag("Target");
        //int numTargets = numChildren - 2; // number of targets
        int numTargets = arrTargets.Length;
        SendMessageToRS("NT" + numTargets.ToString()); // send RS the number of targets

        if (numTargets <= MAX_NUM_TARGETS)
        {
            foreach (GameObject obj in arrTargets)
            {
                current_target = obj.transform;
                current_pos = current_target.parent.parent.parent.InverseTransformPoint(current_target.position); //position relative to the great grandparent (workobject)
                //current_rot = current_target.localRotation; // local rotation works

                Quaternion WorkObjLocalRotation = Quaternion.Inverse(current_target.transform.rotation) * Table_Workobject.rotation;
                current_rot = WorkObjLocalRotation;

                // transform to RS coordinates:
                current_pos *= 1000; // to mm
                current_pos.y *= (-1); // to right hand coordinate system
                current_rot.y *= (-1); // to right hand quaternion

                // string of targets
                targets = "ST" + current_pos.x.ToString() + "|" + current_pos.y.ToString() + "|" + current_pos.z.ToString() + "|" +
                    current_rot.w.ToString() + "|" + current_rot.x.ToString() + "|" + current_rot.y.ToString() + "|" + current_rot.z.ToString() + "|";
                PrintInfoText(targets);
                SendMessageToRS(targets);
                targets_sent = true;

                component = obj.GetComponent<TargetConfiguration>();

                // check reachability of the targets
                if (message_received == "NotReachable")
                {
                    //int target_pos = (i - 1) * 10;
                    //string strTarget = "T" + target_pos.ToString();
                    HighlightTarget(obj.name);
                }
                else if (!component.weldingGunState)
                {
                    NotHighlightTarget(obj.name,false); // return the target to the target material
                }
                else
                {
                    NotHighlightTarget(obj.name,true);
                }

                // Now we send the information of the target configuration with strings, for example TCJoint|v50|fine|
                           
                target_type = component.type;
                target_zone = component.zone;
                target_speed = component.speed;
                target_conf = "TC" + target_type + "|" + target_speed + "|" + target_zone + "|";

                PrintInfoText(target_conf);
                SendMessageToRS(target_conf);
            }
        }
        else
            PrintInfoText("Number of targets exceed maximum capacity");

    }

    public void OnClickStartSimulation()
    {
        if (targets_sent)
        {
            SendMessageToRS("SS");
            client2 = new TcpClient(IP_ADDRESS, LOCAL_PORT);
            nwStream2 = client2.GetStream();
            updateRobPos = true;
            updateTool = true;
        }
        else
            PrintInfoText("Please, send the targets to RS first");
    }

    public void OnClickStopSimulation()
    {
        stopRobot = true;
    }

    public void OnClickRobotAtTarget()
    {
        if (targets_sent) // targets sent to RS
        {
            GameObject[] arrTargets = GameObject.FindGameObjectsWithTag("Target");

            //int numChildren = Table_Workobject.childCount;  // number of children the workobject contains 
            //int numTargets = numChildren - 2;               // number of targets
            SendMessageToRS("JT" + target_pos.ToString());
            if ((target_pos + 1) <= arrTargets.Length)
                target_pos++;
            else
                target_pos = 1; // returns to the first target

            // Filter the received message and move the robot: 
            MoveRobotToTarget(message_received);
        }
        else
            PrintInfoText("Please, send the targets to RS first");
    }

    public void OnClickGoHome()
    {
        SendMessageToRS("GH"); // Go Home in RS
        MoveRobotToHome();
    }

    private void MoveRobotToTarget(string target)
    {
        string[] strJoints = new string[6]; // array of strings with the joint values

        if (message_received != "Reachability problem")
        {
            strJoints = target.Split("[|]".ToCharArray(), StringSplitOptions.RemoveEmptyEntries); // get an array of 6 strings of joint values
            float angle = 0.00f;
            for (int i = 0; i < 6; i++)
            {
                angle = float.Parse(strJoints[i]);
                angle = (angle * 3.14159265359f) / 180.0f; // angle in rads
                robotBuilder.UpdateMechJoints(i, angle); // update the position of the robot through the robot builder
                updateTool = true; // starts updating in the update loop
            }
        }
        else
            PrintInfoText("Reachability problem");

    }

    private void MoveRobotToHome()
    {
        robotBuilder.UpdateMechJoints(0, 0.0000f);
        robotBuilder.UpdateMechJoints(1, 0.0000f);
        robotBuilder.UpdateMechJoints(2, 0.0000f);
        robotBuilder.UpdateMechJoints(3, 0.0000f);
        robotBuilder.UpdateMechJoints(4, 0.52359879017f);
        robotBuilder.UpdateMechJoints(5, 0.0000f);
        updateTool = true;
    }

    public void OnToggleMoveTool()
    {
        //updateTool = false;

        //enable_manipulation_tool = !enable_manipulation_tool;
        //if (enable_manipulation_tool)
        //{
        //    if (!components_added)
        //    {
        //        // get the components from another gameobject. Copy all components from one gameobject to another
        //        Tool = GameObject.Find("PKI_500_di_M2001");
        //        Tool_Components = GameObject.Find("Tool_Components"); 

        //        Tool.AddComponent<BoxCollider>(); // separatedly to not take the box collider of the game object

        //        foreach (var component in Tool_Components.GetComponents<Component>())
        //        {
        //            var componentType = component.GetType();
        //            if (componentType != typeof(Transform) && componentType != typeof(MeshFilter) && componentType != typeof(MeshRenderer) && componentType != typeof(BoxCollider))
        //            {
        //                UnityEditorInternal.ComponentUtility.CopyComponent(component);
        //                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(Tool);
        //            }
        //        }
        //        Destroy(Tool_Components); // no longer needed

        //        Tool.AddComponent<BoundsControl>(); // separatedly to not see the bounds control of Tool_Components
        //        components_added = true;
        //    }
        //    else
        //    {
        //        //reactivate the bounds control and object manipulator
        //        boundControl = Tool.GetComponent(typeof(BoundsControl)) as BoundsControl;
        //        boundControl.enabled = true;

        //        objManipulator = Tool.GetComponent(typeof(ObjectManipulator)) as ObjectManipulator;
        //        objManipulator.enabled = true;
        //    }

        //    // update orientation of the desired target
        //    // poner como requsito que el robot esté en un target 

        //}
        //else
        //{
        //    // disable the manipulation of the tool
        //    boundControl = Tool.GetComponent(typeof(BoundsControl)) as BoundsControl;
        //    boundControl.enabled = false;

        //    objManipulator = Tool.GetComponent(typeof(ObjectManipulator)) as ObjectManipulator;
        //    objManipulator.enabled = false;
        //}


    }
    #endregion

    #region DebugText in InfoCube
    private string[] _DebugText = new string[30];
    private int _DebugTextIndex = 0;
    private bool SameLineLast = false;
    private bool FirstTime = true;
    private int _LineCount = 0;

    private void TextClear()
    {
        for (int i = 0; i < _DebugText.Length; i++)
        { _DebugText[i] = ""; }
        _DebugTextIndex = 0;
    }

    public void PrintInfoText(string TextLine, bool SameLine = false)
    {
        _LineCount++;
        if (TextLine.Length > 500)
            TextLine = TextLine.Substring(0, 500);
        if (FirstTime)
        {
            FirstTime = false;
            SameLineLast = !SameLine;
        }
        if (SameLine && (SameLine == SameLineLast))
        {
            if (_DebugTextIndex == 0)
            {
                _DebugText[_DebugTextIndex] = _LineCount.ToString() + " " + TextLine;
            }
            else
            {
                _DebugTextIndex--;
                _DebugText[_DebugTextIndex] = _LineCount.ToString() + " " + TextLine;
            }
        }
        else if (_DebugTextIndex < _DebugText.Length)
        {
            _DebugText[_DebugTextIndex] = _LineCount.ToString() + " " + TextLine;
        }
        else
        {
            for (int i = 0; i < (_DebugText.Length - 1); i++)
            {
                _DebugText[i] = _DebugText[i + 1];
            }
            _DebugTextIndex = _DebugText.Length - 1;
            _DebugText[_DebugTextIndex] = _LineCount.ToString() + " " + TextLine;
        }
        _DebugTextIndex++;
        if (InfoCubeText != null)
        {
            Component comp = InfoCubeText.GetComponent("TextMesh");
            if (comp != null)
            {
                string tx = "";
                for (int i = 0; i < _DebugText.Length; i++)
                { tx += _DebugText[i] + "\n"; }
                TextMesh t = (TextMesh)comp;
                t.text = tx;
            }
        }
        SameLineLast = SameLine;
    }

    #endregion

    #region Interface methods
    private void HighlightTarget(string target)
    {
        GameObject[] arrPieces = GameObject.FindGameObjectsWithTag("WeldPiece");
        GameObject sphere;

        foreach (GameObject obj in arrPieces)
        {
            sphere = GameObject.Find("Station/leftTable_Workobj/WeldingPieces/" + obj.name + "/" + target + "/Sphere_Orig"); // get the sphere form that point in the hierarchy
            sphere.GetComponent<Renderer>().material = Highlight_Material; // change the material of the sphere
        }
    }

    private void NotHighlightTarget(string target, bool weldGunActive)
    {
        GameObject[] arrPieces = GameObject.FindGameObjectsWithTag("WeldPiece");
        GameObject sphere;

        foreach (GameObject obj in arrPieces)
        {
            sphere = GameObject.Find("Station/leftTable_Workobj/WeldingPieces/" + obj.name + "/" + target + "/Sphere_Orig"); // get the sphere form that point in the hierarchy
            if(!weldGunActive)
                sphere.GetComponent<Renderer>().material = Target_Material; // change the material of the sphere
            else
                sphere.GetComponent<Renderer>().material = GunActive_Material;
        }
    }
    #endregion

    #region Communication TCP/IP
    private void SendMessageToRS(string message)
    {
        updateTool = false;
        updateRobPos = false;

        // Open the client: we won't do it in the start because we dont need that it is always open
        TcpClient client = new TcpClient(IP_ADDRESS, LOCAL_PORT);
        NetworkStream nwStream = client.GetStream();

        //---send the text---
        byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(message);
        PrintInfoText("Sending : " + message);
        nwStream.Write(bytesToSend, 0, bytesToSend.Length);

        //---read back the text---
        byte[] bytesToRead = new byte[client.ReceiveBufferSize];
        int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
        message_received = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
        PrintInfoText("Received : " + message_received);
        client.Close(); // Close the client
    }

    private void SendMessageToRS_Op(string message) // optimized function
    {
        // Open the client: we won't do it in the start because we dont need that it is always open
        // THIS IS DONE IN THE START SIMULATION
        //client2 = new TcpClient(IP_ADDRESS, LOCAL_PORT);
        //nwStream2 = client2.GetStream();

        //---send the text---
        byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(message);
        nwStream2.Write(bytesToSend, 0, bytesToSend.Length);

        //---read back the text---
        byte[] bytesToRead = new byte[client2.ReceiveBufferSize];
        int bytesRead = nwStream2.Read(bytesToRead, 0, client2.ReceiveBufferSize);
        message_received = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
        //client2.Close(); // Close the client
    }

    #endregion



}
