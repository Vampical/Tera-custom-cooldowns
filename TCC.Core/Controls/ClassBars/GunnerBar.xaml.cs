﻿using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using TCC.ViewModels;

namespace TCC.Controls.ClassBars
{
    /// <summary>
    /// Logica di interazione per GunnerBar.xaml
    /// </summary>
    public partial class GunnerBar
    {
        public GunnerBar()
        {
            InitializeComponent();
        }

        private GunnerBarManager _dc;
        private DoubleAnimation _an;
        private void GunnerBar_OnLoaded(object sender, RoutedEventArgs e)
        {
            _dc = (GunnerBarManager) DataContext;
            _an = new DoubleAnimation(_dc.StaminaTracker.Factor * 359.99 + 40, TimeSpan.FromMilliseconds(150));
            _dc.StaminaTracker.PropertyChanged += ST_PropertyChanged;

        }

        private void ST_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(_dc.StaminaTracker.Factor)) return;
            _an.To = _dc.StaminaTracker.Factor*(359.99 - 80) + 40;
            MainReArc.BeginAnimation(Arc.EndAngleProperty, _an);
        }
    }
}
