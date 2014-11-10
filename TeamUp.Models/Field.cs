﻿namespace TeamUp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using TeamUp.Models.Base;

    public class Field : AuditInfo
    {
        private ICollection<Game> games;

        public Field()
        {
            this.games = new HashSet<Game>();
            this.Img = "Content\\Images\\Fields\\default.jpg";
        }

        [Required]
        [MinLength(5)]
        [MaxLength(50)]
        public string Name { get; set; }

        public int Id { get; set; }

        [Required]
        public string Phone { get; set; }

        [MaxLength(200)]
        public string Website { get; set; }

        [MaxLength(200)]
        public string Img { get; set; }

        public TimeSpan? OpenningHour { get; set; }

        public TimeSpan? ClosingHour { get; set; }

        [MaxLength(50)]
        public string MoreInfo { get; set; }

        [Required]
        public virtual Address Address { get; set; }

        public virtual ICollection<Game> Games { get; set; }
    }
}
