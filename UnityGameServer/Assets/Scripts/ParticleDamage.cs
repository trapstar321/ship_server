using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDamage : MonoBehaviour
{
    // Start is called before the first frame update
    public virtual float Damage(string particleSystemName, GameObject parent) {
        return 0;
    }
}
