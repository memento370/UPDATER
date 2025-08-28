#define DEBUG
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
//using Hardcodet.Wpf.TaskbarNotification;
using Updater.Annotations;
using Updater.DataContractModels;
using Updater.HashZip.ZIPLib.Zip;
using Updater.Localization;
using Updater.Models;
using Updater.Properties;
using Updater.UtillsClasses;

namespace Updater
{
    public /*sealed*/ partial class MainWindow : Window, INotifyPropertyChanged, IComponentConnector
    {




        private LocString _startText = new LocString("Готов к работе", "Ready to work");

        private LocString _initText = new LocString("Инициализация", "Initialization");

        private LocString _cancelText = new LocString("Отменено пользователем", "Cancel by user");

        private LocString _getUpdateInfo = new LocString("Получение информации о обновлении", "Get update information");

        private LocString _createFolders = new LocString("Создание папок", "Creating folders");

        private LocString _updateSuccess = new LocString("Обновление успешно завершено", "Update success");

        private LocString _updateError = new LocString("Во время обновления произошли ошибки", "Update error");

        private LocString _checkFiles = new LocString("Проверка файлов для обновления", "Check files for update");

        private LocString _selfUpdateStr = new LocString("Доступна новая версия апдейтера", "New version of updater is available");

        private TaskCompletionSource<int> _tcsDownloadUnpucking = new TaskCompletionSource<int>();

        private Queue<FileModel> _downloadQueue = new Queue<FileModel>();

        private Queue<FileModel> _unpuckingQueue = new Queue<FileModel>();

        private bool _hasError;

        private int _step;

        private ulong bytesReceivedAtLastTick;

        private ulong allBytes;

        private const int VERSION = 3;

        private string _updateUrl = "http://l2-absolute.com/api/files/";

        private string _savePath = AppDomain.CurrentDomain.BaseDirectory;

        private CancellationTokenSource _cts = new CancellationTokenSource();

        private bool _isBusy;

        private readonly UpdateConfig _updateConfig = new UpdateConfig();

        private LocString _info;

        private LocString _downloadInfo;

        private double _progressFull = 100.0;

        private double _progressFile = 100.0;

        private double _maxProgressFile = 100.0;

        private double _maxProgressFull = 100.0;

        private string _downloadSpeed;

        /*
		internal Grid grid;

		internal Rectangle BackGround;

		internal Rectangle BackGroundEng;

		internal System.Windows.Controls.Button Cancel;

		internal System.Windows.Controls.Button QuickCheck;

		internal System.Windows.Controls.Button FullCheck;

		internal System.Windows.Controls.Button StartButton;

		internal System.Windows.Controls.ProgressBar FileProgress;

		internal System.Windows.Controls.ProgressBar FullProgress;

		internal System.Windows.Controls.Label Information;

		internal TextBlock ProcFile;

		internal TextBlock ProcFull;

		internal Grid LangGrid;

		internal TextBlock LangTitle;

		internal System.Windows.Controls.RadioButton RusRadio;

		internal System.Windows.Controls.RadioButton EngRadio;

		internal System.Windows.Controls.Label FullSize;

		internal System.Windows.Controls.Label Speed;

		internal TaskbarIcon TaskBarIco;

		internal System.Windows.Controls.MenuItem TrayStart;

		internal System.Windows.Controls.MenuItem TrayFC;

		internal System.Windows.Controls.MenuItem TrayQC;

		internal System.Windows.Controls.MenuItem TrayExit;

		internal TextBlock TextOnNotify;

		internal System.Windows.Controls.Label Download_Information;

		internal System.Windows.Controls.Button Site;

		internal System.Windows.Controls.Button Forum;

		internal System.Windows.Controls.Button Tray;

		internal System.Windows.Controls.Button Exit;

		internal System.Windows.Controls.Button FileButton;

		internal TextBlock PathText;

		internal System.Windows.Controls.Button Donate;

		internal System.Windows.Controls.Button Supp; 


		//private bool _contentLoaded;
		*/

        public bool IsDownload
        {
            get;
            set;
        }

        public bool IsUnpacking
        {
            get;
            set;
        }

        public bool IsQuestion
        {
            get;
            set;
        }

        public string SavePath
        {
            get
            {
                return _savePath;
            }
            set
            {
                _savePath = value;
                OnPropertyChanged("SavePath");
            }
        }

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                _isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }

