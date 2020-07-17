using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCCommon; //使用PValue

namespace PSLCalcu
{
    public interface ICondEvaluatable
    {
        /// <summary>
        /// Should return text for the expression
        /// </summary>
        string ExpressionText
        {
            get;
            set;
        }

        /// <summary>
        /// Should return true if the expression is valid and can be evaluated
        /// </summary>
        bool IsValid
        {
            get;
        }

        ///<summary>This method should evaluate the expression and return the result as double. It should return double.NaN in the case the expression cant be evaluated e.g. log( -ve no. )</summary>
        ///<param name="dvalueX">The value of X at which we want to evaluate the expression</param>
        ///<returns>The result of expression evaluation as a double</returns>
        List<PValue> Evaluate(List<PValue>[] dvalues);
    }
}
