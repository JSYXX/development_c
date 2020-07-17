using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCCommon; //使用PValue

namespace PSLCalcu
{
    /// <summary>
    /// 时间逻辑算法及表达式解析   
    /// 该程序改编自标准版算术逻辑表达式解析程序Expression，因此程序中又很多多余内容并未删除
    /// ——逻辑表达式的变量序号要从1开始，不能从0开始，否则会报错。
    /// ——时间取非于普通的逻辑非不同，必须制定在哪一段时间内取非。如{1}!{2}，表示在{1}的时间段内，取时间序列{2}的非。
    /// ——逻辑表达式后面可以跟最小时间阈值参数，用分号分隔。如({1}!{2})&{3};2，表示最小时间阈值为2分钟。
    /// 版本：1.0
    ///    
    /// 修改纪录
    ///     
    ///		2017.03.21 版本：1.0 gaofeng 创建。    
    /// <author>
    ///		<name>gaofeng</name>
    ///		<date>2017.03.21</date>
    /// </author> 
    /// </summary>
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
    public class CondExpression : ICondEvaluatable
    {
        string text = "";
        string textInternal = "";
        bool isValid = false;
        
        
        //构析函数
        public CondExpression(string expressionText)
        {           
            this.ExpressionText = expressionText;
        }

        #region 公用属性
        public string ExpressionText
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
                this.textInternal = "(" + value + ")";
                this.textInternal = ModuleTrim();         //去除空格
                this.textInternal = InsertPrecedenceBrackets();
                this.Validate();    //检查一遍，没问题，置isValid为true。在构造函数中，给ExpressionText赋值，会运行此处的set。用validate对公式进行检查。如果公式没错，置isValid参数。
            }
        }
        public bool IsValid
        {
            get
            {
                return this.isValid;
            }
        }
        #endregion

        #region 公用方法

        public List<PValue> Evaluate(List<PValue>[] dvalues)
        {
            if (this.isValid == false)
                return null;
            int temp;
            return this.EvaluateInternal(dvalues, 0, out temp);
        }
        #endregion

        #region 私有方法
        //检查公式有效性
        private void Validate()
        {
            try
            {
                //if expression missing the "(" or ")",we assume it to be invalid  (检查括号)
                if (CheckBrackets())
                {
                    List<PValue>[] evalues;
                    int temp;
                    int count = GetVariableCount(this.textInternal);
                    if (count >= 1)
                        evalues = new List<PValue>[count];
                    else
                    {
                        this.isValid = false;
                        return;
                    }//if
                    for (int i = 0; i < count; i++)
                    {
                        evalues[i] = new List<PValue>();
                        evalues[0].Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 1, DateTimeKind.Local), 0));
                    }//for
                    //if expression does not throw an exception when evaluated at "1", we assume it to be valid
                    this.EvaluateInternal(evalues, 0, out temp);
                    this.isValid = true;
                }
                else
                {
                    this.isValid = false;
                }//if
            }

            catch (FormatException)
            {
                this.isValid = false;
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                this.isValid = false;
            }
            catch (Exception ex)
            {
                this.isValid = false;
            }
        }
        //对表达式进行解析并计算，迭代法
        public List<PValue> EvaluateInternal(List<PValue>[] dvalues, int startIndex, out int endIndex)
        {
            //exceptions are bubbled up
            //for Relational and logical calculate
            //dAnswer is the running total
            List<PValue> dAnswer = null, dOperand = null;
            char chCurrentChar, chOperator = '+';
           
            int dvariable;
            int count = GetVariableCount(this.textInternal);
            if (count > dvalues.Length)
            {
                endIndex = 0;
                return null;
            }

            for (int i = startIndex + 1; i < this.textInternal.Length; i++)
            {
                startIndex = i;
                chCurrentChar = this.textInternal[startIndex];
                
                //calculate the &(and),|(or), !(not)
                //发现逻辑运算，先在逻辑运算符右侧进行迭代，然后用返回值
                if (IsLogicalOperator(chCurrentChar))
                {
                    dAnswer = dOperand;
                    //dOperand = EvaluateInternal(dvalues, i + 1, out endIndex);
                    dOperand = this.EvaluateInternal(dvalues, i , out endIndex); //发现逻辑运算，把左边的值存入dAnswer，先算右边的值放入dOperand
                    i = endIndex;
                    dAnswer = DoLogicalOperation(dAnswer, dOperand, chCurrentChar); //把左边和右边的值进行运算
                    endIndex = i ;
                    return dAnswer;
                }    
                
                //if found variable list
                //如果发现左括号{，则
                //——1、找到右}.，将i移动到}之后的字符
                //——2、取中间的参数序号
                //——3、根据参数序号取参数
                else if (chCurrentChar == '{')
                {
                    while (this.textInternal[i] != '}')
                        i++;
                    dvariable = Convert.ToInt16(this.textInternal.Substring(startIndex + 1, i - startIndex - 1));
                    dOperand = dvalues[dvariable - 1];
                }
                
                    //if found a bracket, solve it first
                //发现左括号，进入迭代，将左括号内部的进行统一计算
                else if (chCurrentChar == '(')
                {
                    dOperand = this.EvaluateInternal(dvalues, i, out endIndex);
                    i = endIndex;
                }
                //if found closing bracket, return result
                //发现右括号，是当次迭代结束，将)内部的结果汇总，并返回上一次迭代。
                //特别注意，迭代内部的右括号)，对应本次迭代外部的左括号(
                //因此，第一次Evaluate调用EvaluateInternal，直接从i+1开始。默认认为Evaluate已经看到最左侧的（。
                //而，第一次调用EvaluateInternal，到字符串最后一个),是第一次调用EvaluateInternal的结束，对应者左侧的）
                //因此，循环是从i+1开始，到this.textInternal.Length结束
                else if (chCurrentChar == ')')
                {
                    dAnswer = DoOperation(dAnswer, dOperand, chOperator);
                    endIndex = i;
                    return dAnswer;
                }//if

                
                if (dAnswer==null || dOperand==null)
                {
                //    endIndex = i;
                //    return null;
                }//if
            }//for
            endIndex = this.textInternal.Length;
            return null;
        }
        
        
        //整理公式表达式添加括号       
        string InsertPrecedenceBrackets()
        {
            int i = 0, j = 0;
            int iBrackets = 0;
            bool bReplace = false;
            int iLengthExpression;
            string strExpression = this.textInternal;

            //Precedence for * && /
            i = 1;
            iLengthExpression = strExpression.Length;
            while (i <= iLengthExpression)
            {
                //时间的非逻辑与数字的非逻辑不一样。时间的非逻辑 span1！span2，表示在span1范围内挖掉span2。因此不能像数字逻辑一样，在！前后加括号。比如a&!c =》 a&(!c)
                //基于上述原因，将给！加括号的程序注销
                /*
                if (strExpression.Substring(-1 + i, 1) == "!")
                {
                    //前面加(，向后面检查()，然后加)。
                    strExpression = strExpression.Substring(-1 + 1, -1 + i) + "(" + strExpression.Substring(-1 + i);
                    iLengthExpression = strExpression.Length;
                    i++;
                    bReplace = true;
                    j = i;
                    i++;
                    while (bReplace == true)
                    {
                        j = j + 1;
                        if (strExpression.Substring(-1 + j, 1) == "(")
                            iBrackets = iBrackets + 1;
                        if (strExpression.Substring(-1 + j, 1) == ")")
                        {
                            if (iBrackets == 0)
                            {
                                strExpression = strExpression.Substring(-1 + 1, j - 1) + ")" + strExpression.Substring(-1 + j);
                                bReplace = false;
                                i = i + 1;
                                break;
                            }//if
                            else
                                iBrackets = iBrackets - 1;
                        }
                        if (strExpression.Substring(-1 + j, 1) == "+" || strExpression.Substring(-1 + j, 1) == "-"
                            || strExpression.Substring(-1 + j, 1) == "*" || strExpression.Substring(-1 + j, 1) == "/"
                            || strExpression.Substring(-1 + j, 1) == "%" || strExpression.Substring(-1 + j, 1) == "^"
                            || strExpression.Substring(-1 + j, 1) == "&" || strExpression.Substring(-1 + j, 1) == "|"
                            || strExpression.Substring(-1 + j, 1) == "<" || strExpression.Substring(-1 + j, 1) == ">"
                            || strExpression.Substring(-1 + j, 1) == "=")
                        {
                            if (iBrackets == 0)
                            {
                                strExpression = strExpression.Substring(-1 + 1, j - 1) + ")" + strExpression.Substring(-1 + j);
                                bReplace = false;
                                i = i + 1;
                                break;
                            }//if
                        }//if
                    }
                }
                 */
                if (strExpression.Substring(-1 + i, 1) == "&" || strExpression.Substring(-1 + i, 1) == "|")
                {
                    //前面加)，向前面检查&和|，然后加(。
                    for (j = i - 1; j > 0; j--)
                    {
                    }
                    //后面加(，向后面检查()，然后加)。
                }
                if (strExpression.Substring(-1 + i, 1) == ">" || strExpression.Substring(-1 + i, 1) == "<" || strExpression.Substring(-1 + i, 1) == "=")
                {
                    //先检查是否为双字符号
                    for (j = i - 1; j > 0; j--)
                    {
                        //前面加)，向前面检查()，然后加(。
                        //后面加(，向后面检查()，然后加)。
                    }
                }
                if (strExpression.Substring(-1 + i, 1) == "*" || strExpression.Substring(-1 + i, 1) == "/")
                {
                    for (j = i - 1; j > 0; j--)
                    {
                        if (strExpression.Substring(-1 + j, 1) == ")")
                            iBrackets = iBrackets + 1;
                        if (strExpression.Substring(-1 + j, 1) == "(")
                            iBrackets = iBrackets - 1;
                        if (iBrackets < 0)
                            break;
                        if (iBrackets == 0)
                        {
                            if (strExpression.Substring(-1 + j, 1) == "+" || strExpression.Substring(-1 + j, 1) == "-")
                            {
                                strExpression = strExpression.Substring(-1 + 1, j) + "(" + strExpression.Substring(-1 + j + 1);
                                bReplace = true;
                                i = i + 1;
                                break;
                            }//if
                        }//if
                    }//for
                    iBrackets = 0;
                    j = i;
                    i = i + 1;
                    while (bReplace == true)
                    {
                        j = j + 1;
                        if (strExpression.Substring(-1 + j, 1) == "(")
                            iBrackets = iBrackets + 1;
                        if (strExpression.Substring(-1 + j, 1) == ")")
                        {
                            if (iBrackets == 0)
                            {
                                strExpression = strExpression.Substring(-1 + 1, j - 1) + ")" + strExpression.Substring(-1 + j);
                                bReplace = false;
                                i = i + 1;
                                break;
                            }//if
                            else
                                iBrackets = iBrackets - 1;
                        }//if
                        if (strExpression.Substring(-1 + j, 1) == "+" || strExpression.Substring(-1 + j, 1) == "-")
                        {
                            if (iBrackets == 0)
                            {
                                strExpression = strExpression.Substring(-1 + 1, j - 1) + ")" + strExpression.Substring(-1 + j);
                                bReplace = false;
                                i = i + 1;
                                break;
                            }//if
                        }//if
                    }//while
                }//if

                iLengthExpression = strExpression.Length;
                i = i + 1;
            }//while


            //Precedence for ^ && % 
            i = 1;
            iLengthExpression = strExpression.Length;
            while (i <= iLengthExpression)
            {
                if (strExpression.Substring(-1 + i, 1) == "^" || strExpression.Substring(-1 + i, 1) == "%")
                {
                    for (j = i - 1; j > 0; j--)
                    {
                        if (strExpression.Substring(-1 + j, 1) == ")")
                            iBrackets = iBrackets + 1;
                        if (strExpression.Substring(-1 + j, 1) == "(")
                            iBrackets = iBrackets - 1;
                        if (iBrackets < 0)
                            break;
                        if (iBrackets == 0)
                        {
                            if (strExpression.Substring(-1 + j, 1) == "+"
                                || strExpression.Substring(-1 + j, 1) == "-"
                                || strExpression.Substring(-1 + j, 1) == "*"
                                || strExpression.Substring(-1 + j, 1) == "/")
                            {
                                strExpression = strExpression.Substring(-1 + 1, j) + "(" + strExpression.Substring(-1 + j + 1);
                                bReplace = true;
                                i = i + 1;
                                break;
                            }//if
                        }//if
                    }//for
                    iBrackets = 0;
                    j = i;
                    i = i + 1;
                    while (bReplace == true)
                    {
                        j = j + 1;
                        if (strExpression.Substring(-1 + j, 1) == "(")
                            iBrackets = iBrackets + 1;
                        if (strExpression.Substring(-1 + j, 1) == ")")
                        {
                            if (iBrackets == 0)
                            {
                                strExpression = strExpression.Substring(-1 + 1, j - 1) + ")" + strExpression.Substring(-1 + j);
                                bReplace = false;
                                i = i + 1;
                                break;
                            }//if
                            else
                                iBrackets = iBrackets - 1;
                        }//if
                        if (strExpression.Substring(-1 + j, 1) == "+" || strExpression.Substring(-1 + j, 1) == "-"
                            || strExpression.Substring(-1 + j, 1) == "*" || strExpression.Substring(-1 + j, 1) == "/")
                        {
                            strExpression = strExpression.Substring(-1 + 1, j - 1) + ")" + strExpression.Substring(-1 + j);
                            bReplace = false;
                            i = i + 1;
                            break;
                        }//if
                    }//while
                }//if
                iLengthExpression = strExpression.Length;
                i = i + 1;
            }//while
            return strExpression;
        }
        //检查括号
        private bool CheckBrackets()
        {
            int i;
            int iBrackets = 0;
            int iLengthExpression;
            string strExpression = this.textInternal;

            i = 1;
            iLengthExpression = strExpression.Length;
            while (i <= iLengthExpression)
            {
                if (strExpression.Substring(-1 + i, 1) == "(")
                    iBrackets = iBrackets + 1;
                if (strExpression.Substring(-1 + i, 1) == ")")
                    iBrackets = iBrackets - 1;
                i++;
            }
            if (iBrackets == 0) return true;
            else return false;
        }
        //去掉所有空格
        private string ModuleTrim()
        {
            int i = 0;
            int iLengthExpression;
            string strExpression = this.textInternal.Trim();

            i = 1;
            iLengthExpression = strExpression.Length;
            while (i <= iLengthExpression)
            {
                //delete white-space  /
                if ((strExpression[-1 + i] == ' '))
                {
                    strExpression = strExpression.Substring(-1 + 1, -1 + i) + strExpression.Substring(-1 + i + 1);
                }//if
                //add *, example: from "3x" to "3*x"
                if ((strExpression[-1 + i] == '{') && (i > 1))
                {
                    if (char.IsDigit(strExpression[-1 + i - 1]))
                    {
                        strExpression = strExpression.Substring(-1 + 1, -1 + i) + "*" + strExpression.Substring(-1 + i);
                        i++;
                    }//if
                }//if
                //add *, example: from "3(x+1)" to "3*(x+1)"
                if ((strExpression[-1 + i] == '(') && (i > 1))
                {
                    if (char.IsDigit(strExpression[-1 + i - 1]))
                    {
                        strExpression = strExpression.Substring(-1 + 1, -1 + i) + "*" + strExpression.Substring(-1 + i);
                        i++;
                    }//if
                }//if
                iLengthExpression = strExpression.Length;
                i++;
            }//while
            return strExpression;
        }
        //获得变量的数量
        static int GetVariableCount(string sourcestr)
        {
            //获取参数个数的方法：找{}之间的最大数字
            //由此方法，也可看出，
            string substr1 = "{";
            string substr2 = "}";
            int count = 0;
            int index;
            string tmpstr;
            int i, j;
            for (i = 0; i < sourcestr.Length; i++)                      //表达式字符从头便利到尾
            {
                if ((i + substr1.Length) < sourcestr.Length)            //如果没有到末尾
                {
                    tmpstr = sourcestr.Substring(i, substr1.Length);    //取出当前字符
                    if (tmpstr == substr1)                              //当前字符为{
                    {
                        j = i;                                          //记住当前的位置
                        while (sourcestr.Substring(j, substr2.Length) != substr2)   //从当前位置找到另一个}为止
                            j++;
                        index = Convert.ToInt16(sourcestr.Substring(i + 1, j - i - 1));//取出{}之间的数字，比如1，比如32，比如9
                        if (index > count)      //如果这个数字大于count，则用index替代count
                            count = index;
                    }
                }
            }
            return count;
        }


        //是否为逻辑运算符
        static bool IsLogicalOperator(char character)
        {
            if (character == '&' || character == '|' || character == '!')
                return true;
            return false;
        }       
       //进行两个数据的算数运算
        static List<PValue> DoOperation(List<PValue> dOperand1, List<PValue> dOperand2, char chOperator)
        {
            switch (chOperator)
            {
                case '+':
                   return dOperand2;               
            }
            return null;
        }
        //进行两个数据的逻辑运算
        static List<PValue> DoLogicalOperation(List<PValue> dOperand1, List<PValue> dOperand2, char chROperator)
        {
            List<PValue> RAnswer = null;            
            switch (chROperator)
            {                
                case '|':
                    RAnswer = SpanLogic.SpansOr(dOperand1, dOperand2);
                    break;
                case '!':
                    RAnswer = SpanLogic.SpansNot(dOperand1, dOperand2);
                    break;
                case '&':                    
                default:
                    RAnswer = SpanLogic.SpansAnd(dOperand1, dOperand2);
                    break;
            }
            return RAnswer;
        }
        
        #endregion

        
	    
    }
}
