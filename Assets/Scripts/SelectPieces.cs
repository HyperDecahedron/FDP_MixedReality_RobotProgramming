using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.UI;

public class SelectPieces : MonoBehaviour
{
    GameObject[] piecesReal;
    GameObject[] pieces;
    public GameObject weldParent;
    public int selectedPiece = 0;
    GameObject appBar;
    GameObject tooltip;
    GameObject mainMenu;
    public GameObject menu; 

    Vector3 pos = new Vector3(0.0f, 0.0f, 0.5f);
    Quaternion rot = Quaternion.identity;
    //Vector3 scale = new Vector3(1f, 1f, 1f);

    bool[] first; 


    void Start()
    {
        piecesReal = GameObject.FindGameObjectsWithTag("WeldPiece");
        pieces = new GameObject[piecesReal.Length];

        first = new bool[piecesReal.Length]; //variable to know if it is the first time entering the app.

        //Copy each weld piece to show
        for( int i = 0; i<piecesReal.Length; i++)
        {
            pieces[i] = Instantiate(piecesReal[i], this.transform); //weldParent.transform);
            pieces[i].tag = "Untagged";
            piecesReal[i].SetActive(false);
            first[i] = true;
        }

        foreach( GameObject obj in pieces)
        {
            appBar = obj.transform.GetChild(0).gameObject;  // First child must be the appBar
            appBar.SetActive(false);    //deactivates the AppBar
            tooltip = obj.transform.GetChild(2).gameObject;
            tooltip.SetActive(true);
            obj.GetComponent<ObjectManipulator>().enabled = false;  //Does not allow the manipulation of the piece
            obj.SetActive(false);   // begins with all pieces not visible
        }
        Debug.Log("Started");
        menu.SetActive(false);
    }

    // Everytime we activate the selection menu we deactivate the app bar and the manipulation of the pieces
    public void ActiveMenu()
    {
        GameObject[] arrayPiece = GameObject.FindGameObjectsWithTag("WeldPiece");
        foreach(GameObject obj in arrayPiece)
        {
            obj.SetActive(false);
        }

        mainMenu = GameObject.Find("MainMenu");

        //pos = mainMenu.transform.position;
        //pos = Camera.main.transform.position;
        //rot = mainMenu.transform.rotation; // Quaternion.identity; //mainMenu.transform.rotation;
        //rot = Camera.main.transform.rotation;
        //rot = Quaternion.LookRotation(pos, Vector3.up);

        //menu.transform.position = pos + new Vector3(0.5f, 0f, 0.5f);    // + new Vector3(0.05f, 0.03f, 0f);
        //menu.transform.rotation = rot;
        //transform.localScale = scale;

        pieces[selectedPiece].SetActive(true); //shows the first piece
        //Debug.Log("Num: " + pieces.Length);
        //Debug.Log("Actual piece: " + selectedPiece);

        foreach( GameObject obj in pieces)
        {
            appBar = obj.transform.GetChild(0).gameObject;  // First child must be the appBar
            appBar.SetActive(false);    //deactivates the AppBar
            tooltip = obj.transform.GetChild(2).gameObject;
            tooltip.SetActive(true);
            obj.GetComponent<ObjectManipulator>().enabled = false;  //Does not allow the manipulation of the piece
        }
    }

    public void NextPiece()
    {
        pieces[selectedPiece].gameObject.SetActive(false);
        selectedPiece = (selectedPiece + 1) % pieces.Length;
        pieces[selectedPiece].gameObject.SetActive(true);
    }

    public void PreviousPiece()
    {
        pieces[selectedPiece].gameObject.SetActive(false);
        selectedPiece--;
        if( selectedPiece < 0)
        {
            selectedPiece += pieces.Length;
        }
        pieces[selectedPiece].gameObject.SetActive(true);
    }

    public void SelectPiece()
    {
        Vector3 clonePos = pieces[selectedPiece].transform.position;
        pieces[selectedPiece].SetActive(false);
        GameObject piece = piecesReal[selectedPiece];
        piece.SetActive(true);
        if (first[selectedPiece])
        {
            //The first time we show the piece next to the one in the menu.
            first[selectedPiece] = false;
            piece.transform.position = clonePos;
        }
        appBar = piece.transform.GetChild(0).gameObject; 
        appBar.SetActive(true);    //activates the AppBar
        piece.GetComponent<ObjectManipulator>().enabled = true;    //activates the manipulation

        menu.SetActive(false);
    }

    public void Out()
    {
        //pieces[selectedPiece].SetActive(false);
        menu.SetActive(false);
    }

    public void GoHome()
    {
        GameObject[] piecesHome = GameObject.FindGameObjectsWithTag("WeldPiece");

        foreach (GameObject obj in piecesHome)
        {
            obj.GetComponent<PieceConfiguration>().GoHomePosition();
        }
    }
}
