using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack
{
    public string attackName;
    public float multiplier;
    public bool done;

    public PlayerAttack Clone() {
        return new PlayerAttack() { attackName=attackName, multiplier = this.multiplier };
    }
}
