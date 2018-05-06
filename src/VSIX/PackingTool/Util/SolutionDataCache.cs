using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CnSharp.VisualStudio.Extensions;
using EnvDTE;

namespace CnSharp.VisualStudio.SharpDeploy.Util
{
    public class SolutionDataCache : ConcurrentDictionary<string,SolutionProperties>
    {
        private static SolutionDataCache instance;
        protected SolutionDataCache()
        {
           
        }

        public static SolutionDataCache Instance => instance ?? (instance = new SolutionDataCache());

        public SolutionProperties GetSolutionProperties(string solutionFile)
        {
            SolutionProperties sp;
            while (!TryGetValue(solutionFile,out sp))
            {
                System.Threading.Thread.Sleep(500);
            }
            return sp;
        }
    }

    public class SolutionProperties
    {
        private List<Project> _projects;

        public List<Project> Projects
        {
            get { return _projects; }
            set
            {
                _projects = value;
                ClassicProjects = _projects?.Where(p => p.IsNetFrameworkProject()).ToList();
                SdkBasedProjects = _projects?.Where(p => p.IsSdkBased()).ToList();
            }
        }

        public List<Project> ClassicProjects { get;private set; }
        public List<Project> SdkBasedProjects { get; private set; }
        public bool HasClassicProjects => ClassicProjects?.Any() == true;
        public bool HasSdkBasedProjects => SdkBasedProjects?.Any() == true;
    }
}
