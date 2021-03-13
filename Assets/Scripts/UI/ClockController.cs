using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

public class ClockController : MonoBehaviour
{
	#region Atributtes
	
	public ClockType colorClock;

    [SerializeField]
    Color yellowClock;

	[SerializeField]
	Color alphaOff;

	[SerializeField]
	Image bgImage;

	[SerializeField]
	Image shadow;

	public AudioClip warningSound;

    TextMeshProUGUI textTime;

    private float _watchWhite = 030, _watchBlack = 030;

    private float currentTime
    {
        get
        {
			if (_watchWhite <= 0 || _watchBlack <= 0)
			{
				return 0;
			}

            return 30f - (boardController.whiteTurnToMove ? Time.realtimeSinceStartup - _watchWhite : Time.realtimeSinceStartup - _watchBlack);
        }
    }

    private bool clockTurnWhite = true;

    cgChessBoardScript boardController { get { return GameController.instance.boardController; } }
	
    #endregion

    #region Unity Callbacks

    private void Start()
    {
        textTime = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
	{
		ClockSystem();
	}
	#endregion

	#region Methods

	private void ClockSystem()
	{
		if (boardController.firstMove) //Check if the first move has been done to start counting
		{
			if (colorClock == ClockType.WHITE)
			{
				SetToYellow(); //Indicates the first move
			}

			if (colorClock == ClockType.BLACK)
			{
				RestartToAlphaOff();
			}
			return;
		}

		if (boardController.whiteTurnToMove) // It's white turn
		{
            if (clockTurnWhite == false)
            {
                clockTurnWhite = true;
                _watchWhite = Time.realtimeSinceStartup;
            }

			if (colorClock == ClockType.BLACK)
			{
				RestartToAlphaOff(); //Restart color from enemy
			}

			if (colorClock == ClockType.WHITE)
			{                

				if (currentTime > 5f)
				{
					SetToYellow(); //Change the color to indicates who's the turn
				}
				else
				{
					BlinkingRed();

					if (!boardController.GetComponent<AudioSource>().isPlaying)
						boardController.GetComponent<AudioSource>().PlayOneShot(warningSound);

					if (currentTime <= 0 && boardController.playerCanMove)
					{
						EndGame("time");
					}

				}
			}
		}
		else  //It's black
		{
            if (clockTurnWhite)
            {
                clockTurnWhite = false;
                _watchBlack = Time.realtimeSinceStartup;
            }

            if (colorClock == ClockType.WHITE)
			{
				RestartToAlphaOff(); //Restart color from enemy
			}

			if (colorClock == ClockType.BLACK)
			{

                if (currentTime > 5f)
				{
					SetToYellow(); //Change the color to indicates who's the turn
				}
				else
				{
					BlinkingRed();

					if (!boardController.GetComponent<AudioSource>().isPlaying)
						boardController.GetComponent<AudioSource>().PlayOneShot(warningSound);

					if (currentTime < 0 && boardController.playerCanMove)
					{
						EndGame("time");
					}
				}
			}
		}

        if (_watchWhite < 0)
            _watchWhite = 0;

        if (_watchBlack < 0)
            _watchBlack = 0;

        if (colorClock == ClockType.WHITE && clockTurnWhite)
        {
            textTime.text = currentTime.ToString(0 + "#:##"); //Update the UI with the format
            return;
        }
        else
            textTime.text = "00:30";


        if (colorClock == ClockType.BLACK && !clockTurnWhite)
            textTime.text = currentTime.ToString(0 + "#:##"); //Update the UI with the format
        else
            textTime.text = "00:30";


    }

	private void BlinkingRed()
	{
		if (bgImage.color == Color.yellow)
			bgImage.color = new Color(255f, 0, 0);

		if (bgImage.color.a == 1)
		{
			bgImage.DOFade(0, 0.5f);
			shadow.enabled = false;
		}
		else if (bgImage.color.a == 0)
		{
			bgImage.DOFade(1, 0.5f);
		}
	}

	private void RestartToAlphaOff()
	{
		bgImage.color = alphaOff; 
		shadow.color = alphaOff;
	}

	private void SetToYellow()
	{
		shadow.color = Color.black;
        bgImage.color = new Color(75, 43, 130);// Color.yellow;
	}

    private void EndGame(string razon)
    {
        GameController.instance.MatchLosed(razon);
        Destroy(this);//Stop clock script
    }

    #endregion
}
