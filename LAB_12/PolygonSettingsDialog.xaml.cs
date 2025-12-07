using System.Windows;

namespace LAB_12
{
    public partial class PolygonSettingsDialog : Window
    {
        public int NumberOfVertices => (int)VerticesSlider.Value;

        public PolygonSettingsDialog()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
