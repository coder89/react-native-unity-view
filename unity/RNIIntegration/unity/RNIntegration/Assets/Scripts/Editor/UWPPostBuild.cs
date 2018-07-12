/*
MIT License
Copyright (c) 2017 Jiulong Wang
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

#if UNITY_WSA

using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using Application = UnityEngine.Application;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;

public static class UWPPostBuild
{
    [PostProcessBuild]
    public static void OnPostBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.WSAPlayer)
        {
            return;
        }

        UpdateUnityProjectFiles(pathToBuiltProject);
    }

    /// <summary>
    /// Enumerates Unity output files and add necessary files into VS project file.
    /// It only add a reference entry into project file, without actually copy it.
    /// </summary>
    private static void UpdateUnityProjectFiles(string pathToBuiltProject)
    {
        string uwpScriptsDirRelative = "../../../unity/" + Path.Combine(Path.GetFileName(Directory.GetCurrentDirectory()), "Assets", "Scripts", "Editor", "UWP");
        string uwpScriptsDir = Path.Combine(Directory.GetCurrentDirectory() + "/Assets/Scripts/Editor/UWP");

        string projectName = Path.GetFileNameWithoutExtension(Directory.GetFiles(pathToBuiltProject).First(m => m.EndsWith(".sln")));
        string pathToUWPProject = Path.Combine(pathToBuiltProject, projectName);

        string csharpProjectFile = Path.Combine(pathToUWPProject, projectName + ".csproj");
        if (File.Exists(csharpProjectFile))
        {
            // Handle as .NET scripting backend
            //File.Copy(
            //    Path.Combine(uwpScriptsDir, "MainPage.xaml"),
            //    Path.Combine(pathToUWPProject, "MainPage.xaml"),
            //    true);
            //File.Copy(
            //    Path.Combine(uwpScriptsDir, "MainPage.xaml.cs"),
            //    Path.Combine(pathToUWPProject, "MainPage.xaml.cs"),
            //    true);

            string csharpProjectFile_Text = File.ReadAllText(csharpProjectFile);
            XNamespace defaultNS = "http://schemas.microsoft.com/developer/msbuild/2003";
            XDocument csharpProject = XDocument.Parse(csharpProjectFile_Text);
            XElement xamlRootParent = csharpProject.Root;

            foreach (var ct in xamlRootParent.Elements(defaultNS + "PropertyGroup").Select(m => m.Element(defaultNS + "OutputType")))
            {
                if (ct != null)
                {
                    ct.SetValue("Library");
                }
            }

            xamlRootParent.Add(
                new XElement(defaultNS + "ItemGroup",
                    new XElement(defaultNS + "Compile",
                        new XAttribute("Include", Path.Combine(uwpScriptsDirRelative, "UnityUtils.cs")),
                        new XElement(defaultNS + "Link", "UnityUtils.cs")),
                    new XElement(defaultNS + "Compile",
                        new XAttribute("Include", Path.Combine(uwpScriptsDirRelative, "UnityView.xaml.cs")),
                        new XElement(defaultNS + "Link", "UnityView.xaml.cs"),
                        new XElement(defaultNS + "DependentUpon", "UnityView.xaml")),
                    new XElement(defaultNS + "Page",
                        new XAttribute("Include", Path.Combine(uwpScriptsDirRelative, "UnityView.xaml")),
                        new XElement(defaultNS + "Link", "UnityView.xaml"),
                        new XElement(defaultNS + "Generator", "MSBuild:Compile"),
                        new XElement(defaultNS + "SubType", "Designer"))));
            csharpProjectFile_Text = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" + csharpProject.ToString(SaveOptions.None);
            File.WriteAllText(csharpProjectFile, csharpProjectFile_Text);
            return;
        }

        string cppProjectFile = Path.Combine(pathToUWPProject, projectName + ".vcxproj");
        if (File.Exists(cppProjectFile))
        {
            // Handle as IL2CPP scripting backend
            //File.Copy(
            //    Path.Combine(uwpScriptsDir, "App.xaml.cpp"),
            //    Path.Combine(pathToUWPProject, "App.xaml.cpp"),
            //    true);
            //File.Copy(
            //    Path.Combine(uwpScriptsDir, "App.xaml.h"),
            //    Path.Combine(pathToUWPProject, "App.xaml.h"),
            //    true);
            //File.Copy(
            //    Path.Combine(uwpScriptsDir, "MainPage.xaml"),
            //    Path.Combine(pathToUWPProject, "MainPage.xaml"),
            //    true);
            //File.Copy(
            //    Path.Combine(uwpScriptsDir, "MainPage.xaml.cpp"),
            //    Path.Combine(pathToUWPProject, "MainPage.xaml.cpp"),
            //    true);
            //File.Copy(
            //    Path.Combine(uwpScriptsDir, "MainPage.xaml.h"),
            //    Path.Combine(pathToUWPProject, "MainPage.xaml.h"),
            //    true);

            string cppProjectFile_Text = File.ReadAllText(cppProjectFile);
            XNamespace defaultNS = "http://schemas.microsoft.com/developer/msbuild/2003";
            XDocument csharpProject = XDocument.Parse(cppProjectFile_Text);
            XElement xamlRootParent = csharpProject.Root;

            foreach (var ct in xamlRootParent.Elements(defaultNS + "PropertyGroup").Select(m => m.Element(defaultNS + "ConfigurationType")))
            {
                if (ct != null)
                {
                    ct.SetValue("DynamicLibrary");
                }
            }

            xamlRootParent.Add(
                new XElement(defaultNS + "PropertyGroup",
                    new XElement(defaultNS + "IncludePath",
                        @"$(ProjectDir)" + uwpScriptsDirRelative + @";$(IncludePath)")));
            xamlRootParent.Add(
                new XElement(defaultNS + "ItemGroup",
                    new XElement(defaultNS + "ClCompile",
                        new XAttribute("Include", Path.Combine(uwpScriptsDirRelative, "UnityUtils.cpp"))),
                    new XElement(defaultNS + "ClCompile",
                        new XAttribute("Include", Path.Combine(uwpScriptsDirRelative, "UnityView.xaml.cpp")),
                        new XElement(defaultNS + "DependentUpon", Path.Combine(uwpScriptsDirRelative, "UnityView.xaml"))),
                    new XElement(defaultNS + "ClInclude",
                        new XAttribute("Include", Path.Combine(uwpScriptsDirRelative, "UnityUtils.h"))),
                    new XElement(defaultNS + "ClInclude",
                        new XAttribute("Include", Path.Combine(uwpScriptsDirRelative, "UnityView.xaml.h")),
                        new XElement(defaultNS + "DependentUpon", Path.Combine(uwpScriptsDirRelative, "UnityView.xaml"))),
                    new XElement(defaultNS + "Page",
                        new XAttribute("Include", Path.Combine(uwpScriptsDirRelative, "UnityView.xaml")),
                        new XElement(defaultNS + "SubType", "Designer"))));
            cppProjectFile_Text = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" + csharpProject.ToString(SaveOptions.None);
            File.WriteAllText(cppProjectFile, cppProjectFile_Text);
            return;
        }
    }
}

#endif
