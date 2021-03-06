﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

using Game.HelperClassesCore;
using Game.Newt.v2.GameItems.MapParts;
using Game.HelperClassesWPF;

namespace Game.Newt.v2.GameItems
{
    /// <summary>
    /// This is a cache of visual elements that can be shared across multiple items (meshes and stuff)
    /// </summary>
    public class SharedVisuals
    {
        public SharedVisuals(double starSize = .25)
        {
            _starSize = starSize;
        }

        private readonly double _starSize;
        private Geometry3D _starMesh = null;
        public Geometry3D StarMesh
        {
            get
            {
                if (_starMesh == null)
                {
                    //_starMesh = UtilityWPF.GetSquare2D(.25d);		// can't use a 2D anymore, because they can rotate the camera
                    _starMesh = UtilityWPF.GetCube(_starSize);
                }

                return _starMesh;
            }
        }

        private SortedList<MineralType, Geometry3D> _mineralMeshes = new SortedList<MineralType, Geometry3D>();
        public Geometry3D GetMineralMesh(MineralType mineralType)
        {
            if (!_mineralMeshes.ContainsKey(mineralType))
            {
                #region Cache Geometry

                int numSides;
                List<TubeRingDefinition_ORIG> rings = new List<TubeRingDefinition_ORIG>();

                double sqrt2 = Math.Sqrt(2d);

                switch (mineralType)
                {
                    case MineralType.Ice:
                        #region Ice

                        // Going for an ice cube  :)

                        numSides = 4;
                        rings.Add(new TubeRingDefinition_ORIG(sqrt2, sqrt2, 0d, true, false));
                        rings.Add(new TubeRingDefinition_ORIG(sqrt2, sqrt2, 1d, true, false));

                        #endregion
                        break;

                    case MineralType.Iron:
                        #region Iron

                        // This will be an iron bar (with rust)

                        numSides = 4;
                        rings.Add(new TubeRingDefinition_ORIG(.8d, .8d, 0d, true, false));
                        rings.Add(new TubeRingDefinition_ORIG(1d, 1d, .2d, true, false));
                        rings.Add(new TubeRingDefinition_ORIG(1d, 1d, 1.1d, true, false));
                        rings.Add(new TubeRingDefinition_ORIG(.8d, .8d, .2d, true, false));

                        #endregion
                        break;

                    case MineralType.Graphite:
                        #region Graphite

                        // A shiny lump of coal

                        numSides = 4;
                        rings.Add(new TubeRingDefinition_ORIG(sqrt2 * .75d, sqrt2 * .75d, 0d, true, false));
                        rings.Add(new TubeRingDefinition_ORIG(sqrt2, sqrt2, .5d, true, false));
                        rings.Add(new TubeRingDefinition_ORIG(sqrt2 * .75d, sqrt2 * .75d, .5d, true, false));

                        #endregion
                        break;

                    case MineralType.Gold:
                        #region Gold

                        // A reflective gold bar
                        //TODO:  Expand the tube ring definition to support making a trapazoid bar

                        numSides = 6;
                        rings.Add(new TubeRingDefinition_ORIG(sqrt2 * .8d, .8d, 0d, true, false));
                        rings.Add(new TubeRingDefinition_ORIG(sqrt2, 1, .3d, true, false));
                        rings.Add(new TubeRingDefinition_ORIG(sqrt2 * .8d, .8d, .3d, true, false));

                        #endregion
                        break;

                    case MineralType.Platinum:
                        #region Platinum

                        // A reflective platinum bar/plate

                        numSides = 8;
                        rings.Add(new TubeRingDefinition_ORIG(sqrt2 * .8d, .8d, 0d, true, false));
                        rings.Add(new TubeRingDefinition_ORIG(sqrt2, 1, .3d, true, false));
                        rings.Add(new TubeRingDefinition_ORIG(sqrt2 * .8d, .8d, .3d, true, false));

                        #endregion
                        break;

                    case MineralType.Emerald:
                        #region Emerald

                        // A semi transparent double trapazoid

                        numSides = 4;
                        rings.Add(new TubeRingDefinition_ORIG(sqrt2 * .75d, sqrt2 * .75d, 0d, true, false));
                        rings.Add(new TubeRingDefinition_ORIG(sqrt2, sqrt2, .25d, true, false));
                        rings.Add(new TubeRingDefinition_ORIG(sqrt2 * .75d, sqrt2 * .75d, .25d, true, false));

                        #endregion
                        break;

                    case MineralType.Saphire:
                        #region Saphire

                        // A jeweled oval

                        numSides = 10;
                        rings.Add(new TubeRingDefinition_ORIG(sqrt2 * .66, sqrt2 * .35, 0d, true, false));
                        rings.Add(new TubeRingDefinition_ORIG(sqrt2 * .85, sqrt2 * .66, .075d, true, false));
                        rings.Add(new TubeRingDefinition_ORIG(sqrt2, sqrt2 * .75, .05d, true, false));
                        rings.Add(new TubeRingDefinition_ORIG(sqrt2 * .85, sqrt2 * .66, .05d, true, false));
                        rings.Add(new TubeRingDefinition_ORIG(sqrt2 * .66, sqrt2 * .35, .075d, true, false));

                        #endregion
                        break;

                    case MineralType.Ruby:
                        #region Ruby

                        // A jeweled oval

                        numSides = 10;
                        rings.Add(new TubeRingDefinition_ORIG(sqrt2 * .66, sqrt2 * .35, 0d, true, false));
                        rings.Add(new TubeRingDefinition_ORIG(sqrt2 * .85, sqrt2 * .66, .075d, true, false));
                        rings.Add(new TubeRingDefinition_ORIG(sqrt2, sqrt2 * .75, .05d, true, false));
                        rings.Add(new TubeRingDefinition_ORIG(sqrt2 * .85, sqrt2 * .66, .05d, true, false));
                        rings.Add(new TubeRingDefinition_ORIG(sqrt2 * .66, sqrt2 * .35, .075d, true, false));

                        #endregion
                        break;

                    case MineralType.Diamond:
                        #region Diamond

                        // A jewel

                        numSides = 8;
                        rings.Add(new TubeRingDefinition_ORIG(.001, .001, 0, true, false));       //NOTE: If the object is going to be semitransparent, then don't use the pyramid overload.  I think it's because the normal points down, and lighting isn't the way you'd expect
                        rings.Add(new TubeRingDefinition_ORIG(1.125d, 1.125d, .6d, true, false));
                        rings.Add(new TubeRingDefinition_ORIG(.56d, .56d, .19d, true, false));

                        #endregion
                        break;

                    case MineralType.Rixium:
                        #region Rixium

                        // A petagon rod
                        // There are also some toruses around it, but they are just visuals.  This rod is the collision mesh

                        numSides = 5;
                        rings.Add(new TubeRingDefinition_ORIG(.001, .001, 0, true, false));       //NOTE: If the object is going to be semitransparent, then don't use the pyramid overload.  I think it's because the normal points down, and lighting isn't the way you'd expect
                        rings.Add(new TubeRingDefinition_ORIG(.8, .8, .3d, true, false));
                        rings.Add(new TubeRingDefinition_ORIG(.8, .8, 1d, true, false));
                        rings.Add(new TubeRingDefinition_ORIG(.001, .001, .3d, true, false));

                        #endregion
                        break;

                    default:
                        throw new ApplicationException("Unknown MineralType: " + mineralType.ToString());
                }

                // Create and store the mesh
                _mineralMeshes.Add(mineralType, UtilityWPF.GetMultiRingedTube_ORIG(numSides, rings, false, true));

                #endregion
            }

            return _mineralMeshes[mineralType];
        }

