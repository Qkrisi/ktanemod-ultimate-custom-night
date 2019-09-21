using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using KModkit;
using System.Linq;

public class qkUCNScript : MonoBehaviour {
	//general
	public KMAudio Audio;
	public Vector3 testvect;
	public bool solved = false;
	public Vector3 cambuttonPosition;
	public Vector3 ventbuttonPosition;
	public Vector3 ductbuttonPosition;
	public KMSelectable camButton;
	public KMSelectable ventButton;
	public KMSelectable ductButton;
	public GameObject currentMark;
	public GameObject generalCam;
	public GameObject generalVent;
	public GameObject generalDuct;
	public string[] Ignoreds;
	public KMBombInfo Bomb;
	int ticker = 0;
	int solveCount = 0;
	int lastCalcStage=0;
	List<String> solvedModules = new List<String>();
	private int stages;
	private int stageNumber = 0;
	bool TwitchPlaysActive;
	static int moduleIdCounter;
	int moduleId;
	private List<string> solvables;
	private float waitTime = 20f;
	private float waitTimeDuct = 10f;

	//cam
	public GameObject screen;
	public Material notSelected;
	public Material selected;
	public Material doorOpen;
	public Material doorClosed;
	public KMSelectable camButton1;
	public KMSelectable camButton2;
	public KMSelectable camButton3;
	public GameObject[] selections;
	public GameObject[] doorButtons;
	private int closedDoor = 0;
	private int currentPlace = 0;
	private int currentlyViewing = 1;
	public Material cam1No;
	public Material cam1Yes;
	public Material cam2No;
	public Material cam2Yes;
	public Material cam3No;
	public Material cam3Yes;

	//vent
	public List<Vector3> route1;
	public List<Vector3> route2;
	public List<Vector3> route3;
	public GameObject[] VentSnares;
	public GameObject VentCharacter1;
	public GameObject VentCharacter2;

	//duct
	public GameObject[] ductDoorButtons;
	public GameObject[] corridorButtons;
	public GameObject ductCharacter1;
	public GameObject ductCharacter2;
	public GameObject audioLure;
	public Material[] ductMaps;
	public GameObject ductMap;
	public List<Vector3> adj1;
	public List<Vector3> adj2;
	public List<Vector3> adj3;
	public List<Vector3> adj4;
	public List<Vector3> adj5;
	public List<Vector3> adj6;
	public List<Vector3> adj7;
	public List<Vector3> adj8;
	public List<Vector3> adj9;
	public List<Vector3> adj10;
	public List<Vector3> adj11;
	public List<Vector3> adj12;
	public List<Vector3> corridors;
	private bool rightOpen = false;
	private bool stayinplace = false;
	

