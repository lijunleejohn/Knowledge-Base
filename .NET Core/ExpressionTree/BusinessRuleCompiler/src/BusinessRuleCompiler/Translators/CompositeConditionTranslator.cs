using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BusinessRuleCompiler.Translators
{
    internal partial class ConditionTranslator
    {
        // CompositeConditionTranslator deals with CompositeCondition, relationship type of BinaryExpress, like (a>3.5) OR (b<8.7), (a> AND b etc.
        private class CompositeConditionTranslator
        {
            private readonly ConditionTranslator _parent;

            internal CompositeConditionTranslator(ConditionTranslator translator)
            {
                _parent = translator;
            }

            internal Expression Translate(CompositeCondition condition)
            {
                List<Exception> exceptions = new List<Exception>();
                Expression[] translated = condition.Conditions.Select(child => Translate(child)).ToArray();

                if (exceptions.Any())
                {
                    throw new InvalidOperationException("Rule compilation exception", new AggregateException(exceptions));
                }

                // here left and right are all Expression type, meaning like: (Symbol EQ "LME-CO") OR (Symbol EQ "LME-GO")
                return translated.Aggregate((left, right) => Expression.MakeBinary(condition.Operator, left, right));

                Expression Translate(Condition condition)
                {
                    try
                    {
                        return _parent.Translate(condition);
                    }
                    catch (AggregateException ex)
                    {
                        foreach (Exception innerEx in ex.InnerExceptions)
                        {
                            exceptions.Add(innerEx);
                        }

                        return Expression.Empty();
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);

                        return Expression.Empty();
                    }
                }
            }
        }
    }
}
