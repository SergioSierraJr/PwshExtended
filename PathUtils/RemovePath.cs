using System.Management.Automation;
using System.Xml;

namespace PwshExtended.PathUtils;

[Cmdlet(VerbsCommon.Remove, "Path")]
public class RemovePath : Cmdlet
{
    [Parameter(ValueFromPipeline = true)]
    public string? Path { get; set; }

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
        var xmlData = new XmlDocument();
        xmlData.Load(pathToXml);
        foreach (XmlElement existingPath in xmlData.SelectNodes("root/ExtraPaths/Path")!)
            if (existingPath.InnerText.Equals(Path))
            {
                existingPath.ParentNode?.RemoveChild(existingPath);
                xmlData.Save(pathToXml);
                break;
            }

        SyncPath.RefreshPathFunc();

    }
}