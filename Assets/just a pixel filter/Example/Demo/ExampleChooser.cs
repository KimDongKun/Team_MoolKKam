using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace justA.PixelFilter.Example
{
    public class ExampleChooser : MonoBehaviour
    {
        public Button clearButton;

        public Volume volume;

        public Button[] exampleButtons;

        public VolumeProfile[] exampleProfiles;

        void Start()
        {
            if (exampleButtons.Length != exampleProfiles.Length)
            {
                Debug.LogError("Number of buttons and profiles do not match.");
                return;
            }

            for (int i = 0; i < exampleButtons.Length; i++)
            {
                int index = i;
                exampleButtons[i].onClick.AddListener(() => SetVolumeProfile(exampleProfiles[index]));
            }

            clearButton.onClick.AddListener(() => SetVolumeProfile(null));
        }

        private void SetVolumeProfile(VolumeProfile profile)
        {
            volume.profile = profile;
            volume.sharedProfile = profile;
        }
    }
}
