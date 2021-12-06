using System.Collections.Generic;
using Nexus.NEPeopleData.Runtime;
using UnityEngine;
using AppStructure.Data;

namespace Nexus.Positionswriter.Runtime
{
    public class PositionsWriter : MonoBehaviour
    {
        #region Variables
        [SerializeField] float areaWidth;   //in meters
        [SerializeField] float areaHeight;  //in meters
        [SerializeField] float planeWidth, planeHeight; //in Unity units
        List<PeopleGroupInfo> groupInfos = new List<PeopleGroupInfo>();
        public NEPeopleGroupsData peopleData;   //Scriptable object to write data to
        #endregion

        private Vector2 ConvertToCm(Vector2 localXY)
        {
            //convert to -0.5 to 0.5
            float xRatio = localXY.x / (planeWidth * 2f);
            float yRatio = localXY.y / (planeHeight * 2f);

            //0-1
            xRatio += 0.5f;
            yRatio += 0.5f;

            //cm
            float xCm = xRatio * areaWidth * 100f;
            float yCm = yRatio * areaHeight * 100f;

            return new Vector2(xCm, yCm);
        }

        //Called from Editor script (button on inspector)
        public void WritePositions()
        {
            GroupInfo[] groups = transform.GetComponentsInChildren<GroupInfo>();

            foreach(GroupInfo gi in groups)
            {
                PeopleGroupInfo pgi = GetGroupWithID(gi.groupId);

                //pgi.conversationClip = gi.clip;
                pgi.groupCenter = ConvertToCm(new Vector2(gi.transform.position.x, gi.transform.position.z));

                PersonPosition[] positions = gi.transform.GetComponentsInChildren<PersonPosition>();

                foreach (PersonPosition pp in positions)
                {
                    if (pp.clip == null)
                        Debug.Log(pp.gameObject.name + " has no clip...");
                    if (pp.modelPrefab == null)
                        Debug.Log(pp.gameObject.name + " has no prefab...");

                    //PeopleGroupInfo pgi = GetGroupWithID(pp.groupId);

                    //pgi.peoplePositions.Add(ConvertToCm(new Vector2(pp.transform.position.x, pp.transform.position.z)));
                    //pgi.conversationClip.Add(pp.clip);

                    PersonInfo newPerson = new PersonInfo();
                    newPerson.positionCm = ConvertToCm(new Vector2(pp.transform.position.x, pp.transform.position.z));
                    newPerson.clip = pp.clip;
                    newPerson.rotation = pp.transform.rotation;
                    newPerson.umbrellaPositionCm = pp.umbrella.transform.localPosition;
                    newPerson.umbrellaLocalEuler = pp.umbrella.transform.localEulerAngles;

                    newPerson.prefab = pp.modelPrefab;
                    pgi.people.Add(newPerson);
                }
            }

            peopleData.peopleGroups.Clear();

            //now print them all out...
            foreach (PeopleGroupInfo pgi in groupInfos)
            {
                peopleData.peopleGroups.Add(pgi);
            }

            Debug.Log("All done. Made " + groupInfos.Count + " groups");
        }

        private PeopleGroupInfo GetGroupWithID(int groupID)
        {
            foreach (PeopleGroupInfo pgi in groupInfos)
            {
                if (pgi.groupID == groupID)
                {
                    return pgi;
                }
            }

            //otherwise make a new one
            PeopleGroupInfo newPGI = new PeopleGroupInfo();
            newPGI.groupID = groupID;
            groupInfos.Add(newPGI);
            return newPGI;
        }
    }
}
