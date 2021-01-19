using System;
using System.ComponentModel;

namespace MTYD.Model
{
    public class MealInfo: INotifyPropertyChanged
    {
        int _mealQuantity;
        long _mealPrice;
        string _mealName, _mealCalories, _mealImage;
        public event PropertyChangedEventHandler PropertyChanged;

        public string ItemUid { get; set; }

        public int MealQuantity
        {
            set
            {
                if (_mealQuantity != value)
                {
                    _mealQuantity = value;
                    OnPropertyChanged("MealQuantity");
                }
            }
            get
            {
                return _mealQuantity;
            }

        }

        public long MealPrice
        {
            set
            {
                if (_mealPrice != value)
                {
                    _mealPrice = value;
                    OnPropertyChanged("MealPrice");
                }
            }
            get
            {
                return _mealPrice;
            }

        }

        public string MealName
        {
            set
            {
                if (_mealName != value)
                {
                    _mealName = value;
                    OnPropertyChanged("MealName");
                }
            }
            get
            {
                return _mealName;
            }

        }

        public string MealCalories
        {
            set
            {
                if (_mealCalories != value)
                {
                    _mealCalories = value;
                    OnPropertyChanged("MealCalories");
                }
            }
            get
            {
                return _mealCalories;
            }

        }

        public string MealImage
        {
            set
            {
                if (_mealImage != value)
                {
                    _mealImage = value;
                    OnPropertyChanged("MealImage");
                }
            }
            get
            {
                return _mealImage;
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
}
