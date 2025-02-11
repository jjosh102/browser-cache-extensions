using System;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.IO;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[GitHubActions("deploy-to-nuget",
    GitHubActionsImage.UbuntuLatest,
    OnPushTags = new[] { "v*" },  // Runs when pushing a tag like "v1.0.0"
    InvokedTargets = new[] { nameof(Pack), nameof(Publish) },
    ImportSecrets = new[] { "NUGET_API_KEY" })]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Pack);

    [Parameter] readonly string Configuration = "Release";

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    Target Clean => _ => _
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").DeleteDirectories();
            ArtifactsDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            SourceDirectory.GlobFiles(SourceDirectory, "**/*.csproj")
            .ForEach(project => DotNetRestore(s => s.SetProjectFile(project)));
        });

    Target Compile => _ => _
     .DependsOn(Restore)
     .Executes(() =>
     {
         SourceDirectory.GlobFiles(SourceDirectory, "**/*.csproj")
             .ForEach(project => DotNetBuild(s => s
                 .SetProjectFile(project)
                 .SetConfiguration(Configuration)
                 .EnableNoRestore()));
     });


    Target Pack => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            foreach (var project in SourceDirectory.GlobFiles("**/*.csproj"))
            {
                DotNetPack(s => s
                    .SetProject(project)
                    .SetConfiguration(Configuration)
                    .SetOutputDirectory(ArtifactsDirectory));
            }
        });

    bool IsRunningOnGitHubActions => Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true";

    Target Publish => _ => _
        .DependsOn(Pack)
        .OnlyWhenDynamic(() => IsRunningOnGitHubActions)
        .Requires(() => Environment.GetEnvironmentVariable("NUGET_API_KEY"))
        .Executes(() =>
        {
            SourceDirectory.GlobFiles(ArtifactsDirectory, "*.nupkg")
                .ForEach(package =>
                {
                    DotNetNuGetPush(s => s
                        .SetTargetPath(package)
                        .SetSource("https://api.nuget.org/v3/index.json")
                        .SetApiKey(Environment.GetEnvironmentVariable("NUGET_API_KEY")));
                });
        });
}
