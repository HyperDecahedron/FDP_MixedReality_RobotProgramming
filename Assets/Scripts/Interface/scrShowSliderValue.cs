using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;

public class scrShowSliderValue : MonoBehaviour
{
    [SerializeField]
    private TextMesh textMesh = null;

    public void OnSliderUpdated(SliderEventData eventData)
    {
        if (textMesh == null)
        {
            textMesh = GetComponent<TextMesh>();
        }

        if (textMesh != null)
        {
            textMesh.text = $"{eventData.NewValue:F2}";
        }
    }

}
