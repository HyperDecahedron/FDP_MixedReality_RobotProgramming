using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Experimental.Physics;
using Microsoft.MixedReality.Toolkit.UI;


public class ConfigurationMenuScript : MonoBehaviour
{
    GameObject target;
    public GameObject numberPad;

    public void UpdateConfiguration()
    {
        string menuName = this.name;
        string Id = menuName.Substring(1, menuName.Length-1);
        target = GameObject.Find("t" + Id);
        target.GetComponent<TargetConfiguration>().ChangeConfiguration();
    }

    public void UpdateGunState()
    {
        target.GetComponent<TargetConfiguration>().ChangeStateWeldingGun();
    }

    public void UpdateTransformConfiguration()
    {
        string menuName = this.name;
        string Id = menuName.Substring(1, menuName.Length-1);
        target = GameObject.Find("t" + Id);
        target.GetComponent<TargetConfiguration>().UpdateTransform(); //updates when the target is touched
    }

    public void ChangePositionX()
    {
        numberPad.SetActive(true);
        target = GameObject.Find("t" + this.name.Substring(1, this.name.Length-1));
        numberPad.transform.position = target.transform.position + new Vector3(0.3f, 0f, -0.2f);
        target.GetComponent<TargetConfiguration>().field = 1;
    }
    public void ChangePositionY()
    {
        numberPad.SetActive(true);
        target = GameObject.Find("t" + this.name.Substring(1, this.name.Length - 1));
        numberPad.transform.position = target.transform.position + new Vector3(0.3f, 0f, -0.2f);
        target.GetComponent<TargetConfiguration>().field = 2;
    }
    public void ChangePositionZ()
    {
        target = GameObject.Find("t" + this.name.Substring(1, this.name.Length - 1));
        target.GetComponent<TargetConfiguration>().field = 3;
        numberPad.SetActive(true);
        numberPad.transform.position = target.transform.position + new Vector3(0.3f, 0f, -0.2f);
    }
    public void ChangeRotationX()
    {
        target = GameObject.Find("t" + this.name.Substring(1, this.name.Length - 1));
        target.GetComponent<TargetConfiguration>().field = 4;
        numberPad.SetActive(true);
        numberPad.transform.position = target.transform.position + new Vector3(0.3f, 0f, -0.2f);
    }
    public void ChangeRotationY()
    {
        target = GameObject.Find("t" + this.name.Substring(1, this.name.Length - 1));
        target.GetComponent<TargetConfiguration>().field = 5;
        numberPad.SetActive(true);
        numberPad.transform.position = target.transform.position + new Vector3(0.3f, 0f, -0.2f);
    }
    public void ChangeRotationZ()
    {
        target = GameObject.Find("t" + this.name.Substring(1, this.name.Length - 1));
        target.GetComponent<TargetConfiguration>().field = 6;
        numberPad.SetActive(true);
        numberPad.transform.position = target.transform.position + new Vector3(0.3f, 0f, -0.2f);
    }

    public ElasticsManager regularSnap;
    public ElasticsManager emptySnap;
    public void activateSnap()
    {
        target = GameObject.Find("t" + this.name.Substring(1, this.name.Length - 1));
        //target.GetComponent<ObjectManipulator>().ElasticsManager = regularSnap;
    }
    public void removeSnap()
    {
        target = GameObject.Find("t" + this.name.Substring(1, this.name.Length - 1));
        //target.GetComponent<ObjectManipulator>().ElasticsManager = emptySnap;
    }

}
