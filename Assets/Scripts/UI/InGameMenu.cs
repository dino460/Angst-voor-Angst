using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;


public class InGameMenu : MonoBehaviour
{
	[SerializeField] private PlayerController playerController;

	public GameObject pauseFirstButton; 
	public GameObject optionsFirstButton; 
	public GameObject optionsClosedButton;


	private Resolution[] resolutions;
	[SerializeField] private TMPro.TMP_Dropdown resolutionsDropdown;
	[SerializeField] private GameObject optionsMenu;
	[SerializeField] private GameObject inGameMenu;

	[SerializeField] private AudioMixer mainMixer;


	private void Start()
	{
		#if !UNITY_WEBGL
		
		resolutions = Screen.resolutions;
		Screen.SetResolution(Global.screenRes.width, Global.screenRes.height, Global.isFullscreen);

		for (int i = 0; i < resolutions.Length; i++)
		{
			resolutionsDropdown.options[i].text = ResolutionToStr(resolutions[i]);
			resolutionsDropdown.value = i;
			resolutionsDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData(
				resolutionsDropdown.options[i].text));
		}
		
		#endif
	}


	private string ResolutionToStr(Resolution res)
	{
		return res.width + " x " + res.height;
	}


	public void SetResolution(int resolutionIndex)
	{
		Resolution resolution = resolutions[resolutionIndex];
		Global.screenRes = resolutions[resolutionIndex];
		Screen.SetResolution(resolution.width, resolution.height, Global.isFullscreen);
	}


	public void ToggleFullscreen(bool isFullscreen)
	{
		Screen.fullScreen = isFullscreen;
		Global.isFullscreen = isFullscreen;
	}


	public void OpenMenu()
	{
		inGameMenu.SetActive(true);
		playerController.isMenuOpen = true;
		EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.SetSelectedGameObject(pauseFirstButton);
	}


	public void ExitToMenu()
	{
		SceneManager.LoadScene(0);
	}


	public void Options()
	{
		optionsMenu.SetActive(true);
		inGameMenu.SetActive(false);
		EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.SetSelectedGameObject(optionsFirstButton);
	}


	public void SetVolume(float sliderValue)
	{
		mainMixer.SetFloat("MasterVolume", sliderValue);
	}


	public void Return()
	{
		inGameMenu.SetActive(true);
		optionsMenu.SetActive(false);
		EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.SetSelectedGameObject(optionsClosedButton);
	}


	public void CloseMenu()
	{
		optionsMenu.SetActive(false);
		inGameMenu.SetActive(false);
		playerController.isMenuOpen = false;
	}


	public void PlayAgain()
	{
		Global.life	= Global.maxLife;
		Global.mana	= Global.maxMana;
		Global.playerDead = false;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}


	public void ExitToMenuAfterDead()
	{
		Global.life	= Global.maxLife;
		Global.mana	= Global.maxMana;
		Global.playerDead = false;
		SceneManager.LoadScene(0);
	}


	public void Exit()
	{
		Debug.Log("Exit working");
		Application.Quit();
	}
}
