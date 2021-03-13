using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
public class UIGameController : MonoBehaviour
{
    #region Atributtes

    public MultifunctionalPanelUI popupPanel;

    [Header("Clocks")]
    public ClockController upClock;
    public ClockController downClock;

    [Header("Panel")]
    [SerializeField]
    GameObject endGamePanel; 

    [Header("Buttons")]
    [SerializeField]
    Button abandonarButton;
    [SerializeField]
    Button tablasButton;
    [SerializeField]
    Button exitsButton;
    [SerializeField]
    Button rematchButton;

    [Header("Text")]
    [SerializeField]
    TextMeshProUGUI localNameText;
    [SerializeField]
    TextMeshProUGUI remoteNameText;

    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        //Add Listeners
        abandonarButton.onClick.AddListener(Abandonar);
        tablasButton.onClick.AddListener(Tablas);
        exitsButton.onClick.AddListener(Exit);
        rematchButton.onClick.AddListener(Rematch);
        InitializeClocks();
    }


    private void InitializeClocks()
    {
        downClock.colorClock = ClockType.WHITE;
        upClock.colorClock = ClockType.BLACK;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Set player names
        localNameText.text = UserControl.userData.name;
        if (!PhotonNetwork.IsConnected)
        {
            tablasButton.gameObject.SetActive(false);
            remoteNameText.text = "Skynet";
            upClock.gameObject.SetActive(false);
            downClock.gameObject.SetActive(false);
        }

    }

    private void OnDrawGizmos()
    {
        Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(cursorRay, out hit, 100.0f))
        {
            Gizmos.DrawWireSphere(hit.point, 0.5f);
        }
    }
    #endregion

    #region Methods
    private void Tablas()
    {
        popupPanel.ShowYesNoMode("Do you want to offet Draw to your opponent?", OfrecerTablas);
    }

    private void OfrecerTablas()
    {
        GameController.instance.networkManager.photonView.RPC("DrawPropositon", NetworkManager.instance.rpcTarget);
        popupPanel.HideModal();
    }

    private void Abandonar()
    {
        popupPanel.ShowYesNoMode("Are you sure you want to left this match?", AbandonarPartida);
    }

    void Exit()
    {
        GameController.instance.VolverAlMenu();
    }

    private void AbandonarPartida()
    {
        GameController.instance.MatchLosed("resign");
    }

    private void Rematch()
    {
        GameController.instance.RecargarPartida();
    }

    public void SetOtherPlayerName(string textName)
    {
        remoteNameText.text = textName;
    }

    internal void EndGameMode()
    {
        endGamePanel.SetActive(true);
        upClock.gameObject.SetActive(false);
        downClock.gameObject.SetActive(false);
        tablasButton.gameObject.SetActive(false);
        abandonarButton.gameObject.SetActive(false);
        exitsButton.gameObject.SetActive(true);
        rematchButton.gameObject.SetActive(true);
    }
    #endregion

}
