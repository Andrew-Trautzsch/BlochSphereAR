using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveTrail : MonoBehaviour
{
    private TrailRenderer trailRenderer;

    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.enabled = true;
    }

    public void DisableTrail(){
        trailRenderer.enabled = false;
    }
    public void EnableTrail(){
        trailRenderer.enabled = true;
    }
}