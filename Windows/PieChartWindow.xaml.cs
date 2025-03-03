using LiveCharts;
using LiveCharts.Wpf;
using MSProj_Analog.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MSProj_Analog.Windows
{
    public partial class PieChartWindow : Window
    {
        public SeriesCollection SeriesCollection { get; set; }

        public PieChartWindow(ICollection<ProjectTask> tasks)
        {
            InitializeComponent();
            LoadPieChart(tasks);
            DataContext = this;
        }

        private void LoadPieChart(ICollection<ProjectTask> tasks)
        {
            SeriesCollection = new SeriesCollection();

            foreach (var task in tasks)
            {
                double duration = (task.EndDate - task.StartDate).TotalDays;

                SeriesCollection.Add(new PieSeries
                {
                    Title = task.Name,
                    Values = new ChartValues<double> { duration },
                    DataLabels = true
                });
            }

            PieChart.Series = SeriesCollection;
        }
    }
}
