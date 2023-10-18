using System.Management.Automation;
using System.Xml;

namespace PwshExtended.PathUtils;

[Cmdlet(VerbsData.Sync, "Path")]
public class SyncPath : Cmdlet
{
    [Parameter(ValueFromPipeline = true)]
    public string? Path { get; set; }

    protected override void ProcessRecord() => RefreshPathFunc();
    
    public static void RefreshPathFunc()
    {
        var pathToXml = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)+"/.config/powershell/ExtraPaths.xml";
        if (!File.Exists(pathToXml))
            throw new Exception("Error! ExtraPaths.xml does not exist");
        var xmlData = new XmlDocument();
        xmlData.Load(pathToXml);
        foreach (XmlElement existingPath in xmlData.SelectNodes("root/ExtraPaths/Path")!)
        {
            var oldPath = Environment.GetEnvironmentVariable("PATH");
            if (oldPath != null && oldPath.Split(":").Contains(existingPath.InnerText))
                continue;
            if (oldPath != null && oldPath[^1] != ':')
                oldPath += ':';
            var newPath = oldPath + existingPath.InnerText;
            Environment.SetEnvironmentVariable("PATH", newPath);
        }
    }
}