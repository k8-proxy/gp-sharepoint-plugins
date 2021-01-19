using System;
using System.IO;
using System.Linq;
using static Bullseye.Targets;
using static SimpleExec.Command;

namespace build
{
    class Program
    {
        private const string Prefix = "FileHandler";
        private const string publishOutput = "../../artifacts";
        private const string envVarMissing = " environment variable is missing. Aborting.";

        private static class Targets
        {
            public const string CleanBuildOutput = "clean-build-output";
            public const string CleanPackOutput = "clean-pack-output";
            public const string Build = "build";
            public const string Test = "test";
            public const string Publish = "publish";
            public const string CopyPackOutput = "copy-pack-output";
        }

        static void Main(string[] args)
        {
            Target(Targets.CleanBuildOutput, () =>
            {
                Run("dotnet", "clean -c Release -v m --nologo", echoPrefix: Prefix);
            });

            Target(Targets.Build, DependsOn(Targets.CleanBuildOutput), () =>
            {
                Run("dotnet", "build -c Release --nologo", echoPrefix: Prefix);
            });
            
            Target(Targets.CleanPackOutput, () =>
            {
                if (Directory.Exists(publishOutput))
                {
                    Directory.Delete(publishOutput, true);
                }
            });

            Target(Targets.Publish, DependsOn(Targets.Build, Targets.CleanPackOutput), () =>
            {
                Run("dotnet", $"publish -c Release -o \"{Directory.CreateDirectory(publishOutput).FullName}\" --no-build --nologo", echoPrefix: Prefix);
            });

            Target("default", DependsOn(Targets.Publish));

            RunTargetsAndExit(args, ex => ex is SimpleExec.NonZeroExitCodeException || ex.Message.EndsWith(envVarMissing), Prefix);
        }
    }
}
