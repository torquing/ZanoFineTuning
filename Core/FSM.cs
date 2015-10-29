using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ZanoFineTuning.Core
{
    // Simple Lambda Function base State Machine using strings to represent states and events
    public class FSM
    {

        class Transition
        {
            public String From, To;
            public Function Function;
        }

        class Event
        {
            public String State, Evt;
            public Function Function;
        }

        public delegate void Function(params object[] args);

        List<Transition> Transitions = new List<Transition>(4);
        List<Event> Events = new List<Event>(4);

        public String State { get; private set; }

        public FSM()
        {
            State = "NULL"; // Default state is 'NULL'
        }

        class Debug
        {
            public HashSet<String> Triggers = new HashSet<String>();
            public HashSet<String> Transitions = new HashSet<String>();
        }

        public void Print()
        {
            Dictionary<String, Debug> d = new Dictionary<String, Debug>();
            foreach (var t in Transitions)
            {
                Debug n = null;
                if (d.TryGetValue(t.From, out n) == false)
                {
                    n = new Debug();
                    d.Add(t.From, n);
                }
                n.Transitions.Add(String.Format("{0} -> 0x{1:X8}", t.To, t.Function.GetMethodInfo().GetHashCode()));
            }

            foreach (var e in Events)
            {
                Debug n = null;
                if (d.TryGetValue(e.State, out n) == false)
                {
                    n = new Debug();
                    d.Add(e.State, n);
                }
                n.Triggers.Add(String.Format("{0} -> 0x{1:X8}", e.Evt, e.Function.GetMethodInfo().GetHashCode()));

            }

            StringBuilder sb = new StringBuilder(2048);
            sb.AppendLine(GetType().Name);

            foreach (var np in d)
            {
                var n = np.Value;
                sb.AppendFormat(" - {0}", np.Key);
                sb.AppendLine();
                foreach (var t in n.Transitions)
                {
                    sb.AppendFormat("   > {0}", t);
                    sb.AppendLine();
                }
                foreach (var e in n.Triggers)
                {
                    sb.AppendFormat("   . {0}", e);
                    sb.AppendLine();
                }
            }

            Console.WriteLine(sb);

        }

        public void Move(String newState, params object[] args)
        {
            String oldState = State;
            State = newState;

            foreach (var transition in Transitions)
            {
                if (transition.From == oldState && transition.To == newState)
                {
                    transition.Function(args);
                }
            }

            foreach (var transition in Transitions)
            {
                if (transition.From == "ANY" && transition.To == newState)
                {
                    transition.Function(args);
                }
            }
        }

        public void Trigger(String evt, params object[] args)
        {
            String currentState = State;

            foreach (var e in Events)
            {
                if (e.State == currentState && e.Evt == evt)
                {
                    e.Function(args);
                }
            }

            foreach (var e in Events)
            {
                if (e.State == "ANY" && e.Evt == evt)
                {
                    e.Function(args);
                }
            }
        }

        public void OnMove(String from, String to, Function fn)
        {

            Transitions.Add(new Transition()
            {
                From = from,
                To = to,
                Function = fn
            });
        }

        public void OnMove(String from_to_str, Function fn)
        {
            foreach (var te in from_to_str.Split(','))
            {
                var tp = te.Split('>');
                var ta = tp[0].Split('|');
                var tb = tp[1].Split('|');

                foreach (var f in ta)
                {
                    foreach (var t in tb)
                    {
                        Transitions.Add(new Transition()
                        {
                            From = f.Trim(),
                            To = t.Trim(),
                            Function = fn
                        });
                    }
                }
            }
        }

        public void OnMove(String[] from, String to, Function fn)
        {
            foreach (String f in from)
            {
                Transitions.Add(new Transition()
                {
                    From = f,
                    To = to,
                    Function = fn
                });
            }
        }

        public void OnMove(String from, String[] to, Function fn)
        {
            foreach (String t in to)
            {
                Transitions.Add(new Transition()
                {
                    From = from,
                    To = t,
                    Function = fn
                });
            }
        }

        public void OnMove(String[] from, String[] to, Function fn)
        {
            foreach (String f in from)
            {
                foreach (String t in to)
                {
                    Transitions.Add(new Transition()
                    {
                        From = f,
                        To = t,
                        Function = fn
                    });
                }
            }
        }

        public void OnTrigger(String state, String @event, Function fn)
        {
            Events.Add(new Event()
            {
                State = state,
                Evt = @event,
                Function = fn
            });
        }


        public void OnTrigger(String[] states, String @event, Function fn)
        {
            foreach (var s in states)
            {
                Events.Add(new Event()
                {
                    State = s,
                    Evt = @event,
                    Function = fn
                });
            }
        }

        public void OnTrigger(String state, String[] events, Function fn)
        {
            foreach (var e in events)
            {
                Events.Add(new Event()
                {
                    State = state,
                    Evt = e,
                    Function = fn
                });
            }
        }

        public void OnTrigger(String[] states, String[] events, Function fn)
        {
            foreach (var s in states)
            {
                foreach (var e in events)
                {
                    Events.Add(new Event()
                    {
                        State = s,
                        Evt = e,
                        Function = fn
                    });
                }
            }
        }

        public void OnTrigger(String state_evt_str, Function fn)
        {
            foreach (var se in state_evt_str.Split(','))
            {
                var sp = se.Split('.');
                var sa = sp[0].Split('|');
                var sb = sp[1].Split('|');
                foreach (var s in sa)
                {
                    foreach (var e in sb)
                    {
                        Events.Add(new Event()
                        {
                            State = s.Trim(),
                            Evt = e.Trim(),
                            Function = fn
                        });
                    }
                }
            }
        }

    }
}