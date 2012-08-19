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
	#region Class: HangarBayToolItem

	public class HangarBayToolItem : PartToolItemBase
	{
		#region Constructor

		public HangarBayToolItem(EditorOptions options)
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
				return "Hangar Bay";
			}
		}
		public override string Description
		{
			get
			{
				return "Stores and repairs smaller ships";
			}
		}
		public override string Category
		{
			get
			{
				return PartToolItemBase.CATEGORY_CONTAINER;
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
			return new HangarBayDesign(this.Options);
		}

		#endregion
	}

	#endregion
	#region Class: HangarBayDesign

	public class HangarBayDesign : PartDesignBase
	{
		#region Constructor

		public HangarBayDesign(EditorOptions options)
			: base(options) { }

		#endregion

		#region Public Properties

		public override PartDesignAllowedScale AllowedScale
		{
			get
			{
				return PartDesignAllowedScale.X_Y_Z;
			}
		}
		public override PartDesignAllowedRotation AllowedRotation
		{
			get
			{
				return PartDesignAllowedRotation.X_Y_Z;
			}
		}

		private Model3DGroup _geometry = null;
		public override Model3D Model
		{
			get
			{
				if (_geometry == null)
				{
					_geometry = CreateGeometry(false);
				}

				return _geometry;
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
			const double BOXWIDTHHALF = .47d;
			const double PLATEHEIGHT = .1d;
			const double PLATEHEIGHTHALF = PLATEHEIGHT * .5d;
			const double TRIMWIDTH = .1d;
			const double TRIMWIDTHHALF = TRIMWIDTH * .5d;

			Model3DGroup retVal = new Model3DGroup();

			GeometryModel3D geometry;
			MaterialGroup material;
			DiffuseMaterial diffuse;
			SpecularMaterial specular;

			#region Main Box

			geometry = new GeometryModel3D();
			material = new MaterialGroup();
			diffuse = new DiffuseMaterial(new SolidColorBrush(this.Options.WorldColors.HangarBay));
			this.MaterialBrushes.Add(new MaterialColorProps(diffuse, diffuse.Brush, this.Options.WorldColors.HangarBay));
			material.Children.Add(diffuse);
			specular = this.Options.WorldColors.HangarBaySpecular;
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

			if (isFinal)
			{
				geometry.Geometry = UtilityWPF.GetCube_IndependentFaces(new Point3D(-.5, -.5, -.5), new Point3D(.5, .5, .5));
			}
			else
			{
				geometry.Geometry = UtilityWPF.GetCube_IndependentFaces(new Point3D(-BOXWIDTHHALF, -BOXWIDTHHALF, -BOXWIDTHHALF), new Point3D(BOXWIDTHHALF, BOXWIDTHHALF, BOXWIDTHHALF));
			}

			retVal.Children.Add(geometry);

			#endregion

			if (!isFinal)
			{
				#region Plates

				for (int cntr = -1; cntr <= 1; cntr += 2)
				{
					geometry = new GeometryModel3D();
					material = new MaterialGroup();
					diffuse = new DiffuseMaterial(new SolidColorBrush(this.Options.WorldColors.HangarBayTrim));
					this.MaterialBrushes.Add(new MaterialColorProps(diffuse, diffuse.Brush, this.Options.WorldColors.HangarBayTrim));
					material.Children.Add(diffuse);
					specular = this.Options.WorldColors.HangarBayTrimSpecular;
					this.MaterialBrushes.Add(new MaterialColorProps(specular));
					material.Children.Add(specular);

					//if (!isFinal)
					//{
					EmissiveMaterial selectionEmissive = new EmissiveMaterial(Brushes.Transparent);
					material.Children.Add(selectionEmissive);
					this.SelectionEmissives.Add(selectionEmissive);
					//}

					geometry.Material = material;
					geometry.BackMaterial = material;

					double z = (.5d - PLATEHEIGHTHALF) * cntr;
					geometry.Geometry = UtilityWPF.GetCube_IndependentFaces(new Point3D(-.5d, -.5d, z - PLATEHEIGHTHALF), new Point3D(.5d, .5d, z + PLATEHEIGHTHALF));

					retVal.Children.Add(geometry);
				}

				#endregion

				#region Supports

				for (int xCntr = -1; xCntr <= 1; xCntr += 2)
				{
					for (int yCntr = -1; yCntr <= 1; yCntr += 2)
					{
						geometry = new GeometryModel3D();
						material = new MaterialGroup();
						diffuse = new DiffuseMaterial(new SolidColorBrush(this.Options.WorldColors.HangarBayTrim));
						this.MaterialBrushes.Add(new MaterialColorProps(diffuse, diffuse.Brush, this.Options.WorldColors.HangarBayTrim));
						material.Children.Add(diffuse);
						specular = this.Options.WorldColors.HangarBayTrimSpecular;
						this.MaterialBrushes.Add(new MaterialColorProps(specular));
						material.Children.Add(specular);

						//if (!isFinal)
						//{
						EmissiveMaterial selectionEmissive = new EmissiveMaterial(Brushes.Transparent);
						material.Children.Add(selectionEmissive);
						this.SelectionEmissives.Add(selectionEmissive);
						//}

						geometry.Material = material;
						geometry.BackMaterial = material;

						double x = (.5d - TRIMWIDTHHALF) * xCntr;
						double y = (.5d - TRIMWIDTHHALF) * yCntr;
						geometry.Geometry = UtilityWPF.GetCube_IndependentFaces(new Point3D(x - TRIMWIDTHHALF, y - TRIMWIDTHHALF, -.5d), new Point3D(x + TRIMWIDTHHALF, y + TRIMWIDTHHALF, .5d));

						retVal.Children.Add(geometry);
					}
				}

				#endregion
			}

			//	Transform
			Transform3DGroup transform = new Transform3DGroup();
			if (isFinal)
			{
				transform.Children.Add(_scaleTransform.Clone());
				transform.Children.Add(new RotateTransform3D(_rotateTransform.Clone()));
				transform.Children.Add(_translateTransform.Clone());
			}
			else
			{
				transform.Children.Add(_scaleTransform);
				transform.Children.Add(new RotateTransform3D(_rotateTransform));
				transform.Children.Add(_translateTransform);
			}
			retVal.Transform = transform;

			//	Exit Function
			return retVal;
		}

		#endregion
	}

	#endregion
	#region Class: HangarBay

	public class HangarBay
	{
		public const string PARTTYPE = "HangarBay";
	}

	#endregion
}