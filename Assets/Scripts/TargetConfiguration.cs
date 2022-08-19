using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.Experimental.Physics;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.UI;


public class TargetConfiguration : MonoBehaviour
{
    #region INITIALIZATION
    public float zoneValue;   // Zone values from 0=Fine to 200
    public float typeValue;   // Type of path: 0=Linear; 0.5=Circular; 1=Joint
    public float speedValue;  // Speed values from v5 to v500
    const float ZONE = 0.15f;    //default value of z5
    const float TYPE = 0f;       //default value of Linear
    const float SPEED = 1f;      //default value of v10

    public string type;
    public string zone;
    public string speed;

    const string FINE = "fine";    //default value of z5
    const string LINE = "Linear";       //default value of Linear
    const string V50 = "v50";      //default value of v10

    public bool weldingGunState = false; 

    GameObject zoneSlider;
    GameObject typeSlider;
    GameObject speedSlider;
    GameObject weldGunButton;

    [SerializeField]
    private GameObject numberPad;
    [SerializeField]
    private GameObject enterButton;

    public GameObject target;
    GameObject targetPrefab;

    public int selection = 0;
    GameObject delMenu;
    GameObject confMenu;
    GameObject transformMenu;
    GameObject transMenu;
    [SerializeField]
    private GameObject targetMenu;

    TextMeshPro textMeshZone = null;
    TextMeshPro textMeshSpeed = null;
    TextMeshPro textMeshName = null;
    TextMeshPro textMeshGun = null;

    TextMeshPro textMeshPosX = null;
    TextMeshPro textMeshPosY = null;
    TextMeshPro textMeshPosZ = null;
    TextMeshPro textMeshRotX = null;
    TextMeshPro textMeshRotY = null;
    TextMeshPro textMeshRotZ = null;

    [SerializeField]
    private GameObject toolPrefab;
    GameObject tool;

    [SerializeField]
    private Material TargetMaterial;
    [SerializeField]
    private Material ActiveTargetMaterial;

    public int field; //transform field to change manually
    float newValue;
    Quaternion initialRotation;
    [SerializeField]
    private Transform Table_Workobject = null;

    #endregion

    void Start()
    {
        //confMenu = target.transform.GetChild(7).gameObject;
        delMenu = target.transform.GetChild(5).gameObject;
        delMenu.SetActive(false);

        //Default construction
        zoneValue = ZONE; 
        typeValue = TYPE;
        speedValue = SPEED;

        speed = V50;
        zone = FINE;
        type = LINE;

        transformMenu = target.transform.GetChild(6).gameObject;
        transformMenu.SetActive(false);
        numberPad.SetActive(false);

        if( weldingGunState )
        {
            string targetId = target.name.Substring(1, target.name.Length-1);
            confMenu = FindInActiveObjectByName("m" + targetId);
            weldGunButton = confMenu.transform.GetChild(3).gameObject;
            textMeshGun = weldGunButton.transform.GetChild(0).GetComponent<TextMeshPro>();
            GameObject sphere = target.transform.GetChild(0).gameObject;
            textMeshGun.text = "Active";
            sphere.GetComponent<Renderer>().material = ActiveTargetMaterial;
        }
    }

