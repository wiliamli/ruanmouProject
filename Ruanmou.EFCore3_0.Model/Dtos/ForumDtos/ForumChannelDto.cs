﻿using Ruanmou04.EFCore.Model.Models.Forum;
using System;
using System.Collections.Generic;
using System.Linq;
using Ruanmou04.Core.Model.DtoHelper;

namespace Ruanmou04.EFCore.Model.Dtos.ForumDtos
{
    public class ForumChannelDto
    {
        public int Id { get; set; }

        /// <summary>
        /// 频道名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public int CreatedId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改人
        /// </summary>
        public int ModifiedId { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifiedDate { get; set; } = DateTime.Now;


    }

    public static class ForumChannelDtoExt
    {
        public static ForumChannelDto ToDto(this ForumChannel forumChannel)
        {
            ForumChannelDto dto = null;
            if (forumChannel != null)
            {
                dto = DataMapping<ForumChannel, ForumChannelDto>.Trans(forumChannel);
            }
            return dto;
        }

        public static IEnumerable<ForumChannelDto> ToDtos(this IEnumerable<ForumChannel> forumChannels)
        {
            IEnumerable<ForumChannelDto> dtos = null;
            if (forumChannels != null)
            {
                dtos = forumChannels.Select(m => DataMapping<ForumChannel, ForumChannelDto>.Trans(m));
            }
            return dtos;
        }

        public static ForumChannel ToEntity(this ForumChannelDto dto)
        {
            ForumChannel forumChannel = null;
            if (dto != null)
            {
                forumChannel = DataMapping<ForumChannelDto, ForumChannel>.Trans(dto);
            }
            return forumChannel;
        }
    }
}