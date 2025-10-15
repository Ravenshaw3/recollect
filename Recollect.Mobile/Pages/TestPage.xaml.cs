using Microsoft.Maui.Controls;

namespace Recollect.Mobile.Pages;

public partial class TestPage : ContentPage
{
    public TestPage()
    {
        InitializeComponent();
    }

    private void OnTestClicked(object sender, EventArgs e)
    {
        StatusLabel.Text = "Button clicked successfully!";
    }
}