	void Start(){
		moduleId=moduleIdCounter++;
		int rdm=UnityEngine.Random.Range(1,4);
		switch(rdm){
			case 1:
				StartCoroutine(MoveCharacter(VentCharacter1, route1, true, true));
				break;
			case 2:
				StartCoroutine(MoveCharacter(VentCharacter1, route2, true, true));
				break;
			case 3:
				StartCoroutine(MoveCharacter(VentCharacter1, route3, true, true));
				break;
		}
		
		camButton1.OnInteract += delegate(){
			Audio.PlaySoundAtTransform("Click", camButton1.transform);
			selections[0].GetComponent<Renderer>().material=selected;
			selections[1].GetComponent<Renderer>().material=notSelected;
			selections[2].GetComponent<Renderer>().material=notSelected;
			currentlyViewing=1;
			if(currentPlace==1){
				screen.GetComponent<Renderer>().material=cam1Yes;
			}
			else{
				screen.GetComponent<Renderer>().material=cam1No;
			}
			return false;
		};
		camButton2.OnInteract += delegate(){
			Audio.PlaySoundAtTransform("Click", camButton2.transform);
			selections[0].GetComponent<Renderer>().material=notSelected;
			selections[1].GetComponent<Renderer>().material=selected;
			selections[2].GetComponent<Renderer>().material=notSelected;
			currentlyViewing=2;
			if(currentPlace==2){
				screen.GetComponent<Renderer>().material=cam2Yes;
			}
			else{
				screen.GetComponent<Renderer>().material=cam2No;
			}
			return false;
		};
		camButton3.OnInteract += delegate(){
			Audio.PlaySoundAtTransform("Click", camButton3.transform);
			selections[0].GetComponent<Renderer>().material=notSelected;
			selections[1].GetComponent<Renderer>().material=notSelected;
			selections[2].GetComponent<Renderer>().material=selected;
			currentlyViewing=3;
			if(currentPlace==3){
				screen.GetComponent<Renderer>().material=cam3Yes;
			}
			else{
				screen.GetComponent<Renderer>().material=cam3No;
			}
			return false;
		};
		
		doorButtons[0].GetComponent<KMSelectable>().OnInteract += delegate(){
			if(closedDoor!=1){
				Audio.PlaySoundAtTransform("Door", doorButtons[0].transform);}
			else{
				Audio.PlaySoundAtTransform("Error", doorButtons[0].transform);
			}
			doorButtons[0].GetComponent<Renderer>().material=doorClosed;
			doorButtons[1].GetComponent<Renderer>().material=doorOpen;
			doorButtons[2].GetComponent<Renderer>().material=doorOpen;
			closedDoor=1;
			return false;
		};
		doorButtons[1].GetComponent<KMSelectable>().OnInteract += delegate(){
			if(closedDoor!=2){
				Audio.PlaySoundAtTransform("Door", doorButtons[1].transform);}
			else{
				Audio.PlaySoundAtTransform("Error", doorButtons[1].transform);
			}
			doorButtons[0].GetComponent<Renderer>().material=doorOpen;
			doorButtons[1].GetComponent<Renderer>().material=doorClosed;
			doorButtons[2].GetComponent<Renderer>().material=doorOpen;
			closedDoor=2;
			return false;
		};
		doorButtons[2].GetComponent<KMSelectable>().OnInteract += delegate(){
			if(closedDoor!=3){
				Audio.PlaySoundAtTransform("Door", doorButtons[2].transform);}
			else{
				Audio.PlaySoundAtTransform("Error", doorButtons[2].transform);
			}
			doorButtons[0].GetComponent<Renderer>().material=doorOpen;
			doorButtons[1].GetComponent<Renderer>().material=doorOpen;
			doorButtons[2].GetComponent<Renderer>().material=doorClosed;
			closedDoor=3;
			return false;
		};

		VentSnares[0].GetComponent<KMSelectable>().OnInteract += delegate(){
			Audio.PlaySoundAtTransform("Click", VentSnares[0].transform);
			closeVent(VentSnares[0]);
			return false;
		};
		VentSnares[1].GetComponent<KMSelectable>().OnInteract += delegate(){
			Audio.PlaySoundAtTransform("Click", VentSnares[1].transform);
			closeVent(VentSnares[1]);
			return false;
		};
		VentSnares[2].GetComponent<KMSelectable>().OnInteract += delegate(){
			Audio.PlaySoundAtTransform("Click", VentSnares[2].transform);
			closeVent(VentSnares[2]);
			return false;
		};

		ductDoorButtons[0].GetComponent<KMSelectable>().OnInteract += delegate(){
			Debug.LogFormat("Left");
			rightOpen=false;
			ductMap.GetComponent<Renderer>().material = ductMaps[0];
			Audio.PlaySoundAtTransform("Click", ductDoorButtons[0].transform);
			return false;
		};
		ductDoorButtons[1].GetComponent<KMSelectable>().OnInteract += delegate(){
			Debug.LogFormat("Right");
			rightOpen=true;
			ductMap.GetComponent<Renderer>().material = ductMaps[1];
			Audio.PlaySoundAtTransform("Click", ductDoorButtons[1].transform);
			return false;
		};

		corridorButtons[0].GetComponent<KMSelectable>().OnInteract += delegate(){
			setLure(corridorButtons[0]);
			return false;	
		};
		corridorButtons[1].GetComponent<KMSelectable>().OnInteract += delegate(){
			setLure(corridorButtons[1]);
			return false;	
		};
		corridorButtons[2].GetComponent<KMSelectable>().OnInteract += delegate(){
			setLure(corridorButtons[2]);
			return false;	
		};
		corridorButtons[3].GetComponent<KMSelectable>().OnInteract += delegate(){
			setLure(corridorButtons[3]);
			return false;	
		};
		corridorButtons[4].GetComponent<KMSelectable>().OnInteract += delegate(){
			setLure(corridorButtons[4]);
			return false;	
		};
		corridorButtons[5].GetComponent<KMSelectable>().OnInteract += delegate(){
			setLure(corridorButtons[5]);
			return false;	
		};
		corridorButtons[6].GetComponent<KMSelectable>().OnInteract += delegate(){
			setLure(corridorButtons[6]);
			return false;	
		};
		corridorButtons[7].GetComponent<KMSelectable>().OnInteract += delegate(){
			setLure(corridorButtons[7]);
			return false;	
		};
		corridorButtons[8].GetComponent<KMSelectable>().OnInteract += delegate(){
			setLure(corridorButtons[8]);
			return false;	
		};
		corridorButtons[9].GetComponent<KMSelectable>().OnInteract += delegate(){
			setLure(corridorButtons[9]);
			return false;	
		};
		corridorButtons[10].GetComponent<KMSelectable>().OnInteract += delegate(){
			setLure(corridorButtons[10]);
			return false;	
		};
		corridorButtons[11].GetComponent<KMSelectable>().OnInteract += delegate(){
			setLure(corridorButtons[11]);
			return false;	
		};
		
		camButton.OnInteract += delegate(){
			Audio.PlaySoundAtTransform("Click", camButton.transform);
			generalCam.transform.localScale=new Vector3(1f,1f,1f);
			generalVent.transform.localScale=new Vector3(0f,0f,0f);
			generalDuct.transform.localScale=new Vector3(0f,0f,0f);
			currentMark.transform.localPosition=cambuttonPosition;
			return false;
		};
		ventButton.OnInteract += delegate(){
			Audio.PlaySoundAtTransform("Click", ventButton.transform);
			generalCam.transform.localScale=new Vector3(0f,0f,0f);
			generalVent.transform.localScale=new Vector3(1f,1f,1f);
			generalDuct.transform.localScale=new Vector3(0f,0f,0f);
			currentMark.transform.localPosition=ventbuttonPosition;
			return false;
		};
		ductButton.OnInteract += delegate(){
			Audio.PlaySoundAtTransform("Click", ductButton.transform);
			generalCam.transform.localScale=new Vector3(0f,0f,0f);
			generalVent.transform.localScale=new Vector3(0f,0f,0f);
			generalDuct.transform.localScale=new Vector3(1f,1f,1f);
			currentMark.transform.localPosition=ductbuttonPosition;
			return false;
		};



		for(int i = 0;i<12;i++){
			//Debug.LogFormat(i.ToString());
			corridorButtons[i].transform.localPosition=new Vector3(corridors[i].x, 0.03276379f, corridors[i].z);
		}
		//Debug.LogFormat(getIndex(corridors, testvect).ToString());
		
		StartCoroutine(startDucts());
		StartCoroutine(Checkforstage());
		NewStage(true);
		
	}

