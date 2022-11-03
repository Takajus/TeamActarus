using System.Collections;
using System.Collections.Generic;
using Actarus;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class ActarusAction : Action
{
    ActarusController AC;
    public override void OnAwake()
    {
        AC = gameObject.GetComponent<ActarusController>();
    }
}
