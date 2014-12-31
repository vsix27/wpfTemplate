namespace Vsix.Viewer.Helpers
{
  public  class ManifestHelper
    {
      /// <summary>
      /// 
      /// </summary>
      /// <param name="guid">CBA50698-0FCB-4F74-8E13-AE1F05871CCE</param>
      /// <param name="projectName">Wpf Project with MVVM, MVP...</param>
      /// <param name="projectVersion">1.3</param>
      /// <param name="projectDescription">Empty VSIX Project with MVVM, MVP</param>
      /// <param name="author"></param>
      /// <returns></returns>
      public static string SampleVersion1Xml(string guid, string projectName, string projectVersion, string projectDescription,string author)
      {
          return string .Format(@"<?xml version='1.0' encoding='utf-8'?>
<Vsix xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' Version='1.0.0' xmlns='http://schemas.microsoft.com/developer/vsx-schema/2010'>
  <Identifier Id='{0}'>
    <Name>{1}</Name>
    <Description xml:space='preserve'>{2}</Description>
    <Version>{3}</Version>
    <Author>{4}</Author>
    <Locale>1033</Locale>
    <InstalledByMsi>false</InstalledByMsi>
    <SupportedProducts>
      <VisualStudio Version='10.0'>        <Edition>Pro</Edition>      </VisualStudio>
      <VisualStudio Version='11.0'>        <Edition>Pro</Edition>      </VisualStudio>
      <VisualStudio Version='12.0'>        <Edition>Pro</Edition>      </VisualStudio>
      <VisualStudio Version='13.0'>        <Edition>Pro</Edition>      </VisualStudio>
      <VisualStudio Version='14.0'>        <Edition>Pro</Edition>      </VisualStudio>
    </SupportedProducts>
    <SupportedFrameworkRuntimeEdition MinVersion='3.5' />
  </Identifier>

  <References>
    <Reference Id='Microsoft.VisualStudio.MPF' MinVersion='10.0'>
      <Name>Visual Studio MPF</Name>
    </Reference>
  </References>

  <Content>
    <VsPackage>|%CurrentProject%;PkgdefProjectOutputGroup|</VsPackage>
    <MefComponent>|%CurrentProject%|</MefComponent>
  </Content>
</Vsix>", guid, projectName, projectDescription, projectVersion, author);
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="guid">b553d51a-30d4-433b-863d-7f67a93c460a</param>
      /// <param name="projectName"></param>
      /// <returns></returns>
      public static string SampleVersion2Xml(string guid, string projectName, string projectVersion, string author)
      {
          return string .Format( @"<?xml version='1.0' encoding='utf-8'?>
         <PackageManifest Version='2.0.0' xmlns='http://schemas.microsoft.com/developer/vsx-schema/2011'>
    <Metadata>
      <Identity Id='VSIXProjectForWpfTemplate..{0}' Version='{1}' Language='en-US' Publisher='{2}' />
      <DisplayName>VSIXProjectForWpfTemplate</DisplayName>
      <Description>Empty VSIX Project.</Description>
    </Metadata>
    <Installation  InstalledByMsi='false'>
      <InstallationTarget Id='Microsoft.VisualStudio.Pro' Version='[14.0)' />
    </Installation>
    <Dependencies>
      <Dependency Id='Microsoft.Framework.NDP' DisplayName='Microsoft .NET Framework' Version='[4.5,)' />
    </Dependencies>
    <Assets>
      <Asset Type='Microsoft.VisualStudio.ItemTemplate' Path='Output\ItemTemplates' />
      <Asset Type='Microsoft.VisualStudio.ProjectTemplate' Path='Output\ProjectTemplates' />
    </Assets>
  </PackageManifest>",
                     guid,  projectVersion, author);
           
      }
    }
}
