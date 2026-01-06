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
    public class RemoveAlignedContainedCurves : GH_Component
    {
        public RemoveAlignedContainedCurves()
            : base("Remove Aligned Contained Curves", "RemAligned",
                  "Removes curves that are fully aligned and contained within other curves",
                  WeevilConstants.Category, WeevilConstants.Curves)
        { }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "List of curves to filter", GH_ParamAccess.list);
            pManager.AddNumberParameter("Tolerance", "T", "Alignment Tolerance", GH_ParamAccess.item, 0.01);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Filtered Curves", "F", "Curves with aligned Contained curves removed", GH_ParamAccess.list);
            pManager.AddBooleanParameter("IsAligned", "A", "Boolean List represent Aligned curves from input curves", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var curves = new List<Curve>();
            var tolerance = 0.01;

            if (!DA.GetDataList(0, curves)) return;
            DA.GetData(1, ref tolerance);

            if (curves == null || curves.Count == 0) return;

            var filteredCurves = new List<Curve>();
            var isAligned = new List<bool>();

            bool[] toRemove = new bool[curves.Count];

            for (int i = 0; i < curves.Count; i++)
            {
                if (toRemove[i]) continue;

                Curve curveA = curves[i];
                if (curveA == null) continue;

                for (int j = 0; j < curves.Count; j++)
                {
                    if (i == j || toRemove[j]) continue;

                    Curve curveB = curves[j];
                    if (curveB == null) continue;

                    // Check if curveA is fully contained and aligned within curveB
                    if (IsFullyAlignedAndContained(curveA, curveB, tolerance))
                    {
                        toRemove[i] = true;
                        break;
                    }
                }
            }
            // Build output lists
            for (int i = 0; i < curves.Count; i++)
            {
                isAligned.Add(toRemove[i]);

                if (!toRemove[i])
                {
                    filteredCurves.Add(curves[i]);
                }
            }

            DA.SetDataList(0, filteredCurves);
            DA.SetDataList(1, isAligned);
        }

        }
        private static bool IsFullyAlignedAndContained(Curve innerCurve, Curve outerCurve, double tolerance)
        {
            // first check: see if the innercurve endings are actually withing the outercurve
            Point3d innerStart = innerCurve.PointAtStart;
            Point3d innerEnd = innerCurve.PointAtEnd;

            double tStart, tEnd;
            outerCurve.ClosestPoint(innerStart, out tStart);
            outerCurve.ClosestPoint(innerStart, out tEnd);

            Point3d outerStart = outerCurve.PointAt(tStart);
            Point3d outerEnd = outerCurve.PointAt(tEnd);

            if (innerStart.DistanceTo(outerStart) > tolerance) return false;
            if (innerEnd.DistanceTo(outerEnd) > tolerance) return false;

            // Second check: see if the innercurve samples are actually withing the outercurve samples
            int sampleCount = 10;
            for (int i = 0; i <= sampleCount; i++)
            {
                //divide curve to points
                double t = (double)i / sampleCount;
                Point3d samplePt = innerCurve.PointAtNormalizedLength(t);
                //get those points on the other curve and see if they are very close to each other
                double tOuter;
                outerCurve.ClosestPoint(samplePt, out tOuter);
                Point3d closestPt = outerCurve.PointAt(tOuter);

                if (samplePt.DistanceTo(closestPt) > tolerance) return false;

            }

            // Third Check: Verify that inner curve is shorter than outer curve
            double innerLength = innerCurve.GetLength();
            double outerLength = outerCurve.GetLength();
            if (innerLength > outerLength - tolerance)
                return false;

            return true;
        }
        public override Guid ComponentGuid
        {
            get { return new Guid("A1B2C3D4-E5F6-4A5B-8C9D-0E1F2A3B4C5D"); }
        }

        protected override System.Drawing.Bitmap Icon
        {
            get { return null; }
        }
    }
}
