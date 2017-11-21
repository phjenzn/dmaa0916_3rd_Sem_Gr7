﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Models
{
    public class Row
    {
        public int Id { get; set; }
        public List<Seat> Seats { get; set; }
        public int SeatNumber { get; set; }
        public double Price { get; set; }

    }
}
