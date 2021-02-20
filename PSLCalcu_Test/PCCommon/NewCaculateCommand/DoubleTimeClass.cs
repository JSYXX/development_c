using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCCommon.NewCaculateCommand
{
    public class DoubleTimeClass
    {
        public DoubleTimeClass(double sTime, double eTime)
        {
            _startTime = sTime;
            _endTime = eTime;
        }
        private double _startTime;
        private double _endTime;

        public double startTime
        {
            get
            {
                return _startTime;
            }

            set
            {
                _startTime = value;
            }
        }

        public double endTime
        {
            get
            {
                return _endTime;
            }

            set
            {
                _endTime = value;
            }
        }
    }
}
