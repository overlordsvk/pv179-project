﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Game.DAL.Entities;
using Game.Infrastructure;

namespace Game.DAL.Entity.Entities
{
    public class Chat : IEntity
    {
        public int Id { get; set; }

        public virtual ICollection<Character> Chatters { get; set; }

        [MaxLength(256)]
        public string Subject { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        public DateTime? LastMessageTimestamp { get; set; }

        [NotMapped]
        public string TableName { get; } = nameof(GameDbContext.Chat);
    }
}
