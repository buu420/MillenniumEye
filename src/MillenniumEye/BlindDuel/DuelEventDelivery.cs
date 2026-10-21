namespace MillenniumEye;

// No guessed speech duration: automatic output queues until deliberate player
// input. The first public event interrupts stale focus; subsequent targets queue.
internal static class DuelEventDelivery
{
    private static bool _protected;
    private static int _eventFrame;

    public static bool BeginEvent(int frame)
    {
        bool interrupt = !_protected;
        _protected = true;
        _eventFrame = frame;
        return interrupt;
    }

    public static bool ShouldInterrupt(bool requested) => requested && !_protected;

    public static void ObservePlayerInput(bool deliberateInput, int frame)
    {
        if (deliberateInput && frame > _eventFrame) _protected = false;
    }

    public static void Reset() { _protected = false; _eventFrame = -1; }
}
