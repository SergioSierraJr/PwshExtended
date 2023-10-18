using System.Management.Automation;
using System.Xml;

namespace PwshExtended.PathUtils;

[Cmdlet(VerbsCommon.Add, "Path")]
public class AddPath : Cmdlet
{
    [Parameter(ValueFromPipeline = true)]
    public string? Path { get; set; }
    
    
    protected override void ProcessRecord()
    {
        var pathToXml = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)+"/.config/powershell/ExtraPaths.xml";
        var xmlData = new XmlDocument();
        if (!File.Exists(pathToXml))
        {
            Console.WriteLine("Add-Path: Creating ExtraPaths.xml...");
            xmlData.AppendChild(xmlData.CreateXmlDeclaration("1.0", "UTF-8", "yes"));
            xmlData.AppendChild(xmlData.CreateElement("root"));
            xmlData.SelectSingleNode("root")?.AppendChild(xmlData.CreateElement("ExtraPaths"));
            xmlData.Save(pathToXml);
        }
        xmlData.Load(pathToXml);

        if (Environment.GetEnvironmentVariable("PATH")!.Split(':').Contains(Path))
            throw new Exception("Error! " + Path + " is already in path!");
        if (!File.Exists(Path) && !Directory.Exists(Path))
            throw new Exception("Error! " + Path + " does not exist!");
        
        var pathToBeAdded = xmlData.CreateElement("Path");
        pathToBeAdded.InnerText = Path;
        xmlData.SelectSingleNode("root/ExtraPaths")?.AppendChild(pathToBeAdded);
        SyncPath.RefreshPathFunc();
        xmlData.Save(pathToXml);
    }
}