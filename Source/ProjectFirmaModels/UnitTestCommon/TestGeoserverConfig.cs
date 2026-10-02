using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using LtInfo.Common.DesignByContract;
using NUnit.Framework;


namespace ProjectFirmaModels.UnitTestCommon
{
    [TestFixture]
    public class GeoserverConfigTest
    {
        // Might need to vary, but I'm hoping it's consistent
        public const string SvnSitkaGeoserverWorkspacesRootDir = "c:\\svn\\sitkatech\\trunk\\ProjectFirma\\Geoserver\\data_dir\\workspaces";

        public static List<string> GetWorkspaceDirectories()
        {
            var workspaceDirectories =  Directory.GetDirectories(SvnSitkaGeoserverWorkspacesRootDir).ToList();
            return workspaceDirectories;
        }

        public static string GetDatastoreXmlFilePathForWorkspaceDir(string workspaceDir)
        {
            var datastoreFilePath = Path.Combine(workspaceDir, "GeoServerSqlDatasource\\datastore.xml");
            return datastoreFilePath;
        }

        /// <summary>
        /// Retrieve the "37bb41ce:1612f9e6bee:-8888" portion of
        ///
        ///  <id>DataStoreInfoImpl--37bb41ce:1612f9e6bee:-8888</id>
        ///
        /// from the datastore.xml file given.
        /// </summary>
        /// <param name="datastoreXmlFilePath">C:\SVN\Sitkatech\Trunk\ProjectFirma\GeoServer\data_dir\workspaces\AshlandDemoProjectFirma\GeoServerSqlDataSource\datastore.xml for example</param>
        /// <param name="IdValueKey">"DataStoreInfoImpl" for example</param>
        /// <returns></returns>
        public static string GetIdValue(string datastoreXmlFilePath, string IdValueKey)
        {
            string xmlFileContents = File.ReadAllText(datastoreXmlFilePath);
            const string idValueMatchGroupLabel = "IDValue";

            string idRegexString = $"<id>{IdValueKey}(-*)(?<{idValueMatchGroupLabel}>.*)</id>";
            Regex regex = new Regex(idRegexString);

            var match = regex.Match(xmlFileContents);
            Check.Ensure(match.Success, $"Could not find {IdValueKey} in file path {datastoreXmlFilePath}");

            return match.Groups[idValueMatchGroupLabel].Value;
        }

        [Test]
        public void TestGeoserverHasUniqueDataStoreInfoImplPerProjectFirmaWorkspace()
        {
            const string dataStoreInfoImpl = "DataStoreInfoImpl";

            List<String> workspaceDirs = GetWorkspaceDirectories();
            // Sanity check - all workspaces should be distinct. This shouldn't ever happen, but you never know...
            bool anyDuplicateWorkpaceNames = workspaceDirs.GroupBy(wd => wd).Any(c => c.Count() > 1);
            Check.Ensure(!anyDuplicateWorkpaceNames, $"Found duplicate workspaces where we weren't expecting them!");

            var workspaceDirNameToDataStoreInfoImplDict = new Dictionary<string, string>();
            foreach (string workspaceDir in workspaceDirs)
            {
                string pathToCurrentDatastoreXmlFile = GetDatastoreXmlFilePathForWorkspaceDir(workspaceDir);
                string dataStoreInfoImplValue = GetIdValue(pathToCurrentDatastoreXmlFile, dataStoreInfoImpl);
                workspaceDirNameToDataStoreInfoImplDict.Add(workspaceDir, dataStoreInfoImplValue);
            }

            // Retrieve all duplicate values for the workspaces
            var lookupForDuplicateValues = workspaceDirNameToDataStoreInfoImplDict.ToLookup(x => x.Value, x => x.Key).Where(x => x.Count() > 1);
            string errorString = string.Empty;
            foreach (var dupeItem in lookupForDuplicateValues)
            {
                var keys = dupeItem.Aggregate("", (s, v) => s + ", " + v);
                string message = "The following keys have the value " + dupeItem.Key + ":" + keys;
                errorString += message;
            }

            Check.Ensure(errorString == string.Empty, $"Found duplicate values for {dataStoreInfoImpl} in the following Geoserver datastore.xml workspaces: {errorString}. These values must be distinct across Geoserver Workspaces, and should not be copied.");
        }
    }
}