	void closeVent(GameObject Snare){
		//Debug.LogFormat("Got it");
		VentSnares[0].GetComponent<Renderer>().enabled=false;
		VentSnares[1].GetComponent<Renderer>().enabled=false;
		VentSnares[2].GetComponent<Renderer>().enabled=false;
		Snare.GetComponent<Renderer>().enabled=true;
	}

	IEnumerator MoveCharacter(GameObject ch, List<Vector3> route, bool firstchar, bool fullfirstchar){
		if(!firstchar){
			yield return new WaitForSeconds(.5f);
		}
		
		int index=0;
		int newcharindex = 0;
		while(true){
			yield return null;
			ch.transform.localPosition=route[index];
			yield return new WaitForSeconds(waitTime);
			if((route==route1 && index==16) || (route==route2 && index==15) || (route==route3 && index==6)){
				if(!solved){
				GetComponent<KMBombModule>().HandleStrike();}
				index=-1;
				if(firstchar){
					int rdm=UnityEngine.Random.Range(1,4);
					switch(rdm){
					case 1:
						StartCoroutine(MoveCharacter(VentCharacter1, route1, true, false));
						break;
					case 2:
						StartCoroutine(MoveCharacter(VentCharacter1, route2, true, false));
						break;
					case 3:
						StartCoroutine(MoveCharacter(VentCharacter1, route3, true, false));
						break;
					}
					yield break;
				}
			}
			if(route==route1){
			if(index==14 || index==15){
				if((route==route1 && VentSnares[0].GetComponent<Renderer>().enabled) || (route==route2 && VentSnares[1].GetComponent<Renderer>().enabled) || (route==route3 && VentSnares[2].GetComponent<Renderer>().enabled)){
					Audio.PlaySoundAtTransform("Bang", ch.transform);
					index=-1;
					if(firstchar){
					int rdm=UnityEngine.Random.Range(1,4);
					switch(rdm){
					case 1:
						StartCoroutine(MoveCharacter(VentCharacter1, route1, true, false));
						break;
					case 2:
						StartCoroutine(MoveCharacter(VentCharacter1, route2, true, false));
						break;
					case 3:
						StartCoroutine(MoveCharacter(VentCharacter1, route3, true, false));
						break;
					}
					yield break;
				}
				}
			}}
			else{
				if(route==route2){
					if(index==11 || index==12){
				if((route==route1 && VentSnares[0].GetComponent<Renderer>().enabled) || (route==route2 && VentSnares[1].GetComponent<Renderer>().enabled) || (route==route3 && VentSnares[2].GetComponent<Renderer>().enabled)){
					Audio.PlaySoundAtTransform("Bang", ch.transform);
					index=-1;
					if(firstchar){
					int rdm=UnityEngine.Random.Range(1,4);
					switch(rdm){
					case 1:
						StartCoroutine(MoveCharacter(VentCharacter1, route1, true, false));
						break;
					case 2:
						StartCoroutine(MoveCharacter(VentCharacter1, route2, true, false));
						break;
					case 3:
						StartCoroutine(MoveCharacter(VentCharacter1, route3, true, false));
						break;
					}
					yield break;
				}
				}
			}
				}
				else{
					if(index==4 || index==5){
						if((route==route1 && VentSnares[0].GetComponent<Renderer>().enabled) || (route==route2 && VentSnares[1].GetComponent<Renderer>().enabled) || (route==route3 && VentSnares[2].GetComponent<Renderer>().enabled)){
							Audio.PlaySoundAtTransform("Bang", ch.transform);
							index=-1;
							if(firstchar){
					int rdm=UnityEngine.Random.Range(1,4);
					switch(rdm){
					case 1:
						StartCoroutine(MoveCharacter(VentCharacter1, route1, true, false));
						break;
					case 2:
						StartCoroutine(MoveCharacter(VentCharacter1, route2, true, false));
						break;
					case 3:
						StartCoroutine(MoveCharacter(VentCharacter1, route3, true, false));
						break;
					}
					yield break;
				}
				}
			}
				}
			}
			index++;
			newcharindex++;
			if(newcharindex==3 && fullfirstchar){
				StartCoroutine(MoveCharacter(VentCharacter2, route1, false, false));
			}
		}
		yield break;
	}

