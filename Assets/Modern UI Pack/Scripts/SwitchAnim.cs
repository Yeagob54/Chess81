﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Michsky.UI.ModernUIPack
{
    public class SwitchAnim : MonoBehaviour
    {
        [Header("SWITCH")]
        public Animator switchAnimator;

        [Header("SETTINGS")]
        [Tooltip("IMPORTANT! EVERY SWITCH MUST HAVE A DIFFERENT ID")]
        public int switchID = 0;
        public bool isOn;
        public bool saveValue;
        [Tooltip("Use it if you're using this switch first time. 1 = ON, and 0 = OFF")]
        [Range(0, 1)] public int playerPrefsHelper;

        public UnityEvent OffEvents;
        public UnityEvent OnEvents;

        private Button offButton;
        private Button onButton;

        private string onTransition = "Switch On";
        private string offTransition = "Switch Off";

        void Start()
        {
            playerPrefsHelper = PlayerPrefs.GetInt(switchID + "Switch");

            if (saveValue == true)
            {
                if (playerPrefsHelper == 1)
                {
                    isOn = true;
                    OnEvents.Invoke();
                    switchAnimator.Play(onTransition);
                }

                else
                {
                    isOn = false;
                    OffEvents.Invoke();
                    switchAnimator.Play(offTransition);
                }
            }

            else
            {
                if (isOn == true)
                {
                    switchAnimator.Play(onTransition);
                    OnEvents.Invoke();
                    isOn = true;
                }

                else
                {
                    switchAnimator.Play(offTransition);
                    OffEvents.Invoke();
                    isOn = false;
                }
            }
        }

        public void AnimateSwitch()
        {
            if (isOn == true)
            {
                isOn = false;
                OffEvents.Invoke();
                switchAnimator.Play(offTransition);
                playerPrefsHelper = 0;
            }

            else
            {
                isOn = true;
                OnEvents.Invoke();
                switchAnimator.Play(onTransition);
                playerPrefsHelper = 1;
            }

            if (saveValue == true)
            {
                PlayerPrefs.SetInt(switchID + "Switch", playerPrefsHelper);
            }
        }
    }
}