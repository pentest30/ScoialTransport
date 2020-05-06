using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace tacchograaph_reader.Core.Commands.DddFiles
{
    public class ProgressNotification :INotification
    {
        public int Value { get; set; }
    }
}
