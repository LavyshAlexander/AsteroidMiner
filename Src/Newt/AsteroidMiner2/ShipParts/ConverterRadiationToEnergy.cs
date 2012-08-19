﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using Game.Newt.AsteroidMiner2.ShipEditor;
using Game.Newt.HelperClasses;
using Game.Newt.NewtonDynamics;

namespace Game.Newt.AsteroidMiner2.ShipParts
{
	#region Class: ConverterRadiationToEnergyToolItem

	public class ConverterRadiationToEnergyToolItem : PartToolItemBase
	{
		#region Constructor

		public ConverterRadiationToEnergyToolItem(EditorOptions options, SolarPanelShape shape)
			: base(options)
		{
			this.Shape = shape;
			_visual2D = PartToolItemBase.GetVisual2D(this.Name, this.Description, options.EditorColors);
			this.TabName = PartToolItemBase.TAB_SHIPPART;
		}

		#endregion

		#region Public Properties

		public override string Name
		{
			get
			{
				return "Solar Panel (" + this.Shape.ToString().ToLower().Replace('_', ' ') + ")";
			}
		}
		public override string Description
		{
			get
			{
				return "Converts radiation to energy (solar panel)";
			}
		}
		public override string Category
		{
			get
			{
				return PartToolItemBase.CATEGORY_CONVERTERS;
			}
		}

		public SolarPanelShape Shape
		{
			get;
			private set;
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
			return new ConverterRadiationToEnergyDesign(this.Options, this.Shape);
		}

		#endregion
	}

	#endregion
	#region Class: ConverterRadiationToEnergyDesign

	public class ConverterRadiationToEnergyDesign : PartDesignBase
	{
		#region Declaration Section

		public const double THICKNESS = .1d;

		#endregion

		#region Constructor

		public ConverterRadiationToEnergyDesign(EditorOptions options, SolarPanelShape shape)
			: base(options)
		{
			this.Shape = shape;
		}

		#endregion

		#region Public Properties

