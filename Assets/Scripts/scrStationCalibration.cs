using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Collections.Concurrent;
using System;

public class scrStationCalibration : MonoBehaviour
{
    [SerializeField]
    public Transform StationOrigin;
    [SerializeField]
    public RadialView MainMenuRadialView;
    [SerializeField]
    public Interactable ButtonPin;
    [SerializeField]
    public GameObject WorkspaceBoundingBox;

    // process things that come in on background threads on the unity thread
    private ConcurrentQueue<Action> UpdateQueue = new ConcurrentQueue<Action>();
    private int updateCount = 0;
    // Start is called before the first frame update
    void Start()
    {
       // _searchAreaControl = WorkspaceBoundingBox.GetComponent<SearchAreaController>();
      //  EnableStationMenuRadialView();
    }

    private void EnableStationMenuRadialView()
    {
#if WINDOWS_UWP
       
            if (MainMenuRadialView != null)
            {
                MainMenuRadialView.enabled = true;
            }
        if (ButtonPin != null)
        {
            ButtonPin.IsToggled = false;
        }
#endif
    }

    public void ToggleLockWorkspace()
    {
        if (WorkspaceBoundingBox != null)
        {
            Component objBounds = WorkspaceBoundingBox.GetComponent("BoundsControl");
            Component objMan = WorkspaceBoundingBox.GetComponent("ObjectManipulator");
            if ((objBounds != null) && (objMan != null))
            {
                ObjectManipulator contrObjMan = (ObjectManipulator)objMan;
                contrObjMan.enabled = !contrObjMan.enabled;
               // _searchAreaControl.EnablePointerChange = contrObjMan.enabled;


              //  BoundsControl contrObjBounds = (BoundsControl)objBounds;
              //   contrObjBounds.enabled = contrObjMan.enabled;
            }
        }
    }

