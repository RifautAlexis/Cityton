using System;
using System.Collections.Generic;
using System.Text;

namespace Cityton.Data.DTOs
{
    public class GroupDTO
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ParticipantGroupDTO> Members { get; set; }
        public bool HasRequested { get; set; }

    }
}