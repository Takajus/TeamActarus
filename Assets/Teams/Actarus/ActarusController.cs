using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DoNotModify;
using BehaviorDesigner.Runtime;
using System.Linq;

namespace Actarus {

    public class ActarusController : BaseSpaceShipController
    {

        BehaviorTree BT;
        public override void Initialize(SpaceShipView spaceship, GameData data)
        {
            BT = GetComponent<BehaviorTree>();
        }

        public override InputData UpdateInput(SpaceShipView spaceship, GameData data)
        {
            BT.SetVariableValue("speed", spaceship.Velocity.magnitude);
            SpaceShipView otherSpaceship = data.GetSpaceShipForOwner(1 - spaceship.Owner);
            float thrust = 1.0f;
            float targetOrient = spaceship.Orientation;
            float distance = Vector2.Distance(spaceship.Position, data.SpaceShips.Select(x => x.Position).OrderBy(x=> Vector2.Distance(x,spaceship.Position)).Last());
            bool canshoot = distance <= spaceship.Radius;
            BT.SetVariableValue("canshoot", canshoot);

            //Recupération de variable ET LE RESET !!!!
            bool atire = (bool)BT.GetVariable("outFire").GetValue();
            BT.SetVariableValue("outFire", false);


            bool needShoot = AimingHelpers.CanHit(spaceship, otherSpaceship.Position, otherSpaceship.Velocity, 0.15f);
            return new InputData(thrust, targetOrient, needShoot, false, false);
        }
    }

}
