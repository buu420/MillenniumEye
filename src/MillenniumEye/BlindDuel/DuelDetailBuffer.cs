// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System.Collections.Generic;

namespace MillenniumEye;

public sealed class DuelDetailBuffer
{
    public List<string> Lines { get; private set; }
    public int Index { get; set; }
    public void Replace(List<string> lines)
    {
        Lines = lines == null ? null : new List<string>(lines);
        Index = 0;
    }
}
