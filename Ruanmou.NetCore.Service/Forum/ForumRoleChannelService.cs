﻿using Microsoft.EntityFrameworkCore;
using Ruanmou.NetCore.Service;
using Ruanmou04.NetCore.Interface.Forum.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ruanmou04.NetCore.Service.Forum
{
    public class ForumRoleChannelService : BaseService, IForumRoleChannelService
    {
        public ForumRoleChannelService(DbContext context) : base(context)
        {
        }
    }
}