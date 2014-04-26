using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToVisualTree;
using UIElementLeakTester;

using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using Callisto.Controls;

namespace Callisto.TestApp.SamplePages
{
    public sealed partial class CustomDialogSample
    {
        private CustomDialog dialog;

        public CustomDialogSample()
        {
            this.InitializeComponent();

            this.Loaded += (sender, args) => {
                var mp = Frame.Ancestors<Frame>().FirstOrDefault().Descendants<Page>().FirstOrDefault();
                dialog = (mp as MainPage).LoginDialog;
                dialog.IsOpenChanged += (o, e) => {
                    IsOpenChangedTextBlock.Text = (bool)e.NewValue ? "Open" : "Closed";
                };
            };
        }

        private void CustomDialogClicked(object sender, RoutedEventArgs e)
        {
            dialog.IsOpen = true;
        }

        private void MessageDialogClicked(object sender, RoutedEventArgs e)
        {
            MessageDialog md = new MessageDialog(
                "Bacon ipsum dolor sit amet bacon ham drumstick strip steak, sausage frankfurter tenderloin turkey salami andouille bresaola. Venison salami prosciutto, pork belly turducken tri-tip spare ribs chicken strip steak fatback shankle tongue boudin andouille. Meatloaf salami pork ground round turkey jerky meatball ball tip, filet mignon fatback flank prosciutto shank. Turkey boudin ham hock, filet mignon tri-tip bresaola tongue venison spare ribs meatloaf flank beef pancetta. Leberkas turducken flank ground round biltong chuck bacon kielbasa. Beef pastrami meatball, short loin venison swine pork loin shank meatloaf spare ribs.",
                "Bacon Terms and Conditions");

#pragma warning disable 4014
            md.ShowAsync();
#pragma warning restore 4014
        }

        /// <summary>
        /// To ensure there is no memory leak, a new CustomDialog is added to and removed from the visual tree.
        /// Each step waits for it to load before continuing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomDialogTestForMemoryLeak(object sender, RoutedEventArgs e)
        {
            var customDialog = new CustomDialog();

            ObjectTracker.Track(customDialog);

            var childGrid = Frame.Descendants<Grid>().FirstOrDefault() as Grid;
            if (childGrid != null)
            {
                customDialog.Loaded += (o, args) =>
                {
                    childGrid.Children.Remove(customDialog);
                };

                customDialog.Unloaded += (o, args) =>
                {
                    CheckDialogHasBeenGarbageCollected();
                };

                childGrid.Children.Add(customDialog);
            }
        }

        private async void CheckDialogHasBeenGarbageCollected()
        {
            // Ensure control has time to be unloaded
            await Task.Delay(100);

            var resultsTextBox = new TextBox();
            ObjectTracker.GarbageCollect(resultsTextBox);
            await new MessageDialog(resultsTextBox.Text).ShowAsync();
        }
    }
}
