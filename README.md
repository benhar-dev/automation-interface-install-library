# Simple example of installing a library using the Automation Interface

## Disclaimer
This is a personal guide not a peer reviewed journal or a sponsored publication. We make
no representations as to accuracy, completeness, correctness, suitability, or validity of any
information and will not be liable for any errors, omissions, or delays in this information or any
losses injuries, or damages arising from its display or use. All information is provided on an as
is basis. It is the reader’s responsibility to verify their own facts.

The views and opinions expressed in this guide are those of the authors and do not
necessarily reflect the official policy or position of any other agency, organization, employer or
company. Assumptions made in the analysis are not reflective of the position of any entity
other than the author(s) and, since we are critically thinking human beings, these views are
always subject to change, revision, and rethinking at any time. Please do not hold us to them
in perpetuity.

## Overview 
This is a simple example showing a console app, which sets up a new project in order to install a library in to TwinCAT.  

## Getting Started
You only need to run the csharp-app.  The TwinCAT project is the source of the library file. 

## Code Snippets
The main code is as follows

```csharp
using OleMessageFilter;
using System;
using System.IO;
using TCatSysManagerLib;

namespace AutomationInterfaceCode
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {

            // setup the message filter
            MessageFilter.Register();

            // temp folder
            string tempFolderPath = @"c:\tempProject\";
            if (Directory.Exists(tempFolderPath))
            {
                Directory.Delete(tempFolderPath, true);
            }
            Directory.CreateDirectory(tempFolderPath);

            // useful paths
            string currentAssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string currentDirectory = Path.GetDirectoryName(currentAssemblyPath);
            string srcFolderPath = Path.Combine(currentDirectory, "..", "..", "..", "..", "..");

            // application specific paths
            string templateProjectPath = @"C:\TwinCAT\3.1\Components\Base\PrjTemplate\TwinCAT Project.tsproj";
            string templatePlcProjectName = "Standard PLC Template.plcproj";
            string solutionFileName = "TestSolution.sin";

            Type t = Type.GetTypeFromProgID("TcXaeShell.DTE.15.0");
            EnvDTE.DTE dte = (EnvDTE.DTE)Activator.CreateInstance(t);

            dte.SuppressUI = true;
            dte.MainWindow.Visible = false;

            dynamic solution = dte.Solution;
            dynamic project = solution.AddFromTemplate(templateProjectPath, tempFolderPath, solutionFileName);

            ITcSysManager sysManager = project.Object;
            ITcSmTreeItem plc = sysManager.LookupTreeItem("TIPC");

            plc.CreateChild("TestProject", 0, "", templatePlcProjectName);
            ITcSmTreeItem references = sysManager.LookupTreeItem("TIPC^TestProject^TestProject Project^References");
            ITcPlcLibraryManager libManager = (ITcPlcLibraryManager)references;

            string libraryFilePath = Path.Combine(srcFolderPath, "lib", "DemoLibrary.library");
            string absoluteLibraryFilePath = Path.GetFullPath(libraryFilePath);
            libManager.InstallLibrary("System", absoluteLibraryFilePath, true);

            dte.Quit();

            // remove the message filter
            MessageFilter.Revoke();

        }
    }
}
```

## Versions
* TcXaeShell 3.1.4024.47

## Need more help?
Please visit http://beckhoff.com/ for further guides
