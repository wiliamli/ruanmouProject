﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RM04.DBEntity;
using Ruanmou.NetCore.Interface;
using Ruanmou04.Core.Model.DtoHelper;
using Ruanmou04.Core.Utility;
using Ruanmou04.Core.Utility.Extensions;
using Ruanmou04.EFCore.Model.DtoHelper;
using Ruanmou04.NetCore.Interface.SystemManager.Service;
using Ruanmou04.NetCore.Project.Models;
using Ruanmou04.NetCore.Project.Utility;

namespace Ruanmou.NetCore3_0.DemoProject.Controllers
{
    //[CustomAuthorize]
    [Route("api/[controller]/[action]"), ApiController]
    public class RoleController : ControllerBase
    {
        private readonly ICurrentUserInfo _currentUserInfo;
        private readonly ISysRoleService _sysRoleService;

        public RoleController(ICurrentUserInfo currentUserInfo, ISysRoleService sysRoleService)
        {
            _currentUserInfo = currentUserInfo;
            _sysRoleService = sysRoleService;
        }

        /// <summary>
        /// 获取编辑用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]

        public string GetEditRoleByID(int userId)
        {
            var user = _sysRoleService.Find<SysRole>(userId)?.MapTo<SysRole, SysRoleDto>();
            return JsonConvert.SerializeObject(new AjaxResult { success = true, data = user });

        }


        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]

        public string GetRoles(int page, int limit, string name)
        {
            var userData = _sysRoleService.
                Query<SysRole>(u => (!name.IsNullOrEmpty() && u.Text.Contains(name)) || name.IsNullOrEmpty())
                .Select(m => new SysRoleDto
                {
                    Id = m.Id,
                    Description = m.Description,
                    CreateTime = m.CreateTime,
                    Text = m.Text,

                    Status = m.Status,

                }).ToList();

            PagedResult<SysRoleDto> pagedResult = new PagedResult<SysRoleDto> { PageIndex = page, PageSize = limit, Rows = userData, Total = userData.Count };

            return JsonConvert.SerializeObject(pagedResult);


        }

        /// <summary>
        /// 新增或修改数据
        /// </summary>
        /// <param name="sysMenuDto"></param>
        /// <returns></returns>
        [HttpPost]
        public AjaxResult SaveData([FromBody]SysRoleDto sysMenuDto)
        {

            AjaxResult ajaxResult = new AjaxResult { success = false };
            if (sysMenuDto != null)
            {
                if (sysMenuDto.Id > 0)
                {
                    var model = _sysRoleService.Find<SysRole>(sysMenuDto.Id);
                    model.Id = sysMenuDto.Id;
                    model.Description = sysMenuDto.Description;
                    model.Status = sysMenuDto.Status;
                    model.LastModifyTime = DateTime.Now;
                    model.LastModifierId = _currentUserInfo.CurrentUser.Id;
                    _sysRoleService.Update<SysRole>(model);
                }
                else
                {
                    var model = sysMenuDto.MapTo<SysRoleDto, SysRole>();
                    model.CreateTime = DateTime.Now;
                    model.CreateId= _currentUserInfo.CurrentUser.Id;
                    _sysRoleService.Insert<SysRole>(model);
                }
                ajaxResult.msg = "保存成功";
                ajaxResult.success = true;
            }
            else
                ajaxResult.msg = "保存失败";
            return ajaxResult;
        }
    }
}