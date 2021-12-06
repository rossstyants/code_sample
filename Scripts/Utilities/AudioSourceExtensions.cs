using System;
using System.Collections;

namespace UnityEngine
{
    public static class AudioSourceExtensions
    {

        ///the problem with this approach is that if you start a fadeIn and then quickly a fadeOut - the two processes will keep running independently
        ///without knowledge of each other. For better results may need to make a script that inherits from AudioSource and adds the fade funcs -
        ///so that if you call a fadeIn directly after a fadeOut - the fadeOut will not, for example - .stop() the audioClip halfway through the fade in.

        public static IEnumerator FadeVolume(this AudioSource audioSource, float to, float duration, bool delay = false, float delayDuration = 0f, Action callback = null)
        {
            if (delay)
            {
                yield return new WaitForSeconds(delayDuration);
            }

            float from = audioSource.volume;

            for (float t = 0f; t < duration; t += Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp(a: from, b: to, t: t / duration);
                yield return null;
            }

            audioSource.volume = to;

            callback?.Invoke();
        }
    }
}