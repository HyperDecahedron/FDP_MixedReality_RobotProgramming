// V 200
// <copyright file="RobotMechanismBuilder.cs" company="ABB">
// Copyright (c) ABB. All rights reserved.
// </copyright>
namespace RK
{
    using System.Collections.Generic;
    using UnityGLTF;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;
    using Microsoft.MixedReality.Toolkit.UI;
    using System.Threading;
    using System.Threading.Tasks;
    using RobotStudio.Services.RobApi;
    using System;

    public class RobotMechanismBuilder : MonoBehaviour
    {
        #region Data
        [SerializeField]
        private GLTFComponent gltf = null;

        [SerializeField]
        private GameObject pointPrefab = null;

        [SerializeField]
        private GameObject highlightQuadPrefab = null;

        private Mechanism selectedMechanism = null;

        private List<Mechanism> allMechanisms = new List<Mechanism>();

        private string taskName = "T_ROB1";
        private bool _Busy = false;
        private int _LoopDelay = 25;
        private int _LoopCount = 0;
        private bool _UpdateJoints = false;
        private Transform GLTFScene; // holds reference to loaded object

        // NEW 
        private GameObject _link6;
        private GameObject _Tool;
        private Matrix4x4 _ToolOffset = Matrix4x4.identity;

        public static bool MechanismLoaded = false;/// Set to true when the load is complete
        private int ModelIndexLast = 0;
        #endregion

        #region Start and Update
        private void Start()
        {
            this.gltf = this.GetComponent<GLTFComponent>();
            this.gltf.OnKinematicsDataProcessed -= this.OnKinematicsDataProcessed;
            this.gltf.OnKinematicsDataProcessed += this.OnKinematicsDataProcessed;
        }

        private void Update()
        {

        }

        #endregion

        private void OnKinematicsDataProcessed(Dictionary<int, (string name, string kinematics, float[] jointValues)> kinematicsData)
        {
            //this.gltf.OnKinematicsDataProcessed -= this.OnKinematicsDataProcessed;

            this.allMechanisms.Clear();

            InstantiatedGLTFObject igltf = this.transform.GetComponentInChildren<InstantiatedGLTFObject>();

            foreach (int kinematicsDataKey in kinematicsData.Keys)
            {
                // "key" is object name to apply mechanism to
                Transform[] childTransforms = igltf.transform.GetComponentsInChildren<Transform>(true);

                var data = kinematicsData[kinematicsDataKey];

                foreach (Transform childTransform in childTransforms)
                {
                    if (childTransform.name == data.name)
                    {
                        var newMechanism = childTransform.gameObject.AddComponent<Mechanism>();
                        newMechanism.BuildMechanismWithData(
                            data.kinematics,
                            data.jointValues,
                            this.pointPrefab,
                            this.highlightQuadPrefab);

                        this.allMechanisms.Add(newMechanism);
                    }
                }
            }

            this.selectedMechanism = this.allMechanisms[0];
            _link6 = GameObject.Find("Link6"); // is it always Link 6
            _Tool = GameObject.Find("PKI_500_di_M2001");
            _ToolOffset.SetColumn(3, new Vector4(0, 0, 0.1f, 1)); // 100 mm in z
            Debug.Log("OnKinematicsDataProcessed end" );
            UpdateToolPos();
        }

        public void LoadStation(string filePath)
        {
            gltf.LoadOnCommand(filePath);
            Debug.Log("Model Loaded");
        }

        #region Slider interface
        public void OnSliderValueChangedAxis1(SliderEventData data)
        {
            UpdateMechJointsSlider(0, data.NewValue);
        }
        public void OnSliderValueChangedAxis2(SliderEventData data)
        {
            UpdateMechJointsSlider(1, data.NewValue);
        }
        public void OnSliderValueChangedAxis3(SliderEventData data)
        {
            UpdateMechJointsSlider(2, data.NewValue);
        }
        public void OnSliderValueChangedAxis4(SliderEventData data)
        {
            UpdateMechJointsSlider(3, data.NewValue);
        }
        public void OnSliderValueChangedAxis5(SliderEventData data)
        {
            UpdateMechJointsSlider(4, data.NewValue);
        }
        public void OnSliderValueChangedAxis6(SliderEventData data)
        {
            UpdateMechJointsSlider(5, data.NewValue);
        }
        #endregion

        private void UpdateMechJointsSlider(int AxisIndex, float value)
        {
            if (this.selectedMechanism != null)
            {
                float validatedValue = Mathf.Clamp(value, 0, 1);
                float angle = (float)this.selectedMechanism.minLimits[AxisIndex] + ((float)this.selectedMechanism.maxLimits[AxisIndex] - (float)this.selectedMechanism.minLimits[AxisIndex]) * validatedValue;
                this.selectedMechanism.jointValues[AxisIndex] = angle;
                this.selectedMechanism.CallCalculateTransforms();
                UpdateToolPos();
            }
        }

        public void UpdateMechJoints(int AxisIndex, float value)
        {
            try
            {
                if (this.selectedMechanism != null)
                {
                    this.selectedMechanism.jointValues[AxisIndex] = value;
                    this.selectedMechanism.CallCalculateTransforms();
                    //UpdateToolPos(); 
                }
            }
            catch (Exception ex) { }
        }

        public void UpdateToolPos()
        {
           if ((_link6 != null) && (_Tool != null))
            {
                Quaternion rotation = Quaternion.Euler(0, 0, 180); // extra rotation needed for the tool
                Matrix4x4 mRot = Matrix4x4.Rotate(rotation);

                Matrix4x4 mLink6 = _link6.transform.localToWorldMatrix;
                Matrix4x4 newTransform = mLink6 * _ToolOffset * mRot;
                _Tool.transform.SetPositionAndRotation(newTransform.ExtractPosition(), newTransform.ExtractRotation()); // world space
            }
            else
                Debug.Log("Tool not found");
        }

        private static float ToRadians(float angleInDegree)
        {
            return (angleInDegree * Mathf.PI) / 180.0f;
        }
        
    }
}
