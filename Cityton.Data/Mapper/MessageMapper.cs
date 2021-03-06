using Cityton.Data.Common;
using Cityton.Data.DTOs;
using Cityton.Data;
using Cityton.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Cityton.Data.Mapper
{
    public static class MessageMapper
    {

        public static MessageDTO ToDTO(this Message data)
        {
            if (data == null) return null;

            return new MessageDTO
            {
                Id = data.Id,
                Content = data.Content == null ? null : data.Content,
                Media = data.Media == null ? null : new MediaMinimal { Id = data.Media.Id, Url = data.Media.Location },
                Author = new UserMinimal { Id = data.AuthorId, Username = data.AuthorId == null ? "" : data.Author.Username },
                CreatedAt = data.CreatedAt,
                DiscussionId = data.DiscussionId,
            };
        }

        public static IEnumerable<MessageDTO> ToDTO(this IEnumerable<Message> data)
        {
            return data.Select(m => m.ToDTO()).ToList();
        }

    }
}