﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Callisto.Controls;
using UIElementLeakTester;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using XamlControlsUITestApp;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Callisto.TestApp.SamplePages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class FlyoutSample : Page
	{
		public FlyoutSample()
		{
			this.InitializeComponent();
		}

		/// <summary>
		/// Invoked when this page is about to be displayed in a Frame.
		/// </summary>
		/// <param name="e">Event data that describes how this page was reached.  The Parameter
		/// property is typically used to configure the page.</param>
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			positioning.SelectedIndex = 0;
		}

		private void LogEvent(string msg)
		{
			LogOutput.Text += System.Environment.NewLine + msg;
			Debug.WriteLine(msg);
		}

		private void ItemClicked(object sender, TappedRoutedEventArgs args)
		{
			MenuItem mi = sender as MenuItem;
			LogEvent(string.Format("Event: Tapped; Argument: {0}", mi.Tag.ToString()));
		}

		private void ShowFlyoutMenu(object sender, RoutedEventArgs e)
		{
			Flyout f = new Flyout();
			f.PlacementTarget = sender as UIElement;
			f.Placement = PlacementMode.Top;
			f.Closed += (x, y) =>
			{
				LogEvent("Event: Closed");
			};

			Menu menu = new Menu();

			MenuItem mi = new MenuItem();
			mi.Tag = "Easy";
			mi.Tapped += ItemClicked;
			mi.Text = "Easy Game";

			MenuItem mi2 = new MenuItem();
			mi2.Text = "Medium Game";
			mi2.Tag = "Medium";
			mi2.Tapped += ItemClicked;

			MenuItem mi3 = new MenuItem();
			mi3.Text = "Hard Game";
			mi3.Command = new CommandTest();
			mi3.CommandParameter = "test param from command";

			menu.Items.Add(mi);
			menu.Items.Add(mi2);
			menu.Items.Add(new MenuItemSeparator());
			menu.Items.Add(new MenuItem() { Text = "Foobar something really long", Tag = "Long menu option" });
			menu.Items.Add(new MenuItemSeparator());
			menu.Items.Add(mi3);

			f.Content = menu;
			f.IsOpen = true;

			ObjectTracker.Track(f);

			UpdateLayout();
		}

		private void ShowFlyoutMenu2(object sender, RoutedEventArgs e)
		{
			Flyout f = new Flyout();

			Border b = new Border();
			b.Width = 300;
			b.Height = 125;

			TextBlock tb = new TextBlock();
			tb.FontSize = 24.667;
			tb.Text = "Hello";

			b.Child = tb;

			f.Content = b;

			f.Placement = (PlacementMode)Enum.Parse(typeof(PlacementMode), positioning.SelectionBoxItem.ToString());
			f.PlacementTarget = sender as UIElement;

			f.IsOpen = true;

			ObjectTracker.Track(f);
		}


		private void ShowFlyoutMenu3(object sender, RoutedEventArgs e)
		{
			Flyout f = new Flyout();

			f.Margin = new Thickness(20, 12, 20, 20);
			f.VerticalOffset = -12;
			f.HorizontalOffset = -124;
			f.Content = new SampleInput();
			f.Placement = PlacementMode.Top;
			f.PlacementTarget = sender as UIElement;

			LayoutRoot.Children.Add(f.HostPopup);

			f.Closed += (b, c) =>
			{
				LayoutRoot.Children.Remove(f.HostPopup);
			};

			f.IsOpen = true;

			ObjectTracker.Track(f);
		}



		public class CommandTest : ICommand
		{
			public bool CanExecute(object parameter)
			{
				return true;
			}

#pragma warning disable 67 //CanExecute never changes, but event is required by ICommand.
			public event EventHandler CanExecuteChanged;
#pragma warning restore 67

			public void Execute(object parameter)
			{
				Debug.WriteLine(string.Format("Event: Command.Execute; Argument: {0}", parameter.ToString()));
			}
		}
	}
}
