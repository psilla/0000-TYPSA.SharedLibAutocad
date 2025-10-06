using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using System.Text;

namespace TYPSA.SharedLib.Autocad.ProcessRegion
{
    public class cls_00_ProcessRegionResult
    {
        public class ProcessRegionResult
        {
            public List<Region> ValidRegions { get; set; }
            public HashSet<ObjectId> FailedRegionPolylines { get; set; }
            public Dictionary<Handle, string> FailedPolylineMessages { get; set; }
            public Dictionary<Handle, Region> HandleToRegion { get; set; }
            public Dictionary<Handle, Handle> PolyToRegionMap { get; set; }
            public StringBuilder InfoSummary { get; set; }
            public int Total { get; set; }
            public int NullCount { get; set; }
            public int ValidCount { get; set; }
        }


    }
}
