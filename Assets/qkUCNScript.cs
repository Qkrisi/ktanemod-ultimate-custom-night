using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public partial class qkUCNScript : MonoBehaviour {
    #region Variables
    #region General
    public KMAudio Audio;
    public bool solved = false;
    public Vector3 cambuttonPosition;
    public Vector3 ventbuttonPosition;
    public Vector3 ductbuttonPosition;
    public Vector3 perksbuttonPosition;
    public KMSelectable camButton;
    public KMSelectable ventButton;
    public KMSelectable ductButton;
    public KMSelectable perksButton;
    public GameObject currentMark;
    public GameObject generalCam;
    public GameObject generalVent;
    public GameObject generalDuct;
    public GameObject generalPerks;
    public string[] ignoredModules;
    public KMBombInfo Bomb;
    int ticker = 0;
    int solveCount = 0;
    int lastCalcStage = 0;
    List<String> solvedModules = new List<String>();
    private int stages;
    private int stageNumber = 0;
    bool TwitchPlaysActive;
    bool ZenModeActive;
    static int moduleIdCounter;
    int moduleId;
    private int solvables;
    private float waitTime = 20f;
    private float waitTimeDuct = 10f;
    private readonly bool devMode = false;
    private readonly BindingFlags MainFlags = BindingFlags.Public | BindingFlags.Instance;
    private bool _VisibilityOverride = false;
    private bool VisibilityOverride
    {
        get
        {
            return _VisibilityOverride;
        }
        set
        {
            foreach (string _name in new string[] { "PowerManagement", "Music" }) transform.Find(_name).localScale = value ? new Vector3(1, 1, 1) : new Vector3(0, 0, 0);
            _VisibilityOverride = value;
        }
    }
    private readonly Dictionary<string, string[]> HelpCommands = new Dictionary<string, string[]>()
    {
        {
            "help all",
            new string[]
            {
                "General help: Use '!{0} cycle' to cycle cameras, vent and duct and '!{0} [cameras/vent/duct]' to set the screen to a specific place!",
                "Office input help: Use '!{0} toggledoor [front/side]' to toggle the front or side vent door!",
                "Camera help: Use '!{0} cyclecams' to cycle all cameras, '!{0} [cam1/cam2/cam3]' to view a specific camera, '!{0} closedoor #' to close the door of the corresponding camera!",
                "Vent help: Use '!{0} snare #' to snare a route! 1 = BL; 2 = T; 3 = BR",
                "Duct help: Use '!{0} open[right/left]' to open the specified duct door and '!{0} setlure #' to set the audio lure to the specified corner (1-12)!",
                "Perks help: Use '!{0} perk [perk name or index]' to turn on a perk. [perk name] can be 'global music box' (or 'gmb' for short) or 'off'; indexes are numbered 1-2 from top to bottom.",
                "Music help: Use '!{0} changemusic' to change the music!"
            }
        },
        {
            "help general",
            new string[]
            {
                "General help: Use '!{0} cycle' to cycle cameras, vent and duct and '!{0} [cameras/vent/duct]' to set the screen to a specific place!"
            }
        },
        {
            "help office",
            new string[]
            {
                "Office input help: Use '!{0} toggledoor [front/side]' to toggle the front or side vent door!"
            }
        },
        {
            "help camera",
            new string[]
            {
                "Camera help: Use '!{0} cyclecams' to cycle all cameras, '!{0} [cam1/cam2/cam3]' to view a specific camera, '!{0} closedoor #' to close the door of the corresponding camera!"
            }
        },
        {
            "help vent",
            new string[]
            {
                "Vent help: Use '!{0} snare #' to snare a route! 1 = BL; 2 = T; 3 = BR"
            }
        },
        {
            "help duct",
            new string[]
            {
                "Duct help: Use '!{0} open[right/left]' to open the specified duct door and '!{0} setlure #' to set the audio lure to the specified corner (1-12)!"
            }
        },
        {
            "help perks",
            new string[]
            {
                "Perks help: Use '!{0} perk [perk name or index]' to turn on a perk. [perk name] can be 'global music box' (or 'gmb' for short) or 'off'; indexes are numbered 1-2 from top to bottom."
            }
        },
        {
            "help music",
            new string[]
            {
                "Music help: Use '!{0} changemusic' to change the music!"
            }
        }
    };
    #endregion

    #region Doors
    private bool RightVentClosed = false;
    private bool FrontVentClosed = false;
    private bool LeftDoorClosed = false;
    private bool RightDoorClosed = false;
    #endregion

    #region PowerManagement
    public TextMesh PowerText;
    public GameObject UsageBar;
    public Material WhiteMaterial;
    public Material OrangeMaterial;
    private const int MinUsage = 0;
    private const int MaxUsage = 5;
    private readonly float?[] UsageTimes = new float?[]
    {
        null,
        5,
        4,
        3,
        2,
        1
    };
    private int _CurrentUsage = 0;
    private int CurrentUsage
    {
        get
        {
            return _CurrentUsage;
        }
        set
        {
            if (value <= MaxUsage && value >= MinUsage)
            {
                _CurrentUsage = value;
                SetUsage(CurrentUsage);
            }
        }
    }
    private float? WaitTime
    {
        get
        {
            return UsageTimes[CurrentUsage];
        }
    }
    private int _CurrentPower = 100;
    private int CurrentPower
    {
        get
        {
            return _CurrentPower;
        }
        set
        {
            _CurrentPower = value;
            PowerText.text = String.Format("Power: {0}%", CurrentPower);
        }
    }
    #endregion

    #region Cams
    public GameObject screen;
	public Material notSelected;
	public Material selected;
	public Material doorOpen;
	public Material doorClosed;
	public KMSelectable camButton1;
	public KMSelectable camButton2;
	public KMSelectable camButton3;
	public GameObject[] selections;		//The green boxes behind the camera buttons
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
	#endregion

	#region Vent
	public List<Vector3> route1;		//Right
	public List<Vector3> route2;		//Middle
	public List<Vector3> route3;		//Left
	public GameObject[] VentSnares;
	public GameObject VentCharacter1;		//Mangle
	public GameObject VentCharacter2;		//Withered Chica
    private List<bool> _go = new List<bool>() { true, true };
    #endregion

    #region Duct
    public GameObject[] ductDoorButtons;
	public GameObject[] corridorButtons;
	public GameObject ductCharacter1;		//Pigpatch
	public GameObject ductCharacter2;		//Mr. Hippo
	public GameObject audioLure;
	public Material[] ductMaps;
	public GameObject ductMap;
	public List<Vector3> adj1;				//positions adjacent to these buttons
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
	public List<Vector3> corridors;			//Clickable corners
	private bool rightOpen = false;			//If true then right duct door is open otherwise left
	private bool stayinplace = false;       //If true the character won't move on the next tick
    #endregion

    #region Perks
    public Vector3 GlobalMusicBoxPosition;
    public Vector3 OffPosition;
    public GameObject PerkMarker;
    public KMSelectable GlobalMusicBoxButton;
    public KMSelectable OffButton;
    private bool globalMusicBox = false;
    private bool perksOff = true;
    #endregion

    #region Music
    public KMSelectable ChangeMusicButton;
    #endregion
    #endregion

    private bool canGo(int index)
    {
        if (!_go[0] && !_go[1]) _go[index == 0 ? 1 : 0] = true;
        return index == 0 ? _go[1] : _go[0];
    }

    void Start(){
		moduleId=++moduleIdCounter;
		
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
            generalPerks.transform.localScale = new Vector3(0f, 0f, 0f);
			currentMark.transform.localPosition=cambuttonPosition;
            VisibilityOverride = true;
			return false;
		};
		ventButton.OnInteract += delegate(){
			Audio.PlaySoundAtTransform("Click", ventButton.transform);
			generalCam.transform.localScale=new Vector3(0f,0f,0f);
			generalVent.transform.localScale=new Vector3(1f,1f,1f);
			generalDuct.transform.localScale=new Vector3(0f,0f,0f);
            generalPerks.transform.localScale = new Vector3(0f, 0f, 0f);
			currentMark.transform.localPosition=ventbuttonPosition;
            VisibilityOverride = true;
			return false;
		};
		ductButton.OnInteract += delegate(){
			Audio.PlaySoundAtTransform("Click", ductButton.transform);
			generalCam.transform.localScale=new Vector3(0f,0f,0f);
			generalVent.transform.localScale=new Vector3(0f,0f,0f);
			generalDuct.transform.localScale=new Vector3(1f,1f,1f);
            generalPerks.transform.localScale = new Vector3(0f, 0f, 0f);
			currentMark.transform.localPosition=ductbuttonPosition;
            VisibilityOverride = false;
			return false;
		};
        perksButton.OnInteract += delegate (){
            Audio.PlaySoundAtTransform("Click", perksButton.transform);
            generalCam.transform.localScale = new Vector3(0f, 0f, 0f);
            generalVent.transform.localScale = new Vector3(0f, 0f, 0f);
            generalDuct.transform.localScale = new Vector3(0f, 0f, 0f);
            generalPerks.transform.localScale = new Vector3(1f, 1f, 1f);
            currentMark.transform.localPosition = perksbuttonPosition;
            VisibilityOverride = true;
            return false;
        };

        GlobalMusicBoxButton.OnInteract += () => TogglePerks(ref globalMusicBox, GlobalMusicBoxPosition);
        OffButton.OnInteract += () => TogglePerks(ref perksOff, OffPosition, -1);

        ChangeMusicButton.OnInteract += delegate() {
            if (!ChicaAttack) Strike("Chica");
            else
            {
                ChicaAttack = false;
                StopCoroutine(WaitForChicaPress());
                StartCoroutine(HandleChica());
            }
            return false;
        };

        for (int i = 0;i<12;i++){
			corridorButtons[i].transform.localPosition=new Vector3(corridors[i].x, 0.03276379f, corridors[i].z);
		}
		
		ignoredModules = GetComponent<KMBossModule>().GetIgnoredModules("Ultimate Custom Night", new string[]{
				"Ultimate Custom Night",
				"The Time Keeper"
            });

        StartCoroutine(startVent());
		StartCoroutine(startDucts());
        StartCoroutine(HandleAfton());
        StartCoroutine(HandleChica());
        StartCoroutine(HandleInputs());
        StartCoroutine(HandlePower());
		NewStage(true);	
	}

    private bool TogglePerks(ref bool NewPerk, Vector3 NewPosition, int increment = 1)
    {
        Audio.PlaySoundAtTransform("Click", transform);
        if (!NewPerk)
        {
            globalMusicBox = false;
            perksOff = false;
            NewPerk = true;
            PerkMarker.transform.localPosition = NewPosition;
            CurrentUsage += increment;
        }
        return false;
    }

    private void SetUsage(int state)
    {
        var currentScale = UsageBar.transform.localScale;
        currentScale.x = (float)state / 100;
        UsageBar.transform.localScale = currentScale;
        var currentPosition = UsageBar.transform.localPosition;
        currentPosition.x = 0.0138f + (state * 0.005f);
        UsageBar.transform.localPosition = currentPosition;
        UsageBar.GetComponent<Renderer>().material = state >= 4 ? OrangeMaterial : WhiteMaterial;
    }

    void closeVent(GameObject Snare){
		VentSnares[0].GetComponent<Renderer>().enabled=false;
		VentSnares[1].GetComponent<Renderer>().enabled=false;
		VentSnares[2].GetComponent<Renderer>().enabled=false;
		Snare.GetComponent<Renderer>().enabled=true;
	}

    private IEnumerator HandleInputs()
    {
        while(true)
        {
            yield return null;
            if (Input.GetKeyDown(KeyCode.F)) ToggleDoor(ref RightVentClosed);
            if (Input.GetKeyDown(KeyCode.W)) ToggleDoor(ref FrontVentClosed);
            /*if (Input.GetKeyDown(KeyCode.A)) ToggleDoor(ref LeftDoorClosed);
            if (Input.GetKeyDown(KeyCode.D)) ToggleDoor(ref RightDoorClosed);*/
        }
    }

    private void ToggleDoor(ref bool door)
    {
        Audio.PlaySoundAtTransform("Door", transform);
        door = !door;
        CurrentUsage += door ? 1 : -1;
    }

    IEnumerator startVent()
    {
        yield return new WaitForSeconds(30f);
        int rdm = UnityEngine.Random.Range(1, 4);
        switch (rdm)
        {
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
    }

    IEnumerator MoveCharacter(GameObject ch, List<Vector3> route, bool firstchar, bool fullfirstchar){
		if(!firstchar){
			yield return new WaitForSeconds(.5f);
		}
		
		int index=0;
		int newcharindex = 0;
        int setIndex = firstchar ? 0 : 1;
        StartCoroutine(HandleFlash(setIndex));
		while(true){
			yield return null;
            yield return new WaitUntil(() => canGo(setIndex));
			ch.transform.localPosition=route[index];
			yield return new WaitForSeconds(waitTime);
			if((route==route1 && index==16) || (route==route2 && index==15) || (route==route3 && index==6)){
                if (!FrontVentClosed) Strike("Vent");
                else { Audio.PlaySoundAtTransform("Bang", transform); }
                _go[setIndex] = true;
                index =-1;
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
			if(index==13 || index==14 || index==15){
                    _go[setIndex] = false;
				if((route==route1 && VentSnares[0].GetComponent<Renderer>().enabled) || (route==route2 && VentSnares[1].GetComponent<Renderer>().enabled) || (route==route3 && VentSnares[2].GetComponent<Renderer>().enabled)){
					Audio.PlaySoundAtTransform("Bang", ch.transform);
                        _go[setIndex] = true;
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
                if (index > 14) _go[setIndex] = true;
            }
			else{
				if(route==route2){
					if(index==11 || index==12 || index==13){
                        _go[setIndex] = false;
				if((route==route1 && VentSnares[0].GetComponent<Renderer>().enabled) || (route==route2 && VentSnares[1].GetComponent<Renderer>().enabled) || (route==route3 && VentSnares[2].GetComponent<Renderer>().enabled)){
					Audio.PlaySoundAtTransform("Bang", ch.transform);
                            _go[setIndex] = true;
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
                        if (index > 13) _go[setIndex] = true;
			}
				}
				else{
					if(index==4 || index==5){
                        _go[setIndex] = false;
						if((route==route1 && VentSnares[0].GetComponent<Renderer>().enabled) || (route==route2 && VentSnares[1].GetComponent<Renderer>().enabled) || (route==route3 && VentSnares[2].GetComponent<Renderer>().enabled)){
							Audio.PlaySoundAtTransform("Bang", ch.transform);
                            _go[setIndex] = true;
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
                    _go[setIndex] = true;
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

    private IEnumerator HandleFlash(int setIndex)
    {
        GameObject Character = setIndex == 0 ? VentCharacter1 : VentCharacter2;
        while (!solved)
        {
            yield return null;
            if (!canGo(setIndex))
            {
                Character.GetComponent<Renderer>().enabled = false;
                yield return new WaitForSeconds(.5f);
                Character.GetComponent<Renderer>().enabled = true;
                yield return new WaitForSeconds(.5f);
                continue;
            }
            Character.GetComponent<Renderer>().enabled = true;

        }
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
			yield return null;
			yield return new WaitForSeconds(waitTimeDuct);
			switch(getIndex(corridors, charact.transform.localPosition)){
				case 0:
					templist=adj1.ToList();
					break;
				case 1:
					templist=adj2.ToList();
					break;
				case 2:
					templist=adj3.ToList();
					break;
				case 3:
					templist=adj4.ToList();
					break;
				case 4:
					templist=adj5.ToList();
					break;
				case 5:
					templist=adj6.ToList();
					break;
				case 6:
					templist=adj7.ToList();
					break;
				case 7:
					templist=adj8.ToList();
					break;
				case 8:
					templist=adj9.ToList();
					break;
				case 9:
					templist=adj10.ToList();
					break;
				case 10:
					templist=adj11.ToList();
					break;
				case 11:
					templist=adj12.ToList();
					break;
			}
			if(charact.transform.localPosition==new Vector3(audioLure.transform.localPosition.x, 0.03576379f, audioLure.transform.localPosition.z)){
				int rnd = UnityEngine.Random.Range(1,101);
				if(rnd<=40){
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
					Strike("Duct");
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
				return i;	//Returns the index of the item in the list
			}
		}
		return -1;	//Returns -1 if the item is not in the list
	}

	IEnumerator startDucts(){
        yield return new WaitForSeconds(30f);
		StartCoroutine(ductCharacter(ductCharacter1));
		yield return new WaitForSeconds(3f);
		StartCoroutine(ductCharacter(ductCharacter2));
		yield break;
	}

	void Update(){
		if(TwitchPlaysActive){
			waitTime=20f;
			waitTimeDuct=10f;
		}
		else{
			waitTime=10f;
			waitTimeDuct=5f;
		}
		solvables=Bomb.GetSolvableModuleNames().Where(x => !ignoredModules.Contains(x)).Count();
		if(solvables-Bomb.GetSolvedModuleNames().Count()<=0){
			if(!devMode && !solved){
				solved=true;
				GetComponent<KMBombModule>().HandlePass();
			}
		}
		if(Bomb.GetSolvedModuleNames().Count()>lastCalcStage){
			lastCalcStage=Bomb.GetSolvedModuleNames().Count();
			NewStage(false);
		}
	}


	void NewStage(bool first){
		if(closedDoor!=currentPlace){
			Strike("Cameras");
			currentPlace=UnityEngine.Random.Range(1,4);
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
		stages = Bomb.GetSolvableModuleNames().Where(x => !ignoredModules.Contains(x)).Count();
		if(stages<=0){
			solved=true;
			StartCoroutine(AutoSolve());
		}
	}

	private IEnumerator AutoSolve(){
		 yield return new WaitForSeconds(2f);
		 Debug.LogFormat("[Ultimate Custom Night #{0}] There are no modules left that are not ignored by Ultimate Custom Night. Auto-solving module.", moduleId);
		 GetComponent<KMBombModule>().HandlePass();
		 yield break;
	 }

    private IEnumerator HandlePower()
    {
        while(!solved)
        {
            yield return null;
            var time = WaitTime;
            if (time == null) continue;
            yield return new WaitForSeconds((float)time);
            CurrentPower -= 1;
            if(CurrentPower<=0)
            {
                Detonate("You ran out of power");
                yield break;
            }
        }
    }

    private void Strike(string reason){
		if(!solved){
			Debug.LogFormat("[Ultimate Custom Night #{0}] Struck by {1}.", moduleId, reason);
			GetComponent<KMBombModule>().HandleStrike(); //Strike only if the module is not solved
		}
		return;
	}

    private void Detonate(string reason)
    {
        Type ModesType = ReflectionHelper.FindType("OtherModes");
        bool TrainingModeActive = ModesType != null && (int)(ModesType.GetField("currentMode", BindingFlags.Public | BindingFlags.Static).GetValue(null)) == 4;
        if (ZenModeActive || TrainingModeActive)
        {
            Type BombType = ReflectionHelper.FindType("Bomb");
            BombType.GetMethod("Detonate", MainFlags).Invoke(FindObjectOfType(BombType), new object[] { });
            return;
        }
        Type CommandLineType = ReflectionHelper.FindType("CommandLine");
        if(CommandLineType!=null)
        {
            try
            {
                foreach (object commander in (IEnumerable)CommandLineType.GetField("BombCommanders", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(FindObjectOfType(CommandLineType)))
                {
                    commander.GetType().GetMethod("Detonate", MainFlags).Invoke(commander, new object[] { reason });
                    return;
                }
            }
            catch(Exception ex)
            {
                Debug.LogFormat("[Ultimate Custom Night #{0}] An error occurred while detonating the bomb with reason: {1}", moduleId, ex.ToString());
            }
        }
        StartCoroutine(HandleDetonate());
    }

    private IEnumerator HandleDetonate()
    {
        while(true)
        {
            GetComponent<KMBombModule>().HandleStrike();
            yield return null;
        }
    }

    #pragma warning disable 414
    public string TwitchHelpMessage = "Use '!{0} help [topic]' to get the help message of the selected topic! These can be: 'all', 'general', 'office', 'cameras', 'vent', 'duct', 'perks'";
	#pragma warning restore 414
	
	void TwitchHandleForcedSolve()
	{
		StopAllCoroutines();
		solved = true;
		Debug.LogFormat("[Ultimate Custom Night #{0}] Auto-solving module due to TP command requesting a force solve.", moduleId);
		GetComponent<KMBombModule>().HandlePass();
	}

    string GetTwitchPlaysId()
    {
        var gType = ReflectionHelper.FindType("TwitchGame", "TwitchPlaysAssembly");
        object comp = FindObjectOfType(gType);
        var TwitchPlaysObj = comp.GetType().GetField("Modules", MainFlags).GetValue(comp);
        IEnumerable TwitchPlaysModules = (IEnumerable)TwitchPlaysObj;
        foreach (object Module in TwitchPlaysModules)
        {
            var Behaviour = (MonoBehaviour)(Module.GetType().GetField("BombComponent", MainFlags).GetValue(Module));
            var UCN = Behaviour.GetComponent<qkUCNScript>();
            if (UCN == this)
            {
                return (string)Module.GetType().GetProperty("Code", MainFlags).GetValue(Module, null);
            }
        }
        return "#";
    }

    IEnumerator ProcessTwitchCommand(string command){
		string commandl = "";
		int tried = 0;
		command=command.ToLowerInvariant().Trim();
        string[] HelpCommand = null;
        HelpCommands.TryGetValue(command, out HelpCommand);
        if(HelpCommand!=null)
        {
            yield return null;
            var TpId = GetTwitchPlaysId();
            foreach (string msg in HelpCommand) yield return String.Format("sendtochat {0}", String.Format(msg, TpId));
            yield break;
        }
        if (command.Equals("cycle")){
			yield return null;
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
		if(command.Equals("cyclecams")){
			yield return null;
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
		if(command.Contains("closedoor")){
			commandl=command.Replace("closedoor", "");
			if(int.TryParse(commandl, out tried)){
				tried=int.Parse(commandl);
				tried=tried-1;
				if(tried>=0 && tried<=2){
					yield return null;
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
		if(command.Equals("cameras")){
			yield return null;
			camButton.OnInteract();
			yield break;
		}
		if(command.Equals("vent")){
			yield return null;
			ventButton.OnInteract();
			yield break;
		}
		if(command.Equals("duct")){
			yield return null;
			ductButton.OnInteract();
			yield break;
		}
		if(command.Contains("snare")){
			commandl=command.Replace("snare", "");
			if(int.TryParse(commandl, out tried)){
				tried=int.Parse(commandl);
				tried=tried-1;
				if(tried>=0 && tried<=2){
					bool changed = false;
					if(tried==0){tried=2;
					changed=true;};
					if(tried==2 && !changed){tried=0;};
					yield return null;
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
		if(command.Contains("setlure")){
			commandl=command.Replace("setlure", "");
			if(int.TryParse(commandl, out tried)){
				tried=int.Parse(commandl);
				tried=tried-1;
				if(tried>=0 && tried<=11){
					yield return null;
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
		if(command.Equals("openright")){
			yield return null;
			ductButton.OnInteract();
			ductDoorButtons[1].GetComponent<KMSelectable>().OnInteract();
			yield break;
		}
		if(command.Equals("openleft")){
			yield return null;
			ductButton.OnInteract();
			ductDoorButtons[0].GetComponent<KMSelectable>().OnInteract();
			yield break;
		}
		if(command.Equals("cam1")){
			yield return null;
			camButton.OnInteract();
			camButton1.OnInteract();
			yield break;
		}
		if(command.Equals("cam2")){
			yield return null;
			camButton.OnInteract();
			camButton2.OnInteract();
			yield break;
		}
		if(command.Equals("cam3")){
			yield return null;
			camButton.OnInteract();
			camButton3.OnInteract();
			yield break;
		}
        if(command.StartsWith("toggledoor "))
        {
            command = command.Replace("toggledoor ", "");
            yield return null;
            switch(command)
            {
                case "front":
                    ToggleDoor(ref FrontVentClosed);
                    break;
                case "side":
                    ToggleDoor(ref RightVentClosed);
                    break;
                default:
                    yield return "sendtochaterror Invalid office door to toggle!";
                    break;
            }
            yield break;
        }
        if(command.StartsWith("perk "))
        {
            command = command.Replace("perk ", "");
            yield return null;
            perksButton.OnInteract();
            yield return null;
            switch(command)
            {
                case "global music box":
                case "gmb":
                case "1":
                    GlobalMusicBoxButton.OnInteract();
                    break;
                case "off":
                case "2":
                    OffButton.OnInteract();
                    break;
                default:
                    yield return "sendtochaterror Invalid perk button!";
                    break;
            }
            yield break;
        }
        if(command.Equals("changemusic"))
        {
            yield return null;
            if (VisibilityOverride) camButton.OnInteract();
            ChangeMusicButton.OnInteract();
        }
        yield break;
	}
}
