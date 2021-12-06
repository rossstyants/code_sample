using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Nexus.NEPeopleGroups.Runtime
{
    public class PersonController : MonoBehaviour
    {
        #region Variables
        [ReadOnly] public bool InFocus;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private GameObject subtitles;
        [SerializeField, ReadOnly] private Camera _arCamera;
        public GameObject umbrella;
        #endregion

        #region Debug Variables
        private float conversationOffset;
        [SerializeField] private TextMeshPro volumeText;
        [SerializeField] private AnimationCurve exponentialCurve; //Temporary: For estimating perceived volume
        #endregion

        #region Public Methods
        public void Initialise()
        {
            ToggleFocus(false);
        }

        public void SetupSubtitles(Camera camera)
        {
            //subtitles perhaps should just be their own separate entity... that can attach to something
            //and be fed text
            _arCamera = camera;
        }

        public void SetAudioClip(AudioClip clip, float startOffset = 0f)
        {
            conversationOffset = startOffset;
            audioSource.clip = clip;
        }
        public void ToggleFocus(bool inFocus)
        {
            InFocus = inFocus;
            ToggleAudio(inFocus);
            subtitles.SetActive(inFocus && _arCamera != null);
            volumeText.gameObject.SetActive(inFocus);
        }
        #endregion

        #region Methods
        /// <summary>
        /// turn on/off the character's audio
        /// </summary>
        /// <param name="enable"></param>
        private void ToggleAudio(bool enable)
        {
            if (enable)
            {
                audioSource.time = conversationOffset;
                audioSource.Play();
            }
            else
            {
                audioSource.Stop();
            }
        }

        private void Update()
        {
            UpdateDebug();

            UpdateSubtitles();
        }

        private void UpdateSubtitles()
        {
            //the subtitles just face towards the ar camera

            if (InFocus && _arCamera != null)
            {
                //face subtitles towards camera
                var lookPos = subtitles.transform.position - _arCamera.transform.position;
                var rotation = Quaternion.LookRotation(lookPos);
                subtitles.transform.rotation = rotation;
            }
        }

        //This is all temporary debug functionality
        private void UpdateDebug()
        {
            //Keep the conversation flowing
            conversationOffset += Time.deltaTime;

            if (conversationOffset >= audioSource.clip.length)
                conversationOffset -= audioSource.clip.length;

            if (InFocus && _arCamera != null)
            {
                //Calculate (approximately) the perceived volume at this distance
                //and display

                AnimationCurve ac = audioSource.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
                Vector3 distVec = audioSource.transform.position - _arCamera.transform.position;
                float distance = distVec.magnitude;
                float logVol = ac.Evaluate(distance / audioSource.maxDistance);
                float perceivedVolume = exponentialCurve.Evaluate(logVol);

                volumeText.text = "d:" + distance.ToString("F2") + " v:" + perceivedVolume.ToString("F2") + " logv:" + logVol.ToString("F2");
            }
        }
        #endregion
    }
}
