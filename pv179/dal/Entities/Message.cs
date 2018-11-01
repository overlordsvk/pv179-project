﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Game.Infrastructure;

namespace Game.DAL.Entity.Entities
{
    public class Message : IEntity
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Sender))]
        public int? SenderId { get; set; }
        public virtual Character Sender { get; set; }

        [ForeignKey(nameof(Receiver))]
        public int? ReceiverId { get; set; }
        public virtual Character Receiver { get; set; }

        [MaxLength(256)]
        public string Subject { get; set; }

        [MaxLength(2048)]
        public string Text { get; set; }

        public DateTime Timestamp { get; set; }

        [NotMapped]
        public string TableName { get; } = nameof(GameDbContext.Messages);
    }
}