	void setLure(GameObject button){
		Audio.PlaySoundAtTransform("Click", button.transform);
		audioLure.transform.localPosition = new Vector3(button.transform.localPosition.x, 0.03576379f, button.transform.localPosition.z);
		audioLure.GetComponent<Renderer>().enabled=true;
	}

	IEnumerator ductCharacter(GameObject charact){
		Vector3 startingpos = charact.transform.localPosition;
		List<Vector3> templist = new List<Vector3>();
		while(true){
			yield return new WaitForSeconds(waitTimeDuct);
			Debug.LogFormat(getIndex(corridors, charact.transform.localPosition).ToString());
			switch(getIndex(corridors, charact.transform.localPosition)){
				case 0:
					templist=adj1;
					break;
				case 1:
					templist=adj2;
					break;
				case 2:
					templist=adj3;
					break;
				case 3:
					templist=adj4;
					break;
				case 4:
					templist=adj5;
					break;
				case 5:
					templist=adj6;
					break;
				case 6:
					templist=adj7;
					break;
				case 7:
					templist=adj8;
					break;
				case 8:
					templist=adj9;
					break;
				case 9:
					templist=adj10;
					break;
				case 10:
					templist=adj11;
					break;
				case 11:
					templist=adj12;
					break;
			}
			if(charact.transform.localPosition==new Vector3(audioLure.transform.localPosition.x, 0.03576379f, audioLure.transform.localPosition.z)){
				int rnd = UnityEngine.Random.Range(0,2);
				if(rnd==1){
					stayinplace=true;
				}
				else{
					stayinplace=false;
				}
			}
			else{
				stayinplace=false;
			}
			if(!stayinplace){
			if(templist.Contains(new Vector3(audioLure.transform.localPosition.x, 0.03576379f, audioLure.transform.localPosition.z))){
					int currentind = 1;
					while(currentind<templist.Count/2+2){
						templist.Add(new Vector3(audioLure.transform.localPosition.x, 0.03576379f, audioLure.transform.localPosition.z));
						currentind++;
					}
				}
				int rng = UnityEngine.Random.Range(0, templist.Count);
				charact.transform.localPosition=templist[rng];
				if((charact.transform.localPosition==new Vector3(-0.0175f, 0.03576379f, -0.0451f) && !rightOpen && !solved) || (charact.transform.localPosition==new Vector3(0.0166f, 0.03576379f, -0.0478f) && rightOpen && !solved)){
					GetComponent<KMBombModule>().HandleStrike();
					charact.transform.localPosition=startingpos;
				}
				else{
					if(solved){charact.transform.localPosition=startingpos;}
					else{
					if(charact.transform.localPosition==new Vector3(-0.0175f, 0.03576379f, -0.0451f)){
						charact.transform.localPosition=new Vector3(-0.0182f, 0.03576379f, -0.0331f);
					}
					else{
						if(charact.transform.localPosition==new Vector3(0.0166f, 0.03576379f, -0.0478f)){
						charact.transform.localPosition=new Vector3(0.0166f, 0.03576379f, -0.0331f);
					}
					}
				}
				}
			}
		}
	}

