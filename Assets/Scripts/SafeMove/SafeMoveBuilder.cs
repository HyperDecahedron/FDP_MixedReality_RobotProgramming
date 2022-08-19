// <copyright file="SafeMoveBuilder.cs" company="ABB">
// Copyright (c) ABB. All rights reserved.
// </copyright>
namespace RK
{
    using System.Collections.Generic;
    using UnityGLTF;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;

    public class SafeMoveBuilder : MonoBehaviour
    {
        [SerializeField]
        private GLTFComponent gltf = null;

        private List<SafeMoveElement> allSafeMoveElements = new List<SafeMoveElement>();

        public void OnSafeMoveVisibilityChanged(bool value)
        {
            InstantiatedGLTFObject igltf = this.transform.GetComponentInChildren<InstantiatedGLTFObject>();

            SafeMoveElement[] elements = igltf.transform.GetComponentsInChildren<SafeMoveElement>(true);
            foreach (SafeMoveElement element in elements)
            {
                element.gameObject.SetActive(value);
            }
        }

        private void Start()
        {
            this.gltf = this.GetComponent<GLTFComponent>();
            this.gltf.OnSafeMoveDataProcessed -= this.OnSafeMoveDataProcessed;
            this.gltf.OnSafeMoveDataProcessed += this.OnSafeMoveDataProcessed;
        }

        private void OnSafeMoveDataProcessed(Dictionary<int, string> safeMoveData)
        {
            this.gltf.OnSafeMoveDataProcessed -= this.OnSafeMoveDataProcessed;

            InstantiatedGLTFObject igltf = this.transform.GetComponentInChildren<InstantiatedGLTFObject>();

            foreach (int safeMoveDataKey in safeMoveData.Keys)
            {
                // "key" is object name to apply mechanism to
                Transform[] childTransforms = igltf.transform.GetComponentsInChildren<Transform>(true);
                var objectName = safeMoveData[safeMoveDataKey];

                foreach (Transform childTransform in childTransforms)
                {
                    if (childTransform.name == objectName)
                    {
                        var newSafeMoveElement = childTransform.gameObject.AddComponent<SafeMoveElement>();
                        this.allSafeMoveElements.Add(newSafeMoveElement);
#if UNITY_EDITOR
                        Debug.Log($"[SafeMoveBuilder] OnSafeMoveDataProcessed: Added SafeMoveElement object to game object {safeMoveDataKey}");
#endif
                    }
                }
            }

            this.StartCoroutine(this.AnnounceModelOptions());
        }

        private IEnumerator AnnounceModelOptions()
        {
            if (this.allSafeMoveElements.Count > 0)
            {
                yield return new WaitForSeconds(0.3f);

                var modelOptions = this.gameObject.GetComponent<ModelOptions>();

                if (modelOptions == null)
                {
                    modelOptions = this.gameObject.AddComponent<ModelOptions>();
                }

                modelOptions.SetModelOptions(new HashSet<ModelOptionType>() { ModelOptionType.SafeMoveZone });
            }

            yield return null;
        }
    }
}