    public void ToggleHideWorkspace()
    {
        if (WorkspaceBoundingBox != null)
        {
            WorkspaceBoundingBox.SetActive(!WorkspaceBoundingBox.activeSelf);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize_objectAnchorsService()
    {

        //if (_objectAnchorsService == null)
        //    _objectAnchorsService = ObjectAnchorsService.GetService();
        ////_objectTracker = FindObjectOfType<ObjectTracker>();
        //////_objectTracker = FindObjectOfType<ObjectTracker>();

        //if (_objectAnchorsService != null)
        //{
        //    List<System.Guid> mylist = _objectAnchorsService.ModelIds;
        //    if (mylist.Count > 0)
        //    {
        //     //   Matrix4x4? originToCenterTransform = _objectAnchorsService.GetModelOriginToCenterTransform(mylist[0]);
        //        List<IObjectAnchorsTrackingResult> result = _objectAnchorsService.TrackingResults;
        //        if (result.Count > 0)
        //        {
        //            ObjectAnchorsLocation? location = result[0].Location;
        //            if(location.HasValue)
        //            {
        //                float surfaceCoverage = result[0].SurfaceCoverage;
        //                Matrix4x4? modelOriginToCenterTransform = _objectAnchorsService.GetModelOriginToCenterTransform(result[0].ModelId);
        //                 Matrix4x4 m1 = Matrix4x4.TRS(location.Value.Position, location.Value.Orientation, new Vector3(1, 1, 1));
        //                Matrix4x4 newLocation = m1 * modelOriginToCenterTransform.Value;
        //                Matrix4x4 mRot = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(-90.0f,0,180.0f), new Vector3(1, 1, 1)); // spin to match GLTF importer
        //                Matrix4x4 updatedStationOrigin = newLocation * mRot;
        //                StationOrigin.SetPositionAndRotation(updatedStationOrigin.MultiplyPoint(Vector3.zero), updatedStationOrigin.rotation);
        //                scrStationCalibration.DebugText("result.Count " + result.Count.ToString() + " surfaceCoverage " + surfaceCoverage.ToString());
        //            }
        //        }
        //        //   Matrix4x4 m1 = Matrix4x4.TRS(_model.transform.position, _model.transform.rotation, Vector3.one);
        //        //  Matrix4x4 m2 = m1 * originToCenterTransform.Value.inverse;
        //        //   Workspace.transform.SetPositionAndRotation(m2.ExtractPosition(), m2.ExtractRotation());
        //      //  scrStationCalibration.DebugText("Workspace position updated");
        //    }




        //    // _objectAnchorsService.ObjectUpdated += _objectAnchorsService_ObjectUpdated;
        //    //  _objectAnchorsService.RunningChanged += _objectAnchorsService_RunningChanged;
        //    //   _objectAnchorsService.ObjectUpdated += _objectAnchorsService_ObjectUpdated;
        //    //   scrStationCalibration.DebugText("_objectAnchorsService initilized");
        //}
        //else
        //{
        //    scrStationCalibration.DebugText("_objectAnchorsService  Not initilized");
        //}

        ////   SearchAreaBoundingBox.SetActive(true);
        ////  scrApplTools.EnableStationMenu(false);
        ////  scrApplTools.EnableSearchMenu(true);
        ////  EnableWorkspaceBoundingBox(true);
    }


    //   private ConcurrentQueue<Tuple<ObjectAnchorsBoundingBox, IEnumerable<ObjectQuery>>> _queryQueue = new ConcurrentQueue<Tuple<ObjectAnchorsBoundingBox, IEnumerable<ObjectQuery>>>();

    //private void _objectAnchorsService_RunningChanged(object sender, ObjectAnchorsServiceStatus e)
    //{
    //    UpdateQueue.Enqueue(() => { MyRunningChanged(e); });

    //}
    //private void MyRunningChanged(ObjectAnchorsServiceStatus e)
    //{
    //  //  runningChanged++;
    //    bool running = e.HasFlag(ObjectAnchorsServiceStatus.Running);
    //    scrStationCalibration.DebugText("MyRunningChanged  running " + running.ToString());
    //    if (running)
    //    {
    //        //  boxObject.SetActive(false);
    //        //   updateCount = 0;
    //        //  _LocationFound = false;
    //      //  _objectAnchorsService.ObjectUpdated += _objectAnchorsService_ObjectUpdated;
            
    //        //UpdateSearchBoxLocation();
    //    }
    //    else
    //    {
    //      //  _objectAnchorsService.ObjectUpdated -= _objectAnchorsService_ObjectUpdated;
    //      //  if (updateCount > 0)
    //      //  {
    //            // move Station object to the found location
    //        //    GameObject importedModel = GameObject.Find("RsStation");
    //        //    if (importedModel != null)
    //        //    {
    //        //        importedModel.transform.localScale = Vector3.one;
    //        //        importedModel.transform.SetPositionAndRotation(wobj0.transform.position, wobj0.transform.rotation);
    //        //        scrApplTools.DebugText("wobj0 Euler " + wobj0.transform.rotation.eulerAngles.ToString());
    //        //        UpdateSliderValueAfterScaling(Vector3.one.x);
    //        //        scrApplTools.EnableSearchMenu(false);
    //        //        scrApplTools.EnableStationMenu(true);
    //        //        // SearchAreaBoundingBox.SetActive(false);
    //        //    }
    //        //}
    //    }
    //  //  scrApplTools.DebugText("MyRunningChanged " + running.ToString());
    //}

    //private void _objectAnchorsService_ObjectUpdated(object sender, IObjectAnchorsServiceEventArgs e)
    //{
    //    UpdateQueue.Enqueue(() => { MyObjectUpdated(e); });
    //}

    //private void MyObjectUpdated(IObjectAnchorsServiceEventArgs e)
    //{
    //    updateCount++;
    //    scrStationCalibration.DebugText("MyObjectUpdated updateCount " + updateCount.ToString());
    //    //scrStationCalibration.DebugText("updateCount " + updateCount.ToString(), true);
    //    ////   TextDebug2("Running " + running.ToString(), "UpdateCount: " + updateCount.ToString());

    //    //Guid modelId = e.ModelId;
    //    //Matrix4x4? originToCenterTransform = _objectAnchorsService.GetModelOriginToCenterTransform(modelId);
    //    //Matrix4x4 location = Matrix4x4.TRS(e.Location.Value.Position, e.Location.Value.Orientation, new Vector3(1, 1, 1));
    //    //_NewLocation = location * originToCenterTransform.Value;
    //    //_LocationFound = true;

    //    ////  GameObject frame1 = GameObject.Find("GizmoLeft1");
    //    //GameObject frame2 = GameObject.Find("GizmoLeft2");
    //    //if (frame2 != null)
    //    //    frame2.transform.SetPositionAndRotation(location.MultiplyPoint(Vector3.zero), location.rotation);

    //    //wobj0.transform.SetPositionAndRotation(_NewLocation.ExtractPosition(), _NewLocation.ExtractRotation());
    //    //// wobj0.transform.Translate(new Vector3(0, 0.005f, 0)); // translate to robot base

    //    ////  
    //    //GameObject refObject = GameObject.Find("RefObject");
    //    //if ((refObject != null) && refObject.activeSelf)
    //    //    refObject.SetActive(false);

    //    //  boxObject.transform.position = e.Location.Value.Position;
    //    //  boxObject.transform.transform.rotation = e.Location.Value.Orientation;
    //    // AddOrUpdate(e);
    //}


    #region DebugText
    private static string[] _DebugText = new string[30];
    private static int _DebugTextIndex = 0;
    private static bool SameLineLast = false;
    private static bool FirstTime = true;
    public static void DebugText(string TextLine, bool SameLine = false)
    {
        if (TextLine.Length > 100)
            TextLine = TextLine.Substring(0, 100);
        Debug.Log(TextLine);
        if (FirstTime)
        {
            FirstTime = false;
            SameLineLast = !SameLine;
        }
        if (SameLine && (SameLine == SameLineLast))
        {
            if (_DebugTextIndex == 0)
            {
                _DebugText[_DebugTextIndex] = TextLine;
            }
            else
            {
                _DebugTextIndex--;
                _DebugText[_DebugTextIndex] = TextLine;
            }
        }
        else if (_DebugTextIndex < _DebugText.Length)
        {
            _DebugText[_DebugTextIndex] = TextLine;
        }
        else
        {
            for (int i = 0; i < (_DebugText.Length - 1); i++)
            {
                _DebugText[i] = _DebugText[i + 1];
            }
            _DebugTextIndex = _DebugText.Length - 1;
            _DebugText[_DebugTextIndex] = TextLine;
        }
        _DebugTextIndex++;
        GameObject gob = GameObject.Find("DebugText");
        if (gob != null)
        {
            Component comp = gob.GetComponent("TextMesh");
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
}

public static class TransformExtensions
{
    public static Quaternion ExtractRotation(this Matrix4x4 matrix)
    {
        Vector3 forward;
        forward.x = matrix.m02;
        forward.y = matrix.m12;
        forward.z = matrix.m22;

        Vector3 upwards;
        upwards.x = matrix.m01;
        upwards.y = matrix.m11;
        upwards.z = matrix.m21;

        return Quaternion.LookRotation(forward, upwards);
    }
    public static Vector3 ExtractPosition(this Matrix4x4 matrix)
    {
        Vector3 position;
        position.x = matrix.m03;
        position.y = matrix.m13;
        position.z = matrix.m23;
        return position;
    }
}