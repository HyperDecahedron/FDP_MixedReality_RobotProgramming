using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrHandMenuControl : MonoBehaviour
{
    [SerializeField]
    private Transform HandMenu;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HideMenu() // solo hace que aparezca el menu cuando te miras la mano. 
    {
        if (HandMenu == null)
        {
            HandMenu = GetComponent<Transform>();
        }
        if (HandMenu != null)
        {
            HandMenu.transform.SetPositionAndRotation(new Vector3(0, 0.5f, 0), Quaternion.identity);
           // scrInitApplication.DebugText("HideMenu " + HandMenu.name);
        }
    }
}
