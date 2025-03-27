using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using System.IO;
using Avalonia.Platform.Storage;
using Avalonia.Media;
using SkiaSharp;


namespace biolvis4;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void LoadIMG(object sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            AllowMultiple = false,
            FileTypeFilter = new[] { FilePickerFileTypes.ImageAll }
        });

        if (files.Count >= 1)
        {
            var file = files[0];
            var stream = await file.OpenReadAsync();
            var bitmap = new Bitmap(stream);

            var image = this.FindControl<Image>("MainImage");
            if (image != null)
            {
                image.Source = bitmap;
            }
        }
    }

    private void OnRotationChanged(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radioButton)
        {
            var angle = int.Parse(radioButton.Content.ToString());
            var image = this.FindControl<Image>("MainImage");
            if (image != null)
            {
                var rotateTransform = image.RenderTransform as RotateTransform;
                if (rotateTransform != null)
                {
                    rotateTransform.Angle = angle;
                }
                else
                {
                    image.RenderTransform = new RotateTransform(angle);
                }
            }
        }
    }

    public void InvCol(object sender, RoutedEventArgs e)
    {
        var image = this.FindControl<Image>("MainImage");
        if (image != null && image.Source is Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream);
                stream.Position = 0;
                using (var skBitmap = SKBitmap.Decode(stream))
                {
                    for (int x = 0; x < skBitmap.Width; x++)
                    {
                        for (int y = 0; y < skBitmap.Height; y++)
                        {
                            var color = skBitmap.GetPixel(x, y);
                            skBitmap.SetPixel(x, y, new SKColor(
                                (byte)(255 - color.Red),
                                (byte)(255 - color.Green),
                                (byte)(255 - color.Blue),
                                color.Alpha
                            ));
                        }
                    }

                    using (var skImage = SKImage.FromBitmap(skBitmap))
                    using (var data = skImage.Encode())
                    {
                        var newStream = new MemoryStream(data.ToArray());
                        image.Source = new Bitmap(newStream);
                    }
                }
            }
        }
    }

    public void UpsideDown(object sender, RoutedEventArgs e)
    {
        var image = this.FindControl<Image>("MainImage");
        if (image != null && image.Source is Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream);
                stream.Position = 0;
                using (var skBitmap = SKBitmap.Decode(stream))
                {
                    var flippedBitmap = new SKBitmap(skBitmap.Width, skBitmap.Height);
                    using (var canvas = new SKCanvas(flippedBitmap))
                    {
                        canvas.Scale(-1, 1, skBitmap.Width / 2f, 0);
                        canvas.DrawBitmap(skBitmap, 0, 0);
                    }

                    using (var skImage = SKImage.FromBitmap(flippedBitmap))
                    using (var data = skImage.Encode())
                    {
                        var newStream = new MemoryStream(data.ToArray());
                        image.Source = new Bitmap(newStream);
                    }
                }
            }
        }
    }
}