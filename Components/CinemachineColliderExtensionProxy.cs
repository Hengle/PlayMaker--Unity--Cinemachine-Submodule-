﻿// (c) Copyright HutongGames, LLC 2010-2018. All rights reserved.
// Author jean@hutonggames.com
// This code is licensed under the MIT Open source License

using UnityEngine;
using Cinemachine;

using HutongGames.PlayMaker.Ecosystem.Utils;
using HutongGames.PlayMaker;

#if UNITY_EDITOR
using UnityEditor;
#endif


/// <summary>

/// </summary>
[ExecuteInEditMode]
public class CinemachineColliderExtensionProxy : MonoBehaviour
{


    [UnityEngine.Tooltip("Reference to the CinemachineCollider component")]
    [ExpectComponent(typeof(CinemachineCollider))]
    public Owner ColliderExtension;

    [Header("Events")]
    public PlayMakerEventTarget eventTarget = new PlayMakerEventTarget(true);

    [EventTargetVariable("eventTarget")]
    [ShowOptions]
    public PlayMakerEvent onTargetObscuredBeganEvent = new PlayMakerEvent("CINEMACHINE / COLLIDER / ON TARGET OBSCURED BEGAN");

    [EventTargetVariable("eventTarget")]
    [ShowOptions]
    public PlayMakerEvent onTargetObscuredEndedEvent = new PlayMakerEvent("CINEMACHINE / COLLIDER / ON TARGET OBSCURED ENDED");

    [EventTargetVariable("eventTarget")]
    [ShowOptions]
    public PlayMakerEvent onCameraDisplacedBeganEvent = new PlayMakerEvent("CINEMACHINE / COLLIDER / ON CAMERA DISPLACED BEGAN");

    [EventTargetVariable("eventTarget")]
    [ShowOptions]
    public PlayMakerEvent onCameraDisplacedEndedEvent = new PlayMakerEvent("CINEMACHINE / COLLIDER / ON CAMERA DISPLACED ENDED");

    [Header("Variables")]
    public PlayMakerFsmVariableTarget variableTarget = new PlayMakerFsmVariableTarget();

    [FsmVariableTargetVariable("variableTarget")]
    public PlayMakerFsmVariable TargetObscuredVariable = new PlayMakerFsmVariable(VariableSelectionChoice.Bool);

    [FsmVariableTargetVariable("variableTarget")]
    public PlayMakerFsmVariable CameraWasDisplaced = new PlayMakerFsmVariable(VariableSelectionChoice.Bool);


    public bool debug = false;

    CinemachineCollider _cache;
    CinemachineVirtualCameraBase _vcam;


    bool _currentlyObscured;
    bool _currentlyDisplaced;

    #if UNITY_EDITOR
    bool _eventadded;
    #endif

    // Use this for initialization
    void Start()
    {
        _cache = ColliderExtension.gameObject.GetComponent<CinemachineCollider>();

        TargetObscuredVariable.GetVariable(variableTarget);
        CameraWasDisplaced.GetVariable(variableTarget);

    }

    void OnEnable()
    {
        _currentlyObscured = false;
        _currentlyDisplaced = false;
    }

    void Update()
    {
        #if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
        {
            CreateGlobalEventIfNecessary();
        }
        #endif


        if (_cache != null)
            _vcam = _cache.VirtualCamera.LiveChildOrSelf as CinemachineVirtualCameraBase;


        if (_vcam == null)
        {
            return;
        }

        // variables
        if (TargetObscuredVariable.initialized)
        {
            TargetObscuredVariable.FsmBool.Value = _cache.IsTargetObscured(_vcam);
        }

        if (CameraWasDisplaced.initialized)
        {
            CameraWasDisplaced.FsmBool.Value = _cache.CameraWasDisplaced(_vcam);
        }

        // events
        if (_cache.IsTargetObscured(_vcam) && !_currentlyObscured)
        {
            _currentlyObscured = true;
            onTargetObscuredBeganEvent.SendEvent(null, eventTarget, debug);
        }

        if (!_cache.IsTargetObscured(_vcam) && _currentlyObscured) // only call on our way back from being obscured
        {
            _currentlyObscured = false;
            onTargetObscuredEndedEvent.SendEvent(null, eventTarget, debug);
        }

        if (_cache.CameraWasDisplaced(_vcam) && !_currentlyDisplaced)
        {
            _currentlyDisplaced = true;
            onCameraDisplacedBeganEvent.SendEvent(null, eventTarget, debug);
        }

        if (!_cache.CameraWasDisplaced(_vcam) && _currentlyDisplaced) // only call on our way back from being obscured
        {
            _currentlyDisplaced = false;
            onCameraDisplacedEndedEvent.SendEvent(null, eventTarget, debug);
        }

    }


#if UNITY_EDITOR
    void CreateGlobalEventIfNecessary()
    {
        if (!_eventadded)
        {
            AddGlobalEvent("CINEMACHINE / COLLIDER / ON TARGET OBSCURED BEGAN");
            AddGlobalEvent("CINEMACHINE / COLLIDER / ON TARGET OBSCURED ENDED");
            AddGlobalEvent("CINEMACHINE / COLLIDER / ON CAMERA DISPLACED BEGAN");
            _eventadded =  AddGlobalEvent("CINEMACHINE / COLLIDER / ON CAMERA DISPLACED ENDED");
        }
    }

    bool AddGlobalEvent(string globalEventName)
    {

       
        if (!FsmEvent.IsEventGlobal(globalEventName))
        {

            FsmEvent _event = FsmEvent.GetFsmEvent(globalEventName);
            if (_event == null)
            {
                _event = new FsmEvent(globalEventName);
                FsmEvent.AddFsmEvent(_event);
            }

            _event.IsGlobal = true;

            return true;
        }

        return false;
    }
#endif
}