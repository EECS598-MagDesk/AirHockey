using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aiManager : MonoBehaviour
{
    private Vector3 targetPos;
    private Vector3 defaultPos;

    // Start is called before the first frame update
    void Start()
    {
        targetPos = new Vector3(-0.6f, 0.0001f, 0f);
        defaultPos = new Vector3(-0.6f, 0.0001f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void TrackPos(Vector3 puckPos)
    {
        targetPos.z = puckPos.z;
        targetPos.x = puckPos.x;
        defaultPos.z = puckPos.z;
    }

    public Vector3 GetPosition()
    {
        if (targetPos.x > -0.2)
        {
            return defaultPos;
        }
        else
        {
            return targetPos;
        }
        
    }
}
