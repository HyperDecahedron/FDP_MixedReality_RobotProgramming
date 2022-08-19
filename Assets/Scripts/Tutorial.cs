using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutorial : MonoBehaviour
{
    Vector3 pos = new Vector3(0.0f, 0.0f, 0.5f);
    Quaternion rot = Quaternion.identity;

    public TextMeshPro stepTitle = null;
    public TextMeshPro stepDescription = null;
    public TextMeshPro stepDescriptionTitle = null;

    public GameObject tutorial;

    string[] titles;
    string[] description;
    string[] descriptionTitle;

    int id = 0;     //variable to state which step we are in
    int totalSteps = 14;

    void Start()
    //public void ActiveTutorial()
    {
        initialization();

        //Open the tutorial
        callStep(0);
        id = 0;

    }

    public void NextStep()
    {
        id = (id + 1) % totalSteps; 
        callStep(id);
    }

    public void PreviousStep()
    {
        id--;
        if(id < 0)
        {
            id += totalSteps;
        }
        callStep(id);
    }

    private void callStep(int aux)
    {
        stepTitle.text = titles[aux];
        stepDescription.text = description[aux];
        stepDescriptionTitle.text = descriptionTitle[aux];
    }

    private void initialization()
    {
        //Here is the construction of all the text that is going to appear on the screen
        titles = new string[totalSteps];
        description = new string[totalSteps];
        descriptionTitle = new string[totalSteps];

        titles[0] = "Step 1:";
        descriptionTitle[0] = "Open the menus";
        description[0] = "- Face up your hand palm.\r\n" +
            "- You have a shortcut to the menus there.\r\n" +
            "- Open the Main Menu by pressing its button.";


        titles[1] = "Step 2:";
        descriptionTitle[1] = "Place the Station";
        description[1] = "- Press QR Code Search button on Main Menu. \r\n " +
            "- Move to the QR code to scan it until you see the frame set in place.\r\n" +
            "- Once it is read, press QR Code Search button again to stop reading. The robot station will be placed there.";

        titles[2] = "Step 3:";
        descriptionTitle[2] = "Select the Weld Piece";
        description[2] = "- Press Select Welding Piece button on the Main Menu.\r\n" +
            "- Choose the piece you want to work with.\r\n" +
            "- Once it is selected you can move it around.";

        titles[3] = "Step 4:";
        descriptionTitle[3] = "Place the Weld Piece";
        description[3] = "Manually:\r\n" +
            "   - Grab the piece and place it where wanted.\r\n" +
            "   - Save the position with the button Save Home Position on the piece menu.\r\n \r\n" +
            "Automatically:\r\n" +
            "   - Press QR Code Search button on Main Menu.\r\n" +
            "   - Scan the QR code to place the model alligned with the real piece.\r\n" +
            "   - Save the position with the button Save As Home on the piece menu.";

        titles[4] = "Step 5:";
        descriptionTitle[4] = "Work with the Weld Piece";
        description[4] = "- Move the piece and pin it with the Manipulation button.\r\n" +
            "- Rotate and scale the piece with the Adjust button.\r\n" +
            "- Show or hide its name with the Name button.\r\n" +
            "- Hide the toolbar with the button Hide.\r\n" +
            "- Hide the piece with the button Remove. It can be access again through the Main Menu.";

        titles[5] = "Step 6:";
        descriptionTitle[5] = "Add Targets";
        description[5] = "- Open the Target Menu with the Hand Menu shortcut (hand palm).\r\n" +
            "- Press Add Target button to add targets.\r\n" +
            "- Scale up the Weld Piece to place the targets.\r\n" +
            "- Grab the target and place it on the Weld Piece. It will snap to the vertices.\r\n" +
            " If the target is not snapped correctly to the vertix, enlarge the piece.";

        titles[6] = "Step 7:";
        descriptionTitle[6] = "Delete Targets";
        description[6] = "- To delete ALL targets press Delete All Targets button on the Target Menu.\r\n" +
            "- To delete only selected targets press Delete Selected Target button and touch whichever you want to delete.";

        titles[7] = "Step 8:";
        descriptionTitle[7] = "Rotate Targets";
        description[7] = "- To rotate the target press Rotate Target button on the Target Menu.\r\n" +
            "- To rotate the target with the robot tool showing for extra help, press Rotate At Target on the Robot Menu.\r\n";

        titles[8] = "Step 9:";
        descriptionTitle[8] = "Configure Targets";
        description[8] = "- To configure the Speed, Zone and Velocity value for each target press Edit Target Configuration button on the Target Menu.\r\n" +
            "- Select the target you want to configure.\r\n" +
            "- Move the sliders to change the configuration that will have the path on each target.";

        titles[9] = "Step 10:";
        descriptionTitle[9] = "Weld Piece at Home";
        description[9] = "When the targets are set place the Weld Piece to the Home Position.\r\n" +
            "- Press the button Weld Piece Home on the Main Menu.\r\n" +
            "The piece and its targets will go back to the original position, rotation and scale as in the real world piece.";

        titles[10] = "Step 12:";
        descriptionTitle[10] = "Send Targets to Robot";
        description[10] = "- Open the Robot Menu with the Hand Menu (hand palm).\r\n" +
            "- Press the button Send Targets on the Robot Menu.\r\n" +
            "- To view the robot at each target press At Target button.\r\n" +
            "- The targets can be changed if desired as the previous steps explained.";

        titles[11] = "Step 13:";
        descriptionTitle[11] = "Play the Path";
        description[11] = "- To play the Simulation of the path press Start Sim button on the Robot Menu.\r\n" +
            "- To stop the simulation press Stop Sim.";

        titles[12] = "Step 14:";
        descriptionTitle[12] = "Move the Robot";
        description[12] = "- To move the robot manually press Jog Robot button on the Robot Menu.\r\n" +
            "- To go back to the home position press Go Home button.\r\n" +
            "- To change the tool orientation at each target press Tool At Target.";

        titles[13] = "WELD DONE";
    }
}
