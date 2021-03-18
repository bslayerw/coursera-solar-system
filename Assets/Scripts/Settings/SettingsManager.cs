#region MIT License

// # Released under MIT License
// 
// Copyright (c) 2021 Byron Wright.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the
// following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR
// ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 

#endregion

using System.Globalization;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Settings
{
    [System.Serializable]
    public struct SelectionDisplayData
    {
        public string title;
        public string text;
        public Sprite image;
    }

    public class SettingsManager : MonoBehaviour
    {
        [SerializeField] public UISettings uiSettings;

        [SerializeField] private Slider speedMultiplierSlider;
        [SerializeField] private TextMeshProUGUI currentSpeedValueLabel;
        [SerializeField] private TMP_Dropdown planetTargetCameraDropdown;
        [SerializeField] private TMP_Dropdown cameraLocationDropdown;
        [SerializeField] private TextMeshProUGUI selectionTitle;
        [SerializeField] private TextMeshProUGUI selectionDescription;
        [SerializeField] private Image selectionImage;
        
        [SerializeField] private Transform[] cameraTargets;
        [SerializeField] private GameObject[] vCams;
        [SerializeField] private GameObject currentVcam;

        [SerializeField] private LookAtTarget cameraLookAtTarget;
        public SelectionDisplayData[] selectionData;

        private void OnEnable()
        {
            Debug.Log($"SettingsManager OnEnable, setting delegate on {cameraLookAtTarget}");
            cameraLookAtTarget.ObjectClickDelegate = OnObjectClick;
            SetupSpeedSlider();
            SetupPlanetTargetCameraDropdown();
            SetupCameraLocationDropdown();
            // populate data display
            SetDataDisplay(0);
        }

        private void OnObjectClick(GameObject obj)
        {
            var currentTargetTag = obj.tag;
            Debug.Log($"object with tag: {currentTargetTag} was clicked");
            // find the target in the currently available targets
            for (var i = 0; i < cameraTargets.Length; i++)
            {
                if (cameraTargets[i].CompareTag(currentTargetTag))
                {
                    // found the target the user clicked on in the available targets
                    // set the current selection to the target
                    Debug.Log($"found cameraTarget with the same tag");
                    planetTargetCameraDropdown.value = i;
                }
            }
        }

        private void SetupSpeedSlider()
        {
            speedMultiplierSlider.value = UISettings.Instance.speedMultiplier;
            currentSpeedValueLabel.text = speedMultiplierSlider.value.ToString("0.00", CultureInfo.InvariantCulture);
            speedMultiplierSlider.onValueChanged.AddListener(delegate { OnSpeedSliderValueChanged(); });
        }

        private void SetupPlanetTargetCameraDropdown()
        {
            planetTargetCameraDropdown.value = UISettings.Instance.currentPlanetTargetSelection;
            planetTargetCameraDropdown.onValueChanged.AddListener(delegate { OnPlanetTargetSelectionChanged(); });
        }

        private void SetupCameraLocationDropdown()
        {
            cameraLocationDropdown.value = UISettings.Instance.currentPlanetTargetSelection;
            cameraLocationDropdown.onValueChanged.AddListener(delegate { OnCameraLocationSelectionChanged(); });
        }

        private void OnCameraLocationSelectionChanged()
        {
            Debug.Log(
                $"location selected: [{cameraLocationDropdown.value}]:{cameraLocationDropdown.options[cameraLocationDropdown.value].text}"
            );

            if (vCams is null || vCams.Length == 0)
            {
                Debug.LogError(
                    "there are no camera targets. Make sure you've added all the camera look at targets to the camera targets list in the inspector"
                );
                return;
            }

            SwitchVcam(cameraLocationDropdown.value, planetTargetCameraDropdown.value);
        }

        private void OnPlanetTargetSelectionChanged()
        {
            Debug.Log(
                $"planet selected: [{planetTargetCameraDropdown.value}]:{planetTargetCameraDropdown.options[planetTargetCameraDropdown.value].text}"
            );

            if (cameraTargets is null || cameraTargets.Length == 0)
            {
                Debug.LogError(
                    "there are no camera targets. Make sure you've added all the camera look at targets to the camera targets list in the inspector"
                );
                return;
            }

            SwitchVcam(cameraLocationDropdown.value, planetTargetCameraDropdown.value);
            SetDataDisplay(planetTargetCameraDropdown.value);
        }

        private void OnSpeedSliderValueChanged()
        {
            uiSettings.speedMultiplier = speedMultiplierSlider.value;
            currentSpeedValueLabel.text = speedMultiplierSlider.value.ToString("0.00", CultureInfo.InvariantCulture);
        }

        private void SwitchVcam(int vcamIndex, int targetIndex)
        {
            if (!(currentVcam is null))
                // disable the current vcam to switch, this is how cinemachine handles which camera to use.
                // The vCam that's enabled with the highest priority to get used. If all vCams have the same priority
                // the last one enabled will be used. 
                currentVcam.SetActive(false);

            var nextVcam = vCams[vcamIndex];
            nextVcam.SetActive(true);
            currentVcam = nextVcam;
            var vcam = currentVcam.GetComponent<CinemachineVirtualCamera>();
            vcam.LookAt = cameraTargets[targetIndex];

            if (vcamIndex == targetIndex) vcam.Follow = cameraTargets[targetIndex];
        }

        private void SetDataDisplay(int index)
        {
            if (selectionData[index].title != null || selectionData[index].title != "")
            {
                selectionTitle.text = selectionData[index].title;
            }
            else
            {
                selectionTitle.text = planetTargetCameraDropdown.options[planetTargetCameraDropdown.value].text;
            }
            selectionDescription.text = selectionData[index].text.Replace("\\n", "\n");
            selectionImage.sprite = selectionData[index].image;
        }
    }
}