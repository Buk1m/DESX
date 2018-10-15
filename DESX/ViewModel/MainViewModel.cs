using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DESX.Model;
using DESX.Utility;
using Microsoft.Win32;
using ModemTalking.Commands;

namespace DESX.ViewModel
{
    public class MainViewModel : BaseViewModel
    {

        #region Constructor
        public MainViewModel()
        {
            OpenMessageFile = new RelayCommand(() =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog {Filter = "Text files (*.txt)|*.txt"};
                if (openFileDialog.ShowDialog() != true)
                    return;
                MessageFileName = openFileDialog.FileName;
                MessageTextBox = File.ReadAllText(openFileDialog.FileName);
            });

            OpenKeysFile = new RelayCommand(() =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog {Filter = "Text files (*.txt)|*.txt"};
                if (openFileDialog.ShowDialog() != true)
                    return;
                KeysFileName = openFileDialog.FileName;
                var content = File.ReadAllLines(openFileDialog.FileName);
                if (content.Length >= 3)
                {
                    FirstKey = content[0];
                    SecondKey = content[1];
                    ThirdKey = content[2];
                }
                else
                {
                    MessageBox.Show("Invalid keys format \nFile should contain three keys in separate lines");
                }
            });

            Encrypt = new RelayCommand(() =>
            {
                if (string.IsNullOrEmpty(MessageTextBox) || string.IsNullOrEmpty(FirstKey) ||
                    string.IsNullOrEmpty(SecondKey) || string.IsNullOrEmpty(ThirdKey))
                {
                    MessageBox.Show("Feed all the information");
                    return;
                }

                var keys = new string[] {FirstKey, SecondKey, ThirdKey}.ToList();
                ResultMessage = DESXAlgorithm.Encrypt(MessageTextBox, keys);
            });

            Decrypt = new RelayCommand(() =>
            {
                if (string.IsNullOrEmpty(MessageTextBox) || string.IsNullOrEmpty(FirstKey) ||
                    string.IsNullOrEmpty(SecondKey) || string.IsNullOrEmpty(ThirdKey))
                {
                    MessageBox.Show("Feed all the information");
                    return;
                }

                var keys = new string[] {FirstKey, SecondKey, ThirdKey}.ToList();
                ResultMessage = DESXAlgorithm.Decrypt(MessageTextBox, keys);
            });

            SwitchMessages = new RelayCommand(() =>
            {
                MessageTextBox = ResultMessage;
                ResultMessage = String.Empty;
            });

            GenerateKeys = new RelayCommand(() =>
            {
                FirstKey = Converters.BitArrayToString(Operations.GenerateRandomKey());
                SecondKey = Converters.BitArrayToString(Operations.GenerateRandomKeyWithParityBits());
                ThirdKey = Converters.BitArrayToString(Operations.GenerateRandomKey());
            });
        }
        #endregion

        #region Public Properties

        public string MessageTextBox
        {
            get => _message;

            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public string MessageFileName
        {
            get => _messageFileName;

            set
            {
                _messageFileName = value;
                OnPropertyChanged();
            }
        }

        public string KeysFileName
        {
            get => _keysFileName;

            set
            {
                _keysFileName = value;
                OnPropertyChanged();
            }
        }

        public string ResultMessage
        {
            get => _resultMessage;

            set
            {
                _resultMessage = value;
                OnPropertyChanged();
            }
        }

        public string FirstKey
        {
            get => _firstKey;

            set
            {
                _firstKey = value;
                OnPropertyChanged();
            }
        }

        public string SecondKey
        {
            get => _secondKey;

            set
            {
                _secondKey = value;
                OnPropertyChanged();
            }
        }

        public string ThirdKey
        {
            get => _thirdKey;

            set
            {
                _thirdKey = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public ICommand OpenMessageFile { get; set; }
        public ICommand OpenKeysFile { get; set; }
        public ICommand Decrypt { get; set; }
        public ICommand Encrypt { get; set; }
        public ICommand SwitchMessages { get; set; }
        public ICommand GenerateKeys { get; set; }

        #endregion

        #region Private

        private string _message;
        private string _messageFileName;
        private string _keysFileName;
        private string _resultMessage;
        private string _firstKey = "";
        private string _secondKey = "";
        private string _thirdKey = "";

        #endregion

    }
}