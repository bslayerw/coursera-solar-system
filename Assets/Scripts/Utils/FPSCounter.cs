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

using TMPro;
using UnityEngine;

namespace Utils
{
    [System.Serializable]
    struct FPSColor {
        public Color color;
        public int minimumFPS;
    }
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text averageFPSLabel;
        [SerializeField] private FPSColor[] coloring;
        
        public int AverageFPS { get; private set; }
        public int HighestFPS { get; private set; }
        public int LowestFPS { get; private set; }
        public int frameRange = 60;
        private int[] _fpsBuffer;
        private int _fpsBufferIndex;
        
        void Update () {
            if (_fpsBuffer == null || _fpsBuffer.Length != frameRange) {
                InitializeBuffer();
            }
            UpdateBuffer();
            CalculateFPS();
            Display(averageFPSLabel, AverageFPS);
        }

        private void Display (TMP_Text label, int fps) {
            label.text = $"FPS: {stringsFrom00To99[Mathf.Clamp(fps, 0, 99)]}";
            for (var i = 0; i < coloring.Length; i++)
            {
                if (fps < coloring[i].minimumFPS) continue;
                label.color = coloring[i].color;
                break;
            }
        }

        private void CalculateFPS () {
            var sum = 0;
            var highest = 0;
            var lowest = int.MaxValue;
            for (var i = 0; i < frameRange; i++) {
                var fps = _fpsBuffer[i];
                sum += fps;
                if (fps > highest) {
                    highest = fps;
                }
                if (fps < lowest) {
                    lowest = fps;
                }
            }
            AverageFPS = sum / frameRange;
            HighestFPS = highest;
            LowestFPS = lowest;
        }
        
        void UpdateBuffer () {
            _fpsBuffer[_fpsBufferIndex++] = (int)(1f / Time.unscaledDeltaTime);
            if (_fpsBufferIndex >= frameRange) {
                _fpsBufferIndex = 0;
            }
        }
        
        void InitializeBuffer () {
            if (frameRange <= 0) {
                frameRange = 1;
            }
            _fpsBuffer = new int[frameRange];
            _fpsBufferIndex = 0;
        }

        private static readonly string[] stringsFrom00To99 = {
            "00", "01", "02", "03", "04", "05", "06", "07", "08", "09",
            "10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
            "20", "21", "22", "23", "24", "25", "26", "27", "28", "29",
            "30", "31", "32", "33", "34", "35", "36", "37", "38", "39",
            "40", "41", "42", "43", "44", "45", "46", "47", "48", "49",
            "50", "51", "52", "53", "54", "55", "56", "57", "58", "59",
            "60", "61", "62", "63", "64", "65", "66", "67", "68", "69",
            "70", "71", "72", "73", "74", "75", "76", "77", "78", "79",
            "80", "81", "82", "83", "84", "85", "86", "87", "88", "89",
            "90", "91", "92", "93", "94", "95", "96", "97", "98", "99"
        };
    }
}