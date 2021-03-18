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

using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace Editor.Build
{
    /// <summary>
    ///     GitException includes the error output from a Git.Run() command as well as the
    ///     ExitCode it returned.
    /// </summary>
    public class GitException : InvalidOperationException
    {
        /// <summary>
        ///     The exit code returned when running the Git command.
        /// </summary>
        public readonly int ExitCode;

        public GitException(int exitCode, string errors) : base(errors)
        {
            ExitCode = exitCode;
        }
    }

    public static class Git
    {
        [MenuItem("DevOps/Git/Build Version Status")]
        public static void BuildVersionStatus()
        {
            EditorUtility.DisplayDialog(
                "Current Build Version",
                $"{BuildVersion}",
                "Ok",
                "Close");
        }
        /* Properties ============================================================================================================= */

        /// <summary>
        ///     Retrieves the build version from git based on the most recent matching tag and
        ///     commit history. This returns the version as: {major.minor.build} where 'build'
        ///     represents the nth commit after the tagged commit.
        ///     Note: The initial 'v' and the commit hash code are removed.
        /// </summary>
        public static string BuildVersion
        {
            get
            {
                var version = Run(@"describe --tags --long --match ""v[0-9]*""");
                // Remove initial 'v' and ending git commit hash.
                version = version.Replace('-', '.');
                version = version.Substring(1, version.LastIndexOf('.') - 1);
                return version;
            }
        }

        /// <summary>
        ///     The currently active branch.
        /// </summary>
        public static string Branch => Run(@"rev-parse --abbrev-ref HEAD");

        /// <summary>
        ///     Returns a listing of all uncommitted or untracked (added) files.
        /// </summary>
        public static string Status => Run(@"status --porcelain");


        /* Methods ================================================================================================================ */

        /// <summary>
        ///     Runs git.exe with the specified arguments and returns the output.
        /// </summary>
        public static string Run(string arguments)
        {
            using (var process = new Process())
            {
                var exitCode = process.Run(
                    @"git", 
                    arguments,
                    Application.dataPath,
                    out var output, 
                    out var errors, 
                    true
                    );
                if (exitCode == 0)
                    return output;
                throw new GitException(exitCode, errors);
            }
        }
    }
}