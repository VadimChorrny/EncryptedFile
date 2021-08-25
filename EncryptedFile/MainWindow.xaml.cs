using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EncryptedFile
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Thread threads = null;
        private string fileName;
        public MainWindow()
        {
            InitializeComponent();
            fileName = tbFilePath.Text; // in future need remove
        }

        private void btnFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                fileName = dlg.FileName;
                tbFilePath.Text = fileName;
            }
        }

        private void EncryptFile()
        {
            byte[] readedBytes;
            byte[] xoredBytes;

            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                readedBytes = new byte[stream.Length];
                xoredBytes = new byte[stream.Length];

                stream.Read(readedBytes, 0, (int)stream.Length);

                for (int i = 0; i < readedBytes.Length; i++)
                {
                    progress.Value = i;
                    int xoredInt = readedBytes[i] ^ Int32.Parse(tbPassword.Text);
                    xoredBytes[i] = (byte)xoredInt;
                }
            }
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                stream.Write(xoredBytes, 0, xoredBytes.Length);
            }
        }

        private byte[] DecryptFileInMemory()
        {
            byte[] readedBytes;
            byte[] xoredBytes;

            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                readedBytes = new byte[stream.Length];
                xoredBytes = new byte[stream.Length];

                stream.Read(readedBytes, 0, (int)stream.Length);

                for (int i = 0; i < readedBytes.Length; i++)
                {
                    int xoredInt = readedBytes[i] ^ Int32.Parse(tbPassword.Text);
                    xoredBytes[i] = (byte)xoredInt;
                }
            }

            return xoredBytes;
        }

        private void WorkWithFile()
        {
            if ((bool)rbEncrypted.IsChecked)
            {
                rbDecrypted.IsChecked = false;
                threads = new Thread(EncryptFile);
                threads.IsBackground = true;
                threads.Start();
            }
            else if ((bool)rbDecrypted.IsChecked)
            {
                rbEncrypted.IsChecked = false;
            }
            else
            {
                MessageBox.Show("You don't select radio button!");
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            threads = new Thread(WorkWithFile);
            threads.IsBackground = true;
            threads.Start();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            threads.Join();
        }
    }
}
