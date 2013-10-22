using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadeauThea
{
    class Trial
    {
        public static enum Behavior {NoCue, Cue};
        public static enum Action {Point, Push};

        private readonly Behavior behavior;
        private readonly Action action;

        Trial(Behavior b, Action a)
        {
            behavior = b;
            action = a;
        }

    }
}
