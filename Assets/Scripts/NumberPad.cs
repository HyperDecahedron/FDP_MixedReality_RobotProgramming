using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class NumberPad : MonoBehaviour
{
    public TextMeshPro textMeshNumber;

    string newNum;
    public string number;

    public bool ip = false; 

    void Start()
    {
        //empty the string when entering
        number = "";
    }

    private void UpdateNumber(string newNum)
    {
        number = number + newNum;
        textMeshNumber.text = number;
    }

    #region BUTTONS

    public void one()
    {
        UpdateNumber("1");
    }
    public void two()
    {
        UpdateNumber("2");
    }
    public void three()
    {
        UpdateNumber("3");
    }
    public void four()
    {
        UpdateNumber("4");
    }
    public void five()
    {
        UpdateNumber("5");
    }
    public void six()
    {
        UpdateNumber("6");
    }
    public void seven()
    {
        UpdateNumber("7");
    }
    public void eight()
    {
        UpdateNumber("8");
    }
    public void nine()
    {
        UpdateNumber("9");
    }
    public void cero()
    {
        UpdateNumber("0");
    }
    public void dot()
    {
        UpdateNumber(".");
    }
    public void positive()
    {
        UpdateNumber("+");
    }
    public void negative()
    {
        UpdateNumber("-");
    }
    public void del()
    {
        if (number != "")
        {
            number = number.Substring(0, number.Length - 1);
            textMeshNumber.text = number;
        }
    }
    public void enter()
    {
        if (ip)
        {
            GameObject mainMenu = GameObject.Find("ScriptMainMenu");
            mainMenu.GetComponent<myMainMenu>().UpdateIP(number);
        }
        else
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
            foreach (GameObject obj in targets)
            {
                obj.GetComponent<TargetConfiguration>().GetNewValue(number);
            }
        }
        number = "";
        textMeshNumber.text = number;
    }
    public void close()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
        foreach (GameObject obj in targets)
        {
            obj.GetComponent<TargetConfiguration>().CloseChangeTransform();
        }
        number = "";
        textMeshNumber.text = number;
    }

    #endregion

}
