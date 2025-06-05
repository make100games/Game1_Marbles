using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Blur
{
    public static IEnumerator ShowBlur(Volume blurVolume, DepthOfField blur)
    {
        if(blur != null)
        {
            blurVolume.gameObject.SetActive(true);
            var maxBlur = 300f;
            var noBlur = 120f;
            var durationInSeconds = 1;
            for (var timePassed = 0f; timePassed < durationInSeconds; timePassed += Time.deltaTime)
            {
                var factor = timePassed / durationInSeconds;
                blur.focalLength.Override(Mathf.Lerp(noBlur, maxBlur, factor));
                yield return null;
            }
        }
        else
        {
            Debug.Log("GameOverBlur DepthOfField effect was null. Unable to show blur effect");
        }
    }

    public static void HideBlur(Volume blurVolume)
    {
        blurVolume.gameObject.SetActive(false);
    }

    public static DepthOfField ObtainBlur(Volume blurVolume)
    {
        DepthOfField temp;
        if (blurVolume.profile.TryGet<DepthOfField>(out temp))
        {
            return temp;
        }
        return null;
    }
}
