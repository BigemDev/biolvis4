using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using System.IO;
using Avalonia.Platform.Storage;

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
}