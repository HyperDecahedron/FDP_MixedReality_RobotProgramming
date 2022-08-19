using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

using Microsoft.MixedReality.Toolkit.Experimental.Physics; 
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using TMPro;

public class DeployTargets : MonoBehaviour
{
    #region FUNCTIONS
    public GameObject targetPrefab; //prefab of targets
    private GameObject newTarget;

    public GameObject menuConfPrefab;
    public GameObject menuTransPrefab;
    private GameObject confMenu;
    private GameObject transMenu;
    public GameObject menus;

    int id; // variable to increment the target name by 10. eg t10, t20, t30...

    [SerializeField]
    private TextMeshPro textMeshActiveTarget = null;

    void Start()
    { 
        id = 110;    //initialization
    }

    //Creates a new target
    public void addTarget()
    {
        GameObject[] pieces = GameObject.FindGameObjectsWithTag("WeldPiece");
        if ( pieces.Length >= 1)
        {
            generateTarget(id); //Calls the target constructor
            id = id + 10;    // updates the id number for the next target

            GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");

            foreach (GameObject obj in targets)
            {
                obj.GetComponent<TargetConfiguration>().selection = 0;
            }
        }
        else
        {
            Debug.Log("Add a piece first");
        }
    }

    //Deletes all targets
    public void deleteTargets()
    {
        GameObject[] destroyObject = GameObject.FindGameObjectsWithTag("Target");
        foreach( GameObject obj in destroyObject)
        {
            destroyTarget(obj); // Calls the destructor
        }

        id = 10;    //resets the name
    }

    //Activates the selection of targets to be deleted
    public void activateDeleteSelectedTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");

        foreach( GameObject obj in targets)
        {
            obj.GetComponent<TargetConfiguration>().selection = 0;
            obj.GetComponent<TargetConfiguration>().selection = 1;
        }
    }

    //Activates the configuration edit
    public void activateEditConfiguration()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");

        foreach( GameObject obj in targets)
        {
            //obj.GetComponent<TargetConfiguration>().selection = 0;
            obj.GetComponent<TargetConfiguration>().selection = 2;
            Debug.Log("2");
        }
    }

    public void updateTargetConfiguration()
    {
        string name = textMeshActiveTarget.text; // GetComponent<TextMesh>().text;
        GameObject activeTarget = GameObject.Find(name);
        activeTarget.GetComponent<TargetConfiguration>().ChangeConfiguration();
    }

    //Deletes the selected target
    public void deleteSelectedTarget(GameObject obj)
    {
        destroyTarget(obj); //Calls the destructor
    }

    //Activates the selection of targets to be rotated
    public void activateRotateTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");

        foreach (GameObject obj in targets)
        {
            obj.GetComponent<TargetConfiguration>().selection = 0;
            obj.GetComponent<TargetConfiguration>().selection = 3;
        }
    }

    //Activates the selection of targets to be rotated with the tool
    public void activateRotateTargetTool()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");

        foreach (GameObject obj in targets)
        {
            obj.GetComponent<TargetConfiguration>().selection = 0;
            obj.GetComponent<TargetConfiguration>().selection = 4;
        }
    }

    //Deactivates the configuration edit, delete and rotate
    public void cancelTargetEdit()
    {
        // To be called when we don't have any target selection button triggered
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");

        foreach (GameObject obj in targets)
        {
            obj.GetComponent<TargetConfiguration>().selection = 0;
            obj.GetComponent<TargetConfiguration>().ChangeRotation(false);
            obj.GetComponent<TargetConfiguration>().ChangeRotationTool(false);
        }

        GameObject[] menus = GameObject.FindGameObjectsWithTag("Menu");
        foreach( GameObject obj in menus)
        {
            obj.SetActive(false);
        }
        GameObject[] transMenu = GameObject.FindGameObjectsWithTag("Trans");
        foreach (GameObject obj in transMenu)
        {
            obj.SetActive(false);
        }
    }
#endregion

    #region TARGET CONSTRUCTOR AND DESTRUCTOR
    private void generateTarget(int id)
    {
        GameObject objParent = GameObject.FindWithTag("WeldPiece");

        // Creates the new target from the prefab where the user points. 
        Vector3 pos = objParent.transform.position + new Vector3(0f, 0.1f, -0.3f); ;
        //pos = Camera.main.transform.position;   // I think this would work. Check it at ASSAR
        //Quaternion rot = Quaternion.identity;

        newTarget = Instantiate(targetPrefab, objParent.transform) as GameObject;
        newTarget.name = "t" + id;
        newTarget.tag = "Target";
        //newTarget.transform.localScale = new Vector3(5,5,5);
        newTarget.transform.position = pos; // new Vector3(0f, 0f, 0.5f);
        newTarget.transform.rotation = targetPrefab.transform.rotation;
        newTarget.GetComponent<BoundsControl>().enabled = false;
        newTarget.SetActive(true);

        confMenu = Instantiate(menuConfPrefab, menus.transform) as GameObject;
        confMenu.transform.position = newTarget.transform.position + new Vector3(0.01f, 0f, 0f);
        confMenu.transform.rotation = Quaternion.identity;
        confMenu.name = "m" + id;
        confMenu.tag = "Menu";
        confMenu.SetActive(false);

        transMenu = Instantiate(menuTransPrefab, menus.transform) as GameObject;
        transMenu.transform.position = newTarget.transform.position + new Vector3(0.01f, 0f, 0f);
        transMenu.transform.rotation = menuTransPrefab.transform.rotation;
        transMenu.name = "p" + id;
        transMenu.tag = "Trans";
        transMenu.SetActive(false);
    }

    private void destroyTarget(GameObject obj)
    {
        Destroy(obj);
    }
     #endregion

}

