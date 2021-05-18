using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MTYD.Model
{
    public class SubHist : INotifyPropertyChanged
    {
        public string Date { get; set; }
        public ObservableCollection<Meals> mealColl { get; set; }
        bool _collVisible;
        public string mealPlanName { get; set; }
        public int mealCollHeight { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public bool CollVisible
        {
            set
            {
                if (_collVisible != value)
                {
                    _collVisible = value;
                    OnPropertyChanged("CollVisible");
                }
            }
            get
            {
                return _collVisible;
            }

        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class Meals
    {
        public string mealName { get; set; }
        public string qty { get; set; }
    }
}
