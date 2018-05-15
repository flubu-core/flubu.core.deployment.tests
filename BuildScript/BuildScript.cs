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
                .Do(UnzipDeployPackages);
        }

        protected void UnzipDeployPackages(ITaskContext context)
        {
            var zips = Directory.GetFiles(string.Empty, "*.zip").ToList();

            if (zips.Count == 0)
            {
                throw new TaskExecutionException("No zip files found.", 1);
            }

            context.Tasks().CreateDirectoryTask("C:\\DeploymentTests", true).Execute(context);
            foreach (var zip in zips)
            {
                if (zip.StartsWith("FlubuCore.WebApi-Net462"))
                {
                    context.Tasks().UnzipTask("zip", "C:\\DeploymentTests\\FlubuCore.WebApi-Net46").Execute(context);
                }
            }
        }
    }
}
