﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.GauntletUI;
using TaleWorlds.Library;

namespace TournamentsXPanded.Settings.GUI.Views
{
    public class HoverRichTextWidget : RichTextWidget
    {
        [DataSourceProperty]
        public Action HoverBegin { get; set; } = null;
        [DataSourceProperty]
        public Action HoverEnd { get; set; } = null;

        public HoverRichTextWidget(UIContext context) : base(context)
        {
        }

        protected override void OnHoverBegin()
        {
            base.OnHoverBegin();
            HoverBegin?.Invoke();
        }

        protected override void OnHoverEnd()
        {
            base.OnHoverEnd();
            HoverEnd?.Invoke();
        }
    }
}
