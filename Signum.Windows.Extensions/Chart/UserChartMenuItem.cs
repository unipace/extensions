﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Entities.Extensions;
using Signum.Entities.DynamicQuery;
using Signum.Entities;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.IO;
using Microsoft.Win32;
using Prop = Signum.Windows.Extensions.Properties;
using Signum.Services;
using System.Windows.Documents;
using Signum.Utilities;
using Signum.Entities.Chart;
using Signum.Windows.Reports;
using Signum.Entities.Authorization;
using Signum.Windows.Authorization;
using System.Windows.Data;

namespace Signum.Windows.Chart
{
    public class UserChartMenuItem : MenuItem
    {
        public static readonly DependencyProperty CurrentUserChartProperty =
            DependencyProperty.Register("CurrentUserChart", typeof(UserChartDN), typeof(UserChartMenuItem), new UIPropertyMetadata((d, e) => ((UserChartMenuItem)d).UpdateCurrent((UserChartDN)e.NewValue)));
        public UserChartDN CurrentUserChart
        {
            get { return (UserChartDN)GetValue(CurrentUserChartProperty); }
            set { SetValue(CurrentUserChartProperty, value); }
        }

        public ChartWindow ChartWindow { get; set; }

        public UserChartMenuItem()
        {
            if (!Navigator.IsViewable(typeof(UserChartDN), true))
                Visibility = System.Windows.Visibility.Hidden;

            this.Loaded += new RoutedEventHandler(UserChartMenuItem_Loaded);
        }

        void UserChartMenuItem_Loaded(object sender, RoutedEventArgs e)
        {
            Initialize(); 
        }

        ChartRequest ChartRequest
        {
            get { return (ChartRequest)ChartWindow.DataContext; }
            set { ChartWindow.DataContext = value; }
        }

        QueryDescription Description
        {
            get { return Navigator.Manager.GetQueryDescription(ChartRequest.QueryName); }
        }

        private void UpdateCurrent(UserChartDN current)
        {
            Header = new TextBlock
            {
                Inlines = 
                { 
                    new Run(
                    current == null ? Prop.Resources.MyCharts : current.DisplayName), 
                    UserCharts == null || UserCharts.Count==0 ? (Inline)new Run():  new Bold(new Run(" (" + UserCharts.Count + ")")) 
                }
            };

            foreach (var item in this.Items.OfType<MenuItem>().Where(mi => mi.IsCheckable))
            {
                item.IsChecked = ((Lite<UserChartDN>)item.Tag).RefersTo(current);
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Icon = ExtensionsImageLoader.GetImageSortName("favorite.png").ToSmallImage();
        }

        List<Lite<UserChartDN>> UserCharts; 

        public void Initialize()
        {
            Items.Clear();

            this.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(MenuItem_Clicked));

            UserCharts = Server.Return((IChartServer s) => s.GetUserCharts(ChartRequest.QueryName));
            
            if (UserCharts.Count > 0)
            {
                foreach (Lite<UserChartDN> uc in UserCharts)
                {
                    MenuItem mi = new MenuItem()
                    {
                        IsCheckable = true,
                        Header = uc.ToString(),
                        Tag = uc,
                    };
                    Items.Add(mi);
                }
            }

            UpdateCurrent(CurrentUserChart);

            Items.Add(new Separator());

            if (Navigator.IsCreable(typeof(UserChartDN),  true))
            {
                Items.Add(new MenuItem()
                {
                    Header = Signum.Windows.Extensions.Properties.Resources.Create,
                    Icon = ExtensionsImageLoader.GetImageSortName("add.png").ToSmallImage()
                }.Handle(MenuItem.ClickEvent, New_Clicked));
            }

            Items.Add(new MenuItem()
            {
                Header = Signum.Windows.Extensions.Properties.Resources.Edit,
                Icon = ExtensionsImageLoader.GetImageSortName("edit.png").ToSmallImage()
            }.Handle(MenuItem.ClickEvent, Edit_Clicked)
            .Bind(MenuItem.IsEnabledProperty, this, "CurrentUserChart", notNullAndEditable));

            Items.Add(new MenuItem()
            {
                Header = Signum.Windows.Extensions.Properties.Resources.Remove,
                Icon = ExtensionsImageLoader.GetImageSortName("remove.png").ToSmallImage()
            }.Handle(MenuItem.ClickEvent, Remove_Clicked)
            .Bind(MenuItem.IsEnabledProperty, this, "CurrentUserChart", notNullAndEditable));

            if (autoSet!= null)
                SetCurrent(autoSet);
        }

        static IValueConverter notNullAndEditable = ConverterFactory.New((UserChartDN uq) => uq != null && uq.IsAllowedFor(TypeAllowedBasic.Modify));


        private void MenuItem_Clicked(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            if (e.OriginalSource is MenuItem)
            {
                MenuItem b = (MenuItem)e.OriginalSource;
                Lite<UserChartDN> userChart = (Lite<UserChartDN>)b.Tag;

                var uc = userChart.Retrieve();

                SetCurrent(uc);
            }
        }

        private void SetCurrent(UserChartDN uc)
        {
            CurrentUserChart = uc;

            this.ChartRequest = UserChartDN.ToRequest(CurrentUserChart);

            this.ChartWindow.UpdateFiltersOrdersUserInterface();

            this.ChartWindow.GenerateChart();
        }

        private void New_Clicked(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            UserChartDN userChart = UserChartDN.FromRequest(ChartRequest);

            userChart = Navigator.View(userChart, new ViewOptions
            {
                AllowErrors = AllowErrors.No,
                View = new UserChart { QueryDescription = Description }
            });

            if (userChart != null)
            {
                userChart.Save();

                Initialize();

                CurrentUserChart = userChart;
            }
        }

        private void Edit_Clicked(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            Navigator.Navigate(CurrentUserChart, new NavigateOptions
            {
                View = new UserChart { QueryDescription = Description },
                Closed = (s, args) => Initialize()
            });
        }

        private void Remove_Clicked(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            if (MessageBox.Show(Window.GetWindow(this), Prop.Resources.AreYouSureToRemove0.Formato(CurrentUserChart), Prop.Resources.RemoveUserQuery,
                MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                Server.Execute((IChartServer s) => s.RemoveUserChart(CurrentUserChart.ToLite()));

                CurrentUserChart = null;

                Initialize();
            }
        }


        [ThreadStatic]
        static UserChartDN autoSet;
        internal static IDisposable AutoSet(UserChartDN uc)
        {
            var old = autoSet;
            autoSet = uc;
            return new Disposable(() => autoSet = old);
        }
    }
}
