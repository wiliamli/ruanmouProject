﻿

namespace Ruanmou04.EFCore.Dtos.DtoHelper
{
    /// <summary>
    /// ajax返回结果
    /// </summary>
    public class AjaxResult
    {
        public AjaxResult(string errorMsg)
        {
            //this.success = false;
            msg = errorMsg;
        }

        public AjaxResult(string successMsg, object result)
        {
            //this.success = true;
            msg = successMsg;
            data = result;
        }
        public AjaxResult(object result)
        {
            data = result;
        }
        public AjaxResult(string errorMsg, bool isSuccess)
        {
            this.success = isSuccess;
            this.msg = errorMsg;
        }
        public AjaxResult()
        {   
        }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public object data { get; set; }

        /// <summary>
        /// 返回消息
        /// </summary>
        public string msg { get; set; }
         
        public static AjaxResult Success(string msg,object data=null)
        {
            return new AjaxResult()
            {
                success = true,  
                msg = msg,
                data=data
            };
        }

        public static AjaxResult Failure(string msg)
        {
            return new AjaxResult()
            {
                success = false,   
                msg = msg,
            };
        }
    }
}
