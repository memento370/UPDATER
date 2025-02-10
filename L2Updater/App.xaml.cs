using System.Windows;

namespace Updater
{
    public partial class App : Application
    {
        /*
		private bool _contentLoaded;

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				base.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
				Uri resourceLocator = new Uri("/VLGame;component/app.xaml", UriKind.Relative);
				Application.LoadComponent(this, resourceLocator);
			}
		}
		
		[STAThread]
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public static void Main()
		{
			App app = new App();
			app.InitializeComponent();
			app.Run();
		}*/













        /*
		 
		        <TaskbarIcon TaskbarIcon.Name="TaskBarIco" TaskbarIcon.ToolTipText="VLGame" TaskbarIcon.PopupActivation="LeftOrDoubleClick" FrameworkElement.Style="{DynamicResource TaskbarIconStyle}" TaskbarIcon.IconSource="l2.ico" p9:TaskbarIcon.DoubleClickCommand="{av:Binding OpenFromTrayCommand}" xmlns:p9="clr-namespace:Hardcodet.Wpf.TaskbarNotification;assembly=Hardcodet.Wpf.TaskbarNotification" xmlns="clr-namespace:Hardcodet.Wpf.TaskbarNotification;assembly=Hardcodet.Wpf.TaskbarNotification">
            <FrameworkElement.ContextMenu xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
                <ContextMenu>
                    <MenuItem Name="TrayStart" Header="Старт" IsEnabled="{Binding IsBusy, Converter={StaticResource InverseBooleanConverter}}" />
                    <MenuItem Name="TrayFC" Header="Полная проверка" IsEnabled="{Binding IsBusy, Converter={StaticResource InverseBooleanConverter}}" />
                    <MenuItem Name="TrayQC" Header="Быстрая проверка" IsEnabled="{Binding IsBusy, Converter={StaticResource InverseBooleanConverter}}" />
                    <MenuItem Name="TrayExit" Header="Выход" Command="{Binding ExitCommand}" />
                </ContextMenu>
            </FrameworkElement.ContextMenu>
            <p9:TaskbarIcon.TrayToolTip>
                <TextBlock Name="TextOnNotify" Text="VLGame" HorizontalAlignment="Center" VerticalAlignment="Center" Foreground="#FFFFF0F0" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" />
            </p9:TaskbarIcon.TrayToolTip>
        </TaskbarIcon>
		
		
		
		    <FrameworkElement.Resources>
        <ResourceDictionary>
            <utillsClasses:InverseBooleanConverter x:Key="InverseBooleanConverter" />
        </ResourceDictionary>
    </FrameworkElement.Resources>
	
	<ResourceDictionary>
            <utillsClasses:InverseBooleanConverter x:Key="InverseBooleanConverter" />
        </ResourceDictionary>
		
		<ResourceDictionary />
		
		 d:DataContext="{d:DesignInstance Type=utillsClasses:ViewModelBase}"
		 
		 {Binding IsBusy, Converter={StaticResource InverseBooleanConverter}}

		*/

    }
}
