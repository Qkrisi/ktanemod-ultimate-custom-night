using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossModuleTestScript : MonoBehaviour {

	void Start () {
		GetComponent<KMSelectable>().OnInteract += delegate(){
			GetComponent<KMBombModule>().HandlePass();
			return false;
		};
	}
	
	public string TwitchHelpMessage = "Use '!{0} <insert literally anything here>' to solve the module!";
	IEnumerator ProcessTwitchCommand(string thisisnotneededbythemodule){
		yield return null;
		GetComponent<KMBombModule>().HandlePass();
	}
}
