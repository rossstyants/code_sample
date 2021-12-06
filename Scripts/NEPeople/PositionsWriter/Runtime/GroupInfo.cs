using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nexus.Positionswriter.Runtime
{
    [ExecuteAlways]
    public class GroupInfo : MonoBehaviour
    {
        public int groupId;

        private void Update()
        {
            if (!Application.isPlaying)
                gameObject.name = "Group " + groupId;
        }
    }
}
