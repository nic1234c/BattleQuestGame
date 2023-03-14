using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClips : MonoBehaviour
{
    [SerializeField] List<AudioClip> clips;
     

	public AudioClip GetClip(string clip) {
		AudioClip retVal = null;
			switch (clip)
			{
				case "Attack":
					retVal = clips[0];
					break;
				case "Hit":
					retVal = clips[1];
					break;
				case "Hover":
					retVal = clips[2];
					break;
				case "Move":
					retVal = clips[3];
					break;
				case "Death":
					retVal = clips[4];
					break;
				case "Press":
					retVal = clips[5];
					break;
				case "Trade":
					retVal = clips[6];
					break;
				default:
					break;
			}
		return retVal;
	}                
}
