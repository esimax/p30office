using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;

namespace POC.Module.ABMembership.Models
{
    public class FullTextContainsFunction : ICustomFunctionOperatorFormattable
    {
        #region ICustomFunctionOperator Members
        public object Evaluate(params object[] operands)
        {
            throw new NotImplementedException();
        }
        public string Name
        {
            get { return "FullTextContains"; }
        }
        public Type ResultType(params Type[] operands)
        {
            return typeof(bool);
        }
        #endregion
        #region ICustomFunctionOperatorFormattable Members
        public string Format(Type providerType, params string[] operands)
        {
            if (providerType == typeof(MSSqlConnectionProvider))
                return string.Format("contains({0}, {1})", operands[0], operands[1]);
            throw new NotSupportedException(string.Concat("This provider is not supported: ",
                providerType.Name));
        }
        #endregion
    }
}
