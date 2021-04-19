using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace MTYD.Model
{
    public class MealInfo: INotifyPropertyChanged
    {
        int _mealQuantity;
        long _mealPrice;
        string _mealName, _mealCalories, _mealImage, _heartSource;
        Color _background;
        bool _seeDesc, _seeImage;
        public event PropertyChangedEventHandler PropertyChanged;

        public string ItemUid { get; set; }
        public string MealDesc { get; set; }

        public string HeartSource
        {
            set
            {
                if (_heartSource != value)
                {
                    _heartSource = value;
                    OnPropertyChanged("HeartSource");
                }
            }
            get
            {
                return _heartSource;
            }

        }

        public Color Background
        {
            set
            {
                if (_background != value)
                {
                    _background = value;
                    OnPropertyChanged("Background");
                }
            }
            get
            {
                return _background;
            }

        }

        public bool SeeDesc
        {
            set
            {
                if (_seeDesc != value)
                {
                    _seeDesc = value;
                    OnPropertyChanged("SeeDesc");
                }
            }
            get
            {
                return _seeDesc;
            }

        }

        public bool SeeImage
        {
            set
            {
                if (_seeImage != value)
                {
                    _seeImage = value;
                    OnPropertyChanged("SeeImage");
                }
            }
            get
            {
                return _seeImage;
            }

        }

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
