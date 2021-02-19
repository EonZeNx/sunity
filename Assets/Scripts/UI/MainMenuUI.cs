using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using MLAPI.Transports.UNET;
using System;

public class MainMenuUI : MonoBehaviour
{
    public Text ErrorText;

    public Text HostPortText;

    public Text JoinIpText;
    public Text JoinPortText;

    public GameObject GameUI;

    public void Host()
    {
        try
        {
            var port = int.Parse(HostPortText.text);

            var networkManager = NetworkingManager.Singleton;
            var networkTransport = NetworkingManager.Singleton.GetComponent<UnetTransport>();

            networkTransport.ServerListenPort = port;

            networkManager.StartHost();

            gameObject.SetActive(false);
            GameUI.SetActive(true);
        } catch (Exception)
        {
            ErrorText.text = "An error has occured.";
        }
    }

    public void Join()
    {
        try
        {
            var ip = JoinIpText.text;
            var port = int.Parse(JoinPortText.text);

            var networkManager = NetworkingManager.Singleton;
            var networkTransport = NetworkingManager.Singleton.GetComponent<UnetTransport>();

            networkTransport.ConnectAddress = ip;
            networkTransport.ConnectPort = port;

            networkManager.StartClient();

            gameObject.SetActive(false);
            GameUI.SetActive(true);
        } catch (Exception)
        {
            ErrorText.text = "An error has occured.";
        }
    }
}
