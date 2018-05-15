using System;
using System.IO;
using System.Linq;
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
                .Do(Deploy);
        }

        protected void Deploy(ITaskContext context)
        {
            context.Tasks().RunProgramTask("C:\\DeploymentTests\\DeployPackages\\FlubuCore.WebApi-Net462\\build.exe")
                .WorkingFolder("C:\\DeploymentTests\\DeployPackages\\FlubuCore.WebApi-Net462").Execute(context);
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
            }
        }
    }
}
