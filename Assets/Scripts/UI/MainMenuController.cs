using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;


public class MainMenuController : MonoBehaviour
{

	[SerializeField] private GameObject MainMenu;
	[SerializeField] private GameObject OptionsMenu;
	[SerializeField] private GameObject LoadMenu;
	private Resolution[] resolutions;

	public GameObject pauseFirstButton; 
	public GameObject optionsFirstButton; 
	public GameObject optionsClosedButton;

	public GameObject loadFirstButton; 
	public GameObject loadCloseButton;

	public GameObject profilesFirstButton;

	public TMPro.TMP_Dropdown resolutionDropdown;

	public AudioMixer audioMixer;


	private void Start(){
		resolutions = Screen.resolutions;
		resolutionDropdown.ClearOptions();

		List<string> options = new List<string>();

		int currentResolutionIndex = 0;
		for(int i = 0; i < resolutions.Length; i++){

			string option = resolutions[i].width + " x " + resolutions[i].height;
			options.Add(option);

			if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height){
				currentResolutionIndex = i;
			}

		}

		resolutionDropdown.AddOptions(options);
		resolutionDropdown.value = currentResolutionIndex;
		resolutionDropdown.RefreshShownValue();

		EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.SetSelectedGameObject(pauseFirstButton);

	}

	public void SetResolution(int resolutionIndex){
		
		Resolution resolution = resolutions[resolutionIndex];
		Global.screenRes = resolutions[resolutionIndex];
		Screen.SetResolution(resolution.width, resolution.height, Global.isFullscreen);

	}

    public void PlayButton(){
    	MainMenu.SetActive(false);
		LoadMenu.SetActive(true);
		EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.SetSelectedGameObject(profilesFirstButton);
    }

    public void QuitButton(){
    	Debug.Log("Closing game");
    	Application.Quit();
    }

    public void OptionsButton(){
    	MainMenu.SetActive(false);
    	OptionsMenu.SetActive(true);
    	EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.SetSelectedGameObject(optionsFirstButton);
    }

    public void ReturnButton(){
    	OptionsMenu.SetActive(false);
    	MainMenu.SetActive(true);
    	EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.SetSelectedGameObject(optionsClosedButton);
    }

	public void ReturnLoadButton(){
    	LoadMenu.SetActive(false);
    	MainMenu.SetActive(true);
    	EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.SetSelectedGameObject(loadCloseButton);
    }

    public void SetVolume(float volume){
    	audioMixer.SetFloat("MasterVolume", volume);
    }

    public void ToggleFullscreen(bool isFullscreen){
		Screen.fullScreen = isFullscreen;
		Global.isFullscreen = isFullscreen;
	}

	public void LoadProfile(){
		int prof = int.Parse(EventSystem.current.currentSelectedGameObject.name[7].ToString());
		Global.profileLoaded = prof;
		SceneManager.LoadScene(1);
	}

	public void DeleteProfile(){
		int prof = int.Parse(EventSystem.current.currentSelectedGameObject.name[7].ToString());
		string path = Application.persistentDataPath + "/data" + prof + ".txt";
		SaveSystem.DeleteSaveData(path);
	}
}
