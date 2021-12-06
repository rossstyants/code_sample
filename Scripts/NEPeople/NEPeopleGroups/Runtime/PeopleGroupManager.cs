using System.Collections.Generic;
using Nexus.NEPeopleData.Runtime;
using UnityEngine;

namespace Nexus.NEPeopleGroups.Runtime
{
    public class PeopleGroupManager : MonoBehaviour
    {
        #region Variables
        [SerializeField, ReadOnly] private bool initialised;
        public List<PeopleGroup> PeopleGroups = new List<PeopleGroup>();
        [SerializeField] private GameObject peopleGroupPrefab;  //this prefab is just for the group (not the people - they are instantiated separately)
        [SerializeField] private Transform peopleParentTransform; //where to put them in the hieararchy
        [SerializeField, ReadOnly] private PeopleGroup inFocusGroup; //which group is currently in focus (this could be use to lower vollume on other groups etc.)
        [SerializeField, ReadOnly] private Vector3 worldCenter; //where the user initiallly clicked - to create the AR scene
        public Material umbrellaMaterial;
        #endregion

        //slightly larger than the audioSource maxDistance to allow for the fact that this is in the center of the group
        //and people are spread out within it.
        const float focusTriggerDistance = 4.2f;

        public void Initialise(Vector3 worldCenterPosition, Vector2 trafficMeterPositionOnMap)
        {
            initialised = true;

            //the center is not actually the center - it is where the traffic meter is placed - for now.

            worldCenter = new Vector3(worldCenterPosition.x - trafficMeterPositionOnMap.x, worldCenterPosition.y, worldCenterPosition.z - trafficMeterPositionOnMap.y);
        }

        public PeopleGroup AddPeopleGroup(PeopleGroupInfo peopleGroupInfo, GameObject personPrefab)
        {
            Debug.Assert(initialised);

            PeopleGroup newGroup = GameObject.Instantiate(peopleGroupPrefab, peopleParentTransform).GetComponent<PeopleGroup>();

            newGroup.transform.position = new Vector3((peopleGroupInfo.groupCenter.x / 100f) + worldCenter.x,
                                                        worldCenter.y,
                                                        (peopleGroupInfo.groupCenter.y / 100f) + worldCenter.z);

            newGroup.Initialise(focusTriggerDistance);
            newGroup.AddSomePeople(worldCenter, peopleGroupInfo, personPrefab);
            //newGroup.InitialiseConversation(peopleGroupInfo.conversationClip);
            PeopleGroups.Add(newGroup);
            return newGroup;
        }

        public void UpdateWhichGroupIsInFocusByOnlyDistance(Transform triggerTransform)
        {
            float distanceBuffer = 0.1f;    //so it's not too twitchy
            Vector3 triggerPos = triggerTransform.position;

            foreach (PeopleGroup pg in PeopleGroups)
            {
                //ignore y position
                triggerPos.y = pg.transform.position.y;

                //get distance to group
                float dist = Vector3.Distance(pg.transform.position, triggerPos);

                if (pg.InFocus && dist > (focusTriggerDistance + distanceBuffer))
                {
                    pg.ToggleFocus(false);
                }
                else if (!pg.InFocus && dist <= (focusTriggerDistance - distanceBuffer))
                {
                    pg.ToggleFocus(true);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="triggerTransform"></param>
        public void UpdateWhichGroupIsInFocus(Transform triggerTransform)
        {
            const float minAngleDiff = 24f;
            const float tooClose = 0.3f;
            const float tooFarAway = 2f;
            Vector3 triggerPos = triggerTransform.position;
            //        float[] pgScores = new float[PeopleGroups.Count];

            float highestScore = 0f;
            PeopleGroup highestScoringGroup = null;

            foreach (PeopleGroup pg in PeopleGroups)
            {
                float score = 0f;

                //ignore y position
                triggerPos.y = pg.transform.position.y;

                //get distance to group
                float dist = Vector3.Distance(pg.transform.position, triggerPos);

                //get angle to group - just on the  x,z plane
                Vector3 camForward = triggerTransform.forward;
                camForward.y = 0f;
                Vector3 vectorToGroup = pg.transform.position - triggerPos;

                float angleDiff = Vector3.Angle(camForward, vectorToGroup);

                //so heuristic is something like - if distance is not too small -
                //i.e. you're on top of them and not too big... i.e. too far away
                //and you're facing towards them within an angle cone
                //and they're the best match from this criteria - then they're in focus
                //and that's the only conversation you'll hear?

                if (angleDiff < minAngleDiff)
                {
                    score += 1f;
                }

                if (dist > tooClose && dist < tooFarAway)
                {
                    score += 1f;
                    score += (tooFarAway - dist);
                }

                if (score >= 2f && score > highestScore) //(score above 2 means angle is good and distance is good)
                {
                    highestScore = score;
                    highestScoringGroup = pg;
                }
            }

            //Now see if the highest scoring group is not the currently in focus group
            //and switch
            //or if there's no group currently in focus then see if
            //the highest scoring group should be focused (score above 2 means angle is good and distance is good)

            if (highestScoringGroup == null && inFocusGroup != null)
            {
                //no group has scored highly enough to be in focus
                inFocusGroup.ToggleFocus(false);
                inFocusGroup = null;
            }
            else if (highestScoringGroup != inFocusGroup)
            {
                if (inFocusGroup == null)
                {
                    //no group was in focus before...
                    inFocusGroup = highestScoringGroup;
                    highestScoringGroup.ToggleFocus(true);
                }
                else
                {
                    //switching focus from another group
                    inFocusGroup.ToggleFocus(false);
                    inFocusGroup = highestScoringGroup;
                    inFocusGroup.ToggleFocus(true);
                }
            }
        }

        //public void ChangeUmbrellaColour(Color newColor)
        //{
        //    umbrellaMaterial.color = newColor;
        //}
    }
}
