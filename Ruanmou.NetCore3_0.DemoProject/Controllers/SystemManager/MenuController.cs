﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Ruanmou04.Core.Dtos.DtoHelper;
using Ruanmou04.Core.Utility;
using Ruanmou04.Core.Utility.DtoUtilities;
using Ruanmou04.Core.Utility.Extensions;
using Ruanmou04.EFCore.Dtos.DtoHelper;
using Ruanmou04.EFCore.Model.Models.SystemManager;
using Ruanmou04.NetCore.AOP.Filter;
using Ruanmou04.NetCore.Dtos.SystemManager.MenuDtos;
using Ruanmou04.NetCore.Interface;
using Ruanmou04.NetCore.Interface.SystemManager.Service;

namespace Ruanmou.NetCore3_0.DemoProject.Controllers
{
    [CustomAuthorize]
    [Route("api/[controller]/[action]"), ApiController]
    public class MenuController : ControllerBase
    {
        private readonly ICurrentUserInfo _currentUserInfo;
        private readonly IUserMenuService _userMenuService;
        private readonly ISysRoleMenuMappingService _roleMenuMappingService;

        public MenuController(ICurrentUserInfo currentUserInfo, IUserMenuService userMenuService, ISysRoleMenuMappingService roleMenuMappingService)
        {
            _currentUserInfo = currentUserInfo;
            _userMenuService = userMenuService;
            _roleMenuMappingService = roleMenuMappingService;
        }
        /// <summary>
        /// 获取权限菜单 
        /// </summary>
        /// <param name="menuType">1后台的，2是网站，3是论坛</param>
        /// <returns></returns>
        [HttpPost]
        public AjaxResult GetMenuList()
        {
            var menu = _userMenuService.GetAuthorityMenuList(_currentUserInfo.CurrentUser.Id);
            return new AjaxResult { Success = true, data = menu };
        }

        /// <summary>
        /// 获取编辑用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]

        public string GetEditMenuByID(int userId)
        {
            var result = new AjaxResult();
            result.ExecuteAction(DataOperateType.Query, () => result.data = _userMenuService.Find<SysMenu>(userId)?.MapTo<SysMenu, SysMenuDto>());
            return JsonConvert.SerializeObject(result);

        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]

        public AjaxResult RemoveMenuById(int id)
        {
            if (_roleMenuMappingService.Exists<SysRoleMenuMapping>(rm => rm.SysMenuId == id))
            {
                return AjaxResult.Failure("菜单数据已存在角色授权，请先移除授权再删除");// { success = false, msg = "菜单数据已存在角色授权，请先移除授权再删除" };
            }
            _userMenuService.Delete<SysMenu>(id);
            return AjaxResult.SuccessResult( "删除成功" );

        }
        /// <summary>
        /// 获取所有菜单数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]

        public string GetRoleMenu()
        {
            var menuData = _userMenuService.
                 Query<SysMenu>(u => (u.Status))
                 .Select(m => new SysMenuDto
                 {
                     Id = m.Id,
                     Text = m.Text
                 });

            return JsonConvert.SerializeObject(new AjaxResult { data = menuData, Success = true });
        }
        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]

        public string GetMenus(int page, int limit, string name)
        {
            var userData = _userMenuService.
                Query<SysMenu>(u => u.Status && (!name.IsNullOrEmpty() && u.Text.Contains(name)) || name.IsNullOrEmpty())
                .Select(m => new SysMenuDto
                {
                    Id = m.Id,
                    Description = m.Description,
                    MenuIcon = m.MenuIcon,
                    Text = m.Text,
                    MenuLevel = m.MenuLevel,
                    MenuType = m.MenuType,
                    Sort = m.Sort,
                    ParentId = m.ParentId,
                    Status = m.Status,
                    Url = m.Url
                }).ToList();

            PagedResult<SysMenuDto> pagedResult = new PagedResult<SysMenuDto> { PageIndex = page, PageSize = limit, Rows = userData, Total = userData.Count };

            return JsonConvert.SerializeObject(pagedResult);


        }

        /// <summary>
        /// 新增或修改数据
        /// </summary>
        /// <param name="sysMenuDto"></param>
        /// <returns></returns>
        [HttpPost]
        public AjaxResult SaveData([FromBody]SysMenuDto sysMenuDto)
        {

            AjaxResult ajaxResult = new AjaxResult { Success = false };
            if (sysMenuDto != null)
            {
                if (sysMenuDto.Id > 0)
                {
                    var menu = _userMenuService.Find<SysMenu>(sysMenuDto.Id);
                    menu.Id = sysMenuDto.Id;
                    menu.Description = sysMenuDto.Description;
                    menu.MenuIcon = sysMenuDto.MenuIcon;
                    menu.Text = sysMenuDto.Text;
                    menu.MenuLevel = sysMenuDto.MenuLevel;
                    //menu.MenuType = sysMenuDto.MenuType;
                    menu.Sort = sysMenuDto.Sort;
                    //menu.ParentId = sysMenuDto.ParentId;
                    menu.Status = sysMenuDto.Status;
                    menu.Url = sysMenuDto.Url;
                    menu.LastModifyTime = DateTime.Now;
                    menu.LastModifierId = _currentUserInfo.CurrentUser.Id;
                    ajaxResult.ExecuteAction(DataOperateType.Save, () => _userMenuService.Update<SysMenu>(menu));
                }
                else
                {
                    var menu = sysMenuDto.MapTo<SysMenuDto, SysMenu>();
                    menu.CreateTime = DateTime.Now;
                    menu.CreatorId = _currentUserInfo.CurrentUser.Id;
                    menu.ParentId = 0;
                    ajaxResult.ExecuteAction(DataOperateType.Save, () => _userMenuService.Update<SysMenu>(menu));
                }
            }
            return ajaxResult;
        }
    }
}