        public ICommand ExitCommand
        {
            get;
            set;
        }

        public ICommand TrayCommand
        {
            get;
            set;
        }

        public ICommand OpenFromTrayCommand
        {
            get;
            set;
        }

        public ICommand CancelCommand
        {
            get;
            set;
        }

        public ICommand QuickCommand
        {
            get;
            set;
        }

        public ICommand FullCommand
        {
            get;
            set;
        }

        public ICommand StartCommand
        {
            get;
            set;
        }

        public LocString Info
        {
            get
            {
                return _info;
            }
            set
            {
                _info = value;
                Debug.WriteLine(_info?.GetLocStr);
                OnPropertyChanged("Info");
            }
        }

        public string DownloadSpeed
        {
            get
            {
                return _downloadSpeed;
            }
            set
            {
                _downloadSpeed = value;
                OnPropertyChanged("DownloadSpeed");
            }
        }

        public LocString DownloadInfo
        {
            get
            {
                return _downloadInfo;
            }
            set
            {
                _downloadInfo = value;
                Debug.WriteLine(_downloadInfo?.GetLocStr);
                OnPropertyChanged("DownloadInfo");
            }
        }

        public double ProgressFull
        {
            get
            {
                return _progressFull;
            }
            set
            {
                _progressFull = value;
                OnPropertyChanged("ProgressFull");
            }
        }

        public double ProgressFile
        {
            get
            {
                return _progressFile;
            }
            set
            {
                _progressFile = value;
                OnPropertyChanged("ProgressFile");
            }
        }

        public double MaxProgressFile
        {
            get
            {
                return _maxProgressFile;
            }
            set
            {
                _maxProgressFile = value;
                OnPropertyChanged("MaxProgressFile");
            }
        }

        public double MaxProgressFull
        {
            get
            {
                return _maxProgressFull;
            }
            set
            {
                _maxProgressFull = value;
                OnPropertyChanged("MaxProgressFull");
            }
        }

        /*
		 
		<FrameworkElement.Resources>
        <ResourceDictionary>
            <utillsClasses:InverseBooleanConverter x:Key="InverseBooleanConverter" />
        </ResourceDictionary>
		</FrameworkElement.Resources>

		*/



