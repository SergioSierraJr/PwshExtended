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
        var xmlData = new XmlDocument();
        if (!File.Exists(pathToXml))
            throw new Exception("Error! There are no extra paths to remove");
        xmlData.Load(pathToXml);
        foreach (XmlElement existingPath in xmlData.SelectNodes("root/ExtraPaths/Path")!)
            if (existingPath.InnerText.Equals(Path))
            {
                existingPath.ParentNode?.RemoveChild(existingPath);
                xmlData.Save(pathToXml);
                break;
            }

        string newPath = "";
        foreach (var pathSection in Environment.GetEnvironmentVariable("PATH")?.Split(":")!)
            if (pathSection != Path && pathSection.Length > 0)
                if (pathSection[^1].Equals(':'))
                    newPath += pathSection;
                else
                    newPath += pathSection + ":";
        
        Environment.SetEnvironmentVariable("PATH", newPath);
        
    }
}