	int getIndex(List<Vector3> baselist, Vector3 search){
		for(int i = 0;i<baselist.Count;i++){
			if(baselist[i]==search){
				return i;
			}
		}
		return -1;
	}

	IEnumerator startDucts(){
		StartCoroutine(ductCharacter(ductCharacter1));
		yield return new WaitForSeconds(3f);
		StartCoroutine(ductCharacter(ductCharacter2));
		yield break;
	}

	IEnumerator Checkforstage(){
		while(true){
			yield return new WaitForSeconds(1f);
			if(TwitchPlaysActive){
			waitTime=20f;
			waitTimeDuct=10f;
		}
		else{
			waitTime=10f;
			waitTimeDuct=5f;
		}
		solvables=Bomb.GetSolvableModuleNames();
		solvables.RemoveAll(item => item == "Ultimate Custom Night");
		if(solvables.Count()-Bomb.GetSolvedModuleNames().Count()<=0){
			solved=true;
			GetComponent<KMBombModule>().HandlePass();
		}
		if(Bomb.GetSolvedModuleNames().Count()>lastCalcStage){
			lastCalcStage=Bomb.GetSolvedModuleNames().Count();
			NewStage(false);
		}
		}
	 }

	void NewStage(bool first){
		if(closedDoor!=currentPlace){
			if(!solved && !first){
				GetComponent<KMBombModule>().HandleStrike();
				currentPlace=UnityEngine.Random.Range(1,4);
			}
		}
		else{
			currentPlace=UnityEngine.Random.Range(1,4);
		}
		switch(currentlyViewing){
				case 1:
					screen.GetComponent<Renderer>().material=cam1No;
					break;
				case 2:
					screen.GetComponent<Renderer>().material=cam2No;
					break;
				case 3:
					screen.GetComponent<Renderer>().material=cam3No;
					break;
			}
		if(currentPlace==currentlyViewing){
			switch(currentlyViewing){
				case 1:
					screen.GetComponent<Renderer>().material=cam1Yes;
					break;
				case 2:
					screen.GetComponent<Renderer>().material=cam2Yes;
					break;
				case 3:
					screen.GetComponent<Renderer>().material=cam3Yes;
					break;
			}
		}
	}

	void CheckAutoSolve(){
		stages = Bomb.GetSolvableModuleNames().Where(x => !Ignoreds.Contains(x)).Count();
		if(stages==0){
			solved=true;
			StartCoroutine(AutoSolve());
		}
	}

	private IEnumerator AutoSolve(){
		 yield return new WaitForSeconds(2f);
		 Debug.LogFormat("[Ultimate Custom Night #{0}] There are no modules that is not ignored by Forget Perspective. Auto-solving module.", moduleId);
		 GetComponent<KMBombModule>().HandlePass();
		 yield break;
	 }
	