        private bool _isSearching;
        public bool IsSearching
        {
            get { return !_isSearching; }
            set
            {
                if (_isSearching != value)
                {
                    _isSearching = value;
                    OnPropertyChanged("IsSearching");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void radioButton_RUS_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.Lang = 0;
            LangInfo.Lang = Updater.Localization.Languages.Rus;
            OnPropertyChanged("Info");
            Cancel.Content = "ВІДМІНА";
            QuickCheck.Content = "ШВИДКА ПЕРЕВІРКА";
            FullCheck.Content = "ОНОВИТИ";
            StartButton.Content = "";
            Main.Content = "Головна";
            Register.Content = "Реєстрація";
            About.Content = "Про гру";
            BaseInfo.Content = "База знаннь";
            Cabinet.Content = "Кабінет";
     
        }

        private void radioButton_ENG_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.Lang = 1;
            LangInfo.Lang = Updater.Localization.Languages.Eng;
            OnPropertyChanged("Info");
           
            Cancel.Content = "CANCEL";
            QuickCheck.Content = "QUICK CHECK";
            FullCheck.Content = "UPDATE";
            StartButton.Content = "";  // Якщо це кнопка без тексту, залиште порожнім або адаптуйте
            Main.Content = "Main";
            Register.Content = "Registration";
            About.Content = "About";
            BaseInfo.Content = "Info Base";
            Cabinet.Content = "Cabinet";
   
        }

        private void SetLanguage()
        {/*
			switch (Settings.Default.Lang)
			{
			case 0:
				RusRadio.IsChecked = true;
				EngRadio.IsChecked = false;
				break;
			case 1:
				RusRadio.IsChecked = false;
				EngRadio.IsChecked = true;
				break;
			default:
				RusRadio.IsChecked = false;
				EngRadio.IsChecked = true;
				break;
			}
			RusRadio.IsChecked = true;*/
        }

        private void CreatDesctopShortCut()
        {
        }

        private void ShowStandardBalloon(double proc)
        {
            string text = "Progress";
            string text2 = $"{proc:0}%";
            //TaskBarIco.ShowBalloonTip(text, text2, (BalloonIcon)1);
        }

        private void UpdateStart(UpdateTypes type, string updateUrl, CancellationToken token)
        {
            try
            {
                Update(type, updateUrl, token);
            }
            catch (OperationCanceledException)
            {
                Info = _cancelText;
            }
            catch (Exception ex2)
            {
                System.Windows.MessageBox.Show("Ooops error: " + updateUrl + "\n" + ex2.Message);
                Info = _updateError;
            }
        }

        private void UpdateEnd(Task obj)
        {
            ProgressFile = MaxProgressFile;
            ProgressFull = MaxProgressFull;
            IsBusy = false;
            CommandManager.InvalidateRequerySuggested();
            _cts = null;
            DownloadInfo = null;
        }

        private void Update(UpdateTypes type, string updateUrl, CancellationToken token)
        {
            _updateUrl = updateUrl;
            ProgressFull = 0.0;
            Info = _initText;
            string savePath = SavePath;
            ProgressFull += 2.0;
            Info = _getUpdateInfo;
            UpdateInfoModel updateInfo = DownloadUtills.GetUpdateInfo(_updateUrl);
            List<FileModel> allFileInfos = UpdateUtills.GetAllFileInfos(updateInfo.Folder, type, updateInfo.Folder.Name);
            token.ThrowIfCancellationRequested();
            IEnumerable<FileModel> source = allFileInfos.Where((FileModel c) => !c.CheckHash);
            ProgressFull += 5.0;
            FileModel[] array = source.ToArray();
            Info = _createFolders;
            FolderUtills.CheckOrCreateFolders(updateInfo.Folder, savePath);
            token.ThrowIfCancellationRequested();
            ProgressFull += 3.0;
            Info = _checkFiles;
            FileModel[] array2 = CheckFilesAsync(allFileInfos, savePath, type, token);
            token.ThrowIfCancellationRequested();
            if (array2.Any())
            {
                DownloadAndUnpacking(array2, token);
            }
            DownloadInfo = null;
            Info = ((!_hasError) ? _updateSuccess : _updateError);
            MaxProgressFull = 100.0;
            ProgressFull = 100.0;
        }

        public FileModel[] CheckFilesAsync(IEnumerable<FileModel> files, string rootPath, UpdateTypes type, CancellationToken token)
        {
            MaxProgressFull = files.Count();
            ProgressFull = 0.0;
            List<FileModel> list = new List<FileModel>();
            foreach (FileModel file in files)
            {
                token.ThrowIfCancellationRequested();
                DownloadInfo = new LocString("Проверяем " + file.Name, "Check " + file.Name);
                if (!UpdateUtills.CheckFile(rootPath, file, type))
                {
                    DownloadInfo = new LocString("Требуется обновить " + file.Name, "Need Update " + file.Name);
                    list.Add(file);
                }
                else
                {
                    DownloadInfo = new LocString(file.Name + " OK", file.Name + " OK");
                }
                ProgressFull++;
            }
            return list.ToArray();
        }

        private void DownloadAndUnpacking(FileModel[] filesForUpdate, CancellationToken token)
        {
            _hasError = false;
            MaxProgressFull = filesForUpdate.Length;
            ProgressFull = 0.0;
            _tcsDownloadUnpucking = new TaskCompletionSource<int>();
            Task<int> task = _tcsDownloadUnpucking.Task;
            _downloadQueue = new Queue<FileModel>(filesForUpdate);
            _unpuckingQueue = new Queue<FileModel>();
            Task.Factory.StartNew(delegate
            {
                DownloadFiles(token);
            }, token);
            task.Wait(token);
            token.ThrowIfCancellationRequested();
        }

        private void DownloadFiles(CancellationToken token)
        {
            allBytes = 0uL;
            bytesReceivedAtLastTick = 0uL;
            System.Timers.Timer timer = CreateDownloadTimer();
            timer.Start();
            IsDownload = true;

            //string remotePath = "";
            //string savePath = "";


            while (_downloadQueue.Any())
            {
                FileModel fileModel = _downloadQueue.Dequeue();
                try
                {
                    while (IsQuestion)
                    {
                        Thread.Sleep(1000);
                    }
                    token.ThrowIfCancellationRequested();
                    Info = new LocString("Качаем " + fileModel.Name, "Download " + fileModel.Name);
                    string remotePath = System.IO.Path.Combine(_updateUrl, fileModel.Path, fileModel.Name + ".zip");
                    string savePath = System.IO.Path.Combine(SavePath, fileModel.SavePath, fileModel.Name + ".zip");
                    Download(savePath, remotePath, fileModel, token);
                    Info = new LocString("Cкачали " + fileModel.Name, "Downloaded " + fileModel.Name);
                    token.ThrowIfCancellationRequested();
                    _unpuckingQueue.Enqueue(fileModel);
                    if (!IsUnpacking)
                    {
                        IsUnpacking = true;
                        Task.Factory.StartNew(delegate
                        {
                            UnpackingFiles(token);
                        }, token);
                    }
                }
                catch (OperationCanceledException)
                {
                    _tcsDownloadUnpucking.TrySetResult(0);
                    IsDownload = false;
                    timer.Stop();
                    string text3 = DownloadSpeed = (DownloadSpeed = "");
                    timer.Dispose();
                    return;
                }
                catch (Exception exx)
                {
                    _hasError = true;
                    Info = new LocString("Не удалось скачать " + fileModel.Name, "Error download " + fileModel.Name);
                    //System.Windows.MessageBox.Show(Info.GetLocStr + "\n" + remotePath + "\n" + savePath + "\n" + Environment.NewLine + "[" + exx.Message + "]");
                    System.Windows.MessageBox.Show(Info.GetLocStr + "\n" + Environment.NewLine + "[" + exx.Message + "]");
                    if (File.Exists(System.IO.Path.Combine(fileModel.SavePath, fileModel.Name + ".zip")))
                    {
                        File.Delete(System.IO.Path.Combine(fileModel.SavePath, fileModel.Name + ".zip"));
                    }
                }
            }
            timer.Stop();
            DownloadSpeed = "";
            timer.Dispose();
            IsDownload = false;
            TryFinish();
        }

        private System.Timers.Timer CreateDownloadTimer()
        {
            System.Timers.Timer timer = new System.Timers.Timer
            {
                Interval = 1000.0
            };
            timer.Elapsed += delegate
            {
                ulong num = allBytes;
                ulong num2 = num - bytesReceivedAtLastTick;
                DownloadSpeed = $"{(double)num2 / 1024.0 / 1024.0:0.00} Мb/s";
                bytesReceivedAtLastTick = num;
            };
            return timer;
        }

        private void UnpackingFiles(CancellationToken token)
        {
            IsUnpacking = true;
            while (_unpuckingQueue.Any())
            {
                FileModel fileModel = _unpuckingQueue.Dequeue();
                try
                {
                    token.ThrowIfCancellationRequested();
                    Info = new LocString("Распаковаем " + fileModel.Name, "Unpacking " + fileModel.Name);
                    string text = System.IO.Path.Combine(SavePath, fileModel.SavePath, fileModel.Name);
                    string fileFolderPath = System.IO.Path.Combine(SavePath, fileModel.SavePath);
                    Unpacking(fileFolderPath, text, token);
                    Info = new LocString("Проверяем " + fileModel.Name, "Checking " + fileModel.Name);
                    if (UpdateUtills.CheckFile(SavePath, fileModel, UpdateTypes.Full, postCheck: true))
                    {
                        File.Delete(text + ".zip");
                        Info = new LocString("Успешно " + fileModel.Name, "Success " + fileModel.Name);
                        ProgressFull++;
                        token.ThrowIfCancellationRequested();
                        continue;
                    }
                    _downloadQueue.Enqueue(fileModel);
                    if (!IsDownload)
                    {
                        IsDownload = true;
                        Task.Factory.StartNew(delegate
                        {
                            DownloadFiles(token);
                        }, token);
                    }
                }
                catch (OperationCanceledException)
                {
                    _tcsDownloadUnpucking.TrySetResult(0);
                    IsUnpacking = false;
                    return;
                }
                catch (Exception)
                {
                    _hasError = true;
                    Info = new LocString("Не удалось распаковать " + fileModel.Name, "Error unpacking " + fileModel.Name);
                    System.Windows.MessageBox.Show(Info.GetLocStr);
                    if (File.Exists(fileModel.SavePath + fileModel.Name + ".zip"))
                    {
                        File.Delete(fileModel.SavePath + fileModel.Name + ".zip");
                    }
                }
            }
            IsUnpacking = false;
            TryFinish();
        }

        private void Download(string savePath, string remotePath, FileModel file, CancellationToken token, bool retry = true)
        {
            try
            {
                long num = 0L;
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(remotePath);
                httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
                if (File.Exists(savePath))
                {
                    num = new FileInfo(savePath).Length;
                }
                httpWebRequest.AddRange(num);
                using HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using Stream stream = httpWebResponse.GetResponseStream();
                long contentLength = httpWebResponse.ContentLength;
                using FileStream fileStream = (num == 0L) ? new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None) : new FileStream(savePath, FileMode.Append, FileAccess.Write, FileShare.None);
                byte[] array = new byte[2048];
                MaxProgressFile = contentLength + num;
                int num2;
                while ((num2 = stream.Read(array, 0, array.Length)) > 0)
                {
                    token.ThrowIfCancellationRequested();
                    fileStream.Write(array, 0, num2);
                    ProgressFile = fileStream.Length;
                    int num3 = (int)(ProgressFile * 100.0 / MaxProgressFile);
                    DownloadInfo = new LocString($"Скачивание {file.Name} {num3}%", $"Downloading {file.Name} {num3}%");
                    allBytes += (ulong)num2;
                }
                DownloadInfo = null;
            }
            catch (Exception ex)
            {
                if (retry)
                {
                    if (File.Exists(file.SavePath + file.Name + ".zip"))
                    {
                        File.Delete(file.SavePath + file.Name + ".zip");
                    }
                    Download(savePath, remotePath, file, token, retry: false);
                    return;
                }
                throw ex;
            }
        }

