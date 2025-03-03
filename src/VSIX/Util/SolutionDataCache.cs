using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CnSharp.VisualStudio.Extensions;
using EnvDTE;

namespace CnSharp.VisualStudio.SharpUpdater.Util
{
    public class SolutionDataCache : ConcurrentDictionary<string,SolutionProperties>
    {
        private static SolutionDataCache instance;
        protected SolutionDataCache()
        {
           
        }

        public static SolutionDataCache Instance => instance ?? (instance = new SolutionDataCache());

        public SolutionProperties GetSolutionProperties(string solutionFile,int retryTimes = 3)
        {
            int i = 0;
            SolutionProperties sp;
            while (!TryGetValue(solutionFile,out sp))
            {
                System.Threading.Thread.Sleep(500);
                i++;
                if(i == retryTimes)
                {
                    throw new ApplicationException("Load projects failed.");
                }
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
                if (_projects != null)
                {
                    ClassicProjects.AddRange(_projects?.Where(p => p.IsNetFrameworkProject()));
                    SdkBasedProjects.AddRange(_projects?.Where(p => p.IsSdkBased()));
                }
                else
                {
                    ClassicProjects.Clear();
                    SdkBasedProjects.Clear();
                }
            }
        }

        public List<Project> ClassicProjects { get; private set; } = new List<Project>();
        public List<Project> SdkBasedProjects { get; private set; } = new List<Project>();
        public bool HasClassicProjects => ClassicProjects?.Any() == true;
        public bool HasSdkBasedProjects => SdkBasedProjects?.Any() == true;

        public void AddProject(Project project)
        {
            if(_projects == null) _projects = new List<Project>();
            _projects.Add(project);
            if (project.IsNetFrameworkProject())
            {
               ClassicProjects.Add(project);
            }
            else if (project.IsSdkBased())
            {
               SdkBasedProjects.Add(project);
            }
        }

        public void RemoveProject(Project project)
        {
            _projects.Remove(project);
            if (project.IsNetFrameworkProject())
            {
                ClassicProjects.Remove(project);
            }
            else if (project.IsSdkBased())
            {
                SdkBasedProjects.Remove(project);
            }
        }
    }
}
