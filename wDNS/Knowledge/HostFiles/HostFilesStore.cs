﻿using System.Reflection;

namespace wDNS.Knowledge.HostFiles;

public class HostFilesStore(ILogger<HostFilesStore> logger) : KnowledgeOrganizer
{
    public override async Task Initialize()
    {
        logger.LogInformation("Reading {{conf}} directory");

#if DEBUG
        Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        logger.LogDebug("Set current directory to {{{CurrentDirectory}}}", Environment.CurrentDirectory);
#endif

        var confDir = new DirectoryInfo("conf");
        confDir.Create();

        logger.LogInformation("Reading Hosts files");
        var hostsDir = confDir.CreateSubdirectory("hosts");

        await Read(hostsDir);
    }

    public async Task Read(DirectoryInfo directory)
    {
        var count = 0;

        foreach (var file in directory.EnumerateFiles())
        {
            var hosts = await Read(file);
            Add(hosts);

            count++;
        }

        logger.LogInformation("Loaded {Count} hosts files", count);
    }

    // TODO: Since this is async, the load order is not guaranteed. I should fix that.
    private async Task<HostFile> Read(FileInfo hostsFile)
    {
        using var stream = hostsFile.OpenRead();
        using var reader = new StreamReader(stream);

        var hosts = await HostFile.Read(reader);
        logger.LogDebug("Hosts file {{{File}}} contains {Count} questions", hostsFile.Name, hosts.Answers.Count);

        return hosts;
    }
}
