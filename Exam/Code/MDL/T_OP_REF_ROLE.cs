//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace MDL
{
    using System;
    using System.Collections.Generic;
    
    public partial class T_OP_REF_ROLE
    {
        public System.Guid ID { get; set; }
        public Nullable<System.Guid> ROLE_ID { get; set; }
        public Nullable<System.Guid> OP_ID { get; set; }
    
        public virtual T_OPERATION T_OPERATION { get; set; }
        public virtual T_ROLE T_ROLE { get; set; }
    }
}