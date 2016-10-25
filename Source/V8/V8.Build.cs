using UnrealBuildTool;
using System.IO;
using System;

public class V8 : ModuleRules
{
    protected string ThirdPartyPath
    {
        get { return Path.GetFullPath(Path.Combine(ModuleDirectory, "..", "..", "ThirdParty")); }
    }

    public int[] GetV8Version()
    {
        string[] VersionHeader = Utils.ReadAllText(Path.Combine(ThirdPartyPath, "v8", "include", "v8-version.h")).Replace("\r\n", "\n").Replace("\t", " ").Split('\n');
        string VersionMajor = "0";
        string VersionMinor = "0";
        string VersionPatch = "0";
        foreach (string Line in VersionHeader)
        {
            if (Line.StartsWith("#define V8_MAJOR_VERSION"))
            {
                VersionMajor = Line.Split(' ')[2];
            }
            else if (Line.StartsWith("#define V8_MINOR_VERSION "))
            {
                VersionMinor = Line.Split(' ')[2];
            }
            else if (Line.StartsWith("#define V8_PATCH_VERSION "))
            {
                VersionPatch = Line.Split(' ')[2];
            }
        }
        return new int[] { Int32.Parse(VersionMajor), Int32.Parse(VersionMinor), Int32.Parse(VersionPatch) };
    }

    public V8(TargetInfo Target)
    {
        PrivateIncludePaths.AddRange(new string[]
        {
            Path.Combine(ThirdPartyPath, "v8", "include"),
            Path.Combine("V8", "Private")
        });

        PublicIncludePaths.AddRange(new string[]
        {
            Path.Combine("V8", "Public")
        });

        PublicDependencyModuleNames.AddRange(new string[] 
        { 
            "Core", "CoreUObject", "Engine"
        });

        if (UEBuildConfiguration.bBuildEditor)
        {
            PublicDependencyModuleNames.AddRange(new string[]
            {
                "DirectoryWatcher"
            });
        }

        PrivateDependencyModuleNames.AddRange(new string[] 
        { 
            "Sockets"
        });

        if (UEBuildConfiguration.bBuildEditor)
        {
            PrivateDependencyModuleNames.AddRange(new string[] 
            { 
                "UnrealEd"
            });
        }

        bEnableExceptions = true;

        LoadV8(Target);
    }

