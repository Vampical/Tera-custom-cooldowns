﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using TCC.ViewModels;

namespace TCC.Controls
{
    /// <summary>
    /// Interaction logic for VanguardInfoControl.xaml
    /// </summary>
    public partial class VanguardInfoControl : UserControl
    {
        TimeSpan growDuration;
        DoubleAnimation scaleUp;
        DoubleAnimation moveUp;
        DoubleAnimation scaleDown;
        DoubleAnimation moveDown;
        DoubleAnimation scaleRipple;
        DoubleAnimation fadeRipple;


        public VanguardInfoControl()
        {
            InitializeComponent();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
         {
            rootBorder.RenderTransform.BeginAnimation(TranslateTransform.YProperty, scaleUp);
            rootBorder.Effect.BeginAnimation(DropShadowEffect.BlurRadiusProperty, moveUp);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_animDown)
            {

                rootBorder.RenderTransform.BeginAnimation(TranslateTransform.YProperty, scaleDown);
                rootBorder.Effect.BeginAnimation(DropShadowEffect.BlurRadiusProperty, moveDown);
            }
            else
            {
                rootBorder.RenderTransform = new TranslateTransform(0, 0);
                (rootBorder.Effect as DropShadowEffect).BlurRadius = 3;
            }

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            growDuration = TimeSpan.FromMilliseconds(150);
            scaleUp = new DoubleAnimation(-3, growDuration) { EasingFunction = new QuadraticEase() };
            moveUp =  new DoubleAnimation(8, growDuration) { EasingFunction = new QuadraticEase() };
            scaleDown = new DoubleAnimation(0, growDuration) { EasingFunction = new QuadraticEase() };
            moveDown = new DoubleAnimation(3, growDuration) { EasingFunction = new QuadraticEase() };
            scaleRipple = new DoubleAnimation(0, 7, TimeSpan.FromMilliseconds(650)) { EasingFunction = new QuadraticEase() };
            fadeRipple = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(650));
            ContextMenu = new ContextMenu();
            var i = new MenuItem {Header = "Remove"};
            i.Click += RemoveCharacter;
            ContextMenu.Items.Add(i);
        }

        private void RemoveCharacter(object sender, RoutedEventArgs e)
        {
            InfoWindowViewModel.Instance.Characters.Remove((Character) DataContext);
        }

        bool _animDown = true;
        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _animDown = false;
            //var scaleTrans = (ripple.RenderTransform as TransformGroup).Children[0];
            //(ripple.RenderTransform as TransformGroup).Children[1] = new TranslateTransform(e.MouseDevice.GetPosition(this).X - ripple.Width / 2, e.MouseDevice.GetPosition(this).Y - ripple.Height / 2);

            //scaleTrans.BeginAnimation(ScaleTransform.ScaleXProperty, scaleRipple);
            //scaleTrans.BeginAnimation(ScaleTransform.ScaleYProperty, scaleRipple);
            //ripple.BeginAnimation(OpacityProperty, fadeRipple);
            Point relativePoint = rootBorder.TransformToAncestor(WindowManager.InfoWindow)
                              .Transform(new Point(0, 0));
            WindowManager.InfoWindow.ExpandCharacter(relativePoint, rootBorder.ActualWidth, this.ActualHeight);
            InfoWindowViewModel.Instance.SelectCharacter((Character)DataContext);
            Task.Delay(300).ContinueWith(t => _animDown = true);
        }
        
        private void RootBorder_OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
