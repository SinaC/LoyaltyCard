﻿using System;

namespace LoyaltyCard.Services.Popup
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class PopupAssociatedViewAttribute : Attribute
    {
        public Type ViewType { get; private set; }

        public PopupAssociatedViewAttribute(Type viewType)
        {
            ViewType = viewType;
        }
    }
}
