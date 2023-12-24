namespace wDNS.Knowledge;

public class HostFileReader(ILogger<HostFileReader> logger, IKnowledgeProvider knowledge)
{
    public async Task Read(DirectoryInfo directory)
    {
        var count = 0;

        foreach (var file in directory.EnumerateFiles())
        {
            var hosts = await Read(file);
            knowledge.Add(hosts);

            count++;
        }

        logger.LogInformation("Added {Count} hosts files", count);
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
