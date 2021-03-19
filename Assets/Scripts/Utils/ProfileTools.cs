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

using System.Collections.Generic;
using System.Text;
using Unity.Profiling;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;

namespace Utils
{
    
    public class ProfileTools : MonoBehaviour
    {
        string statsText;
        private StringBuilder _stringBuilder ;
        
        ProfilerRecorder _systemMemoryRecorder;
        ProfilerRecorder _gcMemoryRecorder;
        ProfilerRecorder _mainThreadTimeRecorder;
        
        public int FPS { get; private set; }
        
        private void Start()
        {
            _stringBuilder = new StringBuilder(500);
        }
        
        void OnEnable()
        {
            _systemMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory");
            _gcMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory");
            _mainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);
            
        }

        void OnDisable()
        {
            _systemMemoryRecorder.Dispose();
            _gcMemoryRecorder.Dispose();
            _mainThreadTimeRecorder.Dispose();
        }
        private void Update()
        {
            FPS = (int)(1f / Time.unscaledDeltaTime);
            _stringBuilder.Clear();
            _stringBuilder.AppendLine($"Frame Time: {GetRecorderFrameAverage(_mainThreadTimeRecorder) * (1e-6f):F1} ms");
            _stringBuilder.AppendLine($"GC Memory: {_gcMemoryRecorder.LastValue / (1024 * 1024)} MB");
            _stringBuilder.AppendLine($"System Memory: {_systemMemoryRecorder.LastValue / (1024 * 1024)} MB");
            statsText = _stringBuilder.ToString();
        }
        
        static double GetRecorderFrameAverage(ProfilerRecorder recorder)
        {
            var samplesCount = recorder.Capacity;
            if (samplesCount == 0)
                return 0;

            double r = 0;
            unsafe
            {
                var samples = stackalloc ProfilerRecorderSample[samplesCount];
                recorder.CopyTo(samples, samplesCount);
                for (var i = 0; i < samplesCount; ++i)
                    r += samples[i].Value;
                r /= samplesCount;
            }

            return r;
        }
        
        struct StatInfo
        {
            public ProfilerCategory Cat;
            public string Name;
            public ProfilerMarkerDataUnit Unit;
        }

        public static unsafe void EnumerateProfilerStats()
        {
            var availableStatHandles = new List<ProfilerRecorderHandle>();
            ProfilerRecorderHandle.GetAvailable(availableStatHandles);

            var availableStats = new List<StatInfo>(availableStatHandles.Count);
            foreach (var h in availableStatHandles)
            {
                var statDesc = ProfilerRecorderHandle.GetDescription(h);
                var statInfo = new StatInfo()
                {
                    Cat = statDesc.Category,
                    Name = statDesc.Name,
                    Unit = statDesc.UnitType
                };
                availableStats.Add(statInfo);
            }
            availableStats.Sort((a, b) =>
            {
                var result = string.Compare(a.Cat.ToString(), b.Cat.ToString());
                if (result != 0)
                    return result;

                return string.Compare(a.Name, b.Name);
            });
            
            var sb = new StringBuilder("Available stats:\n");
            foreach (var s in availableStats)
            {
                sb.AppendLine($"{(int)s.Cat}\t\t - {s.Name}\t\t - {s.Unit}");
            }

            Debug.Log(sb.ToString());
        }
    }
    
    
}