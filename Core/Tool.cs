using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZanoFineTuning.Tools.Accelerometer;

namespace ZanoFineTuning.Core
{
    public static class T
    {
        public class ToolDescription
        {
            public FSM fsm;
            public String title, description, category;
        }
        public static Dictionary<String, ToolDescription> Tools; 
        
        private static String name = String.Empty;
        private static FSM t = null;

        public static String Name
        {
            get { return name; }
        }

        public static void Set(String name)
        {
            Trigger("stopped");
            A.Trigger("leave_tool");
            Z.UnlistenAll();

            if (name == "NULL")
            {
                V.TopPanelBackButton = null;
                return;
            }

            ToolDescription desc;
            if (Tools.TryGetValue(name, out desc))
            {
                T.t = desc.fsm;
                T.name = name;
                V.TopPanelBackButton = new BackButton("back");
                A.Trigger("enter_tool");
                Trigger("started");
            }
        }

        public static void Trigger(String evt, params object[] args)
        {
            if (t != null)
            {
                t.Trigger(evt, args);
            }
        }
        
        public static void Add(FSM fsm, String name, String title, String description, String category)
        {
            if (Tools == null)
            {
                Tools = new Dictionary<string, ToolDescription>();
            }

            Tools[name] = new ToolDescription()
            {
                description = description,
                title = title,
                fsm = fsm,
                category = category
            };
        }

    }
}
