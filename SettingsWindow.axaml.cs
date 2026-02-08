using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace ZZZDmgCal;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
    }

	private void OnThemeSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem)
		{
			// Lấy giá trị ThemeVariant từ Tag chúng ta đã đặt ở XAML
			var theme = selectedItem.Tag as ThemeVariant;

			// Áp dụng cho toàn bộ ứng dụng
			if (Application.Current != null && theme != null)
			{
				Application.Current.RequestedThemeVariant = theme;
			}
		}
	}
}