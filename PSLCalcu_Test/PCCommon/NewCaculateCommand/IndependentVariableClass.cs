using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCCommon.NewCaculateCommand
{
    public class IndependentVariableClass
    {
        private int _id;
        private List<double> _valueList;
        private double _y;

        public int id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        public List<double> valueList
        {
            get
            {
                return _valueList;
            }

            set
            {
                _valueList = value;
            }
        }

        public double y
        {
            get
            {
                return _y;
            }

            set
            {
                _y = value;
            }
        }
    }
}
