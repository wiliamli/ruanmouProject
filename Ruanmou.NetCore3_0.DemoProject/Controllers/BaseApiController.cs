﻿using System;
using Microsoft.AspNetCore.Mvc;
using Ruanmou04.Core.Utility.MvcResult;

namespace Ruanmou04.NetCore.Project.Controllers
{
    /// <summary>
    /// WebApi基类
    /// </summary>
    public class BaseApiController : ControllerBase
    {
        /// <summary>
        /// 标准数据返回
        /// </summary>
        /// <param name="action">action</param>
        /// <returns></returns>
        protected StandardJsonResult StandardAction(Action action)
        {
            var result = new StandardJsonResult();
            result.StandardAction(action);
            return result;
        }

        /// <summary>
        /// 标准数据返回
        /// </summary>
        /// <typeparam name="T">返回参数</typeparam>
        /// <param name="func">func</param>
        /// <returns></returns>
        protected StandardJsonResult<T> StandardAction<T>(Func<T> func)
        {
            var result = new StandardJsonResult<T>();
            result.StandardAction(() =>
            {
                result.Data = func();
            });
            return result;
        }
    }
}
