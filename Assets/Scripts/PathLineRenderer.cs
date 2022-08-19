using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathLineRenderer : MonoBehaviour
{
    public LineRenderer lr;
    
    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    public void ShowPath()
    {
        Update();
    }

    private void Update()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");

        lr.positionCount = targets.Length;

        int i = 0;
        foreach (GameObject obj in targets)
        {
            //Debug.Log(obj.name);
            lr.SetPosition(i, obj.transform.position);
            i++;
        }
    }
}
