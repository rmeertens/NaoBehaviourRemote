using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NaoRemote
{
    class TrialSequence : List<BehaviorSequence>
    {
        private TrialSequence() : base() { }

        static private void Shuffle(TrialSequence seq) 
        {

        }

        public void Shuffle()
        {
            int n = this.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                BehaviorSequence value = this[k];
                this[k] = this[n];
                this[n] = value;
            }
        }

        static public TrialSequence CreatePredictiveTrialSequence()
        {
            TrialSequence seq = new TrialSequence();
            for(int i = 0; i < 2; i +=1)
            {
                seq.Add(BehaviorSequence.PushLeftCueSequence());
                seq.Add(BehaviorSequence.PushRightCueSequence());
            }
            for (int i = 0; i < 5; i += 1)
            {
                seq.Add(BehaviorSequence.PointLeftNoCueSequence());
                seq.Add(BehaviorSequence.PointRightNoCueSequence());
            }
            seq.Shuffle();
            return seq;
        }

        public static class ThreadSafeRandom
        {
            [ThreadStatic]
            private static Random Local;

            public static Random ThisThreadsRandom
            {
                get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
            }
        }

        static class MyExtensions
        {
            
        }
    }
}
