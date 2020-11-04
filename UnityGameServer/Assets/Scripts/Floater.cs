using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    public Rigidbody rigidBody;
    public float depthBeforeSubmerged = 1f;
    public float displacementAmount = 3f;
    public int floatCount = 1;
    public float waterDrag = 0.99f;
    public float waterAngularDrag = 0.5f;

    private void FixedUpdate()
    {
        rigidBody.AddForceAtPosition(Physics.gravity / floatCount, transform.position, ForceMode.Acceleration);
        float x = ((transform.position.x * WaterWaves.instance.perlinScale) + (Time.time * WaterWaves.instance.waveSpeed) + WaterWaves.instance.offset);
        float z = ((transform.position.z * WaterWaves.instance.perlinScale) + (Time.time * WaterWaves.instance.waveSpeed) + WaterWaves.instance.offset);
        float waveHeight = WaterWaves.instance.GetWaveHeight(x, z);
        //float waveHeight = WaterWaves.instance.GetWaveHeight(transform.position.x, transform.position.z);
        
        if (transform.position.y < waveHeight)
        {
            float displacementMultiplier = Mathf.Clamp01((waveHeight - transform.position.y) / depthBeforeSubmerged) * displacementAmount;
            rigidBody.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0f), transform.position, ForceMode.Acceleration);
            rigidBody.AddForce(displacementMultiplier * -rigidBody.velocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
            rigidBody.AddTorque(displacementMultiplier * -rigidBody.angularVelocity * waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }

}
