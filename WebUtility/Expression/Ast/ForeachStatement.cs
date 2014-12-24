using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Reflection;
using System.IO;

namespace Cvv.WebUtility.Expression
{
    class ForeachStatement: StatementNode
    {
        private string _varName;
        private ExpressionNode _collection;
        private BlockStatement _blockStatement;

        public ForeachStatement(Token relatedToken)
            : base(relatedToken)
        {
            _blockStatement = new BlockStatement(relatedToken);
        }

        public string VarName
        {
            get { return _varName; }
            internal set { _varName = value; }
        }
        
        public ExpressionNode Collection
        {
            get { return _collection; }
            set { _collection = value; }
        }

        public BlockStatement BlockStatement
        {
            get { return _blockStatement; }
        }

        public override object Execute(IContext context, Stream writer)
        {

            object obj = _collection.Execute(context, writer);

            object varData = null;

            if (context.ContainsKey(_varName))
            {
                varData = context[_varName];
            }

            if (obj is IEnumerable)
            {
                IEnumerable em = (IEnumerable)obj;
                int index = 0;
                bool isOdd = true;
                string indexName = _varName + "_index";
                string oddName = _varName + "_odd";
                object indexData;
                object oddData;

                context.TryGetValue(indexName, out indexData);
                context.TryGetValue(oddName, out oddData);

                foreach (object o in em)
                {
                    try
                    {
                        context[_varName] = o;
                        context[indexName] = index++;
                        context[oddName] = isOdd;

                        _blockStatement.Execute(context, writer);

                        isOdd = !isOdd;
                        context.Remove(_varName);
                        context.Remove(indexName);
                        context.Remove(oddName);
                    }
                    catch (BreakException)
                    {
                        break;
                    }
                    catch (ContinueException)
                    {
                        continue;
                    }
                }

                if (indexData != null)
                {
                    context[indexName] = indexData;
                }

                if (oddData != null)
                {
                    context[oddName] = oddData;
                }
            }
            else
            {
                if (obj != null)
                {
                    PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    foreach (PropertyInfo info in properties)
                    {
                        try
                        {
                            context[_varName] = info.GetValue(obj, null);
                            _blockStatement.Execute(context, writer);
                            context.Remove(_varName);
                        }
                        catch (BreakException)
                        {
                            break;
                        }
                        catch (ContinueException)
                        {
                            continue;
                        }
                    }
                }
            }

            if (varData != null)
            {
                context[_varName] = varData;
            }

            return null;
        }
    }
}
