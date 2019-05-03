using MadsKristensen.AddAnyFile.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MadsKristensen.AddAnyFile
{
    public partial class FileNameDialog : Window
    {
        private const string DEFAULT_TITLE = "Add new file: {0}";
        private const string DEFAULT_TEXT = "Enter a file name";

        private static readonly List<string> _tips = new List<string> {
            "Tip: 'folder/file.ext' also creates a new folder for the file",
            "Tip: You can create files starting with a dot, like '.gitignore'",
            "Tip: You can create files without file extensions, like 'LICENSE'",
            "Tip: Create folder by ending the name with a forward slash",
            "Tip: Use glob style syntax to add related files, like 'widget.(html,js)'",
            "Tip: Separate names with commas to add multiple files and folders"
        };

        public FileNameDialog(string folder, string projectName)
        {
            InitializeComponent();

            lblFolder.Content = string.Format("{0}/", folder);

            Loaded += (s, e) =>
            {
                Icon = BitmapFrame.Create(new Uri("pack://application:,,,/AddAnyFile;component/Resources/icon.png", UriKind.RelativeOrAbsolute));
                Title = string.Format(DEFAULT_TITLE, projectName);
                SetRandomTip();

                txtName.Focus();
                txtName.CaretIndex = 0;
                txtName.Text = DEFAULT_TEXT;
                txtName.Select(0, txtName.Text.Length);

                txtName.PreviewKeyDown += (a, b) =>
                {
                    if (b.Key == Key.Escape)
                    {
                        if (string.IsNullOrWhiteSpace(txtName.Text) || txtName.Text == DEFAULT_TEXT)
                        {
                            Close();
                        }
                        else
                        {
                            txtName.Text = string.Empty;
                        }
                    }
                    else if (txtName.Text == DEFAULT_TEXT)
                    {
                        txtName.Text = string.Empty;
                        btnCreate.IsEnabled = true;
                    }
                };

                txtName.TextChanged += (o, ev) =>
                {
                    if (GetInput() == DEFAULT_TEXT || string.IsNullOrWhiteSpace(GetInput()))
                    {
                        ClearCheckBoxes();
                    }
                    else
                    {
                        SetCheckBoxes();
                    }
                };
            };
        }

        private void SetCheckBoxes()
        {
            var type = InputTypeHelper.DetermineType(GetInput());
            cbClass.IsChecked = (type & SelectedInputType.Class) != SelectedInputType.None;
            cbInterface.IsChecked = (type & SelectedInputType.Interface) != SelectedInputType.None;
            cbController.IsChecked = (type & SelectedInputType.Controller) != SelectedInputType.None;
            cbEnum.IsChecked = (type & SelectedInputType.Enum) != SelectedInputType.None;
            cbFile.IsChecked = type == SelectedInputType.None;
            cbFolder.IsChecked = (type & SelectedInputType.Folder) != SelectedInputType.None;
            cbBase.IsChecked = (type & SelectedInputType.Base) != SelectedInputType.None;

            if (cbBase.IsChecked == true || cbInterface.IsChecked == true || cbClass.IsChecked == true
                || cbController.IsChecked == true || cbEnum.IsChecked == true)
            {
                UncheckFileFolder();
            }
        }

        private void ClearCheckBoxes(params CheckBox[] except)
        {
            foreach (var control in wpTypes.Children)
            {
                if (!except.Contains(control) && control is CheckBox cb)
                {
                    cb.IsChecked = false;
                }
            }
        }

        private string GetInput()
        {
            return txtName.Text.Trim();
        }

        public FileNameDialogResult Input => new FileNameDialogResult
        {
            Class = cbClass.IsChecked == true,
            Controller = cbController.IsChecked == true,
            Input = GetInput(),
            Interface = cbInterface.IsChecked == true,
            Enum = cbEnum.IsChecked == true,
            File = cbFile.IsChecked == true,
            Folder = cbFile.IsChecked == true,
            BaseClass = cbBase.IsChecked == true
        };

        private void SetRandomTip()
        {
            var rnd = new Random(DateTime.Now.GetHashCode());
            var index = rnd.Next(_tips.Count);
            lblTips.Content = _tips[index];
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CbFolder_Click(object sender, RoutedEventArgs e)
        {
            ClearAllButNotFileFolder();
            if (cbFile.IsChecked == true)
            {
                cbFile.IsChecked = false;
            }
        }

        private void ClearAllButNotFileFolder()
        {
            ClearCheckBoxes(cbFile, cbFolder);
        }

        private void CbFile_Click(object sender, RoutedEventArgs e)
        {
            ClearAllButNotFileFolder();
            if (cbFolder.IsChecked == true)
            {
                cbFolder.IsChecked = false;
            }
        }

        private void Default_CheckBoxClick(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked == true)
            {
                UncheckFileFolder();
            }
        }

        private void UncheckFileFolder()
        {
            cbFolder.IsChecked = false;
            cbFile.IsChecked = false;
        }
    }
}
