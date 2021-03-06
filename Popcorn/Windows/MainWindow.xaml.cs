﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Controls;
using Popcorn.Extensions;
using Popcorn.Messaging;
using Popcorn.Utils;
using Popcorn.ViewModels.Windows;

namespace Popcorn.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private LowLevelKeyboardListener _listener;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            Initialized += OnInitialized;
            InitializeComponent();
            Closing += OnClosing;
            var vm = DataContext as WindowViewModel;
            if (vm != null)
            {
                vm.NavigationService = MainFrame.NavigationService;
            }

            StateChanged += OnStateChanged;
            Messenger.Default.Register<DropFileMessage>(this, e =>
            {
                if (e.Event == DropFileMessage.DropFileEvent.Enter)
                {
                    BorderThickness = new Thickness(1);
                    BorderBrush = (SolidColorBrush) new BrushConverter().ConvertFrom("#CCE51400");
                    GlowBrush = (SolidColorBrush) new BrushConverter().ConvertFrom("#CCE51400");
                    DoubleAnimation da = new DoubleAnimation
                    {
                        To = 0.5d,
                        Duration = new Duration(TimeSpan.FromMilliseconds(750)),
                        EasingFunction = new PowerEase
                        {
                            EasingMode = EasingMode.EaseInOut,
                            Power = 2d
                        }
                    };
                    BeginAnimation(OpacityProperty, da);
                }
                else
                {
                    BorderThickness = new Thickness(0);
                    BorderBrush = Brushes.Transparent;
                    GlowBrush = Brushes.Transparent;
                    DoubleAnimation da = new DoubleAnimation
                    {
                        To = 1.0d,
                        Duration = new Duration(TimeSpan.FromMilliseconds(750)),
                        EasingFunction = new PowerEase
                        {
                            EasingMode = EasingMode.EaseInOut,
                            Power = 2d
                        }
                    };
                    BeginAnimation(OpacityProperty, da);
                }
            });
        }

        private void OnInitialized(object sender, EventArgs e)
        {
            _listener = new LowLevelKeyboardListener();
            _listener.OnKeyPressed += OnKeyPressed;
            _listener.HookKeyboard();
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _listener.UnHookKeyboard();
        }

        private void OnKeyPressed(object sender, KeyPressedArgs e)
        {
            if (e.KeyPressed == Key.Down || e.KeyPressed == Key.Up)
            {
                var movieScrollviewer =
                    this.FindChild<AnimatedScrollViewer>("MovieScrollViewer");
                var showScrollviewer =
                    this.FindChild<AnimatedScrollViewer>("ShowScrollViewer");
                if (movieScrollviewer != null && movieScrollviewer.IsVisible)
                    movieScrollviewer.Focus();

                if (showScrollviewer != null && showScrollviewer.IsVisible)
                    showScrollviewer.Focus();
            }
        }

        private void OnStateChanged(object sender, EventArgs e)
        {
            MovieDetailsUc.Margin = WindowState == WindowState.Maximized
                ? new Thickness(0, 0, 16, 0)
                : new Thickness(0, 0, 0, 0);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            var searchBox =
                this.FindChild<TextBox>("SearchBox");
            if (e.Key == Key.I && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && !searchBox.IsFocused)
            {
                var vm = DataContext as WindowViewModel;
                vm?.OpenAboutCommand.Execute(null);
            }
            else if (e.Key == Key.F1)
            {
                var vm = DataContext as WindowViewModel;
                vm?.OpenHelpCommand.Execute(null);
            }
            else if (e.Key == Key.F3 || (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift &&
                     e.Key == Key.F)
            {
                searchBox.Focus();
            }
        }
    }
}