using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nexus.positionswriter.runtime
{
    public class DrawVolumeCircles : MonoBehaviour
    {
        [SerializeField] private bool showCircles = true;
        [SerializeField] private AnimationCurve exponentialCurve; //For estimating perceived volume
        [SerializeField] private int numCircles = 20;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private float maxDrawDistance = 1.5f; //-1 means draw to audioSource max

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!showCircles)
                return;

            if (maxDrawDistance < 0f)
                maxDrawDistance = audioSource.maxDistance;
            Handles.color = Color.green;
            
            float radius = 0f;
            float radiusAdd = maxDrawDistance / (float)numCircles;
            for (int i = 0; i < numCircles; i++)
            {
                radius += radiusAdd;

                float volDistRatio = 1f;

                if (radius > audioSource.minDistance)
                {
                    //will not get the actual logarithmic value because that's not representative of what we hear anyway
                    volDistRatio = 1f - ((radius - audioSource.minDistance) / (audioSource.maxDistance - audioSource.minDistance));
                }

                AnimationCurve ac = audioSource.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
                //Vector3 distVec = audioSource.transform.position - _arCamera.transform.position;
                //float distance = distVec.magnitude;
                float logVol = ac.Evaluate(radius / audioSource.maxDistance);
                float perceivedVolume = exponentialCurve.Evaluate(logVol);

                Handles.color = Color.Lerp(Color.black, Color.green, perceivedVolume);
                Handles.DrawWireDisc(transform.position, new Vector3(0, 1, 0), radius);
                Handles.DrawWireDisc(transform.position, new Vector3(0, 1, 0), radius + 0.01f);
            }
        }
#endif
    }
}