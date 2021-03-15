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
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Settings
{
    public class SettingsManager : MonoBehaviour
    {
        [SerializeField] public UISettings uiSettings;
        [SerializeField] private Slider speedMultiplierSlider;

        [SerializeField] private TextMeshProUGUI currentSpeedValueLabel;


        private void OnEnable()
        {
            SetupSpeedSlider();
        }

        private void SetupSpeedSlider()
        {
            speedMultiplierSlider.value = UISettings.Instance.speedMultiplier;
            currentSpeedValueLabel.text = speedMultiplierSlider.value.ToString("0.00", CultureInfo.InvariantCulture);
            speedMultiplierSlider.onValueChanged.AddListener (delegate {OnSpeedSliderValueChanged ();});
        }
        
        public void OnSpeedSliderValueChanged()
        {
            Debug.Log (speedMultiplierSlider.value);
            uiSettings.speedMultiplier = speedMultiplierSlider.value;
            currentSpeedValueLabel.text = speedMultiplierSlider.value.ToString("0.00", CultureInfo.InvariantCulture);
        }
    }
}