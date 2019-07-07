#addin "Cake.Incubator&version=5.0.1"

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Task("CleanSolution")
    .Does(() =>
    {
        var solution = ParseSolution(".\\Fs.sln");

        foreach(var project in solution.Projects)
        {
            // check solution items and exclude solution folders, since they are virtual
            if(project.Name == "Solution Items")
                continue;

            var customProject = ParseProject(project.Path, configuration: configuration, platform: "AnyCPU");

            foreach(var path in customProject.OutputPaths)
                CleanDirectory(path.FullPath);
        }
});

Task("UpdateAssemblyInfo")
    .Does(() =>
    {
        var assemblyInfoParseResult = ParseAssemblyInfo("SharedAssemblyInfo.cs");
        var settings = new AssemblyInfoSettings()
        {
            Version                 = assemblyInfoParseResult.AssemblyVersion,
            FileVersion             = assemblyInfoParseResult.AssemblyFileVersion,
            InformationalVersion    = assemblyInfoParseResult.AssemblyInformationalVersion,

            Product                 = assemblyInfoParseResult.Product,
            Company                 = assemblyInfoParseResult.Company,
            Trademark               = assemblyInfoParseResult.Trademark,
            Copyright               = $"© {DateTime.Today.Year} Insire",

            InternalsVisibleTo      = assemblyInfoParseResult.InternalsVisibleTo,

            MetaDataAttributes = new []
            {
                new AssemblyInfoMetadataAttribute()
                {
                    Key = "Platform",
                    Value = "AnyCPU",
                },
                new AssemblyInfoMetadataAttribute()
                {
                    Key = "Compiled on:",
                    Value = "[UTC]" + DateTime.UtcNow.ToString(),
                },
            }
        };

        if (BuildSystem.IsLocalBuild)
        {
            settings.Version                 = Increase(settings.Version);
            settings.FileVersion             = Increase(settings.FileVersion);
            settings.InformationalVersion    = Increase(settings.InformationalVersion);
        }
        else
        {
            if(BuildSystem.IsRunningOnAzurePipelinesHosted)
            {
                var build = int.Parse(EnvironmentVariable("BUILD_BUILDNUMBER") ?? "no version found from AzurePipelinesHosted");
                settings.Version                 = IncreaseWith(settings.Version, build);
                settings.FileVersion             = IncreaseWith(settings.FileVersion, build);
                settings.InformationalVersion    = IncreaseWith(settings.InformationalVersion, build);

                Information($"Version: {settings.Version}");
                Information($"FileVersion: {settings.FileVersion}");
                Information($"InformationalVersion: {settings.InformationalVersion}");
            }
            else
            {
                var version = EnvironmentVariable("APPVEYOR_BUILD_VERSION") ?? "no version found from AppVeyorEnvironment";
                settings.Version                 = version;
                settings.FileVersion             = version;
                settings.InformationalVersion    = version;

                Information($"Version: {version}");
            }
        }

        CreateAssemblyInfo(new FilePath("SharedAssemblyInfo.cs"), settings);

        string Increase(string data)
        {
            var version = new Version(data);
            return new Version(version.Major,version.Minor,version.Build+1, version.Revision).ToString();
        }

        string IncreaseWith(string data, int build)
        {
            var version = new Version(data);
            return new Version(version.Major,version.Minor,build, version.Revision).ToString();
        }
});

Task("Build")
   .Does(()=>
   {
      var version = string.Empty;
      if (BuildSystem.AppVeyor.IsRunningOnAppVeyor)
      {
         version = EnvironmentVariable("APPVEYOR_BUILD_VERSION") ?? "no version found from AppVeyorEnvironment";
      }
      else
      {
         var assemblyInfoParseResult = ParseAssemblyInfo("SharedAssemblyInfo.cs");
         version = assemblyInfoParseResult.AssemblyVersion;
      }

      Build(@".\FS\FS.csproj");
      
      void Build(string path)
      {
         var settings = new ProcessSettings()
            .UseWorkingDirectory(".")
            .WithArguments(builder => builder
               .Append("build")
               .AppendQuoted(path)
               .Append("--force")
               .Append($"-c {configuration}")
         );

         StartProcess("dotnet", settings);
      }
   });


///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
   .IsDependentOn("CleanSolution")
   .IsDependentOn("UpdateAssemblyInfo")
   .IsDependentOn("Build");

RunTarget(target);