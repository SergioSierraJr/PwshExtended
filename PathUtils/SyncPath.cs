using System.Management.Automation;
using System.Xml;

namespace PwshExtended.PathUtils;

[Cmdlet(VerbsData.Sync, "Path")]
public class SyncPath : Cmdlet
{
    protected override void ProcessRecord()
    {
        var pathToXml = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)+"/.config/powershell/ExtraPaths.xml";
        if (!File.Exists(pathToXml))
        {
            ErrorRecord errorRecord = new ErrorRecord(
                new Exception("ExtraPaths.xml does not exist"),
                null,
                ErrorCategory.ResourceUnavailable,
                this);
            ThrowTerminatingError(errorRecord);
        }
        RefreshPathFunc();
    }
    
    public static void RefreshPathFunc()
    {
        var pathToXml = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)+"/.config/powershell/ExtraPaths.xml";
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