		public override PartDesignAllowedScale AllowedScale
		{
			get
			{
				//	This is an odd one.  I want X and Y to be independent, but Z to be some ratio of them
				//	Maybe don't enforce a certain size for Z, but make the solar panel less effective if too thin, and no more effective if too thick (just has more mass, so the extra thickness is useless)
				//	Figure out a way to warn the user if too thin or thick (have a grace range though)
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

		public SolarPanelShape Shape
		{
			get;
			private set;
		}

		/// <summary>
		/// These are the edges of this solar panel, before scale/orientation/translate
		/// </summary>
		internal IEnumerable<Point> Vertices
		{
			get;
			private set;
		}

		#endregion

		#region Public Methods

		public override Model3D GetFinalModel()
		{
			return CreateGeometry(true);
		}

		public override PartDNA GetDNA()
		{
			ConverterRadiationToEnergyDNA retVal = new ConverterRadiationToEnergyDNA();

			base.FillDNA(retVal);
			retVal.Shape = this.Shape;

			return retVal;
		}
		public override void SetDNA(PartDNA dna)
		{
			if (!(dna is ConverterRadiationToEnergyDNA))
			{
				throw new ArgumentException("The class passed in must be ConverterRadiationToEnergyDNA");
			}

			ConverterRadiationToEnergyDNA dnaCast = (ConverterRadiationToEnergyDNA)dna;

			base.StoreDNA(dna);

			this.Shape = dnaCast.Shape;
		}

		public override CollisionHull CreateCollisionHull(NewtonDynamics.WorldBase world)
		{
			//	Get points
			if(this.Vertices == null)
			{
				CreateGeometry(true);
			}

			double halfThick = THICKNESS * .5d;

			List<Point3D> points = new List<Point3D>();
			points.AddRange(this.Vertices.Select(o => new Point3D(o.X, o.Y, -halfThick)));
			points.AddRange(this.Vertices.Select(o => new Point3D(o.X, o.Y, halfThick)));

			//	Transform
			Transform3DGroup transform = new Transform3DGroup();
			transform.Children.Add(new ScaleTransform3D(this.Scale));
			transform.Children.Add(new RotateTransform3D(new QuaternionRotation3D(this.Orientation)));
			transform.Children.Add(new TranslateTransform3D(this.Position.ToVector()));

			//	Exit Function
			return CollisionHull.CreateConvexHull(world, 0, points, 0.002d, transform.Value);
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

			#region Determine Points

			List<Point> points = new List<Point>();

			//NOTE: In ConverterRadiationToEnergy.GetStats, it's expecting the two triangles to be (0 1 2), (0 2 3)
			switch (this.Shape)
			{
				case SolarPanelShape.Triangle:
					points.Add(new Point(0d, .5d));		//	90
					points.Add(new Point(-.43301270189221d, -.25d));		//	210
					points.Add(new Point(.43301270189221d, -.25d));		//	330
					break;

				case SolarPanelShape.Right_Triangle:
					points.Add(new Point(-.5d, -.5d));
					points.Add(new Point(.5d, -.5d));
					points.Add(new Point(-.5d, .5d));
					break;

				case SolarPanelShape.Square:
					points.Add(new Point(-.5d, -.5d));
					points.Add(new Point(.5d, -.5d));
					points.Add(new Point(.5d, .5d));
					points.Add(new Point(-.5d, .5d));
					break;

				case SolarPanelShape.Trapazoid:
					//	45 degree trapazoid looks funny, so divide the square into 3 vertical strips
					points.Add(new Point(-.5d, -.5d));
					points.Add(new Point(.5d, -.5d));
					points.Add(new Point(.16666667d, .5d));
					points.Add(new Point(-.16666667d, .5d));
					break;

				case SolarPanelShape.Right_Trapazoid:
					points.Add(new Point(-.5d, -.5d));
					points.Add(new Point(.5d, -.5d));
					points.Add(new Point(.16666667d, .5d));
					points.Add(new Point(-.5d, .5d));
					break;

				default:
					break;
			}

			this.Vertices = points;

			#endregion

			MeshGeometry3D capGeometry, sideGeometry;
			GetShape(out capGeometry, out sideGeometry, points, THICKNESS, .95d);

			#region Caps

			geometry = new GeometryModel3D();
			material = new MaterialGroup();
			diffuse = new DiffuseMaterial(new SolidColorBrush(this.Options.WorldColors.ConverterBase));
			this.MaterialBrushes.Add(new MaterialColorProps(diffuse, diffuse.Brush, this.Options.WorldColors.ConverterBase));
			material.Children.Add(diffuse);
			specular = this.Options.WorldColors.ConverterBaseSpecular;
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

			geometry.Geometry = capGeometry;

			retVal.Children.Add(geometry);

			#endregion

			#region Sides

			for (int cntr = 0; cntr <= 1; cntr++)
			{
				geometry = new GeometryModel3D();
				material = new MaterialGroup();
				diffuse = new DiffuseMaterial(new SolidColorBrush(this.Options.WorldColors.ConverterEnergy));
				this.MaterialBrushes.Add(new MaterialColorProps(diffuse, diffuse.Brush, this.Options.WorldColors.ConverterEnergy));
				material.Children.Add(diffuse);
				specular = this.Options.WorldColors.ConverterEnergySpecular;
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

				geometry.Geometry = sideGeometry;

				retVal.Children.Add(geometry);
			}

			#endregion

			//	Transform
			Transform3DGroup transformGlobal = new Transform3DGroup();
			if (isFinal)
			{
				transformGlobal.Children.Add(_scaleTransform.Clone());
				transformGlobal.Children.Add(new RotateTransform3D(_rotateTransform.Clone()));
				transformGlobal.Children.Add(_translateTransform.Clone());
			}
			else
			{
				transformGlobal.Children.Add(_scaleTransform);
				transformGlobal.Children.Add(new RotateTransform3D(_rotateTransform));
				transformGlobal.Children.Add(_translateTransform);
			}
			retVal.Transform = transformGlobal;

			//	Exit Function
			return retVal;
		}

		/// <summary>
		/// This creates the equivalent of three plates (the middle plate is invisible).
		/// Imagine two trapazoidal pyrimads opposing each other
		/// </summary>
		/// <remarks>
		/// TODO: This is a good start toward a TubeRingPath for GetMultiRingedTube
		/// </remarks>
		/// <param name="points">These are the points at z=0</param>
		/// <param name="thickness">This is the distance from the top to the bottom plate</param>
		/// <param name="scalePercent">
		/// How much bigger/smaller the top/bottom plates are from the center plate
		/// NOTE: For a convex polygon, percent needs to be 0 to 1
		/// </param>
		private static void GetShape(out MeshGeometry3D caps, out MeshGeometry3D sides, List<Point> points, double thickness, double scalePercent)
		{
			caps = new MeshGeometry3D();
			sides = new MeshGeometry3D();

			Vector3D centerPoint = new Vector3D(points.Sum(o => o.X), points.Sum(o => o.Y), 0d) / points.Count;
			Point[] capPoints = new Point[points.Count];
			for (int cntr = 0; cntr < points.Count; cntr++)
			{
				Vector3D vector = new Vector3D(points[cntr].X, points[cntr].Y, 0d) - centerPoint;
				vector *= scalePercent;
				vector += centerPoint;
				capPoints[cntr] = new Point(vector.X, vector.Y);
			}

			Point[] midPoints = points.ToArray();

			double thicknessHalf = thickness * .5d;

			#region Top Cap

			Transform3D polyTransform = MultiRingEndCapSprtGetTransform(Transform3D.Identity, true, -thicknessHalf);
			Transform3D polyTransformNormal = MultiRingEndCapSprtGetNormalTransform(polyTransform);

			int pointOffsetCaps = 0;
			GetShapeSprtEndCap(ref pointOffsetCaps, caps, capPoints, polyTransform, polyTransformNormal, true);

			#endregion

			#region Sides

			int pointOffsetSides = 0;

			GetShapeSprtSide(ref pointOffsetSides, sides, Transform3D.Identity, capPoints, midPoints, thicknessHalf, -thicknessHalf);
			GetShapeSprtSide(ref pointOffsetSides, sides, Transform3D.Identity, midPoints, capPoints, thicknessHalf, 0);

			#endregion

			#region Bottom Cap

			polyTransform = MultiRingEndCapSprtGetTransform(Transform3D.Identity, false, thicknessHalf);
			polyTransformNormal = MultiRingEndCapSprtGetNormalTransform(polyTransform);

			GetShapeSprtEndCap(ref pointOffsetCaps, caps, capPoints, polyTransform, polyTransformNormal, false);

			#endregion
		}

		/// <summary>
		/// This is a copy of UtilityWPF.MultiRingEndCapSprtPlateSoft
		/// </summary>
		private static void GetShapeSprtEndCap(ref int pointOffset, MeshGeometry3D geometry, Point[] points, Transform3D transform, Transform3D normalTransform, bool isFirst)
		{
			#region Positions/Normals

			//if (isFirst)		//	UtilityWPF had this if statement, because the cap went off of the previous side's points.  But in this solar panel class, the caps are their own geometry, so need unique points
			//{
			for (int thetaCntr = 0; thetaCntr < points.Length; thetaCntr++)
			{
				Point3D point = new Point3D(points[thetaCntr].X, points[thetaCntr].Y, 0d);
				geometry.Positions.Add(transform.Transform(point));

				Vector3D normal = new Vector3D(0, 0, 1);

				geometry.Normals.Add(normalTransform.Transform(normal).ToUnit());
			}
			//}

			#endregion

			#region Add the triangles

			// Start with 0,1,2
			geometry.TriangleIndices.Add(pointOffset + 0);
			geometry.TriangleIndices.Add(pointOffset + 1);
			geometry.TriangleIndices.Add(pointOffset + 2);

			int lowerIndex = 2;
			int upperIndex = points.Length - 1;
			int lastUsedIndex = 0;
			bool shouldBumpLower = true;

			// Do the rest of the triangles
			while (lowerIndex < upperIndex)
			{
				geometry.TriangleIndices.Add(pointOffset + lowerIndex);
				geometry.TriangleIndices.Add(pointOffset + upperIndex);
				geometry.TriangleIndices.Add(pointOffset + lastUsedIndex);

				if (shouldBumpLower)
				{
					lastUsedIndex = lowerIndex;
					lowerIndex++;
				}
				else
				{
					lastUsedIndex = upperIndex;
					upperIndex--;
				}

				shouldBumpLower = !shouldBumpLower;
			}

			#endregion

			pointOffset = geometry.Positions.Count;
		}
		/// <summary>
		/// This is a copy of UtilityWPF.MultiRingMiddleSprtTubeSoft
		/// </summary>
		private static void GetShapeSprtSide(ref int pointOffset, MeshGeometry3D geometry, Transform3D transform, Point[] points1, Point[] points2, double distFrom1to2, double curZ)
		{
			if (points1.Length != points2.Length)
			{
				throw new ApplicationException("The point lists need to be the same size");
			}

			#region Points/Normals

			//TODO: Don't add the bottom ring's points, only the top

			//	Ring 1
			for (int cntr = 0; cntr < points1.Length; cntr++)
			{
				geometry.Positions.Add(transform.Transform(new Point3D(points1[cntr].X, points1[cntr].Y, curZ)));
				geometry.Normals.Add(transform.Transform(new Vector3D(points1[cntr].X, points1[cntr].Y, 0d).ToUnit()));		//	the normals point straight out of the side
			}

			//	Ring 2
			for (int cntr = 0; cntr < points1.Length; cntr++)
			{
				geometry.Positions.Add(transform.Transform(new Point3D(points2[cntr].X, points2[cntr].Y, curZ + distFrom1to2)));
				geometry.Normals.Add(transform.Transform(new Vector3D(points2[cntr].X, points2[cntr].Y, 0d).ToUnit()));		//	the normals point straight out of the side
			}

			#endregion

			#region Triangles

			int zOffsetBottom = pointOffset;
			int zOffsetTop = zOffsetBottom + points1.Length;

			for (int cntr = 0; cntr < points1.Length - 1; cntr++)
			{
				//	Top/Left triangle
				geometry.TriangleIndices.Add(zOffsetBottom + cntr + 0);
				geometry.TriangleIndices.Add(zOffsetTop + cntr + 1);
				geometry.TriangleIndices.Add(zOffsetTop + cntr + 0);

				//	Bottom/Right triangle
				geometry.TriangleIndices.Add(zOffsetBottom + cntr + 0);
				geometry.TriangleIndices.Add(zOffsetBottom + cntr + 1);
				geometry.TriangleIndices.Add(zOffsetTop + cntr + 1);
			}

			//	Connecting the last 2 points to the first 2
			//	Top/Left triangle
			geometry.TriangleIndices.Add(zOffsetBottom + (points1.Length - 1) + 0);
			geometry.TriangleIndices.Add(zOffsetTop);		//	wrapping back around
			geometry.TriangleIndices.Add(zOffsetTop + (points1.Length - 1) + 0);

			//	Bottom/Right triangle
			geometry.TriangleIndices.Add(zOffsetBottom + (points1.Length - 1) + 0);
			geometry.TriangleIndices.Add(zOffsetBottom);
			geometry.TriangleIndices.Add(zOffsetTop);

			#endregion

			pointOffset = geometry.Positions.Count;
		}

		private static Transform3D MultiRingEndCapSprtGetTransform(Transform3D transform, bool isFirst, double z)
		{
			//	This overload is for a flat plate

			Transform3DGroup retVal = new Transform3DGroup();

			if (isFirst)
			{
				//	This still needs to be flipped for a flat cap so the normals turn out right
				//retVal.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 180d)));
				//retVal.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 180d)));
				retVal.Children.Add(new TranslateTransform3D(0, 0, z));
				retVal.Children.Add(transform);
			}
			else
			{
				retVal.Children.Add(new TranslateTransform3D(0, 0, z));
				retVal.Children.Add(transform);
			}

			return retVal;
		}
		private static Transform3D MultiRingEndCapSprtGetNormalTransform(Transform3D transform)
		{
			//	Can't use all of the transform passed in for the normal, because translate portions will skew the normals funny
			Transform3DGroup retVal = new Transform3DGroup();
			if (transform is Transform3DGroup)
			{
				foreach (var subTransform in ((Transform3DGroup)transform).Children)
				{
					if (!(subTransform is TranslateTransform3D))
					{
						retVal.Children.Add(subTransform);
					}
				}
			}
			else if (transform is TranslateTransform3D)
			{
				retVal.Children.Add(Transform3D.Identity);
			}
			else
			{
				retVal.Children.Add(transform);
			}

			return retVal;
		}

		#endregion
	}

	#endregion
	#region Class: ConverterRadiationToEnergy

	public class ConverterRadiationToEnergy : PartBase
	{
		#region Declaration Section

		public const string PARTTYPE = "ConverterRadiationToEnergy";

		private ItemOptions _itemOptions = null;

		private ConverterRadiationToEnergyDNA _dnaCast = null;
		private ConverterRadiationToEnergyDesign _design = null;

		private IContainer _energyTanks = null;
		private RadiationField _radiationField = null;

		private double _mass = 0d;

		//	These have been translated and oriented to their location within the body (still in body coords, not world coords)
		private Vector3D _normalFront = new Vector3D(0, 0, 0);
		private Vector3D _normalBack = new Vector3D(0, 0, 0);
		private Point3D _centerPoint = new Point3D(0, 0, 0);

		#endregion

		#region Constructor

		public ConverterRadiationToEnergy(EditorOptions options, ItemOptions itemOptions, ConverterRadiationToEnergyDNA dna, IContainer energyTanks, RadiationField radiationField)
			: base(options, dna)
		{
			_itemOptions = itemOptions;
			_dnaCast = dna;
			_energyTanks = energyTanks;
			_radiationField = radiationField;

			_design = new ConverterRadiationToEnergyDesign(options, dna.Shape);
			_design.SetDNA(dna);

			//	Need to call this here, so that _design.Vertices is set
			_model = _design.GetFinalModel();

			this.ClarityPercent_Front = 1d;
			this.ClarityPercent_Back = 1d;

			Point3D center;
			Vector3D normal;
			GetStats(out _mass, out center, out normal);

			//	Store the center and normals
			Transform3DGroup transform = new Transform3DGroup();
			transform.Children.Add(new RotateTransform3D(new QuaternionRotation3D(dna.Orientation)));
			transform.Children.Add(new TranslateTransform3D(dna.Position.ToVector()));

			_centerPoint = transform.Transform(center);
			_normalFront = transform.Transform(normal);
			_normalBack = transform.Transform(normal * -1d);
		}

		#endregion

		#region Public Properties

		public override double DryMass
		{
			get
			{
				return _mass;
			}
		}
		public override double TotalMass
		{
			get
			{
				return _mass;
			}
		}

		public override Point3D Position
		{
			get
			{
				return this.DNA.Position;
			}
		}
		public override Quaternion Orientation
		{
			get
			{
				return this.DNA.Orientation;
			}
		}

		private Model3D _model = null;
		public override Model3D Model
		{
			get
			{
				return _model;
			}
		}

		/// <summary>
		/// Goes from 0 to 1.  Set this to less than one if the panel is partially blocked by other parts
		/// </summary>
		public double ClarityPercent_Front
		{
			get;
			set;
		}
		/// <summary>
		/// Goes from 0 to 1.  Set this to less than one if the panel is partially blocked by other parts (or zero if it's only a one
		/// sided panel)
		/// </summary>
		public double ClarityPercent_Back
		{
			get;
			set;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// This
		/// </summary>
		/// <param name="elapsedTime"></param>
		/// <param name="transform"></param>
		public void Transfer(double elapsedTime, Transform3D worldTransform)
		{
			if (_energyTanks == null)
			{
				return;
			}

			Point3D worldPoint = worldTransform.Transform(_centerPoint);

			double radiation = 0d;

			//	Front side
			if (this.ClarityPercent_Front > 0d)
			{
				Vector3D worldNormal = worldTransform.Transform(_normalFront);
				radiation += _radiationField.GetRadiation(worldPoint, worldNormal, true) * this.ClarityPercent_Front;
			}

			//	Back side
			if (this.ClarityPercent_Back > 0d)
			{
				Vector3D worldNormal = worldTransform.Transform(_normalBack);
				radiation += _radiationField.GetRadiation(worldPoint, worldNormal, true) * this.ClarityPercent_Back;
			}

			//	Convert the radiation into energy
			double amountToAdd = radiation * elapsedTime * _itemOptions.SolarPanelConversionRate;

			//	Add it (doesn't matter if the tank is full, just try)
			_energyTanks.AddQuantity(amountToAdd, false);
		}

		public override CollisionHull CreateCollisionHull(WorldBase world)
		{
			return _design.CreateCollisionHull(world);
		}

		#endregion

		#region Private Methods

		private void GetStats(out double mass, out Point3D centerPoint, out Vector3D normal)
		{
			if (_design.Vertices == null)
			{
				throw new InvalidOperationException("_design.Vertices should have been populated by now");
			}

			ScaleTransform3D scale = new ScaleTransform3D(this.DNA.Scale);

			//	Convert the points to 3D and scale them
			List<Point3D> verticesScaled = _design.Vertices.Select(o => scale.Transform(new Point3D(o.X, o.Y, 0d))).ToList();

			//	Get the normal
			switch (verticesScaled.Count)
			{
				case 3:
					normal = new Triangle(verticesScaled[0], verticesScaled[1], verticesScaled[2]).Normal;
					break;

				case 4:
					normal = new Triangle(verticesScaled[0], verticesScaled[1], verticesScaled[2]).Normal +
										new Triangle(verticesScaled[0], verticesScaled[2], verticesScaled[3]).Normal;
					break;

				default:
					throw new ApplicationException("Unexpected number of points: " + verticesScaled.Count.ToString());
			}

			//	Figure out the centerpoint
			centerPoint = new Point3D(verticesScaled.Sum(o => o.X) / verticesScaled.Count, verticesScaled.Sum(o => o.Y) / verticesScaled.Count, verticesScaled.Sum(o => o.Z) / verticesScaled.Count);

			//	Figure out the mass
			double volume = normal.Length * (this.DNA.Scale.Z * ConverterRadiationToEnergyDesign.THICKNESS);
			mass = volume * _itemOptions.SolarPanelDensity;
		}

		#endregion
	}

	#endregion

	#region Class: ConverterRadiationToEnergyDNA

	public class ConverterRadiationToEnergyDNA : PartDNA
	{
		public SolarPanelShape Shape
		{
			get;
			set;
		}
	}

	#endregion

	#region Enum: SolarPanelShape

	public enum SolarPanelShape
	{
		Triangle,
		Right_Triangle,
		Square,
		Trapazoid,
		Right_Trapazoid
	}

	#endregion
}