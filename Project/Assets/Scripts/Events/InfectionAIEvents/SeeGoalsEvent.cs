using System;
using System.Collections.Generic;
using EventBus.Composite.Presentation.Events;
using UnityEngine;


namespace InfectedAI {
    public sealed class SeeAimsEvent : CompositePresentationEvent<List<GameObject>> { }
}

