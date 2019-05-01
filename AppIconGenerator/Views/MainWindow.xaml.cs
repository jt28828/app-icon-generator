using System.ComponentModel;
using System.Windows;
using AppIconGenerator.ViewModels;

namespace AppIconGenerator.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly MainPageViewModel _viewModel;

        public MainWindow()
        {
            _viewModel = new MainPageViewModel();
            InitializeComponent();
            _viewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        private void OnAddButtonClicked(object sender, RoutedEventArgs e)
        {
            _viewModel.OpenFilePicker();
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_viewModel.SelectedImagePreview))
            {
                MainImage.Source = _viewModel.SelectedImagePreview;
                SetControlsVisible();
            }
        }

        private void OnSaveButtonClicked(object sender, RoutedEventArgs e)
        {
            string fileName = null;

            if (!string.IsNullOrEmpty(FileNameBox.Text))
            {
                fileName = FileNameBox.Text;
            }

            _viewModel.SaveImages(fileName);
        }

        /// <summary>
        /// Sets the controls for saving items as visible
        /// </summary>
        private void SetControlsVisible()
        {
            Resources["FileNameLabel"] = Resources["FileNameLabelVisible"];
            Resources["FileNameTextBox"] = Resources["FileNameTextBoxVisible"];
            Resources["SaveButton"] = Resources["SaveButtonVisible"];
        }
    }
}