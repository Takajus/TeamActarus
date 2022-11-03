using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Actarus
{
    public class GoTo : ActarusAction
    {
        public override TaskStatus OnUpdate()
        {
            Owner.SetVariableValue("canGoTo", true);
            return TaskStatus.Success;
        }
    }
}
