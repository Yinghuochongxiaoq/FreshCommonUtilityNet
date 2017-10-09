#region	Vesion Info
//======================================================================
//Copyright(C) FreshMan.All right reserved.
//命名空间：FreshCommonUtilityNetTest
//文件名称：ExpressionLear
//创 建 人：FreshMan
//创建日期：2017/6/30 23:30:03
//用    途：记录类的用途
//======================================================================
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FreshCommonUtilityNetTest
{
    public class ExpressionLear
    {
        /// <summary>
        /// entry code
        /// </summary>
        public static void LearnInfo()
        {
            //InitExpression();
            //ModifyExpression();
            Student student = new Student
            {
                Id = 1,
                Name = "zhang san",
                Location = new Location
                {
                    Row = 10,
                    Col = 20
                }
            };
            VisitProperties<Student>(student);
            var s = TransExp<Student, Student>(student);
        }

        /// <summary>
        /// Demo
        /// </summary>
        private static void InitExpression()
        {
            //Expression<Func<int, int, int>> expression = (a, b) => a * b + 2;
            //var function = expression.Compile();
            //System.Console.WriteLine(function(4, 5));

            ParameterExpression paraLeft = Expression.Parameter(typeof(int), "a");
            ParameterExpression paraRight = Expression.Parameter(typeof(int), "b");

            BinaryExpression binaryLeft = Expression.Multiply(paraLeft, paraRight);
            ConstantExpression conRight = Expression.Constant(2, typeof(int));

            BinaryExpression binaryBody = Expression.Add(binaryLeft, conRight);

            Expression<Func<int, int, int>> lambda = Expression.Lambda<Func<int, int, int>>(binaryBody, paraLeft,
                paraRight);

            Console.WriteLine(lambda.ToString());
            Func<int, int, int> function = lambda.Compile();

            int result = function(2, 3);
            Console.WriteLine("result:" + result);

            BinaryExpression body = Expression.Add(Expression.Constant(2), Expression.Constant(3));

            Expression<Func<int>> expression = Expression.Lambda<Func<int>>(body, null);
            Func<int> lambdaTwo = expression.Compile();
            Console.WriteLine(lambdaTwo());
        }

        /// <summary>
        /// Modify expression
        /// </summary>
        private static void ModifyExpression()
        {
            Expression<Func<int, int, int>> lambda = (a, b) => a + b * 2;

            var operationVisistor = new OperationVisitor();
            Expression modifyExpression = operationVisistor.Modify(lambda);
            Console.WriteLine(modifyExpression.ToString());
        }

        private static void VisitProperties<T>(object obj)
        {
            var type = obj.GetType();
            var paraExpression = Expression.Parameter(typeof(T), "paramsObject");
            foreach (var propertyInfo in type.GetProperties())
            {
                var propType = propertyInfo.PropertyType;
                if (propType.IsPrimitive || propType == typeof(string))
                {
                    VisitProperty<T>(obj, propertyInfo, paraExpression, paraExpression);
                }
                else
                {
                    Console.WriteLine("not primitive property: " + propertyInfo.Name);
                    var otherType = propType;
                    MemberExpression memberExpression = Expression.Property(paraExpression, propertyInfo);
                    foreach (var otherProp in otherType.GetProperties())
                    {
                        VisitProperty<T>(obj, otherProp, memberExpression, paraExpression);
                    }
                }
            }
        }

        private static void VisitProperty<T>(object obj, PropertyInfo prop, Expression instanceExpression,
            ParameterExpression parameterExpression)
        {
            Console.WriteLine("property name: " + prop.Name);
            MemberExpression memExpression = Expression.Property(instanceExpression, prop);
            Expression objectExpression = Expression.Convert(memExpression, typeof(object));
            Expression<Func<T, object>> lambdaExpression = Expression.Lambda<Func<T, object>>(objectExpression,
                parameterExpression);
            Console.WriteLine("expression tree: " + lambdaExpression);
            Func<T, object> func = lambdaExpression.Compile();
            Console.WriteLine("value:" + func((T)obj));
        }

        private static Dictionary<string, object> _dic = new Dictionary<string, object>();

        private static TOut TransExp<TIn, TOut>(TIn tIn)
        {
            string key = string.Format("trans_exp_{0}{1}", typeof(TIn).FullName, typeof(TOut).FullName);
            if (!_dic.ContainsKey(key))
            {
                ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "P");
                List<MemberBinding> memberBindingList = new List<MemberBinding>();
                foreach (var item in typeof(TOut).GetProperties())
                {
                    if (!item.CanWrite)
                    {
                        continue;
                    }
                    MemberExpression property = Expression.Property(parameterExpression,
                        typeof(TIn).GetProperty(item.Name));
                    MemberBinding memberBinding = Expression.Bind(item, property);
                    memberBindingList.Add(memberBinding);
                }
                MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)),
                    memberBindingList);
                Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression,
                    new ParameterExpression[] { parameterExpression });
                Func<TIn, TOut> func = lambda.Compile();
                _dic[key] = func;
            }
            return ((Func<TIn, TOut>)_dic[key])(tIn);
        }
    }

    public class OperationVisitor : ExpressionVisitor
    {
        public Expression Modify(Expression expression)
        {
            return Visit(expression);
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            if (b.NodeType == ExpressionType.Add)
            {
                Expression left = Visit(b.Left);
                Expression right = Visit(b.Right);
                return Expression.Subtract(left, right);
            }

            return base.VisitBinary(b);
        }
    }

    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Location Location { get; set; }
    }


    public class Location
    {
        public int Row { get; set; }
        public int Col { get; set; }
    }
}
