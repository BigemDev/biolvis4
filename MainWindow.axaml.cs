using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using System.IO;
using Avalonia.Platform.Storage;
using Avalonia.Media;

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
}