        private static void Unpacking(string fileFolderPath, string filePath, CancellationToken token)
        {
            using ZipFile zipFile = ZipFile.Read(filePath + ".zip");
            foreach (ZipEntry item in zipFile)
            {
                token.ThrowIfCancellationRequested();
                item.Extract(fileFolderPath, ExtractExistingFileAction.OverwriteSilently);
            }
        }

        private void TryFinish()
        {
            if (!IsDownload && !IsUnpacking)
            {
                _tcsDownloadUnpucking.TrySetResult(1);
            }
        }

        public bool AskQuestion(string message)
        {
            IsQuestion = true;
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(message, "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
            IsQuestion = false;
            return messageBoxResult == MessageBoxResult.OK;
        }

        private int GetRemoteVersion(string link)
        {
            try
            {
                using WebClient webClient = new WebClient();
                string value = webClient.DownloadString(System.IO.Path.Combine(link, "updaterver.txt"));
                return Convert.ToInt32(value);
            }
            catch
            {
                return 0;
            }
        }

        private bool DownloadFile(string link, string fileName)
        {
            try
            {
                using WebClient webClient = new WebClient();
                webClient.DownloadFile(System.IO.Path.Combine(link, fileName), fileName);
                if (File.Exists(fileName))
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("Hardcodet"))
            {
                return Assembly.Load(Updater.Properties.Resources.Hardcodet_Wpf_TaskbarNotification);
            }
            return null;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CreateCommands()
        {

            ExitCommand = new RelayCommand(delegate
            {
                //((UIElement)(object)TaskBarIco).Visibility = Visibility.Hidden;
                Settings.Default.Save();
                System.Windows.Application.Current.Shutdown();
            });
            TrayCommand = new RelayCommand(delegate
            {
                ShowStandardBalloon(ProgressFull * 100.0 / MaxProgressFull);
                base.WindowState = WindowState.Minimized;
            });
            OpenFromTrayCommand = new RelayCommand(delegate
            {
                base.WindowState = WindowState.Normal;
            });
            CancelCommand = new RelayCommand(delegate
            {
                _cts?.Cancel();
            }, (object can) => IsBusy);
            QuickCommand = new RelayCommand(delegate
            {
                StartUpdateTask(UpdateTypes.Quick);
            }, (object can) => !IsBusy);
            FullCommand = new RelayCommand(delegate
            {
                StartUpdateTask(UpdateTypes.Full);
            }, (object can) => !IsBusy);
            StartCommand = new RelayCommand(delegate
            {
                StartGame();
            }, (object can) => !IsBusy);
        }

        private void StartGame()
        {
            string currentGamePath = GetCurrentGamePath();
            if (!File.Exists(currentGamePath + "\\l2.exe"))
            {
                System.Windows.MessageBox.Show("Not found l2.exe");
                return;
            }
            int num = 429568;
            FileInfo fileInfo = new FileInfo(currentGamePath + "\\l2.exe");
            if (fileInfo.Length != num)
            {
                //Process.Start(currentGamePath + "\\l2.exe");
                ProcessStartInfo pi = new ProcessStartInfo();


                pi.WorkingDirectory = currentGamePath + "\\";

                pi.FileName = @"l2.exe";

                pi.UseShellExecute = true;

                try
                {
                    Process.Start(pi);

                    /*
                    //создаем новый процесс
                    Process proc = new Process();
                    //Запускаем Блокнто
                    //proc.StartInfo.FileName = @"Notepad.exe";
                    proc.StartInfo.FileName = pi.FileName;
                    proc.StartInfo.Arguments = "";
                    proc.StartInfo.UseShellExecute = true;
                    proc.Start();*/
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("CAn not run is " + pi.WorkingDirectory + pi.FileName + "... " + Environment.NewLine + " [" + ex.Message + "]");
                    return;
                }
                System.Windows.Application.Current.Shutdown();
                return;
            }
            else
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = currentGamePath + "\\l2.bin",
                    Verb = "runas",
                    WorkingDirectory = "system",
                    UseShellExecute = false
                };
                Process process = new Process
                {
                    StartInfo = startInfo
                };
                process.Start();
            }
            System.Windows.Application.Current.Shutdown();
        }

