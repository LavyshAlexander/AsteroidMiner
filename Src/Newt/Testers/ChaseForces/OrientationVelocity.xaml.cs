﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Game.Newt.v2.GameItems;

namespace Game.Newt.Testers.ChaseForces
{
    //NOTE: This is almost an exact copy of the LinearVelocity control, should probably make a common control, but this is just a tester, and I'm feeling lazy
    public partial class OrientationVelocity : UserControl
    {
        #region Events

        public event EventHandler ValueChanged = null;

        #endregion

        #region Declaration Section

        const string TITLE = "OrientationVelocity Panel";

        #endregion

        #region Constructor

        public OrientationVelocity()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        #endregion

        #region Public Methods

        public MapObject_ChaseOrientation_Velocity GetChaseObject(IMapObject item)
        {
            return new MapObject_ChaseOrientation_Velocity(item)
            {
                Multiplier = trkMultiplier.Value,
                MaxVelocity = chkMaxVelocity.IsChecked.Value ? trkMaxVelocity.Value : (double?)null,
            };
        }

        #endregion
        #region Protected Methods

        protected virtual void OnValueChanged()
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, new EventArgs());
            }
        }

        #endregion

        #region Event Listeners

        private void chkMaxVelocity_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chkMaxVelocity.IsChecked.Value)
                {
                    trkMaxVelocity.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    trkMaxVelocity.Visibility = System.Windows.Visibility.Collapsed;
                }

                OnValueChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Slider_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                OnValueChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
