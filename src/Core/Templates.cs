namespace CnSharp.Updater
{
    public sealed class Templates
    {
        public const string ManifestXml =
            @"<Manifest xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <Id>$id$</Id>
  <AppName>$id$</AppName>
  <Owner>$owner$</Owner>
  <Description><![CDATA[$description$]]></Description>
  <Copyright>$copyright$</Copyright>
  <MinVersion>$version$</MinVersion>
  <ReleaseUrl></ReleaseUrl>
  <Version>$version$</Version>
  <ShortcutIcon></ShortcutIcon>
  <ReleaseNotes><![CDATA[]]>
  </ReleaseNotes>
  <Files>
  </Files>
</Manifest>";

        public const string IgnoreFiles = @".log
.pdb
*.vshost.
updater.exe
";
    }
}
