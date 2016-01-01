using Windows.UI.Notifications;
using NotificationsExtensions.Tiles;

namespace Monizze.LiveTile
{
    public interface ITileUpdater
    {
        void UpdateTile(string balance);
    }

    public sealed class TileUpdater : ITileUpdater
    {
        public void UpdateTile(string balance)
        {
            var content = new TileContent
            {
                Visual = new TileVisual
                {
                    Branding = TileBranding.Name,
                    TileMedium = new TileBinding
                    {
                        Content = new TileBindingContentAdaptive
                        {
                            //BackgroundImage = new TileBackgroundImage
                            //{
                            //    Source = new TileImageSource("Assets/Mostly Cloudy-Background.jpg")
                            //},
                            TextStacking = TileTextStacking.Center,
                            Children =
                            {
                                new TileText
                                {
                                    Text = $"€{balance}",
                                    Style = TileTextStyle.Title,
                                    Align = TileTextAlign.Center
                                }
                            }
                        }
                    },
                    TileWide = new TileBinding
                    {
                        Content = new TileBindingContentAdaptive
                        {
                            TextStacking = TileTextStacking.Center,
                            Children =
                            {
                                new TileText
                                {
                                    Text = $"€{balance}",
                                    Style = TileTextStyle.Header,
                                    Align = TileTextAlign.Center
                                }
                            }
                        }
                    }
                }
            };
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
            var notification = new TileNotification(content.GetXml());
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }
    }
}