        private SortedList<double, Geometry3D> _rixiumTorusMeshes = new SortedList<double, Geometry3D>();
        public Geometry3D GetRixiumTorusMesh(double radius)
        {
            if (!_rixiumTorusMeshes.ContainsKey(radius))
            {
                _rixiumTorusMeshes.Add(radius, UtilityWPF.GetTorus(30, 5, .03, radius));
            }

            return _rixiumTorusMeshes[radius];
        }

        [ThreadStatic]
        private static List<Geometry3D> _brainMeshes;
        public static Geometry3D BrainMesh
        {
            get
            {
                List<Geometry3D> localList = _brainMeshes;		// ThreadStatic attribute makes sure this is unique per thread
                if (localList == null)
                {
                    localList = new List<Geometry3D>();
                    _brainMeshes = localList;
                }

                Geometry3D retVal = null;
                if (localList.Count < 10)
                {
                    Point3D[] spherePoints = new Point3D[100];
                    for (int cntr = 0; cntr < spherePoints.Length; cntr++)
                    {
                        spherePoints[cntr] = Math3D.GetRandomVector_Spherical_Shell(.5d).ToPoint();
                    }

                    TriangleIndexed[] sphereTriangles = Math3D.GetConvexHull(spherePoints);

                    retVal = UtilityWPF.GetMeshFromTriangles(sphereTriangles);

                    localList.Add(retVal);
                }
                else
                {
                    retVal = localList[StaticRandom.Next(localList.Count)];
                }

                return retVal;
            }
        }
    }
}
