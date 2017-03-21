#r "System"
#load "build-util.csx"

using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

string access_token = Environment.GetEnvironmentVariable("MAPBOX_ACCESS_TOKEN");
if(string.IsNullOrWhiteSpace(access_token)){
    Console.Error.WriteLine("%MAPBOX_ACCESS_TOKEN% not set - cannot run tests");
    Environment.Exit(1);
}

//ATTENTION: latest version of `srciptcs` seems to change the current directory
string rootDir = Environment.GetEnvironmentVariable("ROOTDIR");
Console.WriteLine("rootDir [build.csx]: {0}", rootDir);
Console.WriteLine("cwd [build.csx]: {0}", Directory.GetCurrentDirectory());
Directory.SetCurrentDirectory(rootDir);
Console.WriteLine("cwd [build.csx]: {0}", Directory.GetCurrentDirectory());
string commitMessage = Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_MESSAGE");
Console.WriteLine("commit message: \"{0}\"", commitMessage);

string configuration = Environment.GetEnvironmentVariable("configuration");
Console.WriteLine($"configuration: \"{configuration}\"");

bool publishNuget = commitMessage.IndexOf("[publish nuget]", StringComparison.InvariantCultureIgnoreCase) >= 0;
bool publishDocs = commitMessage.IndexOf("[publish docs]", StringComparison.InvariantCultureIgnoreCase) >= 0;

//publish docs only on 'DebugNet' configuration
publishDocs = publishDocs && configuration == "DebugNet";

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

Console.WriteLine("configuration: {0}", configuration);
Console.WriteLine("Versions:");
Console.WriteLine("  - DLLs  : {0}", versionDLL);
Console.WriteLine("  - nupkg : {0}", versionNupkg);


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
    "msbuild MapboxSdkCs.sln /p:Configuration={0}",
    configuration
);
Console.WriteLine("building [{0}]", buildCmd);
if (!RunCommand(buildCmd, true))
{
    Console.Error.WriteLine("build failed");
    Environment.Exit(1);
}


string nugetCmd = string.Format("nuget pack -properties version={0}", versionNupkg);
Console.WriteLine("creating nupkg: [{0}]", nugetCmd);
Console.WriteLine("Skipping nuget pack! TODO: Build 'DebugNet' and 'DebugUWP' on one configuration!");
// if (!RunCommand(nugetCmd, true))
// {
//     Console.Error.WriteLine("creation of nupkg failed");
//     Environment.Exit(1);
// }




if (!publishNuget)
{
    Console.WriteLine("NOT publishing to nuget.org");
}
else
{
    Console.WriteLine("publishing to nuget.org");
    string nugetCmd = string.Format("nuget push MapboxSdkCs.{0}.nupkg {1} -Source https://www.nuget.org/api/v2/package", versionNupkg, nugetApiKey);
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
            originalCommit = "https://github.com/mapbox/mapbox-sdk-cs/commit/" + originalCommit;
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
            string.Format("git remote set-url --add --push origin https://{0}@github.com/mapbox/mapbox-sdk-unity.git", githubToken),
            string.Format("git remote set-url --add --push origin https://{0}@github.com/mapbox/mapbox-sdk-cs.git", githubToken),
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
