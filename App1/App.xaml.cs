using VertiGIS.Mobile;
using VertiGIS.Mobile.Infrastructure.Configuration;

namespace App1
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            AppManager.Initialize(this);

            // Uncomment the following to add the styles from this page to the application.
            // Overrides default styles provided by VertiGIS.Mobile.
            //var res = new Styles().Resources;
            //Resources.MergedDictionaries.Add(res);

            // Register additional assemblies to search for configured assembly attributes.
            AppManager.Instance.AssemblyManager.RegisterAssemblies(this.GetType().Assembly);
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new ContentPage()
            {
                Content = GetContent()
            });
        }

        private View GetContent()
        {
            // Stack
            var stack = new VerticalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Spacing = 15
            };

            // Spinner
            var spinner = new ActivityIndicator
            {
                IsRunning = true,
                WidthRequest = 75,
                HeightRequest = 75
            };
            stack.Children.Add(spinner);

            // Label
            var label = new Label()
            {
                TextColor = Colors.Black
            };
            stack.Children.Add(label);

            void ShowStatus(object? sender, LoadingEventArgs e)
            {
                if (GlobalConfiguration.Instance.StartupLoadStatus)
                {
                    label.Text = e.LoadAction;
                }
                else
                {
                    AppManager.LoadingAction -= ShowStatus;
                }
            }

            AppManager.LoadingAction += ShowStatus;

            return stack;
        }

        protected override async void OnStart()
        {
            await AppManager.Instance.InitializeAsync();

            var appPage = await AppManager.Instance.Bootstrapper.LoadAppAsync(new Uri("resource://app.json"));
            await AppManager.Instance.Bootstrapper.DisplayAppAsync(appPage.Page);
        }

        // <inheritdoc/>
        protected override void OnSleep()
        {
            AppManager.Instance.OnBackgrounded();

            base.OnSleep();
        }

        // <inheritdoc/>
        protected override void OnResume()
        {
            // OnResume will get called when background processing begins.
            // UWP activated events are raised in UWP App.xaml.cs.
            if (DeviceInfo.Platform != DevicePlatform.WinUI)
            {
                AppManager.Instance.OnActivated();
            }

            base.OnResume();
        }
    }
}