	public string TwitchHelpMessage = "Use '!{0} cycle' to cycle cameras, vent and duct! Use '!{0} cyclecams' to cycle the cameras only! Use '!{0} cameras', '!{0} vent' and '{0} duct' to change the view! Use '!{0} cam1' '!{0} cam2' and '!{0} cam3' to see cameras manualy! Use '!{0} closedoor #' to close a door! For ex. '!{0} closedoor 1' will close the door for cam 1. Use '!{0} snare #' to snare a vent route! 1 = BL; 2 = T; 3 = BR;! Use '!{0} closeright' to close the right duct door and '!{0} closeleft' to close the left duct door! Use '!{0} setlure #' to set the lure to a corner! Corners are numbered from 1 to 12.";
	IEnumerator ProcessTwitchCommand(string command){
		yield return null;
		string commandl = "";
		int tried = 0;
		command=command.ToUpper();
		if(command.Equals("CYCLE")){
			camButton.OnInteract();
			camButton1.OnInteract();
			yield return new WaitForSeconds(1.5f);
			camButton2.OnInteract();
			yield return new WaitForSeconds(1.5f);
			camButton3.OnInteract();
			yield return new WaitForSeconds(1.5f);
			camButton1.OnInteract();
			ventButton.OnInteract();
			yield return new WaitForSeconds(1.5f);
			ductButton.OnInteract();
			yield return new WaitForSeconds(1.5f);
			camButton.OnInteract();
			yield break;
		}
		if(command.Equals("CYCLECAMS")){
			camButton.OnInteract();
			camButton1.OnInteract();
			yield return new WaitForSeconds(1.5f);
			camButton2.OnInteract();
			yield return new WaitForSeconds(1.5f);
			camButton3.OnInteract();
			yield return new WaitForSeconds(1.5f);
			camButton1.OnInteract();
			yield break;
		}
		if(command.Contains("CLOSEDOOR")){
			commandl=command.Replace("CLOSEDOOR", "");
			if(int.TryParse(commandl, out tried)){
				tried=int.Parse(commandl);
				tried=tried-1;
				if(tried>=0 && tried<=2){
					camButton.OnInteract();
					doorButtons[tried].GetComponent<KMSelectable>().OnInteract();
					yield break;
				}
				else{
					yield return "sendtochaterror Number not valid!";
					yield break;
				}
			}
				else{
					yield return "sendtochaterror Number not valid!";
					yield break;
				}
		}
		if(command.Equals("CAMERAS")){
			camButton.OnInteract();
			yield break;
		}
		if(command.Equals("VENT")){
			ventButton.OnInteract();
			yield break;
		}
		if(command.Equals("DUCT")){
			ductButton.OnInteract();
			yield break;
		}
		if(command.Contains("SNARE")){
			commandl=command.Replace("SNARE", "");
			if(int.TryParse(commandl, out tried)){
				tried=int.Parse(commandl);
				tried=tried-1;
				if(tried>=0 && tried<=2){
					bool changed = false;
					if(tried==0){tried=2;
					changed=true;};
					if(tried==2 && !changed){tried=0;};
					ventButton.OnInteract();
					VentSnares[tried].GetComponent<KMSelectable>().OnInteract();
					yield break;
				}
				else{
					yield return "sendtochaterror Number not valid!";
					yield break;
				}
			}
				else{
					yield return "sendtochaterror Number not valid!";
					yield break;
				}
		}
		if(command.Contains("SETLURE")){
			commandl=command.Replace("SETLURE", "");
			if(int.TryParse(commandl, out tried)){
				tried=int.Parse(commandl);
				tried=tried-1;
				if(tried>=0 && tried<=11){
					ductButton.OnInteract();
					corridorButtons[tried].GetComponent<KMSelectable>().OnInteract();
					yield break;
				}
				else{
					yield return "sendtochaterror Number not valid!";
					yield break;
				}
			}
				else{
					yield return "sendtochaterror Number not valid!";
					yield break;
				}
		}
		if(command.Equals("CLOSERIGHT")){
			ductButton.OnInteract();
			ductDoorButtons[1].GetComponent<KMSelectable>().OnInteract();
			yield break;
		}
		if(command.Equals("CLOSELEFT")){
			ductButton.OnInteract();
			ductDoorButtons[0].GetComponent<KMSelectable>().OnInteract();
			yield break;
		}
		if(command.Equals("CAM1")){
			camButton.OnInteract();
			camButton1.OnInteract();
			yield break;
		}
		if(command.Equals("CAM2")){
			camButton.OnInteract();
			camButton2.OnInteract();
			yield break;
		}
		if(command.Equals("CAM3")){
			camButton.OnInteract();
			camButton3.OnInteract();
			yield break;
		}
		yield break;
	}
}