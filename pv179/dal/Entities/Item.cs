﻿using DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class Item : IEntity
    {
        public int Id { get; set; }

        [MaxLength(256)]
        public string Name { get; set; }

        [Range(0,500)]
        public int Attack { get; set; }

        [Range(0, 500)]
        public int Defense { get; set; }

        [Range(0,100)]
        public int Weight { get; set; }

        [Range(0,int.MaxValue)]
        public int Price { get; set; }

        public bool Equipped { get; set; }

        [ForeignKey(nameof(Owner))]
        public int? OwnerId { get; set; }
        public virtual Character Owner { get; set; }

        [ForeignKey(nameof(ShopOwner))]
        public int? ShopOwnerId { get; set; }
        public virtual Character ShopOwner { get; set; }

        public int WeaponTypeId { get; set; }
        public virtual WeaponType WeaponType { get; set; }
    }
}
