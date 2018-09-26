using Midi;
using vJoyInterfaceWrap;

namespace midi2controller
{
    enum PadColor
    {
        OFF = 12,
        DIM_RED = 13,
        MEDIUM_RED = 14,
        FULL_RED = 15,
        DIM_GREEN = 28,
        DIM_AMBER = 29,
        MEDIUM_ORANGE = 30, 
        FULL_ORANGE_RED = 31,
        MEDIUM_GREEN = 44,
        MEDIUM_YELLOW_GREEN = 45,
        MEDIUM_AMBER = 46,
        FULL_ORANGE = 47,
        FULL_GREEN = 60,
        FULL_YELLOW_GREEN = 61,
        FULL_YELLOW = 62,
        FULL_AMBER = 63
    }

    class Pad
    {
        private PadColor currentColor;
        private Channel midiChannel;
        private bool isPressed;

        public Pad(Channel channel)
        {
            midiChannel = channel;
        }

        public PadColor GetColor()
        {
            return currentColor;
        }

        public void SetColor(PadColor color)
        {
            currentColor = color;
        }

        public bool IsPadPressed()
        {
            return isPressed;
        }

        public void SetIsPressed(bool pressed)
        {
            isPressed = pressed;
        }
    }
}
