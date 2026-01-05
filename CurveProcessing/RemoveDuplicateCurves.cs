using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weevil.Core;

namespace Weevil.CurveProcessing
{
    public class RemoveDuplicateCurves :GH_Component
    {
        public RemoveDuplicateCurves()
            : base("Remove Duplicate Curves", "RemDupCrv",
                  "Removes Duplicate curves from a list based on geometric similarity",
                   WeevilConstants.Category, WeevilConstants.Curves
                  )
        { }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "List of curves to filter", GH_ParamAccess.list);
            pManager.AddNumberParameter("Tolerance", "T", "Tolerance for curve comparison", GH_ParamAccess.item, 0.001);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Unique Curves", "U", "List of Unique Curves", GH_ParamAccess.list);
            pManager.AddBooleanParameter("IsDuplicated", "D", "Boolean List represent duplicated curves from input curves", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var curves = new List<Curve>();
            double tolerance = 0.001;

            if (!DA.GetDataList(0, curves)) return;
            DA.GetData(1, ref tolerance);

            var uniqueCurves = new List<Curve>();
            var isDuplicateList = new List<bool>();

            for (int i = 0; i < curves.Count; i++)
            {
                if (curves[i] == null)
                {
                    isDuplicateList.Add(false);
                    continue;
                }

                bool isDuplicated = false;

                foreach (Curve unique in uniqueCurves)
                {
                    if (RemoveDuplicateCurves.AreCurvesDuplicated(curves[i], unique, tolerance))
                    {
                        isDuplicated = true;
                        break;
                    }
                }

                isDuplicateList.Add(isDuplicated);

                if(!isDuplicated)
                {
                    uniqueCurves.Add(curves[i]);
                }
            }

            DA.SetDataList(0, uniqueCurves);
            DA.SetDataList(1, isDuplicateList);
        }

        private static bool AreCurvesDuplicated(Curve c1, Curve c2, double tolerance)
        {
            // Quick length check
            if (Math.Abs(c1.GetLength() - c2.GetLength()) > tolerance)
                return false;

            // Sample points along both curves
            int sampleCount = 10;
            for (int i = 0; i <= sampleCount; i++)
            {
                double t = (double)i / sampleCount;
                Point3d p1 = c1.PointAtNormalizedLength(t);
                Point3d p2 = c2.PointAtNormalizedLength(t);

                if (p1.DistanceTo(p2) > tolerance)
                {
                    // Try reversed direction
                    Point3d p2Rev = c2.PointAtNormalizedLength(1.0 - t);
                    if (p1.DistanceTo(p2Rev) > tolerance)
                        return false;
                }
            }
            return true;
        }
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("A1B2C3D4-E5F6-4A5B-8C9D-0E1F2A3B4C5D");
    }
}
