using UnityEngine;
using NxAppStructure;
using System.Collections.Generic;
using Nexus.NEPeopleData.Runtime;

namespace AppStructure.Data
{
    [CreateAssetMenu(fileName = "NEPeopleGroupsData", menuName = "Nexus/Instance/NEPeopleGroupsData", order = 1)]
    public class NEPeopleGroupsData : NxAppData
    {
        #region Variables
        public List<PeopleGroupInfo> peopleGroups = new List<PeopleGroupInfo>();
        #endregion

        #region CustomMethods
        #endregion

        #region RequiredMethods
        /// <summary>
        /// Called as soon as the app has been initialized and the state machine is ready. 
        /// </summary>
        public override void Initialize()
        {
        }
        #endregion

    }
}
