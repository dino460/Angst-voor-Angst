using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	private InputManager controller;
	[SerializeField] private CinemachineVirtualCamera vcam;


	[SerializeField] private float positionMod;
    [SerializeField] private float originYposition;
    [SerializeField] private float bossAreaYposition;
    [SerializeField] private float bossAreaDistance;
    				 private float normalDistance = 18f;


    private PlayerController playerController;


	private void Awake()
	{
		controller = new InputManager();

		controller.Player.MoveCamera.performed += _0 =>
			positionMod = _0.ReadValue<float>();
        controller.Player.MoveCamera.canceled += _0 =>
            positionMod = 0f;
	}


	private void Start()
	{
		vcam.m_Follow = GameObject.Find("Player").GetComponent<Transform>();
		playerController = GameObject.Find("Player").GetComponent<PlayerController>();
	}


    private void OnEnable()
    {
        controller.Enable();
    }


	private void Update()
	{
		if (!playerController.insideBossArea)
		{
			MoveCameraY(positionMod/3f);
		}

		if (playerController.insideBossArea)
		{
			if (vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance != bossAreaDistance)
			{
				vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = bossAreaDistance;
				vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = bossAreaYposition;
			}
		}
		else
		{
			if (vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance != normalDistance)
			{
				vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = normalDistance;
				vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = originYposition;
			}		
		}
	}


    private void MoveCameraY(float yMod)
    {
        vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = originYposition + yMod;
    }
}
