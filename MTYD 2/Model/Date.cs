using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace MTYD.Model
{
    public class Date : INotifyPropertyChanged
    {
        Color Fill;
        Color Outline;

        public event PropertyChangedEventHandler PropertyChanged;
        public string dotw { get; set; }
        public string month { get; set; }
        public string day { get; set; }
        public string fullDateTime { get; set; }
        //public Color fillColor { get; set; }
        public int index { get; set; }
        public Color fillColor
        {
            set
            {
                if (Fill != value)
                {
                    Fill = value;
                    OnPropertyChanged("fillColor");
                }
            }
            get
            {
                return Fill;
            }

        }

        public Color outlineColor
        {
            set
            {
                if (Outline != value)
                {
                    Outline = value;
                    OnPropertyChanged("outlineColor");
                }
            }
            get
            {
                return Outline;
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
