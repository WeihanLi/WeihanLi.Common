﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WeihanLi.Common {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("WeihanLi.Common.Resource", typeof(Resource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 参数不合法.
        /// </summary>
        internal static string InvalidArgument {
            get {
                return ResourceManager.GetString("InvalidArgument", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 参数类型不合法，参数类型不能是{0}.
        /// </summary>
        internal static string InvalidArgumentType {
            get {
                return ResourceManager.GetString("InvalidArgumentType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 操作失败.
        /// </summary>
        internal static string InvokeFail {
            get {
                return ResourceManager.GetString("InvokeFail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to LogHelper 尚未初始化，请先初始化 LogHelper.
        /// </summary>
        internal static string LogHelperNotInitialized {
            get {
                return ResourceManager.GetString("LogHelperNotInitialized", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} must be lambda expression.
        /// </summary>
        internal static string propertyExpression_must_be_lambda_expression {
            get {
                return ResourceManager.GetString("propertyExpression_must_be_lambda_expression", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Task类型不能序列化.
        /// </summary>
        internal static string TaskCanNotBeSerialized {
            get {
                return ResourceManager.GetString("TaskCanNotBeSerialized", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 不支持的LogHelperLevel.
        /// </summary>
        internal static string UnSupportedLogHelperLevel {
            get {
                return ResourceManager.GetString("UnSupportedLogHelperLevel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to value must be positive.
        /// </summary>
        internal static string ValueMustBePositive {
            get {
                return ResourceManager.GetString("ValueMustBePositive", resourceCulture);
            }
        }
    }
}
