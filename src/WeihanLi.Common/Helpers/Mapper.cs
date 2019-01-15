namespace WeihanLi.Common.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using static System.Linq.Expressions.Expression;

    /// <summary>
    /// Mapper
    /// https://github.com/bieyuan/.net-core-DTO.git
    /// </summary>
    /// <typeparam name="TSource">TSource</typeparam>
    /// <typeparam name="TTarget">TTarget</typeparam>
    public static class Mapper<TSource, TTarget> where TSource : class where TTarget : class
    {
        private static Func<TSource, TTarget> MapFunc { get; set; }

        private static Action<TSource, TTarget> MapAction { get; set; }

        /// <summary>
        /// map TSource to TTarget
        /// </summary>
        public static TTarget Map(TSource source)
        {
            if (MapFunc == null)
                MapFunc = GetMapFunc();
            return MapFunc(source);
        }

        public static List<TTarget> MapList(IEnumerable<TSource> sources)
        {
            if (MapFunc == null)
                MapFunc = GetMapFunc();
            var result = new List<TTarget>();
            foreach (var item in sources)
            {
                result.Add(MapFunc(item));
            }
            return result;
        }

        /// <summary>
        /// 将对象TSource的值赋给给TTarget
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void Map(TSource source, TTarget target)
        {
            if (MapAction == null)
                MapAction = GetMapAction();
            MapAction(source, target);
        }

        private static Func<TSource, TTarget> GetMapFunc()
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);
            //Func委托传入变量
            var parameter = Parameter(sourceType, "p");

            var memberBindings = new List<MemberBinding>();
            var targetTypes = CacheUtil.TypePropertyCache.GetOrAdd(targetType, t => t.GetProperties()).Where(x => x.PropertyType.IsPublic && x.CanWrite);
            foreach (var targetItem in targetTypes)
            {
                var sourceItem = sourceType.GetProperty(targetItem.Name);

                //判断实体的读写权限
                if (sourceItem == null || !sourceItem.CanRead || sourceItem.PropertyType.IsNotPublic)
                    continue;

                //标注NotMapped特性的属性忽略转换
                if (sourceItem.GetCustomAttribute<NotMappedAttribute>() != null)
                    continue;

                var sourceProperty = Property(parameter, sourceItem);

                //当非值类型且类型不相同时
                if (!sourceItem.PropertyType.IsValueType && sourceItem.PropertyType != targetItem.PropertyType)
                {
                    //判断都是(非泛型)class
                    if (sourceItem.PropertyType.IsClass && targetItem.PropertyType.IsClass &&
                        !sourceItem.PropertyType.IsGenericType && !targetItem.PropertyType.IsGenericType)
                    {
                        var expression = GetClassExpression(sourceProperty, sourceItem.PropertyType, targetItem.PropertyType);
                        memberBindings.Add(Bind(targetItem, expression));
                    }

                    //集合数组类型的转换
                    if (typeof(IEnumerable).IsAssignableFrom(sourceItem.PropertyType) && typeof(IEnumerable).IsAssignableFrom(targetItem.PropertyType))
                    {
                        var expression = GetListExpression(sourceProperty, sourceItem.PropertyType, targetItem.PropertyType);
                        memberBindings.Add(Bind(targetItem, expression));
                    }

                    continue;
                }

                //可空类型转换到非可空类型，当可空类型值为null时，用默认值赋给目标属性；不为null就直接转换
                if (IsNullableType(sourceItem.PropertyType) && !IsNullableType(targetItem.PropertyType))
                {
                    var hasValueExpression = Equal(Property(sourceProperty, "HasValue"), Constant(true));
                    var conditionItem = Condition(hasValueExpression, Convert(sourceProperty, targetItem.PropertyType), Default(targetItem.PropertyType));
                    memberBindings.Add(Bind(targetItem, conditionItem));
                    continue;
                }

                //非可空类型转换到可空类型，直接转换
                if (!IsNullableType(sourceItem.PropertyType) && IsNullableType(targetItem.PropertyType))
                {
                    var memberExpression = Convert(sourceProperty, targetItem.PropertyType);
                    memberBindings.Add(Bind(targetItem, memberExpression));
                    continue;
                }

                if (targetItem.PropertyType != sourceItem.PropertyType)
                    continue;

                memberBindings.Add(Bind(targetItem, sourceProperty));
            }

            //创建一个if条件表达式
            var test = NotEqual(parameter, Constant(null, sourceType));// p==null;
            var ifTrue = MemberInit(New(targetType), memberBindings);
            var condition = Condition(test, ifTrue, Constant(null, targetType));

            var lambda = Lambda<Func<TSource, TTarget>>(condition, parameter);
            return lambda.Compile();
        }

        /// <summary>
        /// 类型是clas时赋值
        /// </summary>
        /// <param name="sourceProperty"></param>
        /// <param name="sourceType"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        private static Expression GetClassExpression(Expression sourceProperty, Type sourceType, Type targetType)
        {
            //条件p.Item!=null
            var testItem = NotEqual(sourceProperty, Constant(null, sourceType));

            //构造回调 Mapper<TSource, TTarget>.Map()
            var mapperType = typeof(Mapper<,>).MakeGenericType(sourceType, targetType);
            var iftrue = Call(mapperType.GetMethod(nameof(Map), new[] { sourceType }), sourceProperty);

            var conditionItem = Condition(testItem, iftrue, Constant(null, targetType));

            return conditionItem;
        }

        /// <summary>
        /// 类型为集合时赋值
        /// </summary>
        /// <param name="sourceProperty"></param>
        /// <param name="sourceType"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        private static Expression GetListExpression(Expression sourceProperty, Type sourceType, Type targetType)
        {
            //条件p.Item!=null
            var testItem = NotEqual(sourceProperty, Constant(null, sourceType));

            //构造回调 Mapper<TSource, TTarget>.MapList()
            var sourceArg = sourceType.IsArray ? sourceType.GetElementType() : sourceType.GetGenericArguments()[0];
            var targetArg = targetType.IsArray ? targetType.GetElementType() : targetType.GetGenericArguments()[0];
            var mapperType = typeof(Mapper<,>).MakeGenericType(sourceArg, targetArg);

            var mapperExecMap = Call(mapperType.GetMethod(nameof(MapList), new[] { sourceType }), sourceProperty);

            Expression iftrue;
            if (targetType == mapperExecMap.Type)
            {
                iftrue = mapperExecMap;
            }
            else if (targetType.IsArray)//数组类型调用ToArray()方法
            {
                iftrue = Call(mapperExecMap, mapperExecMap.Type.GetMethod("ToArray"));
            }
            else if (typeof(IDictionary).IsAssignableFrom(targetType))
            {
                iftrue = Constant(null, targetType);//字典类型不转换
            }
            else
            {
                iftrue = Convert(mapperExecMap, targetType);
            }

            var conditionItem = Condition(testItem, iftrue, Constant(null, targetType));

            return conditionItem;
        }

        private static Action<TSource, TTarget> GetMapAction()
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);
            //Func委托传入变量
            var sourceParameter = Parameter(sourceType, "p");

            var targetParameter = Parameter(targetType, "t");

            //创建一个表达式集合
            var expressions = new List<Expression>();

            var targetTypes = CacheUtil.TypePropertyCache.GetOrAdd(targetType, t => t.GetProperties()).Where(x => x.PropertyType.IsPublic && x.CanWrite);
            foreach (var targetItem in targetTypes)
            {
                var sourceItem = sourceType.GetProperty(targetItem.Name);

                //判断实体的读写权限
                if (sourceItem == null || !sourceItem.CanRead || sourceItem.PropertyType.IsNotPublic)
                    continue;

                //标注NotMapped特性的属性忽略转换
                if (sourceItem.GetCustomAttribute<NotMappedAttribute>() != null)
                    continue;

                var sourceProperty = Property(sourceParameter, sourceItem);
                var targetProperty = Property(targetParameter, targetItem);

                //当非值类型且类型不相同时
                if (!sourceItem.PropertyType.IsValueType && sourceItem.PropertyType != targetItem.PropertyType)
                {
                    //判断都是(非泛型)class
                    if (sourceItem.PropertyType.IsClass && targetItem.PropertyType.IsClass &&
                        !sourceItem.PropertyType.IsGenericType && !targetItem.PropertyType.IsGenericType)
                    {
                        var expression = GetClassExpression(sourceProperty, sourceItem.PropertyType, targetItem.PropertyType);
                        expressions.Add(Assign(targetProperty, expression));
                    }

                    //集合数组类型的转换
                    if (typeof(IEnumerable).IsAssignableFrom(sourceItem.PropertyType) && typeof(IEnumerable).IsAssignableFrom(targetItem.PropertyType))
                    {
                        var expression = GetListExpression(sourceProperty, sourceItem.PropertyType, targetItem.PropertyType);
                        expressions.Add(Assign(targetProperty, expression));
                    }

                    continue;
                }

                if (targetItem.PropertyType != sourceItem.PropertyType)
                    continue;

                expressions.Add(Assign(targetProperty, sourceProperty));
            }

            //当Target!=null判断source是否为空
            var testSource = NotEqual(sourceParameter, Constant(null, sourceType));
            var ifTrueSource = Block(expressions);
            var conditionSource = IfThen(testSource, ifTrueSource);

            //判断target是否为空
            var testTarget = NotEqual(targetParameter, Constant(null, targetType));
            var conditionTarget = IfThen(testTarget, conditionSource);

            var lambda = Lambda<Action<TSource, TTarget>>(conditionTarget, sourceParameter, targetParameter);
            return lambda.Compile();
        }

        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