        private string GetCurrentGamePath()
        {
            return LangInfo.Lang switch
            {
                Updater.Localization.Languages.Rus => System.IO.Path.Combine(SavePath, "system_ru"),
                Updater.Localization.Languages.Eng => System.IO.Path.Combine(SavePath, "system_en"),
                _ => System.IO.Path.Combine(SavePath, "system"),
            };
        }

        public string GetPatchPath()
        {
            bool flag = true;
            return System.IO.Path.Combine(_updateConfig.PatchPath);
        }

        private void StartUpdateTask(UpdateTypes type)
        {
            IsBusy = true;
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            CancellationToken token = _cts.Token;
            string patchUrl = GetPatchPath();
            Task.Factory.StartNew(delegate
            {
                UpdateStart(type, patchUrl, token);
            }, token).ContinueWith(UpdateEnd, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void SelfUpdate()
        {
            try
            {
                string text = "upd.exe";
                if (File.Exists(text))
                {
                    File.Delete(text);
                }
                if (_updateConfig.UpdaterVersion > 3)
                {
                    System.Windows.MessageBox.Show(_selfUpdateStr.GetLocStr);
                    if (DownloadFile(_updateConfig.SelfUpdatePath, text))
                    {
                        string text2 = System.Windows.Application.ResourceAssembly.GetName().Name + ".exe";
                        string fileName = System.IO.Path.GetFileName(Assembly.GetEntryAssembly().Location);
                        Process.Start(text, _updateConfig.SelfUpdatePath + " " + text2 + " " + fileName);
                        System.Windows.Application.Current.Shutdown();
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("upd not found, self update canceled");
                    }
                }
            }
            catch
            {
            }
        }

        public MainWindow()
        {
            // using System.Net;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
                                                        | SecurityProtocolType.Tls
                                                       |
                                                       SecurityProtocolType.Tls11
                                                       |
                                                       SecurityProtocolType.Ssl3;
            // Use SecurityProtocolType.Ssl3 if needed for compatibility reasons

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            //InitializeComponent();

            SetLanguage();
            CreateCommands();
            base.DataContext = this;
            Info = _startText;
            _updateConfig = GetUpdateConfig();
            base.Title = _updateConfig.UpdaterTitle;
        }

        private UpdateConfig GetUpdateConfig()
        {
            try
            {
                return DownloadUtills.GetUpdateConfig(_updateUrl);

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Config not found, stop working.... " + Environment.NewLine + " [" + ex.Message + "]");
                Environment.Exit(0);
            }
            return new UpdateConfig();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            //SelfUpdate();
            //StartUpdateTask(UpdateTypes.Quick);
        }

        private void Site_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(_updateConfig.SiteLink);
        }

        private void Reg_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(_updateConfig.RegLink);
        }

