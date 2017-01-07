#r System
#load "build-util.csx"

using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

string commitMessage = Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_MESSAGE");
Console.WriteLine("commit message: \"{0}\"", commitMessage);

bool publishNuget = commitMessage.IndexOf("[publish nuget]", StringComparison.InvariantCultureIgnoreCase) >= 0;
bool publishDocs = commitMessage.IndexOf("[publish docs]", StringComparison.InvariantCultureIgnoreCase) >= 0;

if (publishNuget) { Console.WriteLine("going to publish to nuget.org"); }
if (publishDocs) { Console.WriteLine("going to publish docs"); }

string nugetApiKey;
if (publishNuget)
{
    nugetApiKey = Environment.GetEnvironmentVariable("NUGET_API_KEY");
    if (string.IsNullOrWhiteSpace(nugetApiKey))
    {
        Console.Error.WriteLine("cannot publish to nuget.org without %NUGET_API_KEY%");
        Environment.Exit(1);
    }
}

string githubToken;
if (publishDocs)
{
    githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
    if (string.IsNullOrWhiteSpace(githubToken))
    {
        Console.Error.WriteLine("cannot publish docs without %GITHUB_TOKEN%");
        Environment.Exit(1);
    }
}

string versionDLL;
string versionNupkg;

using (TextReader tr = new StreamReader("versions.txt"))
{
    versionDLL = tr.ReadLine().Split(":".ToCharArray())[1].Trim();
    versionNupkg = tr.ReadLine().Split(":".ToCharArray())[1].Trim();
}

string configuration = Environment.GetEnvironmentVariable("configuration");
Console.WriteLine("configuration: {0}", configuration);
Console.WriteLine("Versions:");
Console.WriteLine("  - DLLs  : {0}", versionDLL);
Console.WriteLine("  - nupkg : {0}", versionNupkg);


string rootDir = Environment.GetEnvironmentVariable("ROOTDIR");

//////// PATCH SharedAssemblyInfo.cs
string sharedAssemblyInfo = Path.Combine(
    rootDir
    , "src"
    , "SharedAssemblyInfo.cs"
);
Console.WriteLine("Patching [{0}] to version [{1}]", sharedAssemblyInfo, versionDLL);
string assemblyInfo;
using (TextReader tr = new StreamReader(sharedAssemblyInfo, Encoding.UTF8))
{
    assemblyInfo = tr.ReadToEnd();
}
Console.WriteLine("old assemblyInfo:" + Environment.NewLine + assemblyInfo);
Regex regex = new Regex("AssemblyVersion\\(\"([^)]*)");
assemblyInfo = regex.Replace(assemblyInfo, string.Format("AssemblyVersion(\"{0}\"", versionDLL));
regex = new Regex("AssemblyFileVersion\\(\"([^)]*)");
assemblyInfo = regex.Replace(assemblyInfo, string.Format("AssemblyFileVersion(\"{0}\"", versionDLL));
Console.WriteLine("new assemblyInfo:" + Environment.NewLine + assemblyInfo);

using (TextWriter tw = new StreamWriter(sharedAssemblyInfo, false, Encoding.UTF8))
{
    tw.Write(assemblyInfo);
}
//////// PATCH SharedAssemblyInfo.cs


string buildCmd = string.Format(
    "msbuild MapboxSDKUnityCore.sln /p:Configuration={0}",
    configuration
);
Console.WriteLine("building [{0}]", buildCmd);
if (!RunCommand(buildCmd, true))
{
    Console.Error.WriteLine("build failed");
    Environment.Exit(1);
}


string nugetCmd = string.Format("nuget pack -properties version={0};configuration={1}", versionNupkg, configuration);
Console.WriteLine("creating nupkg: [{0}]", nugetCmd);
if (!RunCommand(nugetCmd, true))
{
    Console.Error.WriteLine("creation of nupkg failed");
    Environment.Exit(1);
}




if (!publishNuget)
{
    Console.WriteLine("NOT publishing to nuget.org");
}
else
{
    Console.WriteLine("publishing to nuget.org");
    string nugetCmd = string.Format("nuget push MapboxSDKforUnityCore.{0}.nupkg {1} -Source https://www.nuget.org/api/v2/package", versionNupkg, nugetApiKey);
    if (!RunCommand(nugetCmd))
    {
        Console.Error.WriteLine("publishing to nuget.org failed");
        Environment.Exit(1);
    }
}


if (!publishDocs)
{
    Console.WriteLine("NOT publishing docs");
}
else
{
    Console.WriteLine("publishing dcos");
    try
    {
        string originalCommit = Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT");
        if (string.IsNullOrWhiteSpace(originalCommit))
        {
            originalCommit = "no SHA available";
        }
        else
        {
            originalCommit = "https://github.com/mapbox/mapbox-sdk-unity-core/commit/" + originalCommit;
        }

        string commitAuthor = Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOR");
        if (string.IsNullOrWhiteSpace(commitAuthor))
        {
            commitAuthor = "no commit author available";
        }

        string docsDir = Path.Combine(rootDir, "src", "Documentation", "_site");
        Console.WriteLine("docs directory: {0}", docsDir);
        Environment.CurrentDirectory = docsDir;
        List<string> cmds = new List<string>(new string[]{
            //"dir",
            "git init .",
            "git add .",
            string.Format("git commit -m \"pushed via [{0}] by [{1}]\"", originalCommit,commitAuthor),
            string.Format("git remote add origin https://{0}@github.com/mapbox/mapbox-sdk-unity.git", githubToken),
            "git checkout -b gh-pages",
            "git push -f origin gh-pages"
        });
        foreach (var cmd in cmds)
        {
            if (!RunCommand(cmd))
            {
                Console.Error.WriteLine("publishing docs failed");
                Environment.Exit(1);
            }
        }
    }
    finally
    {
        Environment.CurrentDirectory = rootDir;
    }
}
