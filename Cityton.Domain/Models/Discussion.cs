﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Cityton.Data.Models
{
    class Discussion : BaseEntities
    {

        public DateTime CreatedAt { get; set; }

        /*****/

        public ICollection<UserInDiscussion> UserInDiscussion { get; set; }
        public ICollection<Message> Messages { get; set; }

    }
}
