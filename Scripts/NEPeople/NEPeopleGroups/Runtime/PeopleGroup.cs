using System.Collections.Generic;
using Nexus.NEPeopleData.Runtime;
using UnityEngine;

namespace Nexus.NEPeopleGroups.Runtime
{
    /// <summary>
    /// A group of people in the NE experience. Can have their focus toggled.
    /// </summary>
    public class PeopleGroup : MonoBehaviour
    {
        [ReadOnly] public bool InFocus;
        [ReadOnly] public List<PersonController> People = new List<PersonController>();
        [SerializeField] private Collider proximityCollider;

        //debug
        [SerializeField] private GameObject floorCircle; //Show the activation distance

        public void Initialise(float triggerRadius)
        {
            ToggleFocus(false);
            float floorCircleTextureScale = 0.2f * triggerRadius;
            floorCircle.transform.localScale = new Vector3(floorCircleTextureScale, floorCircleTextureScale, floorCircleTextureScale);
            proximityCollider.transform.localScale = new Vector3(triggerRadius * 2f, triggerRadius * 2f, triggerRadius * 2f);
        }

        public void AddSomePeople(Vector3 worldCenterPosition, PeopleGroupInfo peopleGroupInfo, GameObject personPrefab)
        {
            //this is a debug value that will be removed - it's an offset into the audio files
            //of the people in the group - useful when different groups are sharing an audio clip
            float groupOffset = Random.Range(0f, peopleGroupInfo.people[0].clip.length);

            foreach (PersonInfo person in peopleGroupInfo.people)
            {
                    AddPerson(  person,
                                new Vector3((person.positionCm.x / 100f) + worldCenterPosition.x,
                                            worldCenterPosition.y,
                                            (person.positionCm.y / 100f) + worldCenterPosition.z),
                                groupOffset);
            }
        }

        public void OnArCameraEnter()
        {
            this.ToggleFocus(true);
        }
        public void OnArCameraExit()
        {
            this.ToggleFocus(false);
        }

        public void ToggleFocus(bool inFocus)
        {
            InFocus = inFocus;

            foreach (PersonController person in People)
            {
                person.ToggleFocus(inFocus);
            }
        }

        private PersonController AddPerson(PersonInfo personInfo, Vector3 worldPosition, float groupOffset)
        {
            PersonController newPerson = GameObject.Instantiate(personInfo.prefab, transform).GetComponent<PersonController>();
            newPerson.transform.position = worldPosition;
            newPerson.Initialise();

            newPerson.transform.rotation = personInfo.rotation;
            newPerson.SetAudioClip(personInfo.clip, groupOffset);

            newPerson.umbrella.transform.localPosition = personInfo.umbrellaPositionCm;
            newPerson.umbrella.transform.localEulerAngles = personInfo.umbrellaLocalEuler;

            People.Add(newPerson);
            return newPerson;
        }
    }
}