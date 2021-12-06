using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nexus.NEPeopleData.Runtime
{
    [System.Serializable]
    public class PersonInfo
    {
        public GameObject prefab;
        public Vector2 positionCm;
        public AudioClip clip;
        public Quaternion rotation;
        public Vector3 umbrellaPositionCm;
        public Vector3 umbrellaLocalEuler;
    }
}
