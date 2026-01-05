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
    /// <summary>
    /// Component that joins aligned Lines based on their directional angle
    /// </summary>
    public class JoinAlignedCurves : GH_Component
    {
        public JoinAlignedCurves()
            : base("JoinAlignedCurves",
                  "JoinAligned",
                  "joins curves that are directionally aligned within a specified angle tolerance",
                  WeevilConstants.Category, WeevilConstants.Curves
                  )
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "Curves to clean and join", GH_ParamAccess.list);
            pManager.AddNumberParameter("Deviation Angle", "A", "Maximum angle deviation in degrees for curves to be considered aligned", GH_ParamAccess.item, 2.0);
            // prevent warning 
            pManager[1].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Joined Curves", "C", "Joined Curves", GH_ParamAccess.list);
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // init input vars
            var curves = new List<Curve>();
            var angleTolerance = 2.0;

            // get inputparams, add them to vars
            if (!DA.GetDataList(0, curves)) return;
            DA.GetData(1, ref angleTolerance);

            // null check curves
            if (curves == null || curves.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No curves provided");
                return;
            }

            // init working lists
            var remaining = new List<Curve>(curves);
            var result = new List<Curve>();

            /// <summary>
            /// Takes first curve, find all aligned curves, join them together, repeat until done
            /// </summary>

            // process curves until all are evaluated
            while (remaining.Count > 0)
            {
                // create a group with its first element from the remaining list 
                Curve baseCrv = remaining[0];
                remaining.RemoveAt(0);
                var group = new List<Curve> { baseCrv };

                // get direction at both curve endings
                var baseStart = baseCrv.TangentAtStart;
                var baseEnd = baseCrv.TangentAtEnd;

                // find all curves aligned with the base curve 
                for (int i = remaining.Count - 1; i >= 0; i--)
                {
                    Curve other = remaining[i];

                    var otherStart = other.TangentAtStart;
                    var otherEnd = other.TangentAtEnd;

                    double angle1 = Vector3d.VectorAngle(baseStart, otherStart) * (180.0 / Math.PI);
                    double angle2 = Vector3d.VectorAngle(baseStart, otherEnd) * (180.0 / Math.PI);
                    double angle3 = Vector3d.VectorAngle(baseEnd, otherStart) * (180.0 / Math.PI);
                    double angle4 = Vector3d.VectorAngle(baseEnd, otherEnd) * (180.0 / Math.PI);

                    // Find the minimum angle among all combinations
                    double minAngle = Math.Min(Math.Min(angle1, angle2), Math.Min(angle3, angle4));

                    // If aligned within tolerance at any end combination, add to group
                    if (minAngle < angleTolerance)
                    {
                        group.Add(other);
                        remaining.RemoveAt(i);
                    }
                }
                // Attempt to join all curves in the group
                Curve[] joined = Curve.JoinCurves(group);

                // Add results to output list
                if (joined.Length > 0)
                    result.AddRange(joined);
                else
                    result.AddRange(group); // Fallback if joining fails
            }
            // Set output data
            DA.SetDataList(0, result);

        }   
        protected override System.Drawing.Bitmap Icon
        {
            get { return null; }
        }

        public override Guid ComponentGuid => new Guid("FD6C0395-3D14-48D1-A055-C39798920EC0");
    }

}