    GameObject FindInActiveObjectByName(string name)
    {
        Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].hideFlags == HideFlags.None)
            {
                if (objs[i].name == name)
                {
                    return objs[i].gameObject;
                }
            }
        }
        return null;
    }

    //If there has been an interaction in the target we see if it is to delete or configure it
    public void Interaction()
    {
        Debug.Log("interaction");
        string targetName = target.name;
        string targetId = targetName.Substring(1, targetName.Length-1);
        confMenu = FindInActiveObjectByName("m" + targetId);
        confMenu.SetActive(false);  //general menu for all targets
        delMenu = target.transform.GetChild(5).gameObject;
        transMenu = FindInActiveObjectByName("p" + targetId);
        transMenu.SetActive(false);

        switch (selection)
        {
            case 0:
                //Hide all 
                confMenu.SetActive(false);
                delMenu.SetActive(false);
                transformMenu.SetActive(false);
                transMenu.SetActive(false);
                //numberPad.SetActive(false);
                ChangeRotation(false);
                ChangeRotationTool(false);

                targetMenu.transform.GetChild(0).transform.GetChild(2).GetComponent<Interactable>().IsToggled = false;
                targetMenu.transform.GetChild(0).transform.GetChild(3).GetComponent<Interactable>().IsToggled = false;
                targetMenu.transform.GetChild(0).transform.GetChild(4).GetComponent<Interactable>().IsToggled = false;

                //Debug.Log("Nothing");
                break;
            case 1:
                //Delete
                textMeshName = delMenu.transform.GetChild(3).GetComponent<TextMeshPro>();
                textMeshName.text = target.name;
                delMenu.SetActive(true);
                ChangeRotation(false);
                ChangeRotationTool(false);
                confMenu.SetActive(false);
                transformMenu.SetActive(false);
                break;
            case 2:
                //Configure PATH
                confMenu.SetActive(true);
                textMeshName = confMenu.transform.GetChild(6).GetComponent<TextMeshPro>();
                textMeshName.text = target.name;
                //UpdateConfiguration();
                //UpdateInformation();
                confMenu.transform.position = target.transform.position + new Vector3(0.2f, 0f, 0.1f);
                confMenu.transform.rotation = Quaternion.identity;
                ChangeRotation(false);
                ChangeRotationTool(false);
                break;
            case 3:
                //Configuration TARGET
                ChangeRotation(true);
                //Debug.Log("Rotation");
                break;
            case 4:
                //Rotation with Tool
                ChangeRotationTool(true);
                break;
            default:
                //Debug.Log("Nothing Default");
                confMenu.SetActive(false);
                delMenu.SetActive(false);
                ChangeRotation(false);
                ChangeRotationTool(false);
                break;
        }
    }

    #region CHANGE ROTATION AND POSITION WITHOUT TOOL (ALSO MANUAL)
    public void ChangeRotation(bool aux)
    {
        target.GetComponent<BoundsControl>().enabled = aux;
        target.GetComponent<ConstraintManager>().enabled = aux;
        //target.GetComponent<RuntimeTransformHandle>().enabled = aux;

        string targetId = target.name.Substring(1, target.name.Length-1);
        transMenu = FindInActiveObjectByName("p" + targetId);
        transMenu.SetActive(aux);
        initialRotation = transMenu.transform.rotation;
        transMenu.transform.position = target.transform.position + new Vector3(0.2f, 0f, 0.1f);
        UpdateTransform();
    }

    public void GetNewValue(string number)
    {
        //Checks which target is open to change manually
        if( field != 0)
        {
            //Field is different from cero when one of the fields is open to be updated
            UpdatePositionManually(number);
        }
    }

    public void CloseChangeTransform()
    {
        numberPad.SetActive(false);
        field = 0;
    }

    /*public void ChangePositionX()
    {
        numberPad.SetActive(true);
        numberPad.transform.position = target.transform.position + new Vector3(0.4f, 0f, -0.2f);
        field = 1;
    }
    public void ChangePositionY()
    {
        numberPad.SetActive(true);
        numberPad.transform.position = target.transform.position + new Vector3(0.4f, 0f, -0.2f);
        field = 2;
    }
    public void ChangePositionZ()
    {
        numberPad.SetActive(true);
        numberPad.transform.position = target.transform.position + new Vector3(0.4f, 0f, -0.2f);
        field = 3;
    }
    public void ChangeRotationX()
    {
        numberPad.SetActive(true);
        numberPad.transform.position = target.transform.position + new Vector3(0.4f, 0f, -0.2f);
        field = 4;
    }
    public void ChangeRotationY()
    {
        numberPad.SetActive(true);
        numberPad.transform.position = target.transform.position + new Vector3(0.4f, 0f, -0.2f);
        field = 5;
    }
    public void ChangeRotationZ()
    {
        numberPad.SetActive(true);
        numberPad.transform.position = target.transform.position + new Vector3(0.4f, 0f, -0.2f);
        field = 6;
    }*/

    public void UpdatePositionManually(string newValue)
    {
        numberPad.SetActive(false);
        float value;
        //Vector3 auxPos = target.transform.parent.parent.parent.InverseTransformPoint(target.transform.position);    //add the new value to the local position
        Vector3 auxPos = target.transform.localPosition;
        Vector3 auxRot = target.transform.eulerAngles;  

        switch (field)
        {
            //check which field to change
            case 1:
                //POS X
                value = float.Parse(newValue) / 1000;
                auxPos.x = value;
                //target.transform.parent.parent.parent.InverseTransformPoint(target.transform.position) = auxPos;
                target.transform.parent.parent.parent.position = auxPos;
                //target.transform.localPosition = auxPos;
                field = 0; //updated
                break;
            case 2:
                //POS Y
                value = float.Parse(newValue) / 1000;
                auxPos.y = value;
                target.transform.parent.parent.parent.position = auxPos;
                //target.transform.localPosition = auxPos;
                field = 0; //updated
                break;
            case 3:
                //POS Z
                value = float.Parse(newValue) / 1000;
                auxPos.z = value;
                target.transform.parent.parent.parent.position = auxPos;
                //target.transform.localPosition = auxPos;
                field = 0; //updated
                break;
            case 4:
                //ROT X
                value = float.Parse(newValue);
                auxRot.z = value;
                target.transform.parent.parent.parent.eulerAngles = auxRot;
                //target.transform.localEulerAngles = auxRot;
                field = 0; //updated
                break;
            case 5:
                //ROT Y
                value = float.Parse(newValue);
                auxRot.x = value;
                target.transform.parent.parent.parent.eulerAngles = auxRot;
                //target.transform.localEulerAngles = auxRot;
                field = 0; //updated
                break;
            case 6:
                //ROT Z
                value = float.Parse(newValue);
                auxRot.y = value;
                target.transform.parent.parent.parent.eulerAngles = auxRot;
                //target.transform.localEulerAngles = auxRot;
                field = 0; //updated
                break;
            default:
                break;
        }
        UpdateTransform();
        transformMenu.transform.rotation = initialRotation;
    }

    public void UpdateTransform()
    {
        if (selection == 3)
        {
            Debug.Log("updating");

            Vector3 current_pos = target.transform.parent.parent.parent.InverseTransformPoint(target.transform.position); //position relative to the great grandparent (workobject)
            Quaternion current_rot = Quaternion.Inverse(target.transform.transform.rotation) * Table_Workobject.rotation;

            float posX = Mathf.Round(current_pos.x * 1000f) / 1000f;
            posX = posX * 1000f; //show in mm and not in m
            float posY = Mathf.Round(current_pos.y * 1000f) / 1000f;
            posY = posY * 1000f; //show in mm and not in m
            float posZ = Mathf.Round(current_pos.z * 1000f) / 1000f;
            posZ = posZ * 1000f; //show in mm and not in m
            float rotX = Mathf.Round(current_rot.eulerAngles.z * 100f) / 100f;
            float rotY = Mathf.Round(current_rot.eulerAngles.x * 100f) / 100f;
            float rotZ = Mathf.Round(current_rot.eulerAngles.y * 100f) / 100f;

            //Show the rotation and position to the user
            /*float posX = Mathf.Round(target.transform.localPosition.x * 1000f) / 1000f;
            posX = posX * 1000f; //show in mm and not in m
            float posY = Mathf.Round(target.transform.localPosition.y * 1000f) / 1000f;
            posY = posY * 1000f; //show in mm and not in m
            float posZ = Mathf.Round(target.transform.localPosition.z * 1000f) / 1000f;
            posZ = posZ * 1000f; //show in mm and not in m
            float rotX = Mathf.Round(target.transform.localEulerAngles.z * 100f) / 100f;
            float rotY = Mathf.Round(target.transform.localEulerAngles.x * 100f) / 100f;
            float rotZ = Mathf.Round(target.transform.localEulerAngles.y * 100f) / 100f;*/

            textMeshPosX = transMenu.transform.GetChild(1).transform.GetChild(3).transform.GetChild(0).GetComponent<TextMeshPro>();
            //textMeshPosX = target.transform.GetChild(6).transform.GetChild(1).transform.GetChild(3).transform.GetChild(0).GetComponent<TextMeshPro>();
            textMeshPosX.text = posX.ToString();
            textMeshPosY = transMenu.transform.GetChild(2).transform.GetChild(3).transform.GetChild(0).GetComponent<TextMeshPro>();//target.transform.GetChild(6).transform.GetChild(2).transform.GetChild(3).transform.GetChild(0).GetComponent<TextMeshPro>();
            textMeshPosY.text = posY.ToString();
            textMeshPosZ = transMenu.transform.GetChild(3).transform.GetChild(3).transform.GetChild(0).GetComponent<TextMeshPro>(); //target.transform.GetChild(6).transform.GetChild(3).transform.GetChild(3).transform.GetChild(0).GetComponent<TextMeshPro>();
            textMeshPosZ.text = posZ.ToString();

            textMeshRotX = transMenu.transform.GetChild(5).transform.GetChild(3).transform.GetChild(0).GetComponent<TextMeshPro>(); //target.transform.GetChild(6).transform.GetChild(5).transform.GetChild(3).transform.GetChild(0).GetComponent<TextMeshPro>();
            textMeshRotX.text = rotX.ToString();
            textMeshRotY = transMenu.transform.GetChild(6).transform.GetChild(3).transform.GetChild(0).GetComponent<TextMeshPro>();//target.transform.GetChild(6).transform.GetChild(6).transform.GetChild(3).transform.GetChild(0).GetComponent<TextMeshPro>();
            textMeshRotY.text = rotY.ToString();
            textMeshRotZ = transMenu.transform.GetChild(7).transform.GetChild(3).transform.GetChild(0).GetComponent<TextMeshPro>();//target.transform.GetChild(6).transform.GetChild(7).transform.GetChild(3).transform.GetChild(0).GetComponent<TextMeshPro>();
            textMeshRotZ.text = rotZ.ToString();
        } 
    }
    #endregion

    #region CHANGE ROTATION WITH TOOL
    bool createTool = true;
    public void ChangeRotationTool(bool aux)
    {
        target.GetComponent<BoundsControl>().enabled = aux;
        target.GetComponent<ConstraintManager>().enabled = aux;

        if (aux && createTool)
        {
            tool = Instantiate(toolPrefab, target.transform);
            tool.transform.position = target.transform.position;
            tool.transform.localRotation = Quaternion.identity;
            tool.SetActive(aux);
            createTool = false;
        }
        else if( !aux && !createTool )
        {
            createTool = true;
            Destroy(tool);
        }
    }
    #endregion

    #region CHANGE PATH CONFIGURATION
    public void ChangeConfiguration()
    {
        typeSlider = confMenu.transform.GetChild(0).gameObject;
        zoneSlider = confMenu.transform.GetChild(1).gameObject;
        speedSlider = confMenu.transform.GetChild(2).gameObject;

        typeValue = typeSlider.GetComponent<StepSlider>().SliderValue;
        zoneValue = zoneSlider.GetComponent<StepSlider>().SliderValue;
        speedValue = speedSlider.GetComponent<StepSlider>().SliderValue;

        type = TypeConversion(typeValue);
        zone = ZoneConversion(zoneValue);
        speed = SpeedConversion(speedValue);

        UpdateInformation();
    }

    public void ChangeStateWeldingGun()
    {
        weldGunButton = confMenu.transform.GetChild(3).gameObject;
        textMeshGun = weldGunButton.transform.GetChild(0).GetComponent<TextMeshPro>();

        weldingGunState = !weldingGunState;

        //sphere = GameObject.Find("Station/leftTable_Workobj/WeldingPieces/" + obj.name + "/" + target + "/Sphere_Orig"); // get the sphere form that point in the hierarchy
        //sphere.GetComponent<Renderer>().material = Highlight_Material; // change the material of the sphere

        GameObject sphere = target.transform.GetChild(0).gameObject;

        if (weldingGunState)
        {
            textMeshGun.text = "Active";
            sphere.GetComponent<Renderer>().material = ActiveTargetMaterial;
        }
        else
        {
            textMeshGun.text = "Not Active";
            sphere.GetComponent<Renderer>().material = TargetMaterial;
        }
    }

    private void UpdateInformation()
    {
        textMeshZone = zoneSlider.transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshPro>();
        textMeshSpeed = speedSlider.transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshPro>();

        textMeshZone.text = zone;
        textMeshSpeed.text = speed;
    }

    private string TypeConversion(float t)
    {
        string type;
        t = Mathf.Round(t * 100f) / 100f;

        switch (t)
        {
            case 0.00f:
                type = "Linear"; break;
            case 0.50f:
                type = "Circle"; break;
            case 1.00f:
                type = "Joint"; break;
            default:
                type = "Linear"; break;
        }
        return type; 
    }

    private string ZoneConversion(float z)
    {
        string zone;
        z = Mathf.Round(z * 100f) / 100f;

        switch (z)
        {
            case 0.00f:
                zone = "fine"; break;
            case 0.08f:
                zone = "z1"; break;
            case 0.15f:
                zone = "z5"; break;
            case 0.23f:
                zone = "z10"; break; ;
            case 0.31f:
                zone = "z15"; break ;
            case 0.38f:
                zone = "z20"; break ;
            case 0.46f:
                zone = "z30"; break ;
            case 0.54f:
                zone = "z40"; break ;
            case 0.62f:
                zone = "z50"; break ;
            case 0.69f:
                zone = "z60"; break ;
            case 0.77f:
                zone = "z80"; break ;
            case 0.85f:
                zone = "z100"; break ;
            case 0.92f:
                zone = "z150"; break ;
            case 1.00f:
                zone = "z200"; break;
            default:
                zone = "z10"; break;
        }
        return zone;
    }

    private string SpeedConversion(float s)
    {
        string speed;
        s = Mathf.Round(s * 100f) / 100f;

        switch (s)
        {
            case 0.00f:
                speed = "v5"; break;
            case 0.07f:
                speed = "v10"; break;
            case 0.14f:
                speed = "v20"; break;
            case 0.21f:
                speed = "v30"; break;
            case 0.29f:
                speed = "v40"; break;
            case 0.36f:
                speed = "v50"; break;
            case 0.43f:
                speed = "v60"; break;
            case 0.5f:
                speed = "v80"; break;
            case 0.57f:
                speed = "v100"; break;
            case 0.64f:
                speed = "v150"; break;
            case 0.71f:
                speed = "v200"; break;
            case 0.79f:
                speed = "v250"; break;
            case 0.86f:
                speed = "v300"; break;
            case 0.93f:
                speed = "v400"; break;
            case 1f:
                speed = "v500"; break;
            default:
                speed = "v10"; break;
        }
        return speed;
    }
    #endregion
}
