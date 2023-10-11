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
