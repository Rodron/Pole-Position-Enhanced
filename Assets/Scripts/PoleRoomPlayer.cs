﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

[AddComponentMenu("")]
public class PoleRoomPlayer : NetworkRoomPlayer
{
    [SyncVar]
    public string Name;

    [SyncVar]
    public int SelectedCar;

    [SyncVar]
    public int maxLap;

    [SyncVar]
    public bool classifLap;

    public static event Action<PoleRoomPlayer, string> OnMessage;

    public override void OnStartClient()
    {
        if (LogFilter.Debug) Debug.LogFormat("OnStartClient {0}", SceneManager.GetActiveScene().path);
        CmdChangeName("Player Joining...");
        OnMessage = null;
        OnMessage += FindObjectOfType<UIManager>().OnPlayerMessage;
        classifLap = true;
        base.OnStartClient();
    }

    public override void OnClientEnterRoom()
    {
        if (LogFilter.Debug) Debug.LogFormat("OnClientEnterRoom {0}", SceneManager.GetActiveScene().path);
        if (hasAuthority)
        {
            if (FindObjectOfType<PoleNetworkManager>().roomSlots[0] == this.GetComponent<NetworkRoomPlayer>())
            {
                FindObjectOfType<UIManager>().lapsObject.SetActive(true);
            }
        }
    }

    public override void OnClientExitRoom()
    {
        if (LogFilter.Debug) Debug.LogFormat("OnClientExitRoom {0}", SceneManager.GetActiveScene().path);
    }

    #region Commands

    [Command]
    public void CmdChangeName(string pName)
    {
        Name = pName;
    }

    [Command]
    public void CmdChangeCar(int car)
    {
        SelectedCar = car;
    }

    [Command]
    public void CmdSend(string message)
    {
        if (message.Trim() != "")
            RpcReceive(message.Trim());
    }

    [Command]
    public void CmdSetMaxLap(int lap)
    {
        maxLap = lap;
    }

    [Command]
    public void CmdChangeClassifLap(){
        classifLap = !classifLap;
    }

    #endregion

    #region ClientRpc

    [ClientRpc]
    public void RpcReceive(string message)
    {
        OnMessage?.Invoke(this, message);
    }

    #endregion
}
