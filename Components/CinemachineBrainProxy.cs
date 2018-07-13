﻿// (c) Copyright HutongGames, LLC 2010-2018. All rights reserved.
// Author jean@hutonggames.com
// This code is licensed under the MIT Open source License

using UnityEngine;
using Cinemachine;

using HutongGames.PlayMaker;

/// <summary>
/// This Component listen to m_CameraActivatedEvent and m_CameraCutEvent CinemachineBrain unity events and broadcast PlayMaker Events
/// 
/// CINEMACHINE / ON CAMERA ACTIVATED
/// Event data GameObject will reference the ICinemachineCamera.VirtualCameraGameObject
/// 
/// CINEMACHINE / ON CAMERA CUT
/// Event data GameObject will reference the CinemachineBrain.gameObject
/// 
/// </summary>
[RequireComponent(typeof(CinemachineBrain))]
public class CinemachineBrainProxy : MonoBehaviour {

	CinemachineBrain _brain;

	// Use this for initialization
	void Start () {

		_brain = this.GetComponent<CinemachineBrain> ();
		if (_brain != null) {
            _brain.m_CameraActivatedEvent.AddListener(HandleCameraActivatedAction);
            _brain.m_CameraCutEvent.AddListener(HandleCameraCutEventAction);
		}
	}

    void HandleCameraActivatedAction(ICinemachineCamera arg0)
    {
        Fsm.EventData.GameObjectData = arg0.VirtualCameraGameObject ;

        PlayMakerFSM.BroadcastEvent("CINEMACHINE / ON CAMERA ACTIVATED");
    }

    void HandleCameraCutEventAction(CinemachineBrain arg0)
    {
        Fsm.EventData.GameObjectData = arg0.gameObject;

        PlayMakerFSM.BroadcastEvent("CINEMACHINE / ON CAMERA CUT");
    }
}
