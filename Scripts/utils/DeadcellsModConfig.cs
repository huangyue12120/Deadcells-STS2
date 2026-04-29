using BaseLib.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deadcells.Scripts.utils;

[ConfigHoverTipsByDefault]
public sealed class DeadcellsModConfig : SimpleModConfig
{
    [ConfigSection("Cards")]
    public static bool MagnificentCardBg { get; set; } = true;
}
