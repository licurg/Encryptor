using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Microsoft.Win32;
using System.Security.Cryptography;
using System.IO;
using System.Collections;


namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        private int action;
        public int algorithm;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void OnLoad(object sender, RoutedEventArgs e)
        {
            algorithm = 0;
            settings.NavigationService.Navigate(new Uri("TranspositionAlg.xaml", UriKind.Relative));
            passwords.NavigationService.Navigate(new Uri("TranspositionDecr.xaml", UriKind.Relative));
        }
        private void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG;)|*.BMP;*.JPG;*.GIF;*.PNG;";
            openFile.RestoreDirectory = true;
            if (openFile.ShowDialog() == true)
            {
                filePath.Text = openFile.FileName;
                
                beforeImg.Source = new BitmapImage(new Uri(openFile.FileName));
            }
        }
        private void TabChanged(object sender, SelectionChangedEventArgs e)
        {
            switch((sender as TabControl).SelectedIndex)
            {
                case 0:
                    action = 0;
                    encrypt.Visibility = Visibility.Visible;
                    inverse.Visibility = Visibility.Visible;

                    decrypt.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    action = 1;
                    encrypt.Visibility = Visibility.Collapsed;
                    inverse.Visibility = Visibility.Collapsed;

                    decrypt.Visibility = Visibility.Visible;
                    break;
            }
        }
        private void AlgoChanged(object sender, EventArgs e)
        {
            switch((sender as ComboBox).SelectedIndex)
            {
                case 0:
                    algorithm = 0;
                    if (action == 0)
                        settings.NavigationService.Navigate(new Uri("TranspositionAlg.xaml", UriKind.Relative));
                    else
                        passwords.NavigationService.Navigate(new Uri("TranspositionDecr.xaml", UriKind.Relative));
                    break;
                case 1:
                    algorithm = 1;
                    if (action == 0)
                        settings.NavigationService.Navigate(new Uri("DesAlg.xaml", UriKind.Relative));
                    else
                        passwords.NavigationService.Navigate(new Uri("DesDecr.xaml", UriKind.Relative));
                    break;
                case 2:
                    algorithm = 2;
                    if (action == 0)
                        settings.NavigationService.Navigate(new Uri("TripleDesAlg.xaml", UriKind.Relative));
                    else
                        passwords.NavigationService.Navigate(new Uri("TripleDesDecr.xaml", UriKind.Relative));
                    break;
                case 3:
                    algorithm = 3;
                    if (action == 0)
                        settings.NavigationService.Navigate(new Uri("AesAlg.xaml", UriKind.Relative));
                    else
                        passwords.NavigationService.Navigate(new Uri("AesDecr.xaml", UriKind.Relative));
                    break;
                case 4:
                    algorithm = 4;
                    if (action == 0)
                        settings.NavigationService.Navigate(new Uri("Rc2Alg.xaml", UriKind.Relative));
                    else
                        passwords.NavigationService.Navigate(new Uri("RC2Decr.xaml", UriKind.Relative));
                    break;
            }
        }
        private void StartEncrypt(object sender, RoutedEventArgs e)
        {
            RunAlgorithm(algorithm, 0, (beforeImg.Source as BitmapSource));
        }
        private void inverseEncrypt(object sender, RoutedEventArgs e)
        {
            RunAlgorithm(algorithm, 1, (afterImg.Source as BitmapSource));
        }
        private void StartDecrypt(object sender, RoutedEventArgs e)
        {
            RunAlgorithm(algorithm, 2, (beforeImg.Source as BitmapSource));
        }
        private void Export(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "BMP Files|*.bmp";
            saveFileDialog.FilterIndex = 3;
            saveFileDialog.CreatePrompt = true;
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.RestoreDirectory = true;

            if ((bool)saveFileDialog.ShowDialog())
            {
                dynamic encoder = new BmpBitmapEncoder();

                encoder.Frames.Add(BitmapFrame.Create((afterImg.Source as BitmapSource)));
                using (FileStream stream = File.Open(saveFileDialog.FileName, FileMode.Create))
                {
                    encoder.Save(stream);
                    stream.Close();
                }
                if(action == 0)
                {
                    window.Cursor = Cursors.Wait;
                    string key = "";
                    switch (algorithm)
                    {
                        case 0:
                            TranspositionAlg transposition = new TranspositionAlg();
                            key = transposition.ManageExport();
                            break;
                        case 1:
                            DesAlg des = new DesAlg();
                            key = des.ManageExport();
                            break;
                        case 2:
                            TripleDesAlg tripleDes = new TripleDesAlg();
                            key = tripleDes.ManageExport();
                            break;
                        case 3:
                            AesAlg aes = new AesAlg();
                            key = aes.ManageExport();
                            break;
                        case 4:
                            Rc2Alg rc2 = new Rc2Alg();
                            key = rc2.ManageExport();
                            break;
                    }
                    using (StreamWriter file = new StreamWriter(saveFileDialog.FileName + ".txt", false))
                    {
                        file.Write(key);
                        key = "";
                        file.Close();
                    }
                    window.Cursor = Cursors.Arrow;
                    MessageBox.Show("Файл розшифровки: " + saveFileDialog.FileName + ".txt) успішно створено!", "Успіх!", MessageBoxButton.OK);
                }
            }
        }
        private void OpenKey(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Text Files(*.txt)|*.txt";
            openFile.RestoreDirectory = true;
            if (openFile.ShowDialog() == true)
            {
                window.Cursor = Cursors.Wait;
                keyPath.Text = openFile.FileName;
                switch (algorithm)
                {
                    case 0:
                        dynamic decr = new TranspositionDecr();
                        bool result = decr.ReadKey(openFile.FileName);
                        if (result)
                            passwords.Visibility = Visibility.Visible;
                        else
                            passwords.Visibility = Visibility.Collapsed;
                        break;
                    case 1:
                        decr = new DesDecr();
                        result = decr.ReadKey(openFile.FileName);
                        if (result)
                            passwords.Visibility = Visibility.Visible;
                        else
                            passwords.Visibility = Visibility.Collapsed;
                        break;
                    case 2:
                        decr = new TripleDesDecr();
                        result = decr.ReadKey(openFile.FileName);
                        if (result)
                            passwords.Visibility = Visibility.Visible;
                        else
                            passwords.Visibility = Visibility.Collapsed;
                        break;
                    case 3:
                        decr = new AesDecr();
                        result = decr.ReadKey(openFile.FileName);
                        if (result)
                            passwords.Visibility = Visibility.Visible;
                        else
                            passwords.Visibility = Visibility.Collapsed;
                        break;
                    case 4:
                        decr = new RC2Decr();
                        result = decr.ReadKey(openFile.FileName);
                        if (result)
                            passwords.Visibility = Visibility.Visible;
                        else
                            passwords.Visibility = Visibility.Collapsed;
                        break;
                }
                window.Cursor = Cursors.Arrow;
            }
        }
        private void RunAlgorithm(int algorithm, int action, BitmapSource image)
        {
            window.Cursor = Cursors.Wait;
            switch (algorithm)
            {
                case 0:
                    if (action != 2)
                    {
                        TranspositionAlg transposition = new TranspositionAlg();
                        afterImg.Source = transposition.RunTransposition(image, action);
                    }
                    else
                    {
                        TranspositionDecr transposition = new TranspositionDecr();
                        afterImg.Source = transposition.DecryptImg(image);
                    }
                    break;
                case 1:
                    if (action != 2)
                    {
                        DesAlg des = new DesAlg();
                        afterImg.Source = des.RunDes(image, action);
                    }
                    else
                    {
                        DesDecr des = new DesDecr();
                        afterImg.Source = des.DecryptImg(image);
                    }
                    break;
                case 2:
                    if (action != 2)
                    {
                        TripleDesAlg tripleDes = new TripleDesAlg();
                        afterImg.Source = tripleDes.RunTripleDes(image, action);
                    }
                    else
                    {
                        TripleDesDecr des = new TripleDesDecr();
                        afterImg.Source = des.DecryptImg(image);
                    }
                    break;
                case 3:
                    if (action != 2)
                    {
                        AesAlg aes = new AesAlg();
                        afterImg.Source = aes.RunAes(image, action);
                    }
                    else
                    {
                        AesDecr aes = new AesDecr();
                        afterImg.Source = aes.DecryptImg(image);
                    }
                    break;
                case 4:
                    if (action != 2)
                    {
                        Rc2Alg rc2 = new Rc2Alg();
                        afterImg.Source = rc2.RunRc2(image, action);
                    }
                    else
                    {
                        RC2Decr rc2 = new RC2Decr();
                        afterImg.Source = rc2.DecryptImg(image);
                    }
                    break;
            }
            window.Cursor = Cursors.Arrow;
        }
    }
}
