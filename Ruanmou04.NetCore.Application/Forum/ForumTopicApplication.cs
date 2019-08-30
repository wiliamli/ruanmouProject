﻿using Ruanmou04.Core.Utility;
using Ruanmou04.EFCore.Model.Dtos.ForumDtos;
using Ruanmou04.EFCore.Model.Models.Forum;
using Ruanmou04.NetCore.Interface.Forum.Applications;
using Ruanmou04.NetCore.Interface.Forum.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ruanmou04.NetCore.Application.Forum
{
    public class ForumTopicApplication : IForumTopicApplication
    {
        private IForumTopicService forumTopicService;

        public ForumTopicApplication(IForumTopicService forumTopicService)
        {
            this.forumTopicService = forumTopicService;
        }

        public int AddTopic(ForumTopicDto forumTopicDto)
        {
            var forumTopic = forumTopicService.Insert(forumTopicDto.ToEntity());
            return forumTopic.Id;
        }

        public ForumTopicDto GetForumTopic(int topicId)
        {
           return forumTopicService.Find<ForumTopic>(topicId).ToDto();
        }

        public PagedResult<ForumTopicDto> GetPagedResult(int channelId, PagingInput pagingInput)
        {
            PagedResult<ForumTopic> pagedResult = forumTopicService.QueryPage<ForumTopic, DateTime>
                (m => m.ChannelId == channelId, pagingInput.PageIndex,
                pagingInput.PageSize, n => n.ModifiedDate, false);

            return pagedResult.ToPaged();

        }

        public void UpdateTopic(ForumTopicDto forumTopicDto)
        {
            forumTopicService.Update<ForumTopic>(forumTopicDto.ToEntity());
        }
    }
}
