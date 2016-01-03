using System;
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
                    TileSmall = new TileBinding
                    {
                        Branding = TileBranding.None,
                        Content = new TileBindingContentAdaptive
                        {
                            BackgroundImage = new TileBackgroundImage
                            {
                                Source = new TileImageSource("Assets/background.jpg")
                            },
                            TextStacking = TileTextStacking.Center,
                            Children =
                            {
                                new TileText
                                {
                                    Text = "Balance",
                                    Style = TileTextStyle.Caption,
                                    Align = TileTextAlign.Center
                                },
                                new TileText
                                {
                                    Text = $"€{balance}",
                                    Style = TileTextStyle.Caption,
                                    Align = TileTextAlign.Center
                                }
                            }
                        }
                    },
                    TileMedium = new TileBinding
                    {
                        Branding = TileBranding.Name,
                        DisplayName = DateTime.Now.ToString("HH:mm dd/MM"),
                        Content = new TileBindingContentAdaptive
                        {
                            BackgroundImage = new TileBackgroundImage
                            {
                                Source = new TileImageSource("Assets/background.jpg")
                            },
                            TextStacking = TileTextStacking.Top,
                            Children =
                            {
                                new TileText
                                {
                                    Text = "Balance",
                                    Style = TileTextStyle.Body,
                                    Align = TileTextAlign.Left
                                },
                                new TileText
                                {
                                    Text = $"€{balance}",
                                    Style = TileTextStyle.Caption,
                                    Align = TileTextAlign.Left
                                }
                            }
                        }
                    },
                    TileWide = new TileBinding
                    {
                        Branding = TileBranding.Name,
                        DisplayName = $"updated {DateTime.Now.ToString("HH:mm dd/MM")}",
                        Content = new TileBindingContentAdaptive
                        {
                            BackgroundImage = new TileBackgroundImage
                            {
                                Source = new TileImageSource("Assets/background.jpg")
                            },
                            TextStacking = TileTextStacking.Center,
                            Children =
                            {
                                new TileText
                                {
                                    Text = "Balance",
                                    Style = TileTextStyle.Subtitle,
                                    Align = TileTextAlign.Center
                                },
                                new TileText
                                {
                                    Text = $"€{balance}",
                                    Style = TileTextStyle.Base,
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
