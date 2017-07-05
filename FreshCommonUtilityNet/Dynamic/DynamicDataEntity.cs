using System;
using System.Collections.Generic;
using System.Dynamic;

// ReSharper disable once CheckNamespace
namespace FreshCommonUtility.Dynamic
{
    /// <summary>
    /// 动态实体类
    /// </summary>
    /// <summary>
    /// 动态实体类
    /// </summary>
    public class DynamicDataEntity : DynamicObject
    {
        /// <summary>
        /// 内置集合参数
        /// </summary>
        private readonly Dictionary<string, object> _objEntity = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary> 
        /// Provides the implementation of getting a member.  Derived classes can override
        /// this method to customize behavior.  When not overridden the call site requesting the 
        /// binder determines the behavior. 
        /// </summary>
        /// <param name="binder">The binder provided by the call site.</param> 
        /// <param name="result">The result of the get operation.</param>
        /// <returns>true if the operation is complete, false if the call site should determine behavior.</returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            //object objValue;
            //_objEntity.TryGetValue(binder.Name, out objValue);
            //result = objValue;
            _objEntity.TryGetValue(binder.Name, out result);
            return true;
        }

        /// <summary> 
        /// Provides the implementation of setting a member.  Derived classes can override
        /// this method to customize behavior.  When not overridden the call site requesting the
        /// binder determines the behavior.
        /// </summary> 
        /// <param name="binder">The binder provided by the call site.</param>
        /// <param name="value">The value to set.</param> 
        /// <returns>true if the operation is complete, false if the call site should determine behavior.</returns> 
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            //Dictionary<string, object>总是可以设置value，不管key存不存在
            _objEntity[binder.Name] = value;
            return true;
        }

        /// <summary>
        ///  返回所有动态成员的Name的列表
        /// </summary>
        /// <returns>动态成员名称的列表</returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _objEntity.Keys;
        }

        /// <summary>
        /// 通过动态类型的成员名称获取值
        /// </summary>
        /// <param name="name">成员的名称</param>
        /// <returns>该成员的值,key不存在时返回null</returns>
        public object this[string name]
        {
            get
            {
                object value;
                _objEntity.TryGetValue(name, out value);
                return value;
            }
            set
            {
                _objEntity[name] = value;
            }
        }
    }
}
