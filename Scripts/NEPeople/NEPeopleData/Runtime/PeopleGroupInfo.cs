using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nexus.NEPeopleData.Runtime
{
    [Serializable]
    public class PeopleGroupInfo
    {
        public int groupID;
        public Vector2 groupCenter;
        public List<PersonInfo> people = new List<PersonInfo>();
    }
}
