#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using SyncFusionApp.MauiControls.Samples.Base;

namespace SyncFusionApp.MauiControls.Samples.CartesianChart.SfCartesianChart
{
    public partial class RealTimeChart : SampleView
    {
        public RealTimeChart()
        {
            InitializeComponent();
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            realTimeChartViewModel.StopTimer();
            realTimeChartViewModel.StartTimer();
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            if (realTimeChartViewModel != null)
            {
                realTimeChartViewModel.StopTimer();
            }

            realTimeChart.Handler?.DisconnectHandler();
        }
    }
}
