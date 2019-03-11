/*===============================================================================
Copyright (c) 2017 PTC Inc. All Rights Reserved.
 
Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
===============================================================================*/

using System.Collections;
using UnityEngine; 
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class ObjectTrackableEventHandler : DefaultTrackableEventHandler
{
    public GameObject rocket;
    public GameObject warningPlane;

    #region PROTECTED_METHODS
    protected override void OnTrackingLost()
	{
        print("Lost");
        rocket.SetActive(false);
        warningPlane.SetActive(false);
        base.OnTrackingLost();
	}

	protected override void OnTrackingFound()
	{
        rocket.SetActive(true);
        warningPlane.SetActive(true);
        PlayableDirector pd = rocket.GetComponent<PlayableDirector>();        
        pd.Play();

        print("Found");
        base.OnTrackingFound();
	}

    #endregion // PROTECTED_METHODS
}
