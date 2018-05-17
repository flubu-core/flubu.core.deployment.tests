﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Scripting;

namespace BuildScript
{
    public class BuildScript : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
        }

        protected override void ConfigureTargets(ITaskContext session)
        {
            session.CreateTarget("Unzip.deploy.package")
                .Do(UnzipDeployPackages)
                .Do(Deploy)
                .DoAsync(this.TestWebApi);
        }

        protected void Deploy(ITaskContext context)
        {
            context.Tasks()
                .CopyFileTask(@".\DeploymentConfig.net462.json", "C:\\DeploymentTests\\DeployPackages\\FlubuCore.WebApi-Net462\\DeploymentConfig.json", true).Execute(context);

            context.Tasks().RunProgramTask("C:\\DeploymentTests\\DeployPackages\\FlubuCore.WebApi-Net462\\build.exe")
                .WorkingFolder("C:\\DeploymentTests\\DeployPackages\\FlubuCore.WebApi-Net462").Execute(context);

            context.Tasks()
                .CopyFileTask(@".\DeploymentConfig.NetCoreApp1.1-Linux.json", "C:\\DeploymentTests\\DeployPackages\\FlubuCore.WebApi-NetCoreApp1.1-LinuxMacInstaller\\DeploymentConfig.json", true).Execute(context);

            context.Tasks().RunProgramTask("C:\\DeploymentTests\\DeployPackages\\FlubuCore.WebApi-NetCoreApp1.1-LinuxMacInstaller\\build.exe")
                .WorkingFolder("C:\\DeploymentTests\\DeployPackages\\FlubuCore.WebApi-NetCoreApp1.1-LinuxMacInstaller").Execute(context);

            context.Tasks()
                .CopyFileTask(@".\DeploymentConfig.NetCoreApp2.0-Linux.json", "C:\\DeploymentTests\\DeployPackages\\FlubuCore.WebApi-NetCoreApp2.0-LinuxMacInstaller\\DeploymentConfig.json", true).Execute(context);

            context.Tasks().RunProgramTask("C:\\DeploymentTests\\DeployPackages\\FlubuCore.WebApi-NetCoreApp2.0-LinuxMacInstaller\\build.exe")
                .WorkingFolder("C:\\DeploymentTests\\DeployPackages\\FlubuCore.WebApi-NetCoreApp2.0-LinuxMacInstaller").Execute(context);

            context.Tasks()
                .CopyFileTask(@".\DeploymentConfig.NetCoreApp1.1-Windows.json", "C:\\DeploymentTests\\DeployPackages\\FlubuCore.WebApi-NetCoreApp1.1-WindowsInstaller\\DeploymentConfig.json", true).Execute(context);

            context.Tasks().RunProgramTask("C:\\DeploymentTests\\DeployPackages\\FlubuCore.WebApi-NetCoreApp1.1-WindowsInstaller\\build.exe")
                .WorkingFolder("C:\\DeploymentTests\\DeployPackages\\FlubuCore.WebApi-NetCoreApp1.1-WindowsInstaller").Execute(context);

            context.Tasks()
                .CopyFileTask(@".\DeploymentConfig.NetCoreApp2.0-Windows.json", "C:\\DeploymentTests\\DeployPackages\\FlubuCore.WebApi-NetCoreApp2.0-WindowsInstaller\\DeploymentConfig.json", true).Execute(context);

            context.Tasks().RunProgramTask("C:\\DeploymentTests\\DeployPackages\\FlubuCore.WebApi-NetCoreApp2.0-WindowsInstaller\\build.exe")
                .WorkingFolder("C:\\DeploymentTests\\DeployPackages\\FlubuCore.WebApi-NetCoreApp2.0-WindowsInstaller").Execute(context);
        }

        protected void UnzipDeployPackages(ITaskContext context)
        {
            var zips = Directory.GetFiles(".", "*.zip").ToList();

            if (zips.Count == 0)
            {
                throw new TaskExecutionException("No zip files found.", 1);
            }

            context.Tasks().CreateDirectoryTask("C:\\DeploymentTests", true).Execute(context);
            context.Tasks().CreateDirectoryTask("C:\\DeploymentTests\\DeployPackages", true).Execute(context);
            foreach (var zip in zips)
            {
                context.LogInfo($"Found zip file: {zip}.");
                if (zip.StartsWith(@".\FlubuCore.WebApi-Net462"))
                {
                    context.LogInfo($"Unziping '{zip}'.");
                    context.Tasks().UnzipTask(zip, "C:\\DeploymentTests\\DeployPackages\\FlubuCore.WebApi-Net462").Execute(context);
                }

                if (zip.StartsWith(@".\FlubuCore.WebApi-NetCoreApp2.0-WindowsInstaller"))
                {
                    context.LogInfo($"Unziping '{zip}'.");
                    context.Tasks().UnzipTask(zip, "C:\\DeploymentTests\\DeployPackages\\FlubuCore.WebApi-NetCoreApp2.0-WindowsInstaller").Execute(context);
                }

                if (zip.StartsWith(@".\FlubuCore.WebApi-NetCoreApp2.0-LinuxMacInstaller"))
                {
                    context.LogInfo($"Unziping '{zip}'.");
                    context.Tasks().UnzipTask(zip, "C:\\DeploymentTests\\DeployPackages\\FlubuCore.WebApi-NetCoreApp2.0-LinuxMacInstaller").Execute(context);
                }

                if (zip.StartsWith(@".\FlubuCore.WebApi-NetCoreApp1.1-WindowsInstaller"))
                {
                    context.LogInfo($"Unziping '{zip}'.");
                    context.Tasks().UnzipTask(zip, "C:\\DeploymentTests\\DeployPackages\\FlubuCore.WebApi-NetCoreApp1.1-WindowsInstaller").Execute(context);
                }

                if (zip.StartsWith(@".\FlubuCore.WebApi-NetCoreApp1.1-LinuxMacInstaller"))
                {
                    context.LogInfo($"Unziping '{zip}'.");
                    context.Tasks().UnzipTask(zip, "C:\\DeploymentTests\\DeployPackages\\FlubuCore.WebApi-NetCoreApp1.1-LinuxMacInstaller").Execute(context);
                }
            }
        }

        protected async Task TestWebApi(ITaskContext context)
        {
            var client = context.Tasks().CreateHttpClient("http://localhost");
            var result = await client.GetAsync("http://localhost/Flubu/api/healthcheck");
            if (!result.IsSuccessStatusCode)
            {
                throw new TaskExecutionException("Flubu web api net462 not working properly", 0);
            }
        }
    }
}
