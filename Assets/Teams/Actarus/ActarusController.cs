using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DoNotModify;
using BehaviorDesigner.Runtime;
using System.Linq;

namespace Actarus
{
    public class ActarusController : BaseSpaceShipController
    {
        public class Sector
        {
            public Vector2 zone;
            public List<WayPointView> pointList;
            public float range;
            public int key;
        }

        private float orient = 0f;
        private Sector[] sectors = new Sector[4];
        private int sectorindex;
        BehaviorTree BT;
        Camera cameraPoint;
        WayPointView currentWaypoint;
        private bool dropMine;
        public override void Initialize(SpaceShipView spaceship, GameData data)
        {
            BT = GetComponent<BehaviorTree>();
            DefineSector(data);
        }

        public override InputData UpdateInput(SpaceShipView spaceship, GameData data)
        {
            //BT.SetVariableValue("speed", spaceship.Velocity.magnitude);
            bool canGoTo = (bool)BT.GetVariable("canGoTo").GetValue();

            CheckSector(spaceship, true);

            SpaceShipView otherSpaceship = data.GetSpaceShipForOwner(1 - spaceship.Owner);
            float thrust = 1.0f;
            float targetOrient = spaceship.Orientation;
            float distance = Vector2.Distance(spaceship.Position, data.SpaceShips.Select(x => x.Position).OrderBy(x => Vector2.Distance(x, spaceship.Position)).Last());
            bool canshoot = distance <= spaceship.Radius;
            //BT.SetVariableValue("canshoot", canshoot);

            //Recupération de variable ET LE RESET !!!!
            bool atire = (bool)BT.GetVariable("outFire").GetValue();
            BT.SetVariableValue("outFire", false);


            bool needShoot = AimingHelpers.CanHit(spaceship, otherSpaceship.Position, otherSpaceship.Velocity, 0.15f);
            return new InputData(thrust, orient, needShoot, dropMine, false);
        }

        public void DefineSector(GameData data)
        {
            cameraPoint = FindObjectOfType<Camera>();
            sectors[0] = new Sector();
            sectors[1] = new Sector();
            sectors[2] = new Sector();
            sectors[3] = new Sector();
            sectors[0].zone = new Vector2(cameraPoint.gameObject.transform.position.x - 2f * cameraPoint.orthographicSize, cameraPoint.gameObject.transform.position.y + 2.5f * cameraPoint.aspect);
            sectors[1].range = 20f;
            sectors[0].range = 20f;
            sectors[2].range = 20f;
            sectors[3].range = 20f;
            sectors[1].zone = new Vector2(cameraPoint.gameObject.transform.position.x + 2f * cameraPoint.orthographicSize, cameraPoint.gameObject.transform.position.y + 2.5f * cameraPoint.aspect);
            sectors[2].zone = new Vector2(cameraPoint.gameObject.transform.position.x - 2f * cameraPoint.orthographicSize, cameraPoint.gameObject.transform.position.y - 2.5f * cameraPoint.aspect);
            sectors[3].zone = new Vector2(cameraPoint.gameObject.transform.position.x + 2f * cameraPoint.orthographicSize, cameraPoint.gameObject.transform.position.y - 2.5f * cameraPoint.aspect);

            for (int a = 0; a < sectors.Length; a++)
            {
                sectors[a].pointList = new List<WayPointView>();
                for (int i = 0; i < data.WayPoints.Count; i++)
                {
                    if (Vector2.Distance(sectors[a].zone, data.WayPoints[i].Position) <= sectors[a].range)
                    {
                        sectors[a].pointList.Add(data.WayPoints[i]);
                    }
                }
            }
        }

        static int GetSpecificSectorBindPoint(SpaceShipView spaceship, Sector sector)
        {
            int bindcount = 0;
            bool allpointTake = true;
            for (int i = 0; i < sector.pointList.Count; i++)
            {
                if (sector.pointList[i].Owner != spaceship.Owner)
                {
                    allpointTake = false;
                }
            }
            if (!allpointTake)
            {
                for (int j = 0; j < sector.pointList.Count; j++)
                {
                    if (sector.pointList[j].Owner != spaceship.Owner)
                    {
                        bindcount++;
                    }
                }
            }

            sector.key = bindcount;
            return bindcount;
        }

        public void CheckSector(SpaceShipView spaceship, bool OnSector)
        {
            List<int> bindpoints = new List<int>();
            for (int i = 0; i < sectors.Length; i++)
            {
                bindpoints.Add(GetSpecificSectorBindPoint(spaceship, sectors[i]));
            }
            dropMine = false;
            sectorindex = Array.IndexOf(sectors, sectors.First(i => i.key == bindpoints.Max()));
            if (OnSector)
            {
                WayPointView wayPoint = sectors[sectorindex].pointList
                    .OrderBy(i => Vector2.Distance(spaceship.Position, i.Position)).First(i=> i.Owner != spaceship.Owner);
                if (currentWaypoint != null)
                {
                    if(currentWaypoint.Owner == spaceship.Owner)
                    {
                        dropMine = true;
                    }
                }
                currentWaypoint = wayPoint;
                GoTo(wayPoint, spaceship);
            }
        }

        void GoTo(WayPointView wayPoint, SpaceShipView spaceShip)
        {
            orient = AimingHelpers.ComputeSteeringOrient(spaceShip, wayPoint.Position);
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawLine(new Vector3(-cameraPoint.orthographicSize * 2, cameraPoint.aspect * 2.5f, 0f), new Vector3(0f, cameraPoint.aspect * 2.5f, 0f));
            Gizmos.DrawLine(new Vector3(0f, cameraPoint.aspect * 2.5f, 0f), new Vector3(0f, 0f, 0f));
            Gizmos.DrawLine(new Vector3(-cameraPoint.orthographicSize * 2, 0f, 0f), new Vector3(0f, 0f, 0f));
            Gizmos.DrawLine(new Vector3(-cameraPoint.orthographicSize * 2, 0f, 0f), new Vector3(-cameraPoint.orthographicSize * 2, cameraPoint.aspect * 2.5f, 0f));
        }
    }

}
