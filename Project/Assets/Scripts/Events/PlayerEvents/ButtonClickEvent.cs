using System;
using EventBus.Composite.Presentation.Events;

namespace Player
{
    public sealed class ButtonPressEvent : CompositePresentationEvent<EnumButton> { }
}