        private void Donate_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(_updateConfig.DonationLink);
        }

        private void Supp_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(_updateConfig.SupportLink);
        }

        private void FileButton_Click(object sender, RoutedEventArgs e)
        {
            ChooseFolder();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(_updateConfig.AboutServerLink);
        }
        private void Base_Info_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(_updateConfig.BaseInfoLink);
        }
        private void Cabinet_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(_updateConfig.CabinetLink);
        }

        private void ChooseFolder()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.SelectedPath = SavePath;
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Settings.Default.SavePath = folderBrowserDialog.SelectedPath;
                SavePath = folderBrowserDialog.SelectedPath;
                Settings.Default.Save();
            }
        }


        /*
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri resourceLocator = new Uri("/VLGame;component/mainwindow.xaml", UriKind.Relative);
				System.Windows.Application.LoadComponent(this, resourceLocator);
			}
		}

		/*
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Expected O, but got Unknown
			switch (connectionId)
			{
			case 1:
				((MainWindow)target).ContentRendered += Window_ContentRendered;
				break;
			case 2:
				grid = (Grid)target;
				grid.MouseDown += Grid_MouseDown;
				break;
			case 3:
				BackGround = (Rectangle)target;
				break;
			case 4:
				BackGroundEng = (Rectangle)target;
				break;
			case 5:
				Cancel = (System.Windows.Controls.Button)target;
				break;
			case 6:
				QuickCheck = (System.Windows.Controls.Button)target;
				break;
			case 7:
				FullCheck = (System.Windows.Controls.Button)target;
				break;
			case 8:
				StartButton = (System.Windows.Controls.Button)target;
				break;
			case 9:
				FileProgress = (System.Windows.Controls.ProgressBar)target;
				break;
			case 10:
				FullProgress = (System.Windows.Controls.ProgressBar)target;
				break;
			case 11:
				Information = (System.Windows.Controls.Label)target;
				break;
			case 12:
				ProcFile = (TextBlock)target;
				break;
			case 13:
				ProcFull = (TextBlock)target;
				break;
			case 14:
				LangGrid = (Grid)target;
				break;
			case 15:
				LangTitle = (TextBlock)target;
				break;
			case 16:
				RusRadio = (System.Windows.Controls.RadioButton)target;
				RusRadio.Checked += radioButton_RUS_Checked;
				break;
			case 17:
				EngRadio = (System.Windows.Controls.RadioButton)target;
				EngRadio.Checked += radioButton_ENG_Checked;
				break;
			case 18:
				FullSize = (System.Windows.Controls.Label)target;
				break;
			case 19:
				Speed = (System.Windows.Controls.Label)target;
				break;
			case 20:
				TaskBarIco = (TaskbarIcon)target;
				break;
			case 21:
				TrayStart = (System.Windows.Controls.MenuItem)target;
				break;
			case 22:
				TrayFC = (System.Windows.Controls.MenuItem)target;
				break;
			case 23:
				TrayQC = (System.Windows.Controls.MenuItem)target;
				break;
			case 24:
				TrayExit = (System.Windows.Controls.MenuItem)target;
				break;
			case 25:
				TextOnNotify = (TextBlock)target;
				break;
			case 26:
				Download_Information = (System.Windows.Controls.Label)target;
				break;
			case 27:
				Site = (System.Windows.Controls.Button)target;
				Site.Click += Site_Click;
				break;
			case 28:
				Forum = (System.Windows.Controls.Button)target;
				Forum.Click += Forum_Click;
				break;
			case 29:
				Tray = (System.Windows.Controls.Button)target;
				break;
			case 30:
				Exit = (System.Windows.Controls.Button)target;
				break;
			case 31:
				FileButton = (System.Windows.Controls.Button)target;
				FileButton.Click += FileButton_Click;
				break;
			case 32:
				PathText = (TextBlock)target;
				break;
			case 33:
				Donate = (System.Windows.Controls.Button)target;
				Donate.Click += Donate_Click;
				break;
			case 34:
				Supp = (System.Windows.Controls.Button)target;
				Supp.Click += Supp_Click;
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}

		*/
        private void SwitchLanguageButton_Click(object sender, RoutedEventArgs e)
        {
            // Перемикаємо мову циклічно: Rus -> Eng -> Rus...
            if (LangInfo.Lang == Updater.Localization.Languages.Rus)
            {
                LangInfo.Lang = Updater.Localization.Languages.Eng;
                Settings.Default.Lang = 1;  // 1 для Eng
            }
            else
            {
                LangInfo.Lang = Updater.Localization.Languages.Rus;
                Settings.Default.Lang = 0;  // 0 для Rus
            }

            // Зберігаємо налаштування
            Settings.Default.Save();

            // Оновлюємо UI (наприклад, текст кнопок, якщо потрібно)
            OnPropertyChanged("Info");  // Оновлює bindings, якщо є залежності від мови

            // Опціонально: показуємо повідомлення користувачу
            //MessageBox.Show("Мову змінено. Шлях до системних файлів оновлено.");

            // Якщо потрібно, перезавантажуємо UI або оновлюємо інші елементи
            // Наприклад, якщо є фонові зображення або тексти, що залежать від мови, викличте методи на кшталт radioButton_ENG_Checked або radioButton_RUS_Checked
            if (LangInfo.Lang == Updater.Localization.Languages.Rus)
            {
                radioButton_RUS_Checked(null, null);
            }
            else
            {
                radioButton_ENG_Checked(null, null);
            }
        }
    }


}