    private bool LoadV8(TargetInfo Target)
    {
        int[] v8_version = GetV8Version();
        bool ShouldLink_libsampler = v8_version[1] >= 3;

        if ((Target.Platform == UnrealTargetPlatform.Win64) || (Target.Platform == UnrealTargetPlatform.Win32))
        {
            string LibrariesPath = Path.Combine(ThirdPartyPath, "v8", "lib");

            if (Target.Platform == UnrealTargetPlatform.Win64)
            {
                LibrariesPath = Path.Combine(LibrariesPath, "Win64");
            }
            else
            {
                LibrariesPath = Path.Combine(LibrariesPath, "Win32");
            }

            if (Target.Configuration == UnrealTargetConfiguration.Debug)
            {
                LibrariesPath = Path.Combine(LibrariesPath, "Debug");
            }
            else
            {
                LibrariesPath = Path.Combine(LibrariesPath, "Release");
            }

            PublicAdditionalLibraries.Add(Path.Combine(LibrariesPath, "v8_base_0.lib"));
            PublicAdditionalLibraries.Add(Path.Combine(LibrariesPath, "v8_base_1.lib"));
            PublicAdditionalLibraries.Add(Path.Combine(LibrariesPath, "v8_base_2.lib"));
            PublicAdditionalLibraries.Add(Path.Combine(LibrariesPath, "v8_base_3.lib"));
            PublicAdditionalLibraries.Add(Path.Combine(LibrariesPath, "v8_libbase.lib"));
            PublicAdditionalLibraries.Add(Path.Combine(LibrariesPath, "v8_libplatform.lib"));
            
            PublicAdditionalLibraries.Add(Path.Combine(LibrariesPath, "v8_nosnapshot.lib"));

            if (ShouldLink_libsampler)
            {
                PublicAdditionalLibraries.Add(Path.Combine(LibrariesPath, "v8_libsampler.lib"));
            }
            

            Definitions.Add(string.Format("WITH_V8=1"));
            Definitions.Add(string.Format("WITH_V8_FAST_CALL=0"));
            Definitions.Add(string.Format("WITH_JSWEBSOCKET=1"));

            return true;
        }
        else if (Target.Platform == UnrealTargetPlatform.Android)
        {
            string LibrariesPath = Path.Combine(ThirdPartyPath, "v8", "lib", "Android");
            PublicLibraryPaths.Add(Path.Combine(LibrariesPath, "ARMv7"));
            PublicLibraryPaths.Add(Path.Combine(LibrariesPath, "ARM64"));
            PublicLibraryPaths.Add(Path.Combine(LibrariesPath, "x86"));
            PublicLibraryPaths.Add(Path.Combine(LibrariesPath, "x64"));

            PublicAdditionalLibraries.Add("v8_base");
            PublicAdditionalLibraries.Add("v8_libbase");
            PublicAdditionalLibraries.Add("v8_libplatform");
            PublicAdditionalLibraries.Add("v8_nosnapshot");

            if (ShouldLink_libsampler)
            {
                PublicAdditionalLibraries.Add("v8_libsampler");
            }

            Definitions.Add(string.Format("WITH_V8=1"));
            Definitions.Add(string.Format("WITH_V8_FAST_CALL=0"));
            Definitions.Add(string.Format("WITH_JSWEBSOCKET=0"));

            return true;
        }
        else if (Target.Platform == UnrealTargetPlatform.Linux)
        {
            string LibrariesPath = Path.Combine(ThirdPartyPath, "v8", "lib", "Linux");
            PublicLibraryPaths.Add(Path.Combine(LibrariesPath, "x64"));

            PublicAdditionalLibraries.Add("v8_base");
            PublicAdditionalLibraries.Add("v8_libbase");
            PublicAdditionalLibraries.Add("v8_libplatform");
            PublicAdditionalLibraries.Add("v8_nosnapshot");

            if (ShouldLink_libsampler)
            {
                PublicAdditionalLibraries.Add("v8_libsampler");
            }

            Definitions.Add(string.Format("WITH_V8=1"));
            Definitions.Add(string.Format("WITH_V8_FAST_CALL=0"));
            Definitions.Add(string.Format("WITH_JSWEBSOCKET=0"));

            return true;
        }
        else if (Target.Platform == UnrealTargetPlatform.Mac)
        {
            string LibrariesPath = Path.Combine(ThirdPartyPath, "v8", "lib", "Mac", "x64");
            PublicLibraryPaths.Add(LibrariesPath);

            PublicAdditionalLibraries.Add(Path.Combine(LibrariesPath,"libv8_base.a"));
            PublicAdditionalLibraries.Add(Path.Combine(LibrariesPath,"libv8_libbase.a"));
            PublicAdditionalLibraries.Add(Path.Combine(LibrariesPath,"libv8_libplatform.a"));
            PublicAdditionalLibraries.Add(Path.Combine(LibrariesPath,"libv8_nosnapshot.a"));

            if (ShouldLink_libsampler)
            {
                PublicAdditionalLibraries.Add(Path.Combine(LibrariesPath, "libv8_libsampler.a"));
            }

            Definitions.Add(string.Format("WITH_V8=1"));
            Definitions.Add(string.Format("WITH_V8_FAST_CALL=0"));
            Definitions.Add(string.Format("WITH_JSWEBSOCKET=0"));

            return true;
        }
        Definitions.Add(string.Format("WITH_V8=0"));
        Definitions.Add(string.Format("WITH_V8_FAST_CALL=0"));
        Definitions.Add(string.Format("WITH_JSWEBSOCKET=0"));
        return false;
    }
}
