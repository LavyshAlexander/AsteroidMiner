﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using Game.Newt.AsteroidMiner2.ShipEditor;
using Game.Newt.HelperClasses;

namespace Game.Newt.AsteroidMiner2.ShipParts
{
    #region Class: ProjectileGunToolItem

    public class ProjectileGunToolItem : PartToolItemBase
    {
        #region Constructor

        public ProjectileGunToolItem(EditorOptions options)
            : base(options)
        {
            _visual2D = PartToolItemBase.GetVisual2D(this.Name, this.Description, options.EditorColors);
            this.TabName = PartToolItemBase.TAB_SHIPPART;
        }

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return "Projectile Weapon";
            }
        }
        public override string Description
        {
            get
            {
                return "Fires projectiles from ammo box";
            }
        }
        public override string Category
        {
            get
            {
                return PartToolItemBase.CATEGORY_WEAPON;
            }
        }

        private UIElement _visual2D = null;
        public override UIElement Visual2D
        {
            get
            {
                return _visual2D;
            }
        }

        #endregion

        #region Public Methods

        public override PartDesignBase GetNewDesignPart()
        {
            return new ProjectileGunDesign(this.Options);
        }

        #endregion
    }

    #endregion
    #region Class: ProjectileGunDesign

    public class ProjectileGunDesign : PartDesignBase
    {
        #region Declaration Section

        public const PartDesignAllowedScale ALLOWEDSCALE = PartDesignAllowedScale.XYZ;		// This is here so the scale can be known through reflection

        #endregion

        #region Constructor

        public ProjectileGunDesign(EditorOptions options)
            : base(options) { }

        #endregion

        #region Public Properties

        public override PartDesignAllowedScale AllowedScale
        {
            get
            {
                return ALLOWEDSCALE;
            }
        }
        public override PartDesignAllowedRotation AllowedRotation
        {
            get
            {
                return PartDesignAllowedRotation.X_Y_Z;
            }
        }

        private Model3DGroup _geometries = null;
        public override Model3D Model
        {
            get
            {
                if (_geometries == null)
                {
                    _geometries = CreateGeometry(false);
                }

                return _geometries;
            }
        }

        #endregion

        #region Public Methods

        public override Model3D GetFinalModel()
        {
            return CreateGeometry(true);
        }

        #endregion

        #region Private Methods

        private Model3DGroup CreateGeometry(bool isFinal)
        {
            Model3DGroup retVal = new Model3DGroup();

            GeometryModel3D geometry;
            MaterialGroup material;
            DiffuseMaterial diffuse;
            SpecularMaterial specular;

            //int domeSegments = isFinal ? 2 : 10;
            int cylinderSegments = isFinal ? 6 : 35;

            #region Mount Box

            geometry = new GeometryModel3D();
            material = new MaterialGroup();
            diffuse = new DiffuseMaterial(new SolidColorBrush(WorldColors.GunBase));
            this.MaterialBrushes.Add(new MaterialColorProps(diffuse, WorldColors.GunBase));
            material.Children.Add(diffuse);
            specular = WorldColors.GunBaseSpecular;
            this.MaterialBrushes.Add(new MaterialColorProps(specular));
            material.Children.Add(specular);

            if (!isFinal)
            {
                EmissiveMaterial selectionEmissive = new EmissiveMaterial(Brushes.Transparent);
                material.Children.Add(selectionEmissive);
                this.SelectionEmissives.Add(selectionEmissive);
            }

            geometry.Material = material;
            geometry.BackMaterial = material;

            geometry.Geometry = UtilityWPF.GetCube(new Point3D(-.115, -.1, -.5), new Point3D(.115, .1, -.1));

            retVal.Children.Add(geometry);

            #endregion

            #region Barrel

            geometry = new GeometryModel3D();
            material = new MaterialGroup();
            diffuse = new DiffuseMaterial(new SolidColorBrush(WorldColors.GunBarrel));
            this.MaterialBrushes.Add(new MaterialColorProps(diffuse, WorldColors.GunBarrel));
            material.Children.Add(diffuse);
            specular = WorldColors.GunBarrelSpecular;
            this.MaterialBrushes.Add(new MaterialColorProps(specular));
            material.Children.Add(specular);

            if (!isFinal)
            {
                EmissiveMaterial selectionEmissive = new EmissiveMaterial(Brushes.Transparent);
                material.Children.Add(selectionEmissive);
                this.SelectionEmissives.Add(selectionEmissive);
            }

            geometry.Material = material;
            geometry.BackMaterial = material;

            const double OUTERRADIUS = .045;
            const double INNERRADIUS = .04;

            List<TubeRingBase> rings = new List<TubeRingBase>();

            rings.Add(new TubeRingRegularPolygon(0, false, OUTERRADIUS * 1.75d, OUTERRADIUS * 1.75d, false));		// Start at the base of the barrel
            rings.Add(new TubeRingRegularPolygon(.49, false, OUTERRADIUS * 1.75d, OUTERRADIUS * 1.75d, false));
            rings.Add(new TubeRingRegularPolygon(.01, false, OUTERRADIUS, OUTERRADIUS, false));
            rings.Add(new TubeRingRegularPolygon(.4, false, OUTERRADIUS, OUTERRADIUS, false));
            rings.Add(new TubeRingRegularPolygon(.01, false, OUTERRADIUS * 1.25d, OUTERRADIUS * 1.25d, false));
            rings.Add(new TubeRingRegularPolygon(.08, false, OUTERRADIUS * 1.25d, OUTERRADIUS * 1.25d, false));		// This is the tip of the barrel
            rings.Add(new TubeRingRegularPolygon(0, false, INNERRADIUS, INNERRADIUS, false));		// Curl to the inside
            rings.Add(new TubeRingRegularPolygon(-.95d, false, INNERRADIUS, INNERRADIUS, false));		// Loop back to the base

            geometry.Geometry = UtilityWPF.GetMultiRingedTube(cylinderSegments, rings, true, false, new TranslateTransform3D(0, 0, -.49d));

            retVal.Children.Add(geometry);

            #endregion

            #region Trim

            geometry = new GeometryModel3D();
            material = new MaterialGroup();
            diffuse = new DiffuseMaterial(new SolidColorBrush(WorldColors.GunTrim));
            this.MaterialBrushes.Add(new MaterialColorProps(diffuse, WorldColors.GunTrim));
            material.Children.Add(diffuse);
            specular = WorldColors.GunTrimSpecular;
            this.MaterialBrushes.Add(new MaterialColorProps(specular));
            material.Children.Add(specular);

            if (!isFinal)
            {
                EmissiveMaterial selectionEmissive = new EmissiveMaterial(Brushes.Transparent);
                material.Children.Add(selectionEmissive);
                this.SelectionEmissives.Add(selectionEmissive);
            }

            geometry.Material = material;
            geometry.BackMaterial = material;

            geometry.Geometry = UtilityWPF.GetCube(new Point3D(-.095, -.11, -.45), new Point3D(.095, .11, -.15));

            retVal.Children.Add(geometry);

            #endregion

            // Transform
            retVal.Transform = GetTransformForGeometry(isFinal);

            // Exit Function
            return retVal;
        }

        #endregion
    }

    #endregion
    #region Class: ProjectileGun

    public class ProjectileGun
    {
        public const string PARTTYPE = "ProjectileGun";
    }

    #endregion
}
