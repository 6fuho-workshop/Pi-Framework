using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiFramework
{
    public partial class PiBase
    {
        public static GameBase game { get;internal set; }

        public static PiSystemEvents systemEvents { get; private set; }

        public static PiServiceLocator services { get; private set; }

        public static PiConsole console { get; private set; }

    }
}