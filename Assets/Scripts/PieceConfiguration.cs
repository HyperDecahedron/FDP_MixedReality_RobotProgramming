using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceConfiguration : MonoBehaviour
{
    Vector3 pos = new Vector3(0.0f,0.0f,0.5f);
    Quaternion rot = Quaternion.identity;
    Vector3 scale = new Vector3(1f, 1f, 1f);

    public void SaveHomePosition()
    {
        pos = transform.position;
        rot = transform.rotation;
        scale = transform.localScale;
    }

    public void GoHomePosition()
    {
        transform.position = pos;
        transform.rotation = rot;
        transform.localScale = scale;
    }
}
