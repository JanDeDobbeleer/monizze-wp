using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Monizze.Common.Interfaces;

namespace Monizze.Managers
{
    public class NotificationManager: INotificationManager
    {
        public async Task<bool> ShowMessageBox(string message, string buttonConfirmText, string buttonCancelText)
        {
            var dialog = new MessageDialog(message);
            dialog.Commands.Add(new UICommand(buttonConfirmText) { Id = 0 });
            dialog.Commands.Add(new UICommand(buttonCancelText) { Id = 1 });
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;
            var result = await dialog.ShowAsync();
            return result.Id.Equals(0);
        }

        public async Task<Tuple<bool, string>> ShowInteractionBox(string title, string info, string boxContent, string placeHolderText, string actionButton, string cancelButton)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                RequestedTheme = ElementTheme.Dark
            };
            // Setup Content
            var panel = new StackPanel();
            panel.Children.Add(new TextBlock
            {
                Text = info,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 10, 0, 10)
            });
            var textBox = new TextBox();
            textBox.TextChanged += (sender, args) =>
            {
                dialog.IsPrimaryButtonEnabled = !string.IsNullOrWhiteSpace(textBox.Text);
            };
            if (string.IsNullOrWhiteSpace(boxContent))
            {
                textBox.PlaceholderText = placeHolderText;
            }
            else
            {
                textBox.Text = boxContent;
            }
            panel.Children.Add(textBox);
            dialog.Content = panel;

            // Add Buttons
            dialog.PrimaryButtonText = actionButton;
            dialog.IsPrimaryButtonEnabled = !string.IsNullOrWhiteSpace(textBox.Text);
            dialog.SecondaryButtonText = cancelButton;

            // Show Dialog
            var result = await dialog.ShowAsync();
            return new Tuple<bool, string>(result == ContentDialogResult.Primary, textBox.Text);
        }
    }
}
