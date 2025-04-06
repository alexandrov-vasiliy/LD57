using System;
using TMPro;
using UnityEngine;

public class DataMonitorView : MonoBehaviour
{
    public TMP_Text depth;
    public TMP_Text speed;
    public TMP_Text health;

    public Rigidbody playerRB;
    public DepthController DepthController;
    public Health Health;

    private void Update()
    {
        depth.text = "Depth: " + DepthController.depth.ToString("0") + "/" + DepthController.maxDepth;
        speed.text = "Speed: " + playerRB.linearVelocity.magnitude.ToString("0");
        health.text = "Health: " + Health.HP.ToString("0");
    }
}
